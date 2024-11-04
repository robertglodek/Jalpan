using System.Security.Cryptography.X509Certificates;

namespace Jalpan.Secrets.Valut.Issuers;

public class EmptyCertificatesIssuer : ICertificatesIssuer
{
    public Task<X509Certificate2?> IssueAsync() => Task.FromResult<X509Certificate2?>(null);
}