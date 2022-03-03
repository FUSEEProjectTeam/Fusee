namespace Fusee.Engine.Common
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
        /// The color attribute location index.
        /// </summary>
        public static readonly int ColorAttribLocation = 1;

        /// <summary>
        /// The color attribute location index.
        /// </summary>
        public static readonly int Color1AttribLocation = 2;

        /// <summary>
        /// The color attribute location index.
        /// </summary>
        public static readonly int Color2AttribLocation = 3;

        /// <summary>
        /// The normal attribute location index.
        /// </summary>
        public static readonly int NormalAttribLocation = 4;

        /// <summary>
        /// The uv attribute location index.
        /// </summary>
        public static readonly int UvAttribLocation = 5;

        /// <summary>
        /// The tangent attribute location index.
        /// </summary>
        public static readonly int TangentAttribLocation = 6;

        /// <summary>
        /// The bitangent attribute location index.
        /// </summary>
        public static readonly int BitangentAttribLocation = 7;

        /// <summary>
        /// The bone weight attribute location index.
        /// </summary>
        public static readonly int BoneWeightAttribLocation = 8;

        /// <summary>
        /// The bone index attribute location index.
        /// </summary>
        public static readonly int BoneIndexAttribLocation = 9;

        /// <summary>
        /// The Fusee platform id attribute location index.
        /// </summary>
        public static readonly int FuseePlatformIdLocation = 10;

        /// <summary>
        /// First attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat1 = 11;

        /// <summary>
        /// Second attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat2 = 12;

        /// <summary>
        /// Third attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat3 = 13;

        /// <summary>
        /// Fourth attribute location for the instanced model matrix.
        /// Vertex Attributes can be of type vec4 at maximum.
        /// </summary>
        public static readonly int InstancedModelMat4 = 14;

        /// <summary>
        /// Attribute location for the instanced color.
        /// </summary>
        public static readonly int InstancedColor = 15;
    }
}