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
        #region Fields
        /// <summary>
        /// The img data of the first image.
        /// </summary>
        protected ImageData imgData;
        /// <summary>
        /// The img data of the second image.
        /// </summary>
        protected ImageData imgData2;
        /// <summary>
        /// The <see cref="ITexture"/> for the first texture.
        /// </summary>
        protected ITexture iTex;
        /// <summary>
        /// The <see cref="ITexture"/> for the second texture.
        /// </summary>
        protected ITexture iTex2;
        /// <summary>
        /// The color parameter that will be passed into the shader.
        /// </summary>
        protected IShaderParam _vColorParam;
        /// <summary>
        /// The texture parameter that will be passed into the shader.
        /// </summary>
        protected IShaderParam _texture1Param;

        /// <summary>
        /// The <see cref="ShaderProgram"/> that is used to render this material.
        /// </summary>
        public ShaderProgram sp;
        #endregion
        #region Constructors
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
        #endregion
        #region Overrides
        /// <summary>
        /// Sets the materials shaderprogram onto the rendercontext upon render time.
        /// </summary>
        /// <param name="renderContext">The render context that handles the drawing of the application.</param>
        virtual public void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
        }
        #endregion
    }
}
