using System.Security.Cryptography.X509Certificates;

namespace Jalpan.Secrets.Valut.Issuers;

public interface ICertificatesIssuer
{
    Task<X509Certificate2?> IssueAsync();
}