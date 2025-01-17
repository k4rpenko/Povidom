namespace UserServer.Interface.Hash
{
    public interface IHASH
    {
        byte[] GenerateKey();
        string Encrypt(string message, string key);
        string HashSha256(string message);
    }
}
