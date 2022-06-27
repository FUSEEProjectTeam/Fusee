using NUnit;
using NUnit.Framework;
using NUnitLite;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Threading;

namespace Fusee.Tests.Render.Desktop
{

    [SetUpFixture]
    public class SetupTrace
    {
        [OneTimeSetUp]
        [Apartment(ApartmentState.STA)]
        public void StartTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        [OneTimeTearDown]
        public void EndTest()
        {
            Trace.Flush();
        }
    }

    public class RenderTests
    {

        [Test]

        public void AdvancedUITest()
        {
            var referenceIm = Image.Load(@"References\AdvancedUI.png");
            //var referenceIm = Image.Load(@"References\AdvancedUI.png");
            var testIm = Image.Load("AdvancedUITest.png");
            //var testIm = Image.Load("AdvancedUITest.png");
            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);
            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void BoneAnimationTest()
        //{
        //
        //
        //
        //    var referenceIm = Image.Load(@"References\BoneAnimation.png");
        //    var testIm = Image.Load("BoneAnimationTest.png");
        //
        //    var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);
        //
        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void GeometryEditingTest()
        {


            var referenceIm = Image.Load(@"References\GeometryEditing.png");
            var testIm = Image.Load("GeometryEditingTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void MeshingAroundTest()
        {

            var referenceIm = Image.Load(@"References\MeshingAround.png");
            var testIm = Image.Load("MeshingAroundTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void PickingTest()
        {


            var referenceIm = Image.Load(@"References\Picking.png");
            var testIm = Image.Load("PickingTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void SimpleTest()
        {

            var referenceIm = Image.Load(@"References\Simple.png");
            var testIm = Image.Load("SimpleTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void SimpleDeferredTest()
        //{
        //    Program.Example = new Fusee.Examples.SimpleDeferred.Core.SimpleDeferred();
        //    Program.Init("SimpleDeferredTest.png");

        //    var referenceIm = Image.Load(@"References\SimpleDeferred.png");
        //    var testIm = Image.Load("SimpleDeferredTest.png");

        //    var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

        //    Assert.InRange(percent, 0.01f, 1f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void ThreeDFontTest()
        {

            var referenceIm = Image.Load(@"References\ThreeDFont.png");
            var testIm = Image.Load("ThreeDFontTest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void UITest()
        {

            var referenceIm = Image.Load(@"References\UI.png");
            var testIm = Image.Load("UITest.png");

            var percent = CompareImage(referenceIm as Image<Rgba32>, testIm as Image<Rgba32>);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
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