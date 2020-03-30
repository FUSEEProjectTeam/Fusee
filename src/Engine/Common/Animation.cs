﻿using System;
using System.Collections.Generic;

namespace Fusee.Engine.Common
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
        public List<AnimationTrack> AnimationTracks = new List<AnimationTrack>();

    }
}
