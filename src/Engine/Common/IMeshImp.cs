﻿using ProtoBuf;
namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for implementing handles on attribute arrays stored on GPU memory such as vertices, normals, uvs etc.
    /// This interface does not require any particular public implementation. It is meant as a markup-Interface to identify
    /// types.
    /// </summary>
    public interface IAttribImp
    {

    }

    /// <summary>
    /// Interface for Mesh implementations. The implementation should handle typical mesh informations like: vertices, triangles, normals, colors, UV's.
    /// It is also required to implement a connection to the current RenderContext in order to apply the Mesh for rendering.
    /// The Mesh should preferable use handles for its informations in order to communicate with a RenderContext. The handles are referring to so called BufferObjects.
    /// </summary>

    [ProtoContract]
    public interface IMeshImp
    {
        /// <summary>
        /// Implementation Task: Invalidates the vertices of the mesh, e.g. reset the VertexBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateVertices();

        /// <summary>
        /// Implementation Tasks: Get a value indicating whether [vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if VertexBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool VerticesSet { get; }

        /// <summary>
        /// Implementation Task: Invalidates the second vertices of the mesh, e.g. reset the Vertex1BufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateVertices1();

        /// <summary>
        /// Implementation Tasks: Get a value indicating whether [second vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if Vertex1BufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool Vertices1Set { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the normals of the mesh, e.g. reset the NormalBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateNormals();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if NormalBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool NormalsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the second normals of the mesh, e.g. reset the Normal1BufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateNormals1();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [second normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if Normal1BufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool Normals1Set { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateColors();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if ColorBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool ColorsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the triangles, e.g. reset the ElementBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateTriangles();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if ElementBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool TrianglesSet { get; }

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [UVs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool UVsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the UV's, e.g. reset the UVBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateUVs();

        /// <summary>
        /// Implementation Tasks: Invalidates the bone weights of the mesh, e.g. reset the BoneWeightsBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateBoneWeights();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [boneweights set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if BoneWeightsBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool BoneWeightsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the bone indices of the mesh, e.g. reset the BoneIndicesBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateBoneIndices();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [boneindices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if BoneIndicesBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool BoneIndicesSet { get; }
        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [Tangents Set].
        /// </summary>
        bool TangentsSet { get; }
        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [BiTangents Set].
        /// </summary>
        bool BiTangentsSet { get; }

        /// <summary>
        ///     Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        OpenGLPrimitiveType MeshType { get; set; }
    }
}