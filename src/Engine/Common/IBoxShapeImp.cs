using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Interface to abstract a box shape
    /// </summary>
    public interface IBoxShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets the half extents.
        /// </summary>
        /// <value>
        /// The half extents (half the length of widht, heigt and depth).
        /// </value>
        float3 HalfExtents { get; }
    }
}
