using System;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Provides the abillity to create or interact directly with the point data.
    /// </summary>
    [ProtoContract]
    public class Mesh : SceneComponentContainer, IDisposable
    {
        #region Fields

        #region RenderContext Asset Management
        // Event of mesh Data changes
        /// <summary>
        /// MeshChanged event notifies the observing MeshManager about changes of the Mesh's properties or its disposal.
        /// </summary>
        public event EventHandler<MeshDataEventArgs> MeshChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used in order to recognize each Mesh. The identifier is not saved in persistent memory.
        /// </summary>
        public readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();
        #endregion

        private float3[] _vertices;
        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        [ProtoMember(1)]
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
        public bool VerticesSet { get { return (_vertices!= null) && _vertices.Length > 0; } }
        
        private uint[] _colors;
        /// <summary>
        /// Gets or sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [ProtoMember(2)]
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
        ///   <c>true</c> if a colore is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return (_colors != null) && _colors.Length > 0; } }
        
        private float3[] _normals;
        /// <summary>
        /// Gets or sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        [ProtoMember(3)]
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
        /// Gets or sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        [ProtoMember(4)]
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
        /// Gets or sets the boneweights.
        /// </summary>
        /// <value>
        /// The boneweights.
        /// </value>
        [ProtoMember(5)]
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
        /// Gets a value indicating whether boneweights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if boneweights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return (_boneWeights != null) && _boneWeights.Length > 0; } }

        private float4[] _boneIndices;
        /// <summary>
        /// Gets or sets the boneindices.
        /// </summary>
        /// <value>
        /// The boneindices.
        /// </value>
        [ProtoMember(6)]
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
        /// Gets a value indicating whether boneindices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if boneindices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return (_boneIndices != null) && _boneIndices.Length > 0; } }

        private ushort[] _triangles;
        /// <summary>
        /// Gets or sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        [ProtoMember(7)]
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
        [ProtoMember(8)]
        public AABBf BoundingBox;

        #endregion

        /// <summary>
        /// Implementation of the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            var del = MeshChanged;
            if (del != null)
            {
                del(this, new MeshDataEventArgs(this, MeshChangedEnum.Disposed));
            }
        }

        /// <summary>
        /// Destructor calls Dispose in order to fire MeshChanged event
        /// </summary>
        ~Mesh()
        {
            Dispose();
        }
    }
}



