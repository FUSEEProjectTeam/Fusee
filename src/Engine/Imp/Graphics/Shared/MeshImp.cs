﻿using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID

namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// This is the implementation of the <see cref="IMeshImp" /> interface.
    /// It is used to check the status of the informations of a mesh and flush informations if required.
    /// </summary>
    internal class MeshImp : IMeshImp
    {
        #region Internal Fields

        public int VertexArrayObject { get; set; }
        public int VertexBufferObject { get; set; }
        public int NormalBufferObject { get; set; }
        public int ColorBufferObject { get; set; }
        public int ColorBufferObject1 { get; set; }
        public int ColorBufferObject2 { get; set; }
        public int UVBufferObject { get; set; }
        public int BoneIndexBufferObject { get; set; }
        public int BoneWeightBufferObject { get; set; }
        public int ElementBufferObject { get; set; }
        public int TangentBufferObject { get; set; }
        public int BitangentBufferObject { get; set; }
        public int FlagsBufferObject { get; set; }
        public int NElements { get; set; }

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
        /// Invalidates the flags, e.g. reset the FlagsBufferObject of this instance by setting it to 0.
        /// </summary>
        public void InvalidateFlags()
        {
            FlagsBufferObject = 0;
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
        /// Returns whether the tangents have been set.
        /// </summary>
        public bool TangentsSet { get { return TangentBufferObject != 0; } }

        /// <summary>
        /// Returns whether the bitangents have been set.
        /// </summary>
        public bool BiTangentsSet { get { return BitangentBufferObject != 0; } }

        /// <summary>
        /// Returns whether the flags have been set.
        /// </summary>
        public bool FlagsSet { get { return FlagsBufferObject != 0; } }


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
        public PrimitiveType MeshType { get; set; }

        #endregion Public Fields & Members pairs
    }
}