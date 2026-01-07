using FlowDash.Application.Authentication.Common;
using FlowDash.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Mappings
{
    internal class AuthenticationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(User User, TokenResult AccessToken, RefreshToken RefreshToken, CookieOptions cookiesOptions), AuthResult>()
                 .Map(dest => dest.AccessToken, src => src.AccessToken)
                 .Map(dest => dest.AccessTokenExpiresOn, src => src.AccessToken.ExpiresOn)
                 .Map(dest => dest.CookieTokenExpiaryOptions, src => src.cookiesOptions)
                 .Map(dest => dest.RefreshToken, src => src.RefreshToken.Token)
                 .Map(dest => dest.FirstName, src => src.User.FirstName)
                 .Map(dest => dest.LastName, src => src.User.LastName)
                 .Map(dest => dest.Email, src => src.User!.Email);
        }
    }
}
