// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    /// <summary>
    /// Implements the mesh.
    /// </summary>
    public class MeshImp : IMeshImp, IDisposable
    {
        /// <summary>
        /// Not implemented!
        /// </summary>
        [JSExternal]
        public MeshImp()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Not implemented!
        /// </summary>
        [JSExternal]
        public void InvalidateVertices()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns wether the vertices are set.
        /// </summary>
        [JSExternal]
        public bool VerticesSet { get; }
        /// <summary>
        /// Returns wether the tangents are set.
        /// </summary>
        [JSExternal]
        public bool TangentsSet { get; }
        /// <summary>
        /// Returns wether the bitangents are set.
        /// </summary>
        [JSExternal]
        public bool BiTangentsSet { get; }
        /// <summary>
        /// Throws exception if there are invalidate normals.
        /// </summary>
        [JSExternal]
        public void InvalidateNormals()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns if the normals are set.
        /// </summary>
        [JSExternal]
        public bool NormalsSet { get; }
        /// <summary>
        /// Throws exception if ther are invalidate colors.
        /// </summary>
        [JSExternal]
        public void InvalidateColors()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns wether the colors are set.
        /// </summary>
        [JSExternal]
        public bool ColorsSet { get; }
        /// <summary>
        /// Throws exception if there are invalidate triangles.
        /// </summary>
        [JSExternal]
        public void InvalidateTriangles()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns wether the triangles are set.
        /// </summary>
        [JSExternal]
        public bool TrianglesSet { get; }
        /// <summary>
        /// Returns wether the uv´s have been set.
        /// </summary>
        [JSExternal]
        public bool UVsSet { get; }
        /// <summary>
        /// Throws exception if there are invalidate uv´s.
        /// </summary>
        [JSExternal]
        public void InvalidateUVs()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Throws exception if there are invalidate bone weights.
        /// </summary>
        [JSExternal]
        public void InvalidateBoneWeights()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Throws exception if there are invalidate tangents.
        /// </summary>
        [JSExternal]
        public void InvalidateTangents()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Throws exception if ther are invalidate bitangents.
        /// </summary>
        [JSExternal]
        public void InvalidateBiTangents()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns wether the bone weights have been set.
        /// </summary>
        [JSExternal]
        public bool BoneWeightsSet { get; }
        /// <summary>
        /// Throws exception if there are invalidate bone indices.
        /// </summary>
        [JSExternal]
        public void InvalidateBoneIndices()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns wether the bone indices have been set.
        /// </summary>
        [JSExternal]
        public bool BoneIndicesSet { get; }

   
        public OpenGLPrimitiveType MeshType { get; set; }


        /// <summary>
        /// Not implemented!
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
