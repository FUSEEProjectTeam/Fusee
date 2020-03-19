using System;
using System.IO;
using Xunit;
using Fusee.Serialization.V1;

namespace Fusee.Serialization.PyTest
{
    public class TestScene
    {
               
        [Fact]
        public void LoadTestFile()
        {
            FusFile file = Protobuf.Serializer.Deserialize<FusFile>(new FileStream("Test.fus", FileMode.Open));
        }
    }
}
