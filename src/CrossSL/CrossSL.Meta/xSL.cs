using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CrossSL.Meta
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable StaticFieldInGenericType
    // ReSharper disable FieldCanBeMadeReadOnly.Local

    public static class xSL<TShader> where TShader : xSLShader, new()
    {
        private static bool _translated;
        private static string _error;

        private static string _vertex;
        private static string _fragment;

        private static readonly TShader Instance;

        static xSL()
        {
            _translated = false;
            _error = String.Empty;

            _vertex = @"void main()
                {
                    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
                }";

            _fragment = @"
                #ifdef GL_ES
                    precision float highp;
                #endif

                void main()
                {
                    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
                }";

            Init();

            Instance = new TShader();
        }

        // to be modified by xSL
        private static void Init()
        {
            // dummy implementation
        }

        // to be modified by xSL
        private static void ShaderInfo()
        {
            if (!_translated)
                Debug.WriteLine("xSL: Shader '" + typeof (TShader).Name +
                                "' has not been translated.");

            if (_error.Length > 0)
                throw new TaskCanceledException(_error);
        }

        public static string VertexShader
        {
            get
            {
                ShaderInfo();
                return _vertex;
            }
        }

        public static string FragmentShader
        {
            get
            {
                ShaderInfo();
                return _fragment;
            }
        }

        public static TShader ShaderObject
        {
            get
            {
                ShaderInfo();
                return Instance;
            }
        }
    }

    // ReSharper restore FieldCanBeMadeReadOnly.Local
    // ReSharper restore StaticFieldInGenericType
    // ReSharper restore InconsistentNaming
}