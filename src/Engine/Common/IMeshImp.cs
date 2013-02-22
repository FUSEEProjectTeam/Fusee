#pragma warning disable 1591 //disables the warning about missing XML-comments

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

#pragma warning restore 1591