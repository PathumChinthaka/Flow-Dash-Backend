using FlowDash.Application.Authentication.Common;
using FlowDash.Domain.Entities;
using UserModel = FlowDash.Domain.Entities.User;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Mappings
{
    public class AuthenticationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(UserModel user, TokenResult tokenResult, RefreshToken refreshToken, CookieOptions cookiesOptions), AuthResult>()
                 .Map(dest => dest.AccessToken, src => src.tokenResult.Value)
                 .Map(dest => dest.AccessTokenExpiresOn, src => src.tokenResult.ExpiresOn)
                 .Map(dest => dest.CookieTokenExpiaryOptions, src => src.cookiesOptions)
                 .Map(dest => dest.RefreshToken, src => src.refreshToken.Token)
                 .Map(dest => dest.FirstName, src => src.user.FirstName)
                 .Map(dest => dest.LastName, src => src.user.LastName)
                 .Map(dest => dest.Email, src => src.user!.Email);
        }
    }
}
