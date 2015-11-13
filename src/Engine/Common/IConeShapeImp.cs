using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of a cone-shaped collision shape
    /// </summary>
    public interface IConeShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets or sets the index of the cone up.
        /// </summary>
        /// <value>
        /// The index of the cone up.
        /// </value>
        int ConeUpIndex { get; set; }
        /// <summary>
        /// Retrieves the cone' height.
        /// </summary>
        /// <value>
        /// The height of the cone.
        /// </value>
        float Height { get; }
        /// <summary>
        /// Gets the cone's radius.
        /// </summary>
        /// <value>
        /// The radius of the cone.
        /// </value>
        float Radius { get; }
    }
}
