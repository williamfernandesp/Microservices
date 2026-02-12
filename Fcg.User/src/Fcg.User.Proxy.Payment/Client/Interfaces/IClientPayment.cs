using Fcg.User.Common;

namespace Fcg.User.Proxy.Payment.Client.Interfaces
{
    public interface IClientPayment
    {
        Task<Response> ProcessPaymentAsync(Guid userId, Guid gameId);
    }
}
