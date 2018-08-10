namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic Base representation of a constraint.
    /// A constraint defines how motion and rotation is constrained, typically 
    /// by a connection between two bodies A and B.
    /// </summary>
    public interface IConstraintImp
    {
        /// <summary>
        /// Gets the rigid body A.
        /// </summary>
        /// <value>
        /// The rigid body A.
        /// </value>
        IRigidBodyImp RigidBodyA { get; }
        /// <summary>
        /// Gets the rigid body B.
        /// </summary>
        /// <value>
        /// The rigid body B.
        /// </value>
        IRigidBodyImp RigidBodyB { get; }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns>The uid.</returns>
        int GetUid();
        /// <summary>
        /// Gets or sets a user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        object UserObject { get; set; }
    }
}
