namespace CrossSL
{
    // ReSharper disable InconsistentNaming

    internal enum SLShaderType
    {
        VertexShader,
        FragmentShader
    }

    internal enum SLEnvironment
    {
        OpenGL,
        OpenGLES,
        OpenGLMix
    }

    internal static class SLVersion
    {
        internal static string[][] VIDs =
        {
            new[]
            {
                "110",
                "120",
                "130",
                "140",
                "150",
                "330",
                "400",
                "420",
                "430",
                "440"
            },
            new[]
            {
                "100"
            },
            new[]
            {
                "110"
            }
        };
    }

    internal enum SLVariableType
    {
        xSLUnknown,
        xSLAttributeAttribute,
        xSLVaryingAttribute,
        xSLUniformAttribute,
        xSLConstAttribute
    }

    // ReSharper restore InconsistentNaming
}