using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FlyFramework.Extentions
{
    public class SimpleStringCipher
    {
        //
        // 摘要:
        //     This constant string is used as a "salt" value for the PasswordDeriveBytes function
        //     calls. This size of the IV (in bytes) must = (keysize / 8). Default keysize is
        //     256, so the IV must be 32 bytes long. Using a 16 character string here gives
        //     us 32 bytes when converted to a byte array.
        public byte[] InitVectorBytes;

        //
        // 摘要:
        //     This constant is used to determine the keysize of the encryption algorithm.
        public const int DefaultKeysize = 256;

        public static SimpleStringCipher Instance { get; }

        //
        // 摘要:
        //     Default password to encrypt/decrypt texts. It's recommented to set to another
        //     value for security. Default value: "gsKnGZ041HLL4IM8"
        public static string DefaultPassPhrase { get; set; }

        //
        // 摘要:
        //     Default value: Encoding.ASCII.GetBytes("jkE49230Tf093b42")
        public static byte[] DefaultInitVectorBytes { get; set; }

        //
        // 摘要:
        //     Default value: Encoding.ASCII.GetBytes("hgt!16kl")
        public static byte[] DefaultSalt { get; set; }

        static SimpleStringCipher()
        {
            DefaultPassPhrase = "gsKnGZ041HLL4IM8";
            DefaultInitVectorBytes = Encoding.ASCII.GetBytes("jkE49230Tf093b42");
            DefaultSalt = Encoding.ASCII.GetBytes("hgt!16kl");
            Instance = new SimpleStringCipher();
        }

        public SimpleStringCipher()
        {
            InitVectorBytes = DefaultInitVectorBytes;
        }

        public string Encrypt(string plainText, string passPhrase = null, byte[] salt = null, int? keySize = null, byte[] initVectorBytes = null)
        {
            if (plainText == null)
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            if (salt == null)
            {
                salt = DefaultSalt;
            }

            if (!keySize.HasValue)
            {
                keySize = 256;
            }

            if (initVectorBytes == null)
            {
                initVectorBytes = InitVectorBytes;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(keySize.Value / 8);
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            using ICryptoTransform transform = aes.CreateEncryptor(bytes2, initVectorBytes);
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string cipherText, string passPhrase = null, byte[] salt = null, int? keySize = null, byte[] initVectorBytes = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            if (salt == null)
            {
                salt = DefaultSalt;
            }

            if (!keySize.HasValue)
            {
                keySize = 256;
            }

            if (initVectorBytes == null)
            {
                initVectorBytes = InitVectorBytes;
            }

            byte[] array = Convert.FromBase64String(cipherText);
            using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt);
            byte[] bytes = rfc2898DeriveBytes.GetBytes(keySize.Value / 8);
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            using ICryptoTransform transform = aes.CreateDecryptor(bytes, initVectorBytes);
            using MemoryStream stream = new MemoryStream(array);
            using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            byte[] array2 = new byte[array.Length];
            int i;
            int num;
            for (i = 0; i < array2.Length; i += num)
            {
                num = cryptoStream.Read(array2, i, array2.Length - i);
                if (num == 0)
                {
                    break;
                }
            }

            return Encoding.UTF8.GetString(array2, 0, i);
        }
    }
}
