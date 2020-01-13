using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Common
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
        /// Gets and sets a user object.
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
