using System;
using System.Collections.Generic;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Engine
{
    public interface IRenderContextImp
    {
        float4x4 ModelView { set; get; }

        float4x4 Projection { set; get; }

        float4 ClearColor { set; get; }

        float ClearDepth { set; get; }

        IShaderProgramImp CreateShader(string vs, string ps);

        IEnumerable<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram);

        IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName);

        float GetParamValue(IShaderProgramImp shaderProgram, IShaderParam param);

        [JSChangeName("SetShaderParam1f")]
        void SetShaderParam(IShaderParam param, float val);

        [JSChangeName("SetShaderParam2f")]
        void SetShaderParam(IShaderParam param, float2 val);

        [JSChangeName("SetShaderParam3f")]
        void SetShaderParam(IShaderParam param, float3 val);

        [JSChangeName("SetShaderParam4f")]
        void SetShaderParam(IShaderParam param, float4 val);

        [JSChangeName("SetShaderParamMtx4f")]
        void SetShaderParam(IShaderParam param, float4x4 val);

        [JSChangeName("SetShaderParamInt")]
        void SetShaderParam(IShaderParam param, int val);

        //HP Functions
        //Bitmap LoadImage(String filename);
        
        ImageData LoadImage(String filename);
        ITexture CreateTexture(String filename);
        ITexture CreateTexture(ImageData img);
        void SetShaderParamTexture(IShaderParam param, ITexture texId);

        //HP Functions End

        void Clear(ClearFlags flags);

        void SetVertices(IMeshImp mesh, float3[] vertices);

        void SetNormals(IMeshImp mr, float3[] normals);

        void SetUVs(IMeshImp mr, float2[] uvs);

        void SetColors(IMeshImp mr, uint[] colors);

        void SetTriangles(IMeshImp mr, short[] triangleIndices);

        void SetShader(IShaderProgramImp shaderProgramImp);

        void Viewport(int x, int y, int width, int height);

        void Render(IMeshImp mr);

        IMeshImp CreateMeshImp();
    }
}