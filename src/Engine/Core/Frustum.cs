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

            // compute distance between plane-normal(direction) and point
            public float distance(float3 p) {
	            return (d + float3.Dot(normal,p));
            }


        }


        public void UpdateFrustum(float4x4 mvp)
        {
            // Set the Planes using the ITMV: Inverted ModelViewProjection
            pl[(int)PlaneSide.NEARP].setCoefficients(mvp.M31 + mvp.M41, mvp.M32 + mvp.M42, mvp.M33 + mvp.M43, mvp.M34 + mvp.M44);


            pl[(int)PlaneSide.FARP].setCoefficients(-mvp.M31 + mvp.M41, -mvp.M32 + mvp.M42, -mvp.M33 + mvp.M43, -mvp.M34 + mvp.M44);


            pl[(int)PlaneSide.BOTTOM].setCoefficients(mvp.M21 + mvp.M41, mvp.M22 + mvp.M42, mvp.M23 + mvp.M43, mvp.M24 + mvp.M44);


            pl[(int)PlaneSide.TOP].setCoefficients(-mvp.M21 + mvp.M41, -mvp.M22 + mvp.M42, -mvp.M23 + mvp.M43, -mvp.M24 + mvp.M44);


            pl[(int)PlaneSide.LEFT].setCoefficients(mvp.M11 + mvp.M41, mvp.M12 + mvp.M42, mvp.M13 + mvp.M43, mvp.M14 + mvp.M44);


            pl[(int)PlaneSide.RIGHT].setCoefficients(-mvp.M11 + mvp.M41, -mvp.M12 + mvp.M42, -mvp.M13 + mvp.M43, -mvp.M14 + mvp.M44);

        }

        public bool PointInFrustum(float3 point)
        {
            //Loop through the planes and check the distance against the point
            for (int i = 0; i < 6; i++)
            {
                float dist = pl[i].distance(point);
                
                if (dist < 0)
                {
                    //Debug.WriteLine("dist: "+dist+" | point: "+point+" | Index: "+i);
                    return false;
                }
            }
            return true;
        }


        public bool SphereInFrustum(float3 point, float radius)
        {
            
            for (int i = 0; i < 6; i++)
                if (pl[i].distance(point) <= -radius)
                {
                    return false;
                }
                    
            return true;
        }

       

    }
}
