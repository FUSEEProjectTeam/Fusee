﻿namespace Fusee.Engine.Common
{
    /// <summary>
    /// Declares buffer locations. OpenGL specific error: GL_INVALID_VALUE is generated if index is greater than or equal to GL_MAX_VERTEX_ATTRIBS.
    /// GL_MAX_VERTEX_ATTRIBS seems to be 16.
    /// </summary>
    public static class AttributeLocations
    {
        /// <summary>
        /// The vertex attribute location index.
        /// </summary>
        public static readonly int VertexAttribLocation = 0;

        /// <summary>
        /// The binding index for the VAO.
        /// </summary>
        public static readonly int VertexAttribBindingIndex = VertexAttribLocation;

        /// <summary>
        /// The color attribute location index.
        /// </summary>
        public static readonly int ColorAttribLocation = 1;

        /// <summary>
        /// The binding index for the Color buffer.
        /// </summary>
        public static readonly int ColorAttribBindingIndex = ColorAttribLocation;

        /// <summary>
        /// The color attribute location index.
        /// </summary>
        public static readonly int Color1AttribLocation = 2;

        /// <summary>
        /// The binding index for the second Color buffer.
        /// </summary>
        public static readonly int Color1AttribBindingIndex = Color1AttribLocation;

        /// <summary>
        /// The color attribute location index.
        /// </summary>
        public static readonly int Color2AttribLocation = 3;

        /// <summary>
        /// The binding index for the third Color buffer.
        /// </summary>
        public static readonly int Color2AttribBindingIndex = Color2AttribLocation;

        /// <summary>
        /// The normal attribute location index.
        /// </summary>
        public static readonly int NormalAttribLocation = 4;

        /// <summary>
        /// The binding index for the Normal buffer.
        /// </summary>
        public static readonly int NormalAttribBindingIndex = NormalAttribLocation;

        /// <summary>
        /// The uv attribute location index.
        /// </summary>
        public static readonly int UvAttribLocation = 5;

        /// <summary>
        /// The binding index for the UV buffer.
        /// </summary>
        public static readonly int UvAttribBindingIndex = UvAttribLocation;

        /// <summary>
        /// The tangent attribute location index.
        /// </summary>
        public static readonly int TangentAttribLocation = 6;

        /// <summary>
        /// The binding index for the Tangent buffer.
        /// </summary>
        public static readonly int TangentAttribBindingIndex = TangentAttribLocation;

        /// <summary>
        /// The bitangent attribute location index.
        /// </summary>
        public static readonly int BitangentAttribLocation = 7;

        /// <summary>
        /// The binding index for the Bitangent buffer.
        /// </summary>
        public static readonly int BitangentAttribBindingIndex = BitangentAttribLocation;

        /// <summary>
        /// The bone weight attribute location index.
        /// </summary>
        public static readonly int BoneWeightAttribLocation = 8;

        /// <summary>
        /// The binding index for the Bone Weight buffer.
        /// </summary>
        public static readonly int BoneWeightAttribBindingIndex = BoneWeightAttribLocation;

        /// <summary>
        /// The bone index attribute location index.
        /// </summary>
        public static readonly int BoneIndexAttribLocation = 9;

        /// <summary>
        /// The binding index for the Bone Index buffer.
        /// </summary>
        public static readonly int BoneIndexAttribAttribBindingIndex = BoneIndexAttribLocation;

        /// <summary>
        /// First attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat1 = 10;

        /// <summary>
        /// The binding index for the instance model matrix buffer.
        /// </summary>
        public static readonly int InstancedModelMatBindingIndex = InstancedModelMat1;

        /// <summary>
        /// Second attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat2 = 11;

        /// <summary>
        /// Third attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat3 = 12;

        /// <summary>
        /// Fourth attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat4 = 13;

        /// <summary>
        /// Attribute location for the instanced color.
        /// </summary>
        public static readonly int InstancedColor = 14;

        /// <summary>
        /// The binding index for the instanced color buffer.
        /// </summary>
        public static readonly int InstancedColorBindingIndex = InstancedColor;

        /// <summary>
        /// Attribute location for the flags buffer.
        /// </summary>
        public static readonly int FlagsAttribLocation = 15;

        /// <summary>
        /// The binding index for the flags buffer.
        /// </summary>
        public static readonly int FlagsBindingIndex = FlagsAttribLocation;
    }
}