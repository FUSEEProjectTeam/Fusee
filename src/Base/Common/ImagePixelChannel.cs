namespace Fusee.Base.Common
{
    /// <summary>
    /// The ImagePixelChannel contains data about pixel bits and pixel encoding
    /// </summary>
    public struct ImagePixelChannel
    {
        /// <summary>
        /// The first bit belonging to this pixel channel
        /// </summary>
        public int FirstBit { get; }

        /// <summary>
        /// The number of contiguous bits contributing to this channel's data starting at FirstBit
        /// </summary>
        public int NumBits { get; }

        /// <summary>
        /// How the bits are to be interpreted, probably something like "int", "uint", "float"
        /// </summary>
        public PixelEncoding Encoding { get; }  

        /// <summary>
        /// Initializes an Instance of ImagePixelChannel
        /// </summary>
        /// <param name="firstBit"></param>
        /// <param name="numBits"></param>
        /// <param name="encoding"></param>
        public ImagePixelChannel(int firstBit, int numBits, PixelEncoding encoding)
        {
            FirstBit = firstBit;
            NumBits = numBits;
            Encoding = encoding;
        }
    }
}