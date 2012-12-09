
using NUnit.Framework;
using Fusee.Math;

namespace Tests.Math.Core
{
    [TestFixture]
    class Double2Test
    {
        public double2 V1, V2, Res;
        [SetUp]
        public void Init()
        {
            V1 = new double2(5.5,5.5);
            V2 = new double2(1.5, 2.5);
        }

        [Test]
        public void Add()
        {
            Res = double2.Add(V1, V2);

            Assert.AreEqual(7, Res.x);
            Assert.AreEqual(8, Res.y);

            Res = double2.Add(V2, V1);

            Assert.AreEqual(7, Res.x);
            Assert.AreEqual(8, Res.y);

        }

    }
}
