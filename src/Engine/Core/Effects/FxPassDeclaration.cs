namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// An effect pass declaration contains the relevant shader source code as well as a <see cref="RenderStateSet"/> declaration for a <see cref="ShaderEffect"/>.
    /// </summary>
    public interface IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/>.
        /// </summary>
        RenderStateSet StateSet { get; set; }
        /// <summary>
        /// Vertex shader as string
        /// </summary>
        /// 
        string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        string GS { get; set; }

    }

    /// <summary>
    /// An effect pass declaration contains the vertex, pixel and geometry shader source code as well as a <see cref="RenderStateSet"/> for a <see cref="ShaderEffect"/>.
    /// </summary>
    public struct FxPassDeclaration : IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/>.
        /// </summary>
        public RenderStateSet StateSet { get; set; }

        /// <summary>
        /// Vertex shader as string
        /// </summary>
        public string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        public string GS { get; set; }

        /// <summary>
        /// Pixel- or fragment shader as string
        /// </summary>
        public string PS { get; set; }
    }
}