using FlowDash.Contract.Common.Pagination;

namespace FlowDash.Contract.User.GetList
{
    public class GetUsersRequest : PaginationRequest
    {
        public string? Search { get; init; }
        public bool? IsActive { get; init; }
    }
}
