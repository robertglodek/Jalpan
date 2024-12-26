using Microsoft.AspNetCore.Identity;
using Taskly.Services.Identity.Application.Services;

namespace Taskly.Services.Identity.Infrastructure.Auth;

internal sealed class PasswordService(IPasswordHasher<IPasswordService> passwordHasher) : IPasswordService
{
    public bool IsValid(string hashedPassword, string password)
        => passwordHasher.VerifyHashedPassword(this, hashedPassword, password) != PasswordVerificationResult.Failed;

    public string Hash(string password)
        => passwordHasher.HashPassword(this, password);
}