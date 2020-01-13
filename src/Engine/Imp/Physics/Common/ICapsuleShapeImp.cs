namespace Fusee.Engine.Imp.Physics.Common
{
    /// <summary>
    /// IMplementation agnostic representation for a capsule-shaped collision object.
    /// </summary>
    public interface ICapsuleShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Retrieves the half height.
        /// </summary>
        /// <value>
        /// Half of the height's value.
        /// </value>
        float HalfHeight { get; }
        /// <summary>
        /// Retrieves the capsule's radius.
        /// </summary>
        /// <value>
        /// The radius of the capsule.
        /// </value>
        float Radius { get; }
        /// <summary>
        /// Retrieves the index of the axis pointing towards the capsule's "north pole"
        /// </summary>
        /// <value>
        /// The up axis.
        /// </value>
        int UpAxis { get; }
    }
}
