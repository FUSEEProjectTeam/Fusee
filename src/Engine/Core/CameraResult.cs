using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
{
    public struct CameraResult : IEquatable<CameraResult>
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

        public override bool Equals(object obj)
        {
            if ((obj is null) || !GetType().Equals(obj.GetType()))
                return false;
            else
                return Equals((CameraResult)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Camera.GetHashCode(), View.Row1.GetHashCode(), View.Row2.GetHashCode(), View.Row3.GetHashCode(), View.Row4.GetHashCode());
        }
    }
}