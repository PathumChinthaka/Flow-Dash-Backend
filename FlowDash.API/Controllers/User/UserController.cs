using FlowDash.Application.User.Queries.Get;
using FlowDash.Application.User.Queries.GetList;
using FlowDash.Contract.Common.Pagination;
using FlowDash.Contract.User.Get;
using FlowDash.Contract.User.GetList;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FlowDash.API.Controllers.User
{
    [SwaggerTag("User")]
    [Route("users")]
    public class UserController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly ISender _mediator;

        public UserController
        (
            IMapper mapper,
            ISender mediator
        )
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user details by userId")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var query = new GetUserQuery(id); 
            var user = await _mediator.Send(query, cancellationToken);

            return Ok(user);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of users")]
        [ProducesResponseType(typeof(PaginatedResponse<GetUserResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<GetUserResponse>>> GetUsers([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
        {
            var query = _mapper.Map<GetUsersQuery>(request);
            var result = await _mediator.Send(query, cancellationToken);

            var response = new PaginatedResponse<GetUserResponse>
            {
                Items = _mapper.Map<IEnumerable<GetUserResponse>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };

            return Ok(response);
        }
    }
}
