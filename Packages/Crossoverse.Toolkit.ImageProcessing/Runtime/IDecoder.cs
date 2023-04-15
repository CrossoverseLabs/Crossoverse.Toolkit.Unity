namespace Crossoverse.Toolkit.ImageProcessing
{
    public interface IDecoder
    {
        Image Decode(byte[] encodedData);
    }
}