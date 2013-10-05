using System;
using System.Collections.Generic;

using Fusee.Engine;
namespace Fusee.SceneManagement
{

    /// <summary>
    /// The Material class holds information about its Shaderprogram, Texture Data and Shader parameters. This class is in prototype state.
    /// </summary>
    public class Material
    {
        protected ImageData imgData;
        protected ImageData imgData2;
        protected ITexture iTex;
        protected ITexture iTex2;
        protected IShaderParam _vColorParam;
        protected IShaderParam _texture1Param;

        public ShaderProgram sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        public Material()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="_program">The Shaderprogram of this material instance.</param>
        public Material(ShaderProgram _program)
        {
            sp = _program;
        }

        /// <summary>
        /// Sets the materials shaderprogram onto the rendercontext upon render time.
        /// </summary>
        /// <param name="renderContext">The render context that handles the drawing of the application.</param>
        virtual public void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
        }
    }
}
