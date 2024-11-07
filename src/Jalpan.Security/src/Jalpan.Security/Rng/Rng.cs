using System.Security.Cryptography;
using System.Text;

namespace Jalpan.Security.Rng;

internal sealed class Rng : IRng
{
    private static readonly HashSet<char> SpecialChars = ['/', '\\', '=', '+', '?', ':', '&'];

    public string Generate(int length = 50, bool removeSpecialChars = true)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var result = Convert.ToBase64String(bytes);

        if (!removeSpecialChars) return result;
        var sb = new StringBuilder(result.Length);
        foreach (var c in result.Where(c => !SpecialChars.Contains(c)))
        {
            sb.Append(c);
        }
        return sb.ToString();

    }
}
