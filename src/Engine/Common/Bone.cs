using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Marker component (contains no data). If contained within a node, the node 
    /// serves as a bone in a bone animation.
    /// </summary>
    public class Bone : SceneComponent
    {
        /// <summary>
        /// The name of this component.
        /// </summary>
        public string Name;
    }
}
