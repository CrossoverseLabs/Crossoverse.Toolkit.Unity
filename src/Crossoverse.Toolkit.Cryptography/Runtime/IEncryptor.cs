using System.Threading.Tasks;

namespace Crossoverse.Toolkit.Cryptography
{
    public interface IEncryptor
    {
        string Key { get; set; }
        string Salt { get; set; }
        byte[] Encrypt(byte[] data);
        Task<byte[]> EncryptAsync(byte[] data);
    }
}
