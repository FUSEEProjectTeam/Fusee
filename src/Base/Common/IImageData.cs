using System;
using System.Collections.Generic;

namespace Fusee.Base.Common
{
    // Interface describing what operations are possible on arbitrary image data
    public interface IImageData
    {
        // Taken from the existing ImageData:
        int Width { get; }
        int Height { get; }

        // Pixelformat description has changed to something more generic
        ImagePixelFormat PixelFormat { get; }


        // Write a block of pixels to this instance from some other IImageData
        void Blt(int xDst, int yDst, IImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0);
    
        // Expose a set of pixel lines (enables IImageData to be used as src in other instances' Blt)
        IEnumerator<ScanLine> ScanLines(int xSrc = 0, int ySrc = 0, int width = 0, int height = 0);

        bool IsEmpty { get; }


    }
}