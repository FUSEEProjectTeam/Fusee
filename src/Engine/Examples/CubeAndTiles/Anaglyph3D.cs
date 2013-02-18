using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    internal class StereoParams
    {
        internal static float EyeDistance = 30f;
        internal static float Convergence = 0f;
    }

    public class Anaglyph3D
    {
        private readonly RenderContext _rContext;
        private readonly bool _zoomWithConvergence;

        internal bool IsLeftEye { get; private set; }

        // Constructor
        public Anaglyph3D(RenderContext rc, bool zoomWithConvergence = true)
        {
            _rContext = rc;

            IsLeftEye = true;
            _zoomWithConvergence = zoomWithConvergence;
        }

        // update values
        // ...

        // switch between eyes
        public void SwitchEye(bool isLeftEye)
        {
            IsLeftEye = !isLeftEye;
            SwitchEye();
        }

        public void SwitchEye()
        {
            if (_rContext == null) return;

            _rContext.Clear(ClearFlags.Depth);

            IsLeftEye = !IsLeftEye;

            if (IsLeftEye)
                _rContext.ColorMask(true, false, false, false);
            else
                _rContext.ColorMask(false, true, true, false);
        }

        public void NormalMode()
        {
            if (_rContext == null) return;
            
            _rContext.ColorMask(true, true, true, false);
        }

        // CameraTranslations
        public float4x4 LookAt3D(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ, float upX, float upY, float upZ)
        {
            return LookAt3D(new float3(eyeX, eyeY, eyeZ), new float3(targetX, targetY, targetZ), new float3(upX, upY, upZ));
        }

        public float4x4 LookAt3D(float3 eye, float3 target, float3 up)
        {
            var targZ = target.z;

            if (_zoomWithConvergence)
            {
                StereoParams.EyeDistance = 0.002f*eye.z - 5;
                targZ = eye.z - 500;
            }

            var x = IsLeftEye ? eye.x - StereoParams.EyeDistance : eye.x + StereoParams.EyeDistance;

            var newEye = new float3(x, eye.y, eye.z);
            var newTarget = new float3(target.x, target.y, targZ);

            return float4x4.LookAt(newEye, newTarget, up);
        }
    }
}