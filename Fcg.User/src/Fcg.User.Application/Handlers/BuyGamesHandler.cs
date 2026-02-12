using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain;
using Fcg.User.Proxy.Games.Client.Interface;
using Fcg.User.Proxy.Payment.Client.Interfaces;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class BuyGamesHandler : IRequestHandler<BuyGamesRequest, Response>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientGames _clientGame;
        private readonly IClientPayment _clientPayment;

        public BuyGamesHandler(IClientGames clientGame, IUserRepository userRepository, IClientPayment clientPayment)
        {
            _clientGame = clientGame;
            _userRepository = userRepository;
            _clientPayment = clientPayment;
        }

        public async Task<Response> Handle(BuyGamesRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                response.AddError("Usuário não encontrado.");
                return response;
            }

            if (request.GamesId != null && request.GamesId.Any())
            {
                var externalGameResponse = await _clientGame.GetGamesAsync(request.GamesId);

                if (externalGameResponse.HasErrors || externalGameResponse.Result == null)
                {
                    response.AddError("Erro ao obter informações dos jogos externos.");
                    return response;
                }

                foreach (var gameData in externalGameResponse.Result)
                {
                    var paymentResult = await _clientPayment.ProcessPaymentAsync(user.Id, gameData.Id);
                    
                    if (paymentResult.HasErrors)
                    {
                        response.AddError($"Pagamento recusado para o jogo {gameData.Id}: {paymentResult.Erros.FirstOrDefault()}");
                        return response;
                    }

                    user.AddGameToLibrary(gameData.Id);
                }

                try
                {
                    await _userRepository.BuyGameAsync(user);
                }
                catch (Exception ex)
                {
                    response.AddError($"Erro ao atualizar biblioteca local: {ex.Message}");
                }
            }

            return response;
        }
    }
}
