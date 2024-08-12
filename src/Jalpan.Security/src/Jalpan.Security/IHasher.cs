namespace Jalpan.Security;

public interface IHasher
{
    string Hash(string data);
    byte[] Hash(byte[] data);
}
