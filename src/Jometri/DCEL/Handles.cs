using System;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// A handle to assign a abstract reference to a Vertex
    /// </summary>
    public struct VertHandle : IComparable<VertHandle>
    {
        /// <summary>
        /// Reference key for a vertex
        /// </summary>
        public int Id;

        internal VertHandle(int vertHandle)
        {
            Id = vertHandle;
        }

        #region  Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        /// <returns></returns>
        public static bool operator <(VertHandle first, VertHandle second)
        {
            return first.Id < second.Id;
        }

        /// <summary>
        /// Overload for "greater than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        /// <returns></returns>
        public static bool operator >(VertHandle first, VertHandle second)
        {
            return first.Id > second.Id;
        }

        /// <summary>
        /// Overload for != operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        /// <returns></returns>
        public static bool operator !=(VertHandle first, VertHandle second)
        {
            return first.Id != second.Id;
        }

        /// <summary>
        /// Overload for == operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        /// <returns></returns>
        public static bool operator ==(VertHandle first, VertHandle second)
        {
            return (first.Id == second.Id);
        }

        #endregion

        public int CompareTo(VertHandle other)
        {
           return Id.CompareTo(other.Id);
        }
    }

    /// <summary>
    /// A handle to assign a abstract reference to a HalfEdge
    /// </summary>
    public struct HalfEdgeHandle : IComparable<HalfEdgeHandle>
    {
        /// <summary>
        /// Reference key for a half edge
        /// </summary>
        public int Id;

        internal HalfEdgeHandle(int halfEdgeHandle)
        {
            Id = halfEdgeHandle;
        }

        #region   Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator <(HalfEdgeHandle first, HalfEdgeHandle second)
        {
            return first.Id < second.Id;
        }

        /// <summary>
        /// Overload for "greater than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator >(HalfEdgeHandle first, HalfEdgeHandle second)
        {
            return first.Id > second.Id;
        }

        /// <summary>
        /// Overload for != operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator !=(HalfEdgeHandle first, HalfEdgeHandle second)
        {
            return first.Id != second.Id;
        }

        /// <summary>
        /// Overload for == operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator ==(HalfEdgeHandle first, HalfEdgeHandle second)
        {
            return (first.Id == second.Id);
        }
        #endregion

        public int CompareTo(HalfEdgeHandle other)
        {
            return Id.CompareTo(other.Id);
        }
    }

    /// <summary>
    /// A handle to assign a abstract reference to a Face
    /// </summary>
    public struct FaceHandle : IComparable<FaceHandle>
    {
        /// <summary>
        /// Reference key for a face
        /// </summary>
        public int Id;

        internal FaceHandle(int faceHandle)
        {
            Id = faceHandle;
        }

        #region   Overloading comparison operators

        /// <summary>
        /// Overload for "smaller than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator <(FaceHandle first, FaceHandle second)
        {
            return first.Id < second.Id;
        }

        /// <summary>
        /// Overload for "greater than" operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator >(FaceHandle first, FaceHandle second)
        {
            return first.Id > second.Id;
        }

        /// <summary>
        /// Overload for != operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator !=(FaceHandle first, FaceHandle second)
        {
            return first.Id != second.Id;
        }

        /// <summary>
        /// Overload for == operator
        /// </summary>
        /// <param name="first">First parameter to be compared</param>
        /// <param name="second">Second parameter to be compared</param>
        public static bool operator ==(FaceHandle first, FaceHandle second)
        {
            return (first.Id == second.Id);
        }
        #endregion

        public int CompareTo(FaceHandle other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}

