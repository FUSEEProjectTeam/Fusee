using Fusee.Engine;
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

        public SpecularMaterial(RenderContext rc, ShaderProgram shaderProgram, string texturepath)
        {
            sp = shaderProgram;
            Textureparam = sp.GetShaderParam("texture1");
            SpecularLevel = sp.GetShaderParam("specularLevel");
            Shininess = sp.GetShaderParam("shininess");

            ImageData image = rc.LoadImage(texturepath);
            Tex = rc.CreateTexture(image);
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