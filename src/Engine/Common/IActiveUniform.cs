namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for handling uniforms that are determined "active" after the shader is compiled.
    /// </summary>
    public interface IActiveUniform
    {
        /// <summary>
        /// The Handle to be used when setting or getting the parameter value from the shader.
        /// </summary>
        public IUniformHandle Handle
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the name of the shader parameter.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the number of items stored under this parameter name. Only differs from 1 if the parameter is a uniform array.
        /// If Size > 1 this value represents the number of array entries.
        /// </summary>
        /// <remarks>
        /// See also <a href="https://www.khronos.org/opengles/sdk/docs/man/xhtml/glGetActiveUniform.xml">the Khronos group's documentation on glGetActiveUniform</a>.
        /// </remarks>
        public int Size { get; set; }

        /// <summary>
        /// The hash code of this ShaderParamInfo.
        /// </summary>
        public int Hash { get; }

        /// <summary>
        /// Method that nows how to get this uniforms value.
        /// </summary>
        public GetUniformValue UniformValueGetter { get; set; }

        /// <summary>
        /// Determines if this is a global uniform.
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// If the value has changed the shader uniform must be changed too.
        /// </summary>
        public bool HasValueChanged { get; set; }
    }
}