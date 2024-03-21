﻿using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Contains GLSL methods and attributes to generate ShaderShards
    /// </summary>
    public static class GLSL
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum Type
        {
            Mat3,
            Mat4,
            Vec2,
            IVec2,
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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Create glsl uniform variable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static string CreateUniform(Type type, string varName)
        {
            return $"uniform {DecodeType(type)} {varName};";
        }

        /// <summary>
        /// Create glsl out variable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static string CreateOut(Type type, string varName)
        {
            return $"out {DecodeType(type)} {varName};\n";
        }

        /// <summary>
        /// Create glsl in variable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static string CreateIn(Type type, string varName)
        {
            return $"in {DecodeType(type)} {varName};\n";
        }

        /// <summary>
        /// Create a glsl variable from given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
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
            {
                if (!Attribute.IsDefined(field, typeof(Effects.NoUniformAttribute)))
                    res.Add($"{DecodeType(field.FieldType)} {field.Name};");
            }

            foreach (var prop in type.GetProperties())
            {
                if (!Attribute.IsDefined(prop, typeof(Effects.NoUniformAttribute)))
                    res.Add($"{DecodeType(prop.PropertyType)} {prop.Name};");
            }

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
            else if (type == typeof(int2))
                return "ivec2";
            else if (type == typeof(Texture) ||
                type == typeof(WritableTexture))
                return "sampler2D";
            else if (type == typeof(Texture1D))
                return "sampler1D";
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
                Type.IVec2 => "ivec2",
                Type.Vec3 => "vec3",
                Type.Vec4 => "vec4",
                Type.Boolean => "bool",
                Type.Float => "float",
                Type.Int => "int",
                Type.Sampler2D => "sampler2D",
                Type.SamplerCube => "samplerCube",
                Type.Void => "void",
                Type.ArrayTexture => "highp sampler2DArray",
                Type.Sampler2DShadow => "highp sampler2DShadow",
                Type.SamplerCubeShadow => "highp samplerCubeShadow",
                Type.ArrayTextureShadow => "highp sampler2DArrayShadow",
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