using Fusee.Engine.Common;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Creates a polygonal line.
    /// </summary>
    public class Line : Mesh
    {
        /// <summary>
        /// Creates a instance of type 'Line'.
        /// </summary>
        /// <param name="points">The vertices, the line should connect.</param>
        /// <param name="lineThickness">The thickness of the line.</param>
        /// <param name="rectWidth"></param>
        /// <param name="rectHeight"></param>       
        public Line(List<float3> points, float lineThickness, float rectWidth = 1, float rectHeight = 1)
        {
            var segmentCache = new float3[4];

            var verts = new List<float3>();
            var normals = new List<float3>();
            var tris = new List<ushort>();
            var uvs = new List<float2>();

            var segmentCount = points.Count - 1;

            for (var i = 0; i <= segmentCount - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];

                //range [-0.5, 0.5]
                if (rectHeight != 1)
                {
                    start.y = ((start.y / rectHeight) * 2 - 1) / 2;
                    end.y = ((end.y / rectHeight) * 2 - 1) / 2;
                }
                if (rectWidth != 1)
                {
                    start.x = ((start.x / rectWidth) * 2 - 1) / 2;
                    end.x = ((end.x / rectWidth) * 2 - 1) / 2;
                }

                var dirVec = end - start;
                dirVec.Normalize();
                var angleToXAxis = (float)System.Math.Atan2(end.y - start.y, end.x - start.x);

                var perpendicularVec = RotateVectorInXYPlane(float3.UnitY, angleToXAxis);
                perpendicularVec.Normalize();

                var v0 = start + -perpendicularVec * lineThickness / 2;
                var v1 = start + perpendicularVec * lineThickness / 2;
                var v2 = end + perpendicularVec * lineThickness / 2;
                var v3 = end + -perpendicularVec * lineThickness / 2;

                if (i == 0 && segmentCount == 1)
                {
                    verts.Add(v0);
                    verts.Add(v1);
                    verts.Add(v3);
                    verts.Add(v2);

                    uvs.Add(new float2(0, 0));
                    uvs.Add(new float2(0, 1));
                    uvs.Add(new float2(1, 1));
                    uvs.Add(new float2(1, 0));

                    normals.Add(-float3.UnitZ);
                    normals.Add(-float3.UnitZ);
                    normals.Add(-float3.UnitZ);
                    normals.Add(-float3.UnitZ);

                    var lastVertexIndex = (verts.Count - 1);

                    tris.Add((ushort)(lastVertexIndex - 3));
                    tris.Add((ushort)(lastVertexIndex));
                    tris.Add((ushort)(lastVertexIndex - 2));
                    tris.Add((ushort)(lastVertexIndex - 3));
                    tris.Add((ushort)(lastVertexIndex - 1));
                    tris.Add((ushort)(lastVertexIndex));
                }
                else
                {

                    if (i > 0 && i < segmentCount - 1)
                    {
                        var vec03Cache = segmentCache[3] - segmentCache[0];
                        var vec03CacheLength = vec03Cache.Length;
                        vec03Cache.Normalize();

                        var vec30 = v0 - v3;
                        var vec30Length = vec30.Length;
                        vec30.Normalize();

                        var vec12Cache = segmentCache[2] - segmentCache[1];
                        var vec12CacheLength = vec12Cache.Length;
                        vec12Cache.Normalize();

                        var vec21 = v1 - v2;
                        var vec21Length = vec21.Length;
                        vec21.Normalize();

                        //calculate inter-segment vertices
                        Jometri.GeometricOperations.IsLineIntersectingLine(segmentCache[0],
                            segmentCache[3] + (vec03CacheLength * vec03Cache), v0 + (vec30Length * vec30), v3,
                            out var intersectionPoint1);

                        Jometri.GeometricOperations.IsLineIntersectingLine(segmentCache[1],
                            segmentCache[2] + (vec12CacheLength * vec12Cache), v1 + (vec21Length * vec21), v2,
                            out var intersectionPoint2);

                        verts.Add(intersectionPoint1);
                        verts.Add(intersectionPoint2);

                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);

                        uvs.Add(new float2(1 * i, 1));
                        uvs.Add(new float2(1 * i, 0));

                    }
                    else if (i == 0)
                    {
                        verts.Add(v0);
                        verts.Add(v1);

                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);

                        uvs.Add(new float2(0, 0));
                        uvs.Add(new float2(0, 1));
                    }
                    else if (i == segmentCount - 1)
                    {
                        var vec03Cache = segmentCache[3] - segmentCache[0];
                        var vec03CacheLength = vec03Cache.Length;
                        vec03Cache.Normalize();

                        var vec30 = v0 - v3;
                        var vec30Length = vec30.Length;
                        vec30.Normalize();

                        var vec12Cache = segmentCache[2] - segmentCache[1];
                        var vec12CacheLength = vec12Cache.Length;
                        vec12Cache.Normalize();

                        var vec21 = v1 - v2;
                        var vec21Length = vec21.Length;
                        vec21.Normalize();

                        //calculate inter-segment vertices
                        Jometri.GeometricOperations.IsLineIntersectingLine(segmentCache[0],
                            segmentCache[3] + (vec03CacheLength * vec03Cache), v0 + (vec30Length * vec30), v3,
                            out float3 intersectionPoint1);

                        Jometri.GeometricOperations.IsLineIntersectingLine(segmentCache[1],
                            segmentCache[2] + (vec12CacheLength * vec12Cache), v1 + (vec21Length * vec21), v2,
                            out float3 intersectionPoint2);

                        verts.Add(intersectionPoint1);
                        verts.Add(intersectionPoint2);

                        verts.Add(v3);
                        verts.Add(v2);

                        uvs.Add(new float2(i, 0));
                        uvs.Add(new float2(i, 1));
                        uvs.Add(new float2(i + 1, 1));
                        uvs.Add(new float2(i + 1, 0));

                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);
                    }

                    segmentCache[0] = v0;
                    segmentCache[1] = v1;
                    segmentCache[2] = v2;
                    segmentCache[3] = v3;

                    var lastVertexIndex = (verts.Count - 1);

                    if (i > 0)
                    {
                        if (i == segmentCount - 1)
                        {
                            //segment before the last
                            tris.Add((ushort)(lastVertexIndex - 5));
                            tris.Add((ushort)(lastVertexIndex - 2));
                            tris.Add((ushort)(lastVertexIndex - 4));
                            tris.Add((ushort)(lastVertexIndex - 5));
                            tris.Add((ushort)(lastVertexIndex - 3));
                            tris.Add((ushort)(lastVertexIndex - 2));

                            uvs.Add(new float2(0, 0));
                            uvs.Add(new float2(0, 1 * i));
                            uvs.Add(new float2(1 * i, 1 * i));
                            uvs.Add(new float2(0, 1 * i));
                        }

                        tris.Add((ushort)(lastVertexIndex - 3));
                        tris.Add((ushort)(lastVertexIndex));
                        tris.Add((ushort)(lastVertexIndex - 2));
                        tris.Add((ushort)(lastVertexIndex - 3));
                        tris.Add((ushort)(lastVertexIndex - 1));
                        tris.Add((ushort)(lastVertexIndex));

                    }
                }

            }

            Vertices = verts.ToArray();
            Normals = normals.ToArray();
            Triangles = tris.ToArray();
            UVs = uvs.ToArray();
        }

        private float3 RotateVectorInXYPlane(float3 vec, float angle)
        {
            var x = (float)(vec.x * System.Math.Cos(angle) - vec.y * System.Math.Sin(angle));
            var y = (float)(vec.x * System.Math.Sin(angle) + vec.y * System.Math.Cos(angle));
            var z = 0;

            return new float3(x, y, z);
        }
    }
}