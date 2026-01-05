namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITenantIdentifierService
    {
        string GetCurrentTenantName();
        void SetCurrentTenantName(string name);
    }
}
