using System;
using Xunit;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Fusee.Engine.Examples.ImageGenerator.Desktop
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string fuseeRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            fuseeRoot = Path.GetFullPath(Path.Combine(fuseeRoot, "..", "..", "..", "..", "..")); ; 
            string referencePath = Path.GetFullPath(Path.Combine(fuseeRoot, @"src\Tests\Render\Fusee.Engine.Examples.ImageGenerator.Desktop.Test\References\reference.png"));
            string imagePath = Path.GetFullPath(Path.Combine(fuseeRoot, @"bin\Debug\Tests\Render\ImageGenerator.Test\shoottest.png"));

            Program.Main(new string[] { "shoottest.png" });

            var reference = new Bitmap(referencePath);
            var image = new Bitmap(imagePath);

            var count = 0;

            for (int x = 0; x < System.Math.Min(reference.Width, image.Width); x++)
            {
                for (int y = 0; y < System.Math.Min(reference.Height, image.Height); y++)
                {
                    if (!image.GetPixel(x, y).Equals(reference.GetPixel(x, y)))
                        count++;
                }
            }

            var percent = (count * 100) / (reference.Height * reference.Width);

            Assert.True(percent <= 1);
        }
    }
}
