using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Client;

namespace FlowDash.API.Middleware
{
    public class TenantValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantValidationMiddleware> _logger;

        public TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantIdentifierService tenantIdentifier)
        {
            // If user is not authenticated, skip tenant validation
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var tokenTenant = context.User.FindFirst("tenant")?.Value;
            var currentTenant = tenantIdentifier.GetCurrentTenantName();

            if (string.IsNullOrWhiteSpace(tokenTenant))
            {
                _logger.LogWarning("Tenant information missing in token");
                throw new ForbiddenException("Tenant information missing in token");
            }

            if (!string.Equals(tokenTenant, currentTenant, StringComparison.Ordinal))
            {
                _logger.LogWarning("You don't have access to this tenant. TokenTenant: {TokenTenant}, RequestTenant: {RequestTenant}", tokenTenant,currentTenant);
                throw new ForbiddenException("You don't have access to this tenant");
            }

            await _next(context);
        }
    }
}
