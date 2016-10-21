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

        private List<Vertex> Vertices;
        private List<HalfEdge> HalfEdges;
        private List<Face> Faces;

        /// <summary>
        /// Stores geometry in a DCEL (doubly conneted edge list).
        /// </summary>
        public Geometry()
        {
            HalfEdgeHandles = new List<HalfEdgeHandle>();
            FaceHandles = new List<FaceHandle>();
            VertHandles = new List<VertHandle>();

            Vertices = new List<Vertex>();
            HalfEdges = new List<HalfEdge>();
            Faces = new List<Face>();
        }
        #endregion

        #region Structs

        /// <summary>
        /// Each face contains:
        /// A handle to assign a abstract reference to it.
        /// A handle to the first half edge that belongs to this face.
        /// </summary>
        internal struct Face
        {
            internal FaceHandle Handle;
            internal HalfEdgeHandle FirstHalfEdge;
        }

        /// <summary>
        /// Each vertex contains:
        /// A handle to assign a abstract reference to it.
        /// The vertex' coordinates.
        /// </summary>
        internal struct Vertex
        {
            internal VertHandle Handle;

            internal float3 Coord;
            internal HalfEdgeHandle IncidentHalfEdge;

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
        internal struct HalfEdge
        {
            internal HalfEdgeHandle Handle;

            internal VertHandle Origin;
            internal HalfEdgeHandle Twin;
            internal HalfEdgeHandle Next;
            internal FaceHandle IncidentFace;
        }
        #endregion

        #region public Methodes

        //e.g. Get all edges adjecant to a vertex
        //Get all edges that belong to a face
        //and so on

        #endregion

        #region private Methodes
        /// <summary>
        /// Creates half edges from a single face
        /// </summary>
        /// <param name="vertices">The vertices of the face - they have to be in ccw order</param>
        /// <returns></returns>
        private void CreateHalfEdgesFromFace(IEnumerable<float3> vertices)
        {
            var faceHalfEdges = new List<HalfEdge>();
            foreach (var coord in vertices)
            {
                //Create Vertices and VertHandles for each float3
                var vertId = new VertHandle(VertHandles.Count + 1);
                VertHandles.Add(vertId);
                var vert = new Vertex(coord) { Handle = vertId };
                Vertices.Add(vert);

                //Create HalfEdges and HalfEdgeHandles (as many as float3s in the list)
                var halfEdgeId = new HalfEdgeHandle(HalfEdgeHandles.Count + 1);
                HalfEdgeHandles.Add(halfEdgeId);
                var halfEdge = new HalfEdge
                {
                    Handle = halfEdgeId,
                    Origin = vert.Handle,
                    Twin = new HalfEdgeHandle(0)
                };
                faceHalfEdges.Add(halfEdge);
            }

            for (var i = 0; i < faceHalfEdges.Count; i++)
            {
                //Set HalfEdge.Next for each HalfEdge in this face
                if (i + 1 > faceHalfEdges.Count)
                {
                    var current = faceHalfEdges[i];
                    current.Next = faceHalfEdges[0].Handle;
                    faceHalfEdges[i] = current;
                }
                else
                {
                    var current = faceHalfEdges[i];
                    current.Next = faceHalfEdges[i + 1].Handle;
                    faceHalfEdges[i] = current;
                }

                //Find and assign twin half edges by looking for existing half edges with opposit direction of the origin and target vertices
                if (HalfEdgeHandles.Count == 0) continue;

                var thisHalfEdge = faceHalfEdges[i];
                var origin = thisHalfEdge.Origin;
                var target = GetHalfEdge(thisHalfEdge.Next.Id).Origin;

                foreach (var handle in HalfEdgeHandles)
                {
                    var compObj = GetHalfEdge(handle.Id);
                    var compOrigin = compObj.Origin;
                    var compTarget = GetHalfEdge(compObj.Next.Id).Origin;
                    if (origin.Equals(compTarget) && target.Equals(compOrigin))
                    {
                        thisHalfEdge.Twin = compObj.Handle;
                    }
                    faceHalfEdges[i] = thisHalfEdge;
                }
            }

            //Add the faces HalfEdges to the geometrys' list
            HalfEdges.AddRange(faceHalfEdges);

            //Create a new Face and FaceHandle
            var faceId = new FaceHandle(FaceHandles.Count + 1);
            FaceHandles.Add(faceId);
            var face = new Face
            {
                Handle = faceId,
                FirstHalfEdge = faceHalfEdges[0].Handle
            };
            Faces.Add(face);
        }

        private HalfEdge GetHalfEdge(int id)
        {
            foreach (var e in HalfEdges)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("Handle with ID " + id + " was not found!");
        }

        private Face GetFace(int id)
        {
            foreach (var e in Faces)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("Handle with ID " + id + " was not found!");
        }

        private Vertex GetVertice(int id)
        {
            foreach (var e in Vertices)
            {
                if (e.Handle.Id == id)
                    return e;
            }
            throw new HandleNotFoundException("Handle with ID " + id + " was not found!");
        }
    }
    #endregion
}