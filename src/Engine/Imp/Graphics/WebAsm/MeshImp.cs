using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.WebAsm
{
    /// <summary>
    /// Contains a handle for any type of attribute buffer stored on GPU memory such as vertices, normals, uvs etc.
    /// </summary>
    public class AttributeImp : IAttribImp
    {
        internal WebGLBuffer AttributeBufferObject;

    }

    /// <summary>
    /// This is the implementation of the <see cref="IMeshImp" /> interface. 
    /// It is used to check the status of the informations of a mesh and flush informations if required.
    /// </summary>
    public class MeshImp : IMeshImp
    {
        #region Internal Fields
        internal WebGLBuffer VertexBufferObject;
        internal WebGLBuffer NormalBufferObject;
        internal WebGLBuffer ColorBufferObject;
        internal WebGLBuffer UVBufferObject;
        internal WebGLBuffer BoneIndexBufferObject;
        internal WebGLBuffer BoneWeightBufferObject;
        internal WebGLBuffer ElementBufferObject;
        internal WebGLBuffer TangentBufferObject;
        internal WebGLBuffer BitangentBufferObject;
        internal int NElements;
        #endregion

        #region Public Fields & Members pairs
        /// <summary>
        /// Invalidates the vertices.
        /// </summary>
        public void InvalidateVertices()
        {
            VertexBufferObject = null;
        }
        /// <summary>
        /// Gets a value indicating whether [vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertices set]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get { return VertexBufferObject != null; } }

        /// <summary>
        /// Invalidates the normals.
        /// </summary>
        public void InvalidateNormals()
        {
            NormalBufferObject = null;
        }
        /// <summary>
        /// Gets a value indicating whether [normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [normals set]; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get { return NormalBufferObject != null; } }

        /// <summary>
        /// Implementation Tasks: Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateColors()
        {
            ColorBufferObject = null;
        }
        /// <summary>
        /// Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [colors set]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return ColorBufferObject != null; } }

        /// <summary>
        /// Gets a value indicating whether [u vs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [u vs set]; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return UVBufferObject != null; } }

        /// <summary>
        /// Invalidates the UV's.
        /// </summary>
        public void InvalidateUVs()
        {
            UVBufferObject = null;
        }

        /// <summary>
        /// Gets a value indicating whether [boneindices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneindices set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet { get { return BoneIndexBufferObject != null; } }
        /// <summary>
        /// Returns whether the tangents have been set.
        /// </summary>
        public bool TangentsSet { get; }
        /// <summary>
        /// Returns whether be bitangents have been set.
        /// </summary>
        public bool BiTangentsSet { get; }

        /// <summary>
        /// Invalidates the BoneIndices.
        /// </summary>
        public void InvalidateBoneIndices()
        {
            BoneIndexBufferObject = null;
        }

        /// <summary>
        /// Invalidates the Tangents.
        /// </summary>
        public void InvalidateTangents()
        {
            TangentBufferObject = null;
        }

        /// <summary>
        /// Invalidates the BiTangents.
        /// </summary>
        public void InvalidateBiTangents()
        {
            BitangentBufferObject = null;
        }

        /// <summary>
        /// Gets a value indicating whether [boneweights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [boneweights set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet { get { return BoneWeightBufferObject != null; } }

        /// <summary>
        /// Invalidates the BoneWeight's.
        /// </summary>
        public void InvalidateBoneWeights()
        {
            BoneWeightBufferObject = null;
        }
        /// <summary>
        /// Invalidates the triangles.
        /// </summary>
        public void InvalidateTriangles()
        {
            ElementBufferObject = null;
            NElements = 0;
        }
        /// <summary>
        /// Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [triangles set]; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get { return ElementBufferObject != null; } }

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public OpenGLPrimitiveType MeshType { get; set; }


        #endregion
    }
}
