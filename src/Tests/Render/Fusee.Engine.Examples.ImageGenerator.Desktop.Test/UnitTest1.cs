using System;
using Xunit;
using System.Drawing;

namespace Fusee.Engine.Examples.ImageGenerator.Desktop.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var reference = new Bitmap(@"D:\Repos\Fusee\src\Tests\Render\ImageGenerator\Desktop\Assets\reference.png");
            var image = new Bitmap(@"D:\Repos\Fusee\bin\Debug\Tests\Render\ImageGenerator\shoottest.png");

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
