using System;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace CrossSL
{
    // ReSharper disable once InconsistentNaming
    internal static partial class ExtensionMethods
    {
        /// <summary>
        ///     Extension for TypeReference:
        ///     Resolves the type of a given <see cref="TypeReference" />.
        /// </summary>
        /// <param name="typeRef">The <see cref="TypeReference" /> to resolve.</param>
        /// <returns>The resolved type or <see cref="System.Object" /> if type is unknown.</returns>
        internal static Type ToType(this TypeReference typeRef)
        {
            return typeRef.Resolve().ToType();
        }

        /// <summary>
        ///     Extension for TypeDefinion:
        ///     Resolves the type of a given <see cref="TypeDefinition" />.
        /// </summary>
        /// <param name="typeDef">The <see cref="TypeDefinition" /> to resolve.</param>
        /// <returns>The resolved type or <see cref="System.Object" /> if type is unknown.</returns>
        internal static Type ToType(this TypeDefinition typeDef)
        {
            var fullName = typeDef.Module.Assembly.FullName;
            var typeName = Assembly.CreateQualifiedName(fullName, typeDef.FullName);
            return Type.GetType(typeName.Replace('/', '+')) ?? typeof (Object);
        }

        /// <summary>
        ///     Extension for TypeReference:
        ///     Determines whether the given <see cref="TypeReference" /> is of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to compare to.</typeparam>
        /// <param name="typeRef">The <see cref="TypeReference" /> to compare.</param>
        /// <returns></returns>
        internal static bool IsType<T>(this TypeReference typeRef)
        {
            return (typeRef.ToType() == typeof (T));
        }

        /// <summary>
        ///     Extension for TypeDefinion:
        ///     Determines whether the given <see cref="TypeDefinition" /> is of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to compare to.</typeparam>
        /// <param name="typeDef">The <see cref="TypeDefinition" /> to compare.</param>
        /// <returns></returns>
        internal static bool IsType<T>(this TypeDefinition typeDef)
        {
            return (typeDef.ToType() == typeof (T));
        }

        /// <summary>
        ///     Extension for Expression:
        ///     Determines whether the given <see cref="Expression" /> is of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to compare to.</typeparam>
        /// <param name="expr">The <see cref="Expression" /> to compare.</param>
        /// <returns></returns>
        internal static bool IsType<T>(this Expression expr)
        {
            return (expr.GetType() == typeof (T));
        }
    }
}