namespace Jalpan.Auth.Jwt;

public sealed class AuthOptions
{
    public string Algorithm { get; init; } = string.Empty;
    public CertificateOptions? Certificate { get; init; }
    public JwtOptions Jwt { get; init; } = new();

    public sealed class CertificateOptions
    {
        public string? Location { get; init; }
        public string? RawData { get; init; }
        public string? Password { get; init; }
    }

    public sealed class JwtOptions
    {
        public string? Issuer { get; init; }
        public string? IssuerSigningKey { get; init; }
        public string? Authority { get; init; }
        public string? Audience { get; init; }
        public string Challenge { get; init; } = "Bearer";
        public string MetadataAddress { get; init; } = string.Empty;
        public bool SaveToken { get; init; } = true;
        public bool SaveSigninToken { get; init; }
        public bool RequireAudience { get; init; } = true;
        public bool RequireHttpsMetadata { get; init; } = true;
        public bool RequireExpirationTime { get; init; } = true;
        public bool RequireSignedTokens { get; init; } = true;
        public TimeSpan Expiry { get; init; } = TimeSpan.FromHours(1);
        public string? ValidAudience { get; init; }
        public IEnumerable<string>? ValidAudiences { get; init; }
        public string? ValidIssuer { get; init; }
        public IEnumerable<string>? ValidIssuers { get; init; }
        public bool ValidateActor { get; init; }
        public bool ValidateAudience { get; init; } = true;
        public bool ValidateIssuer { get; init; } = true;
        public bool ValidateLifetime { get; init; } = true;
        public bool ValidateTokenReplay { get; init; }
        public bool ValidateIssuerSigningKey { get; init; }
        public bool RefreshOnIssuerKeyNotFound { get; init; } = true;
        public bool IncludeErrorDetails { get; init; } = true;
        public string? AuthenticationType { get; init; }
        public string? NameClaimType { get; init; }
        public string? RoleClaimType { get; init; }
    }
}
