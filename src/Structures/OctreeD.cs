using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Structures
{
    /// <summary>
    /// Tree data structure in which each internal node has up to eight children. 
    /// Octrees are most often used to partition a three-dimensional space by recursively subdividing it into eight octants.
    /// </summary>
    /// <typeparam name="P">The type of an octants payload.</typeparam>
    public abstract class OctreeD<P>
    {
        /// <summary>
        /// The maximum level this octree has - corresponds to the maximum number of subdivisions of an Octant.
        /// Can be used as subdivision termination condition or be set in the method <see cref="SubdivTerminationCondition(OctantD{P})"/>.
        /// </summary>
        public int MaxLevel;

        /// <summary>
        /// The root Octant of this Octree.
        /// </summary>
        public OctantD<P> Root;

        /// <summary>
        /// Needed for subdivision - method that determines what happens to a payload item after the generation of an octants children.
        /// </summary>
        /// <param name="parent">The parent octant.</param>
        /// <param name="child">The child octant a payload item falls into.</param>
        /// <param name="payloadItem">The payload item.</param>
        protected abstract void HandlePayload(OctantD<P> parent, OctantD<P> child, P payloadItem);

        /// <summary>
        /// Needed for subdivision - Method that returns a condition that terminates the subdivision.
        /// </summary>
        /// <param name="octant">The octant to subdivide.</param>
        /// <returns></returns>
        protected abstract bool SubdivTerminationCondition(OctantD<P> octant);

        /// <summary>
        /// Needed for subdivision - method that provides functionality to determine and return the index (position) of the child a payload item will fall into.
        /// </summary>
        /// <param name="octant">The octant to subdivide.</param>
        /// <param name="payloadItem">The payload item for which the child index is determined.</param>
        /// <returns></returns>
        protected abstract int GetChildPosition(OctantD<P> octant, P payloadItem);

        /// <summary>
        /// Subdivision creates the children of an Octant. Here the payload of an Octant will be iterated and for each item it is determined in which child it will fall.
        /// If there isn't a child already it will be created.
        /// </summary>
        /// <param name="octant">The Octant to subdivide.</param>
        public virtual void Subdivide(OctantD<P> octant)
        {
            for (int i = 0; i < octant.Payload.Count; i++)
            {
                var payload = octant.Payload[i];
                var posInParent = GetChildPosition(octant, payload);

                if (octant.Children[posInParent] == null)
                    octant.Children[posInParent] = octant.CreateChild(posInParent);

                HandlePayload(octant, (OctantD<P>)octant.Children[posInParent], payload);
            }
            octant.Payload.Clear();

            for (int i = 0; i < octant.Children.Length; i++)
            {
                var child = (OctantD<P>)octant.Children[i];
                if (child == null) continue;

                if (!SubdivTerminationCondition(child))
                    Subdivide(child);
                else
                    child.IsLeaf = true;
            }
        }

        /// <summary>
        /// Start traversing (breadth first) from the root node.
        /// </summary>
        public void Traverse(Action<OctantD<P>> callback)
        {
            DoTraverse(Root, callback);
        }

        /// <summary>
        /// Start traversing (breadth first) from a given node.
        /// </summary>
        public void Traverse(OctantD<P> node, Action<OctantD<P>> callback)
        {
            DoTraverse(node, callback);
        }

        private static void DoTraverse(OctantD<P> node, Action<OctantD<P>> callback)
        {
            var candidates = new Stack<OctantD<P>>();
            candidates.Push(node);

            while (candidates.Count > 0)
            {
                node = candidates.Pop();
                if (callback == null) throw new NullReferenceException("The parameter 'callback' cannot be null!");
                callback(node);

                // add children as candidates
                IterateChildren(node, (OctantD<P> childNode) =>
                {
                    candidates.Push(childNode);
                });
            }
        }

        /// <summary>
        /// Iterates through the child node and calls the given action for each child.
        /// </summary>
        private static void IterateChildren(OctantD<P> parent, Action<OctantD<P>> iterateAction)
        {
            if (parent.Children != null)
            {
                for (int i = parent.Children.Length - 1; i >= 0; i--)
                {
                    var child = (OctantD<P>)parent.Children[i];
                    if (child != null)
                        iterateAction?.Invoke(child);
                }

            }
        }
    }
}