using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

// Unity3D-Anaglyph-izer translated for Fusee
// source: https://github.com/EsimpleStudios/Unity3D-Anaglyph-izer

namespace Examples.CubeAndTiles
{
    internal class S3DV
    {
        internal static float EyeDistance = 80f;
        internal static float FocalDistance = 3000f;
    }

    public class Anaglyph3D
    {
        internal float4x4 LeftEye { get; private set; }
        internal float4x4 RightEye { get; private set; }

        // KeyCodes
        private const KeyCodes DownEyeDistance = KeyCodes.O;
        private const KeyCodes UpEyeDistance = KeyCodes.P;
        private const KeyCodes DownFocalDistance = KeyCodes.K;
        private const KeyCodes UpFocalDistance = KeyCodes.L;

        // Constructor
        public Anaglyph3D()
        {
            LeftEye = float4x4.LookAt(-S3DV.EyeDistance, 0, 3000, 0, 0, 0, 0, 1, 0);
            RightEye = float4x4.LookAt(+S3DV.EyeDistance, 0, 3000, 0, 0, 0, 0, 1, 0);
        }
       
        public void UpdateEyeDistance(Input key)
        {
            // o and p
            const float eyeDistanceAdjust = 1f;

            if (key.IsKeyDown(UpEyeDistance))
                S3DV.EyeDistance += eyeDistanceAdjust;

            if (key.IsKeyDown(DownEyeDistance))
                S3DV.EyeDistance -= eyeDistanceAdjust;

            // k and l
            const float focalDistanceAdjust = 1f;

            if (key.IsKeyDown(UpFocalDistance))
                S3DV.FocalDistance += focalDistanceAdjust;

            if (key.IsKeyDown(DownFocalDistance))
                S3DV.FocalDistance -= focalDistanceAdjust;

            LeftEye = float4x4.LookAt(-S3DV.EyeDistance, 0, S3DV.FocalDistance, 0, 0, 0, 0, 1, 0);
            RightEye = float4x4.LookAt(+S3DV.EyeDistance, 0, S3DV.FocalDistance, 0, 0, 0, 0, 1, 0);  
        }

        /* ProjectionMatrix
        private static float4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            var x = (2.0f*near)/(right - left);
            var y = (2.0f*near)/(top - bottom);

            var a = (right + left)/(right - left);
            var b = (top + bottom)/(top - bottom);
            var c = -(far + near)/(far - near);
            var d = -(2.0f*far*near)/(far - near);

            const float e = -1.0f;

            var m = new float4x4(x, 0f, a, 0f, 0f, y, b, 0f, 0f, 0f, c, d, 0f, 0f, e, 0f);

            return m;
        }

        private float4x4 ProjectionMatrix(bool isLeftEye)
        {
            // temporary (missing in camera)
            const float nearClipPlane = 1f;
            const float farClipPlane = 10000f;
            const float fieldOfView = 45f;
            var aspect = (float) _Width/_Height;

            // convert FOV to radians
            const double fov = fieldOfView/180.0f*Math.PI; 

            var a = (float) (nearClipPlane*Math.Tan(fov*0.5f));
            var b = nearClipPlane/S3DV.FocalDistance;

            float left;
            float right;

            if (isLeftEye)
            {
                // left camera
                left = (- aspect*a + (S3DV.EyeDistance)*b);
                right = (aspect*a + (S3DV.EyeDistance)*b);
            }
            else 
            {
                // right camera
                left = - aspect*a - (S3DV.EyeDistance)*b;
                right = aspect*a - (S3DV.EyeDistance)*b;
            }

            return PerspectiveOffCenter(left, right, -a, a, nearClipPlane, farClipPlane);
        }
         */
    }
}