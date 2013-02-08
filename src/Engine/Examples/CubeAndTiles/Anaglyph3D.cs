using System;
using Fusee.Engine;
using Fusee.Math;

// Using code by Unity3D-Anaglyph-izer:
// source: https://github.com/EsimpleStudios/Unity3D-Anaglyph-izer

namespace Examples.CubeAndTiles
{
    internal class S3DV
    {
        internal static float EyeDistance = 35f;
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
        }

        // KeyCodes
        private const KeyCodes DownEyeDistance = KeyCodes.D;
        private const KeyCodes UpEyeDistance = KeyCodes.A;
        private const KeyCodes DownConvDistance = KeyCodes.Q;
        private const KeyCodes UpConvDistance = KeyCodes.E;

        // Constructor
        public Anaglyph3D(RenderContext rc)
        {
            RContext = rc;
            _isLeftEye = true;
        }

        // keyboard input
        public void UpdateEyeDistance(Input key)
        {
            var eyeDistanceAdjust = 1f/(Math.Max(1, Math.Abs(S3DV.Convergence/100)));

            if (key.IsKeyDown(UpEyeDistance))
                S3DV.EyeDistance += eyeDistanceAdjust;

            if (key.IsKeyDown(DownEyeDistance))
                S3DV.EyeDistance -= eyeDistanceAdjust;

            const float convergenceAdjust = 8f;

            if (key.IsKeyDown(UpConvDistance))
                S3DV.Convergence += convergenceAdjust;

            if (key.IsKeyDown(DownConvDistance))
                S3DV.Convergence -= convergenceAdjust;
        }

        // switch between eyes
        public void SwitchEye(bool isLeftEye)
        {
            _isLeftEye = !isLeftEye;
            SwitchEye();
        }

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

        // CameraTranslations
        public float4x4 LookAt3D(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ, float upX, float upY, float upZ)
        {
            return LookAt3D(new float3(eyeX, eyeY, eyeZ), new float3(targetX, targetY, targetZ), new float3(upX, upY, upZ));
        }

        public float4x4 LookAt3D(float3 eye, float3 target, float3 up)
        {
            var x = IsLeftEye ? eye.x - S3DV.EyeDistance : eye.x + S3DV.EyeDistance;

            var newEye = new float3(x, eye.y, eye.z);
            var newTarget = new float3(target.x, target.y, target.z + S3DV.Convergence);

            return float4x4.LookAt(newEye, newTarget, up);
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