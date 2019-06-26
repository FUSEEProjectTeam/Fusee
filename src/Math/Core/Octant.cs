using System.Collections.Generic;
using System;

namespace Fusee.Math.Core
{

    /// <summary>
    /// Node for use in an Grid-like structure.
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload.</typeparam>
    public class Bucket<T>
    {
        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; protected set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; protected set; }
        

        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        protected Bucket()
        {

        }

        public Bucket(double3 center, double size)
        {
            Center = center;
            Size = size;

            
        }
    }

    public class GridCell<T>: Bucket<T>
    {
        public T Occupant;

        public GridCell(double3 center, double size)
        {
            Center = center;
            Size = size;

            
        }
    }

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

        public List<T> Payload;

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
