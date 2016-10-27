namespace Fusee.Jometri
{
    /// <summary>
    /// A handle to assign a abstract reference to a Vertex
    /// </summary>
    public struct VertHandle
    {
        internal int Id;

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
        internal int Id;

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
        internal int Id;

        internal FaceHandle(int faceHandle)
        {
            Id = faceHandle;
        }
    }
}

