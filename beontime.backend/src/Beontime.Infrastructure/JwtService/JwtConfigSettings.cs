namespace Beontime.Infrastructure.JwtService
{

    public sealed class JwtConfigSettings
    {
        public const string SectionName = "JwtConfig";

        public string Audience { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string SecurityKey { get; init; } = string.Empty;
    }
}
