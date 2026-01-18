using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Common.Pagination;
using FlowDash.Application.User.Queries.Common;
using MapsterMapper;
using MediatR;

namespace FlowDash.Application.User.Queries.GetList
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<GetUserResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<PaginatedResult<GetUserResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetList(request, cancellationToken);

            var users = result.Items
                .Select(u => _mapper.Map<GetUserResult>(u))
                .ToList();

            return new PaginatedResult<GetUserResult>
            {
                Items = users,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
