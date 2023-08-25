using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Crossoverse.Toolkit.Cryptography
{
    public class RijndaelEncryptor : IEncryptor
    {
        public string Key { get; set; }
        public string Salt { get; set; }

        public int KeySize { get; set; }
        public int BlockSize { get; set; }

        public RijndaelEncryptor()
        {
            KeySize = 128;
            BlockSize = 128;
        }

        public byte[] Encrypt(byte[] data)
        {
            GenerateKeyBytesAndIV(out byte[] keyBytes, out byte[] iv);

            using var rijndael = new RijndaelManaged();
            rijndael.KeySize = KeySize;
            rijndael.BlockSize = BlockSize;
            rijndael.Key = keyBytes;
            rijndael.IV = iv;

            using var encryptor = rijndael.CreateEncryptor();
            var encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);

            return encryptedData;
        }

        public async Task<byte[]> EncryptAsync(byte[] data)
        {
            GenerateKeyBytesAndIV(out byte[] keyBytes, out byte[] iv);

            using var rijndael = new RijndaelManaged();
            rijndael.KeySize = KeySize;
            rijndael.BlockSize = BlockSize;
            rijndael.Key = keyBytes;
            rijndael.IV = iv;

            using var encryptor = rijndael.CreateEncryptor();
            var encryptedData = await Task.Run(() => encryptor.TransformFinalBlock(data, 0, data.Length));

            return encryptedData;
        }

        private void GenerateKeyBytesAndIV(out byte[] keyBytes, out byte[] iv)
        {
            var bSalt = System.Text.Encoding.UTF8.GetBytes(Salt);

            var deriveBytes = new Rfc2898DeriveBytes(Key, bSalt);
            deriveBytes.IterationCount = 1000;

            keyBytes = deriveBytes.GetBytes(KeySize / 8);
            iv = deriveBytes.GetBytes(BlockSize / 8);
        }
    }
}
