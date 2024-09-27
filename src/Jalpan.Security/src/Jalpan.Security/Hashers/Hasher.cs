using System.Security.Cryptography;
using System.Text;

namespace Jalpan.Security.Hashers;

internal sealed class Hasher : IHasher
{
    public string Hash(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data to be hashed cannot be null or empty.", nameof(data));
        }

        var dataBytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = Hash(dataBytes);

        return ConvertBytesToHex(hashBytes);
    }

    private static string ConvertBytesToHex(byte[] bytes)
        => BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();

    public byte[] Hash(byte[] data)
    {
        if (data is null || data.Length == 0)
        {
            throw new ArgumentException("Data to be hashed cannot be empty.", nameof(data));
        }

        return SHA256.HashData(data);
    }
}