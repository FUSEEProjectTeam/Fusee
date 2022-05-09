using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using Xunit.Abstractions;

namespace Fusee.Tests.Render.Desktop
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

            var referenceIm = Image.Load(@"References\AdvancedUI.png");
            //var referenceIm = Image.Load(@"References\AdvancedUI.png");
            var testIm = Image.Load("AdvancedUITest.png");
            //var testIm = Image.Load("AdvancedUITest.png");            
            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        //[Fact]
        //public void BoneAnimationTest()
        //{
        //    Program.Example = new Fusee.Examples.Bone.Core.Bone();
        //    Program.Init("BoneAnimationTest.png");

        //    var referenceIm = Image.Load(@"References\BoneAnimation.png");
        //    var testIm = Image.Load("BoneAnimationTest.png");

        //    var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

        //    Assert.InRange(percent, 0.99f, 1f);
        //    output.WriteLine(percent.ToString());
        //}

        [Fact]
        public void GeometryEditingTest()
        {
            Program.Example = new Fusee.Examples.GeometryEditing.Core.GeometryEditing();
            Program.Init("GeometryEditingTest.png");

            var referenceIm = Image.Load(@"References\GeometryEditing.png");
            var testIm = Image.Load("GeometryEditingTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void MeshingAroundTest()
        {
            Program.Example = new Fusee.Examples.MeshingAround.Core.MeshingAround();
            Program.Init("MeshingAroundTest.png");

            var referenceIm = Image.Load(@"References\MeshingAround.png");
            var testIm = Image.Load("MeshingAroundTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void PickingTest()
        {
            Program.Example = new Fusee.Examples.Picking.Core.Picking();
            Program.Init("PickingTest.png");

            var referenceIm = Image.Load(@"References\Picking.png");
            var testIm = Image.Load("PickingTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void SimpleTest()
        {
            Program.Example = new Fusee.Examples.Simple.Core.Simple();
            Program.Init("SimpleTest.png");

            var referenceIm = Image.Load(@"References\Simple.png");
            var testIm = Image.Load("SimpleTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.98f, 1f);
            output.WriteLine(percent.ToString());
        }

        //[Fact]
        //public void SimpleDeferredTest()
        //{
        //    Program.Example = new Fusee.Examples.SimpleDeferred.Core.SimpleDeferred();
        //    Program.Init("SimpleDeferredTest.png");

        //    var referenceIm = Image.Load(@"References\SimpleDeferred.png");
        //    var testIm = Image.Load("SimpleDeferredTest.png");

        //    var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

        //    Assert.InRange(percent, 0.01f, 1f);
        //    output.WriteLine(percent.ToString());
        //}

        [Fact]
        public void ThreeDFontTest()
        {
            Program.Example = new Fusee.Examples.ThreeDFont.Core.ThreeDFont();
            Program.Init("ThreeDFontTest.png");

            var referenceIm = Image.Load(@"References\ThreeDFont.png");
            var testIm = Image.Load("ThreeDFontTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        [Fact]
        public void UITest()
        {
            Program.Example = new Fusee.Examples.UI.Core.UI();
            Program.Init("UITest.png");

            var referenceIm = Image.Load(@"References\UI.png");
            var testIm = Image.Load("UITest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.InRange(percent, 0.99f, 1f);
            output.WriteLine(percent.ToString());
        }

        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static float CompareImage(Image<Rgba32> referenceIm, Image<Rgba32> testIm)
        {
            var count = 0;

            for (int x = 0; x < System.Math.Min(referenceIm.Width, testIm.Width); x++)
            {
                for (int y = 0; y < System.Math.Min(referenceIm.Height, testIm.Height); y++)
                {
                    if (!testIm[x, y].Equals(referenceIm[x, y]))
                        count++;
                }
            }

            return 1 - ((float)count / (referenceIm.Height * referenceIm.Width));
        }
    }
}