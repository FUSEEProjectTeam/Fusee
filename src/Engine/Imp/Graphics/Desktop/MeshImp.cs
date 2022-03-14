using Fusee.Engine.Common;
using OpenTK.Graphics;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// This is the implementation of the <see cref="IMeshImp" /> interface.
    /// It is used to check the status of the informations of a mesh and flush informations if required.
    /// </summary>
    public class MeshImp : IMeshImp
    {
        #region Internal Fields

        internal VertexArrayHandle VertexArrayObject;
        internal BufferHandle VertexBufferObject;
        internal BufferHandle NormalBufferObject;
        internal BufferHandle ColorBufferObject;
        internal BufferHandle ColorBufferObject1;
        internal BufferHandle ColorBufferObject2;
        internal BufferHandle UVBufferObject;
        internal BufferHandle BoneIndexBufferObject;
        internal BufferHandle BoneWeightBufferObject;
        internal BufferHandle ElementBufferObject;
        internal BufferHandle TangentBufferObject;
        internal BufferHandle BitangentBufferObject;
        internal int NElements;

        #endregion Internal Fields

        #region Public Fields & Members pairs

        /// <summary>
        /// Invalidates the VertexArrayObject.
        /// </summary>
        public void InvalidateVertexArrayObject()
        {
            VertexArrayObject.Handle = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [VertexArrayObject set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [VertexArrayObject set]; otherwise, <c>false</c>.
        /// </value>
        public bool VertexArrayObjectSet { get { return VertexArrayObject.Handle != 0; } }

        /// <summary>
        /// Invalidates the vertices.
        /// </summary>
        public void InvalidateVertices()
        {
            VertexBufferObject.Handle = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertices set]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get { return VertexBufferObject.Handle != 0; } }

        /// <summary>
        /// Invalidates the normals.
        /// </summary>
        public void InvalidateNormals()
        {
            NormalBufferObject.Handle = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [normals set]; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get { return NormalBufferObject.Handle != 0; } }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors()
        {
            ColorBufferObject.Handle = 0;
        }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors1()
        {
            ColorBufferObject1.Handle = 0;
        }

        /// <summary>
        /// Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors2()
        {
            ColorBufferObject2.Handle = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return ColorBufferObject.Handle != 0; } }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 { get { return ColorBufferObject1.Handle != 0; } }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 { get { return ColorBufferObject2.Handle != 0; } }

        /// <summary>
        /// Gets a value indicating whether [u vs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [u vs set]; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return UVBufferObject.Handle != 0; } }

        /// <summary>
        /// Invalidates the UV's.
        /// </summary>
        public void InvalidateUVs()
        {
            UVBufferObject.Handle = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [boneindices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneindices set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return BoneIndexBufferObject.Handle != 0; } }
        /// <summary>
        /// Returns wether the tangents have been set.
        /// </summary>
        public bool TangentsSet { get; }
        /// <summary>
        /// Returns wether be bitangents have been set.
        /// </summary>
        public bool BiTangentsSet { get; }

        /// <summary>
        /// Invalidates the BoneIndices.
        /// </summary>
        public void InvalidateBoneIndices()
        {
            BoneIndexBufferObject.Handle = 0;
        }

        /// <summary>
        /// Invalidates the Tangents.
        /// </summary>
        public void InvalidateTangents()
        {
            TangentBufferObject.Handle = 0;
        }

        /// <summary>
        /// Invalidates the BiTangents.
        /// </summary>
        public void InvalidateBiTangents()
        {
            BitangentBufferObject.Handle = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [boneweights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneweights set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return BoneWeightBufferObject.Handle != 0; } }

        /// <summary>
        /// Invalidates the BoneWeight's.
        /// </summary>
        public void InvalidateBoneWeights()
        {
            BoneWeightBufferObject.Handle = 0;
        }
        /// <summary>
        /// Invalidates the triangles.
        /// </summary>
        public void InvalidateTriangles()
        {
            ElementBufferObject.Handle = 0;
            NElements = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [triangles set]; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get { return ElementBufferObject.Handle != 0; } }

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        #endregion Public Fields & Members pairs
    }
}