using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrossSL.Meta;
using Fusee.Math;
using Fusee.Math.Core;

namespace CrossSL
{
    internal class ShaderMapping
    {
        /// <summary>
        ///     A lookup table for mapping types from .NET to a shading language. This
        ///     table also contains all types whose methods need to be mapped by CrossSL.
        /// </summary>
        internal Dictionary<Type, string> Types = new Dictionary<Type, string>
        {
            {typeof (xSLShader), String.Empty},
            {typeof (Math), String.Empty},
            {typeof (M), String.Empty},

            // data types
            {typeof (void), "void"},
            {typeof (int), "int"},
            {typeof (float), "float"},
            {typeof (double), "double"}
        };

        /// <summary>
        ///     A lookup table for mapping methods from C# to a shading language.
        /// </summary>
        internal static Dictionary<string, string> Methods = new Dictionary<string, string>
        {
            {"Normalize", "normalize"},
            {"Dot", "dot"},
            {"Max", "max"},
            {"Min", "min"},
            {"Sin", "sin"},
            {"Cos", "cos"},
            {"Clamp", "clamp"}
        };

        /// <summary>
        ///     Resolves all <see cref="xSLShader" /> types and methods by reflection at
        ///     runtime, so that they can change and new types and methods can be added
        ///     without the need to manually update the fields of this class.
        /// </summary>
        /// <remarks>
        ///     The <see cref="xSLShader" /> types and their methods are marked with the
        ///     <see cref="xSLShader.MappingAttribute" /> attribute, which contains their
        ///     GLSL equivalent as the constructor argument.
        /// </remarks>
        protected void UpdateMapping()
        {
            var nestedTypes = typeof (xSLShader).GetNestedTypes(BindingFlags.NonPublic);
            var mappingAttr = nestedTypes.FirstOrDefault(type => type == typeof (xSLShader.MappingAttribute));

            // type mapping
            var dataTypes = nestedTypes.Where(type => type.GetCustomAttribute(mappingAttr) != null);

            foreach (var type in dataTypes)
            {
                var attrData = CustomAttributeData.GetCustomAttributes(type);
                var typeData = attrData.First(attr => attr.AttributeType == mappingAttr);
                Types.Add(type, typeData.ConstructorArguments[0].Value.ToString());
            }

            // mepthod mapping
            var allmethods = typeof (xSLShader).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var shMethods = allmethods.Where(type => type.GetCustomAttribute(mappingAttr) != null).ToList();

            foreach (var method in shMethods)
            {
                var attrData = CustomAttributeData.GetCustomAttributes(method);
                var methodData = attrData.First(attr => attr.AttributeType == mappingAttr);

                if (!Methods.ContainsKey(method.Name))
                    Methods.Add(method.Name, methodData.ConstructorArguments[0].Value.ToString());
            }
        }
    }
}