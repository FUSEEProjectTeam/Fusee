using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Used in <see cref="PointCloudOctree"/>. This octant does not contain actual point data.
    /// </summary>
    public class PointCloudOctant : IPointCloudOctant
    {
        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode { get; set; }

        /// <summary>
        /// The globally unique identifier for this octant.
        /// </summary>
        public OctantId OctId { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// List of child octants.
        /// </summary>
        public IEmptyOctant<double3, double>[] Children { get; private set; }

        /// <summary>
        /// If true this octant does not have any children.
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Position in the parent octant/cube.
        /// </summary>
        public int PosInParent { get; private set; }

        /// <summary>
        /// Level/ depth of this octant in the octree.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloudOctant"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="octId"></param>
        /// <param name="children">The octants child octants.</param>
        public PointCloudOctant(double3 center, double size, OctantId octId, PointCloudOctant[] children = null)
        {
            Center = center;
            Size = size;

            OctId = octId;

            Level = OctId.Level;

            int.TryParse(OctId[Level - 1].ToString(), out int posInParent);
            PosInParent = posInParent;

            if (children == null)
                Children = new PointCloudOctant[8];
            else
                Children = children;

        }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov, float4x4 model)
        {
            var translatedCenter = Center + new double3(model.Translation());
            var scaledRad = Size / 2f * new double3(model.Scale());
            var distance = (translatedCenter - camPos).Length;
            if (translatedCenter == camPos)
                distance = 0.0001f;
            var slope = (float)System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * scaledRad.x / (slope * distance);
        }

        /// <summary>
        /// Instantiates a child octant at the given position.
        /// </summary>
        /// <param name="atPosInParent">The <see cref="PosInParent"/> the new child has.</param>
        public IEmptyOctant<double3, double> CreateChild(int atPosInParent)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum, float4x4 model)
        {
            var translatedCenter = new float3(Center) + model.Translation();
            return frustum.Near.InsideOrIntersecting(translatedCenter, (float)Size) &
                frustum.Far.InsideOrIntersecting(translatedCenter, (float)Size) &
                frustum.Left.InsideOrIntersecting(translatedCenter, (float)Size) &
                frustum.Right.InsideOrIntersecting(translatedCenter, (float)Size) &
                frustum.Top.InsideOrIntersecting(translatedCenter, (float)Size) &
                frustum.Bottom.InsideOrIntersecting(translatedCenter, (float)Size);
        }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            return frustum.Near.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Far.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Left.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Right.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Top.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Bottom.InsideOrIntersecting(new float3(Center), (float)Size);
        }
    }
}