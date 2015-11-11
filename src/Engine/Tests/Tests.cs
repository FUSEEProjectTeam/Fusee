

using Fusee.Tests.Math.Core;
using Fusee.Tests.Scene.Core;

namespace Fusee.Tests
{
    class Tests
    {
        public static void Main(string[] args)
        {
            // Scene
            VisitorTests.BasicEnumeratorTests();
            VisitorTests.BasicVisitorTest();
            VisitorTests.BasicViseratorTest();

            // Math
            QuaternionTests.EulerAngleConversion();



        }

    }
}
