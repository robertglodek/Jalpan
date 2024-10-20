using Microsoft.AspNetCore.Identity;

namespace Jalpan.Auth.Services;

internal sealed class PasswordService(IPasswordHasher<IPasswordService> passwordHasher) : IPasswordService
{
    private readonly IPasswordHasher<IPasswordService> _passwordHasher = passwordHasher;

    public bool IsValid(string hash, string password)
        => _passwordHasher.VerifyHashedPassword(this, hash, password) != PasswordVerificationResult.Failed;

    public string Hash(string password)
        => _passwordHasher.HashPassword(this, password);
}
