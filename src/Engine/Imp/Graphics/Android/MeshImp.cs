using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// This is the implementation of the <see cref="IMeshImp" /> interface.
    /// It is used to check the status of the informations of a mesh and flush informations if required.
    /// </summary>
    public class MeshImp : IMeshImp
    {
        #region Internal Fields

        internal int VertexArrayObject;
        internal int VertexBufferObject;
        internal int NormalBufferObject;
        internal int ColorBufferObject;
        internal int ColorBufferObject1;
        internal int ColorBufferObject2;
        internal int UVBufferObject;
        internal int BoneIndexBufferObject;
        internal int BoneWeightBufferObject;
        internal int ElementBufferObject;
        internal int TangentBufferObject;
        internal int BitangentBufferObject;
        internal int NElements;

        #endregion Internal Fields

        #region Public Fields & Members pairs

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
        public bool VertexArrayObjectSet { get { return VertexArrayObject != 0; } }

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
        public bool VerticesSet { get { return VertexBufferObject != 0; } }

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
        public bool NormalsSet { get { return NormalBufferObject != 0; } }

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
        public bool ColorsSet { get { return ColorBufferObject != 0; } }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 { get { return ColorBufferObject1 != 0; } }

        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 { get { return ColorBufferObject2 != 0; } }

        /// <summary>
        /// Gets a value indicating whether [u vs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [u vs set]; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return UVBufferObject != 0; } }

        /// <summary>
        /// Invalidates the UV's.
        /// </summary>
        public void InvalidateUVs()
        {
            UVBufferObject = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [boneindices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneindices set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return BoneIndexBufferObject != 0; } }
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
        /// Gets a value indicating whether [boneweights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneweights set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return BoneWeightBufferObject != 0; } }

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
        public bool TrianglesSet { get { return ElementBufferObject != 0; } }

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public OpenGLPrimitiveType MeshType { get; set; }

        #endregion Public Fields & Members pairs
    }
}