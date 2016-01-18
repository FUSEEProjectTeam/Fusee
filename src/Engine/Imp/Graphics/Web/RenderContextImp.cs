// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    public class RenderContextImp : IRenderContextImp
    {
        [JSExternal]
        public float4x4 ModelView { get; set; }
        [JSExternal]
        public float4x4 Projection { get; set; }
        [JSExternal]
        public float4 ClearColor { get; set; }
        [JSExternal]
        public float ClearDepth { get; set; }
        [JSExternal]
        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public RenderContextImp(IRenderCanvasImp canvas)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IList<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetParamValue(IShaderProgramImp shaderProgram, IShaderParam param)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParam1f")]
        public void SetShaderParam(IShaderParam param, float val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParamI")]
        public void SetShaderParam(IShaderParam param, int val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITexture tex)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void UpdateTextureRegion(ITexture tex, ImageData img, int startX, int startY, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ITexture CreateTexture(ImageData imageData, bool repeat)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ImageData LoadImage(string filename)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ImageData CreateImage(int width, int height, ColorUint color)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ImageData TextOnImage(ImageData imgData, string fontName, float fontSize, string text, string textColor, float startPosX,
            float startPosY)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IFont LoadFont(string filename, uint size)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Clear(ClearFlags flags)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetColors(IMeshImp mr, uint[] colors)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetTriangles(IMeshImp mr, ushort[] triangleIndices)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetShader(IShaderProgramImp shaderProgramImp)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Viewport(int x, int y, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Render(IMeshImp mr)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void GetBufferContent(Rectangle quad, ITexture texId)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IMeshImp CreateMeshImp()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetRenderState(RenderState renderState, uint value)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public uint GetRenderState(RenderState renderState)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ImageData GetPixelColor(int x, int y, int w, int h)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetPixelDepth(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetBoneWeights(IMeshImp mr, float4[] boneWeights)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetBoneIndices(IMeshImp mr, float4[] boneIndices)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetVertices(IMeshImp mesh, float3[] vertices)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParamMtx4fArray")]
        public void SetShaderParam(IShaderParam param, float4x4[] val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParamfloat4x4")]
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParam4fArray")]
        public void SetShaderParam(IShaderParam param, float4[] val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParam4f")]
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParam3f")]
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        [JSChangeName("SetShaderParam2f")]
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            throw new System.NotImplementedException();
        }
    }
}