namespace Jalpan.Security.Hashing;

public interface IHasher
{
    string Hash(string data);
    byte[] Hash(byte[] data);
}
