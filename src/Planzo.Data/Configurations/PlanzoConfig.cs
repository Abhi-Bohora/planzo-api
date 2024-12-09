namespace Planzo.Data.Configurations;

public static class PlanzoConfig
{
    public static string? ConnectionString { get; set; }
    public static JwtSettings? JwtSettings { get; set; }
}

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
}