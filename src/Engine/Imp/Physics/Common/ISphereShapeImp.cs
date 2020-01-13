namespace Fusee.Engine.Imp.Physics.Common
{
    /// <summary>
    /// Implementation agnostic representation of a spherical collision shape.
    /// </summary>
    public interface ISphereShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets and sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        float Radius { get; set; }
    }
}
