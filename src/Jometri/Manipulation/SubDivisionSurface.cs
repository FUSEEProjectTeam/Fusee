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
            _newGeometry = new Geometry();
            _allEdgeVertices = new Dictionary<int, Vertex>();
            _allFaceVertices = new Dictionary<int, Vertex>();
            _geometry = geometry.CloneGeometry();

            GetFaceVertices();
            GetEdgeVertices();

            ComputeNewVertexPosition();
            CreateNewFaces();

            return _geometry;
        }

        private static void CreateNewFaces()
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {

                Vertex faceVertex = new Vertex(_newGeometry.CreateVertHandleId(), _allFaceVertices[face.Handle].VertData.Pos);;
                //add Face Vertex to new Geometry
                _newGeometry.DictVertices.Add(faceVertex.Handle, faceVertex);

                List<int> edgeVerticesHandles = new List<int>();

                HalfEdge start = _geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
                HalfEdge next = start;

                //Find all Edge Vertices of each Face
                do
                {
                    HalfEdge twin = _geometry.GetHalfEdgeByHandle(next.TwinHalfEdge);
                    Vertex currentEdgeVertex;
                    if (_allEdgeVertices.ContainsKey(next.Handle))
                    {
                        currentEdgeVertex = _allEdgeVertices[next.Handle];
                        edgeVerticesHandles.Add(currentEdgeVertex.Handle);
                    }
                    else
                    {
                        currentEdgeVertex = _allEdgeVertices[twin.Handle];
                        edgeVerticesHandles.Add(currentEdgeVertex.Handle);
                    }

                    ConnectVertices(faceVertex, currentEdgeVertex);

                    next = _geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
                } while (start != next);

                //Connect all edge Vertices with the face Vertex
                foreach (int vertexHandle in edgeVerticesHandles)
                {

                    Vertex edgeVertex1 = _allEdgeVertices[vertexHandle];
                    HalfEdge h1 = _newGeometry.GetHalfEdgeByHandle(edgeVertex1.IncidentHalfEdge);                    

                    HalfEdge temp = _geometry.GetHalfEdgeByHandle(vertexHandle);
                    Vertex oldVertex = _geometry.GetVertexByHandle(temp.OriginVertex);
                    temp = _geometry.GetHalfEdgeByHandle(temp.PrevHalfEdge);

                    Vertex edgeVertex2 = _allEdgeVertices[temp.Handle];
                    HalfEdge h2 = _newGeometry.GetHalfEdgeByHandle(edgeVertex2.IncidentHalfEdge);
                    h2 = _newGeometry.GetHalfEdgeByHandle(h2.TwinHalfEdge);


                    HalfEdge edge1 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge edge2 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());

                    //Vertex oldVertex;
                    //Vertex edgeVertex2;
                    //HalfEdge edge = _geometry.GetHalfEdgeByHandle(_allEdgeVertices[vertexHandle].IncidentHalfEdge);
                    //HalfEdge prevEdge = edge;
                    //if (edge.IncidentFace == face.Handle)
                    //{
                    //    oldVertex = _geometry.DictVertices[edge.OriginVertex];
                    //    prevEdge = _geometry.GetHalfEdgeByHandle(edge.PrevHalfEdge);

                    //    if (_allEdgeVertices.ContainsKey(prevEdge.Handle))
                    //    {
                    //        edgeVertex2 = _allEdgeVertices[prevEdge.Handle];
                    //    }
                    //    else
                    //    {
                    //        edgeVertex2 = _allEdgeVertices[prevEdge.TwinHalfEdge];
                    //    }
                    //}
                    //else
                    //{
                    //    HalfEdge twin = _geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);
                    //    oldVertex = _geometry.DictVertices[twin.OriginVertex];
                    //    if (_allEdgeVertices.ContainsKey(prevEdge.Handle))
                    //    {
                    //        edgeVertex2 = _allEdgeVertices[prevEdge.Handle];
                    //    }
                    //    else
                    //    {
                    //        edgeVertex2 = _allEdgeVertices[prevEdge.TwinHalfEdge];
                    //    }
                    //}

                    ////close each Face
                    //HalfEdge edge1 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    //HalfEdge edge2 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());

                    //HalfEdge temp = _newGeometry.GetHalfEdgeByHandle(edgeVertex2.IncidentHalfEdge);

                    //HalfEdge h1 = _newGeometry.GetHalfEdgeByHandle(_allEdgeVertices[vertexHandle].IncidentHalfEdge);
                    //HalfEdge h2 = _newGeometry.GetHalfEdgeByHandle(temp.TwinHalfEdge);

                    edge1.IncidentFace = h1.IncidentFace;
                    edge2.IncidentFace = h1.IncidentFace;
                    h2.IncidentFace = h1.IncidentFace;

                    h1.NextHalfEdge = h2.Handle;
                    h2.NextHalfEdge = edge2.Handle;
                    edge2.NextHalfEdge = edge1.Handle;
                    edge1.NextHalfEdge = h1.Handle;

                    h1.PrevHalfEdge = edge1.Handle;
                    edge1.PrevHalfEdge = edge2.Handle;
                    edge2.PrevHalfEdge = h2.Handle;
                    h2.PrevHalfEdge = h1.Handle;

                    edge1.OriginVertex = oldVertex.Handle;
                    oldVertex.IncidentHalfEdge = edge1.Handle;
                    edge2.OriginVertex = edgeVertex2.Handle;

                    //add / replace
                    _newGeometry.DictHalfEdges.Add(edge1.Handle,edge1);
                    _newGeometry.DictHalfEdges.Add(edge2.Handle,edge2);

                    _newGeometry.ReplaceHalfEdge(h1);
                    _newGeometry.ReplaceHalfEdge(h2);

                    _newGeometry.ReplaceVertex(oldVertex);
                }

                //todo set twins of adajcent faces
                _newGeometry.SetHighestHandles();
            }
            
        }

        private static void ConnectVertices(Vertex faceVertex, Vertex edgeVertex)
        {
            HalfEdge edge1 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
            HalfEdge edge2 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());

            Face face = new Face(_newGeometry.CreateFaceHandleId());

            Vertex edgeV = new Vertex(_newGeometry.CreateVertHandleId(),edgeVertex.VertData.Pos);

            edge1.OriginVertex = faceVertex.Handle;
            edge2.OriginVertex = edgeV.Handle;

            edge1.IncidentFace = face.Handle;

            edgeV.IncidentHalfEdge = edge2.Handle;

            face.OuterHalfEdge = edge1.Handle;

            edge1.TwinHalfEdge = edge2.Handle;
            edge2.TwinHalfEdge = edge1.Handle;

            //Vertex temp = _allEdgeVertices[edgeVertex.IncidentHalfEdge];
            //temp.IncidentHalfEdge = edge2.Handle;

            //_allEdgeVertices[edgeVertex.IncidentHalfEdge] = temp;

            //add all new Data to new Geometry
            _newGeometry.DictHalfEdges.Add(edge1.Handle,edge1);
            _newGeometry.DictHalfEdges.Add(edge2.Handle,edge2);

            _newGeometry.DictFaces.Add(face.Handle,face);
            faceVertex.IncidentHalfEdge = edge1.Handle;
            _newGeometry.ReplaceVertex(faceVertex);

            _newGeometry.DictVertices.Add(edgeV.Handle,edgeV);
        }

        private static void ComputeNewVertexPosition()
        {
            var allVertices = _geometry.GetAllVertices().ToList();

            foreach (Vertex vertex in allVertices)
            {
                var outgoingEdges = _geometry.GetVertexStartingHalfEdges(vertex.Handle).ToList();

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

                float3 newVertexPos = ((1f / edgeCount) * meanFaceVertexPos) + ((2f / edgeCount) * meanEdgeVertexPos) + (((edgeCount - 3f) / edgeCount) * vertex.VertData.Pos);
                Vertex newVertex = new Vertex(vertex.Handle,newVertexPos);
                newVertex.IncidentHalfEdge = vertex.IncidentHalfEdge;

                _geometry.ReplaceVertex(newVertex);

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

                _allEdgeVertices.Add(edge.Handle,temp2);
            }
        }

        private static void GetFaceVertices()
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {
                var faceVertices = _geometry.GetFaceVertices(face.Handle).ToList();
                Vertex tempVertex = new Vertex(face.Handle, GetVerticesMeanPos(faceVertices));
                _allFaceVertices.Add(face.Handle,tempVertex);
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
