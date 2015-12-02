using Fusee.Engine;
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

        public DiffuseMaterial(RenderContext rc, ShaderProgram shaderProgram, string texturepath)
        {
            sp = shaderProgram;
            Textureparam = sp.GetShaderParam("texture1");

            ImageData image = rc.LoadImage(texturepath);
            Tex = rc.CreateTexture(image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);
        }
    }
}