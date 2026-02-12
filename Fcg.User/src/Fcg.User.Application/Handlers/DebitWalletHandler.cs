using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class DebitWalletHandler : IRequestHandler<DebitWalletRequest, Response>
    {
        private readonly IUserRepository _userRepository;

        public DebitWalletHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Response> Handle(DebitWalletRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                response.AddError("Usuário não encontrado.");
                return response;
            }

            user.RemoveFunds(request.Debit);

            await _userRepository.UpdateWalletAsync(user);

            return response;
        }
    }
}
