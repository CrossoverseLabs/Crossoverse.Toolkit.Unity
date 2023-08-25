using System.Threading.Tasks;

namespace Crossoverse.Toolkit.Cryptography
{
    public interface IDecryptor
    {
        string Key { get; set; }
        string Salt { get; set; }
        byte[] Decrypt(byte[] data);
        Task<byte[]> DecryptAsync(byte[] data);
    }
}
