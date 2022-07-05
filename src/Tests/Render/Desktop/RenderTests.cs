using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.IO;

namespace Fusee.Tests.Render.Desktop
{

    [SetUpFixture]
    public class SetupTrace
    {
        [OneTimeSetUp]
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

        //[Test]
        //public void AdvancedUITest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "AdvancedUI.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "AdvancedUI.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void CameraTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Camera.png");
            var testImagePath = Path.Combine(Program.FilePath, "Camera.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void FractalTest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "Fractal.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "Fractal.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        //[Test]
        //public void DeferredTest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "Deferred.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "Deferred.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}


        [Test]
        public void GeometryEditingTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "GeometryEditing.png");
            var testImagePath = Path.Combine(Program.FilePath, "GeometryEditing.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void LabyrinthTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Labyrinth.png");
            var testImagePath = Path.Combine(Program.FilePath, "Labyrinth.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void MaterialsTest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "Materials.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "Materials.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void MeshingAroundTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "MeshingAround.png");
            var testImagePath = Path.Combine(Program.FilePath, "MeshingAround.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void PickingTest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "Picking.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "Picking.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        //[Test]
        //public void PointCloudPotree2Test()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "PointCloudPotree2.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "PointCloudPotree2.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void RenderContextOnlyTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "RenderContextOnly.png");
            var testImagePath = Path.Combine(Program.FilePath, "RenderContextOnly.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        [Test]
        public void RenderLayerTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "RenderLayer.png");
            var testImagePath = Path.Combine(Program.FilePath, "RenderLayer.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void SimpleTest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "Simple.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "Simple.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        [Test]
        public void ThreeDFontTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "ThreeDFont.png");
            var testImagePath = Path.Combine(Program.FilePath, "ThreeDFont.png");

            var percent = CompareImage(referenceImagePath, testImagePath);

            Assert.GreaterOrEqual(percent, 0.98f);
            TestContext.WriteLine(percent.ToString());
        }

        //[Test]
        //public void UITest()
        //{
        //    var referenceImagePath = Path.Combine(Program.FilePath, "References", "UI.png");
        //    var testImagePath = Path.Combine(Program.FilePath, "UI.png");

        //    var percent = CompareImage(referenceImagePath, testImagePath);

        //    Assert.GreaterOrEqual(percent, 0.98f);
        //    TestContext.WriteLine(percent.ToString());
        //}

        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static float CompareImage(string referenceImagePath, string testImagePath)
        {
            var count = 0;

            if (!File.Exists(referenceImagePath) || !File.Exists(testImagePath))
            { return -1; }

            Image<Rgba32> referenceIm = Image.Load(referenceImagePath) as Image<Rgba32>;
            Image<Rgba32> testIm = Image.Load(testImagePath) as Image<Rgba32>;

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