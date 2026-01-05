namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITenantCreationService
    {
        Task CreateSchema(string tenantName);
        Task DeleteSchema(string tenantName);
        Task UpdateDatabase();
    }
}
