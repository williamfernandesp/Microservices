using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain.Queries;
using Fcg.User.Proxy.Games.Client.Interface;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class GetMostOwnedGenreHandler : IRequestHandler<GetMostOwnedGenreRequest, Response<int?>>
    {
        private readonly IUserQuery _userQuery;
        private readonly IClientGames _clientGame;

        public GetMostOwnedGenreHandler(IUserQuery userQuery, IClientGames clientGame)
        {
            _userQuery = userQuery;
            _clientGame = clientGame;
        }

        public async Task<Response<int?>> Handle(GetMostOwnedGenreRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<int?>();

            var user = await _userQuery.GetUserByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                response.AddError($"Usuário {request.UserId} não encontrado!");
                return response;
            }

            var gameIds = await _userQuery.GetGamesByUserIdAsync(user.Id, cancellationToken);

            if (gameIds is null || !gameIds.Any())
            {
                response.Result = null;
                return response;
            }

            var genres = new List<int>();

            foreach (var gameId in gameIds)
            {
                var gameResponse = await _clientGame.GetGameAsync(gameId);

                if (gameResponse.HasErrors || gameResponse.Result is null)
                {
                    response.AddError($"Failed to retrieve game {gameId}.");
                    continue;
                }

                genres.Add(gameResponse.Result.Genre);
            }

            if (genres.Count == 0)
            {
                response.Result = null;
                return response;
            }

            response.Result = genres
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();

            return response;
        }
    }
}
