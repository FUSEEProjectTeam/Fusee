using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Fusee.Engine
{
    // Not documented because this class is not approved yet.
    class Compression
    {
        public static byte[] SerializeAndCompress(object obj)
        {
            using (var ms = new MemoryStream())
            {
                using (var zs = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(zs, obj);
                }
                return ms.ToArray();
            }
        }

        public static object DecompressAndDeserialze(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var zs = new GZipStream(ms, CompressionMode.Decompress, true))
                {
                    var bf = new BinaryFormatter();
                    return bf.Deserialize(zs);
                }
            }
        }
    }
}
