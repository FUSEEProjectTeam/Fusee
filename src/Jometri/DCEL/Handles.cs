namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// A handle to assign a abstract reference to a Vertex
    /// </summary>
    public struct VertHandle
    {
        /// <summary>
        /// Reference key for a vertex
        /// </summary>
        public int Id;

        internal VertHandle(int vertHandle)
        {
            Id = vertHandle;
        }
    }

    /// <summary>
    /// A handle to assign a abstract reference to a HalfEdge
    /// </summary>
    public struct HalfEdgeHandle
    {
        /// <summary>
        /// Reference key for a half edge
        /// </summary>
        public int Id;

        internal HalfEdgeHandle(int halfEdgeHandle)
        {
            Id = halfEdgeHandle;
        }
    }

    /// <summary>
    /// A handle to assign a abstract reference to a Face
    /// </summary>
    public struct FaceHandle
    {
        /// <summary>
        /// Reference key for a face
        /// </summary>
        public int Id;

        internal FaceHandle(int faceHandle)
        {
            Id = faceHandle;
        }
    }
}

