using MediatR;

namespace FlowDash.Application.Common.Pagination
{
    public abstract class PaginationQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
