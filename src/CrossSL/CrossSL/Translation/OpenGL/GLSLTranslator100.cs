using System.Linq;
using System.Text;
using CrossSL.Meta;

namespace CrossSL
{
    internal sealed class GLSLTranslator100 : GLSLTranslator
    {
        internal GLSLTranslator100()
        {
            ShaderMapping = new GLSLMapping110();
            ShaderVisitor = new GLSLVisitor100();
        }

        /// <summary>
        ///     Adds the data type precision definition to the given shader.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        protected override StringBuilder SetPrecision(SLShaderType shaderType)
        {
            var result = new StringBuilder();
            var defaultPrec = true;

            if (ShaderDesc.Precision[(int) shaderType] != null)
            {
                var precAttr = ShaderDesc.Precision[(int) shaderType];

                var floatPrec = precAttr.Properties.FirstOrDefault(prop => prop.Name == "floatPrecision");
                var intPrec = precAttr.Properties.FirstOrDefault(prop => prop.Name == "intPrecision");

                if (floatPrec.Name != null)
                {
                    var floatPrecVal = ((xSLShader.xSLPrecision) floatPrec.Argument.Value).ToString();
                    result.Append("precision ").Append(floatPrecVal.ToLower()).Append("p");
                    result.Append(" float;").NewLine();

                    defaultPrec = false;
                }

                if (intPrec.Name != null)
                {
                    var intPrecVal = ((xSLShader.xSLPrecision) intPrec.Argument.Value).ToString();
                    result.Append("precision ").Append(intPrecVal.ToLower()).Append("p");
                    result.Append(" int;").NewLine();
                }

                if (result.Length > 0)
                    result.Replace("medium", "med").NewLine();
            }

            if (!defaultPrec || shaderType != SLShaderType.FragmentShader)
                return result;

            // default precision
            DebugLog.Warning("Target GLSLES requires [xSLPrecision] at 'FragmentShader()' to set " +
                             "the precision of data type 'float'. Using high precision as default");

            return result.Append("precision highp float;").NewLine(2);
        }
    }
}