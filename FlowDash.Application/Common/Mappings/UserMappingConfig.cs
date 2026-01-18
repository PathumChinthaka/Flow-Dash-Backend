using FlowDash.Application.User.Queries.Common;
using Mapster;
using UserModel = FlowDash.Domain.Entities.User;

namespace FlowDash.Application.Common.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserModel, GetUserResult>()
                 .Map(dest => dest.Name, src => string.Join(" ", src.FirstName, src.LastName).Trim());
        }
    }
}
