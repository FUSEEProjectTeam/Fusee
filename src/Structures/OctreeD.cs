using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Structures
{
    public abstract class OctreeD<P>
    {
        public int MaxLevel;

        public IOctant<double3, double, P> Root;

        public delegate void HandlePayload(IOctant<double3, double, P> parent, IOctant<double3, double, P> child, P payload);


        //TODO: 
        // > Complete Octree?

        //public void Subdivide(IOctant<double3, double, P> octant, Func<bool> SubdivTerminationCondition)
        //{
        //    Subdivide(octant, () => true, SubdivTerminationCondition);
        //}

        public void Subdivide(IOctant<double3, double, P> octant, Func<IOctant<double3, double, P>, P, int> GetChildPosition, Func<bool> SubdivTerminationCondition, HandlePayload handlePayload)
        {
            for (int i = 0; i < octant.Payload.Count; i++)
            {
                var payload = octant.Payload[i];
                var posInParent = GetChildPosition(octant, payload);

                if (octant.Children[posInParent] != null)
                    octant.Children[posInParent] = octant.CreateChild(posInParent);

                handlePayload(octant, octant.Children[posInParent], payload);
            }
            octant.Payload.Clear();

            for (int i = 0; i < octant.Children.Length; i++)
            {
                var child = octant.Children[i];
                if (child == null) continue;

                if (SubdivTerminationCondition())
                    Subdivide(child, GetChildPosition, SubdivTerminationCondition, handlePayload);
                else
                    child.IsLeaf = true;
            }
        }

        /// <summary>
        /// Start traversing from the root node.
        /// </summary>
        public void Traverse(Action<IOctant<double3, double, P>> callback)
        {
            DoTraverse(Root, callback);
        }

        /// <summary>
        /// Start traversing from a given node.
        /// </summary>
        public void Traverse(IOctant<double3, double, P> node, Action<IOctant<double3, double, P>> callback)
        {
            DoTraverse(node, callback);
        }

        private static void DoTraverse(IOctant<double3, double, P> node, Action<IOctant<double3, double, P>> callback)
        {
            var candidates = new Stack<IOctant<double3, double, P>>();
            candidates.Push(node);

            while (candidates.Count > 0)
            {
                node = candidates.Pop();
                callback(node);

                // add children as candidates
                IterateChildren(node, (IOctant<double3, double, P> childNode) =>
                {
                    candidates.Push(childNode);
                });
            }
        }

        /// <summary>
        /// Iterates through the child node and calls the given action for each child.
        /// </summary>        
        private static void IterateChildren(IOctant<double3, double, P> parent, Action<IOctant<double3, double, P>> iterateAction)
        {
            if (parent.Children != null)
            {
                for (int i = parent.Children.Length - 1; i >= 0; i--)
                {
                    IOctant<double3, double, P> child = parent.Children[i];
                    if (child != null)
                        iterateAction?.Invoke(child);
                }

            }
        }
    }
}