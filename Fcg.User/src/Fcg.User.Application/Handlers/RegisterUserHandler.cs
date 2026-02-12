using Fcg.User.Application.Requests;
using Fcg.User.Application.Responses;
using Fcg.User.Domain;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, CreateUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CreateUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var random = new Random();
                var name = $"User{random.Next()}";

                var user = new Domain.User(request.Id, name);

                await _userRepository.SaveAsync(user);

                return new CreateUserResponse
                {
                    Success = true,
                    Message = $"Usuário {name} criado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = $"Erro ao criar usuário: {ex.Message}"
                };
            }
        }
    }
}
