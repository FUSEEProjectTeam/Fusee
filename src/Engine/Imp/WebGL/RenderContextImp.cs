using Fusee.Math;
using JSIL.Meta;
using JSIL.Proxy;

namespace Fusee.Engine.Proxies
{
    [JSProxy(
        typeof(Fusee.Engine.RenderContextImp),
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared
    )]
    public abstract class RenderContextImp
    {
        [JSChangeName("SetShaderParam1f")]
        [JSReplacement("$this.SetShaderParam1f($param, $val)")]
        public abstract void SetShaderParam(ShaderParamHandle param, float val);

        [JSChangeName("SetShaderParam2f")]
        [JSReplacement("$this.SetShaderParam2f($param, $val)")]
        public abstract void SetShaderParam(ShaderParamHandle param, float2 val);

        [JSChangeName("SetShaderParam3f")]
        [JSReplacement("$this.SetShaderParam3f($param, $val)")]
        public abstract void SetShaderParam(ShaderParamHandle param, float3 val);

        [JSChangeName("SetShaderParam4f")]
        [JSReplacement("$this.SetShaderParam4f($param, $val)")]
        public abstract void SetShaderParam(ShaderParamHandle param, float4 val);

        [JSChangeName("SetShaderParamMtx4f")]
        [JSReplacement("$this.SetShaderParamMtx4f($param, $val)")]
        public abstract void SetShaderParam(ShaderParamHandle param, float4x4 val);
    }
}

/*

using JSIL.Meta;
using Fusee.Engine;
using FuseeMath;


namespace Fusee.EngineImp
{
    public class RenderContextImp
    {
        public RenderContextImp(RenderCanvasImp renderCanvas)
        {
        }

        public ShaderParamHandleImp GetShaderParamHandle(ShaderProgramImp program, string paramName)
        {
            return null;
        }

        [JSChangeName("SetShaderParam1f")]
        public void SetShaderParam(ShaderParamHandleImp param, float val)
        {
        }

        [JSChangeName("SetShaderParam2f")]
        public void SetShaderParam(ShaderParamHandleImp param, float2 val)
        {
        }

        [JSChangeName("SetShaderParam3f")]
        public void SetShaderParam(ShaderParamHandleImp param, float3 val)
        {
        }

        [JSChangeName("SetShaderParam4f")]
        public void SetShaderParam(ShaderParamHandleImp param, float4 val)
        {
        }

        // TODO add vector implementations

        [JSChangeName("SetShaderParamMtx4f")]
        public void SetShaderParam(ShaderParamHandleImp param, float4x4 val)
        {
        }

        public float4x4 ModelView
        {
            get 
            { return float4x4.Identity; }
            set 
            {
            }
        }

        public float4x4 Projection
        {
            get
            { return float4x4.Identity; }
            set
            {
            }
        }


        public ShaderProgramImp CreateShader(string vs, string ps)
        {
            return new ShaderProgramImp();
        }

        public void SetShader(ShaderProgramImp program)
        {
        }

        public void Clear(ClearFlags flags)
        {
        }

        public void SetVertices(MeshImp mr, float3[] vertices)
        {
        }


        public void SetNormals(MeshImp mr, float3[] normals)
        {
        }

        public void SetColors(MeshImp mr, uint[] colors)
        {
        }


        public void SetTriangles(MeshImp mr, short[] triangleIndices)
        {
        }

        public void Render(MeshImp mr)
        {
        }

        public void Viewport(int x, int y, int width, int height)
        {
        }
    }
}
*/