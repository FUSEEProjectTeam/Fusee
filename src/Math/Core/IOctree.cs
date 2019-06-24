using System;

namespace Fusee.Math.Core
{

    /// <summary>
    /// Octree data structure. Implement this if you want to create an Octree.
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload in an Octant.</typeparam>
    public interface IOctree<T>
    {
        /// <summary>
        /// The root octant.
        /// </summary>
        Octant<T> Root { get; set; }

        /// <summary>
        /// Tranverses the Octree.
        /// </summary>
        void Traverse();

        /// <summary>
        /// Subdivides the given octant. After subdivision it has to have eight children. 
        /// </summary>
        void Subdivide(Octant<T> octant);
    }
    
}
