using Xunit;
using System.Drawing;
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
            Program.setExample(new Fusee.Examples.AdvancedUI.Core.AdvancedUI());
            Program.Main("AdvancedUITest.png");

            var referenceIm = new Bitmap(@"References\AdvancedUI.png");
            var testIm = new Bitmap("AdvancedUITest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.10f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void BoneAnimationTest()
        {
            Program.setExample(new Fusee.Examples.Bone.Core.Bone());
            Program.Main("BoneAnimationTest.png");

            var referenceIm = new Bitmap(@"References\BoneAnimation.png");
            var testIm = new Bitmap("BoneAnimationTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void BumpMappingTest()
        {
            Program.setExample(new Fusee.Examples.Bump.Core.Bump());
            Program.Main("BumpMappingTest.png");

            var referenceIm = new Bitmap(@"References\BumpMapping.png");
            var testIm = new Bitmap("BumpMappingTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.98f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void GeometryEditingTest()
        {
            Program.setExample(new Fusee.Examples.GeometryEditing.Core.GeometryEditing());
            Program.Main("GeometryEditingTest.png");

            var referenceIm = new Bitmap(@"References\GeometryEditing.png");
            var testIm = new Bitmap("GeometryEditingTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void MeshingAroundTest()
        {
            Program.setExample(new Fusee.Examples.MeshingAround.Core.MeshingAround());
            Program.Main("MeshingAroundTest.png");

            var referenceIm = new Bitmap(@"References\MeshingAround.png");
            var testIm = new Bitmap("MeshingAroundTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void PickingTest()
        {
            Program.setExample(new Fusee.Examples.Picking.Core.Picking());
            Program.Main("PickingTest.png");

            var referenceIm = new Bitmap(@"References\Picking.png");
            var testIm = new Bitmap("PickingTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void SimpleTest()
        {            
            Program.setExample(new Fusee.Examples.Simple.Core.Simple());
            Program.Main("SimpleTest.png");

            var referenceIm = new Bitmap(@"References\Simple.png");
            var testIm = new Bitmap("SimpleTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.98f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void SimpleDeferredTest()
        {
            Program.setExample(new Fusee.Examples.SimpleDeferred.Core.SimpleDeferred());
            Program.Main("SimpleDeferredTest.png");

            var referenceIm = new Bitmap(@"References\SimpleDeferred.png");
            var testIm = new Bitmap("SimpleDeferredTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.01f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void ThreeDFontTest()
        {
            Program.setExample(new Fusee.Examples.ThreeDFont.Core.ThreeDFont());
            Program.Main("ThreeDFontTest.png");

            var referenceIm = new Bitmap(@"References\ThreeDFont.png");
            var testIm = new Bitmap("ThreeDFontTest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void UITest()
        {
            Program.setExample(new Fusee.Examples.UI.Core.UI());
            Program.Main("UITest.png");

            var referenceIm = new Bitmap(@"References\UI.png");
            var testIm = new Bitmap("UITest.png");

            var percent = compareImage(referenceIm, testIm);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }


        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static float compareImage(Bitmap referenceIm, Bitmap testIm)
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
