using System;
using System.Collections.Generic;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Stores threedimensional, polygonal geometry and provides methods for manipulation. 
    /// To actually render the geometry in the engine, convert Geometry to Mesh objects.
    /// </summary>
    public class Geometry
    {
        public class Face
        {
            public int[] InxVert;
            public int[] InxNormal;
            public int[] InxTexCoord;
        }

        internal List<double3> _vertices;

        /// <summary>
        /// The list of vertices (3D positions
        /// </summary>
        public IList<double3> Vertices
        {
            set { _vertices = new List<double3>(value); }
            get { return _vertices; }
        }
        internal List<double3> _normals;
        public IList<double3> Normals
        {
            set { _vertices = new List<double3>(value); }
            get { return _vertices; }
        }
        internal List<double2> _texCoords;
        public IList<double2> TexCoords
        {
            set { _texCoords = new List<double2>(value); }
            get { return _texCoords; }
        }

        internal List<Face> _faces;
        public IList<Face> Faces
        {
            set { _faces = new List<Face>(value); }
            get { return _faces; }
        }

        public Geometry()
        {
            _vertices = new List<double3>();
        }

        public int AddVertex(double3 v)
        {
            _vertices.Add(v);
            return _vertices.Count - 1;
        }

        public int AddTexCoord(double2 uv)
        {
            if (_texCoords == null)
                _texCoords = new List<double2>();
            _texCoords.Add(uv);
            return _texCoords.Count - 1;
        }

        public int AddNormal(double3 normal)
        {
            if (_normals == null)
                _normals = new List<double3>();
            _normals.Add(normal);
            return _normals.Count - 1;
        }

        public int AddFace(int[] vertInx, int[] texCoordInx, int[] normalInx)
        {
            int i;
            Face f = new Face();
            
            // Plausibility checks interleaved...
            if (vertInx == null)
                throw new ArgumentNullException("vertInx");

            f.InxVert = new int[vertInx.Length];
            for(i = 0; i < vertInx.Length; i++)
            {
                if (!(0 <= vertInx[i] && vertInx[i] < _vertices.Count))
                    throw new ArgumentException("Vertex index out of range: " + vertInx[i], "vertInx[" + i + "]");
                f.InxVert[i] = vertInx[i];
            }

            if (texCoordInx != null)
            {
                if (texCoordInx.Length != vertInx.Length)
                    throw new ArgumentException(
                        "Number of texture coordinate indices must match number of vertex indices", "texCoordInx");

                f.InxTexCoord = new int[texCoordInx.Length];
                for (i = 0; i < texCoordInx.Length; i++)
                {
                    if (!(0 <= texCoordInx[i] && texCoordInx[i] < _texCoords.Count))
                        throw new ArgumentException("Texture coordinate index out of range: " + texCoordInx[i], "texCoordInx[" + i + "]");
                    f.InxTexCoord[i] = texCoordInx[i];
                }
            }

            if (normalInx != null)
            {
                if (normalInx.Length != vertInx.Length)
                    throw new ArgumentException("Number of normal indices must match number of vertex indices",
                                                "normalInx");

                f.InxTexCoord = new int[normalInx.Length];
                for (i = 0; i < normalInx.Length; i++)
                {
                    if (!(0 <= normalInx[i] && normalInx[i] < _normals.Count))
                        throw new ArgumentException("Normal index out of range: " + normalInx[i], "normalInx[" + i + "]");
                    f.InxNormal[i] = normalInx[i];
                }
            }

            // Actually add the faces
            if (_faces == null)
                _faces = new List<Face>();

            _faces.Add(f);
            return _faces.Count - 1;
        }

        public bool HasNormals
        {
            get { return _normals != null; }
        }

        public bool HasTexCoords
        {
            get { return _texCoords != null; }
        }

        struct TripleInx
        {
            public int iV, iT, iN;
            public override int GetHashCode()
            {
                return iV.GetHashCode() ^ iT.GetHashCode() ^ iN.GetHashCode();
            }
        }

        public Mesh ToMesh()
        {
            // TODO: make a big case decision based on HasTexCoords and HasNormals around the implementation and implement each case individually

            Dictionary<TripleInx, int> _vDict = new Dictionary<TripleInx, int>();

            List<short> mTris = new List<short>();
            List<float3> mVerts = new List<float3>();
            List<float2> mTexCoords = (HasTexCoords) ? new List<float2>() : null;
            List<float3> mNormals = (HasNormals) ? new List<float3>() : null;

            foreach (Face f in _faces)
            {
                int[] mFace = new int[f.InxVert.Length];
                for (int i = 0; i < f.InxVert.Length; i++)
                {
                    TripleInx ti = new TripleInx()
                                       {
                                           iV = f.InxVert[i],
                                           iT = (HasTexCoords) ? f.InxTexCoord[i] : 0,
                                           iN = (HasNormals) ? f.InxNormal[i] : 0
                                       };
                    int inx;
                    if (!_vDict.TryGetValue(ti, out inx))
                    {
                        // Create a new vertex triplet combination
                        int vInx = f.InxVert[i];
                        mVerts.Add(new float3((float) _vertices[vInx].x, (float) _vertices[vInx].y,
                                              (float) _vertices[vInx].z));
                        if (HasTexCoords)
                        {
                            int tInx = f.InxTexCoord[i];
                            mTexCoords.Add(new float2((float) _texCoords[tInx].x, (float) _texCoords[tInx].y));
                        }
                        if (HasNormals)
                        {
                            int nInx = f.InxNormal[i];
                            mNormals.Add(new float3((float) _normals[nInx].x, (float) _normals[nInx].y,
                                                    (float) _normals[nInx].z));
                        }
                        inx = mVerts.Count - 1;
                        _vDict.Add(ti, inx);
                    }
                    mFace[i] = inx;
                }
                mTris.AddRange(Triangulate(f, mFace));
            }

            Mesh m = new Mesh();
            m.Vertices = mVerts.ToArray();
            if (HasNormals)
                m.Normals = mNormals.ToArray();
            if (HasTexCoords)
                m.UVs = mTexCoords.ToArray();

            m.Triangles = mTris.ToArray();
            return m;
        }

        private short[] Triangulate(Face f, int[] indices)
        {
            if (f.InxVert.Length < 3)
                return null;

            if (indices == null)
                indices = f.InxVert;

            short[] ret = new short[3 * (f.InxVert.Length-2)];
            // Perform a fan triangulation
            for (int i = 2; i < f.InxVert.Length; i++ )
            {
                ret[(i - 2)*3 + 0] = (short)indices[0];
                ret[(i - 2)*3 + 1] = (short)indices[i - 1];
                ret[(i - 2)*3 + 2] = (short)indices[i];
            }
            return ret;
        }
     
    }
}
