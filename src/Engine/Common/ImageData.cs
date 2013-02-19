namespace Fusee.Engine
{
    /// <summary>
    /// Struct containing Image Data for further processing (e.g. texturing)
    /// </summary>
    public struct ImageData
    {
        public int Width;
        public int Height;
        public int Stride;
        public byte[] PixelData;
    }
}