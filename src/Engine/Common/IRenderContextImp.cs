using Fusee.Base.Common;
using Fusee.Math.Core;
using System.Collections.Generic;

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
        /// Constant id that describes the renderer. This can be used in shaders to do platform dependent things.
        /// </summary>
        FuseePlatformId FuseePlatformId { get; }

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
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping. 
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        void EnableDepthClamp();

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        void DisableDepthClamp();

        /// <summary>
        /// Creates a shader object from vertex shader source code and pixel shader source code.
        /// </summary>
        /// <param name="vs">A string containing the vertex shader source.</param>
        /// <param name="gs">A string containing the geometry shader source.</param>
        /// <param name="ps">A string containing the pixel (fragment) shader source code.</param>
        /// <returns>A shader program object identifying the combination of the given vertex and pixel shader.</returns>
        /// <remarks>
        /// Currently only shaders in GLSL (or rather GLSL/ES) source language(s) are supported.
        /// The result is already compiled to code executable on the GPU. <see cref="IRenderContextImp.SetShader"/>
        /// to activate the result as the current shader used for rendering geometry passed to the RenderContext.
        /// </remarks>
        IShaderHandle CreateShaderProgram(string vs, string ps, string gs = null);

        /// <summary>
        /// Creates a shader object from compute shader source code.
        /// </summary>
        /// <param name="cs">A string containing the compute shader source.</param>
        /// <returns></returns>
        IShaderHandle CreateShaderProgramCompute(string cs = null);

        /// <summary>
        /// Removes given shader program from GPU
        /// </summary>
        /// <param name="sp"></param>
        void RemoveShader(IShaderHandle sp);

        /// <summary>
        /// Creates a <see cref="IRenderTarget"/> with the purpose of being used as CPU GBuffer representation.
        /// </summary>
        /// <param name="res">The texture resolution.</param>
        IRenderTarget CreateGBufferTarget(TexRes res);

        /// <summary>
        /// Free all allocated gpu memory that belong to a frame buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        void DeleteFrameBuffer(IBufferHandle bh);

        /// <summary>
        /// Free all allocated gpu memory belonging to a render buffer object.
        /// </summary>
        /// <param name="bh">The platform dependent abstraction of the gpu buffer handle.</param>
        void DeleteRenderBuffer(IBufferHandle bh);

        /// <summary>
        /// Detaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will detach the texture currently associated with the ColorAttachment1.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture currently associated with the DepthAttachment will be detached.</param>       
        void DetachTextureFromFbo(IRenderTarget renderTarget, bool isDepthTex, int attachment = 0);


        /// <summary>
        /// Attaches a texture to the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="attachment">Number of the fbo attachment. For example: attachment = 1 will attach the texture to the ColorAttachment1.</param>
        /// <param name="isDepthTex">Determines if the texture is a depth texture. In this case the texture is attached to the DepthAttachment.</param>        
        /// <param name="texHandle">The gpu handle of the texture.</param>
        void AttacheTextureToFbo(IRenderTarget renderTarget, bool isDepthTex, ITextureHandle texHandle, int attachment = 0);

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
        IList<ShaderParamInfo> GetActiveUniformsList(IShaderHandle shaderProgram);

        /// <summary>
        /// Get a list of shader storage buffer variables accessed by the given shader.
        /// </summary>
        /// <param name="shaderProgram">The shader program to query for parameters.</param>
        IList<ShaderParamInfo> GetShaderStorageBufferList(IShaderHandle shaderProgram);

        /// <summary>
        /// Returns an identifier for the named (uniform) parameter used in the specified shader program.
        /// </summary>
        /// <param name="shaderProgram">The shader program using the parameter.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns>A handle object to identify the given parameter in subsequent calls to SetShaderParam.</returns>
        /// <remarks>
        /// The returned handle can be used to assign values to a (uniform) shader parameter.
        /// </remarks>
        /// <seealso cref="SetShaderParam(IShaderParam,float)"/>
        IShaderParam GetShaderUniformParam(IShaderHandle shaderProgram, string paramName);

        /// <summary>
        /// Sets the specified shader parameter to a float value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float val);

        /// <summary>
        /// Sets the specified shader parameter to a double value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, double val);

        /// <summary>
        /// Sets the shader parameter to a float2 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float2 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float2 val);

        /// <summary>
        /// Sets the shader parameter to a float2 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float2 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float2[] val);

        /// <summary>
        /// Sets the shader parameter to a float3 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float3 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float3 val);

        /// <summary>
        /// Sets the shader parameter to a float3 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float3 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float3[] val);

        /// <summary>
        /// Sets the shader parameter to a float4 value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float4 val);

        /// <summary>
        /// Sets the shader parameter to a float4 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float4 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float4[] val);

        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4x4 matrix that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float4x4 val);

        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix array.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4x4 matrix array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, float4x4[] val);

        /// <summary>
        /// Sets the shader parameter to a integer value.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The integer value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderUniformParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        void SetShaderParam(IShaderParam param, int val);

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="format">The internal sized format of the texture.</param>
        void SetShaderParamImage(IShaderParam param, ITextureHandle texId, TextureType texTarget, ImagePixelFormat format);

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        void SetShaderParamTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget);

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texId">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="texUnit">The texture unit.</param>
        void SetActiveAndBindTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget, out int texUnit);

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texId">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        void SetActiveAndBindTexture(IShaderParam param, ITextureHandle texId, TextureType texTarget);

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texIds">An array of ITextureHandles probably returned from CreateTexture method</param>
        /// /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        void SetShaderParamTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget);

        /// <summary>
        /// Uploads the given data to the SSBO. If the buffer is not created on the GPU by no it will be.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="storageBuffer">The Storage Buffer Object on the CPU.</param>
        /// <param name="data">The data that will be uploaded.</param>
        void StorageBufferSetData<T>(IStorageBuffer storageBuffer, T[] data) where T : struct;

        /// <summary>
        /// Deletes the shader storage buffer on the GPU.
        /// </summary>
        /// <param name="storageBufferHandle">The buffer object.</param>
        void DeleteStorageBuffer(IBufferHandle storageBufferHandle);

        /// <summary>
        /// Connects the given SSBO to the currently active shader program.
        /// </summary>
        /// <param name="currentProgram">The handle of the current shader program.</param>
        /// <param name="buffer">The Storage Buffer object on the CPU.</param>
        /// <param name="ssboName">The SSBO's name.</param>
        void ConnectBufferToShaderStorage(IShaderHandle currentProgram, IStorageBuffer buffer, string ssboName);

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texIds">An array of ITextureHandles returned from CreateTexture method or the ShaderEffectManager.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        /// <param name="texUnitArray">The texture units.</param>
        void SetActiveAndBindTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget, out int[] texUnitArray);

        /// <summary>
        /// Sets a texture active and binds it.
        /// </summary>
        /// <param name="param">The shader parameter, associated with this texture.</param>
        /// <param name="texIds">The texture handle.</param>
        /// <param name="texTarget">The texture type, describing to which texture target the texture gets bound to.</param>
        void SetActiveAndBindTextureArray(IShaderParam param, ITextureHandle[] texIds, TextureType texTarget);

        /// <summary>
        /// Updates the given region of a texture with the passed image data.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="img">The image.</param>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void UpdateTextureRegion(ITextureHandle tex, ITexture img, int startX, int startY, int width, int height);

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureFilterMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="filterMode">The new filter mode.</param>
        void SetTextureFilterMode(ITextureHandle tex, TextureFilterMode filterMode);

        /// <summary>
        /// Sets the textures filter mode (<see cref="TextureWrapMode"/> at runtime.
        /// </summary>
        /// <param name="tex">The handle of the texture.</param>
        /// <param name="wrapMode">The new wrap mode.</param>
        void SetTextureWrapMode(ITextureHandle tex, TextureWrapMode wrapMode);

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>
        /// <remarks>
        /// Method should be called after LoadImage method to process
        /// the BitmapData an make them available for the shader.
        /// </remarks>
        /// <param name="img">An <see cref="ITexture"/>, containing necessary information for the upload to the graphics card.</param>       
        ITextureHandle CreateTexture(ITexture img);

        /// <summary>
        /// Creates a new cube map and binds it to the shader.
        /// </summary>        
        /// <param name="img">An <see cref="IWritableArrayTexture"/>, containing necessary information for the upload to the graphics card.</param>       
        ITextureHandle CreateTexture(IWritableArrayTexture img);

        /// <summary>
        /// Creates a new cube map and binds it to the shader.
        /// </summary>        
        /// <param name="img">An <see cref="IWritableCubeMap"/>, containing necessary information for the upload to the graphics card.</param>       
        ITextureHandle CreateTexture(IWritableCubeMap img);

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>      
        /// <param name="img">An <see cref="IWritableTexture"/>, containing necessary information for the upload to the graphics card.</param>       
        ITextureHandle CreateTexture(IWritableTexture img);

        /// <summary>
        /// Removes the TextureHandle's buffers and textures from the graphics card's memory
        /// </summary>
        /// <remarks>
        /// Method should be called after an TextureHandle is no longer required by the application.
        /// </remarks>
        /// <param name="textureHandle">An TextureHandle object, containing necessary information for the upload to the graphics card.</param>
        void RemoveTextureHandle(ITextureHandle textureHandle);

        /// <summary>
        /// Sets the line width when drawing a mesh with primitive mode line
        /// </summary>
        /// <param name="width">The width of the line.</param>
        void SetLineWidth(float width);

        /// <summary>
        /// Erases the contents of the specified rendering buffers.
        /// </summary>
        /// <param name="flags">A combination of flags specifying the rendering buffers to clear.</param>
        /// <remarks>
        /// Calling this method erases all contents of the rendering buffers. A typical use case for this method
        /// is to erase the contents of the color buffer and the depth buffer (z-buffer) before rendering starts
        /// at the beginning of a rendering loop. Thus, rendering the current frame starts with an empty color and
        /// z-buffer. <see cref="ClearFlags"/> for a list of possible buffers to clear. Make sure to use the bitwise
        /// or-operator (|) to combine several buffers to clear.
        /// </remarks>
        void Clear(ClearFlags flags);

        /// <summary>
        /// Binds the VertexArrayPbject onto the GL render context and assigns its index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        void SetVertexArrayObject(IMeshImp mr);

        /// <summary>
        /// Binds the vertices onto the GL render context and assigns an VertexBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mesh">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="vertices">The vertices.</param>
        /// <exception cref="System.ArgumentException">Vertices must not be null or empty</exception>
        void SetVertices(IMeshImp mesh, float3[] vertices);

        /// <summary>
        /// Binds the tangents onto the GL render context and assigns an TangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="tangents">The tangents.</param>
        /// <exception cref="System.ArgumentException">Tangents must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetTangents(IMeshImp mr, float4[] tangents);

        /// <summary>
        /// Binds the bitangents onto the GL render context and assigns an BiTangentBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name = "bitangents">The bitangents.</param>
        /// <exception cref="System.ArgumentException">BiTangents must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetBiTangents(IMeshImp mr, float3[] bitangents);

        /// <summary>
        /// Binds the normals onto the GL render context and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="System.ArgumentException">Normals must not be null or empty</exception>
        void SetNormals(IMeshImp mr, float3[] normals);

        /// <summary>
        /// Binds the UV coordinates onto the GL render context and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="System.ArgumentException">UVs must not be null or empty</exception>
        void SetUVs(IMeshImp mr, float2[] uvs);

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        void SetColors(IMeshImp mr, uint[] colors);

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        void SetColors1(IMeshImp mr, uint[] colors);

        /// <summary>
        /// Binds the colors onto the GL render context and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        void SetColors2(IMeshImp mr, uint[] colors);

        /// <summary>
        /// Binds the triangles onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="System.ArgumentException">triangleIndices must not be null or empty</exception>
        void SetTriangles(IMeshImp mr, ushort[] triangleIndices);

        /// <summary>
        /// Binds the bone indices onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneIndices">The bone indices.</param>
        /// <exception cref="System.ArgumentException">boneIndices must not be null or empty</exception>
        void SetBoneIndices(IMeshImp mr, float4[] boneIndices);

        /// <summary>
        /// Binds the bone weights onto the GL render context and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="boneWeights">The bone weights.</param>
        /// <exception cref="System.ArgumentException">boneWeights must not be null or empty</exception>
        void SetBoneWeights(IMeshImp mr, float4[] boneWeights);

        /// <summary>
        /// Activates the passed shader program as the current shader for geometry rendering.
        /// </summary>
        /// <param name="shaderProgramImp">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        /// <seealso cref="IRenderContextImp.CreateShaderProgram"/>
        /// <see cref="IRenderContextImp.Render(IMeshImp)"/>
        void SetShader(IShaderHandle shaderProgramImp);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveVertices(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveNormals(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveColors(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveColors1(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveColors2(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveUVs(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveTriangles(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveBoneWeights(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveBoneIndices(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveTangents(IMeshImp mesh);

        /// <summary>
        /// Deletes the buffer associated with the mesh implementation.
        /// </summary>
        /// <param name="mesh">The mesh which buffer respectively GPU memory should be deleted.</param>
        void RemoveBiTangents(IMeshImp mesh);

        /// <summary>
        /// Sets the rectangular output region within the output buffer(s).
        /// </summary>
        /// <param name="x">leftmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="y">topmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="width">horizontal size (in pixels) of the output region.</param>
        /// <param name="height">vertical size (in pixels) of the output region.</param>
        /// <remarks>
        /// Setting the viewport limits the rendering output to the specified rectangular region.
        /// </remarks>
        void Viewport(int x, int y, int width, int height);

        /// <summary>
        /// Only pixels that lie within the scissor box can be modified by drawing commands.
        /// Note that the Scissor test must be enabled for this to work.
        /// </summary>
        /// <param name="x">X Coordinate of the lower left point of the scissor box.</param>
        /// <param name="y">Y Coordinate of the lower left point of the scissor box.</param>
        /// <param name="width">Width of the scissor box.</param>
        /// <param name="height">Height of the scissor box.</param>
        void Scissor(int x, int y, int width, int height);

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
        /// Launch the bound Compute Shader Program.
        /// </summary>
        /// <param name="kernelIndex"></param>
        /// <param name="threadGroupsX">The number of work groups to be launched in the X dimension.</param>
        /// <param name="threadGroupsY">The number of work groups to be launched in the Y dimension.</param>
        /// <param name="threadGroupsZ">he number of work groups to be launched in the Z dimension.</param>
        void DispatchCompute(int kernelIndex, uint threadGroupsX, uint threadGroupsY, uint threadGroupsZ);

        /// <summary>
        /// Defines a barrier ordering memory transactions. At the moment it will insert all supported barriers.
        /// TODO: Define GLbitfield enum
        /// </summary>
        void MemoryBarrier();

        /// <summary>
        /// Gets the content of the buffer.
        /// </summary>
        /// <param name="quad">The quad.</param>
        /// <param name="texId">The texture identifier.</param>
        void GetBufferContent(Rectangle quad, ITextureHandle texId);

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
        /// Renders into the given textures of the RenderTarget.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="texHandles">The texture handles, associated with the given textures. Each handle should be created by the TextureManager in the RenderContext.</param>
        void SetRenderTarget(IRenderTarget renderTarget, ITextureHandle[] texHandles);

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        void SetRenderTarget(IWritableTexture tex, ITextureHandle texHandle);

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The texture.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        void SetRenderTarget(IWritableCubeMap tex, ITextureHandle texHandle);

        /// <summary>
        /// Renders into the given layer of the array texture.
        /// </summary>
        /// <param name="tex">The array texture.</param>
        /// <param name="layer">The layer to render to.</param>
        /// <param name="texHandle">The texture handle, associated with the given texture. Should be created by the TextureManager in the RenderContext.</param>
        void SetRenderTarget(IWritableArrayTexture tex, int layer, ITextureHandle texHandle);

        /// <summary>
        /// Retrieves a sub-image of the given region.
        /// </summary>
        /// <param name="x">The x value of the start of the region.</param>
        /// <param name="y">The y value of the start of the region.</param>
        /// <param name="w">The width to copy.</param>
        /// <param name="h">The height to copy.</param>
        /// <returns>The specified sub-image</returns>
        IImageData GetPixelColor(int x, int y, int w, int h);

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
        /// <param name="capability">The capability to check against</param> 
        /// <returns>uint</returns> 
        uint GetHardwareCapabilities(HardwareCapability capability);

        /// <summary> 
        /// Returns a human readable description of the underlying graphics hardware 
        /// </summary> 
        /// <returns>Description</returns> 
        string GetHardwareDescription();
    }

    /// <summary>
    /// Checks if the hardware is capable of the following entries.
    /// If a numeric value is expected, this will be returned as a uint from the <see cref="IRenderContextImp.GetHardwareCapabilities"/>GetHardwareCapabilites
    /// </summary>
    public enum HardwareCapability
    {
        /// <summary>
        /// Checks if deferred rendering with frame buffer objects is possible
        /// </summary>
        CanRenderDeferred,

        /// <summary>
        /// Checks if geometry shaders can be used.
        /// </summary>
        CanUseGeometryShaders
    }

    /// <summary>
    /// This is the primitive type used by the RenderContext internally to distinguish between the different OpenGL primitives
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// Relates to OpenGl GL_TRIANGLES.
        /// </summary>
        Triangles = 0,

        /// <summary>
        /// Relates to OpenGl GL_TRIANGLES_STRIP.
        /// </summary>
        TriangleStrip,

        /// <summary>
        /// Relates to OpenGl GL_TRIANGLES_FAN.
        /// </summary>
        TriangleFan,

        /// <summary>
        /// Relates to OpenGl GL_QUADS.
        /// </summary>
        Quads,

        /// <summary>
        /// Relates to OpenGl GL_QUADS_STRIP.
        /// </summary>
        QuadStrip,

        /// <summary>
        /// Relates to OpenGl GL_POINTS.
        /// </summary>
        Points,

        /// <summary>
        /// Relates to OpenGl GL_LINES.
        /// </summary>
        Lines,

        /// <summary>
        /// Relates to OpenGl GL_LINE_STRIP.
        /// </summary>
        LineStrip,

        /// <summary>
        /// Relates to OpenGl GL_LINE_LOOP.
        /// </summary>
        LineLoop,

        /// <summary>
        /// Relates to OpenGl GL_PATCHES.
        /// </summary>
        Patches
    }
}