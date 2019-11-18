using System;
using System.Drawing;

namespace Bitmape
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 25;
            int y = 25;
            string str = AppDomain.CurrentDomain.BaseDirectory;
            Bitmap img = new Bitmap(str + "Maze.bmp");
            int[,] image = new int[img.Width, img.Height];
            for (int j = 5; j < img.Height; j += y)
            {
                Console.Write("{");
                for (int i = 5; i < img.Width; i += x)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (pixel.Equals(Color.FromArgb(255,0,0,0)))
                    {
                        image[i, j] = 1;
                    }
                    else if(pixel.Equals(Color.FromArgb(255,0,255,0)))
                    {
                        image[i, j] = -1;
                    }
                    else if (pixel.Equals(Color.FromArgb(255, 255, 0, 0)))
                    {
                        image[i, j] = 2;
                    }
                    else
                    {
                        image[i, j] = 0;
                    }
                    if(i + x > img.Width)
                    {
                        Console.Write(image[i, j]);
                    }
                    else
                    {
                        Console.Write(image[i, j] + ",");
                    }
                    if(x == 25) { x++; } else { x--; }
                }
                Console.Write("},");
                Console.WriteLine();
                if (y == 25) { y++; } else { y--; }
            }
        }
    }
}
