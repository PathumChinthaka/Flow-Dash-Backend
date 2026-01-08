using FlowDash.Application.Authentication.Commands.Login;
using FlowDash.Application.Authentication.Commands.RefreshToken;
using FlowDash.Application.Authentication.Commands.Register;
using FlowDash.Contract.Authentication.Request;
using FlowDash.Contract.Authentication.Response;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FlowDash.API.Controllers.Authentication
{
    [ApiController]
    [SwaggerTag("Authentication")]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISender _mediator;
        private readonly ILogger<AuthenticationController> _logger; 

        public AuthenticationController
        (
            IMapper mapper,
            ISender mediator,
            ILogger<AuthenticationController> logger
        )  
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;  
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register new user")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> LoginAsync(RegisterRequest registerRequest)
        {
            _logger.LogInformation("User registration attempt");

            var command = _mapper.Map<RegisterCommand>(registerRequest);
            await _mediator.Send(command);

            _logger.LogInformation("User registration successfully");

            return Created();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Authenticates the user and generates an access token.")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
        {
            _logger.LogInformation("User login attempt");

            var command = _mapper.Map<LoginCommand>(loginRequest);
            var authResult = await _mediator.Send(command);
            var response = _mapper.Map<AuthResponse>(authResult);

            Response.Cookies.Append("RefreshToken", authResult.RefreshToken, authResult.CookieTokenExpiaryOptions);
            _logger.LogInformation("User logged in successfully");

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("tokens:refresh")]
        [SwaggerOperation(Summary = "Generate new access token using a refresh token.")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("Refreshing token");  // Log information

            string? refreshToken = HttpContext?.Request?.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token not found in cookies");  // Log warning
                return Unauthorized();
            }

            var command = new RefreshTokenCommand(refreshToken);
            var authResult = await _mediator.Send(command);
            var response = _mapper.Map<AuthResponse>(authResult);
            Response.Cookies.Append("RefreshToken", authResult.RefreshToken, authResult.CookieTokenExpiaryOptions);

            _logger.LogInformation("Token refreshed successfully");  // Log success
            return Ok(response);
        }
    }
}
