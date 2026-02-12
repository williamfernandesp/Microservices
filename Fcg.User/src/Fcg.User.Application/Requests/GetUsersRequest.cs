using Fcg.User.Common;
using Fcg.User.Domain.Queries.Responses;
using MediatR;

namespace Fcg.User.Application.Requests
{
    public class GetUsersRequest : IRequest<Response<PagedResponse<GetUsersResponse>>>
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
