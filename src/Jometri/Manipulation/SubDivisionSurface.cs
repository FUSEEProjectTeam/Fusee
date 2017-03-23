using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    public static class SubDivisionSurface
    {

        private static Geometry _geometry;
        private static Geometry _oldGeometry;

        /// <summary>
        /// Performs a SubDivisonSurface with a given Geometry with Peters-Reif SD Algorithm
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static Geometry ReifSubDivision(Geometry geometry)
        {
            _oldGeometry = geometry;
            _geometry = geometry.CloneGeometry();
            var allHalfEdges = _oldGeometry.GetAllEdges();

            foreach (HalfEdge halfEdge in allHalfEdges)
            {
                SplitfEdge(halfEdge.Handle);
            }

            var allFaces = _geometry.GetAllFaces().ToList();
            foreach (Face face in allFaces)
            {
                ReconnectFaces(face.Handle);
            }

            DeleteVertices();

            return _geometry;
        }

        internal static void SplitfEdge(int halfEdgeHandle)
        {
            HalfEdge edgetoSplit = _geometry.GetHalfEdgeByHandle(halfEdgeHandle);
            HalfEdge twinHalfEdge = _geometry.GetHalfEdgeByHandle(edgetoSplit.TwinHalfEdge);

            Vertex vertexP = _geometry.GetVertexByHandle(edgetoSplit.OriginVertex);
            Vertex vertexQ = _geometry.GetVertexByHandle(twinHalfEdge.OriginVertex);

            float3 newVertexPos = GetMiddleOfVertices(vertexP, vertexQ);

            _geometry.InsertVertex(vertexP.Handle, vertexQ.Handle, newVertexPos);
        }

        internal static void DeleteVertices()
        {
            var allVerticesToDelete = _oldGeometry.GetAllVertices();

            foreach (Vertex vertex in allVerticesToDelete)
            {
                Vertex vert = _geometry.GetVertexByHandle(vertex.Handle);

                var allFaces = _geometry.GetVertexAdajacentFaces(vert.Handle).ToList();

                var allHalfEdges = _geometry.GetVertexIncidentHalfEdges(vert.Handle).ToList();
                
                Debug.WriteLine(allFaces.Count());

                Face newFace = new Face(_geometry.CreateFaceHandleId());

                HalfEdge h1 = new HalfEdge();
                HalfEdge n3 = new HalfEdge();

                foreach (Face face in allFaces)
                {
                    var starEdge = _geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
                    while (starEdge.OriginVertex != vert.Handle)
                    {
                        starEdge = _geometry.GetHalfEdgeByHandle(starEdge.NextHalfEdge);
                    }

                    HalfEdge n1 = _geometry.GetHalfEdgeByHandle(starEdge.PrevHalfEdge);
                    h1 =_geometry.GetHalfEdgeByHandle(n1.PrevHalfEdge);
                    HalfEdge n2 = _geometry.GetHalfEdgeByHandle(n1.TwinHalfEdge);
                    n3 = _geometry.GetHalfEdgeByHandle(n2.NextHalfEdge);
                    h1.NextHalfEdge = n3.Handle;
                    h1.IncidentFace = newFace.Handle;
                    n3.PrevHalfEdge = h1.Handle;
                    n3.IncidentFace = newFace.Handle;

                    Vertex tempVert = _geometry.GetVertexByHandle(n1.OriginVertex);
                    tempVert.IncidentHalfEdge = n3.Handle;
                    _geometry.ReplaceVertex(tempVert);

                    _geometry.ReplaceHalfEdge(n3);
                    _geometry.ReplaceHalfEdge(h1);

                    //todo change vertex starting edge if it was deleted
                }

                newFace.OuterHalfEdge = h1.Handle;

                //calc Face normal
                newFace.FaceData.FaceNormal = GetFaceNormal(h1, n3);

                //add new Face
                _geometry.DictFaces.Add(newFace.Handle,newFace);

                //delete HalfEdges Faces and Vertex
                _geometry.DictVertices.Remove(vert.Handle);

                foreach (Face face in allFaces)
                {
                    _geometry.DictFaces.Remove(face.Handle);
                }

                foreach (HalfEdge halfEdge in allHalfEdges)
                {
                    _geometry.DictHalfEdges.Remove(halfEdge.Handle);
                }

            }
        }

        internal static float3 GetFaceNormal(HalfEdge h1, HalfEdge h2)
        {

            HalfEdge h3 = _geometry.GetHalfEdgeByHandle(h2.TwinHalfEdge);
            float3 h1Pos = _geometry.GetVertexByHandle(h1.OriginVertex).VertData.Pos;
            float3 n2Pos = _geometry.GetVertexByHandle(h3.OriginVertex).VertData.Pos;
            float3 n3Pos = _geometry.GetVertexByHandle(h2.OriginVertex).VertData.Pos;

            float3 v1 = new float3((h1Pos.x -n2Pos.x),(h1Pos.y - n2Pos.y),(h1Pos.z - n2Pos.z));
            float3 v2 = new float3((h1Pos.x -n3Pos.x),(h1Pos.y - n3Pos.y),(h1Pos.z - n3Pos.z));

            return float3.Cross(v1, v2);
        }

        internal static void ReconnectFaces(int faceHandle)
        {
            //Find an old Vertex for each Face
            var faceVertices = _geometry.GetFaceVertices(faceHandle);
            var oldFaceVertices = _oldGeometry.GetFaceVertices(faceHandle);

            Vertex startVertex = new Vertex();

            foreach (Vertex faceVertex in faceVertices)
            {
                if (faceVertex == oldFaceVertices.ElementAt(1))
                {
                    startVertex = faceVertex;
                }
            }

            if (startVertex.Handle == 0)
                throw new ArgumentException("Error");

            //Change old Face to only new Vertices
            HalfEdge startHalfEdge = _geometry.GetHalfEdgeByHandle(_geometry.GetFaceByHandle(faceHandle).OuterHalfEdge);

            while (startHalfEdge.OriginVertex != startVertex.Handle)
            {
                startHalfEdge = _geometry.GetHalfEdgeByHandle(startHalfEdge.NextHalfEdge);
            }

            HalfEdge nextHalfEdge = startHalfEdge;

            int loop = faceVertices.Count() / 2;
            for (int i = 0; i < loop; i++)

            {
                int p = _geometry.GetHalfEdgeByHandle(nextHalfEdge.PrevHalfEdge).OriginVertex;
                int q = _geometry.GetHalfEdgeByHandle(nextHalfEdge.NextHalfEdge).OriginVertex;
                _geometry.InsertDiagonal(p, q);

                nextHalfEdge = _geometry.GetHalfEdgeByHandle(nextHalfEdge.NextHalfEdge);
                nextHalfEdge = _geometry.GetHalfEdgeByHandle(nextHalfEdge.NextHalfEdge);
            }
        }

        internal static float3 GetMiddleOfVertices(Vertex a, Vertex b)
        {

            float newX = (a.VertData.Pos.x + b.VertData.Pos.x) / 2.0f;
            float newY = (a.VertData.Pos.y + b.VertData.Pos.y) / 2.0f;
            float newZ = (a.VertData.Pos.z + b.VertData.Pos.z) / 2.0f;

            return new float3(newX, newY, newZ);
        }
    }
}
