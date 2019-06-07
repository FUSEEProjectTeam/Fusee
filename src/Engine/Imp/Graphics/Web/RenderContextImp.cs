// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    /// <summary>
    /// Create an instance of this class to access the render context.
    /// </summary>
    public class RenderContextImp : IRenderContextImp
    {
        /// <summary>
        /// Gets and sets the background color.
        /// </summary>
        [JSExternal]
        public float4 ClearColor { get; set; }
        /// <summary>
        /// Gets and sets the depth.
        /// </summary>
        [JSExternal]
        public float ClearDepth { get; set; }
        /// <summary>
        /// Creates the shader.
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        [JSExternal]
        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Removes the shader.
        /// </summary>
        /// <param name="sp"></param>
        [JSExternal]
        public void RemoveShader(IShaderProgramImp sp)
        {
            throw new NotImplementedException();
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
        public void SetShaderParamTexture(IShaderParam param, ITextureHandle texId)
        {
            throw new System.NotImplementedException();
        }

        public void SetShaderParamTexture(IShaderParam param, ITextureHandle texId, GBufferHandle gHandle)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITextureHandle tex)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void UpdateTextureRegion(ITextureHandle tex, ITexture img, int startX, int startY, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public ITextureHandle CreateTexture(ITexture imageData, bool repeat)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void CopyDepthBufferFromDeferredBuffer(ITextureHandle texture)
        {
            throw new NotImplementedException();
        }

        public ITextureHandle CreateWritableTexture(int width, int height, WritableTextureFormat textureFormat)
        {
            TextureHandle returnTexture = null;

            try
            {
                switch (textureFormat)
                {
                    case WritableTextureFormat.Depth:
                        returnTexture = CreateDepthFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.CubeMap:
                        returnTexture = CreateCubeMapFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.RenderTargetTexture:
                        returnTexture = CreateRenderTargetTextureFramebuffer(width, height);
                        break;
                    case WritableTextureFormat.GBuffer:
                        returnTexture = CreateGBufferFramebuffer(width, height);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                Diagnostics.Log($"Error creating writable Texture: "+e);
            }
            return returnTexture;
        }

        private TextureHandle CreateGBufferFramebuffer(int width, int height)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        private TextureHandle CreateRenderTargetTextureFramebuffer(int width, int height)
        {
            throw new NotImplementedException();
        }

        private TextureHandle CreateCubeMapFramebuffer(int width, int height)
        {
            throw new NotImplementedException();
        }

        private TextureHandle CreateDepthFramebuffer(int width, int height)
        {
            throw new NotImplementedException();
        }

        // This should not be extern for the moment
        public ITextureHandle CreateWritableTexture()
        {
            throw new NotImplementedException("CreateWritableTexture not implmented!");
        }


        public string GetImageType(Stream stream)
        {
            string headerCode = GetHeaderInfo(stream).ToUpper();

            if (headerCode.StartsWith("89504E470D0A1A0A"))
            {
                return "image/png";
            }
            else if (headerCode.StartsWith("FFD8FFE0"))
            {
                return "image/jpeg";
            }
            else if (headerCode.StartsWith("424D"))
            {
                return "image/bmp";
            }
            else if (headerCode.StartsWith("474946"))
            {
                return "image/gif";
            }
            else if (headerCode.StartsWith("49492A"))
            {
                return "image/tiff";
            }
            else
            {
                return ""; //UnKnown
            }
        }

        public string GetHeaderInfo(Stream stream)
        {
            byte[] buffer = new byte[8];

            BinaryReader reader = new BinaryReader(stream);
            reader.Read(buffer, 0, buffer.Length);
            stream.Position = 0;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public byte[] GetFileBytes(Stream stream)
        {
            byte[] fileArray;
            int bufsize = 1024;
            long length=-1;
            bool allInOne;
            try
            {
                length = stream.Length;
                allInOne = (length < int.MaxValue);
            }
            catch (Exception ex)
            {
                allInOne = false;
            }

            if (allInOne)
                bufsize = (int)length;

            // no length: read in chunks
            MemoryStream ms = null;
            int count = 0;
            do
            {
                byte[] buf = new byte[bufsize];
                count = stream.Read(buf, 0, bufsize);
                if (allInOne && count == bufsize)
                {
                    return buf;
                }
                if (ms == null)
                    ms = new MemoryStream();
                allInOne = false;
                ms.Write(buf, 0, count);
            } while (stream.CanRead && count > 0);
            return ms.ToArray();
        }


        [JSExternal]
        public ImageData LoadImage(Stream stream)
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
        public IFont LoadFont(Stream stream, uint size)
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

        public void RemoveBiTangents(IMeshImp mesh)
        {
            throw new NotImplementedException();
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

        // This is not yet possible.
        // FBO in WebGL is in beta stadium - late 2016
        public void RenderDeferred(IMeshImp mr)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void GetBufferContent(Rectangle quad, ITextureHandle texId)
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

        //public void SetRenderTarget(ITextureHandle texture, bool deferredNormalPass = false)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Not needed for Web because EXT_FRAMEBUFFER is not possible with WebGL (2016)
        /// </summary>
        /// <param name="texture"></param>
        [JSExternal]
        public void SetRenderTarget(ITextureHandle texture)
        {
            throw new NotImplementedException();
        }

        public void SetCubeMapRenderTarget(ITextureHandle texture, int position)
        {
            throw new NotImplementedException();
        }

      
        /// <summary>
        /// Returns the pixel color.
        /// </summary>
        /// <param name="x">The Red value.</param>
        /// <param name="y">The green value.</param>
        /// <param name="w">The blue value.</param>
        /// <param name="h">The gamma value.</param>
        /// <returns></returns>
        [JSExternal]
        public IImageData GetPixelColor(int x, int y, int w, int h)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetPixelDepth(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public uint GetHardwareCapabilities(HardwareCapability capability)
        {
            throw new NotImplementedException();
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
        /// <summary>
        /// Sets the uv coordinates.
        /// </summary>
        ///// <param name="mr"> </param>
        /// <param name="uvs">The uv coordinates.</param>
        [JSExternal]
        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sets the bitangents.
        /// </summary>
        ///// <param name="mr"> </param>
        /// <param name="bitangents">The bitangent.</param>
        [JSExternal]
        public void SetBiTangents(IMeshImp mr, float3[] bitangents)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the tangents.
        /// </summary>
        ///// <param name="mr"> </param>
        /// <param name="tangents">Two points on the tangent.</param>
        [JSExternal]
        public void SetTangents(IMeshImp mr, float4[] tangents)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the normals.
        /// </summary>
        ///// <param name="mr"> </param>
        /// <param name="normals">The normal.</param>
        [JSExternal]
        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="vertices">The vertices.</param>
        [JSExternal]
        public void SetVertices(IMeshImp mesh, float3[] vertices)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Removes the vertices from the mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveVertices(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the normals.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveNormals(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the colors.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveColors(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the uvs.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveUVs(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the triangles.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveTriangles(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the bone weights.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveBoneWeights(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the bone indices.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveBoneIndices(IMeshImp mesh)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the tangents.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void RemoveTangents(IMeshImp mesh)
        {
            throw new NotImplementedException();
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

        [JSExternal]
        public bool CreateFBO()
        {
            throw new System.NotImplementedException();
        }
    }
}