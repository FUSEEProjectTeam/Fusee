using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
{
    internal struct CameraResult : IEquatable<CameraResult>
    {
        public Camera Camera { get; private set; }

        public float4x4 View { get; private set; }

        public CameraResult(Camera cam, float4x4 view)
        {
            Camera = cam;
            View = view;
        }

        public bool Equals(CameraResult other)
        {
            return Camera == other.Camera && View == other.View;
        }

        public static bool operator ==(CameraResult left, CameraResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CameraResult left, CameraResult right)
        {
            return left.Equals(right);
        }
    }
}