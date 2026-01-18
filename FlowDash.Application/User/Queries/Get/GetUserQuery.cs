using FlowDash.Application.User.Queries.Common;
using MediatR;

namespace FlowDash.Application.User.Queries.Get
{
    public record GetUserQuery(int Id) : IRequest<GetUserResult>;
}
