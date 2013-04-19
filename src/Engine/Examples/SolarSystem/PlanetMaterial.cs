using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Solar
{
    public class PlanetMaterial : Material
    {
        public IShaderParam Textureparam;
        public ITexture Tex;

        public PlanetMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);

        }
    }
}
