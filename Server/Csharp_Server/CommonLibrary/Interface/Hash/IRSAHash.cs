namespace Server.Interface.Hash
{
    public interface IRSAHash
    {
        public string GeneratePublicKeys();
        public string GeneratePrivateKeys();
        public byte[] Encrypt(string data, string publicKey);
        public string Decrypt(byte[] data, string privateKey);
    }
}
