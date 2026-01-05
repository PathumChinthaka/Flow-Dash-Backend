using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Server;
using FlowDash.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FlowDash.Infrastructure.Services
{
    public class TenantIdentifierService : ITenantIdentifierService
    {
        public const string DefaultTenant = "flowdashdev";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ServerSettings _serverSettings;
        private string? _currentTenantName;

        public TenantIdentifierService(IHttpContextAccessor httpContextAccessor, IOptions<ServerSettings> serverOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _serverSettings = serverOptions.Value;
        }

        public string GetCurrentTenantName()
        {
            if (_currentTenantName != null)
            {
                return _currentTenantName;
            }

            try
            {
                var host = _httpContextAccessor.HttpContext?.Request.Host.Value;
                if (host != null && (host.Contains("localhost") || host.Contains("flowdashdev")))
                {
                    return "flowdashdev";
                }

                if (string.IsNullOrEmpty(host))
                {
                    // this is first load
                    return "public";
                }

                var parts = host.Split('.');

                // Contains subdomain
                if (parts.Length > (_serverSettings.Domain.Split(".").Length))
                {
                    return parts[0].ToLower().Replace("https://", "").Replace("http://", "").Replace("/", "").Replace("http", "").Replace("https", "");
                }
                // No subdomain - Admin
                else if (parts.Length == (_serverSettings.Domain.Split(".").Length))
                {
                    return DefaultTenant;
                }
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }

            throw new InternalServerException("Unable to identify the subdomain");
        }

        public void SetCurrentTenantName(string name)
        {
            _currentTenantName = name;
        }
    }
}
