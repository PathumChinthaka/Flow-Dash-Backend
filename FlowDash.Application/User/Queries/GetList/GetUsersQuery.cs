using FlowDash.Application.Common.Pagination;
using FlowDash.Application.User.Queries.Common;
using MediatR;

namespace FlowDash.Application.User.Queries.GetList
{
    public class GetUsersQuery : PaginationQuery, IRequest<PaginatedResult<GetUserResult>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
