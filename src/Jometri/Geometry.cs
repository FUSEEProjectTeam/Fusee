using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Stores geometry in a DCEL (doubly conneted edge list).
    /// </summary>
    public class Geometry
    {
        #region Members

        /// <summary>
        /// Contains handles to half edges. Use this to find adjecant half edges or the origin vertex.
        /// </summary>
        public IList<HalfEdgeHandle> HalfEdgeHandles;

        /// <summary>
        /// Contains handles to faces. Use this to find the first half edge.
        /// </summary>
        public IList<FaceHandle> FaceHandles;

        /// <summary>
        /// Contains handles to vertices. Use this to get the vertexes coordinates.
        /// </summary>
        public IList<VertHandle> VertHandles;

        private readonly List<Vertex> _vertices;
        private readonly List<HalfEdge> _halfEdges;
        private readonly List<Face> _faces;

        #endregion

        /// <summary>
        /// Stores geometry in a DCEL (doubly conneted edge list).
        /// </summary>
        /// <param name="faces">A collection of the geometrys' faces, each containing the geometric information as a list of float3 in ccw order</param>
        public Geometry(IEnumerable<IList<float3>> faces)
        {
            _vertices = new List<Vertex>();
            _halfEdges = new List<HalfEdge>();
            _faces = new List<Face>();

            HalfEdgeHandles = new List<HalfEdgeHandle>();
            FaceHandles = new List<FaceHandle>();
            VertHandles = new List<VertHandle>();

            CreateHalfEdgesForGeometry(faces);
        }

        #region Structs

        /// <summary>
        /// Each face contains:
        /// A handle to assign a abstract reference to it.
        /// A handle to the first half edge that belongs to this face.
        /// </summary>
        public struct Face
        {
            public FaceHandle Handle;
            public HalfEdgeHandle FirstHalfEdge;
        }

        /// <summary>
        /// Each vertex contains:
        /// A handle to assign a abstract reference to it.
        /// The vertex' coordinates.
        /// </summary>
        public struct Vertex
        {
            public VertHandle Handle;

            public float3 Coord;
            public HalfEdgeHandle IncidentHalfEdge;

            public Vertex(float3 coord)
            {
                Handle = new VertHandle();
                IncidentHalfEdge = new HalfEdgeHandle();
                Coord = coord;
            }
        }

        /// <summary>
        /// Represents a half edge.
        /// Each half edge contains:
        /// A handle to assign a abstract reference to it.
        /// A handle to the half edges' origin vertex.
        /// A handle to the next half edge (in ccw order).
        /// A handle to the face it belongs to.
        /// </summary>
        public struct HalfEdge
        {
            public HalfEdgeHandle Handle;

            public VertHandle Origin;
            public HalfEdgeHandle Twin;
            public HalfEdgeHandle Next;
            public FaceHandle IncidentFace;
        }

        #endregion

        /*Insert methods like:
            >InsertVertex
            >InsertFace
            >Get all edges adjecant to a vertex
            >Get all edges that belong to a face
            >etc.
        */

        #region public Methods

        /// <summary>
        /// Inserts a pair of half edges between two vertices.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <exception cref="Exception"></exception>
        public void InsertHalfEdge(Vertex p, Vertex q)
        {
            var heStartingAtP = HalfEdgesStartingAtV(p);
            var heStaringAtQ = HalfEdgesStartingAtV(q);

            var face = new Face();
            var pStartHe = new HalfEdge();
            var qStartHe = new HalfEdge();

            var halfEdgeHandlesQ = heStaringAtQ as IList<HalfEdgeHandle> ?? heStaringAtQ.ToList();

            foreach (var heP in heStartingAtP)
            {
                var faceHeP = GetHalfEdgeByHandle(heP.Id).IncidentFace;

                foreach (var heQ in halfEdgeHandlesQ)
                {
                    var faceHeQ = GetHalfEdgeByHandle(heQ.Id).IncidentFace;

                    if (faceHeP.Id == faceHeQ.Id)
                    {
                        face = GetFaceByHandle(faceHeP.Id);
                        pStartHe = GetHalfEdgeByHandle(faceHeP.Id);
                        qStartHe = GetHalfEdgeByHandle(faceHeQ.Id);
                    }
                    else
                    {
                        throw new Exception("Vertex " + p + " vertex " + q + " have no common Face!");
                    }
                }
            }

            var newFromP = new HalfEdge();
            var newFromQ = new HalfEdge();
            var newFace = new Face { FirstHalfEdge = newFromP.Handle };

            newFromP.Origin = p.Handle;
            newFromP.Next = pStartHe.Handle;
            newFromP.Twin = newFromQ.Handle;
            newFromP.IncidentFace = newFace.Handle;

            newFromQ.Origin = q.Handle;
            newFromQ.Next = qStartHe.Handle;
            newFromQ.Twin = newFromP.Handle;
            newFromQ.IncidentFace = face.Handle;

            //Assign the handle of the new face to its half edges
            var currentHe = qStartHe;
            do
            {
                currentHe.IncidentFace = newFace.Handle;
                currentHe = GetHalfEdgeByHandle(currentHe.Next.Id);

            } while (currentHe.Handle.Id != qStartHe.Handle.Id);
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges which are starting at a certain vertex.
        /// </summary>
        /// <param name="v">The start vertex.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> HalfEdgesStartingAtV(Vertex v)
        {
            var origin = v.IncidentHalfEdge;
            var halfEdge = GetHalfEdgeByHandle(origin.Id);
            do
            {
                if (halfEdge.Twin.Id != 0)
                {
                    var twin = GetHalfEdgeByHandle(halfEdge.Twin.Id);
                    halfEdge = GetHalfEdgeByHandle(twin.Next.Id);
                    yield return halfEdge.Handle;
                }
                else
                {
                    yield return halfEdge.Handle;
                    break;
                }
            } while (halfEdge.Handle.Id != origin.Id);
        }

        /// <summary>
        /// Gets a half edge by its handle
        /// </summary>
        /// <param name="id">The half edges' reference key</param>
        /// <returns></returns>
        public HalfEdge GetHalfEdgeByHandle(int id)
        {
            foreach (var e in _halfEdges)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + id + " not found!");
        }

        /// <summary>
        /// Gets a face by its handle
        /// </summary>
        /// <param name="id">The faces' reference key</param>
        /// <returns></returns>
        public Face GetFaceByHandle(int id)
        {
            foreach (var e in _faces)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + id + " not found!");
        }

        /// <summary>
        /// Gets a vertex by its handle
        /// </summary>
        /// <param name="id">The vertex' reference key</param>
        /// <returns></returns>
        public Vertex GetVerticeByHandle(int id)
        {
            foreach (var e in _vertices)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + id + " not found!");
        }

        #endregion

        #region private Methods

        private void CreateHalfEdgesForGeometry(IEnumerable<IList<float3>> faces)
        {
            foreach (var face in faces)
            {
                CreateHalfEdgesForFace(face);
            }
        }

        /// <summary>
        /// Creates half edges from a single face
        /// </summary>
        /// <param name="vertices">The vertices of the face, containing the geometric information - they have to be in ccw order</param>
        /// <returns></returns>
        private void CreateHalfEdgesForFace(IEnumerable<float3> vertices)
        {
            //Create new FaceHandle
            var faceId = new FaceHandle(FaceHandles.Count + 1);
            FaceHandles.Add(faceId);

            List<HalfEdge> faceHalfEdges;
            List<Vertex> faceVertices;

            InstantiateVerticesAndHalfEdges(vertices, out faceHalfEdges, out faceVertices);

            for (var i = 0; i < faceHalfEdges.Count; i++)
            {
                var current = faceHalfEdges[i];

                //Set IncidentFace
                current.IncidentFace = faceId;

                //Set HalfEdge.Next for each HalfEdge in this face
                current.Next = i + 1 < faceHalfEdges.Count ? faceHalfEdges[i + 1].Handle : faceHalfEdges[0].Handle;


                if (_halfEdges.Count == 0)
                {
                    faceHalfEdges[i] = current;
                    continue;
                }

                //Find and assign Twin by checking for existing half edges with opposit direction of the origin and target vertices
                var origin = current.Origin;
                var target = new VertHandle();
                foreach (var he in faceHalfEdges)
                {
                    if (he.Handle.Id == current.Next.Id)
                        target = he.Origin;
                }

                foreach (var halfEdge in _halfEdges)
                {
                    var compObj = GetHalfEdgeByHandle(halfEdge.Origin.Id);

                    var compOrigin = compObj.Origin;
                    var compTarget = GetHalfEdgeByHandle(compObj.Next.Id).Origin;

                    if (origin.Equals(compTarget) && target.Equals(compOrigin))
                    {
                        current.Twin = compObj.Handle;
                    }
                }
                faceHalfEdges[i] = current;
            }

            //Create a new Face
            var face = new Face
            {
                Handle = faceId,
                FirstHalfEdge = faceHalfEdges[0].Handle
            };
            _faces.Add(face);

            //Add the faces' Vertices to geometrys' list
            _vertices.AddRange(faceVertices);

            //Add the faces' HalfEdges to geometrys' list
            _halfEdges.AddRange(faceHalfEdges);

        }

        private void InstantiateVerticesAndHalfEdges(IEnumerable<float3> vertices, out List<HalfEdge> faceHalfEdges, out List<Vertex> faceVertices)
        {
            faceHalfEdges = new List<HalfEdge>();
            faceVertices = new List<Vertex>();

            foreach (var coord in vertices)
            {

                var vertHandle = new VertHandle();
                Vertex vert;
                var doesFaceExist = _faces.Count > 0;

                //Check if a Vertex already exists and assign it to the HalfEdge instead of createing a new
                if (doesFaceExist)
                {
                    foreach (var v in _vertices)
                    {
                        if (coord.Equals(v.Coord))
                            vertHandle.Id = v.Handle.Id;
                        else
                        {
                            //Create Vertice and VertHandle
                            vertHandle.Id = VertHandles.Count + 1;
                            VertHandles.Add(vertHandle);
                            vert = new Vertex(coord) { Handle = vertHandle };
                            faceVertices.Add(vert);
                            break;
                        }
                    }
                }
                else
                {
                    //Create Vertices and VertHandle
                    vertHandle.Id = VertHandles.Count + 1;
                    VertHandles.Add(vertHandle);
                    vert = new Vertex(coord) { Handle = vertHandle };
                    faceVertices.Add(vert);
                }

                //Create HalfEdges and HalfEdgeHandles
                var halfEdgeId = new HalfEdgeHandle(HalfEdgeHandles.Count + 1);
                HalfEdgeHandles.Add(halfEdgeId);
                var halfEdge = new HalfEdge
                {
                    Handle = halfEdgeId,
                    Origin = vertHandle,
                    Twin = new HalfEdgeHandle(0),
                    IncidentFace = new FaceHandle(0)
                };
                faceHalfEdges.Add(halfEdge);
            }
        }

        #endregion
    }
}



