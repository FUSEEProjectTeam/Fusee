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
        private enum SegmentType
        {
            Start,
            Middle,
            End,
            Full,
        }

        public enum JoinType
        {
            Bevel,
            Miter
        }

        private float MIN_MITER_JOIN = M.DegreesToRadians(15);

        // A bevel 'nice' join displaces the vertices of the line segment instead of simply rendering a
        // quad to connect the endpoints. This improves the look of textured and transparent lines, since
        // there is no overlapping.
        private  float MIN_BEVEL_NICE_JOIN = M.DegreesToRadians(30);      

                      
        private float _lineThickness;
    
        internal bool relativeSize;
      
        internal bool lineCaps;        

        public bool LineCaps
        {
            get { return lineCaps; }
            set { lineCaps = value;  }
        }

       
        public JoinType LineJoins = JoinType.Bevel;    


        

        public Line(float enclosingRectWidth, float enclosingRectHeight, float3[]Points, float lineThickness)
        {
            _lineThickness = lineThickness;

            var verts = new List<float3>();
            var normals = new List<float3>();
            var tris = new List<ushort>();
            
            if (Points == null)
                return;
            
            // scale based on the size of the rect or use absolute, this is switchable
            var sizeX = enclosingRectWidth;
            var sizeY = enclosingRectHeight;
            var offsetX = 0;
            var offsetY = 0;           

            // Generate the quads that make up the wide line
            var segments = new List<float3[]>();

            ushort lastVertIndex;

            for (var i = 1; i < Points.Length; i++)
            {
                var start = Points[i - 1];
                var end = Points[i];
                start = new float3(start.x * sizeX + offsetX, start.y * sizeY + offsetY,0);
                end = new float3(end.x * sizeX + offsetX, end.y * sizeY + offsetY,0);

                if (lineCaps && i == 1)
                {
                    segments.Add(CreateLineCap(start, end, SegmentType.Start));
                }

                segments.Add(CreateLineSegment(start, end, SegmentType.Middle));
                //segments.Add(CreateLineSegment(start, end, SegmentType.Full));

                if (lineCaps && i == Points.Length - 1)
                {
                    segments.Add(CreateLineCap(start, end, SegmentType.End));
                }
            }

            // Add the line segments to the vertex helper, creating any joins as needed
            for (var i = 0; i < segments.Count; i++)
            {
                if (i < segments.Count - 1)
                {
                    var vec1 = segments[i][1] - segments[i][2];
                    var vec2 = segments[i + 1][2] - segments[i + 1][1];
                    var angle = Angle(new float2(vec1.x,vec1.y), new float2(vec2.x, vec2.y));
                    vec1.Normalize();
                    vec2.Normalize();

                    // Positive sign means the line is turning in a 'clockwise' direction
                    var sign = System.Math.Sign(float3.Cross(vec1, vec2).z);

                    // Calculate the miter point
                    var miterDistance = (float)(_lineThickness / (2f * System.Math.Tan(angle / 2f)));
                    var miterPointA = segments[i][2] - vec1 * miterDistance * sign;
                    var miterPointB = segments[i][3] + vec1 * miterDistance * sign;

                    var joinType = LineJoins;
                    if (joinType == JoinType.Miter)
                    {
                        // Make sure we can make a miter join without too many artifacts.
                        if (miterDistance < vec1.Length / 2 && miterDistance < vec2.Length / 2 && angle > MIN_MITER_JOIN)
                        {
                            segments[i][2] = miterPointA;
                            segments[i][3] = miterPointB;
                            segments[i + 1][0] = miterPointB;
                            segments[i + 1][1] = miterPointA;
                        }
                        else
                        {
                            joinType = JoinType.Bevel;
                        }
                    }

                    if (joinType == JoinType.Bevel)
                    {
                        if (miterDistance < vec1.Length / 2 && miterDistance < vec2.Length / 2 && angle > MIN_BEVEL_NICE_JOIN)
                        {
                            if (sign < 0)
                            {
                                segments[i][2] = miterPointA;
                                segments[i + 1][1] = miterPointA;
                            }
                            else
                            {
                                segments[i][3] = miterPointB;
                                segments[i + 1][0] = miterPointB;
                            }
                        }

                        lastVertIndex = (ushort)(verts.Count - 1);
                        var quat = new float3[] { segments[i][2], segments[i][3], segments[i + 1][0], segments[i + 1][1] };
                        var quatNormals = new float3[] { -float3.UnitZ, -float3.UnitZ, -float3.UnitZ, -float3.UnitZ };
                        
                        verts.AddRange(quat);
                        normals.AddRange(quatNormals);

                    }
                }
                lastVertIndex = (ushort)(verts.Count - 1);
                verts.AddRange(segments[i]);
                normals.AddRange(new float3[] { -float3.UnitZ, -float3.UnitZ, -float3.UnitZ, -float3.UnitZ });
                tris.AddRange(new ushort[] { (ushort)(lastVertIndex + 1), (ushort)(lastVertIndex + 3), (ushort)(lastVertIndex + 2),
                                        (ushort)(lastVertIndex + 1), (ushort)(lastVertIndex + 4), (ushort)(lastVertIndex + 3)});
                
            }

            Vertices = verts.ToArray();
            Normals = normals.ToArray();
            Triangles = tris.ToArray();

            
        }

        private float3[] CreateLineCap(float3 start, float3 end, SegmentType type)
        {
            var vec = (end - start);
            vec.Normalize();

            switch (type)
            {
                case SegmentType.Start:
                    {
                        var capStart = start - (vec * _lineThickness / 2);
                        return CreateLineSegment(capStart, start, SegmentType.Start);
                    }

                case SegmentType.End:
                    {
                        var capEnd = end + (vec * _lineThickness / 2);
                        return CreateLineSegment(end, capEnd, SegmentType.End);
                    }
            }

            throw new ArgumentException("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
            
        }

        private float3[] CreateLineSegment(float3 start, float3 end, SegmentType type)
        {
            var offset = new float3((start.y - end.y), end.x - start.x, 0);
            offset.Normalize();
            offset *= _lineThickness / 2;

            var v1 = start - offset;
            var v2 = start + offset;
            var v3 = end + offset;
            var v4 = end - offset;

            return new float3[] { v1, v2, v3, v4 };

            //Return the VDO with the correct uvs
            //switch (type)
            //{
            //    case SegmentType.Start:
            //        return SetVbo(new[] { v1, v2, v3, v4 }, startUvs);
            //    case SegmentType.End:
            //        return SetVbo(new[] { v1, v2, v3, v4 }, endUvs);
            //    case SegmentType.Full:
            //        return SetVbo(new[] { v1, v2, v3, v4 }, fullUvs);
            //    default:
            //        return SetVbo(new[] { v1, v2, v3, v4 }, middleUvs);
            //}
        }


        // see: https://stackoverflow.com/questions/21483999/using-atan2-to-find-angle-between-two-vectors
        /// <summary>
        /// Returns the angle between two vectos
        /// </summary>
        public static double Angle(float2 A, float2 B)
        {
            // |A·B| = |A| |B| COS(θ)
            // |A×B| = |A| |B| SIN(θ)

            return System.Math.Atan2(Cross(A, B), Dot(A, B));
        }

       
        public static double Dot(float2 A, float2 B)
        {
            return A.x * B.x + A.y * B.y;
        }
        public static double Cross(float2 A, float2 B)
        {
            return A.x * B.y - A.x * B.y;
        }
    }
}