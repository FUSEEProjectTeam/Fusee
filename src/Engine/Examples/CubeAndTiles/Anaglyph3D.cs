using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

// Using code by Unity3D-Anaglyph-izer:
// source: https://github.com/EsimpleStudios/Unity3D-Anaglyph-izer

namespace Examples.CubeAndTiles
{
    internal class S3DV
    {
        internal static float EyeDistance = 35f;
        internal static float CamDistance = 3000f;
        internal static float Convergence = 0f;

        internal static float AspectRatio = 1.77778f;
        internal static float FieldOfView = MathHelper.PiOver4;

        internal static float NearClipping = 1f;
        internal static float FarClipping = 10000f;
    }

    public class Anaglyph3D
    {
        internal RenderContext RContext;
        private bool _isLeftEye;

        internal bool IsLeftEye
        {
            get { return _isLeftEye; }
            private set { _isLeftEye = value; }
        }

        private float4x4 _leftEye;
        private float4x4 _rightEye;

        // KeyCodes
        private const KeyCodes DownEyeDistance = KeyCodes.D;
        private const KeyCodes UpEyeDistance = KeyCodes.A;
        private const KeyCodes DownConvDistance = KeyCodes.Q;
        private const KeyCodes UpConvDistance = KeyCodes.E;
        private const KeyCodes DownCamDistance = KeyCodes.S;
        private const KeyCodes UpCamDistance = KeyCodes.W;

        // Constructor
        public Anaglyph3D(RenderContext rc)
        {
            RContext = rc;

            _leftEye = float4x4.LookAt(-S3DV.EyeDistance*0.5f, 0, S3DV.CamDistance, 0, 0, S3DV.Convergence, 0, 1, 0);
            _rightEye = float4x4.LookAt(+S3DV.EyeDistance*0.5f, 0, S3DV.CamDistance, 0, 0, S3DV.Convergence, 0, 1, 0);

            _isLeftEye = true;
        }

        // keyboard input
        public void UpdateEyeDistance(Input key)
        {
            const float camAdjust = 5f;

            if (key.IsKeyDown(UpCamDistance))
                S3DV.CamDistance = Math.Max(1500, S3DV.CamDistance - camAdjust);

            if (key.IsKeyDown(DownCamDistance))
                S3DV.CamDistance = Math.Min(3000, S3DV.CamDistance + camAdjust);
            
            var eyeDistanceAdjust = 1f/(Math.Max(1, Math.Abs(S3DV.Convergence/100)));

            if (key.IsKeyDown(UpEyeDistance))
                S3DV.EyeDistance += eyeDistanceAdjust;

            if (key.IsKeyDown(DownEyeDistance))
                S3DV.EyeDistance -= eyeDistanceAdjust;

            const float convergenceAdjust = 8f;

            if (key.IsKeyDown(UpConvDistance))
                S3DV.Convergence = Math.Min(S3DV.CamDistance - 500, S3DV.Convergence + convergenceAdjust);

            if (key.IsKeyDown(DownConvDistance))
                S3DV.Convergence = Math.Max(-2000, S3DV.Convergence - convergenceAdjust);

            S3DV.Convergence = Math.Min(S3DV.Convergence, S3DV.CamDistance - 500);

            _leftEye = float4x4.LookAt(-S3DV.EyeDistance * 0.5f, 0, S3DV.CamDistance, 0, 0, S3DV.Convergence, 0, 1, 0);
            _rightEye = float4x4.LookAt(+S3DV.EyeDistance * 0.5f, 0, S3DV.CamDistance, 0, 0, S3DV.Convergence, 0, 1, 0);
        }

        // switch between eyes
        public void SwitchEye()
        {
            RContext.Clear(ClearFlags.Depth);

            _isLeftEye = !_isLeftEye;

            if (_isLeftEye)
                RContext.ColorMask(true, false, false, false);
            else
                RContext.ColorMask(false, true, true, false);
        }

        public void NormalMode()
        {
            RContext.ColorMask(true, true, true, false);           
        }

        public float4x4 EyeTranslation()
        {
            return _isLeftEye ? _leftEye : _rightEye;
        }

        // Frustum
        public void SetFrustum(bool isLeftEye)
        {
            var topBottom = (float)(S3DV.NearClipping * Math.Tan(S3DV.FieldOfView * 0.5f));

            var a = (float) (S3DV.AspectRatio * Math.Tan(S3DV.FieldOfView * 0.5f) * S3DV.Convergence);
            var b = a - S3DV.EyeDistance / 2;
            var c = a + S3DV.EyeDistance / 2;

            float left;
            float right;

            if (isLeftEye)
            {
                // left camera
                left = -b * S3DV.NearClipping / S3DV.Convergence;
                right = c * S3DV.NearClipping / S3DV.Convergence;
            }
            else
            {
                // right camera
                left = -c * S3DV.NearClipping / S3DV.Convergence;
                right = b * S3DV.NearClipping / S3DV.Convergence;
            }
            
            RContext.Frustum(left, right, -topBottom, topBottom, S3DV.NearClipping, S3DV.FarClipping);
        }
    }
}