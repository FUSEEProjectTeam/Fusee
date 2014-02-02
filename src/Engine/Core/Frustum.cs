//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using Fusee.Math;

//namespace Fusee.Engine
//{
//    // This is currently not used (WIP)
//    public class Frustum
//    {
//        private float[,] frustum = new float[6, 4];
//        private Plane[] pl = new Plane[6];
//        public Frustum(float4x4 mvp)
//        {
//            setFrustum(mvp);
//        }

//        private enum PlaneSide
//        {
//            NEARP, FARP, BOTTOM, TOP, LEFT, RIGHT,
//        }

//        private struct Plane
//        {
//            public float3 normal;
//            public float d;
//            public void setCoefficients(float a, float b, float c, float d)
//            {
//                // set the normal vector
//                normal = new float3(a, b, c);
//                //compute the lenght of the vector
//                float l = normal.Length;
//                // normalize the vector
//                normal = new float3(a / l, b / l, c / l);
//                // and divide d by th length as well
//                this.d = d / l;
//            }

//            public float distance(float3 p)
//            {
//                return (d + float3.Dot(normal, p));
//            }


//        }

//        public void setFrustum(float4x4 imvp)
//        {
//            pl[(int)PlaneSide.NEARP].setCoefficients(imvp.M31 + imvp.M41, imvp.M32 + imvp.M42, imvp.M33 + imvp.M43, imvp.M34 + imvp.M44);


//            pl[(int)PlaneSide.FARP].setCoefficients(-imvp.M31 + imvp.M41, -imvp.M32 + imvp.M42, -imvp.M33 + imvp.M43, -imvp.M34 + imvp.M44);


//            pl[(int)PlaneSide.BOTTOM].setCoefficients(imvp.M21 + imvp.M41, imvp.M22 + imvp.M42, imvp.M23 + imvp.M43, imvp.M24 + imvp.M44);


//            pl[(int)PlaneSide.TOP].setCoefficients(-imvp.M21 + imvp.M41, -imvp.M22 + imvp.M42, -imvp.M23 + imvp.M43, -imvp.M24 + imvp.M44);


//            pl[(int)PlaneSide.LEFT].setCoefficients(imvp.M11 + imvp.M41, imvp.M12 + imvp.M42, imvp.M13 + imvp.M43, imvp.M14 + imvp.M44);


//            pl[(int)PlaneSide.RIGHT].setCoefficients(-imvp.M11 + imvp.M41, -imvp.M12 + imvp.M42, -imvp.M13 + imvp.M43, -imvp.M14 + imvp.M44);

//        }

        

//        public bool PointInFrustum(float3 point)
//        {
//            bool result = true;
//            for (int i = 0; i < 6; i++)
//            {

//                if (pl[i].distance(point) < 0)
//                {
//                    return false;
//                }
//            }
//            return result;
//        }


//        public bool SphereInFrustum(float3 point, float radius)
//        {

//            for (int p = 0; p < 6; p++)
//            {
//                if (pl[p].distance(point) <= -radius)
//                {

//                    return false;
//                }
//            }
//            return true;
//        }

       

    

//    }
//}