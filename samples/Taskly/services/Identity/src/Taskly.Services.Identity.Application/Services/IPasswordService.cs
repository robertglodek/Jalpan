namespace Taskly.Services.Identity.Application.Services;

public interface IPasswordService
{
    bool IsValid(string hashedPassword, string password);
    string Hash(string password);
}