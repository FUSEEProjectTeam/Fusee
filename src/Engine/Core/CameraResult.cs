using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// One camera result found after a scene traversal
    /// </summary>
    public struct CameraResult : IEquatable<CameraResult>
    {
        /// <summary>
        /// The camera object
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// The view matrix associated with the found camera
        /// </summary>
        public float4x4 View { get; private set; }

        /// <summary>
        /// Generate a camera result instance from given camera object and view matrix
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="view"></param>
        public CameraResult(Camera cam, float4x4 view)
        {
            Camera = cam;
            View = view;
        }

        /// <inheritdoc/>
        public readonly bool Equals(CameraResult other)
        {
            return Camera == other.Camera && View == other.View;
        }

        /// <inheritdoc/>
        public static bool operator ==(CameraResult? left, CameraResult? right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(CameraResult? left, CameraResult? right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Check if two <see cref="CameraResult"/>s are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override readonly bool Equals(object? obj)
        {
            if ((obj is null) || !GetType().Equals(obj.GetType()))
                return false;
            else
                return Equals((CameraResult)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Camera.GetHashCode(), View.Row1.GetHashCode(), View.Row2.GetHashCode(), View.Row3.GetHashCode(), View.Row4.GetHashCode());
        }
    }
}