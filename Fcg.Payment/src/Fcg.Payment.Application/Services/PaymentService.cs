using Fcg.Payment.Application.Responses;
using Fcg.Payment.Common.Responses;
using Fcg.Payment.Domain.Entities;
using Fcg.Payment.Domain.Enums;
using Fcg.Payment.Domain.Repositories;
using Fcg.Payment.Proxy.Auth.Client.Interfaces;
using Fcg.Payment.Proxy.Games.Client.Interfaces;
using Fcg.Payment.Proxy.User.Client.Interfaces;
using IPublishEndpoint = MassTransit.IPublishEndpoint;
using Fcg.Contracts.Payments;
using Fcg.Contracts.Games;

namespace Fcg.Payment.Application.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClientAuth _clientAuth;
        private readonly IClientUser _clientUser;
        private readonly IClientGames _clientGame;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IClientAuth clientAuth,
            IClientUser clientUser,
            IClientGames clientGame,
            IPublishEndpoint publishEndpoint)
        {
            _paymentRepository = paymentRepository;
            _clientAuth = clientAuth;
            _clientUser = clientUser;
            _clientGame = clientGame;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Response<CheckoutResponse>> CheckoutAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
        {
            var response = new Response<CheckoutResponse>();

            var authCheck = await _clientAuth.GetAuthUserAsync(userId);

            if (authCheck.Result == null || authCheck.Erros.Any())
            {
                response.AddError("Usuário inexistente no serviço de autenticação.");
                return response;
            }

            var payment = new PaymentTransaction(userId, amount);

            await _paymentRepository.AddAsync(payment, cancellationToken);

            response.Result = new CheckoutResponse
            {
                PaymentId = payment.Id,
                Status = payment.Status.ToString(),
                CreatedAt = payment.CreatedAt
            };

            return response;
        }

        public async Task<Response<bool>> ApproveAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var response = new Response<bool>();

            var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

            if (payment is null || payment.Status != PaymentStatus.Pending)
            {
                response.AddError("Pagamento não encontrado ou já processado.");
                return response;
            }

            if (payment.GameId.HasValue && payment.GameId != Guid.Empty)
            {
                await _clientUser.SubtractBalanceAsync(payment.UserId, payment.Amount);
            }
            else
            {
                await _clientUser.AddBalanceAsync(payment.UserId, payment.Amount);
            }

            payment.Approve();

            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            await _publishEndpoint.Publish(new PaymentApprovedEvent(
                payment.Id,
                payment.UserId,
                payment.Amount,
                payment.GameId,
                payment.CompletedAt ?? DateTime.UtcNow), cancellationToken);

            response.Result = true;
            return response;
        }

        public async Task<Response<bool>> RejectAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var response = new Response<bool>();

            var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

            if (payment is null || payment.Status != PaymentStatus.Pending)
            {
                response.AddError("Pagamento não encontrado ou já processado.");
                return response;
            }

            payment.Reject();
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            await _publishEndpoint.Publish(new PaymentRejectedEvent(
                payment.Id,
                payment.UserId,
                payment.Amount,
                payment.GameId,
                payment.CompletedAt ?? DateTime.UtcNow,
                "Rejected"), cancellationToken);

            response.Result = true;
            return response;
        }

        public async Task<Response<PaymentResponse>> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var response = new Response<PaymentResponse>();

            var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

            if (payment is null)
            {
                response.AddError("Pagamento não encontrado.");
                return response;
            }

            response.Result = PaymentResponse.FromEntity(payment);
            return response;
        }

        public async Task<Response<CheckoutResponse>> PurchaseGameAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default)
        {
            var response = new Response<CheckoutResponse>();

            var userCheck = await _clientUser.GetUserAsync(userId);

            if (userCheck.HasErrors)
            {
                response.AddError("Usuário não encontrado no serviço Fcg.User.");
                return response;
            }

            // Opção B (assíncrona): cria pagamento pendente e solicita validação do preço/existência ao serviço de games.
            // O preço será atualizado quando chegar o evento GamePurchaseValidatedEvent.
            var payment = new PaymentTransaction(userId, 0m, gameId);

            await _paymentRepository.AddAsync(payment, cancellationToken);

            await _publishEndpoint.Publish(new GamePurchaseRequestedEvent(
                payment.Id,
                userId,
                gameId,
                DateTime.UtcNow), cancellationToken);

            response.Result = new CheckoutResponse
            {
                PaymentId = payment.Id,
                Status = payment.Status.ToString(),
                CreatedAt = payment.CreatedAt
            };

            return response;
        }
    }
}