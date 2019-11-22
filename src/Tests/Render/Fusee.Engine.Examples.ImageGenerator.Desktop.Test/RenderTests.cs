using System;
using Xunit;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Fusee.Engine.Examples.ImageGenerator.Desktop
{
    public class RenderTests
    {
        private static string fuseeRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "..", "..", "..", "..", ".."));
        private static string referencePath = Path.GetFullPath(Path.Combine(fuseeRoot, @"src\Tests\Render\Fusee.Engine.Examples.ImageGenerator.Desktop.Test\References"));
        private static string imagePath = Path.GetFullPath(Path.Combine(fuseeRoot, @"bin\Debug\Tests\Render\ImageGenerator.Test"));

        [Fact]
        public void AdvancedUITest()
        {
            Program.setExample(new Fusee.Examples.AdvancedUI.Core.AdvancedUI());
            Program.Main(new string[] { "AdvancedUITest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "AdvancedUI.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "AdvancedUITest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void BoneAnimationTest()
        {
            Program.setExample(new Fusee.Examples.Bone.Core.Bone());
            Program.Main(new string[] { "BoneAnimationTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "BoneAnimation.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "BoneAnimationTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void BumpMappingTest()
        {
            Program.setExample(new Fusee.Examples.Bump.Core.Bump());
            Program.Main(new string[] { "BumpMappingTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "BumpMapping.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "BumpMappingTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void GeometryEditingTest()
        {
            Program.setExample(new Fusee.Examples.GeometryEditing.Core.GeometryEditing());
            Program.Main(new string[] { "GeometryEditingTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "GeometryEditing.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "GeometryEditingTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void MeshingAroundTest()
        {
            Program.setExample(new Fusee.Examples.MeshingAround.Core.MeshingAround());
            Program.Main(new string[] { "MeshingAroundTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "MeshingAround.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "MeshingAroundTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void PickingTest()
        {
            Program.setExample(new Fusee.Examples.Picking.Core.Picking());
            Program.Main(new string[] { "PickingTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "Picking.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "PickingTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void SimpleTest()
        {            
            Program.setExample(new Fusee.Examples.Simple.Core.Simple());
            Program.Main(new string[] { "SimpleTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "Simple.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "SimpleTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void SimpleDeferredTest()
        {
            Program.setExample(new Fusee.Examples.SimpleDeferred.Core.SimpleDeferred());
            Program.Main(new string[] { "SimpleDeferredTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "SimpleDeferred.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "SimpleDeferredTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void ThreeDFontTest()
        {
            Program.setExample(new Fusee.Examples.ThreeDFont.Core.ThreeDFont());
            Program.Main(new string[] { "ThreeDFontTest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "ThreeDFont.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "ThreeDFontTest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }

        [Fact]
        public void UITest()
        {
            Program.setExample(new Fusee.Examples.UI.Core.UI());
            Program.Main(new string[] { "UITest.png" });

            var referenceIm = new Bitmap(Path.Combine(referencePath, "UI.png"));
            var testIm = new Bitmap(Path.Combine(imagePath, "UITest.png"));

            var percent = compareImage(referenceIm, testIm);

            Assert.True(percent <= 1);
        }


        /// <summary>
        /// The function compares two images and returns how many pixels are different from another (in percent),
        /// </summary>
        /// <param name="referenceIm">The reference image to compare to.</param>
        /// <param name="testIm">The image that is to be compared.</param>
        /// <returns>The percentage of pixels not the same in the two images.</returns>
        private static int compareImage(Bitmap referenceIm, Bitmap testIm)
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

            return (count * 100) / (referenceIm.Height * referenceIm.Width);
        }
    }
}
