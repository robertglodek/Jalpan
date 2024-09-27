namespace Jalpan.Security.Hashers;

public interface IHasher
{
    string Hash(string data);
    byte[] Hash(byte[] data);
}
