using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Exceptions.Client;
using FlowDash.Application.User.Queries.Common;
using MapsterMapper;
using MediatR;

namespace FlowDash.Application.User.Queries.Get
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(request.Id, cancellationToken) ?? throw new NotFoundException($"User with id {request.Id} not found");

            return _mapper.Map<GetUserResult>(user);
        }
    }
}
