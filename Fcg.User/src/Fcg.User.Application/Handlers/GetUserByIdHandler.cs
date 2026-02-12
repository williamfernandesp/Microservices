using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain.Queries;
using Fcg.User.Proxy.Auth.Client.Interface;
using Fcg.User.Proxy.Games.Client.Interface;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdRequest, Response>
    {
        private readonly IUserQuery _userQuery;
        private readonly IClientAuth _clientAuth;
        private readonly IClientGames _clientGame;

        public GetUserByIdHandler(IUserQuery userQuery, IClientAuth clientAuth, IClientGames clientGame)
        {
            _userQuery = userQuery;
            _clientAuth = clientAuth;
            _clientGame = clientGame;
        }

        public async Task<Response> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userQuery.GetUserByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                response.AddError($"Usuário {request.Id} não encontrado!");
                return response;
            }

            var externalResponse = await _clientAuth.GetAuthUserAsync(user.Id);

            if (externalResponse.HasErrors || string.IsNullOrEmpty(externalResponse.Result.Email))
            {
                response.AddError("Erro ao buscar usuário externo.");
                return response;
            }

            user.Email = externalResponse.Result.Email;

            var gameIds = await _userQuery.GetGamesByUserIdAsync(user.Id, cancellationToken) ?? [];

            if (gameIds != null && gameIds.Any())
            {
                var externalGamesResponse = await _clientGame.GetGamesAsync(gameIds);

                if (externalGamesResponse.HasErrors)
                {
                    response.AddError("Erro ao obter detalhes dos jogos da biblioteca.");
                }
                else if (externalGamesResponse.Result != null)
                {
                    foreach (var g in externalGamesResponse.Result)
                    {
                        user.Library?.Add(new Domain.Queries.Responses.GameResponse
                        {
                            Id = g.Id,
                            Title = g.Title,
                            Description = g.Description,
                            Price = g.Price,
                            Genre = g.Genre
                        });
                    }
                }
            }

            response.SetResult(user);

            return response;
        }
    }
}
