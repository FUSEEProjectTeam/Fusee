using System;
using WebAssembly;
using WebAssembly.Host;

namespace Fusee.Engine.Imp.Graphics.Wasm
{
    public class ImageData : JSHandler
    {
        public ImageData(ReadOnlySpan<byte> image, int imageWidth, int imageHeight)
        {
            using (var uint8ClampedArray = Uint8ClampedArray.From(image))
            {
                Handle = new HostObject("ImageData", uint8ClampedArray, imageWidth, imageHeight);
            }
        }
    }
}
