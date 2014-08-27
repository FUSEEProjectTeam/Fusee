using System;
using System.Collections.Generic;
using System.Text;
using CrossSL.Meta;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CrossSL
{
    internal sealed class FunctionDesc
    {
        internal MethodDefinition Definion;
        internal StringBuilder Signature;
        internal StringBuilder Body;
        internal Collection<VariableDesc> Variables;
    }

    internal sealed class ShaderTarget
    {
        internal SLEnvironment Envr;
        internal int VersionID;
        internal int Version;
    }

    internal sealed class ShaderDesc
    {
        internal string Name;
        internal TypeDefinition Type;
        internal ShaderTarget Target;
        internal xSLShader.xSLDebug DebugFlags;
        internal CustomAttribute[] Precision;
        internal Collection<VariableDesc> Variables;
        internal IEnumerable<FunctionDesc>[] Funcs;
        internal IEnumerable<Instruction> Instructions;
    }

    internal sealed class VariableDesc
    {
        internal IMemberDefinition Definition;
        internal SLVariableType Attribute;
        internal Type DataType;
        internal bool IsArray;
        internal object Value;
        internal Instruction Instruction;
        internal bool IsReferenced;

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />,
        ///     is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" />
        ///     is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (VariableDesc))
                return false;

            var name = ((VariableDesc) obj).Definition.FullName;
            return name == Definition.FullName;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="VariableDesc" />,
        ///     is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="VariableDesc" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="VariableDesc" />
        ///     is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VariableDesc obj)
        {
            return obj.Definition.FullName == Definition.FullName;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing
        ///     algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyFieldInGetHashCode
            return Definition.FullName.GetHashCode();
        }
    }
}