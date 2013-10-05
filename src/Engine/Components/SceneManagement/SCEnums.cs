using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The Projection enums that are used by the Camera class.
/// </summary>
namespace Fusee.SceneManagement
{

    public enum Projection : int
    {
        /// <summary>
        /// Perspective = 0
        /// </summary>
        Perspective = 0,
        /// <summary>
        /// Orthographic = 1
        /// </summary>
        Orthographic = 1,
    }
}
