using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Controls for which layer this node is rendered
    /// </summary>
    /// <seealso cref="Fusee.Engine.Common.SceneComponent" />
    public class RenderLayer : SceneComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public RenderLayers Layer { get; set; } = RenderLayers.Default;
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum RenderLayers
    {
        /// <summary>
        /// Render on all layers
        /// </summary>
        All = 0x00000000,

        /// <summary>
        /// Equivalent to All
        /// </summary>
        Default = All,

        /// <summary>
        /// Render on none layer. This will take precedence over all other RenderLayers.
        /// </summary>
        None = 0x40000000,

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Layer01 = 0x00000001,
        Layer02 = 0x00000002,
        Layer03 = 0x00000004,
        Layer04 = 0x00000008,
        Layer05 = 0x00000010,
        Layer06 = 0x00000020,
        Layer07 = 0x00000040,
        Layer08 = 0x00000080,
        Layer09 = 0x00000100,
        Layer10 = 0x00000200,
        Layer11 = 0x00000400,
        Layer12 = 0x00000800,
        Layer13 = 0x00001000,
        Layer14 = 0x00002000,
        Layer15 = 0x00004000,
        Layer16 = 0x00008000,
        Layer17 = 0x00010000,
        Layer18 = 0x00020000,
        Layer19 = 0x00040000,
        Layer20 = 0x00080000,
        Layer21 = 0x00100000,
        Layer22 = 0x00200000,
        Layer23 = 0x00400000,
        Layer24 = 0x00800000,
        Layer25 = 0x01000000,
        Layer26 = 0x02000000,
        Layer27 = 0x04000000,
        Layer28 = 0x08000000,
        Layer29 = 0x10000000,
        Layer30 = 0x20000000,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}