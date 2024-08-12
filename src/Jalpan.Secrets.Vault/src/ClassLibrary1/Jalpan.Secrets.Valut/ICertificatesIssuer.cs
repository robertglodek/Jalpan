using System.Security.Cryptography.X509Certificates;

namespace Jalpan.Secretc.Vault;

public interface ICertificatesIssuer
{
    Task<X509Certificate2?> IssueAsync();
}