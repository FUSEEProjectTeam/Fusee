using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Frustum
    {
        private float[,] frustum = new float[6, 4];
        private Plane[] pl = new Plane[6];
        public Frustum(float4x4 mvp)
        {
            UpdateFrustum(mvp);
        }

        private enum PlaneSide
        {
            NEARP, FARP, BOTTOM, TOP, LEFT, RIGHT,
        }

        private struct Plane
        {
            public float3 normal;
            public float d;
            public void setCoefficients(float a, float b, float c, float d)
            {
                // set the normal vector
                normal = new float3(a, b, c);
                //compute the lenght of the vector
                float l = normal.Length;
                // normalize the vector
                normal = new float3(a / l, b / l, c / l);
                // and divide d by th length as well
                this.d = d / l;
            }

            public float distance(float3 p) {
	            return (d + float3.Dot(normal,p));
            }


        }

        public void setFrustum(float4x4 mvp)
        {
            //mvp = float4x4.Invert(mvp);
            pl[(int)PlaneSide.NEARP].setCoefficients(mvp.M31 + mvp.M41, mvp.M32 + mvp.M42, mvp.M33 + mvp.M43, mvp.M34 + mvp.M44);


            pl[(int)PlaneSide.FARP].setCoefficients(-mvp.M31 + mvp.M41, -mvp.M32 + mvp.M42, -mvp.M33 + mvp.M43, -mvp.M34 + mvp.M44);


            pl[(int)PlaneSide.BOTTOM].setCoefficients(mvp.M21 + mvp.M41, mvp.M22 + mvp.M42, mvp.M23 + mvp.M43, mvp.M24 + mvp.M44);


            pl[(int)PlaneSide.TOP].setCoefficients(-mvp.M21 + mvp.M41, -mvp.M22 + mvp.M42, -mvp.M23 + mvp.M43, -mvp.M24 + mvp.M44);


            pl[(int)PlaneSide.LEFT].setCoefficients(mvp.M11 + mvp.M41, mvp.M12 + mvp.M42, mvp.M13 + mvp.M43, mvp.M14 + mvp.M44);


            pl[(int)PlaneSide.RIGHT].setCoefficients(-mvp.M11 + mvp.M41, -mvp.M12 + mvp.M42, -mvp.M13 + mvp.M43, -mvp.M14 + mvp.M44);

        }

        public void UpdateFrustum(float4x4 mvp)
        {
            //setFrustum(mvp);
            float t; // Temporary normalized value
            /* Extract the numbers for the RIGHT plane */
            frustum[0, 0] = mvp.M14 - mvp.M11;
            frustum[0, 1] = mvp.M24 - mvp.M21;
            frustum[0, 2] = mvp.M34 - mvp.M31;
            frustum[0, 3] = mvp.M44 - mvp.M41;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[0, 0] * frustum[0, 0] + frustum[0, 1] * frustum[0, 1] + frustum[0, 2] * frustum[0, 2]);
            frustum[0, 0] /= t;
            frustum[0, 1] /= t;
            frustum[0, 2] /= t;
            frustum[0, 3] /= t;

            /* Extract the numbers for the LEFT plane */
            frustum[1, 0] = mvp.M14 + mvp.M11;
            frustum[1, 1] = mvp.M24 + mvp.M21;
            frustum[1, 2] = mvp.M34 + mvp.M31;
            frustum[1, 3] = mvp.M44 + mvp.M41;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[1, 0] * frustum[1, 0] + frustum[1, 1] * frustum[1, 1] + frustum[1, 2] * frustum[1, 2]);
            frustum[1, 0] /= t;
            frustum[1, 1] /= t;
            frustum[1, 2] /= t;
            frustum[1, 3] /= t;

            /* Extract the BOTTOM plane */
            frustum[2, 0] = mvp.M14 + mvp.M12;
            frustum[2, 1] = mvp.M24 + mvp.M22;
            frustum[2, 2] = mvp.M34 + mvp.M32;
            frustum[2, 3] = mvp.M44 + mvp.M42;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[2, 0] * frustum[2, 0] + frustum[2, 1] * frustum[2, 1] + frustum[2, 2] * frustum[2, 2]);
            frustum[2, 0] /= t;
            frustum[2, 1] /= t;
            frustum[2, 2] /= t;
            frustum[2, 3] /= t;

            /* Extract the TOP plane */
            frustum[3, 0] = mvp.M14 - mvp.M12;
            frustum[3, 1] = mvp.M24 - mvp.M22;
            frustum[3, 2] = mvp.M34 - mvp.M32;
            frustum[3, 3] = mvp.M44 - mvp.M42;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[3, 0] * frustum[3, 0] + frustum[3, 1] * frustum[3, 1] + frustum[3, 2] * frustum[3, 2]);
            frustum[3, 0] /= t;
            frustum[3, 1] /= t;
            frustum[3, 2] /= t;
            frustum[3, 3] /= t;

            /* Extract the FAR plane */
            frustum[4, 0] = mvp.M14 - mvp.M13;
            frustum[4, 1] = mvp.M24 - mvp.M23;
            frustum[4, 2] = mvp.M34 - mvp.M33;
            frustum[4, 3] = mvp.M44 - mvp.M43;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[4, 0] * frustum[4, 0] + frustum[4, 1] * frustum[4, 1] + frustum[4, 2] * frustum[4, 2]);
            frustum[4, 0] /= t;
            frustum[4, 1] /= t;
            frustum[4, 2] /= t;
            frustum[4, 3] /= t;

            /* Extract the NEAR plane */
            frustum[5, 0] = mvp.M14 + mvp.M13;
            frustum[5, 1] = mvp.M24 + mvp.M23;
            frustum[5, 2] = mvp.M34 + mvp.M33;
            frustum[5, 3] = mvp.M44 + mvp.M43;

            /* Normalize the result */
            t = (float)System.Math.Sqrt(frustum[5, 0] * frustum[5, 0] + frustum[5, 1] * frustum[5, 1] + frustum[5, 2] * frustum[5, 2]);
            frustum[5, 0] /= t;
            frustum[5, 1] /= t;
            frustum[5, 2] /= t;
            frustum[5, 3] /= t;
            //Debug.WriteLine(mvp);
        }

        public bool PointInFrustum(float3 point, float4x4 _mvp)
        {
            float4 pc = _mvp * new float4(point.x, point.y, point.z, 1); 
            /*
            for (int p = 0; p < 6; p++)
                if (frustum[p, 0] * point.x + frustum[p, 1] * point.y + frustum[p, 2] * point.z + frustum[p, 3] < 0)
                {
                    Debug.WriteLine(point + " is not in frustum. " + frustum[p, 0] + "|" + frustum[p, 1] + "|" + frustum[p, 2] + "|" + frustum[p, 3]);
                    return false;
                }
            return true;*/
            
            setFrustum(_mvp);
            bool result = true;
            for (int i = 0; i < 6; i++)
            {

                if (pl[i].distance(point) < 0)
                {
                    return false;
                }
            }
            return result;
        }


        public bool SphereInFrustum(float3 point, float radius, float4x4 vp)
        {
            
            for (int p = 0; p < 6; p++)
                if (frustum[p,0] * point.x + frustum[p,1] * point.y + frustum[p,2] * point.z + frustum[p,3] <= -radius)
                {
                    //float sum = frustum[p, 0]*point.x + frustum[p, 1]*point.y + frustum[p, 2]*point.z + frustum[p, 3];
                    //Debug.WriteLine(point +" | "+sum);
                    return false;
                }
                    
            return true;
        }

        // Contains also the distance as result, may be used for levelofDetail
        float SphereInFrustumDistance(float3 point, float radius)
        {
            int p;
            float d = 0;

            for (p = 0; p < 6; p++)
            {
                d = frustum[p,0] * point.x + frustum[p,1] * point.y + frustum[p,2] * point.z + frustum[p,3];
                if (d <= -radius)
                    return 0;
            }
            return d + radius;
        }

        public bool CubeInFrustum(float3 point, float size)
        {
            for (int p = 0; p < 6; p++)
            {
                if (frustum[p,0] * (point.x - size) + frustum[p,1] * (point.y - size) + frustum[p,2] * (point.z - size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x + size) + frustum[p,1] * (point.y - size) + frustum[p,2] * (point.z - size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x - size) + frustum[p,1] * (point.y + size) + frustum[p,2] * (point.z - size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x + size) + frustum[p,1] * (point.y + size) + frustum[p,2] * (point.z - size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x - size) + frustum[p,1] * (point.y - size) + frustum[p,2] * (point.z + size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x + size) + frustum[p,1] * (point.y - size) + frustum[p,2] * (point.z + size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x - size) + frustum[p,1] * (point.y + size) + frustum[p,2] * (point.z + size) + frustum[p,3] > 0)
                    continue;
                if (frustum[p,0] * (point.x + size) + frustum[p,1] * (point.y + size) + frustum[p,2] * (point.z + size) + frustum[p,3] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public bool SphereOrCubeInFrustum()
        {
            return true;
        }

    }
}
