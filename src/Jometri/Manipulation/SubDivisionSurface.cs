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
        private static Geometry _newGeometry;

        private static Dictionary<int, Vertex> _allEdgeVertices;
        private static Dictionary<int, Vertex> _allFaceVertices;


        public static Geometry CatmullClarkSubDivision(Geometry geometry)
        {
            //initialising
            _newGeometry = geometry.CloneGeometry();

            _allEdgeVertices = new Dictionary<int, Vertex>();
            _allFaceVertices = new Dictionary<int, Vertex>();
            _geometry = geometry;

            GetFaceVertices();
            GetEdgeVertices();

            ComputeNewVertexPosition();

            AddEdgeVertices();

            _geometry = _newGeometry.CloneGeometry();

            AddFaceVerticesAndNewFaces();

            var allFaces = _newGeometry.GetAllFaces();

            return _newGeometry;
        }

        private static void AddFaceVerticesAndNewFaces()
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {
                Vertex faceVertex = new Vertex(_newGeometry.CreateVertHandleId(), _allFaceVertices[face.Handle].VertData.Pos);
                HalfEdge startEdge = _geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
                HalfEdge nextEdge = startEdge;

                _newGeometry.DictVertices.Add(faceVertex.Handle, faceVertex);
                int i = 0;
                do
                {

                    HalfEdge h1 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h2 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h3 = _newGeometry.GetHalfEdgeByHandle(nextEdge.PrevHalfEdge);
                    HalfEdge h4 = _newGeometry.GetHalfEdgeByHandle(nextEdge.Handle);

                    Face newFace;
                    if (i == 0)
                    {
                        newFace = face;
                    }
                    else
                    {
                        newFace = new Face(_newGeometry.CreateFaceHandleId());
                    }
                    nextEdge = _geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);

                    h1.NextHalfEdge = h2.Handle;
                    h2.NextHalfEdge = h3.Handle;
                    h3.NextHalfEdge = h4.Handle;
                    h4.NextHalfEdge = h1.Handle;

                    h1.PrevHalfEdge = h4.Handle;
                    h2.PrevHalfEdge = h1.Handle;
                    h3.PrevHalfEdge = h2.Handle;
                    h4.PrevHalfEdge = h3.Handle;

                    h1.OriginVertex = nextEdge.OriginVertex;
                    h2.OriginVertex = faceVertex.Handle;

                    h1.IncidentFace = newFace.Handle;
                    h2.IncidentFace = newFace.Handle;
                    h3.IncidentFace = newFace.Handle;
                    h4.IncidentFace = newFace.Handle;

                    faceVertex.IncidentHalfEdge = h2.Handle;
                    newFace.OuterHalfEdge = h1.Handle;

                    //add and replace changed 
                    _newGeometry.DictHalfEdges.Add(h1.Handle, h1);
                    _newGeometry.DictHalfEdges.Add(h2.Handle, h2);

                    _newGeometry.ReplaceHalfEdge(h3);
                    _newGeometry.ReplaceHalfEdge(h4);

                    _newGeometry.ReplaceVertex(faceVertex);

                    if (i == 0)
                    {
                        _newGeometry.ReplaceFace(newFace);
                    }
                    else
                    {
                        _newGeometry.DictFaces.Add(newFace.Handle, newFace);
                    }

                    //face normal
                    List<Vertex> faceVertices = new List<Vertex>();
                    faceVertices.Add(_newGeometry.GetVertexByHandle(h1.OriginVertex));
                    faceVertices.Add(_newGeometry.GetVertexByHandle(h2.OriginVertex));
                    faceVertices.Add(_newGeometry.GetVertexByHandle(h3.OriginVertex));
                    faceVertices.Add(_newGeometry.GetVertexByHandle(h4.OriginVertex));

                    _newGeometry.SetFaceNormal(faceVertices, newFace);

                    i++;
                    nextEdge = _geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);
                } while (startEdge != nextEdge);

                //set TwinEdges

                nextEdge = startEdge;
                do
                {
                    HalfEdge h4 = _newGeometry.GetHalfEdgeByHandle(nextEdge.Handle);
                    HalfEdge h1 = _newGeometry.GetHalfEdgeByHandle(h4.NextHalfEdge);

                    nextEdge = _geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);
                    nextEdge = _geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);

                    HalfEdge h4n = _newGeometry.GetHalfEdgeByHandle(nextEdge.Handle);
                    HalfEdge h2n = _newGeometry.GetHalfEdgeByHandle(h4n.NextHalfEdge);
                    h2n = _newGeometry.GetHalfEdgeByHandle(h2n.NextHalfEdge);

                    h1.TwinHalfEdge = h2n.Handle;
                    h2n.TwinHalfEdge = h1.Handle;

                    _newGeometry.ReplaceHalfEdge(h1);
                    _newGeometry.ReplaceHalfEdge(h2n);



                } while (startEdge != nextEdge);

            }
        }

        private static void AddEdgeVertices()
        {
            var allEdges = _geometry.GetAllEdges();

            foreach (HalfEdge edge in allEdges)
            {
                int vertexOld1 = edge.OriginVertex;

                HalfEdge twinEdge = _geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                int vertexOld2 = twinEdge.OriginVertex;

                //find correct Edge Vertex
                Vertex edgeVertex;
                if (_allEdgeVertices.ContainsKey(edge.Handle))
                {
                    edgeVertex = _allEdgeVertices[edge.Handle];
                }
                else
                {
                    edgeVertex = _allEdgeVertices[twinEdge.Handle];
                }

                _newGeometry.InsertVertex(vertexOld1, vertexOld2, edgeVertex.VertData.Pos);
            }

            _newGeometry.SetHighestHandles();
        }


        private static void ComputeNewVertexPosition()
        {
            var allVertices = _newGeometry.GetAllVertices().ToList();

            foreach (Vertex vertex in allVertices)
            {
                var outgoingEdges = _newGeometry.GetVertexStartingHalfEdges(vertex.Handle).ToList();

                //Get average of all face Points and average of all edge Points
                List<Vertex> faceVertices = new List<Vertex>();
                List<Vertex> edgeVertices = new List<Vertex>();
                foreach (HalfEdge edge in outgoingEdges)
                {
                    HalfEdge twin = _geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                    if (_allFaceVertices.ContainsKey(edge.IncidentFace))
                    {
                        faceVertices.Add(_allFaceVertices[edge.IncidentFace]);
                    }
                    else
                    {
                        faceVertices.Add(_allFaceVertices[twin.IncidentFace]);
                    }

                    if (_allEdgeVertices.ContainsKey(edge.Handle))
                    {
                        edgeVertices.Add(_allEdgeVertices[edge.Handle]);
                    }
                    else
                    {
                        edgeVertices.Add(_allEdgeVertices[twin.Handle]);
                    }
                }

                float3 meanEdgeVertexPos = GetVerticesMeanPos(edgeVertices);
                float3 meanFaceVertexPos = GetVerticesMeanPos(faceVertices);

                float edgeCount = outgoingEdges.Count;

                float3 newVertexPos = (meanFaceVertexPos + 2 * meanEdgeVertexPos + (edgeCount - 3) * vertex.VertData.Pos) / edgeCount;
                Vertex newVertex = new Vertex(vertex.Handle, newVertexPos);
                newVertex.IncidentHalfEdge = vertex.IncidentHalfEdge;

                _newGeometry.ReplaceVertex(newVertex);

            }
        }

        private static void GetEdgeVertices()
        {
            var allEdges = _geometry.GetAllEdges();

            foreach (HalfEdge edge in allEdges)
            {
                int face1 = edge.IncidentFace;
                HalfEdge twin = _geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);
                int face2 = twin.IncidentFace;

                Vertex vertex1 = _geometry.GetVertexByHandle(edge.OriginVertex);
                Vertex vertex2 = _geometry.GetVertexByHandle(twin.OriginVertex);

                Vertex cVertex1 = _allFaceVertices[face1];
                Vertex cVertex2 = _allFaceVertices[face2];

                List<Vertex> temp = new List<Vertex>();
                temp.Add(vertex1);
                temp.Add(vertex2);
                temp.Add(cVertex1);
                temp.Add(cVertex2);

                Vertex temp2 = new Vertex(edge.Handle, GetVerticesMeanPos(temp));
                temp2.IncidentHalfEdge = edge.Handle;

                _allEdgeVertices.Add(edge.Handle, temp2);
            }
        }

        private static void GetFaceVertices()
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {
                var faceVertices = _geometry.GetFaceVertices(face.Handle).ToList();
                Vertex tempVertex = new Vertex(face.Handle, GetVerticesMeanPos(faceVertices));
                _allFaceVertices.Add(face.Handle, tempVertex);
            }
        }

        private static float3 GetVerticesMeanPos(List<Vertex> vertices)
        {
            float3 centroid = new float3();

            foreach (Vertex vertex in vertices)
            {
                centroid += vertex.VertData.Pos;
            }
            centroid = centroid / vertices.Count();
            return centroid;
        }
    }
}
