namespace Fusee.Jometri
{
    public struct VertHandle
    {
        internal int Id;

        internal VertHandle(int vertHandle)
        {
            Id = vertHandle;
        }
    }

    public struct HalfEdgeHandle
    {
        internal int Id;

        internal HalfEdgeHandle(int halfEdgeHandle)
        {
            Id = halfEdgeHandle;
        }
    }

    public struct FaceHandle
    {
        internal int Id;

        internal FaceHandle(int faceHandle)
        {
            Id = faceHandle;
        }
    }
}
