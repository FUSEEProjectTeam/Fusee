using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.Blazor;

namespace Fusee.Engine.Imp.Blazor
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
        internal WebGLVertexArrayObject VertexArrayObject;
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
        public bool VerticesSet => VertexBufferObject != null;

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
        public bool NormalsSet => NormalBufferObject != null;

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
        public bool ColorsSet => ColorBufferObject != null;

        /// <summary>
        /// Gets a value indicating whether [u vs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [u vs set]; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet => UVBufferObject != null;

        /// <summary>
        /// Invalidates the UV's.
        /// </summary>
        public void InvalidateUVs()
        {
            UVBufferObject = null;
        }

        /// <summary>
        /// Gets a value indicating whether [bone indices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bone indices set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet => BoneIndexBufferObject != null;
        /// <summary>
        /// Returns whether the tangents have been set.
        /// </summary>
        public bool TangentsSet { get; }

        /// <summary>
        /// Returns whether be bi-tangents have been set.
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
        /// Gets a value indicating whether [bone weights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bone weights set]; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => BoneWeightBufferObject != null;

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
        /// Remove vertex array object from GPU
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void InvalidateVertexArrayObject()
        {
            VertexArrayObject = null;
        }

        public void InvalidateColors1()
        {
            throw new System.NotImplementedException();
        }

        public void InvalidateColors2()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [triangles set]; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet => ElementBufferObject != null;

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        /// <summary>
        /// Returns a vertex array object set
        /// </summary>
        public bool VertexArrayObjectSet => VertexArrayObject != null;

        public bool ColorsSet1 => throw new System.NotImplementedException();

        public bool ColorsSet2 => throw new System.NotImplementedException();

        #endregion
    }
}