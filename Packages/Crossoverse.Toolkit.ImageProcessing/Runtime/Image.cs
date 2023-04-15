namespace Crossoverse.Toolkit.ImageProcessing
{
    public class Image
    {
		public int Width { get; set; }
		public int Height { get; set; }
		public ColorFormat Format { get; set; }
		public byte[] Data { get; set; }
    }
}