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
    ///  Provides functions to perform a Subdivision Surface on a DCEL geometry.
    /// </summary>
    public class SubdivisionSurface
    {
        /// <summary>
        /// Performs a Catmull-Clark Subdivision-Surface on a given geometry which is stored as DCEL.
        /// </summary>
        /// <param name="geometry">The goemetry to perform the SD on, as DCEL.</param>
        /// <returns>A smoother geoemtry with </returns>
        public static Geometry CatmullClarkSubdivision(Geometry geometry)
        {
            //initialising
            Geometry newGeometry = geometry.CloneGeometry();

            //computes all Face Vertices and all Edge Vertices       
            Dictionary<int, Vertex> allFaceVertices = GetFaceVertices(geometry);
            Dictionary<int, Vertex> allEdgeVertices = GetEdgeVertices(geometry, allFaceVertices);

            //Calculates the new position of existing Vertices
            var allVertices = newGeometry.GetAllVertices().ToList();
            foreach (Vertex vertex in allVertices)
            {
                var outgoingEdges = newGeometry.GetVertexStartingHalfEdges(vertex.Handle).ToList();

                //Get average of all face Points and average of all edge Points
                List<Vertex> faceVertices = new List<Vertex>();
                List<Vertex> edgeVertices = new List<Vertex>();
                foreach (HalfEdge edge in outgoingEdges)
                {
                    HalfEdge twin = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                    if (allFaceVertices.ContainsKey(edge.IncidentFace))
                    {
                        faceVertices.Add(allFaceVertices[edge.IncidentFace]);
                    }
                    else
                    {
                        faceVertices.Add(allFaceVertices[twin.IncidentFace]);
                    }

                    if (allEdgeVertices.ContainsKey(edge.Handle))
                    {
                        edgeVertices.Add(allEdgeVertices[edge.Handle]);
                    }
                    else
                    {
                        edgeVertices.Add(allEdgeVertices[twin.Handle]);
                    }
                }

                float3 meanEdgeVertexPos = GeometricOperations.GetVerticesMeanPos(edgeVertices);
                float3 meanFaceVertexPos = GeometricOperations.GetVerticesMeanPos(faceVertices);

                float edgeCount = outgoingEdges.Count;

                float3 newVertexPos = (meanFaceVertexPos + 2 * meanEdgeVertexPos + (edgeCount - 3) * vertex.VertData.Pos) / edgeCount;
                Vertex newVertex = new Vertex(vertex.Handle, newVertexPos) {IncidentHalfEdge = vertex.IncidentHalfEdge};

                newGeometry.ReplaceVertex(newVertex);
            }

            //adds newly calculated Edge Vertices
            var allEdges = geometry.GetAllHalfEdges();
            int[] doneHe = new int[allEdges.Count()+1];
            foreach (HalfEdge edge in allEdges)
            {
                if(doneHe[edge.Handle] == edge.TwinHalfEdge) continue;
                int vertexOld1 = edge.OriginVertex;

                HalfEdge twinEdge = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                int vertexOld2 = twinEdge.OriginVertex;

                //find correct Edge Vertex
                Vertex edgeVertex;
                if (allEdgeVertices.ContainsKey(edge.Handle))
                {
                    edgeVertex = allEdgeVertices[edge.Handle];
                }
                else
                {
                    edgeVertex = allEdgeVertices[twinEdge.Handle];
                }

                newGeometry.InsertVertex(vertexOld1, vertexOld2, edgeVertex.VertData.Pos);
                doneHe[edge.TwinHalfEdge] = edge.Handle;
            }

            newGeometry.SetHighestHandles();
            geometry = newGeometry.CloneGeometry();

            //creates the new quad faces and connects everything
            AddFaceVerticesAndNewFaces(geometry, newGeometry, allFaceVertices);

            return newGeometry;
        }

        private static void AddFaceVerticesAndNewFaces(Geometry _geometry, Geometry _newGeometry, Dictionary<int, Vertex> allFaceVertices)
        {
            var allFaces = _geometry.GetAllFaces();

            foreach (Face face in allFaces)
            {
                Vertex faceVertex = new Vertex(_newGeometry.CreateVertHandleId(), allFaceVertices[face.Handle].VertData.Pos);
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

                    //create new quad face
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

                    if (i == 0) // the old face becomes the first quad, so no new face to create
                    {
                        _newGeometry.ReplaceFace(newFace);
                    }
                    else //create new face 
                    {
                        _newGeometry.DictFaces.Add(newFace.Handle, newFace);
                    }

                    //sotres Vertices to get the face normal
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

        private static Dictionary<int,Vertex> GetEdgeVertices(Geometry geometry, Dictionary<int, Vertex> faceVertices)
        {
            var allEdges = geometry.GetAllHalfEdges();

            int[] doneHE = new int[allEdges.Count()+1];

            Dictionary<int, Vertex> allEdgeVertices = new Dictionary<int, Vertex>();

            foreach (HalfEdge edge in allEdges)
            {
                if(doneHE[edge.Handle] == edge.TwinHalfEdge) continue;
                int face1 = edge.IncidentFace;
                HalfEdge twin = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);
                int face2 = twin.IncidentFace;

                Vertex vertex1 = geometry.GetVertexByHandle(edge.OriginVertex);
                Vertex vertex2 = geometry.GetVertexByHandle(twin.OriginVertex);

                Vertex cVertex1 = faceVertices[face1];
                Vertex cVertex2 = faceVertices[face2];

                List<Vertex> temp = new List<Vertex> {vertex1, vertex2, cVertex1, cVertex2};

                Vertex finalVertex = new Vertex(edge.Handle, GeometricOperations.GetVerticesMeanPos(temp)) {IncidentHalfEdge = edge.Handle};

                allEdgeVertices.Add(edge.Handle, finalVertex);
                doneHE[edge.TwinHalfEdge] = edge.Handle;
            }
            return allEdgeVertices;
        }

        private static Dictionary<int, Vertex> GetFaceVertices(Geometry _geometry)
        {
            var allFaces = _geometry.GetAllFaces();
            Dictionary<int, Vertex> allFaceVertices = new Dictionary<int, Vertex>();

            foreach (Face face in allFaces)
            {
                var faceVertices = _geometry.GetFaceVertices(face.Handle).ToList();
                Vertex tempVertex = new Vertex(face.Handle, GeometricOperations.GetVerticesMeanPos(faceVertices));
                allFaceVertices.Add(face.Handle, tempVertex);
            }
            return allFaceVertices;
        }
    }
}
