using Fusee.Engine;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class PlanetMaterial : Material
    {
        public IShaderParam TextureParam;
        public ITexture Tex;

        public PlanetMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public PlanetMaterial(ShaderProgram shaderProgram, string texturePath)
        {
            sp = shaderProgram;

            TextureParam = sp.GetShaderParam("texture1");

            var image = SceneManager.RC.LoadImage(texturePath);
            Tex = SceneManager.RC.CreateTexture(image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(TextureParam, Tex);
        }
    }
}