﻿namespace Jalpan.Security.Encryptors;

public interface IEncryptor
{
    string Encrypt(string data, string key);
    string Decrypt(string data, string key);
    byte[] Encrypt(byte[] data, byte[] iv, byte[] key);
    byte[] Decrypt(byte[] data, byte[] iv, byte[] key);
}
