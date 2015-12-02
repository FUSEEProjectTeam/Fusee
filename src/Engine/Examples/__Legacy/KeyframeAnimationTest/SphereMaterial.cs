using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.KeyframeAnimationTest
{
    class SphereMaterial : Material
    {
        private readonly IShaderParam _colorParam;

        public SphereMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
            _colorParam = sp.GetShaderParam("color");
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParam(_colorParam, new float4(1.0f, 0f,0f, 1));
        }
    }
}
