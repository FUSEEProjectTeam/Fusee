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
        /// The vertex' reference.
        /// </summary>
        public int Handle;

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
        /// <param name="pos">The coordinate of the vertex.</param>
        public Vertex(float3 pos)
        {
            Handle = default(int);
            IncidentHalfEdge = default(int);
            VertData = new VertexData { Pos = pos };
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the vertex' handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(Vertex first, Vertex second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the vertex' handle.
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
    /// A handle to the half edges' origin vertex.
    /// A handle to the next half edge.
    /// A handle to the previous half edge.
    /// A handle to the face it belongs to.
    /// Attribute information, e.g. the normal and the texture coordinates.
    /// </summary>
    public struct HalfEdge
    {
        /// <summary>
        /// The half edges' handle
        /// </summary>
        public int Handle;

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

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the HalfEdges' handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(HalfEdge first, HalfEdge second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the HalfEdges' handle
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
    /// A handle to one of the half edges that belongs to the faces outer boundary.
    /// A List that contains handles to one half edge for each hole in a face.
    /// Attribute information, e.g. the face nromal.
    /// Note that unbounded faces can't have a OuterHalfEdge but must have at least one InnerHalfEdge - bounded faces must have a OuterComponent.
    /// </summary>
    public struct Face
    {
        public int Handle;

        public int OuterHalfEdge;

        public FaceData FaceData;

        internal List<int> InnerHalfEdges;

        /// <summary>
        /// Constructor for creating a new Face.
        /// </summary>
        public Face(int handle = 0, int outerHalfEdge = 0) : this()
        {
            Handle = handle;
            OuterHalfEdge = outerHalfEdge;
            FaceData = new FaceData();
            InnerHalfEdges = new List<int>();
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the Faces' handle.
        /// </summary>
        /// <param name="first">First comparison parameter.</param>
        /// <param name="second">Second comparison parameter.</param>
        /// <returns></returns>
        public static bool operator <(Face first, Face second)
        {
            return first.Handle < second.Handle;
        }

        /// <summary>
        /// Overload for "smaller than" operator. Comparison based on the Faces' handle.
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
