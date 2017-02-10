using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// Each vertex contains:
    /// A handle to assign a abstract reference to it.
    /// Attribute information, e.g. the position of the vertex.
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// The vertex's reference.
        /// </summary>
        public readonly int Handle;

        /// <summary>
        /// The handle to the half edge with this vertex as origin.
        /// </summary>
        internal int IncidentHalfEdge;

        /// <summary>
        /// Attribute information.
        /// </summary>
        public VertexData VertData;

        /// <summary>
        /// Constructor for creating a new Vertex.
        /// </summary>
        /// <param name="handle">The reference of the vertex.</param>
        /// <param name="pos">The coordinate of the vertex.</param>
        public Vertex(int handle, float3 pos)
        {
            Handle = handle;
            IncidentHalfEdge = default(int);
            VertData = new VertexData { Pos = pos };
        }

        /// <summary>
        /// Constructor for creating a new Vertex fromn an old one, changing its coordinate.
        /// </summary>
        /// <param name="vert">The old vertex.</param>
        /// <param name="newPos">The new coordinate of the vertex.</param>
        public Vertex(Vertex vert, float3 newPos)
        {
            Handle = vert.Handle;
            IncidentHalfEdge = vert.IncidentHalfEdge;
            VertData = vert.VertData;

            VertData.Pos = newPos;
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the vertex's handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(Vertex first, Vertex second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the vertex's handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator >(Vertex first, Vertex second)
        {
            return first.Handle > second.Handle;
        }

        /// <summary>
        /// Overload for != operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator !=(Vertex first, Vertex second)
        {
            return first.Handle != second.Handle;
        }

        /// <summary>
        /// Overload for == operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator ==(Vertex first, Vertex second)
        {
            return first.Handle == second.Handle;
        }

        /// <summary>Overwrites "Equals".</summary>
        /// <returns>true, if <paramref name="obj" /> and this instance of the object are of the same type and represent the same value.</returns>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object obj)
        {
            if (!(obj is Vertex))
                return false;

            var comp = (Vertex)obj;
            return this == comp;

        }


        /// <summary>Overwrites GetHashCode.</summary>
        /// <returns>Handle as code.</returns>
        public override int GetHashCode()
        {
            return Handle;
        }

        #endregion

    }

    /// <summary>
    /// Represents a half edge.
    /// Each half edge contains:
    /// A handle to assign a abstract reference to it.
    /// A handle to the half edge's origin vertex.
    /// A handle to the next half edge.
    /// A handle to the previous half edge.
    /// A handle to the face it belongs to.
    /// Attribute information, e.g. the normal and the texture coordinates.
    /// </summary>
    public struct HalfEdge
    {
        /// <summary>
        /// The half edge's handle
        /// </summary>
        public readonly int Handle;

        internal int OriginVertex;
        internal int TwinHalfEdge;
        internal int NextHalfEdge;
        internal int PrevHalfEdge;
        internal int IncidentFace;

        /// <summary>
        /// Attribute information.
        /// </summary>
        public HalfEdgeData HalfEdgeData;

        /// <summary>
        /// Constructor for creating a new HalfEdge.
        /// </summary>
        public HalfEdge(int handle = 0, int originVertex = 0, int twinHalfEdge = 0, int nextHalfEdge = 0, int prevHalfEdge = 0, int incidentFace = 0)
        {
            Handle = handle;
            OriginVertex = originVertex;
            TwinHalfEdge = twinHalfEdge;
            NextHalfEdge = nextHalfEdge;
            PrevHalfEdge = prevHalfEdge;
            IncidentFace = incidentFace;
            HalfEdgeData = new HalfEdgeData();
        }

        /// <summary>
        /// Constructor for creating a new HalfEdge.
        /// </summary>
        /// <param name="handle">The reference of the half edge.</param>
        public HalfEdge(int handle)
        {
            Handle = handle;
            OriginVertex = 0;
            TwinHalfEdge = 0;
            NextHalfEdge = 0;
            PrevHalfEdge = 0;
            IncidentFace = 0;
            HalfEdgeData = new HalfEdgeData();
        }

        /// <summary>
        /// Constructor for creating a new HalfEdge from another one.
        /// </summary>
        /// <param name="handle">The reference of the half edge.</param>
        /// <param name="halfEdge">The original HalfEdge.</param>
        public HalfEdge(int handle, HalfEdge halfEdge)
        {
            Handle = handle;
            OriginVertex = halfEdge.OriginVertex;
            TwinHalfEdge = halfEdge.TwinHalfEdge;
            NextHalfEdge = halfEdge.NextHalfEdge;
            PrevHalfEdge = halfEdge.PrevHalfEdge;
            IncidentFace = halfEdge.IncidentFace;
            HalfEdgeData = halfEdge.HalfEdgeData;
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the HalfEdge's handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(HalfEdge first, HalfEdge second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the HalfEdge's handle
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator >(HalfEdge first, HalfEdge second)
        {
            return first.Handle > second.Handle;
        }

        /// <summary>
        /// Overload for != operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator !=(HalfEdge first, HalfEdge second)
        {
            return first.Handle != second.Handle;
        }

        /// <summary>
        /// Overload for == operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator ==(HalfEdge first, HalfEdge second)
        {
            return (first.Handle == second.Handle);
        }

        /// <summary>Overwrites "Equals".</summary>
        /// <returns>true, if <paramref name="obj" /> and this instance are of the same type and represent the same value.</returns>
        /// <param name="obj">Comparison object.</param>
        public override bool Equals(object obj)
        {
            if (!(obj is HalfEdge))
                return false;

            var comp = (HalfEdge)obj;
            return this == comp;

        }


        /// <summary>Overwrites GetHashCode</summary>
        /// <returns>Handle as hash code.</returns>
        public override int GetHashCode()
        {
            return Handle;
        }

        #endregion
    }


    /// <summary>
    /// Each face belonging to a 2D geometry contains:
    /// A handle to assign a abstract reference to it.
    /// A referance to one of the half edges that belongs to the faces outer boundary.
    /// A List that contains handles to one half edge for each hole in a face.
    /// Attribute information, e.g. the face nromal.
    /// Note that unbounded faces can't have a OuterHalfEdge but must have at least one InnerHalfEdge - bounded faces must have a OuterComponent.
    /// </summary>
    public struct Face
    {
        /// <summary>
        /// The reference of the face.
        /// </summary>
        public readonly int Handle;

        /// <summary>
        ///  A reference to one of the half edges that belongs to the faces outer boundary.
        /// </summary>
        public int OuterHalfEdge;

        /// <summary>
        /// Attribute information, e.g. the face nromal.
        /// </summary>
        public FaceData FaceData;

        internal List<int> InnerHalfEdges;

        /// <summary>
        /// Constructor for creating a new Face.
        /// </summary>
        public Face(int handle = 0, int outerHalfEdge = 0)
        {
            Handle = handle;
            OuterHalfEdge = outerHalfEdge;
            FaceData = new FaceData();
            InnerHalfEdges = new List<int>();
        }

        /// <summary>
        /// Constructor for creating a new Face.
        /// </summary>
        /// <param name="handle">The reference of the face.</param>
        public Face(int handle)
        {
            Handle = handle;
            OuterHalfEdge = 0;
            FaceData = new FaceData();
            InnerHalfEdges = new List<int>();
        }

        /// <summary>
        /// Constructor for creating a new Face from an other one.
        /// </summary>
        /// <param name="handle">The reference of the face.</param>
        /// <param name="face">The original face.</param>
        public Face(int handle, Face face)
        {
            Handle = handle;
            OuterHalfEdge = face.OuterHalfEdge;
            FaceData = face.FaceData;
            InnerHalfEdges = new List<int>(face.InnerHalfEdges);
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the Face's handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(Face first, Face second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the Face's handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator >(Face first, Face second)
        {
            return first.Handle > second.Handle;
        }

        /// <summary>
        /// Overload for != operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator !=(Face first, Face second)
        {
            return first.Handle != second.Handle;
        }

        /// <summary>
        /// Overload for == operator.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator ==(Face first, Face second)
        {
            return (first.Handle == second.Handle);
        }

        /// <summary>Overwrites "Equals"</summary>
        /// <returns>true, if <paramref name="obj" /> and this instance are of the same type and represent the same value.</returns>
        /// <param name="obj">Comparison object.</param>
        public override bool Equals(object obj)
        {
            if (!(obj is Face))
                return false;

            var comp = (Face)obj;
            return this == comp;

        }


        /// <summary>Overwrites GetHashCode</summary>
        /// <returns>Handle as hash code.</returns>
        public override int GetHashCode()
        {
            return Handle;
        }

        #endregion
    }
}
