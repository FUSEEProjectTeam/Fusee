﻿using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Base.Common;
using Fusee.Math.Core;
using JSIL.Meta;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// The render context interface contains all functions necessary to manipulate the underlying rendering hardware. Use this class' elements
    /// to render geometry to the RenderCanvas associated with this context. If you are worked with OpenGL or DirectX before you will find
    /// many similarities in this class' methods and properties.
    /// </summary>
    public interface IRenderContextImp
    {
        /// <summary>
        /// The color to use when clearing the color buffer.
        /// </summary>
        /// <value>
        /// The color value is interpreted as a (Red, Green, Blue, Alpha) quadruple with
        /// component values ranging from 0.0f to 1.0f.
        /// </value>
        /// <remarks>
        /// This is the color that will be copied to all pixels in the output color buffer when Clear is called on the render context.
        /// </remarks>
        /// <seealso cref="Clear"/>
        float4 ClearColor { set; get; }

        /// <summary>
        /// The depth value to use when clearing the color buffer.
        /// </summary>
        /// <value>
        /// Typically set to the highest possible depth value. Typically ranges between 0 and 1.
        /// </value>
        /// <remarks>
        /// This is the depth (z-) value that will be copied to all pixels in the depth (z-) buffer when Clear is called on the render context.
        /// </remarks>
        float ClearDepth { set; get; }

        /// <summary>
        /// Creates a shader object from vertex shader source code and pixel shader source code.
        /// </summary>
        /// <param name="vs">A string containing the vertex shader source.</param>
        /// <param name="ps">A string containing the pixel (fragment) shader source code.</param>
        /// <returns>A shader program object identifying the combination of the given vertex and pixel shader.</returns>
        /// <remarks>
        /// Currently only shaders in GLSL (or rather GLSL/ES) source language(s) are supported.
        /// The result is already compiled to code executable on the GPU. <see cref="IRenderContextImp.SetShader"/>
        /// to activate the result as the current shader used for rendering geometry passed to the RenderContext.
        /// </remarks>
        IShaderProgramImp CreateShader(string vs, string ps);

        /// <summary>
        /// Get a list of (uniform) shader parameters accessed by the given shader.
        /// </summary>
        /// <param name="shaderProgram">The shader program to query for parameters.</param>
        /// <returns>
        /// A list of shader parameters accessed by the shader code of the given shader program. The parameters listed here
        /// are the so-called uniform parameters of the shader (in contrast to the varying parameters). The list contains all
        /// uniform parameters that are accessed by either the vertex shader, the pixel shader, or both shaders compiled into
        /// the given shader.
        /// </returns>
        IList<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram);

        /// <summary>
        /// Returns an identifiyer for the named (uniform) parameter used in the specified shader program.
        /// </summary>
        /// <param name="shaderProgram">The shader program using the parameter.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns>A handle object to identify the given parameter in subsequent calls to SetShaderParam.</returns>
        /// <remarks>
        /// The returned handle can be used to assign values to a (uniform) shader paramter.
        /// </remarks>
        /// <seealso cref="SetShaderParam(IShaderParam,float)"/>
        IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName);

        /// <summary>
        /// Gets the value of a shader parameter.
        /// </summary>
        /// <param name="shaderProgram">The program.</param>
        /// <param name="param">The handle.</param>
        /// <returns>The float value.</returns>
        float GetParamValue(IShaderProgramImp shaderProgram, IShaderParam param);

        /// <summary>
        /// Sets the specified shader parameter to a float value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParam1f")]
        void SetShaderParam(IShaderParam param, float val);

        /// <summary>
        /// Sets the shader parameter to a float2 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float2 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParam2f")]
        void SetShaderParam(IShaderParam param, float2 val);

        /// <summary>
        /// Sets the shader parameter to a float3 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float3 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParam3f")]
        void SetShaderParam(IShaderParam param, float3 val);

        /// <summary>
        /// Sets the shader parameter to a float4 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParam4f")]
        void SetShaderParam(IShaderParam param, float4 val);

        /// <summary>
        /// Sets the shader parameter to a float4 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float4 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParam4fArray")]
        void SetShaderParam(IShaderParam param, float4[] val);

        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4x4 matrix that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParamfloat4x4")]
        void SetShaderParam(IShaderParam param, float4x4 val);


        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix array.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4x4 matrix array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParamMtx4fArray")]
        void SetShaderParam(IShaderParam param, float4x4[] val);


        /// <summary>
        /// Sets the shader parameter to a integer value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The integer value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParamI")]
        void SetShaderParam(IShaderParam param, int val);


        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        void SetShaderParamTexture(IShaderParam param, ITexture texId);

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        /// /// <param name="texId">The gBuffer Handle</param>
        void SetShaderParamTexture(IShaderParam param, ITexture texId, GBufferHandle gHandle);

        /// <summary>
        /// Updates the texture from video the given video stream.
        /// </summary>
        /// <param name="stream">The video stream to retrieve an individual image.</param>
        /// <param name="tex">The texture to fill with the image from the video.</param>
        void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITexture tex);

        /// <summary>
        /// Updates the given region of a texture with te passed image data.
        /// </summary>
        /// <param name="tex">The tex.</param>
        /// <param name="img">The img.</param>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void UpdateTextureRegion(ITexture tex, ImageData img, int startX, int startY, int width, int height);

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>
        /// <remarks>
        /// Method should be called after LoadImage method to process
        /// the BitmapData an make them available for the shader.
        /// </remarks>
        /// <param name="imageData">An ImageData struct, containing necessary information for the upload to the graphics card.</param>
        /// <param name="repeat">Indicating if the texture should be clamped or repeated.</param>
        /// <returns>
        /// An ITexture that can be used for texturing in the shader.
        /// </returns>
        ITexture CreateTexture(ImageData imageData, bool repeat);

        /// <summary>
        /// Creates a new writable texture and binds it to the shader.
        /// Creates also a framebufferobject and installs convenience methods for binding and reading.
        /// </summary>
        /// <returns>
        /// An ITexture that can be used for rendering to a texture in the shader.
        /// </returns>
        ITexture CreateWritableTexture(int width, int height, ImagePixelFormat pixelFormat);

        /*
        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="color">The color of the image.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        ImageData CreateImage(int width, int height, ColorUint color);

        /// <summary>
        /// Maps a text in a specific font on an image.
        /// </summary>
        /// <param name="imgData">The ImageData struct with the PixelData from the image.</param>
        /// <param name="fontName">The name of the text-font.</param>
        /// <param name="fontSize">The size of the text-font.</param>
        /// <param name="text">The text that sould be mapped on the iamge.</param>
        /// <param name="textColor">The color of the text-font.</param>
        /// <param name="startPosX">The horizontal start-position of the text on the image.</param>
        /// <param name="startPosY">The vertical start-position of the text on the image.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing</returns>
        ImageData TextOnImage(ImageData imgData, string fontName, float fontSize, String text, string textColor,
            float startPosX, float startPosY);

        /// <summary>
        /// Loads a font file (*.ttf) and processes it with the given font size.
        /// </summary>
        /// <param name="stream">The stream to read font data from.</param>
        /// <param name="size">The font size.</param>
        /// <returns>An <see cref="IFont"/> containing all necessary information for further processing.</returns>
        IFont LoadFont(Stream stream, uint size);

        /// <summary>
        /// Fixes the kerning of a text (if possible).
        /// </summary>
        /// <param name="font">The <see cref="IFont"/> containing information about the font.</param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="text">The text.</param>
        /// <param name="scaleX">The scale x (OpenGL scaling factor).</param>
        /// <returns>The fixed vertices as an array of <see cref="float3"/>.</returns>
        float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX);
        */

        /// <summary>
        /// Erases the contents of the speciefied rendering buffers.
        /// </summary>
        /// <param name="flags">A combination of flags specifying the rendering buffers to clear.</param>
        /// <remarks>
        /// Calling this method erases all contents of the rendering buffers. A typical use case for this method
        /// is to erase the contents of the color buffer and the depth buffer (z-buffer) before rendering starts
        /// at the beginning of a rendering loop. Thus, rendering the current frame starts with an empty color and
        /// z-buffer. <see cref="ClearFlags"/> for a list of possible buffers to clear. Make sure to use the bitwisee
        /// or-operator (|) to combine several buffers to clear.
        /// </remarks>
        void Clear(ClearFlags flags);

        /// <summary>
        /// Binds the vertices onto the GL Rendercontext and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mesh">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="System.ArgumentException">Vertices must not be null or empty</exception>
        void SetVertices(IMeshImp mesh, float3[] vertices);

        /// <summary>
        /// Binds the normals onto the GL Rendercontext and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="System.ArgumentException">Normals must not be null or empty</exception>
        void SetNormals(IMeshImp mr, float3[] normals);

        /// <summary>
        /// Binds the UV coordinates onto the GL Rendercontext and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="System.ArgumentException">UVs must not be null or empty</exception>
        void SetUVs(IMeshImp mr, float2[] uvs);

        /// <summary>
        /// Binds the colors onto the GL Rendercontext and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        void SetColors(IMeshImp mr, uint[] colors);

        /// <summary>
        /// Binds the triangles onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="System.ArgumentException">triangleIndices must not be null or empty</exception>
        void SetTriangles(IMeshImp mr, ushort[] triangleIndices);

        /// <summary>
        /// Binds the boneindices onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The boneindices.</param>
        /// <exception cref="System.ArgumentException">boneIndices must not be null or empty</exception>
        void SetBoneIndices(IMeshImp mr, float4[] boneIndices);

        /// <summary>
        /// Binds the boneweights onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The boneweights.</param>
        /// <exception cref="System.ArgumentException">boneWeights must not be null or empty</exception>
        void SetBoneWeights(IMeshImp mr, float4[] boneWeights);

        /// <summary>
        /// Activates the passed shader program as the current shader for geometry rendering.
        /// </summary>
        /// <param name="shaderProgramImp">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        /// <seealso cref="IRenderContextImp.CreateShader"/>
        /// <see cref="IRenderContextImp.Render(IMeshImp)"/>
        void SetShader(IShaderProgramImp shaderProgramImp);

        /// <summary>
        /// Sets the rectangular output region within the output buffer(s).
        /// </summary>
        /// <param name="x">leftmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="y">topmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="width">horizontal size (in pixels) of the output region.</param>
        /// <param name="height">vertical size (in pixels) of the ouput region.</param>
        /// <remarks>
        /// Setting the Viewport limits the rendering ouptut to the specified rectangular region.
        /// </remarks>
        void Viewport(int x, int y, int width, int height);

        /// <summary>
        /// Enable or disable Color channels to be written to the frame buffer (final image).
        /// Use this function as a color channel filter for the final image.
        /// </summary>
        /// <param name="red">if set to <c>true</c> [red].</param>
        /// <param name="green">if set to <c>true</c> [green].</param>
        /// <param name="blue">if set to <c>true</c> [blue].</param>
        /// <param name="alpha">if set to <c>true</c> [alpha].</param>
        void ColorMask(bool red, bool green, bool blue, bool alpha);

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="mr">The mesh that should be rendered.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="IMeshImp"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        void Render(IMeshImp mr);

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The startpoint of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        void DebugLine(float3 start, float3 end, float4 color);

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The quad.</param>
        /// <param name="texId">The tex identifier.</param>
        void GetBufferContent(Rectangle quad, ITexture texId);

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        IMeshImp CreateMeshImp();
        /// <summary>
        /// Sets the specified render state to the given setting.
        /// </summary>
        /// <param name="renderState">The render state to set.</param>
        /// <param name="value">The new value of the render state.</param>
        void SetRenderState(RenderState renderState, uint value);

        /// <summary>
        /// Retrieves the value of the given render state.
        /// </summary>
        /// <param name="renderState">The render state to retrieve.</param>
        /// <returns>the current value of the render state.</returns>
        uint GetRenderState(RenderState renderState);
        
        /// <summary>
        /// Sets the RenderTarget, if texture is null rendertarget is the main screen, otherwise the picture will be rendered onto given texture
        /// </summary>
        /// <param name="texture">The texture as target</param>
        void SetRenderTarget(ITexture texture, bool deferredNormalPass = false);

        /*
         * TODO: NO tangent space normal maps at this time...
         * 
         * http://gamedev.stackexchange.com/a/72806/44105
         * 
        /// <summary>
        /// This method is a replacement for SetVertices, SetUVs and SetNormals. Taking all three
        /// vertex information arrays a the same time, an implementation can additionally calculate
        /// tangent and bitangent information as well. 
        /// </summary>
        /// <param name="meshImp">The mesh implementation to operate on.</param>
        /// <param name="vertices">The array of vertices</param>
        /// <param name="uVs">The texture coordinate array</param>
        /// <param name="normals">The normals</param>
        void SetVertexData(IMeshImp meshImp, float3[] vertices, float2[] uVs, float3[] normals);
         * */

        /// <summary>
        /// Retrieves a sub-image of the giben region.
        /// </summary>
        /// <param name="x">The x value of the start of the region.</param>
        /// <param name="y">The y value of the start of the region.</param>
        /// <param name="w">The width to copy.</param>
        /// <param name="h">The height to copy.</param>
        /// <returns>The specified sub-image</returns>
        ImageData GetPixelColor(int x, int y, int w, int h);

        /// <summary>
        /// Retrieves the Z-value at the given pixel position.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>The Z value at (x, y).</returns>
        float GetPixelDepth(int x, int y);

        /// <summary> 
        /// Returns the capabilities of the underlying graphics hardware 
        /// </summary> 
        /// <param name="capability"></param> 
        /// <returns>uint</returns> 
        uint GetHardwareCapabilities(HardwareCapability capability);


        /// <summary>
        /// Creates a FrameBufferObject
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        bool CreateFBO();
    }

    public enum HardwareCapability
    {
        DEFFERED_POSSIBLE,
        BUFFERSIZE
    }

}
