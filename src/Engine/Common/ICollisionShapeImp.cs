using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Implementation agnostic representation of a collision shape.
    /// </summary>
    public interface ICollisionShapeImp
    {
        /// <summary>
        /// Retrieves or sets the margin.
        /// </summary>
        /// <value>
        /// The size of the collision shape's margin.
        /// </value>
        float Margin { get; set; }
        /// <summary>
        /// Retrieves or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        float3 LocalScaling { get; set; }
        /// <summary>
        /// Gets or sets a user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        /// <remarks>
        /// User objects can be used to store application specific references 
        /// with the collisions shape. E.g. an object holding the the graphical 
        /// representation of the collision shape.
        /// </remarks>
        object UserObject { get; set; }
    }
}
