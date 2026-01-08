using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Exceptions.Client;
using FlowDash.Application.Exceptions.Server;
using FlowDash.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowDash.Application.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMd5HashGenerator _md5HashGenerator;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler
        (
            IUserRepository userRepository,
            IMd5HashGenerator md5HashGenerator,
            ILogger<RegisterCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _md5HashGenerator = md5HashGenerator;
            _logger = logger;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByEmail(request.Email);

                if (user != null)
                {
                    throw new ConflictException("Email already registered");
                }

                var newUser = new User
                {
                    Email = request.Email.Trim().ToLower(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Password = _md5HashGenerator.Generate(request.Password)
                };

                await _userRepository.Create(newUser);

                return new RegisterResult(newUser.Id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error occurred during user registration: {Message}", ex.Message);
                throw new InternalServerException("User registration failed");
            }
        }
    }
}
