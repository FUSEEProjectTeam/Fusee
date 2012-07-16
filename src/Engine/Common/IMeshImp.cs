namespace Fusee.Engine
{
    public interface IMeshImp
    {
        void InvalidateVertices();
        bool VerticesSet { get; }
        void InvalidateNormals();
        bool NormalsSet { get; }
        void InvalidateColors();
        bool ColorsSet { get; }
        void InvalidateTriangles();
        bool TrianglesSet { get; }
        bool UVsSet { get; }
        void InvalidateUVs();
    }
}
