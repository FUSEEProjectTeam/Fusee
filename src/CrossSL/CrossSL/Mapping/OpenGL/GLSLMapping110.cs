namespace CrossSL
{
    internal sealed class GLSLMapping110 : GLSLMapping
    {
        /// <summary>
        ///     Initializes the <see cref="GLSLMapping110" /> class.
        /// </summary>
        /// <remarks>
        ///     GLSL 1.1 does not support type 'double'.
        /// </remarks>
        public GLSLMapping110()
        {
            Types[typeof (double)] = "float";
        }
    }
}