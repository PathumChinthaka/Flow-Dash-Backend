using FlowDash.Application.Authentication.Common;
using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Client;
using MapsterMapper;
using MediatR;

namespace FlowDash.Application.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        private readonly ITokenService _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenCommandHandler
        (
            ITokenService tokenGenerator,
            IMapper mapper,
            IRefreshTokenRepository refreshTokenRepository
        )
        {
            _jwtTokenGenerator = tokenGenerator;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await _refreshTokenRepository.GetByToken(request.Token)
                ?? throw new UnauthorizedException("Provided token is not valid");

                var token = _jwtTokenGenerator.CreateAccessToken(refreshToken.User);

                var cookiesOption = _jwtTokenGenerator.SetRefreshTokenExpiary(refreshToken);

                return _mapper.Map<AuthResult>((refreshToken.User, token, refreshToken, cookiesOption));
            }
            catch(Exception ex)
            {
                throw new UnauthorizedException("Could not refresh token", ex);
            }
        }
    }
}
