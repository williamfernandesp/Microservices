using Fcg.User.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace Fcg.User.Application.Requests
{
    public class CreditWalletRequest : IRequest<Response>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public decimal Credit { get; set; }
    }
}
