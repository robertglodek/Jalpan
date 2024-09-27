using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.Json;

namespace Jalpan.Security.Signers;

internal sealed class Signer : ISigner
{
    public string Sign(object data, X509Certificate2 certificate)
    {
        ValidateInput(data, certificate);

        var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
        var signatureBytes = Sign(dataBytes, certificate);

        return ConvertBytesToHex(signatureBytes);
    }

    public bool Verify(object data, X509Certificate2 certificate, string signature, bool throwException = false)
    {
        ValidateInput(data, certificate);

        if (string.IsNullOrWhiteSpace(signature))
        {
            throw new ArgumentException("Signature cannot be empty.", nameof(signature));
        }

        var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
        var signatureBytes = ConvertHexToBytes(signature);

        return Verify(dataBytes, certificate, signatureBytes, throwException);
    }

    public byte[] Sign(byte[] data, X509Certificate2 certificate)
    {
        ValidateInput(data, certificate);

        using var rsa = certificate.GetRSAPrivateKey();

        return rsa is null
            ? throw new InvalidOperationException("RSA private key couldn't be loaded.")
            : rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool Verify(byte[] data, X509Certificate2 certificate, byte[] signature, bool throwException = false)
    {
        ValidateInput(data, certificate);

        if (signature is null || signature.Length == 0)
        {
            throw new ArgumentException("Signature cannot be empty.", nameof(signature));
        }

        try
        {
            using var rsa = certificate.GetRSAPublicKey();

            return rsa is null
                ? throw new InvalidOperationException("RSA public key couldn't be loaded.")
                : rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            if (throwException)
            {
                throw;
            }

            return false;
        }
    }

    private static void ValidateInput(object data, X509Certificate2 certificate)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        }

        if (certificate == null)
        {
            throw new ArgumentNullException(nameof(certificate), "Certificate cannot be null.");
        }
    }

    private static string ConvertBytesToHex(byte[] bytes)
        => BitConverter.ToString(bytes).Replace("-", string.Empty);
    
    private static byte[] ConvertHexToBytes(string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }
}