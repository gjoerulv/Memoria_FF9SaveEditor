namespace Memoria.ReUtils
{
    public class DataAesEncryption
    {
        public byte[] Encrypt(byte[] bytes)
        {
            return Crypto.AESCryptography.Encrypt(bytes);
        }
        public byte[] Decrypt(byte[] bytes)
        {
            return Crypto.AESCryptography.Decrypt(bytes);
        }
        public int GetCipherSize(int plainTextSize)
        {
            int num = 16;
            return plainTextSize + num - plainTextSize % num;
        }
    }
}
