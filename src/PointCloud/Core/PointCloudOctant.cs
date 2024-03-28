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
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float, float3, float3)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode { get; set; }

        /// <summary>
        /// Number of point cloud points, this node can hold.
        /// This octant is of dynamic size, will return <see cref="NumberOfPointsInNode"/>
        /// </summary>
        public int PointCapacity => NumberOfPointsInNode;

        /// <summary>
        /// The globally unique identifier for this octant.
        /// </summary>
        public OctantId OctId { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center
        {
            get { return _center; }
            set
            {
                _center = value;
                Min = _center - 0.5f * _size;
                Max = _center + 0.5f * _size;
            }
        }
        private double3 _center;

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double3 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                Min = _center - 0.5f * _size;
                Max = _center + 0.5f * _size;
            }
        }
        private double3 _size;

        /// <summary>
        /// List of child octants.
        /// </summary>
        public IEmptyOctant<double3, double3>[] Children { get; private set; }

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
        /// True if octant is currently visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// The minimum of octants bounding box.
        /// </summary>
        public double3 Min { get; private set; }

        /// <summary>
        /// The maximum of octants bounding box.
        /// </summary>
        public double3 Max { get; private set; }

        /// <summary>
        /// True, if this octant is marked as proxy/helper node.
        /// Proxy nodes can have children but don't necessarily have any payload.
        /// </summary>
        public bool IsProxy { get; } = false;

        /// <summary>
        /// Support lazy loading.
        /// </summary>
        public bool Initialized { get; } = true;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloudOctant"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="octId"></param>
        /// <param name="children">The octants child octants.</param>
        public PointCloudOctant(double3 center, double3 size, OctantId octId, PointCloudOctant[]? children = null)
        {
            Center = center;
            Size = size;

            Min = center - 0.5f * size;
            Max = center + 0.5f * size;

            OctId = octId;

            Level = OctId.Level;
            IsVisible = false;

            _ = int.TryParse(OctId[Level - 1].ToString(), out int posInParent);
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
        /// <param name="translation">Translation of node</param>
        /// <param name="scale">Scale of node</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov, float3 translation, float3 scale)
        {
            var translatedCenter = Center + new double3(translation);
            var scaledRad = Size / 2f * new double3(scale);
            var distance = (translatedCenter - camPos).Length;
            if (translatedCenter == camPos)
                distance = 0.0001f;
            var slope = System.Math.Abs((float)System.Math.Tan(fov / 2d));

            var maxRad = System.Math.Max(System.Math.Max(scaledRad.x, scaledRad.y), scaledRad.z);
            ProjectedScreenSize = screenHeight / 2d * maxRad / (slope * distance);
        }

        /// <summary>
        /// Instantiates a child octant at the given position.
        /// </summary>
        /// <param name="atPosInParent">The <see cref="PosInParent"/> the new child has.</param>
        public IEmptyOctant<double3, double3> CreateChild(int atPosInParent)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <param name="translation">Additional translation.</param>
        /// <param name="scale">Additional scale.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum, float3 translation, float3 scale)
        {
            //var maxSize = (float)System.Math.Max(System.Math.Max(Size.x, Size.y), Size.z);
            var translatedCenter = new float3(Center) + translation;
            var scaledSize = new float3(Size) * scale;
            return frustum.Near.InsideOrIntersecting(translatedCenter, scaledSize) &
                frustum.Far.InsideOrIntersecting(translatedCenter, scaledSize) &
                frustum.Left.InsideOrIntersecting(translatedCenter, scaledSize) &
                frustum.Right.InsideOrIntersecting(translatedCenter, scaledSize) &
                frustum.Top.InsideOrIntersecting(translatedCenter, scaledSize) &
                frustum.Bottom.InsideOrIntersecting(translatedCenter, scaledSize);
        }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            var sizeF = (float3)Size;
            return frustum.Near.InsideOrIntersecting(new float3(Center), sizeF) &
                frustum.Far.InsideOrIntersecting(new float3(Center), sizeF) &
                frustum.Left.InsideOrIntersecting(new float3(Center), sizeF) &
                frustum.Right.InsideOrIntersecting(new float3(Center), sizeF) &
                frustum.Top.InsideOrIntersecting(new float3(Center), sizeF) &
                frustum.Bottom.InsideOrIntersecting(new float3(Center), sizeF);
        }

        /// <summary>
        /// Check if point lies in this octant.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(double3 point)
        {
            return (point.x >= Min.x && point.x <= Max.x) &&
            (point.y >= Min.y && point.y <= Max.y) &&
            (point.z >= Min.z && point.z <= Max.z);
        }

        /// <summary>
        /// Returns true if a sphere is completely inside or is intersecting this <see cref="PointCloudOctant"/>
        /// see: https://web.archive.org/web/19991129023147/http://www.gamasutra.com/features/19991018/Gomez_4.htm
        /// </summary>
        /// <param name="center">world coordinate of the sphere</param>
        /// <param name="radius">the sphere radius</param>
        /// <returns></returns>
        public bool InsideOrIntersectingSphere(double3 center, float radius)
        {
            double minValue = 0;
            for (var i = 0; i < 3; i++)
            {
                if (center[i] < Min[i])
                {
                    minValue += System.Math.Sqrt(center[i] - Min[i]);
                }
                else if (center[i] > Max[i])
                {
                    minValue += System.Math.Sqrt(center[i] - Max[i]);
                }
            }
            return minValue <= (radius * radius);
        }

        /// <summary>
        /// Checks if a given ray originates in, or intersects octant.
        /// </summary>
        /// <param name="ray">The ray to test against.</param>
        /// <returns></returns>
        public bool IntersectRay(RayD ray)
        {
            if (this.Intersects(ray.Origin))
                return true;

            double t1 = (Min[0] - ray.Origin[0]) * ray.Inverse[0];
            double t2 = (Max[0] - ray.Origin[0]) * ray.Inverse[0];

            double tmin = M.Min(t1, t2);
            double tmax = M.Max(t1, t2);

            for (int i = 1; i < 3; i++)
            {
                t1 = (Min[i] - ray.Origin[i]) * ray.Inverse[i];
                t2 = (Max[i] - ray.Origin[i]) * ray.Inverse[i];

                t1 = double.IsNaN(t1) ? 0.0f : t1;
                t2 = double.IsNaN(t2) ? 0.0f : t2;

                tmin = M.Max(tmin, M.Min(t1, t2));
                tmax = M.Min(tmax, M.Max(t1, t2));
            }

            return tmax >= M.Max(tmin, 0.0);
        }
    }
}