using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Common
{
    public struct BufferSet
    {
        public int x;
        public int y;

        public int width;
        public int height;

        public byte[] frameBuffer;
        public byte[] depthBuffer;

        public BufferSet(int x, int y, int width, int height, byte[] frameBuffer, byte[] depthBuffer)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.frameBuffer = frameBuffer;
            this.depthBuffer = depthBuffer;
        }
    }
}
