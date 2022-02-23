using Fusee.Xirkit;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Contains animation data. Generally, a list of animation tracks
    /// </summary>
    public class Animation : SceneComponent
    {
        /// <summary>
        /// The Xirkit Animation thats going to be filled in the FusSceneConverter class
        /// </summary>
        public AnimTimeline animation = new AnimTimeline();

    }
}