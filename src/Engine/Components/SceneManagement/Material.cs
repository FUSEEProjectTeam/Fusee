using System;
using System.Collections.Generic;

using Fusee.Engine;
namespace Fusee.SceneManagement
{
    public class Material
    {
        protected ImageData imgData;
        protected ImageData imgData2;
        protected ITexture iTex;
        protected ITexture iTex2;
        protected IShaderParam _vColorParam;
        protected IShaderParam _texture1Param;

        public ShaderProgram sp;

        public Material()
        {
            
        }
        public Material(ShaderProgram _program)
        {
            sp = _program;
        }

        virtual public void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
        }
    }
}
