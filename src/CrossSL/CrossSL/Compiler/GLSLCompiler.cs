using System;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CrossSL
{
    internal static class GLSLCompiler
    {
        private static GameWindow _window;

        private struct Version
        {
            internal int Major;
            internal int Minor;
        }

        private static Version MapVersion(int target)
        {
            switch (target)
            {
                case 100:
                case 110:
                    return new Version {Major = 2, Minor = 0};
                case 120:
                    return new Version {Major = 2, Minor = 1};
                case 130:
                    return new Version {Major = 3, Minor = 0};
                case 140:
                    return new Version {Major = 3, Minor = 1};
                case 150:
                    return new Version {Major = 3, Minor = 2};

                default:
                    var major = target/100;
                    var minor = target - (major*100);

                    return new Version {Major = major, Minor = minor};
            }
        }

        internal static bool CanCheck(int target)
        {
            var mapped = MapVersion(target);

            if (_window != null)
                _window.Dispose();

            // dummy window
            _window = new GameWindow(1, 1, GraphicsMode.Default, "CrossSL", GameWindowFlags.Default,
                DisplayDevice.Default, mapped.Major, mapped.Minor, GraphicsContextFlags.Default);

            var gcard = GL.GetString(StringName.ShadingLanguageVersion);
            if (gcard.Length > 4)
                gcard = gcard.Remove(4);

            var supV = Int32.Parse(gcard.Replace(".", String.Empty));

            return supV >= target;
        }

        internal static StringBuilder CreateShader(StringBuilder shader, SLShaderType type)
        {
            var result = new StringBuilder();

            var shaderObj = type == SLShaderType.VertexShader
                ? GL.CreateShader(ShaderType.VertexShader)
                : GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(shaderObj, shader.ToString());
            GL.CompileShader(shaderObj);

            string info;
            GL.GetShaderInfoLog(shaderObj, out info);

            if (info.Length > 0 && info != "No errors.\n")
                result.Append(info).NewLine();

            return result;
        }
    }
}