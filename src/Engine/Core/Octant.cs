using System.Collections.Generic;
using System;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Node for use in an Octree.
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload.</typeparam>
    public class Octant<T> : Bucket<T>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public Octant<T>[] Children;

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<T> Payload;

        /// <summary>
        /// The level of the octant in the octree.
        /// </summary>
        public int Level;

        /// <summary>
        /// Creates a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>
        /// <param name="payload"></param>
        public Octant(double3 center, double size, List<T> payload, Octant<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<T>[8];
            else
                Children = children;
            
            Payload = payload;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>        
        public Octant(double3 center, double size, Octant<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<T>[8];
            else
                Children = children;            
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        protected Octant(){}

    }    
}
