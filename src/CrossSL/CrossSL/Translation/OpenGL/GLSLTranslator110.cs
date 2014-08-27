using System.Linq;
using System.Text;

namespace CrossSL
{
    internal class GLSLTranslator110 : GLSLTranslator
    {
        public GLSLTranslator110()
        {
            ShaderMapping = new GLSLMapping110();
            ShaderVisitor = new GLSLVisitor110();
        }

        /// <summary>
        ///     Checks member variables before translating the shader.
        /// </summary>
        /// <remarks>
        ///     GLSL 1.1 does not support data type 'double'.
        ///     GLSL 1.1 does not support const arrays.
        /// </remarks>
        protected internal override void PreVariableCheck()
        {
            base.PreVariableCheck();

            // no support for type 'double'
            var doubleVars = ShaderDesc.Variables.Where(var => var.DataType == typeof (double));
            foreach (var doubleVar in doubleVars)
            {
                DebugLog.Warning("'" + doubleVar.Definition.Name + "' is of type 'double' which" +
                                 " is not supported in GLSL 1.1. Type will be changed to 'float'");
            }

            // no support for const arrays
            var arrays = ShaderDesc.Variables.Where(var => var.IsArray);
            var constArrays = arrays.Where(var => var.Attribute == SLVariableType.xSLConstAttribute);

            foreach (var constArray in constArrays)
                DebugLog.Error("'" + constArray.Definition.Name + "' is a const array. This" +
                               " is not supported in GLSL 1.1");
        }

        /// <summary>
        ///     Adds the data type precision definition to the given shader.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        protected override StringBuilder SetPrecision(SLShaderType shaderType)
        {
            if (ShaderDesc.Precision[(int) shaderType] != null)
                DebugLog.Warning("Using [xSLPrecision] is not supported in GLSL 1.1. Ignored");

            return new StringBuilder();
        }
    }
}