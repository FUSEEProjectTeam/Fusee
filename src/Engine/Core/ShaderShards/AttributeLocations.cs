namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Declares the pre defined uniform parameter with the "fu" prefix, such as "fuVertex", "fuNormal" and so on.
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
        /// The normal attribute location index.
        /// </summary>
        public static readonly int NormalAttribLocation = 2;

        /// <summary>
        /// The uv attribute location index.
        /// </summary>
        public static readonly int UvAttribLocation = 3;

        /// <summary>
        /// The tangent attribute location index.
        /// </summary>
        public static readonly int TangentAttribLocation = 4;

        /// <summary>
        /// The bitangent attribute location index.
        /// </summary>
        public static readonly int BitangentAttribLocation = 5;

        /// <summary>
        /// The boneweight attribute location index.
        /// </summary>
        public static readonly int BoneWeightAttribLocation = 6;

        /// <summary>
        /// The boneindex attribute location index.
        /// </summary>
        public static readonly int BoneIndexAttribLocation = 7;
    }
}
