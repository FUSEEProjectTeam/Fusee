using System;
using System.Drawing;

namespace BitmapCS
{
    class Program
    {
        public static int[,] Bmp()
        {
            int x = 25;
            int y = 25;

            int posX = 0;
            int posY = 0;

            Bitmap img = new Bitmap(@"Assets/" + "Maze.bmp");
            int[,] image = new int[img.Width / x, img.Height / y];
            for (int j = 5; j < img.Height; j += y)
            {
                for (int i = 5; i < img.Width; i += x)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (pixel.Equals(Color.FromArgb(255,0,0,0)))
                    {
                        image[posY, posX] = 1;
                    }
                    else if(pixel.Equals(Color.FromArgb(255,0,255,0)))
                    {
                        image[posY, posX] = -1;
                    }
                    else if (pixel.Equals(Color.FromArgb(255, 255, 0, 0)))
                    {
                        image[posY, posX] = 2;
                    }
                    else
                    {
                        image[posY, posX] = 0;
                    }
                    if(x == 25) { x++; } else { x--; }
                    posX += 1;
                    posX %= 31;
                }
                if (y == 25) { y++; } else { y--; }
                posY += 1;
                posY %= 31;
            }
            Console.WriteLine(image);
            return image;
        }
    }
}
