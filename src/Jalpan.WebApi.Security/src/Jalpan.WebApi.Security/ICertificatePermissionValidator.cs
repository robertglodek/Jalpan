using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace Jalpan.WebApi.Security;

public interface ICertificatePermissionValidator
{
    bool HasAccess(X509Certificate2 certificate, IEnumerable<string> permissions, HttpContext context);
}
