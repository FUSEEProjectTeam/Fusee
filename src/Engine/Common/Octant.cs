using System;
using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Component that allows a SceneNode to save information usually associated with a "PtOctant".
    /// </summary>
    public class Octant : SceneComponent
    {
        /// <summary>
        /// Defines the position in the parent octant.
        /// </summary>
        public int PosInParent;

        /// <summary>
        /// The level of the octree the node lies in.
        /// </summary>
        public int Level;

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Center in world space.
        /// </summary>
        public double3 Center;

        /// <summary>
        /// Length of on side of the cubical node.
        /// </summary>
        public double Size;

        /// <summary>
        /// Defines if the node is a leaf node.
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// Defines if the node was loaded into memory.
        /// </summary>
        public bool WasLoaded;

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode;

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        public int PosInHierarchyTex;

        public byte VisibleChildIndices;

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2f);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        public bool Intersects(float4x4 vf)
        {
            var planes = new float4[6];

            planes[0] = vf.Row3 + vf.Row2;
            planes[1] = vf.Row3 - vf.Row2;
            planes[2] = vf.Row3 + vf.Row0;
            planes[3] = vf.Row3 - vf.Row0;
            planes[4] = vf.Row3 + vf.Row1;
            planes[5] = vf.Row3 - vf.Row1;

            foreach (var plane in planes)
            {
                var side = Classify(plane);
                if (side < 0) return false;
            }
            return true;
            //return PlaneIntersects(planes);
        }

        private float Classify(float4 plane)
        {
            plane.Normalize();

            // maximum extent in direction of plane normal (plane.xyz)
            var r = System.Math.Abs(Size * plane.x)
                + System.Math.Abs(Size * plane.y)
                + System.Math.Abs(Size * plane.z);

            // signed distance between box center and plane
            //float d = plane.Test(mCenter);
            var d = float3.Dot(plane.xyz, (float3)Center) + plane.w;

            // return signed distance
            if (System.Math.Abs(d) < r)
                return 0.0f;
            else if (d < 0.0)
                return (float)(d + r);
            return (float)(d - r);
        }


        //see: http://old.cescg.org/CESCG-2002/DSykoraJJelinek/
        private bool PlaneIntersects(double4[] planes)
        {
            var res = true;
            foreach (var plane in planes)
            {
                var normPlane = plane.Normalize();
                normPlane *= -1;

                var pVert = GetPVert(normPlane.xyz);
                var nVert = GetNVert(normPlane.xyz);

                var m = (normPlane.x * nVert.x) + (normPlane.y * nVert.y) + (normPlane.z * nVert.z);
                var n = (normPlane.x * pVert.x) + (normPlane.y * pVert.y) + (normPlane.z * pVert.z);
                if (m > -normPlane.w)
                {
                    res = false;
                    return res;
                } //outside

                if (n > -normPlane.w)
                {
                    res = true;
                } //intersects
            }

            return res; //inside            
        }

        private double3 GetPVert(double3 planeNormal)
        {

            var Max = Center + new double3(Size, Size, Size);
            var Min = Center - new double3(Size, Size, Size);

            if (planeNormal.x > 0 && planeNormal.y > 0 && planeNormal.z > 0)        //+ + +
                return Max;
            else if (planeNormal.x > 0 && planeNormal.y > 0 && planeNormal.z < 0)   //+ + -
                return new double3(Max.x, Max.y, Min.z);
            else if (planeNormal.x > 0 && planeNormal.y < 0 && planeNormal.z > 0)   //+ - +
                return new double3(Max.x, Min.y, Max.z);
            else if (planeNormal.x > 0 && planeNormal.y < 0 && planeNormal.z < 0)   //+ - -
                return new double3(Max.x, Min.y, Min.z);
            else if (planeNormal.x < 0 && planeNormal.y > 0 && planeNormal.z > 0)   //- + +
                return new double3(Min.x, Max.y, Max.z);
            else if (planeNormal.x < 0 && planeNormal.y > 0 && planeNormal.z < 0)   //- + -
                return new double3(Min.x, Max.y, Min.z);
            else if (planeNormal.x < 0 && planeNormal.y < 0 && planeNormal.z > 0)   //- - +
                return new double3(Min.x, Min.y, Max.z);
            else                                                                    //- - -
                return Min;
        }

        private double3 GetNVert(double3 planeNormal)
        {
            var Max = Center + new double3(Size, Size, Size);
            var Min = Center - new double3(Size, Size, Size);

            if (planeNormal.x > 0 && planeNormal.y > 0 && planeNormal.z > 0)        //+ + +
                return Min;
            else if (planeNormal.x > 0 && planeNormal.y > 0 && planeNormal.z < 0)   //+ + -
                return new double3(Min.x, Min.y, Max.z);
            else if (planeNormal.x > 0 && planeNormal.y < 0 && planeNormal.z > 0)   //+ - +
                return new double3(Min.x, Max.y, Min.z);
            else if (planeNormal.x > 0 && planeNormal.y < 0 && planeNormal.z < 0)   //+ - -
                return new double3(Min.x, Max.y, Max.z);
            else if (planeNormal.x < 0 && planeNormal.y > 0 && planeNormal.z > 0)   //- + +
                return new double3(Max.x, Min.y, Min.z);
            else if (planeNormal.x < 0 && planeNormal.y > 0 && planeNormal.z < 0)   //- + -
                return new double3(Max.x, Min.y, Max.z);
            else if (planeNormal.x < 0 && planeNormal.y < 0 && planeNormal.z > 0)   //- - +
                return new double3(Max.x, Max.y, Min.z);
            else                                                                    //- - -
                return Max;
        }
    }
}
