using FlowDash.Application.Authentication.Common;
using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Client;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FlowDash.Application.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMd5HashGenerator _md5HashGenerator;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler
        (
            ITokenService tokenService,
            IUserRepository userRepository,
            IMd5HashGenerator md5HashGenerator,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger,
            IDateTimeProvider dateTimeProvider
        )
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _md5HashGenerator = md5HashGenerator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email);
            var passwordHash = _md5HashGenerator.Generate(request.Password);

            if (user == null)
            {
                throw new BadRequestException("User name or password is incorrect");
            }

            if (user.Password != passwordHash)
            {
                throw new UnauthorizedException("User name or password is incorrect.");
            }

            var acessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();
            var cookiesOption = _tokenService.SetRefreshTokenExpiary(refreshToken);
            var loginResult = _mapper.Map<AuthResult>((user, acessToken, refreshToken, cookiesOption));
            var SuccessUserDetails = new
            {
                UserId = user.Id,
                UserEmail = user.Email,
            };

            _logger.LogInformation("Login Successful: {SuccessUserDetails}", JsonSerializer.Serialize(SuccessUserDetails));

            return loginResult;
        }
    }
}
