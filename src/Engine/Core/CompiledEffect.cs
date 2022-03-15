using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{

    internal struct CompiledEffects
    {
        internal CompiledEffect ForwardFx;
        internal CompiledEffect DeferredFx;
    }

    /// <summary>
    /// Compiled information of one <see cref="ShaderEffect"/>.
    /// </summary>
    internal class CompiledEffect : IEquatable<CompiledEffect>
    {
        /// <summary>
        /// The handle that identifies the shader program on the gpu.
        /// </summary>
        internal IShaderHandle GpuHandle;

        /// <summary>
        /// The shader parameters of all passes. See <see cref="IFxParam"/> on the parameter infos that are saved.
        /// </summary>
        internal Dictionary<int, IFxParam> ActiveUniforms = new();

        #region equals

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does equal right; false otherwise.
        /// </returns>
        public static bool operator ==(CompiledEffect lhs, CompiledEffect rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                    return true;

                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="lhs">The first instance.</param>
        /// <param name="rhs">The second instance.</param>
        /// <returns>
        /// True, if left does not equal right; false otherwise.
        /// </returns>
        public static bool operator !=(CompiledEffect lhs, CompiledEffect rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Indicates whether the NormapMapChannel is equal to another one.
        /// </summary>
        /// <param name="other">The NormapMapChannel to compare with this one.</param>
        /// <returns>
        /// true if the current NormapMapChannel is equal to the other; otherwise, false.
        /// </returns>
        public bool Equals(CompiledEffect other)
        {
            if (other is null)
                return false;
            return other.GpuHandle == GpuHandle && other.ActiveUniforms == ActiveUniforms;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((obj is null) || !GetType().Equals(obj.GetType()))
                return false;
            else
                return Equals((CompiledEffect)obj);

        }

        /// <summary>
        /// Returns the hash for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(GpuHandle, ActiveUniforms);
        }

        #endregion

    }
}