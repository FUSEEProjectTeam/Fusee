using Xunit;

namespace Fusee.Math.Core
{
    public class QuaternionTest
    {
        [Fact]
        public void IdentityIsZero()
        {
            Assert.Equal(new Quaternion(0, 0, 0, 1), Quaternion.Identity);
        }

        [Fact]
        public void Float3000IsIdentity()
        {
            var q = Quaternion.EulerToQuaternion(new float3(0, 0, 0));
            Assert.Equal(Quaternion.Identity, q);
        }
    }
}
