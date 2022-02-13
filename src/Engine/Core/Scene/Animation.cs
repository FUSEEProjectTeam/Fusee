using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Contains animation data. Generally, a list of animation tracks
    /// </summary>
    public class Animation : SceneComponent
    {
        /// <summary>
        /// The animation tracks making up this animation data. 
        /// Each animation track controls a single value.
        /// </summary>
        public Xirkit.Animation animation = new Xirkit.Animation();

    }
}