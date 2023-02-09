using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.SLIRP.Common;
using Fusee.SLIRP.DataTransformation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace Fusee.SLIRP.Desktop
{
    public class SixLabsJPEGTransformer : IEncoder
    {
        Configuration configuration;
        int byteLength = 0;

        public void Init(EncodingMeta metaData)
        {
            if (!(metaData is SixLabsEncodingMeta sixLabsMetaData))
                throw new InvalidCastException("Passed the wriong meta data to this encoder");

            configuration = sixLabsMetaData.configuration;
            byteLength = sixLabsMetaData.byteLength;

        }

        public Stream Encode(byte[] data, Stream dataStream, int width, int height)
        {
            if (configuration == null)
                throw new NullReferenceException("Configuration is null. Call \"Init()\" before using the encoding!");

            byteLength = data.Length;
            var img = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(data, width, height);

            JpegEncoder encoder = new JpegEncoder();
            encoder.Encode(img, dataStream);

            return dataStream;

        }

        public byte[] Decode(Stream data, int width, int height)
        {
            if (configuration == null)
                throw new NullReferenceException("No configuration set!");

            JpegDecoder decoder = new JpegDecoder();
            CancellationToken token = new CancellationToken();

            var img = decoder.Decode<Bgra32>(configuration, data, token);
            byte[] decodedData = new byte[byteLength];
            Span<byte> byteImg = new Span<byte>(decodedData);
            img.CopyPixelDataTo(byteImg);

            decodedData = byteImg.ToArray();

            return decodedData;

        }


    }
}
