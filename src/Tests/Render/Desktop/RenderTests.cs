using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Fusee.Test.Render.Desktop
{
    public class RenderTests
    {
        private readonly ITestOutputHelper output;

        public RenderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AdvancedUITest()
        {
            Program.Example = new Fusee.Examples.AdvancedUI.Core.AdvancedUI() { rnd = new System.Random(12345) };
            Program.Init("AdvancedUITest.png");

            var referenceIm = new Bitmap(@"References\AdvancedUI.png");
            var testIm = new Bitmap("AdvancedUITest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        //[Fact]
        //public void BoneAnimationTest()
        //{
        //    Program.Example = new Fusee.Examples.Bone.Core.Bone();
        //    Program.Init("BoneAnimationTest.png");

        //    var referenceIm = new Bitmap(@"References\BoneAnimation.png");
        //    var testIm = new Bitmap("BoneAnimationTest.png");

        //    var percent = CompareImage(referenceIm, testIm);

        //    Assert.InRange(percent, 0.99f, 1f);
        //    output.WriteLine(percent.ToString());
        //}

        [Fact]
        public void BumpMappingTest()
        {
            Program.Example = new Fusee.Examples.NormalMap.Core.NormalMap();
            Program.Init("BumpMappingTest.png");

            var referenceIm = new Bitmap(@"References\BumpMapping.png");
            var testIm = new Bitmap("BumpMappingTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.98f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void GeometryEditingTest()
        {
            Program.Example = new Fusee.Examples.GeometryEditing.Core.GeometryEditing();
            Program.Init("GeometryEditingTest.png");

            var referenceIm = new Bitmap(@"References\GeometryEditing.png");
            var testIm = new Bitmap("GeometryEditingTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void MeshingAroundTest()
        {
            Program.Example = new Fusee.Examples.MeshingAround.Core.MeshingAround();
            Program.Init("MeshingAroundTest.png");

            var referenceIm = new Bitmap(@"References\MeshingAround.png");
            var testIm = new Bitmap("MeshingAroundTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void PickingTest()
        {
            Program.Example = new Fusee.Examples.Picking.Core.Picking();
            Program.Init("PickingTest.png");

            var referenceIm = new Bitmap(@"References\Picking.png");
            var testIm = new Bitmap("PickingTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void SimpleTest()
        {
            Program.Example = new Fusee.Examples.Simple.Core.Simple();
            Program.Init("SimpleTest.png");

            var referenceIm = new Bitmap(@"References\Simple.png");
            var testIm = new Bitmap("SimpleTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.98f, 1f);
            output.WriteLine(percent.ToString());
        }

        //[Fact]
        //public void SimpleDeferredTest()
        //{
        //    Program.Example = new Fusee.Examples.SimpleDeferred.Core.SimpleDeferred();
        //    Program.Init("SimpleDeferredTest.png");

        //    var referenceIm = new Bitmap(@"References\SimpleDeferred.png");
        //    var testIm = new Bitmap("SimpleDeferredTest.png");

        //    var percent = CompareImage(referenceIm, testIm);

        //    Assert.InRange(percent, 0.01f, 1f);
        //    output.WriteLine(percent.ToString());
        //}

        [Fact]
        public void ThreeDFontTest()
        {
            Program.Example = new Fusee.Examples.ThreeDFont.Core.ThreeDFont();
            Program.Init("ThreeDFontTest.png");

            var referenceIm = new Bitmap(@"References\ThreeDFont.png");
            var testIm = new Bitmap("ThreeDFontTest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void UITest()
        {
            Program.Example = new Fusee.Examples.UI.Core.UI();
            Program.Init("UITest.png");

            var referenceIm = new Bitmap(@"References\UI.png");
            var testIm = new Bitmap("UITest.png");

            var percent = CompareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static float CompareImage(Bitmap referenceIm, Bitmap testIm)
        {
            var count = 0;

            for (int x = 0; x < System.Math.Min(referenceIm.Width, testIm.Width); x++)
            {
                for (int y = 0; y < System.Math.Min(referenceIm.Height, testIm.Height); y++)
                {
                    if (!testIm.GetPixel(x, y).Equals(referenceIm.GetPixel(x, y)))
                        count++;
                }
            }

            return 1 - ((float)count / (referenceIm.Height * referenceIm.Width));
        }
    }
}