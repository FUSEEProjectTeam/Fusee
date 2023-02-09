using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.DataTransformation
{
    public class SixLabsEncodingMeta : EncodingMeta
    {
        public Configuration configuration;
        public int byteLength = 0;

        public SixLabsEncodingMeta(Configuration configuration, int byteLength)
        {
            this.configuration = configuration;
            this.byteLength = byteLength;
        }
    }
}
