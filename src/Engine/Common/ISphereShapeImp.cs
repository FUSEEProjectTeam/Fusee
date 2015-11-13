using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of a spherical collision shape.
    /// </summary>
    public interface ISphereShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        float Radius { get; set; }
    }
}
