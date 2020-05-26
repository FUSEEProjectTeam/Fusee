using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a collision shape 
    /// consisting of multiple spheres.
    /// </summary>
    public interface IMultiSphereShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Gets the sphere position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        float3 GetSpherePosition(int index);

        /// <summary>
        /// Gets the sphere radius.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        float GetSphereRadius(int index);

        /// <summary>
        /// Gets the sphere count.
        /// </summary>
        /// <value>
        /// The sphere count.
        /// </value>
        int SphereCount { get; }
    }
}