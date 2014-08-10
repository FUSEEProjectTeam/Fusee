using System;

namespace CrossSL.Meta
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedParameter.Local
    // ReSharper disable UnusedMember.Local

    public abstract partial class xSLShader
    {
        [AttributeUsage(AttributeTargets.Class)]
        protected internal sealed class xSLTargetAttribute : Attribute
        {
            public xSLTargetAttribute(xSLTarget.GLSL version)
            {
                // dummy constructor
            }

            public xSLTargetAttribute(xSLTarget.GLSLES version)
            {
                // dummy constructor
            }

            public xSLTargetAttribute(xSLTarget.GLSLMix version)
            {
                // dummy constructor
            }
        }

        [AttributeUsage(AttributeTargets.Class)]
        protected internal sealed class xSLDebugAttribute : Attribute
        {
            public xSLDebugAttribute(xSLDebug setting)
            {
                // dummy constructor
            }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
            AllowMultiple = false)]
        protected sealed class xSLAttributeAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
            AllowMultiple = false)]
        protected sealed class xSLVaryingAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
            AllowMultiple = false)]
        protected sealed class xSLUniformAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
            AllowMultiple = false)]
        protected sealed class xSLConstAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Method)]
        protected internal sealed class xSLPrecisionAttribute : Attribute
        {
            public xSLPrecision floatPrecision { get; set; }
            public xSLPrecision intPrecision { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
        internal sealed class VertexShaderAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
        internal sealed class FragmentShaderAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Property)]
        internal sealed class MandatoryAttribute : Attribute
        {
            // dummy implementation
        }

        [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Method)]
        internal sealed class MappingAttribute : Attribute
        {
            public MappingAttribute(string GLSL)
            {
                // dummy implementation            
            }
        }
    }

    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
    // ReSharper restore InconsistentNaming
}