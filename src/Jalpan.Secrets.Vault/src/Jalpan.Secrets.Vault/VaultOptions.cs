namespace Jalpan.Secrets.Vault;

public sealed class VaultOptions
{
    public bool Enabled { get; init; }
    public string Url { get; init; } = string.Empty;
    public AuthenticationOptions Authentication { get; init; } = new();
    public bool RevokeLeaseOnShutdown { get; init; }
    public int RenewalsInterval { get; init; }
    public KeyValueOptions Kv { get; init; } = new();
    public PkiOptions Pki { get; init; } = new();
    public Dictionary<string, LeaseOptions> Lease { get; init; } = [];

    public sealed class AuthenticationOptions
    {
        public string Type { get; init; } = string.Empty;
        public TokenOptions Token { get; init; } = new();
        public UserPassOptions UserPass { get; init; } = new();

        public sealed class TokenOptions
        {
            public string Token { get; init; } = string.Empty;
        }

        public sealed class UserPassOptions
        {
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
        }
    }

    public sealed class KeyValueOptions
    {
        public bool Enabled { get; init; }
        public string EngineVersion { get; init; } = "V1";
        public string MountPoint { get; init; } = "secret";
        public string Path { get; init; } = string.Empty;
        public int? Version { get; init; }
    }

    public sealed class LeaseOptions
    {
        public bool Enabled { get; init; }
        public string Type { get; init; } = string.Empty;
        public string RoleName { get; init; } = string.Empty;
        public string MountPoint { get; init; } = string.Empty;
        public bool AutoRenewal { get; init; }
        public Dictionary<string, string> Templates { get; init; } = [];
    }

    public sealed class PkiOptions
    {
        public bool Enabled { get; init; }
        public string RoleName { get; init; } = string.Empty;
        public string MountPoint { get; init; } = string.Empty;
        public string CertificateFormat { get; init; } = string.Empty;
        public string PrivateKeyFormat { get; init; } = string.Empty;
        public string CommonName { get; init; } = string.Empty;
        public string Ttl { get; init; } = string.Empty;
        public string SubjectAlternativeNames { get; init; } = string.Empty;
        public string OtherSubjectAlternativeNames { get; init; } = string.Empty;
        public bool ExcludeCommonNameFromSubjectAlternativeNames { get; init; }
        public string IpSubjectAlternativeNames { get; init; } = string.Empty;
        public string UriSubjectAlternativeNames { get; init; } = string.Empty;
        public bool ImportPrivateKey { get; init; }
        public HttpHandlerOptions HttpHandler { get; init; } = new();

        public sealed class HttpHandlerOptions
        {
            public bool Enabled { get; init; }
            public string Certificate { get; init; } = string.Empty;
        }
    }
}