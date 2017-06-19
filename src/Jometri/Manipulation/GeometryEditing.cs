using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    /// <summary>
    /// Provides public methods to edit and manipulate geometry which is stroed as a DCEL
    /// </summary>
    public static class GeometryEditing
    {

        /// <summary>
        /// Extrudes a given Face by a given offset along its normal vector.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="faceHandle">The handle of the face to extrude.</param>
        /// <param name="offset">How far the face shoul get ertuded.</param>
        /// <returns></returns>
        public static Geometry ExtrudeFace(this Geometry geometry, int faceHandle, float offset)
        {
            Face face = geometry.GetFaceByHandle(faceHandle);
            return ExtrudeFaceByHandle(geometry, faceHandle, offset, face.FaceData.FaceNormal);
        }

        /// <summary>
        /// Extrudes a given Face along a specified Vector.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="faceHandle"></param>
        /// <param name="offset"></param>
        /// <param name="extrusionVector"></param>
        /// <returns></returns>
        public static Geometry ExtrudeFace(this Geometry geometry, int faceHandle, float offset, float3 extrusionVector)
        {
            extrusionVector.Normalize();
            return ExtrudeFaceByHandle(geometry, faceHandle, offset, extrusionVector);
        }

        /// <summary>
        /// Insets a Face with a given offset. The new central Face has the same Handle as the original Face.
        /// </summary>
        /// <param name="geometry">The geometry on which to perform a inset as DCEL.</param>
        /// <param name="faceHandle">The Handle of the face which will be inseted.</param>
        /// <param name="insetOffset">The offset of the inset in percent between 0 and 1. .5f means 50% of the original face stays.</param> todo: change desciption!
        /// <returns>Returns the geometry with edited faces.</returns>
        public static Geometry InsetFace(this Geometry geometry, int faceHandle, float insetOffset)
        {

            if (insetOffset >= 1)throw new ArgumentException("insetOffset can not be greate or equal to 1.");
            if (insetOffset <= 0)throw new ArgumentException("insetOffset can not be smaller or equal to 0.");

            Face face = geometry.GetFaceByHandle(faceHandle);
            var allFaceVertices = geometry.GetFaceVertices(faceHandle).ToList();
            float3 meanPos = GeometricOperations.GetVerticesMeanPos(allFaceVertices);

            //Dict sotres countEdges; [0] = edge1.handle, [1] = edge2twin.handle, [2] = edge3.handle, [3] = vertex.Handle
            var edgeStorage = new Dictionary<int, int[]>();

            //
            int countEdges = 0;

            HalfEdge start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            HalfEdge next = start;

            do
            {
                int nextEdge = next.NextHalfEdge;
                Vertex currentVertex = geometry.GetVertexByHandle(next.OriginVertex);

                float3 currentPos = currentVertex.VertData.Pos;
                float3 newPos = ((currentPos - meanPos) * insetOffset) + meanPos;

                Vertex newVertex = new Vertex(geometry.CreateVertHandleId(), newPos);
                HalfEdge nextNext = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
                HalfEdge edge1 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                HalfEdge edge2Twin = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                HalfEdge edge2 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                HalfEdge edge3 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                Face newFace = new Face(geometry.CreateFaceHandleId());

                //store info
                edgeStorage.Add(countEdges, new[] { edge1.Handle, edge2Twin.Handle, edge3.Handle, newVertex.Handle});

                newVertex.IncidentHalfEdge = edge3.Handle;
                newFace.OuterHalfEdge = edge1.Handle;
                edge1.OriginVertex = nextNext.OriginVertex;
                edge3.OriginVertex = newVertex.Handle;
                edge2Twin.OriginVertex = newVertex.Handle;
                //twins
                edge2.TwinHalfEdge = edge2Twin.Handle;
                edge2Twin.TwinHalfEdge = edge2.Handle;
                //nexts
                edge1.NextHalfEdge = edge2.Handle;
                edge2.NextHalfEdge = edge3.Handle;
                edge3.NextHalfEdge = next.Handle;
                next.NextHalfEdge = edge1.Handle;
                //prevs
                edge1.PrevHalfEdge = next.Handle;
                edge2.PrevHalfEdge = edge1.Handle;
                edge3.PrevHalfEdge = edge2.Handle;
                next.PrevHalfEdge = edge3.Handle;
                //face
                edge1.IncidentFace = newFace.Handle;
                edge2.IncidentFace = newFace.Handle;
                edge3.IncidentFace = newFace.Handle;
                next.IncidentFace = newFace.Handle;
                edge2Twin.IncidentFace = face.Handle;

                //write changes 
                geometry.DictVertices.Add(newVertex.Handle, newVertex);
                geometry.DictFaces.Add(newFace.Handle, newFace);
                geometry.DictHalfEdges.Add(edge1.Handle, edge1);
                geometry.DictHalfEdges.Add(edge2Twin.Handle, edge2Twin);
                geometry.DictHalfEdges.Add(edge2.Handle, edge2);
                geometry.DictHalfEdges.Add(edge3.Handle, edge3);
                geometry.ReplaceHalfEdge(next);

                countEdges++;
                next = geometry.GetHalfEdgeByHandle(nextEdge);
            } while (start != next);

            for (int i = 0; i < countEdges; i++)
            {
                int prevFace = i - 1;
                int nextFace = i + 1;

                int[] faceData = edgeStorage[i];

                if (i == 0) prevFace = countEdges-1;
                if (i == countEdges - 1)
                {
                    nextFace = 0;
                    face.OuterHalfEdge = faceData[1];
                    geometry.ReplaceFace(face);
                }

                int[] prevFaceData = edgeStorage[prevFace];
                int[] nextFaceData = edgeStorage[nextFace];

                HalfEdge edge2Twin = geometry.GetHalfEdgeByHandle(faceData[1]);
                HalfEdge edge3 = geometry.GetHalfEdgeByHandle(faceData[2]);
                HalfEdge edge3Twin = geometry.GetHalfEdgeByHandle(prevFaceData[0]);
                HalfEdge edge2 = geometry.GetHalfEdgeByHandle(edge2Twin.TwinHalfEdge);

                edge2Twin.PrevHalfEdge = prevFaceData[1];
                edge2Twin.NextHalfEdge = nextFaceData[1];
                edge2.OriginVertex = nextFaceData[3];
                edge3Twin.TwinHalfEdge = edge3.Handle;
                edge3.TwinHalfEdge = edge3Twin.Handle;
                
                //write
                geometry.ReplaceHalfEdge(edge2Twin);
                geometry.ReplaceHalfEdge(edge2);
                geometry.ReplaceHalfEdge(edge3Twin);
                geometry.ReplaceHalfEdge(edge3);

                //set face normal
                geometry.SetFaceNormal(geometry.GetFaceVertices(edge2.IncidentFace).ToList(),geometry.GetFaceByHandle(edge2.IncidentFace));
            }

            return geometry;
        }

        private static Geometry ExtrudeFaceByHandle(Geometry geometry, int faceHandle, float offset, float3 extrusionVector)
        {
            Geometry oldGeometry = geometry.CloneGeometry();
            Face face = geometry.GetFaceByHandle(faceHandle);

            //get HE of Face
            HalfEdge start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            HalfEdge next = start;

            List<HalfEdge> allH2NEdges = new List<HalfEdge>();

            do
            {
                Vertex nextOriginV = geometry.GetVertexByHandle(next.OriginVertex);
                Vertex newVertex = new Vertex(geometry.CreateVertHandleId(), nextOriginV.VertData.Pos);

                HalfEdge twinEdge = geometry.GetHalfEdgeByHandle(next.TwinHalfEdge);
                HalfEdge prevEdge = geometry.GetHalfEdgeByHandle(next.PrevHalfEdge);
                HalfEdge prevTwinEdge = geometry.GetHalfEdgeByHandle(prevEdge.TwinHalfEdge);

                nextOriginV.VertData.Pos = nextOriginV.VertData.Pos + extrusionVector * offset;

                HalfEdge h4 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                HalfEdge h2n = new HalfEdge(geometry.CreateHalfEdgeHandleId());

                HalfEdge h1 = new HalfEdge(geometry.CreateHalfEdgeHandleId());

                var allIncomingHe = oldGeometry.GetVertexStartingHalfEdges(nextOriginV.Handle);
                foreach (HalfEdge halfEdge in allIncomingHe)
                {
                    if (halfEdge != next)
                    {
                        var edge = UpdateHalfEdgeOrigin(halfEdge, newVertex.Handle);
                        geometry.ReplaceHalfEdge(edge);
                    }
                }

                nextOriginV.IncidentHalfEdge = next.Handle;

                h4.OriginVertex = nextOriginV.Handle;
                h2n.OriginVertex = newVertex.Handle;
                h1.OriginVertex = newVertex.Handle;

                h4.TwinHalfEdge = h2n.Handle;
                h2n.TwinHalfEdge = h4.Handle;

                h4.NextHalfEdge = h1.Handle;
                h1.PrevHalfEdge = h4.Handle;

                h1.TwinHalfEdge = next.TwinHalfEdge;
                twinEdge.TwinHalfEdge = h1.Handle;

                prevTwinEdge.OriginVertex = newVertex.Handle;

                newVertex.IncidentHalfEdge = h2n.Handle;

                geometry.ReplaceHalfEdge(twinEdge);
                geometry.ReplaceHalfEdge(prevTwinEdge);
                geometry.ReplaceVertex(nextOriginV);
                geometry.DictVertices.Add(newVertex.Handle, newVertex);
                geometry.DictHalfEdges.Add(h4.Handle, h4);
                geometry.DictHalfEdges.Add(h1.Handle, h1);
                geometry.DictHalfEdges.Add(h2n.Handle, h2n);

                allH2NEdges.Add(h2n);

                next = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
            } while (start != next);

            start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            next = start;
            do
            {
                Face newFace = new Face(geometry.CreateFaceHandleId());

                HalfEdge twinEdge = geometry.GetHalfEdgeByHandle(next.TwinHalfEdge);

                HalfEdge h1 = geometry.GetHalfEdgeByHandle(twinEdge.TwinHalfEdge);
                HalfEdge h2 = allH2NEdges.First(n => n.OriginVertex == twinEdge.OriginVertex);
                HalfEdge h3 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                HalfEdge h4 = geometry.GetHalfEdgeByHandle(h1.PrevHalfEdge);

                //set Face
                h1.IncidentFace = newFace.Handle;
                h2.IncidentFace = newFace.Handle;
                h3.IncidentFace = newFace.Handle;
                h4.IncidentFace = newFace.Handle;

                h1.NextHalfEdge = h2.Handle;
                h2.NextHalfEdge = h3.Handle;
                h3.NextHalfEdge = h4.Handle;
                h4.NextHalfEdge = h1.Handle;

                h1.PrevHalfEdge = h4.Handle;
                h2.PrevHalfEdge = h1.Handle;
                h3.PrevHalfEdge = h2.Handle;
                h4.PrevHalfEdge = h3.Handle;

                h3.TwinHalfEdge = next.Handle;
                h3.OriginVertex = geometry.GetHalfEdgeByHandle(next.NextHalfEdge).OriginVertex;
                next.TwinHalfEdge = h3.Handle;
                newFace.OuterHalfEdge = h1.Handle;

                //write all changes
                geometry.ReplaceHalfEdge(h1);
                geometry.ReplaceHalfEdge(h2);
                geometry.ReplaceHalfEdge(h4);
                geometry.ReplaceHalfEdge(next);

                geometry.DictHalfEdges.Add(h3.Handle, h3);
                geometry.DictFaces.Add(newFace.Handle, newFace);

                newFace.FaceData.FaceNormal = GeometricOperations.CalculateFaceNormal(geometry.GetFaceVertices(newFace.Handle).ToList());

                geometry.ReplaceFace(newFace);

                next = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
            } while (start != next);

            return geometry;
        }

        internal static HalfEdge UpdateHalfEdgeOrigin(HalfEdge edge, int newVHandle)
        {
            edge.OriginVertex = newVHandle;
            return edge;
        }
    }
}