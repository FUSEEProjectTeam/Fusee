using Fusee.Engine;
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

        public BumpMaterial(RenderContext rc, ShaderProgram shaderProgram, string texturepath, string texturepath2)
        {
            sp = shaderProgram;

            NormalTextureparam = sp.GetShaderParam("normalTex");
            Textureparam = sp.GetShaderParam("texture1");
            SpecularLevel = sp.GetShaderParam("specularLevel");
            Shininess = sp.GetShaderParam("shininess");

            ImageData image = rc.LoadImage(texturepath);
            ImageData image2 = rc.LoadImage(texturepath2);
            Tex = rc.CreateTexture(image);
            NormalTex = rc.CreateTexture(image2);
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