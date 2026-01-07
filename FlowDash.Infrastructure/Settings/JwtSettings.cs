namespace FlowDash.Infrastructure.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public int AccessTokenMinutes { get; set; }
        public string Audience { get; set; } = null!;
        public string RefreshTokenDays { get; set; } = null!;
    }
}
