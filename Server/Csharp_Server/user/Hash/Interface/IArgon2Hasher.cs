namespace Hash.Interface
{
    public interface IArgon2Hasher
    {
        string Encrypt(string password, string key);
        string GenerateKey();
        string GenerateHash(string data, string salt);
    }
}