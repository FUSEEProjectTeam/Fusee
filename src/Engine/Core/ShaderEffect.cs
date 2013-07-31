using System;

namespace Fusee.Engine
{
    public struct EffectPassDeclaration
    {
        public RenderStateSet StateSet;
        public string VS;
        public string PS;
    }

    /// <summary>
    /// A ShaderEffect contains a list of passes with each pass item being a combination of a set of render states, a ShaderProgram (the code running on the GPU), 
    /// </summary>
    public class ShaderEffect
    {
        private readonly RenderStateSet[] _states;
        private readonly ShaderProgram[] _compiledShaders; 
        private readonly string[] _vertexShaderSrc;
        private readonly string[] _pixelShaderSrc;

        internal RenderContext _rc;

        public ShaderEffect(EffectPassDeclaration[] effectDeclaration)
        {
            if (effectDeclaration == null || effectDeclaration.Length == 0)
                throw new ArgumentNullException("effectDeclaration", "must not be null and must contain at least one pass");
            
            int nPasses = effectDeclaration.Length;
            
            _states = new RenderStateSet[nPasses];
            _compiledShaders = new ShaderProgram[nPasses];
            _vertexShaderSrc = new string[nPasses];
            _pixelShaderSrc = new string[nPasses];

            for (int i = 0; i < nPasses; i++)
            {
                _states[i] = effectDeclaration[i].StateSet;
                _vertexShaderSrc[i] = effectDeclaration[i].VS;
                _pixelShaderSrc[i] = effectDeclaration[i].PS;
            }
        }

        public void AttachToContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc", "must pass a valid render context.");

            _rc = rc;
            int i=0, nPasses = _vertexShaderSrc.Length;
            try
            {
                for (i = 0; i < nPasses; i++)
                {
                    _compiledShaders[i] = _rc.CreateShader(_vertexShaderSrc[i], _pixelShaderSrc[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while compiling shader for pass " + i, ex);
            }
        }

        public void DetachFromContext()
        {
            
        }

        public void RenderMesh(Mesh mesh)
        {
            int i = 0, nPasses = _vertexShaderSrc.Length;
            try
            {
                for (i = 0; i < nPasses; i++)
                {
                    // TODO: Use shared uniform paramters - currently SetShader will query the shader params and set all the common uniforms (like matrices and light)
                    _rc.SetShader(_compiledShaders[i]);
                    _states[i].Apply(_rc);

                    // TODO: split up RenderContext.Render into a preparation and a draw call so that we can prepare a mesh once and draw it for each pass.
                    _rc.Render(mesh);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while rendering pass " + i, ex);
            }
        }
    }
}
