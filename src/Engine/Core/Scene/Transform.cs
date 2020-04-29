using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Transformation (position, orientation and size) of the node.
    /// </summary>
    /// <seealso cref="SceneComponent" />
    public class Transform : SceneComponent
    {
        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        public float3 Translation;
        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public float3 Rotation;
        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        public float3 Scale = float3.One;
    }
}
