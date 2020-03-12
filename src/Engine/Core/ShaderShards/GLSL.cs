using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Void
        }

        internal static string CreateUniform(Type type, string varName)
        {
            return $"uniform {DecodeType(type)} {varName};";
        }

        internal static string CreateOut(Type type, string varName)
        {
            return $"out {DecodeType(type)} {varName};";
        }

        internal static string CreateIn(Type type, string varName)
        {
            return $"in  {DecodeType(type)} {varName};";
        }

        internal static string CreateVar(Type type, string varName)
        {
            return $"{DecodeType(type)} {varName}";
        }

        /// <summary>
        /// Creates a GLSL method
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="method">method body goes here</param>
        /// <returns></returns>
        internal static string CreateMethod(Type returnType, string methodName, string[] methodParams, IList<string> method)
        {
            method = method.Select(x => "   " + x).ToList(); // One Tab indent

            var tmpList = new List<string>
            {
                $"{DecodeType(returnType)} {methodName}({string.Join(", ", methodParams)})",
                "{"
            };
            tmpList.AddRange(method);
            tmpList.Add("}");
            tmpList.Add("\n");
            AddTabsToMethods(tmpList);

            return string.Join("\n", tmpList);
        }

        public static string DecodeType(System.Type type)
        {
            if (type == typeof(float3x3))
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
            else
                throw new ArgumentException($"Cannot parse type {type.Name} ");            
        }

        public static string DecodeType(Type type)
        {
            switch (type)
            {
                case Type.Mat3:
                    return "mat3";
                case Type.Mat4:
                    return "mat4";
                case Type.Vec2:
                    return "vec2";
                case Type.Vec3:
                    return "vec3";
                case Type.Vec4:
                    return "vec4";
                case Type.Boolean:
                    return "bool";
                case Type.Float:
                    return "float";
                case Type.Int:
                    return "int";
                case Type.Sampler2D:
                    return "sampler2D";
                case Type.SamplerCube:
                    return "samplerCube";
                case Type.Void:
                    return "void";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void AddTabsToMethods(List<string> list)
        {
            var indent = false;
            for (var i = 0; i < list.Count; i++)
            {
                var s = list[i];
                if (list[i].Contains("}"))
                    break;

                if (indent)
                    list[i] = "   " + s;

                if (list[i].Contains("{"))
                    indent = true;
            }
        }
    }
}