using System;

namespace Crossoverse.Toolkit.Cryptography.Text
{
    public class TextDecryptor : ITextDecryptor
    {
        private readonly IDecryptor _decryptor;

        public TextDecryptor(IDecryptor decryptor)
        {
            _decryptor = decryptor;
        }

        public void SetKey(string key)
        {
            _decryptor.Key = key;
        }

        public void SetSalt(string salt)
        {
            _decryptor.Salt = salt;
        }

        public string Decrypt(string text)
        {
            var bytes = Convert.FromBase64String(text);
            var decryptedBytes = _decryptor.Decrypt(bytes);
            return System.Text.Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
