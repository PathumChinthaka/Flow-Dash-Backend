using FlowDash.Application.Common.Interfaces.Service;

namespace FlowDash.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    }
}
