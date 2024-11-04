namespace Jalpan.Security.Rng;

public interface IRng
{
    string Generate(int length = 50, bool removeSpecialChars = true);
}
