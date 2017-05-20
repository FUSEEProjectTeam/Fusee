using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    /// <summary>
    ///  Provides functions to perform a Sub Division Surface on a DCEL geometry.
    /// </summary>
    public static class SubDivisionSurface
    {

        private static Geometry _geometry;
        private static Geometry _newGeometry;

        private static Dictionary<int, Vertex> _allEdgeVertices;
        private static Dictionary<int, Vertex> _allFaceVertices;

        /// <summary>
        /// Performs a Catmull-Clark Subdivision-Surface with a given geometry which is stored as DCEL.
        /// </summary>
        /// <param name="geometry">The goemetry to perform the SD on, as DCEL.</param>
        /// <returns></returns>
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

                //stores Halfedges without Twin
                Dictionary<int, HalfEdge> halfEdges2 = new Dictionary<int, HalfEdge>();
                Dictionary<int, HalfEdge> halfEdges1 = new Dictionary<int, HalfEdge>();

                _newGeometry.DictVertices.Add(faceVertex.Handle, faceVertex);
                int i = 0;
                do
                {
                    HalfEdge h1 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h2 = new HalfEdge(_newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h3 = _newGeometry.GetHalfEdgeByHandle(nextEdge.PrevHalfEdge);
                    HalfEdge h4 = _newGeometry.GetHalfEdgeByHandle(nextEdge.Handle);

                    Face newFace;
                    newFace = i == 0 ? face : new Face(_newGeometry.CreateFaceHandleId());
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
                    List<Vertex> faceVertices = new List<Vertex>
                    {
                        _newGeometry.GetVertexByHandle(h1.OriginVertex),
                        _newGeometry.GetVertexByHandle(h2.OriginVertex),
                        _newGeometry.GetVertexByHandle(h3.OriginVertex),
                        _newGeometry.GetVertexByHandle(h4.OriginVertex)
                    };

                    _newGeometry.SetFaceNormal(faceVertices, newFace);

                    halfEdges2.Add(i,h2);
                    halfEdges1.Add(i,h1);

                    //for the second Edge per Face connect the twin
                    if (i > 0)
                    {
                        HalfEdge h1N = halfEdges1[i - 1];
                        h1N.TwinHalfEdge= halfEdges2[i].Handle;

                        HalfEdge h2N = halfEdges2[i];
                        h2N.TwinHalfEdge = h1N.Handle;

                        _newGeometry.ReplaceHalfEdge(h1N);
                        _newGeometry.ReplaceHalfEdge(h2N);
                    }   
                    
                    i++;
                    nextEdge = _geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);
                } while (startEdge != nextEdge);

                //set Twin of firts and lasts of each new Face
                HalfEdge h2Firts = halfEdges2[0];            
                HalfEdge h1Last = halfEdges1[i-1];

                h2Firts.TwinHalfEdge = h1Last.Handle;
                h1Last.TwinHalfEdge = h2Firts.Handle;

                _newGeometry.ReplaceHalfEdge(h2Firts);
                _newGeometry.ReplaceHalfEdge(h1Last);
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

                float3 meanEdgeVertexPos = GeometricOperations.GetVerticesMeanPos(edgeVertices);
                float3 meanFaceVertexPos = GeometricOperations.GetVerticesMeanPos(faceVertices);

                float edgeCount = outgoingEdges.Count;

                float3 newVertexPos = (meanFaceVertexPos + 2 * meanEdgeVertexPos + (edgeCount - 3) * vertex.VertData.Pos) / edgeCount;
                Vertex newVertex = new Vertex(vertex.Handle, newVertexPos) {IncidentHalfEdge = vertex.IncidentHalfEdge};

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

                List<Vertex> temp = new List<Vertex> {vertex1, vertex2, cVertex1, cVertex2};

                Vertex finalVertex = new Vertex(edge.Handle, GeometricOperations.GetVerticesMeanPos(temp)) {IncidentHalfEdge = edge.Handle};

                _allEdgeVertices.Add(edge.Handle, finalVertex);
            }
        }

        private static void GetFaceVertices()
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {
                var faceVertices = _geometry.GetFaceVertices(face.Handle).ToList();
                Vertex tempVertex = new Vertex(face.Handle, GeometricOperations.GetVerticesMeanPos(faceVertices));
                _allFaceVertices.Add(face.Handle, tempVertex);
            }
        }
    }
}
