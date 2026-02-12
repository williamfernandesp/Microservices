using Fcg.User.Common;
using MediatR;

namespace Fcg.User.Application.Requests
{
    public class DeleteUserRequest : IRequest<Response>
    {
        public Guid Id { get; set; }
    }
}
