using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    internal struct CameraResult
    {
        public Camera Camera { get; private set; }

        public float4x4 View { get; private set; }

        public CameraResult(Camera cam, float4x4 view)
        {
            Camera = cam;
            View = view;
        }
    }

}