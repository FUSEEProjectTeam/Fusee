using System;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math
{
    /// <summary>
    /// Represents an axis aligned bounding box.
    /// </summary>
    [ProtoContract]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBf
    {
        /// <summary>
        /// The minimum values of the axis aligned bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(1)]
        public float3 min;

        /// <summary>
        /// The maximum values of the axis aligned bounding box in x, y and z direction
        /// </summary>  
        [ProtoMember(2)]
        public float3 max;

        /// <summary>
        /// Create a new axis aligned bounding box
        /// </summary>
        /// <param name="min_">the minimum x y and z values</param>
        /// <param name="max_">the maximum x y and z values</param>
        public AABBf(float3 min_, float3 max_)
        {
            min = min_;
            max = max_;
        }

        /// <summary>
        /// Applies a tranformation on the bounding box. After the tranformation another
        /// axis alignes bounding box results. This is done by transforming all eight 
        /// vertices of the box and re-aligning to the axes afterwards.
        /// </summary>
        /// <param name="m">The transformation matrix</param>
        /// <param name="box">the box to transform</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static AABBf operator *(float4x4 m, AABBf box)
        {
            float3[] cube =
            {
                new float3(box.min.x, box.min.y, box.min.z),
                new float3(box.min.x, box.min.y, box.max.z),
                new float3(box.min.x, box.max.y, box.min.z),
                new float3(box.min.x, box.max.y, box.max.z),
                new float3(box.max.x, box.min.y, box.min.z),
                new float3(box.max.x, box.min.y, box.max.z),
                new float3(box.max.x, box.max.y, box.min.z),
                new float3(box.max.x, box.max.y, box.max.z),
            };

            for (int i = 0; i < 8; i++)
            {
                cube[i] = m*cube[i];
            }

            AABBf ret;
            ret.min = cube[0];
            ret.max = cube[0];
            for (int i = 1; i < 8; i++)
            {
                if (cube[i].x < ret.min.x) ret.min.x = cube[i].x;
                if (cube[i].y < ret.min.y) ret.min.y = cube[i].y;
                if (cube[i].z < ret.min.z) ret.min.z = cube[i].z;
                if (cube[i].x > ret.max.x) ret.max.x = cube[i].x;
                if (cube[i].y > ret.max.y) ret.max.y = cube[i].y;
                if (cube[i].z > ret.max.z) ret.max.z = cube[i].z;
            }
            return ret;
        }

        /// <summary>
        /// Calculates the bounding box around two existing bounding boxes.
        /// </summary>
        /// <param name="a">One of the bounding boxes to build the union from</param>
        /// <param name="b">The other bounding boxe to build the union from</param>
        /// <returns>The smallest axis aligned bounding box containing both input boxes</returns>
        public static AABBf Union(AABBf a, AABBf b)
        {
            AABBf ret;
            ret.min.x = (a.min.x < b.min.x) ? a.min.x : b.min.x;
            ret.min.y = (a.min.y < b.min.y) ? a.min.y : b.min.y;
            ret.min.z = (a.min.z < b.min.z) ? a.min.z : b.min.z;
            ret.max.x = (a.max.x > b.max.x) ? a.max.x : b.max.x;
            ret.max.y = (a.max.y > b.max.y) ? a.max.y : b.max.y;
            ret.max.z = (a.max.z > b.max.z) ? a.max.z : b.max.z;
            return ret;
        }

        /// <summary>
        /// Returns the center of the bounding box
        /// </summary>
        public float3 Center { get { return (max + min)*0.5f; } }

        /// <summary>
        /// Returns the with, height and depth of the box in x, y and z
        /// </summary>
        public float3 Size { get { return (max - min); } }
    }
}
