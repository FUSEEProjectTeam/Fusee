using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    // ReSharper disable once InconsistentNaming
    internal static class GLSL
    {
        internal enum Type
        {
            Mat3,
            Mat4,
            Vec2,
            Vec3,
            Vec4,
            Boolean,
            Float,
            Int,
            Sampler2D,
            SamplerCube,
            ArrayTexture,
            Sampler2DShadow,
            SamplerCubeShadow,
            ArrayTextureShadow,
            Void
        }

        internal static string CreateUniform(Type type, string varName)
        {
            return $"uniform {DecodeType(type)} {varName};";
        }

        internal static string CreateOut(Type type, string varName)
        {
            return $"out {DecodeType(type)} {varName};\n";
        }

        internal static string CreateIn(Type type, string varName)
        {
            return $"in  {DecodeType(type)} {varName};\n";
        }

        internal static string CreateVar(Type type, string varName)
        {
            return $"{DecodeType(type)} {varName}";
        }

        /// <summary>
        /// Creates a GLSL method
        /// </summary>
        /// <param name="returnType">The (GLSL) return type of the method.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="methodParams">All method parameters. Use <see cref="CreateVar(Type, string)"/> to get the correct string.</param>
        /// <param name="methodBody">The method body goes here.</param>
        internal static string CreateMethod(Type returnType, string methodName, string[] methodParams, IList<string> methodBody)
        {
            var tmpList = new List<string>
            {
                $"{DecodeType(returnType)} {methodName}({string.Join(", ", methodParams)})",
                "{"
            };
            tmpList.AddRange(methodBody);
            tmpList.Add("}");
            tmpList.Add("\n");
            AddTabsToMethods(tmpList);

            return string.Join("\n", tmpList);
        }

        /// <summary>
        /// Creates a GLSL method
        /// </summary>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="methodParams">All method parameters. Use <see cref="CreateVar(Type, string)"/> to get the correct string.</param>
        /// <param name="methodBody">The method body goes here.</param>
        internal static string CreateMethod(string returnType, string methodName, string[] methodParams, IList<string> methodBody)
        {
            var tmpList = new List<string>
            {
                $"{returnType} {methodName}({string.Join(", ", methodParams)})",
                "{"
            };

            tmpList.AddRange(methodBody);
            tmpList.Add("}");
            tmpList.Add("\n");
            AddTabsToMethods(tmpList);

            return string.Join("\n", tmpList);
        }

        /// <summary>
        /// Creates a main method with the given method body.
        /// </summary>
        /// <param name="methodBody">The content of the method.</param>
        /// <returns></returns>
        public static string MainMethod(IList<string> methodBody)
        {
            return GLSL.CreateMethod(GLSL.Type.Void, "main",
                new[] { "" }, methodBody);
        }

        /// <summary>
        /// Translates this class or struct to GLSL. Will only convert fields and properties.
        /// </summary>
        /// <param name="type">The type to translate.</param>
        /// <returns></returns>
        public static string DecodeSystemStructOrClass(System.Type type)
        {
            var res = new List<string>
            {
                $"struct {type.Name}",
                "{"
            };

            foreach (var field in type.GetFields())
                res.Add($"{DecodeType(field.FieldType)} {field.Name};");

            foreach (var prop in type.GetProperties())
                res.Add($"{DecodeType(prop.PropertyType)} {prop.Name};");

            res.Add("};");
            AddTabsToMethods(res);
            res.Add("\n");
            return string.Join("\n", res);
        }

        public static string DecodeType(System.Type type)
        {
            if (type.IsEnum)
                return "int";
            else if (type == typeof(float3x3))
                return "mat3";
            else if (type == typeof(float4x4))
                return "mat4";
            else if (type == typeof(float2))
                return "vec2";
            else if (type == typeof(float3))
                return "vec3";
            else if (type == typeof(float4))
                return "vec4";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(Texture) ||
                type == typeof(WritableTexture))
                return "sampler2D";
            else if (type == typeof(WritableCubeMap))
                return "samplerCube";
            else if (type == typeof(WritableArrayTexture))
                return "sampler2DArray";
            if ((type.IsValueType && !type.IsPrimitive) || type.IsClass) // => user-defined struct or class
                return type.Name;
            else
                throw new ArgumentException($"Cannot parse type {type.Name} ");
        }

        public static string DecodeType(Type type)
        {
            return type switch
            {
                Type.Mat3 => "mat3",
                Type.Mat4 => "mat4",
                Type.Vec2 => "vec2",
                Type.Vec3 => "vec3",
                Type.Vec4 => "vec4",
                Type.Boolean => "bool",
                Type.Float => "float",
                Type.Int => "int",
                Type.Sampler2D => "sampler2D",
                Type.SamplerCube => "samplerCube",
                Type.Void => "void",
                Type.ArrayTexture => "sampler2DArray",
                Type.Sampler2DShadow => "sampler2DShadow",
                Type.SamplerCubeShadow => "samplerCubeShadow",
                Type.ArrayTextureShadow => "sampler2DArrayShadow",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        internal static void AddTabsToMethods(List<string> list)
        {
            var indentCnt = 0;

            for (var i = 0; i < list.Count; i++)
            {
                var s = list[i];

                if (s == "}" || s == "};")
                    indentCnt--;

                if (indentCnt > 0)
                {
                    for (int j = 0; j < indentCnt; j++)
                        list[i] = "    " + list[i];
                }

                if (s == "{")
                    indentCnt++;
            }
        }
    }
}