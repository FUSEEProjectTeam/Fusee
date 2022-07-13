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
        [Test]
        public void AdvancedUITest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "AdvancedUI.png");
            var testImagePath = Path.Combine(Program.FilePath, "AdvancedUI.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void CameraTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Camera.png");
            var testImagePath = Path.Combine(Program.FilePath, "Camera.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void FractalTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Fractal.png");
            var testImagePath = Path.Combine(Program.FilePath, "Fractal.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void DeferredTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Deferred.png");
            var testImagePath = Path.Combine(Program.FilePath, "Deferred.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void GeometryEditingTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "GeometryEditing.png");
            var testImagePath = Path.Combine(Program.FilePath, "GeometryEditing.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void LabyrinthTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Labyrinth.png");
            var testImagePath = Path.Combine(Program.FilePath, "Labyrinth.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void MaterialsTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Materials.png");
            var testImagePath = Path.Combine(Program.FilePath, "Materials.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void MeshingAroundTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "MeshingAround.png");
            var testImagePath = Path.Combine(Program.FilePath, "MeshingAround.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void PickingTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Picking.png");
            var testImagePath = Path.Combine(Program.FilePath, "Picking.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void PointCloudPotree2Test()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "PointCloudPotree2.png");
            var testImagePath = Path.Combine(Program.FilePath, "PointCloudPotree2.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void RenderContextOnlyTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "RenderContextOnly.png");
            var testImagePath = Path.Combine(Program.FilePath, "RenderContextOnly.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void RenderLayerTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "RenderLayer.png");
            var testImagePath = Path.Combine(Program.FilePath, "RenderLayer.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void SimpleTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "Simple.png");
            var testImagePath = Path.Combine(Program.FilePath, "Simple.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void ThreeDFontTest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "ThreeDFont.png");
            var testImagePath = Path.Combine(Program.FilePath, "ThreeDFont.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        [Test]
        public void UITest()
        {
            var referenceImagePath = Path.Combine(Program.FilePath, "References", "UI.png");
            var testImagePath = Path.Combine(Program.FilePath, "UI.png");

            var result = CompareImage(referenceImagePath, testImagePath);

            TestContext.WriteLine($"Number of pixels: {result.pixelCount}\nNumber of different pixels: {result.differentPixelCount}\nAverage difference: {result.averageDifference}\nMaximum difference: {result.maximumDifference}\n");
            Assert.LessOrEqual(result.averageDifference, 0.01f);
        }

        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static (float averageDifference, float maximumDifference, int differentPixelCount, int pixelCount) CompareImage(string referenceImagePath, string testImagePath)
        {
            var countDiff = 0;

            var maxDiff = 0.0f;
            var meanDiff = 0.0f;

            if (!File.Exists(referenceImagePath) || !File.Exists(testImagePath))
            { return (999, 999, 999, 999); }

            Image<Rgba32> referenceIm = Image.Load(referenceImagePath) as Image<Rgba32>;
            Image<Rgba32> testIm = Image.Load(testImagePath) as Image<Rgba32>;

            if (referenceIm.Width != testIm.Width || referenceIm.Height != testIm.Height)
            { return (999, 999, 999, 999); }

            for (int x = 0; x < System.MathF.Min(referenceIm.Width, testIm.Width); x++)
            {
                for (int y = 0; y < System.MathF.Min(referenceIm.Height, testIm.Height); y++)
                {
                    var diffR = System.MathF.Abs(testIm[x, y].R - referenceIm[x, y].R);
                    var diffG = System.MathF.Abs(testIm[x, y].G - referenceIm[x, y].G);
                    var diffB = System.MathF.Abs(testIm[x, y].B - referenceIm[x, y].B);

                    var diff = ((diffR + diffG + diffB) / 3f) / 255f;

                    if (!(diff == 0.0f))
                    {
                        countDiff++;
                        if (maxDiff < diff)
                            maxDiff = diff;

                        if (meanDiff == 0.0f)
                        {
                            meanDiff = diff;
                        }
                        else
                        {
                            meanDiff = (meanDiff + diff) / 2f;
                        }
                    }
                }
            }

            return (meanDiff, maxDiff, countDiff, referenceIm.Width * referenceIm.Height);
        }
    }
}