using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class SpecularMaterial : Material
    {
        public IShaderParam Textureparam;
        public IShaderParam SpecularLevel, Shininess;
        public ITexture Tex;

        public SpecularMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public SpecularMaterial(ShaderProgram shaderProgram, string texturepath)
        {
            sp = shaderProgram;
            Textureparam = sp.GetShaderParam("texture1");
            SpecularLevel = sp.GetShaderParam("specularLevel");
            Shininess = sp.GetShaderParam("shininess");
            ImageData Image = SceneManager.RC.LoadImage(texturepath);
            Tex = SceneManager.RC.CreateTexture(Image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);
            renderContext.SetShaderParam(Shininess, 16.0f);
            renderContext.SetShaderParam(SpecularLevel, 256.0f);

        }
    }
}
