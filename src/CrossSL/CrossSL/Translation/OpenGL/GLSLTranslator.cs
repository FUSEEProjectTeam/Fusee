using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace CrossSL
{
    internal class GLSLTranslator : ShaderTranslator
    {
        public GLSLTranslator()
        {
            ShaderMapping = new GLSLMapping();
            ShaderVisitor = new GLSLVisitor();
        }

        /// <summary>
        ///     Translates the a given method to the targeted shader language.
        /// </summary>
        /// <param name="target">The targeted shader language.</param>
        /// <param name="methodDef">The method definition.</param>
        /// <returns>A list of <see cref="FunctionDesc" />s for every translated method.</returns>
        internal override IEnumerable<FunctionDesc> Translate(ShaderTarget target, MethodDefinition methodDef)
        {
            methodDef.Name = methodDef.Name.Replace("VertexShader", "main");
            methodDef.Name = methodDef.Name.Replace("FragmentShader", "main");

            return base.Translate(target, methodDef);
        }

        /// <summary>
        ///     Adds the data type precision definition to the given shader.
        /// </summary>
        /// <param name="shaderType">Type of the shader.</param>
        protected override StringBuilder SetPrecision(SLShaderType shaderType)
        {
            // depending on target
            return new StringBuilder();
        }

        /// <summary>
        ///     Tests the given shaders by passing them to the GLSL/HLSL compiler.
        /// </summary>
        /// <param name="vertexShader">The vertex shader.</param>
        /// <param name="fragmentShader">The fragment shader.</param>
        internal override void PreCompile(StringBuilder vertexShader, StringBuilder fragmentShader)
        {
            if (GLSLCompiler.CanCheck(ShaderDesc.Target.Version))
            {
                if (ShaderDesc.Target.Envr == SLEnvironment.OpenGLES)
                    DebugLog.Warning("Shader will be tested on OpenGL but target is OpenGL ES");

                if (ShaderDesc.Target.Envr == SLEnvironment.OpenGLMix)
                    DebugLog.Warning("Shader will be tested on OpenGL but target is OpenGL and OpenGL ES");

                var vertTest = GLSLCompiler.CreateShader(vertexShader, SLShaderType.VertexShader);
                vertTest.Length = Math.Max(0, vertTest.Length - 3);
                vertTest = vertTest.Replace("0(", "        => 0(");

                var fragTest = GLSLCompiler.CreateShader(fragmentShader, SLShaderType.FragmentShader);
                fragTest.Length = Math.Max(0, fragTest.Length - 3);
                fragTest = fragTest.Replace("0(", "        => 0(");

                if (vertTest.Length > 0)
                    DebugLog.Error("OpenGL found problems while compiling vertex shader:\n" + vertTest);
                else if (fragTest.Length > 0)
                    DebugLog.Error("OpenGL found problems while compiling fragment shader:\n" + fragTest);
                else
                    Console.WriteLine("        => Test was successful. OpenGL did not find any problems.");
            }
            else
                DebugLog.Warning("Cannot test shader as your graphics card does not support the targeted version.");
        }
    }
}