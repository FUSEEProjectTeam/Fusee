using Fusee.Engine.Core.Primitives;
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
        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                Min = _center - 0.5f * _size;
                Max = _center + 0.5f * _size;
            }
        }
        private double _size;

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
        /// Creates a new instance of type <see cref="PointCloudOctant"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="octId"></param>
        /// <param name="children">The octants child octants.</param>
        public PointCloudOctant(double3 center, double size, OctantId octId, PointCloudOctant[]? children = null)
        {
            Center = center;
            Size = size;

            Min = center - 0.5f * size;
            Max = center + 0.5f * size;

            OctId = octId;

            Level = OctId.Level;
            IsVisible = false;

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
        public bool InsideOrIntersectingFrustum(FrustumF frustum, float3 translation, float3 scale)
        {
            var translatedCenter = new float3(Center) + translation;
            var scaledSize = new float3((float)Size) * scale;
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
            return frustum.Near.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Far.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Left.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Right.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Top.InsideOrIntersecting(new float3(Center), (float)Size) &
                frustum.Bottom.InsideOrIntersecting(new float3(Center), (float)Size);
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
        /// Returns true if the shape is completely inside or is intersecting this <see cref="PointCloudOctant"/>
        /// </summary>
        /// <param name="invSphereModel">The inverse model matrix which is currently used to render the <see cref="Sphere"/> (can e. g. be displayed as an ellipsoid)</param>
        /// <returns></returns>
        public bool InsideOrIntersectingSphere(double4x4 invSphereModel)
        {
            //Transform ellipsoid to sphere(invMat) and transform this PointCloudOctant to an AABBf which lies within the sphere's coordinate system.
            //Then test the sphere against the transformed box.
            var size = invSphereModel.ScaleComponent() * (double3.One * Size);
            var center = invSphereModel * Center;
            var box = new AABBd(center - (size * 0.5f), center + (size * 0.5f));

            //Create all planes for the box. If the signed distance from the sphere center (0,0,0) to all planes is less than the radius of the sphere, the sphere is completely inside the box.
            var frontNormal = (-double3.UnitZ * invSphereModel).Normalize();
            var frontPoint = (Center - size.z / 2) * invSphereModel;
            var frontP = new PlaneD(frontNormal, frontPoint);
            var backNormal = (double3.UnitZ * invSphereModel).Normalize();
            var backPoint = (Center + size.z / 2) * invSphereModel;
            var backP = new PlaneD(backNormal, backPoint);

            var leftNormal = (-double3.UnitX * invSphereModel).Normalize();
            var leftPoint = (Center - size.x / 2) * invSphereModel;
            var leftP = new PlaneD(leftNormal, leftPoint);
            var rightNormal = (double3.UnitX * invSphereModel).Normalize();
            var rightPoint = (Center + size.x / 2) * invSphereModel;
            var rightP = new PlaneD(rightNormal, rightPoint);

            var bottomNormal = (-double3.UnitY * invSphereModel).Normalize();
            var bottomPoint = (Center - size.y / 2) * invSphereModel;
            var bottomP = new PlaneD(bottomNormal, bottomPoint);
            var topNormal = (double3.UnitY * invSphereModel).Normalize();
            var topPoint = (Center + size.y / 2) * invSphereModel;
            var topP = new PlaneD(topNormal, topPoint);

            if (frontP.SignedDistanceFromPoint(double3.Zero) <= 0.5f &&
                backP.SignedDistanceFromPoint(double3.Zero) <= 0.5f &&
                leftP.SignedDistanceFromPoint(double3.Zero) <= 0.5f &&
                rightP.SignedDistanceFromPoint(double3.Zero) <= 0.5f &&
                bottomP.SignedDistanceFromPoint(double3.Zero) <= 0.5f &&
                topP.SignedDistanceFromPoint(double3.Zero) <= 0.5f)
                return true; //Ellipsoid is completely inside the box.


            var p = box.ClosestPoint(double3.Zero);
            //If the length of the vector from the center of the sphere to the closest point on the box surface is smaller than the radius of the sphere the box and the sphere overlap.
            return p.Length <= 0.5f; // if true, ellipsoid and box intersect
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