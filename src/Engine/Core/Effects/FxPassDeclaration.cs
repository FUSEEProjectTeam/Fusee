namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// An effect pass declaration contains the relevant shader source code as well as a <see cref="RenderStateSet"/> declaration for the rendering pass declared by this instance.
    /// </summary>
    public interface IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
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
    /// An effect pass declaration contains the vertex, pixel and geometry shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct FxPassDeclaration : IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        public RenderStateSet StateSet { get; set; }

        /// <summary>
        /// Vertex shader as string
        /// </summary>
        // ReSharper disable InconsistentNaming
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

    /// <summary>
    /// A "proto" effect pass declaration contains the vertex and geometry shader source code as well as a partial  Fragment Shader, that is completed in a pre-pass, depending whether we render forward or deferred.
    /// It also contains a <see cref="RenderStateSet"/> declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct FxPassDeclarationProto : IFxPassDeclarationBase
    {
        /// <summary>
        /// The  <see cref="RenderStateSet"/> of the current effect pass.  
        /// </summary>
        public RenderStateSet StateSet { get; set; }
        /// <summary>
        /// Vertex shader as string
        /// </summary>
        // ReSharper disable InconsistentNaming
        public string VS { get; set; }

        /// <summary>
        /// Geometry-shader as string
        /// </summary>
        public string GS { get; set; }

        /// <summary>
        /// Partial Fragment Shader, that is completed in a pre-pass, depending whether we render forward or deferred.
        /// </summary>
        public string ProtoPS { get; set; }
    }
}
