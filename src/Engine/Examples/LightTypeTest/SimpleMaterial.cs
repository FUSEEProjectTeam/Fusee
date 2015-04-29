using Fusee.Engine;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class SimpleMaterial : Material
    {
        public IShaderParam Textureparam;
        public ITexture Tex;

        public SimpleMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public SimpleMaterial(RenderContext rc, ShaderProgram shaderProgram, string texturepath)
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