using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Defines methods for creating the SSAO kernel and the noise texture.
    /// </summary>
    public static class SSAOHelper
    {
        /// <summary>
        /// Creates a hemispherical kernel with more samples closer to the center. 
        /// </summary>
        /// <param name="kernelSize">Number of samples.</param>
        /// <returns></returns>
        //see: http://john-chapman-graphics.blogspot.com/2013/01/ssao-tutorial.html
        public static float3[] CreateKernel(int kernelSize)
        {
            var rnd = new Random();

            var kernel = new float3[kernelSize];

            for (int i = 0; i < kernelSize; ++i)
            {
                kernel[i] = new float3
                (
                    (float)rnd.NextDouble() * 2.0f - 1.0f,
                    (float)rnd.NextDouble() * 2.0f - 1.0f,
                    (float)rnd.NextDouble()
                );

                kernel[i].Normalize();

                kernel[i] *= (float)rnd.NextDouble();

                float scale = i / kernelSize;
                scale = M.Lerp(0.1f, 1.0f, scale * scale);
                kernel[i] *= scale;
            }

            return kernel;
        }

        /// <summary>
        /// Creates a noise texture.
        /// </summary>
        /// <param name="texSize">Width and height of the texture. </param>
        /// <returns></returns>
        public static Texture CreateNoiseTex(int texSize)
        {
            var ssaoNoise = SSAONoise(texSize * texSize);
            var pxData = new List<byte>(); //4 bytes per float, 3 floats per float3

            for (int i = 0; i < ssaoNoise.Length; i++)
            {
                var noise = ssaoNoise[i];
                var bytesX = BitConverter.GetBytes(noise.x);
                var bytesY = BitConverter.GetBytes(noise.y);
                var bytesZ = BitConverter.GetBytes(noise.z);

                pxData.AddRange(bytesX);
                pxData.AddRange(bytesY);
                pxData.AddRange(bytesZ);
            }

            return new Texture(new ImageData(pxData.ToArray(), texSize, texSize, new ImagePixelFormat(ColorFormat.fRGB16)), true, Common.TextureFilterMode.NEAREST);
        }

        private static float3[] SSAONoise(int noiseSize) //should be a multiple of 4...
        {
            var rnd = new Random();

            var noise = new float3[noiseSize];

            for (int i = 0; i < noiseSize; ++i)
            {
                noise[i] = new float3
                (
                    (float)rnd.NextDouble() * 2.0f - 1.0f,
                    (float)rnd.NextDouble() * 2.0f - 1.0f,
                    0.0f
                );
            }

            return noise;
        }
    }
}
