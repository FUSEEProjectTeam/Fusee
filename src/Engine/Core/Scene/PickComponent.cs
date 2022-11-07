using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// PickComponent
    /// </summary>
    public class PickComponent : SceneComponent
    {
        /// <summary>
        /// Pick layer, on picking the result with the higher layer will be prefered
        /// </summary>
        public int PickLayer { get; set; }

        /// <summary>
        /// Posibility to deposit a custom method on how to pick the following mesh(es)
        /// Check visitor module for new mesh types
        /// </summary>
        public Func<PickResult>? CustomPickMethod;
    }
}
