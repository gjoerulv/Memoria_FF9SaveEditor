using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Memoria.Crypto
{
    public class AESCryptography
    {
        public const int KeySize = 256;
        public const int BlockSize = 128;
        private static SecureString password = new SecureString();
        public static byte[] Encrypt(byte[] bytesToBeEncrypted)
        {
            byte[] array = GetPassword();
            byte[] result = null;
            byte[] salt = GetSalt();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(array, salt, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cryptoStream.Close();
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        public static byte[] Decrypt(byte[] bytesToBeDecrypted)
        {
            byte[] array = GetPassword();
            byte[] result = null;
            byte[] salt = GetSalt();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(array, salt, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cryptoStream.Close();
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        private static byte[] GetPassword()
        {
            string[] array = new string[]
            {
            "67434cd0-1ca3-11e5-9a21-1697f925ec7b",
            "7a5313a0-1ca3-11e5-b939-0800200c9a66"
            };
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                for (int j = 0; j < text.Length; j++)
                {
                    password.AppendChar(text[j]);
                }
            }
            byte[] bytes = Encoding.UTF8.GetBytes(password.ToString());
            password.Clear();
            return bytes;
        }
        private static byte[] GetSalt()
        {
            return new byte[]
            {
            3,
            3,
            1,
            4,
            7,
            0,
            9,
            7
            };
        }

        public static void cryptFile(byte[] file, bool encrypt)
        {
            byte[] ret;
            if (encrypt)
                ret = Encrypt(file);
            else
                ret = Decrypt(file);

            File.WriteAllBytes("file.test", ret);
        }
    }
}
