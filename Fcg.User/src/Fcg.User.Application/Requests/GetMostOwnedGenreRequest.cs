using Fcg.User.Common;
using MediatR;

namespace Fcg.User.Application.Requests
{
    public class GetMostOwnedGenreRequest : IRequest<Response<int?>>
    {
        public Guid UserId { get; set; }
    }
}
