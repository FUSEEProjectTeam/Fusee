using System;

namespace CrossSL.Meta
{
    // ReSharper disable InconsistentNaming

    public abstract partial class xSLShader
    {
        protected internal static class xSLTarget
        {
            public enum GLSL
            {
                V110,
                V120,
                V130,
                V140,
                V150,
                V330,
                V400,
                V420,
                V430,
                V440,
            }

            public enum GLSLES
            {
                V100
            }

            public enum GLSLMix
            {
                V110
            }
        }

        [Flags]
        protected internal enum xSLDebug
        {
            None = 0,
            IgnoreShader = 1,
            PreCompile = 2,
            SaveToFile = 4,
            ThrowException = 8
        }

        protected internal enum xSLPrecision
        {
            Low,
            Medium,
            High
        }
    }

    // ReSharper restore InconsistentNaming
}