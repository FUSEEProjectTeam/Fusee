namespace Fusee.Engine
{
    /// <summary>
    /// Struct containing the necessary ImageData for further provessing (e.g. texturing)
    /// </summary>
    public struct ImageData
    {
        public int Width;
        public int Height;
        public int Stride;
        public byte[] PixelData;
    }
}