
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Describes a frustum by using six <see cref="PlaneF"/>s. 
    /// </summary>
    public class FrustumF
    {
        /// <summary>
        /// The near plane of the frustum.
        /// </summary>
        public PlaneF Near { get; private set; }

        /// <summary>
        /// The far plane of the frustum.
        /// </summary>
        public PlaneF Far { get; private set; }

        /// <summary>
        /// The left plane of the frustum.
        /// </summary>
        public PlaneF Left { get; private set; }

        /// <summary>
        /// The right plane of the frustum.
        /// </summary>
        public PlaneF Right { get; private set; }

        /// <summary>
        /// The top plane of the frustum.
        /// </summary>
        public PlaneF Top { get; private set; }

        /// <summary>
        /// The bottom plane of the frustum.
        /// </summary>
        public PlaneF Bottom { get; private set; }

        /// <summary>
        /// (Re)Calculates the frustum planes.
        /// If feeded with a projection matrix, the frustum planes are in View Space.
        /// If feeded with a view projection matrix, the frustum planes are in World Space.
        /// If feeded with a model view projection matrix, the frustum planes are in Model Space.
        /// See: http://www8.cs.umu.se/kurser/5DV051/HT12/lab/plane_extraction.pdf
        /// </summary>
        /// <param name="mat">The matrix from which to extract the planes.</param>
        public void CalculateFrustumPlanes(float4x4 mat)
        {
            // left
            var left = new PlaneF()
            {
                A = mat.M41 + mat.M11,
                B = mat.M42 + mat.M12,
                C = mat.M43 + mat.M13,
                D = mat.M44 + mat.M14
            };

            // right
            var right = new PlaneF()
            {
                A = mat.M41 - mat.M11,
                B = mat.M42 - mat.M12,
                C = mat.M43 - mat.M13,
                D = mat.M44 - mat.M14
            };


            // bottom
            var bottom = new PlaneF()
            {
                A = mat.M41 + mat.M21,
                B = mat.M42 + mat.M22,
                C = mat.M43 + mat.M23,
                D = mat.M44 + mat.M24
            };


            // top
            var top = new PlaneF()
            {
                A = mat.M41 - mat.M21,
                B = mat.M42 - mat.M22,
                C = mat.M43 - mat.M23,
                D = mat.M44 - mat.M24
            };


            // near
            var near = new PlaneF()
            {
                A = mat.M41 + mat.M31,
                B = mat.M42 + mat.M32,
                C = mat.M43 + mat.M33,
                D = mat.M44 + mat.M34
            };


            // far
            var far = new PlaneF()
            {
                A = mat.M41 - mat.M31,
                B = mat.M42 - mat.M32,
                C = mat.M43 - mat.M33,
                D = mat.M44 - mat.M34
            };

            //Negate D because Fusee uses ax +by + cz = d
            left.D *= -1;
            right.D *= -1;
            near.D *= -1;
            far.D *= -1;
            top.D *= -1;
            bottom.D *= -1;

            //Invert plane to get the normal to face outwards.
            left *= -1;
            right *= -1;
            near *= -1;
            far *= -1;
            top *= -1;
            bottom *= -1;

            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            Near = near;
            Far = far;
        }

        /// <summary>
        /// Calculates the eight frustum corners from an input matrix. In most cases this matrix will be the View-Projection-Matrix.
        /// </summary>
        /// <param name="mat">The matrix from which to calculate the frustum corners.</param>
        /// <returns></returns>
        public static IEnumerable<float3> CalculateFrustumCorners(float4x4 mat)
        {
            //1. Calculate the 8 corners of the view frustum in world space. This can be done by using the inverse view-projection matrix to transform the 8 corners of the NDC cube (which in OpenGL is [‒1, 1] along each axis).
            //2. Transform the frustum corners to a space aligned with the shadow map axes.This would commonly be the directional light object's local space. 
            //In fact, steps 1 and 2 can be done in one step by combining the inverse view-projection matrix of the camera with the inverse world matrix of the light.
            var invViewProjection = float4x4.Invert(mat);

            var frustumCorners = new float4[8];

            frustumCorners[0] = invViewProjection * new float4(-1, -1, -1, 1); //nbl
            frustumCorners[1] = invViewProjection * new float4(1, -1, -1, 1); //nbr
            frustumCorners[2] = invViewProjection * new float4(-1, 1, -1, 1); //ntl
            frustumCorners[3] = invViewProjection * new float4(1, 1, -1, 1); //ntr
            frustumCorners[4] = invViewProjection * new float4(-1, -1, 1, 1); //fbl
            frustumCorners[5] = invViewProjection * new float4(1, -1, 1, 1); //fbr
            frustumCorners[6] = invViewProjection * new float4(-1, 1, 1, 1); //ftl
            frustumCorners[7] = invViewProjection * new float4(1, 1, 1, 1); //ftr

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner /= corner.w; //world space frustum corners
                frustumCorners[i] = corner;
                yield return frustumCorners[i].xyz;
            }

        }
    }
}