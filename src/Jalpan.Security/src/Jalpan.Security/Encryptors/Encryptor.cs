using System.Security.Cryptography;
using System.Text;

namespace Jalpan.Security.Encryptors;

internal sealed class Encryptor : IEncryptor
{
    // AES uses a 256-bit key (32 bytes)
    private const int KeySize = 32;

    public string Encrypt(string data, string key)
    {
        ValidateInput(data, key);

        using var aes = Aes.Create();
        aes.Key = GetValidKey(key);
        aes.GenerateIV();

        var iv = Convert.ToBase64String(aes.IV);
        var encryptedData = EncryptData(Encoding.UTF8.GetBytes(data), aes.Key, aes.IV);

        return iv + Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string encryptedData, string key)
    {
        ValidateInput(encryptedData, key);

        var iv = Convert.FromBase64String(encryptedData[..24]);
        var encryptedBytes = Convert.FromBase64String(encryptedData[24..]);

        using var aes = Aes.Create();
        aes.Key = GetValidKey(key);
        aes.IV = iv;

        var decryptedBytes = DecryptData(encryptedBytes, aes.Key, aes.IV);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public byte[] Encrypt(byte[] data, byte[] iv, byte[] key)
    {
        ValidateByteArray(data, nameof(data));
        ValidateByteArray(iv, nameof(iv));
        ValidateByteArray(key, nameof(key));

        return EncryptData(data, key, iv);
    }

    public byte[] Decrypt(byte[] encryptedData, byte[] iv, byte[] key)
    {
        ValidateByteArray(encryptedData, nameof(encryptedData));
        ValidateByteArray(iv, nameof(iv));
        ValidateByteArray(key, nameof(key));

        return DecryptData(encryptedData, key, iv);
    }


    private static byte[] EncryptData(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(key, iv);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }

    private static byte[] DecryptData(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(key, iv);
        using var memoryStream = new MemoryStream(data);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var outputMemoryStream = new MemoryStream();

        cryptoStream.CopyTo(outputMemoryStream);
        return outputMemoryStream.ToArray();
    }

    private static void ValidateInput(string data, string key)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Encryption key cannot be null or empty.", nameof(key));
        }
    }

    private static void ValidateByteArray(byte[] data, string paramName)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        }
    }

    private static byte[] GetValidKey(string key)
    {
        // Ensure the key is exactly 32 bytes long
        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length == KeySize)
        {
            return keyBytes;
        }

        var paddedKey = new byte[KeySize];
        Buffer.BlockCopy(keyBytes, 0, paddedKey, 0, Math.Min(keyBytes.Length, KeySize));

        return paddedKey;
    }
}