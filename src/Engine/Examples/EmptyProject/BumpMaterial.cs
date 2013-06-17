using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class BumpMaterial : Material
    {
        public IShaderParam Textureparam;
        public IShaderParam NormalTextureparam;
        public IShaderParam SpecularLevel, Shininess;
        public ITexture Tex;
        public ITexture NormalTex;

        public BumpMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public BumpMaterial(ShaderProgram shaderProgram, string texturepath, string texturepath2)
        {
            sp = shaderProgram;
            NormalTextureparam = sp.GetShaderParam("normalTex");
            Textureparam = sp.GetShaderParam("texture1");
            SpecularLevel = sp.GetShaderParam("specularLevel");
            Shininess = sp.GetShaderParam("shininess");
            ImageData image = SceneManager.RC.LoadImage(texturepath);
            ImageData image2 = SceneManager.RC.LoadImage(texturepath2);
            Tex = SceneManager.RC.CreateTexture(image);
            NormalTex = SceneManager.RC.CreateTexture(image2);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);
            renderContext.SetShaderParamTexture(NormalTextureparam, NormalTex);
            renderContext.SetShaderParam(Shininess, 16.0f);
            renderContext.SetShaderParam(SpecularLevel, 256.0f);

        }
    }
}
