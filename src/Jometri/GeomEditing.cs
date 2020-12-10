using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Jometri
{
    /// <summary>
    /// Provides methods to edit the components of a geometry.
    /// </summary>
    public static class GeomEditing
    {
        #region Insert new vertex

        /// <summary>
        /// Inserts a new Vertex between two given existing Vertices.
        /// </summary>
        /// <param name="geometry">The Geometry to insert a Vertex.</param>
        /// <param name="p">Handle of Vertex one.</param>
        /// <param name="q">Handle of Vertex two.</param>
        /// <param name="pos">Position of the new Vertex</param>
        /// <returns>New Vertex Handle.</returns>
        public static int InsertVertex(this Geometry geometry, int p, int q, float3 pos)
        {
            var adjacentVertices = geometry.GetVertexAdjacentVertices(p).ToList();
            for (var i = 0; i < adjacentVertices.Count; i++)
            {
                if (adjacentVertices[i].Handle == q) break;
                if (i == adjacentVertices.Count - 1) throw new ArgumentException("Vertices with Handle q=" + q + " and p=" + p + " are not adjacent!");
            }

            var newVertex = new Vertex(geometry.CreateVertHandleId(), pos);

            //add two new Half Edges
            var newHalfEdge1 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
            var newHalfEdge2 = new HalfEdge(geometry.CreateHalfEdgeHandleId());

            //set origin to new Vertex
            newHalfEdge1.OriginVertex = newVertex.Handle;
            newHalfEdge2.OriginVertex = newVertex.Handle;

            newVertex.IncidentHalfEdge = newHalfEdge2.Handle;

            var vertexP = geometry.GetVertexByHandle(p);
            var vertexQ = geometry.GetVertexByHandle(q);

            //Find Half Edge between p and q
            var incomingEdges = geometry.GetVertexStartingHalfEdges(vertexP.Handle);

            var he1 = new HalfEdge();
            var he2 = new HalfEdge();

            foreach (var halfEdge in incomingEdges)
            {
                var twinEdge = geometry.GetHalfEdgeByHandle(halfEdge.TwinHalfEdge);
                if (twinEdge.OriginVertex != vertexQ.Handle) continue;
                he1 = halfEdge;
                he2 = twinEdge;
            }
            var next1 = geometry.GetHalfEdgeByHandle(he2.NextHalfEdge);
            var next2 = geometry.GetHalfEdgeByHandle(he1.NextHalfEdge);

            //change handles
            he1.TwinHalfEdge = newHalfEdge1.Handle;
            newHalfEdge1.TwinHalfEdge = he1.Handle;
            newHalfEdge1.NextHalfEdge = he2.NextHalfEdge;
            he2.NextHalfEdge = newHalfEdge1.Handle;
            newHalfEdge1.PrevHalfEdge = he2.Handle;
            next1.PrevHalfEdge = newHalfEdge1.Handle;

            he2.TwinHalfEdge = newHalfEdge2.Handle;
            newHalfEdge2.TwinHalfEdge = he2.Handle;
            newHalfEdge2.NextHalfEdge = he1.NextHalfEdge;
            he1.NextHalfEdge = newHalfEdge2.Handle;
            newHalfEdge2.PrevHalfEdge = he1.Handle;
            next2.PrevHalfEdge = newHalfEdge2.Handle;

            //reconnect faces
            newHalfEdge1.IncidentFace = he2.IncidentFace;
            newHalfEdge2.IncidentFace = he1.IncidentFace;

            //replace existing Edges
            geometry.ReplaceHalfEdge(he1);
            geometry.ReplaceHalfEdge(he2);

            geometry.ReplaceHalfEdge(next1);
            geometry.ReplaceHalfEdge(next2);

            //add to dict
            geometry.DictVertices.Add(newVertex.Handle, newVertex);
            geometry.DictHalfEdges.Add(newHalfEdge1.Handle, newHalfEdge1);
            geometry.DictHalfEdges.Add(newHalfEdge2.Handle, newHalfEdge2);

            return newVertex.Handle;
        }
        #endregion

        #region Insert Diagonal

        /// <summary>
        /// Inserts a pair of HalfEdges between two (non adjacent) vertices of a Face.
        /// </summary>
        /// <param name="geometry">The Geometry to insert a diagonal.</param>
        /// <param name="p">First Vertex handle.</param>
        /// <param name="q">Second Vertex handle.</param>
        /// <exception cref="Exception"></exception>
        public static void InsertDiagonal(this Geometry geometry, int p, int q)
        {
            var pStartHe = new HalfEdge();
            var qStartHe = new HalfEdge();

            var face = geometry.GetFaceToInsertDiag(p, q, ref pStartHe, ref qStartHe);

            if (geometry.IsVertexAdjacentToVertex(p, q, pStartHe, qStartHe))
                throw new ArgumentException("A diagonal can't be inserted between adjacent Vertices!");

            var newFromP = new HalfEdge(geometry.CreateHalfEdgeHandleId());
            var newFromQ = new HalfEdge(geometry.CreateHalfEdgeHandleId());

            newFromP.OriginVertex = p;
            newFromP.NextHalfEdge = qStartHe.Handle;
            newFromP.PrevHalfEdge = pStartHe.PrevHalfEdge;
            newFromP.IncidentFace = face.Handle;

            newFromQ.OriginVertex = q;
            newFromQ.NextHalfEdge = pStartHe.Handle;
            newFromQ.PrevHalfEdge = qStartHe.PrevHalfEdge;
            newFromQ.IncidentFace = face.Handle;

            newFromP.TwinHalfEdge = newFromQ.Handle;
            newFromQ.TwinHalfEdge = newFromP.Handle;

            geometry.DictHalfEdges.Add(newFromP.Handle, newFromP);
            geometry.DictHalfEdges.Add(newFromQ.Handle, newFromQ);

            //Assign new successor to previous HalfEdges from p and q & assign new predecessor for qStartHe and pStartHe.
            var prevHeP = geometry.GetHalfEdgeByHandle(pStartHe.PrevHalfEdge);
            var prevHeQ = geometry.GetHalfEdgeByHandle(qStartHe.PrevHalfEdge);

            var prevHePUpdate = geometry.DictHalfEdges[prevHeP.Handle];
            prevHePUpdate.NextHalfEdge = newFromP.Handle;
            geometry.DictHalfEdges[prevHeP.Handle] = prevHePUpdate;

            var prevHeQUpdate = geometry.DictHalfEdges[prevHeQ.Handle];
            prevHeQUpdate.NextHalfEdge = newFromQ.Handle;
            geometry.DictHalfEdges[prevHeQ.Handle] = prevHeQUpdate;

            var nextHePUpdate = geometry.DictHalfEdges[pStartHe.Handle];
            nextHePUpdate.PrevHalfEdge = newFromQ.Handle;
            geometry.DictHalfEdges[pStartHe.Handle] = nextHePUpdate;

            var nextHeQUpdate = geometry.DictHalfEdges[qStartHe.Handle];
            nextHeQUpdate.PrevHalfEdge = newFromP.Handle;
            geometry.DictHalfEdges[qStartHe.Handle] = nextHeQUpdate;

            var holes = geometry.GetHoles(face);

            if (holes.Count != 0 && IsNewEdgeToHole(holes, p, q, face)) return;

            var newFace = new Face(geometry.CreateFaceHandleId(), newFromQ.Handle);

            //The face normal of the newFace equals the normal of the original Face because adding a diagonal does not change the face vertices position.
            var newFaceData = newFace.FaceData;
            newFaceData.FaceNormal = face.FaceData.FaceNormal;
            newFace.FaceData = newFaceData;

            geometry.DictFaces.Add(newFace.Handle, newFace);

            //Assign the handle of the new Face to its HalfEdges.
            geometry.AssignFaceHandle(newFace.OuterHalfEdge, newFace);

            //Set Face.OuterHalfEdge to newFromP - old OuterHalfEdge can be part of new Face now!
            var currentFace = face;
            currentFace.OuterHalfEdge = newFromP.Handle;
            face = currentFace;
            geometry.DictFaces[face.Handle] = face;
        }

        private static Dictionary<int, List<HalfEdge>> GetHoles(this Geometry geometry, Face face)
        {
            var holes = new Dictionary<int, List<HalfEdge>>();

            foreach (var he in face.InnerHalfEdges)
            {
                holes.Add(he, geometry.GetHalfEdgeLoop(he).ToList());
            }

            return holes;
        }

        private static void AssignFaceHandle(this Geometry geometry, int heHandle, Face newFace)
        {
            var oldFaceHandle = geometry.GetHalfEdgeByHandle(heHandle).IncidentFace;
            var currentHe = geometry.GetHalfEdgeByHandle(heHandle);
            do
            {
                currentHe.IncidentFace = newFace.Handle;

                geometry.DictHalfEdges[currentHe.Handle] = currentHe;

                currentHe = geometry.GetHalfEdgeByHandle(currentHe.NextHalfEdge);
            } while (currentHe.Handle != heHandle);

            //Assign newFace to possible holes in the "old" face.
            var oldFace = geometry.GetFaceByHandle(oldFaceHandle);
            if (oldFace.InnerHalfEdges.Count == 0) return;

            var inner = new List<int>();
            inner.AddRange(oldFace.InnerHalfEdges);

            foreach (var heh in inner)
            {
                var origin = geometry.GetHalfEdgeByHandle(heh).OriginVertex;

                if (!geometry.IsPointInPolygon(newFace, geometry.GetVertexByHandle(origin))) continue;

                oldFace.InnerHalfEdges.Remove(heh);
                newFace.InnerHalfEdges.Add(heh);

                var curHe = geometry.GetHalfEdgeByHandle(heh);
                do
                {
                    curHe.IncidentFace = newFace.Handle;

                    geometry.DictHalfEdges[curHe.Handle] = curHe;

                    curHe = geometry.GetHalfEdgeByHandle(curHe.NextHalfEdge);

                } while (curHe.Handle != heh);
            }
        }

        private static bool IsNewEdgeToHole(Dictionary<int, List<HalfEdge>> holes, int pHandle, int qHandle,
            Face face)
        {
            if (holes.Count == 0) return false;

            foreach (var hole in holes)
            {
                foreach (var heHandle in hole.Value)
                {
                    if (pHandle != heHandle.OriginVertex && qHandle != heHandle.OriginVertex) continue;

                    face.InnerHalfEdges.Remove(hole.Key);
                    return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Insets a Face with a given offset. The new, center Face has the same Handle as the original Face.
        /// </summary>
        /// <param name="geometry">The geometry on which to perform a face inset.</param>
        /// <param name="faceHandle">The Handle of the face, the new one will be inserted to.</param>
        /// <param name="insetOffset">The offset of the inset in percent. Use values between 0 and 1. A value of 0.5f means 50% of the original face remains.</param>
        /// <returns>Returns the geometry with edited faces.</returns>
        public static Geometry InsetFace(this Geometry geometry, int faceHandle, float insetOffset)
        {
            if (insetOffset >= 1) throw new ArgumentException("insetOffset can not be greater or equal to 1.");
            if (insetOffset <= 0) throw new ArgumentException("insetOffset can not be smaller or equal to 0.");

            var face = geometry.GetFaceByHandle(faceHandle);
            var allFaceVertices = geometry.GetFaceVertices(faceHandle).ToList();
            var meanPos = GeometricOperations.GetVerticesMeanPos(allFaceVertices);

            //Dict stores countEdges; [0] = edge1.handle, [1] = edge2twin.handle, [2] = edge3.handle, [3] = vertex.Handle
            var edgeStorage = new Dictionary<int, int[]>();

            var countEdges = 0;

            var start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            var next = start;

            do
            {
                var nextEdge = next.NextHalfEdge;
                var currentVertex = geometry.GetVertexByHandle(next.OriginVertex);

                var currentPos = currentVertex.VertData.Pos;
                var newPos = (currentPos - meanPos) * insetOffset + meanPos;

                var newVertex = new Vertex(geometry.CreateVertHandleId(), newPos);
                var nextNext = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
                var edge1 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var edge2Twin = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var edge2 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var edge3 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var newFace = new Face(geometry.CreateFaceHandleId());

                //store info
                edgeStorage.Add(countEdges, new[] { edge1.Handle, edge2Twin.Handle, edge3.Handle, newVertex.Handle });

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
                newFace.FaceData.FaceNormal = face.FaceData.FaceNormal;

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

            for (var i = 0; i < countEdges; i++)
            {
                var prevFace = i - 1;
                var nextFace = i + 1;

                var faceData = edgeStorage[i];

                if (i == 0) prevFace = countEdges - 1;
                if (i == countEdges - 1)
                {
                    nextFace = 0;
                    face.OuterHalfEdge = faceData[1];
                    geometry.ReplaceFace(face);
                }

                var prevFaceData = edgeStorage[prevFace];
                var nextFaceData = edgeStorage[nextFace];

                var edge2Twin = geometry.GetHalfEdgeByHandle(faceData[1]);
                var edge3 = geometry.GetHalfEdgeByHandle(faceData[2]);
                var edge3Twin = geometry.GetHalfEdgeByHandle(prevFaceData[0]);
                var edge2 = geometry.GetHalfEdgeByHandle(edge2Twin.TwinHalfEdge);

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
            }

            return geometry;
        }

        internal static HalfEdge UpdateHalfEdgeOrigin(HalfEdge edge, int newVHandle)
        {
            edge.OriginVertex = newVHandle;
            return edge;
        }
    }
}