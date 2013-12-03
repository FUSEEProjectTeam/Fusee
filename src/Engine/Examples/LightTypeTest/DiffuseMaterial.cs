using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class DiffuseMaterial : Material
    {
        public IShaderParam Textureparam;
        public ITexture Tex;

        public DiffuseMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public DiffuseMaterial(ShaderProgram shaderProgram, string texturepath)
        {
            sp = shaderProgram;
            Textureparam = sp.GetShaderParam("texture1");
            ImageData Image = SceneManager.RC.LoadImage(texturepath);
            Tex = SceneManager.RC.CreateTexture(Image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);

        }
    }
}
