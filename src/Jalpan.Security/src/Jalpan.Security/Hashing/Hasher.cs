using System.Security.Cryptography;
using System.Text;

namespace Jalpan.Security.Hashing;

internal sealed class Hasher : IHasher
{
    public string Hash(string data)
    {
        var hash = Hash(Encoding.UTF8.GetBytes(data));
        var builder = new StringBuilder();
        foreach (var @byte in hash)
        {
            builder.Append(@byte.ToString("x2"));
        }

        return builder.ToString();
    }

    public byte[] Hash(byte[] data)
    {
        if (data is null || data.Length == 0)
        {
            throw new ArgumentException("Data to be hashed cannot be empty.", nameof(data));
        }

        return SHA256.HashData(data);
    }
}