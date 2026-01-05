namespace FlowDash.Infrastructure.Settings
{
    public class CorsSettings
    {
        public const string SectionName = "CorsSettings";

        public string PolicyName { get; set; } = null!;
        public string[] AllowedOrigins { get; set; } = null!;
        public string[] AllowedEndPatterns { get; set; } = null!;
    }
}
