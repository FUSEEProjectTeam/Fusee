using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Mesh
    {
        internal IMeshImp _meshImp;
        private float3[] _vertices;
        public float3[] Vertices
        {
            get { return _vertices; }
            set { if (_meshImp!= null) _meshImp.InvalidateVertices(); _vertices = value; }
        }
        public bool VerticesSet { get { return (_meshImp!= null) && _meshImp.VerticesSet; } }

        private uint[] _colors;
        public uint[] Colors
        {
            get { return _colors; }
            set { if (_meshImp != null) _meshImp.InvalidateColors(); _colors = value; }
        }
        public bool ColorsSet { get { return (_meshImp != null) && _meshImp.ColorsSet; } }

        private float3[] _normals;
        public float3[] Normals
        {
            get { return _normals; }
            set { if (_meshImp != null) _meshImp.InvalidateNormals(); _normals = value; }
        }
        public bool NormalsSet { get { return (_meshImp != null) && _meshImp.NormalsSet; } }

        private float2[] _uvs;
        public float2[] UVs
        {
            get { return _uvs; }
            set { if (_meshImp != null) _meshImp.InvalidateUVs(); _uvs = value; }
        }
        public bool UVsSet { get { return (_meshImp != null) && _meshImp.UVsSet; } }



        private short[] _triangles;
        public short[] Triangles
        {
            get { return _triangles; }
            set { if (_meshImp != null) _meshImp.InvalidateTriangles(); _triangles = value; }
        }
        public bool TrianglesSet { get { return (_meshImp != null) && _meshImp.TrianglesSet; } }


    }
}



