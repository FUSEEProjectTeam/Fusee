using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IDisposable
    {
#pragma warning disable CA1819 // Properties should not return arrays

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

        #region Private mesh data member

        private float4[] _boneWeights;
        private float4[] _boneIndices;
        private float4[] _tangents;

        private float3[] _biTangents;
        private float3[] _vertices;
        private float3[] _normals;

        private float2[] _uvs;

        private ushort[] _triangles;
        private uint[] _colors;

        #endregion

        /// <summary>
        /// Gets and sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public float3[] Vertices
        {
            get => _vertices;
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
        public bool VerticesSet => _vertices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool TangentsSet => _tangents?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bi tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bi tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool BiTangentsSet => _biTangents?.Length > 0;


        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public uint[] Colors
        {
            get => _colors;
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
        public bool ColorsSet => _colors?.Length > 0;


        /// <summary>
        /// Gets and sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        public float3[] Normals
        {
            get => _normals;
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
        public bool NormalsSet => _normals?.Length > 0;

        /// <summary>
        /// Gets and sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        public float2[] UVs
        {
            get => _uvs;
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
        public bool UVsSet => _uvs?.Length > 0;

        /// <summary>
        /// Gets and sets the bone weights.
        /// </summary>
        /// <value>
        /// The bone weights.
        /// </value>
        public float4[] BoneWeights
        {
            get => _boneWeights;
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
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => _boneWeights?.Length > 0;

        /// <summary>
        /// Gets and sets the bone indices.
        /// </summary>
        /// <value>
        /// The bone indices.
        /// </value>
        public float4[] BoneIndices
        {
            get => _boneIndices;
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
        public bool BoneIndicesSet => _boneIndices?.Length > 0;

        /// <summary>
        /// Gets and sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        public ushort[] Triangles
        {
            get => _triangles;
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
        public bool TrianglesSet => _triangles?.Length > 0;

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
            get => _tangents;
            set
            {
                _tangents = value;

                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.Tangents));
            }
        }

        /// <summary>
        /// The bi tangent of each triangle for bump mapping.
        /// </summary>
        public float3[] BiTangents
        {
            get => _biTangents;
            set
            {
                _biTangents = value;

                MeshChanged?.Invoke(this, new MeshDataEventArgs(this, MeshChangedEnum.BiTangents));
            }
        }

        /// <summary>
        /// Used by various visitors such as rendering and picking. If set to true the mesh contributes to the traversal result. 
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// The type of mesh which is represented by this instance (e. g. triangle mesh, point, line, etc...)
        /// </summary>
        public int MeshType;


        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        /// <summary>
        /// Fire dispose mesh event
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
        /// Fire dispose mesh event
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);            
        }

        #endregion

#pragma warning restore CA1819 // Properties should not return arrays
    }
}
