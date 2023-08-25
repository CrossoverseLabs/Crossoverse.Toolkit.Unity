using System;

namespace Crossoverse.Toolkit.Cryptography.Text
{
    public class TextEncryptor : ITextEncryptor
    {
        private readonly IEncryptor _encryptor;

        public TextEncryptor(IEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        public void SetKey(string key)
        {
            _encryptor.Key = key;
        }

        public void SetSalt(string salt)
        {
            _encryptor.Salt = salt;
        }

        public string Encrypt(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            var encryptedBytes = _encryptor.Encrypt(bytes);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
