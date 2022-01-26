using Fusee.PointCloud.Common;
using Fusee.Structures;
using System;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Implementation of <see cref="OctreeD{P}"/> for rendering Point Clouds out of core.
    /// </summary>
    public class PointCloudOctree : OctreeD<IPointCloudPoint>
    {
        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PointCloudOctree(PointCloudOctant root)
        {
            Root = root;
        }

        /// <summary>
        /// Needed for subdivision - Method that returns a condition that terminates the subdivision.
        /// </summary>
        /// <param name="child">The octant to subdivide.</param>
        /// <returns></returns>
        protected override bool SubdivTerminationCondition(OctantD<IPointCloudPoint> child)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Needed for subdivision - method that determines what happens to a payload item after the creation of an octants children.
        /// </summary>
        /// <param name="parent">The parent octant.</param>
        /// <param name="child">The child octant a payload item falls into.</param>
        /// <param name="payload">The payload item.</param>
        protected override void HandlePayload(OctantD<IPointCloudPoint> parent, OctantD<IPointCloudPoint> child, IPointCloudPoint payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Needed for subdivision - method that provides functionality to determine and return the index (position) of the child a payload item will fall into.
        /// </summary>
        /// <param name="octant">The octant to subdivide.</param>
        /// <param name="pt">The point for which the child index is determined.</param>
        /// <returns></returns>
        protected override int GetChildPosition(OctantD<IPointCloudPoint> octant, IPointCloudPoint pt)
        {
            throw new NotImplementedException();
        }
    }
}