using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IManagedMesh, IDisposable
    {
        #region RenderContext Asset Management

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> MeshChanged;

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

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
        private uint[] _colors1;
        private uint[] _colors2;

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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
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
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public uint[] Colors1
        {
            get => _colors1;
            set
            {
                _colors1 = value;
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
            }
        }

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 => _colors1?.Length > 0;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public uint[] Colors2
        {
            get => _colors2;
            set
            {
                _colors2 = value;
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
            }
        }

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 => _colors2?.Length > 0;


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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));
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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));
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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));
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
                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
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

                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
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
        /// The tangent of each triangle for normal mapping.
        /// w-component is handedness
        /// </summary>
        public float4[] Tangents
        {
            get => _tangents;
            set
            {
                _tangents = value;

                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents));
            }
        }

        /// <summary>
        /// The bi tangent of each triangle for normal mapping.
        /// </summary>
        public float3[] BiTangents
        {
            get => _biTangents;
            set
            {
                _biTangents = value;

                MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
            }
        }

        /// <summary>
        /// The type of mesh which is represented by this instance (e. g. triangle mesh, point, line, etc...)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        #region IDisposable Support

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~Mesh()
        {
            Dispose(false);
        }

        #endregion

    }
}