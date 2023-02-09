using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Desktop
{
    public static class SnapshotController
    {
        public static void SaveSnapshotAsPng(byte[] frame, int width, int height, string path, string filename)
        {
            var img = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(frame, width, height);
            img.SaveAsPng(Path.Combine(path, filename));
        }

    }
}
