using System.Collections.Generic;
using System.Linq;

namespace Fusee.Jometri
{
    /// <summary>
    ///  Provides functionality to perform a Catmull-Clark Subdivision-Surface algorithm on a Geometry.
    /// </summary>
    public class SubdivisionSurface
    {
        /// <summary>
        /// Performs a Catmull-Clark Subdivision-Surface algorithm on a given geometry which is stored as DCEL.
        /// </summary>
        /// <param name="geometry">The geometry to perform the SD on.</param>
        /// <returns>A smoother geometry with </returns>
        public static Geometry CatmullClarkSubdivision(Geometry geometry)
        {
            //initializing
            var newGeometry = geometry.CloneGeometry();

            //computes all Face Vertices and all Edge Vertices       
            var allFaceVertices = GetFaceVertices(geometry);
            var allEdgeVertices = GetEdgeVertices(geometry, allFaceVertices);

            //Calculates the new position of existing Vertices
            var allVertices = newGeometry.GetAllVertices().ToList();
            foreach (var vertex in allVertices)
            {
                var outgoingEdges = newGeometry.GetVertexStartingHalfEdges(vertex.Handle).ToList();

                //Get average of all face Points and average of all edge Points
                var faceVertices = new List<Vertex>();
                var edgeVertices = new List<Vertex>();
                foreach (var edge in outgoingEdges)
                {
                    var twin = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                    if (allFaceVertices.ContainsKey(edge.IncidentFace))
                        faceVertices.Add(allFaceVertices[edge.IncidentFace]);
                    else
                        faceVertices.Add(allFaceVertices[twin.IncidentFace]);

                    if (allEdgeVertices.ContainsKey(edge.Handle))
                        edgeVertices.Add(allEdgeVertices[edge.Handle]);
                    else
                        edgeVertices.Add(allEdgeVertices[twin.Handle]);

                }

                var meanEdgeVertexPos = GeometricOperations.GetVerticesMeanPos(edgeVertices);
                var meanFaceVertexPos = GeometricOperations.GetVerticesMeanPos(faceVertices);

                float edgeCount = outgoingEdges.Count;

                var newVertexPos = (meanFaceVertexPos + 2 * meanEdgeVertexPos + (edgeCount - 3) * vertex.VertData.Pos) / edgeCount;
                var newVertex = new Vertex(vertex.Handle, newVertexPos) { IncidentHalfEdge = vertex.IncidentHalfEdge };

                newGeometry.ReplaceVertex(newVertex);
            }

            //adds newly calculated Edge Vertices
            var allEdges = geometry.GetAllHalfEdges();
            var doneHe = new int[allEdges.Count() + 1];
            foreach (var edge in allEdges)
            {
                if (doneHe[edge.Handle] == edge.TwinHalfEdge) continue;
                var vertexOld1 = edge.OriginVertex;

                var twinEdge = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);

                var vertexOld2 = twinEdge.OriginVertex;

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

        private static void AddFaceVerticesAndNewFaces(Geometry geometry, Geometry newGeometry, Dictionary<int, Vertex> allFaceVertices)
        {
            var allFaces = geometry.GetAllFaces();

            foreach (var face in allFaces)
            {
                Vertex faceVertex = new Vertex(newGeometry.CreateVertHandleId(), allFaceVertices[face.Handle].VertData.Pos);
                HalfEdge startEdge = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
                HalfEdge nextEdge = startEdge;

                //stores Halfedges without Twin
                Dictionary<int, HalfEdge> halfEdges2 = new Dictionary<int, HalfEdge>();
                Dictionary<int, HalfEdge> halfEdges1 = new Dictionary<int, HalfEdge>();

                newGeometry.DictVertices.Add(faceVertex.Handle, faceVertex);
                int i = 0;
                do
                {
                    HalfEdge h1 = new HalfEdge(newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h2 = new HalfEdge(newGeometry.CreateHalfEdgeHandleId());
                    HalfEdge h3 = newGeometry.GetHalfEdgeByHandle(nextEdge.PrevHalfEdge);
                    HalfEdge h4 = newGeometry.GetHalfEdgeByHandle(nextEdge.Handle);

                    //create new quad face
                    Face newFace;
                    newFace = i == 0 ? face : new Face(newGeometry.CreateFaceHandleId());
                    nextEdge = geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);

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
                    newGeometry.DictHalfEdges.Add(h1.Handle, h1);
                    newGeometry.DictHalfEdges.Add(h2.Handle, h2);

                    newGeometry.ReplaceHalfEdge(h3);
                    newGeometry.ReplaceHalfEdge(h4);

                    newGeometry.ReplaceVertex(faceVertex);

                    if (i == 0) // the old face becomes the first quad, so no new face to create
                    {
                        newGeometry.ReplaceFace(newFace);
                    }
                    else //create new face 
                    {
                        newGeometry.DictFaces.Add(newFace.Handle, newFace);
                    }

                    //Stores Vertices to get the face normal
                    List<Vertex> faceVertices = new List<Vertex>
                    {
                        newGeometry.GetVertexByHandle(h1.OriginVertex),
                        newGeometry.GetVertexByHandle(h2.OriginVertex),
                        newGeometry.GetVertexByHandle(h3.OriginVertex),
                        newGeometry.GetVertexByHandle(h4.OriginVertex)
                    };

                    newGeometry.SetFaceNormal(faceVertices, newFace);

                    halfEdges2.Add(i, h2);
                    halfEdges1.Add(i, h1);

                    //for the second Edge per Face connect the twin
                    if (i > 0)
                    {
                        HalfEdge h1N = halfEdges1[i - 1];
                        h1N.TwinHalfEdge = halfEdges2[i].Handle;

                        HalfEdge h2N = halfEdges2[i];
                        h2N.TwinHalfEdge = h1N.Handle;

                        newGeometry.ReplaceHalfEdge(h1N);
                        newGeometry.ReplaceHalfEdge(h2N);
                    }

                    i++;
                    nextEdge = geometry.GetHalfEdgeByHandle(nextEdge.NextHalfEdge);
                } while (startEdge != nextEdge);

                //set Twin of firsts and lasts of each new Face
                var h2Firts = halfEdges2[0];
                var h1Last = halfEdges1[i - 1];

                h2Firts.TwinHalfEdge = h1Last.Handle;
                h1Last.TwinHalfEdge = h2Firts.Handle;

                newGeometry.ReplaceHalfEdge(h2Firts);
                newGeometry.ReplaceHalfEdge(h1Last);
            }
        }

        private static Dictionary<int, Vertex> GetEdgeVertices(Geometry geometry, Dictionary<int, Vertex> faceVertices)
        {
            var allEdges = geometry.GetAllHalfEdges();

            var doneHe = new int[allEdges.Count() + 1];

            var allEdgeVertices = new Dictionary<int, Vertex>();

            foreach (var edge in allEdges)
            {
                if (doneHe[edge.Handle] == edge.TwinHalfEdge) continue;
                var face1 = edge.IncidentFace;
                var twin = geometry.GetHalfEdgeByHandle(edge.TwinHalfEdge);
                var face2 = twin.IncidentFace;

                var vertex1 = geometry.GetVertexByHandle(edge.OriginVertex);
                var vertex2 = geometry.GetVertexByHandle(twin.OriginVertex);

                var cVertex1 = faceVertices[face1];
                var cVertex2 = faceVertices[face2];

                var temp = new List<Vertex> { vertex1, vertex2, cVertex1, cVertex2 };

                var finalVertex = new Vertex(edge.Handle, GeometricOperations.GetVerticesMeanPos(temp)) { IncidentHalfEdge = edge.Handle };

                allEdgeVertices.Add(edge.Handle, finalVertex);
                doneHe[edge.TwinHalfEdge] = edge.Handle;
            }
            return allEdgeVertices;
        }

        private static Dictionary<int, Vertex> GetFaceVertices(Geometry geometry)
        {
            var allFaces = geometry.GetAllFaces();
            var allFaceVertices = new Dictionary<int, Vertex>();

            foreach (var face in allFaces)
            {
                var faceVertices = geometry.GetFaceVertices(face.Handle).ToList();
                var tempVertex = new Vertex(face.Handle, GeometricOperations.GetVerticesMeanPos(faceVertices));
                allFaceVertices.Add(face.Handle, tempVertex);
            }
            return allFaceVertices;
        }
    }
}
