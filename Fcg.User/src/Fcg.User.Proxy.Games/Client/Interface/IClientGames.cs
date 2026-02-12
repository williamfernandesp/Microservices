using Fcg.User.Common;
using Fcg.User.Proxy.Games.Client.Response;

namespace Fcg.User.Proxy.Games.Client.Interface
{
    public interface IClientGames
    {
        Task<Response<ExternalGamesResponse>> GetGameAsync(Guid id);
        Task<Response<IEnumerable<ExternalGamesResponse>>> GetGamesAsync(IEnumerable<Guid> ids);
    }
}
