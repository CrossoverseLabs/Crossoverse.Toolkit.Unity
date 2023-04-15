using StbImageSharp;

namespace Crossoverse.Toolkit.ImageProcessing
{
    public class StbImageDecoder : IDecoder
    {
        private readonly bool _flipVertically;

        public StbImageDecoder(bool flipVertically = true)
        {
            _flipVertically = flipVertically;
        }

        public Image Decode(byte[] encodedData)
        {
            var flag = _flipVertically ? 1 : 0;
            StbImage.stbi_set_flip_vertically_on_load(flag);

            var result = ImageResult.FromMemory(encodedData);

            var format = result.Comp switch
            {
                ColorComponents.RedGreenBlueAlpha => ColorFormat.RGBA32,
                ColorComponents.RedGreenBlue => ColorFormat.RGB24,
                _ => ColorFormat.RGBA32
            };

            var image = new Image()
            {
                Width = result.Width, 
                Height = result.Height, 
                Format = format,
                Data = result.Data,
            };

            return image;
        }
    }
}