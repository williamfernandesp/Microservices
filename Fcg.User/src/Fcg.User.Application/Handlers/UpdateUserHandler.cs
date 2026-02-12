using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain;
using Fcg.User.Proxy.Auth.Client.Interface;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, Response>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientAuth _clientAuth;

        public UpdateUserHandler(IUserRepository userRepository, IClientAuth clientAuth)
        {
            _userRepository = userRepository;
            _clientAuth = clientAuth;
        }

        public async Task<Response> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                response.AddError("Usuário não encontrado.");
                return response;
            }

            user.Update(request.Name);

            await _userRepository.SaveAsync(user);

            var externalResponse = await _clientAuth.UpdateAuthUserAsync(user.Id, request.Email);

            if (externalResponse.HasErrors)
            {
                response.AddError("Erro ao criar usuário externo.");
            }

            return response;
        }
    }
}
