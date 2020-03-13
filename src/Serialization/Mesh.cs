using System;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Allows creating or modifying geometry. 
    /// A mesh contains vertex data in the form of positions, normals, texture coordinates and face data as an array of triangles.
    /// </summary>
    [ProtoContract]
    public class Mesh : SceneComponentContainer, IDisposable
    {
        #region Fields

        #region RenderContext Asset Management

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
        [ProtoMember(1)]
        public float3[] Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Vertices));
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
        [ProtoMember(2)]
        public uint[] Colors
        {
            get { return _colors; }
            set
            {
                _colors = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Colors));
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
        [ProtoMember(3)]
        public float3[] Normals
        {
            get { return _normals; }
            set
            {
                _normals = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Normals));
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
        [ProtoMember(4)]
        public float2[] UVs
        {
            get { return _uvs; }
            set
            {
                _uvs = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Uvs));
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
        [ProtoMember(5)]
        public float4[] BoneWeights
        {
            get { return _boneWeights; }
            set
            {
                _boneWeights = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.BoneWeights));
            }
        }
        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        /// <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return (_boneWeights != null) && _boneWeights.Length > 0; } }

        private float4[] _boneIndices;
        /// <summary>
        /// Gets and sets the bone indices.
        /// </summary>
        /// <value>
        /// The bone indices.
        /// </value>
        [ProtoMember(6)]
        public float4[] BoneIndices
        {
            get { return _boneIndices; }
            set
            {
                _boneIndices = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.BoneIndices));
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
        [ProtoMember(7)]
        public ushort[] Triangles
        {
            get { return _triangles; }
            set
            {
                _triangles = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Triangles));
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
        /// The (model space) bounding box of this geometry.
        /// Note: needs to be calculated in order to enable frustum culling for this mesh. 
        /// </summary>
        [ProtoMember(8)]
        public AABBf BoundingBox;

        /// <summary>
        /// The tangent of each triangle for bump mapping.
        /// w-component is handedness
        /// </summary>
        [ProtoMember(9)]
        public float4[] Tangents
        {
            get { return _tangents; }
            set
            {
                _tangents = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Tangents));
            }
        }

        /// <summary>
        /// The bitangent of each triangle for bump mapping.
        /// </summary>
        [ProtoMember(10)]
        public float3[] BiTangents
        {
            get { return _biTangents; }
            set
            {
                _biTangents = value;
                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.BiTangents));
            }
        }

        /// <summary>
        /// If set to true the mesh will be rendered and pickable.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// The mesh type.
        /// </summary>
        [ProtoMember(11)]
        public int MeshType = 0;

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Invoke mesh deletion on GPU 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Disposed));
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Invoke mesh deletion on GPU 
        /// </summary>       
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion



    }
}



