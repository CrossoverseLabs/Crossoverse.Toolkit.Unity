using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Crossoverse.Toolkit.Cryptography
{
    public class RijndaelDecryptor : IDecryptor
    {
        public string Key { get; set; }
        public string Salt { get; set; }

        public int KeySize { get; set; }
        public int BlockSize { get; set; }

        public RijndaelDecryptor()
        {
            KeySize = 128;
            BlockSize = 128;
        }

        public byte[] Decrypt(byte[] data)
        {
            GenerateKeyBytesAndIV(out byte[] keyBytes, out byte[] iv);

            using var rijndael = new RijndaelManaged();
            rijndael.KeySize = KeySize;
            rijndael.BlockSize = BlockSize;
            rijndael.Key = keyBytes;
            rijndael.IV = iv;

            using var decryptor = rijndael.CreateDecryptor();
            var decryptedData = decryptor.TransformFinalBlock(data, 0, data.Length);

            return decryptedData;
        }

        public async Task<byte[]> DecryptAsync(byte[] data)
        {
            GenerateKeyBytesAndIV(out byte[] keyBytes, out byte[] iv);

            using var rijndael = new RijndaelManaged();
            rijndael.KeySize = KeySize;
            rijndael.BlockSize = BlockSize;
            rijndael.Key = keyBytes;
            rijndael.IV = iv;

            using var decryptor = rijndael.CreateDecryptor();
            var decryptedData = await Task.Run(() => decryptor.TransformFinalBlock(data, 0, data.Length));

            return decryptedData;
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
