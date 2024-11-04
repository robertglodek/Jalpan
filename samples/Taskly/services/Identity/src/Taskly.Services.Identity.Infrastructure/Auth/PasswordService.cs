using Microsoft.AspNetCore.Identity;
using Taskly.Services.Identity.Application.Services;

namespace Taskly.Services.Identity.Infrastructure.Auth;

internal sealed class PasswordService(IPasswordHasher<IPasswordService> passwordHasher) : IPasswordService
{
    private readonly IPasswordHasher<IPasswordService> _passwordHasher = passwordHasher;

    public bool IsValid(string hashedPassword, string password)
        => _passwordHasher.VerifyHashedPassword(this, hashedPassword, password) != PasswordVerificationResult.Failed;

    public string Hash(string password)
        => _passwordHasher.HashPassword(this, password);
}
