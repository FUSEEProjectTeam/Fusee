using System;
using CrossSL.Meta;

namespace Example
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine(xSL<DiffuseShader>.FragmentShader);
            Console.WriteLine(xSL<DiffuseShader>.VertexShader);

            //var shader = xSL<MyShader>.ShaderObject;

            //shader.FUSEE_ITMV = new float4x4(/* ... */);
            //shader.FuUV = new float2(0, 0);
            //shader.FragmentShader();

            Console.ReadLine();
        }
    }
}