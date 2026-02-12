using Fcg.User.Common;
using Fcg.User.Proxy.Auth.Client.Responses;

namespace Fcg.User.Proxy.Auth.Client.Interface
{
    public interface IClientAuth
    {
        Task<Response<AuthUserResponse>> DeleteAuthUserAsync(Guid id);
        Task<Response<GetUserAuthResponse>> GetAuthUserAsync(Guid id);
        Task<Response<AuthUserResponse>> UpdateAuthUserAsync(Guid id, string email);
    }
}
