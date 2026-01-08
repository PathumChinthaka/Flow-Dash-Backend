using FlowDash.Application.Authentication.Common;
using FlowDash.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Mappings
{
    public class AuthenticationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(User User, TokenResult TokenResult, RefreshToken RefreshToken, CookieOptions cookiesOptions), AuthResult>()
                 .Map(dest => dest.AccessToken, src => src.TokenResult.Value)
                 .Map(dest => dest.AccessTokenExpiresOn, src => src.TokenResult.ExpiresOn)
                 .Map(dest => dest.CookieTokenExpiaryOptions, src => src.cookiesOptions)
                 .Map(dest => dest.RefreshToken, src => src.RefreshToken.Token)
                 .Map(dest => dest.FirstName, src => src.User.FirstName)
                 .Map(dest => dest.LastName, src => src.User.LastName)
                 .Map(dest => dest.Email, src => src.User!.Email);
        }
    }
}
