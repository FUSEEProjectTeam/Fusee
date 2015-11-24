using Fusee.Engine;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides the abillity to create or interact directly with the point data.
    /// </summary>
    /// <remarks>For an example how you can use it, see <see cref="Cube"/>.</remarks>
     
    [ProtoContract]
    public class Mesh
    {
        #region Fields

        internal IMeshImp _meshImp;
        
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
            set { if (_meshImp!= null) _meshImp.InvalidateVertices(); _vertices = value; }
        }
        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get { return (_meshImp!= null) && _meshImp.VerticesSet; } }
        
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
            set { if (_meshImp != null) _meshImp.InvalidateColors(); _colors = value; }
        }
        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a colore is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return (_meshImp != null) && _meshImp.ColorsSet; } }
        
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
            set { if (_meshImp != null) _meshImp.InvalidateNormals(); _normals = value; }
        }
        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get { return (_meshImp != null) && _meshImp.NormalsSet; } }
        
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
            set { if (_meshImp != null) _meshImp.InvalidateUVs(); _uvs = value; }
        }
        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return (_meshImp != null) && _meshImp.UVsSet; } }

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
            set { if (_meshImp != null) _meshImp.InvalidateBoneWeights(); _boneWeights = value; }
        }
        /// <summary>
        /// Gets a value indicating whether boneweights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if boneweights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return (_meshImp != null) && _meshImp.BoneWeightsSet; } }

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
            set { if (_meshImp != null) _meshImp.InvalidateBoneIndices(); _boneIndices = value; }
        }
        /// <summary>
        /// Gets a value indicating whether boneindices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if boneindices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return (_meshImp != null) && _meshImp.BoneIndicesSet; } }

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
            set { if (_meshImp != null) _meshImp.InvalidateTriangles(); _triangles = value; }
        }
        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get { return (_meshImp != null) && _meshImp.TrianglesSet; } }
        #endregion
    }
}



