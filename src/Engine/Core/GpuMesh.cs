using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Delegate that points to a method that is able to create a <see cref="GpuMesh"/>
    /// </summary>
    /// <param name="primitiveType">See <see cref="PrimitiveType"/>.</param>
    /// <param name="vertices">The vertices of the mesh.</param>
    /// <param name="triangles">The triangles of the mesh.</param>
    /// <param name="normals">The normals of the mesh.</param>
    /// <param name="colors">The colors of the mesh.</param>
    /// <param name="colors1">The second color field of the mesh.</param>
    /// <param name="colors2">The third color field of the mesh.</param>
    /// <param name="uvs">The UV coordinates of the mesh.</param>
    /// <param name="tangents">The tangents of the mesh.</param>
    /// <param name="bitangents">The bitangents of the mesh.</param>
    /// <param name="boneIndices">The boneIndices of the mesh.</param>
    /// <param name="boneWeights">The vertiboneWeightsces of the mesh.</param>
    /// <returns></returns>
    public delegate GpuMesh CreateGpuMesh(PrimitiveType primitiveType, float3[] vertices, ushort[] triangles = null,
            float3[] normals = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null, float2[] uvs = null,
            float4[] tangents = null, float3[] bitangents = null, float4[] boneIndices = null, float4[] boneWeights = null);

    /// <summary>
    /// This type of mesh doesn't create a copy of the mesh data in the RAM.
    /// </summary>
    public class GpuMesh : SceneComponent, IManagedMesh, IMeshImp, IDisposable
    {
        /// <summary>
        /// MeshChanged event notifies observing MeshManager the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        #region Internal Fields

        public int VertexArrayObject
        {
            get { return _vertexArrayObject; }
            set
            {
                VertexArrayObjectSet = true;
                _vertexArrayObject = value;
            }
        }
        private int _vertexArrayObject;

        public int VertexBufferObject
        {
            get { return _vertexBufferObject; }
            set
            {
                VerticesSet = true;
                _vertexBufferObject = value;
            }
        }
        private int _vertexBufferObject;

        public int NormalBufferObject
        {
            get { return _normalBufferObject; }
            set
            {
                NormalsSet = true;
                _normalBufferObject = value;
            }
        }
        private int _normalBufferObject;

        public int ColorBufferObject
        {
            get { return _colorBufferObject; }
            set
            {
                ColorsSet = true;
                _colorBufferObject = value;
            }
        }
        private int _colorBufferObject;

        public int ColorBufferObject1
        {
            get { return _colorBufferObject1; }
            set
            {
                ColorsSet1 = true;
                _colorBufferObject1 = value;
            }
        }
        private int _colorBufferObject1;

        public int ColorBufferObject2
        {
            get { return _colorBufferObject2; }
            set
            {
                ColorsSet2 = true;
                _colorBufferObject2 = value;
            }
        }
        private int _colorBufferObject2;

        public int UVBufferObject
        {
            get { return _uvBufferObject; }
            set
            {
                UVsSet = true;
                _uvBufferObject = value;
            }
        }
        private int _uvBufferObject;

        public int BoneIndexBufferObject
        {
            get { return _boneIndexBufferObject; }
            set
            {
                BoneIndicesSet = true;
                _boneIndexBufferObject = value;
            }
        }
        private int _boneIndexBufferObject;

        public int BoneWeightBufferObject
        {
            get { return _boneWeightBufferObject; }
            set
            {
                BoneWeightsSet = true;
                _boneWeightBufferObject = value;
            }
        }
        private int _boneWeightBufferObject;

        public int TangentBufferObject
        {
            get { return _tangentBufferObject; }
            set
            {
                TangentsSet = true;
                _tangentBufferObject = value;
            }
        }
        private int _tangentBufferObject;

        public int BitangentBufferObject
        {
            get { return _bitangentBufferObject; }
            set
            {
                BiTangentsSet = true;
                _bitangentBufferObject = value;
            }
        }
        private int _bitangentBufferObject;

        public int ElementBufferObject { get; set; }
        public int NElements { get; set; }

        #endregion Internal Fields

        #region Public Fields & Members pairs

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox;

        /// <summary>
        /// Invalidates the VertexArrayObject.
        /// </summary>
        public void InvalidateVertexArrayObject()
        {
            VertexArrayObject = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [VertexArrayObject set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [VertexArrayObject set]; otherwise, <c>false</c>.
        /// </value>
        public bool VertexArrayObjectSet { get; private set; }

        /// <summary>
        /// Invalidates the vertices.
        /// </summary>
        public void InvalidateVertices()
        {
            VertexBufferObject = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertices set]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get; private set; }

        /// <summary>
        /// Invalidates the normals.
        /// </summary>
        public void InvalidateNormals()
        {
            NormalBufferObject = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [normals set]; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get; private set; }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors()
        {
            ColorBufferObject = 0;
        }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors1()
        {
            ColorBufferObject1 = 0;
        }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors2()
        {
            ColorBufferObject2 = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [u vs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [u vs set]; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get; private set; }

        /// <summary>
        /// Invalidates the UV's.
        /// </summary>
        public void InvalidateUVs()
        {
            UVBufferObject = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [boneIndices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneIndices set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get; private set; }
        /// <summary>
        /// Returns whether the tangents have been set.
        /// </summary>
        public bool TangentsSet { get; private set; }
        /// <summary>
        /// Returns whether be bitangents have been set.
        /// </summary>
        public bool BiTangentsSet { get; private set; }

        /// <summary>
        /// Invalidates the BoneIndices.
        /// </summary>
        public void InvalidateBoneIndices()
        {
            BoneIndexBufferObject = 0;
        }

        /// <summary>
        /// Invalidates the Tangents.
        /// </summary>
        public void InvalidateTangents()
        {
            TangentBufferObject = 0;
        }

        /// <summary>
        /// Invalidates the BiTangents.
        /// </summary>
        public void InvalidateBiTangents()
        {
            BitangentBufferObject = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [boneWeights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneWeights set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get; private set; }

        /// <summary>
        /// Invalidates the BoneWeight's.
        /// </summary>
        public void InvalidateBoneWeights()
        {
            BoneWeightBufferObject = 0;
        }
        /// <summary>
        /// Invalidates the triangles.
        /// </summary>
        public void InvalidateTriangles()
        {
            ElementBufferObject = 0;
            NElements = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [triangles set]; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get; private set; }

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        #endregion Public Fields & Members pairs

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
        ~GpuMesh()
        {
            Dispose(false);
        }

    }
}
