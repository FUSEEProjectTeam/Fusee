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

        public Frustum(float4x4 mvp)
        {
            UpdateFrustum(mvp);
        }



        public void UpdateFrustum(float4x4 mvp)
        {
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
            Debug.WriteLine(mvp);
        }

        public bool PointInFrustum(float3 point)
        {

            for (int p = 0; p < 6; p++)
                if (frustum[p, 0] * point.x + frustum[p, 1] * point.y + frustum[p, 2] * point.z + frustum[p, 3] < 0)
                {
                    Debug.WriteLine(point + " is not in frustum. " + frustum[p, 0] + "|" + frustum[p, 1] + "|" + frustum[p, 2] + "|" + frustum[p, 3]);
                    return false;
                }
            return true;
        }


        public bool SphereInFrustum(float3 point, float radius)
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
