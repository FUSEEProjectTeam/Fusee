/// Credit jack.sydorenko, firagon
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/
/// Updated/Refactored from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/#post-2528050

using Fusee.Math.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    public class Line : Mesh
    {
        public Line(IReadOnlyList<float3> points, float lineThickness)
        {
            var segmentCache = new float3[4];

            var verts = new List<float3>();
            var normals = new List<float3>();
            var tris = new List<ushort>();

            var segmentCount = points.Count - 1;

            for (var i = 0; i <= segmentCount - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];

                var dirVec = end - start;
                dirVec.Normalize();
                var angleToXAxis = System.Math.Atan2(end.y - start.y, end.x - start.x);

                var perendicularVec = RotateVectorInXYPlane(float3.UnitY, angleToXAxis);
                perendicularVec.Normalize();

                var v0 = start + -perendicularVec * lineThickness / 2;
                var v1 = start + perendicularVec * lineThickness / 2;
                var v2 = end + perendicularVec * lineThickness / 2;
                var v3 = end + -perendicularVec * lineThickness / 2;

                if (i == 0 && segmentCount == 1)
                {
                    verts.Add(v0);
                    verts.Add(v1);
                    verts.Add(v3);
                    verts.Add(v2);

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

                    }
                    else if (i == 0)
                    {
                        verts.Add(v0);
                        verts.Add(v1);

                        normals.Add(-float3.UnitZ);
                        normals.Add(-float3.UnitZ);

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
                            out var intersectionPoint1);

                        Jometri.GeometricOperations.IsLineIntersectingLine(segmentCache[1],
                            segmentCache[2] + (vec12CacheLength * vec12Cache), v1 + (vec21Length * vec21), v2,
                            out var intersectionPoint2);

                        verts.Add(intersectionPoint1);
                        verts.Add(intersectionPoint2);

                        verts.Add(v3);
                        verts.Add(v2);


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
                            //segment befor the last
                            tris.Add((ushort) (lastVertexIndex - 5));
                            tris.Add((ushort) (lastVertexIndex - 2));
                            tris.Add((ushort) (lastVertexIndex - 4));
                            tris.Add((ushort) (lastVertexIndex - 5));
                            tris.Add((ushort) (lastVertexIndex - 3));
                            tris.Add((ushort) (lastVertexIndex - 2));
                        }

                        tris.Add((ushort) (lastVertexIndex - 3));
                        tris.Add((ushort) (lastVertexIndex));
                        tris.Add((ushort) (lastVertexIndex - 2));
                        tris.Add((ushort) (lastVertexIndex - 3));
                        tris.Add((ushort) (lastVertexIndex - 1));
                        tris.Add((ushort) (lastVertexIndex));
                    }
                }

            }

            Vertices = verts.ToArray();
            Normals = normals.ToArray();
            Triangles = tris.ToArray();
        }
       
        private float3 RotateVectorInXYPlane(float3 vec, double angle)
        {
            var x = (float)(vec.x * System.Math.Cos(angle) - vec.y * System.Math.Sin(angle));
            var y = (float)(vec.x * System.Math.Sin(angle) + vec.y * System.Math.Cos(angle));
            var z = 0;

            return new float3(x, y, z);
        }
    }
}