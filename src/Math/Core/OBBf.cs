using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents an oriented bounding box.
    /// </summary>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct OBBf
    {
        /// <summary>
        ///     The minimum values of the oriented bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(1)] public float3 Min;

        /// <summary>
        ///     The maximum values of the oriented bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(2)] public float3 Max;

        /// <summary>
        ///     The roation of the oriented bounding box
        /// </summary>
        [ProtoMember(3)] public float4x4 Rotation;

        /// <summary>
        ///     The translation of the oriented bounding box
        /// </summary>
        [ProtoMember(4)] public float3 Translation;

        /// <summary>
        ///     Returns the center of the bounding box
        /// </summary>
        public float3 Center => (Max + Min) * 0.5f;

        /// <summary>
        ///     Returns the with, height and depth of the box in x, y and z
        /// </summary>
        public float3 Size
        {
            get { return (Max - Min); }
        }

        //public List<float3> MinCubeVerts;           


        /// <summary>
        ///     Create a new axis aligned bounding box
        /// </summary>
        /// <param name="min_">the minimum x y and z values</param>
        /// <param name="max_">the maximum x y and z values</param>
        /// <param name="rotation_">the rotation of this box</param>
        /// <param name="translation_">the translation of this box</param>
        public OBBf(float3 min_, float3 max_, float4x4 rotation_, float3 translation_)
        {
            Min = min_;
            Max = max_;
            Rotation = rotation_;
            Translation = translation_;            
        }

        /// <summary>
        ///     Generates a new  oriented bounding box from a given set of vertices or points
        /// </summary>
        /// <param name="meshVertices"></param>
        public OBBf(float3[] vertices)
        {
            Translation = M.CalculateCentroid(vertices);
            var covarianceMatrix = M.CreateCovarianceMatrix(Translation, vertices);
            var eigen = M.EigenFromCovarianceMat(covarianceMatrix);

            Rotation = eigen.Vectors;

            var changeBasis = Rotation.Invert();

            Min = vertices[0];
            Max = vertices[0];

            for (var i = 0; i < vertices.Count(); i++)
            {
                var currentPointTranslated = vertices[i] - Translation;
                var currentPointTranslatedAndRotated = changeBasis * currentPointTranslated;

                this |= currentPointTranslatedAndRotated;
                //vertices[i] = currentPointTranslatedAndRotated;
            }

         

            //MinCubeVerts = new List<float3>
            //{
            //    new float3(Min.x, Min.y, Min.z),
            //    new float3(Max.x, Min.y, Min.z),
            //    new float3(Max.x, Min.y, Max.z),
            //    new float3(Min.x, Min.y, Max.z),
            //    new float3(Min.x, Max.y, Min.z),
            //    new float3(Max.x, Max.y, Min.z),
            //    new float3(Max.x, Max.y, Max.z),
            //    new float3(Min.x, Max.y, Max.z)
            //};

            //Dimensions = CalcCubeDimensionMin();
            //CornerVerts = CalcCubeVertsFromCenter();

            //if (rotateBack)
            //{
            //    for (var i = 0; i < CornerVerts.Length; i++)
            //    {
            //        var vertex = CornerVerts[i];
            //        vertex = Rotation * vertex;
            //        vertex += Translation;
            //        CornerVerts[i] = vertex;
            //    }

            //    CalcCubeCenter(CornerVerts);

            //    for (var i = 0; i < vertices.Count; i++)
            //    {
            //        vertices[i] = Rotation * vertices[i];
            //        vertices[i] += Translation;
            //    }
            //}
        }

        /// <summary>
        ///     Calculates the bounding box around an existing bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        public static OBBf Union(OBBf a, float3 p)
        {
            OBBf ret;
            ret.Translation = a.Translation;
            ret.Rotation = a.Rotation;
            ret.Min.x = (a.Min.x < p.x) ? a.Min.x : p.x;
            ret.Min.y = (a.Min.y < p.y) ? a.Min.y : p.y;
            ret.Min.z = (a.Min.z < p.z) ? a.Min.z : p.z;
            ret.Max.x = (a.Max.x > p.x) ? a.Max.x : p.x;
            ret.Max.y = (a.Max.y > p.y) ? a.Max.y : p.y;
            ret.Max.z = (a.Max.z > p.z) ? a.Max.z : p.z;
            return ret;
        }

        /// <summary>
        ///     Calculates the oriented bounding box around an existing oriented bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        /// <example>
        ///   Use this operator e.g. to calculate the bounding box for a given list of points.
        ///   <code>
        ///     OBBf box = new OOB(pointList.First(), pointList.First());
        ///     foreach (float3 p in pointList)
        ///         box |= p;
        ///   </code>
        /// </example>
        public static OBBf operator |(OBBf a, float3 p) => Union(a, p);
    }


}
