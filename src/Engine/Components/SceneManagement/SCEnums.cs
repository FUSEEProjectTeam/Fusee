using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Fusee.SceneManagement
{
    /// <summary>
    /// The Projection enums that are used by the Camera class.
    /// </summary>
    public enum Projection : int
    {
        #region Enums
        /// <summary>
        /// Perspective = 0
        /// </summary>
        Perspective = 0,
        /// <summary>
        /// Orthographic = 1
        /// </summary>
        Orthographic = 1,
        #endregion
    }
}
