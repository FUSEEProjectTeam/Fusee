using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IDisposable
    {
        #region RenderContext Asset Management
        // Event of mesh Data changes
        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshDataEventArgs> MeshChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();
        #endregion

        private float3[] _vertices;

        private float3[] _biTangents;
        private float4[] _tangents;

        /// <summary>
        /// Gets and sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public float3[] Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Vertices));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get { return (_vertices != null) && _vertices.Length > 0; } }

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool TangentsSet { get { return (_tangents != null) && _tangents.Length > 0; } }

        /// <summary>
        /// Gets a value indicating whether bitangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bitangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool BiTangentsSet { get { return (_biTangents != null) && _biTangents.Length > 0; } }

        private uint[] _colors;
        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public uint[] Colors
        {
            get { return _colors; }
            set
            {
                _colors = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Colors));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return (_colors != null) && _colors.Length > 0; } }

        private float3[] _normals;
        /// <summary>
        /// Gets and sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        public float3[] Normals
        {
            get { return _normals; }
            set
            {
                _normals = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Normals));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get { return (_normals != null) && _normals.Length > 0; } }

        private float2[] _uvs;
        /// <summary>
        /// Gets and sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        public float2[] UVs
        {
            get { return _uvs; }
            set
            {
                _uvs = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Uvs));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return (_uvs != null) && _uvs.Length > 0; } }

        private float4[] _boneWeights;
        /// <summary>
        /// Gets and sets the bone weights.
        /// </summary>
        /// <value>
        /// The bone weights.
        /// </value>
        public float4[] BoneWeights
        {
            get { return _boneWeights; }
            set
            {
                _boneWeights = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.BoneWeights));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return (_boneWeights != null) && _boneWeights.Length > 0; } }

        private float4[] _boneIndices;
        /// <summary>
        /// Gets and sets the bone indices.
        /// </summary>
        /// <value>
        /// The bone indices.
        /// </value>
        public float4[] BoneIndices
        {
            get { return _boneIndices; }
            set
            {
                _boneIndices = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.BoneIndices));
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return (_boneIndices != null) && _boneIndices.Length > 0; } }

        private ushort[] _triangles;
        /// <summary>
        /// Gets and sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        public ushort[] Triangles
        {
            get { return _triangles; }
            set
            {
                _triangles = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Triangles));
                }
            }


        }

        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get { return (_triangles != null) && _triangles.Length > 0; } }

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox;

        /// <summary>
        /// The tangent of each triangle for bump mapping.
        /// w-component is handedness
        /// </summary>
        public float4[] Tangents
        {
            get { return _tangents; }
            set
            {
                _tangents = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.Tangents));
                }
            }
        }

        /// <summary>
        /// The bitangent of each triangle for bump mapping.
        /// </summary>
        public float3[] BiTangents
        {
            get { return _biTangents; }
            set
            {
                _biTangents = value;
                var del = this.MeshChanged;
                if (del != null)
                {
                    del(this, new MeshDataEventArgs(this, MeshChangedEnum.BiTangents));
                }
            }
        }

        /// <summary>
        /// Used by various visitors such as rendering and picking. If set to true the mesh contributes to the traversal result. 
        /// </summary>
        public bool Active = true;

        public int MeshType = 0;

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Disposed));

            disposed = true;
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire MeshChanged event.
        /// </summary>
        ~Mesh()
        {
            Dispose(false);
        }
    }
}
