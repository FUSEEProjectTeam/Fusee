using Fusee.SLIRP.DataTransformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Common
{
    public interface IEncoder
    {
        public void Init(EncodingMeta metaData);

        public Stream Encode(byte[] data, Stream dataStream, int width, int height);

        public byte[] Decode(Stream data, int width, int height);
    }
}
