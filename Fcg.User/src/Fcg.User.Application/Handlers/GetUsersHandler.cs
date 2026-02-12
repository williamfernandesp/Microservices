using Fcg.User.Application.Requests;
using Fcg.User.Common;
using Fcg.User.Domain.Queries;
using Fcg.User.Domain.Queries.Responses;
using MediatR;

namespace Fcg.User.Application.Handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersRequest, Response<PagedResponse<GetUsersResponse>>>
    {
        private readonly IUserQuery _userQuery;

        public GetUsersHandler(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }

        public async Task<Response<PagedResponse<GetUsersResponse>>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            var s = request.Skip ?? 0;
            var t = request.Take ?? 20;

            var response = new Response<PagedResponse<GetUsersResponse>>
            {
                Result = await _userQuery.GetUsersAsync(s, t)
            };

            return response;
        }
    }
}
