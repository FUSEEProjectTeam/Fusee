using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// PickComponent
    /// </summary>
    public class PickComponent : SceneComponent
    {
        /// <summary>
        /// Pick layer, on picking the result with the higher layer will be preferred
        /// </summary>
        public int PickLayer { get; set; }

        /// <summary>
        /// Possibility to deposit a custom method on how to pick the following mesh(es)
        /// Check visitor module for new mesh types
        /// Passes current <see cref="Mesh"/>,
        /// <see cref="RenderContext.ModelViewProjection"/>,
        /// Pick position in clip space
        /// </summary>
        public Func<Mesh, SceneNode, float4x4, float4x4, float4x4, float2, PickResult>? CustomPickMethod;
    }
}