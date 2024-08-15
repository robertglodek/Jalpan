using System.Security.Cryptography.X509Certificates;

namespace Jalpan.Secrets.Vault;

public interface ICertificatesIssuer
{
    Task<X509Certificate2?> IssueAsync();
}