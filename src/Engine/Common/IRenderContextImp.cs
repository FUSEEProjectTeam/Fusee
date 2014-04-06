using System;
using System.Collections.Generic;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    /// The render context interface contains all functions necessary to manipulate the underlying rendering hardware. Use this class' elements
    /// to render geometry to the RenderCanvas associated with this context. If you are worked with OpenGL or DirectX before you will find
    /// many similarities in this class' methods and properties.
    /// </summary>
    public interface IRenderContextImp
    {
        /// <summary>
        /// The ModelView matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The 4x4 ModelView matrix defining the transformation applied to model coordinates yielding view coordinates.
        /// </value>
        /// <remarks>
        /// Model coordinates are the coordinates directly taken from the model (the mesh geometry - <see cref="IMeshImp"/> ). The rendering pipeline
        /// transforms these coordinates into View coordinates. Further down the pipeline the coordinates will be transformed to screen coordinates to allow the
        /// geometry to be rendered to pixel positions on the screen. The ModelView matrix defines the transformations performed on the original model coordinates
        /// to yield view coordinates. In most cases the matrix is a composition of several translations, rotations, and scale operations.
        /// </remarks>
        float4x4 ModelView { set; get; }

        /// <summary>
        /// The projection matrix used by the rendering pipeline
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="Fusee.Engine.IRenderContextImp.ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry wihth the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        float4x4 Projection { set; get; }

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
        /// The result is already compiled to code executable on the GPU. <see cref="Fusee.Engine.IRenderContextImp.SetShader"/>
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
        /// Sets the shader parameter to a float4x4 matrixvalue.
        /// </summary>
        /// <param name="param">The shader parameter identifier.</param>
        /// <param name="val">The float4x4 matrix that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        [JSChangeName("SetShaderParamMtx4f")]
        void SetShaderParam(IShaderParam param, float4x4 val);

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
        [JSChangeName("SetShaderParamInt")]
        void SetShaderParam(IShaderParam param, int val);

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        void SetShaderParamTexture(IShaderParam param, ITexture texId);

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>
        /// <remarks>
        /// Method should be called after LoadImage method to process
        /// the BitmapData an make them available for the shader.
        /// </remarks>
        /// <param name="imageData">An ImageData struct, containing necessary information for the upload to the graphics card.</param>
        /// <returns>
        /// An ITexture that can be used for texturing in the shader.
        /// </returns>
        ITexture CreateTexture(ImageData imageData);

        /// <summary>
        /// Loads an image file from disk and creates a new Bitmap-object out of it.
        /// </summary>
        /// <remarks>
        /// This is the first step for the texturing Process.
        /// The Bitmap-bits get locked in the memory and are made available for
        /// further processing. The returned ImageData-Struct can be used in the
        /// CreateTexture method.
        /// </remarks>
        /// <param name="filename">Path to the image file</param>
        /// <returns>
        /// An ImageData struct with all necessary information for the texture-binding process.
        /// </returns>
        ImageData LoadImage(String filename);

        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        ImageData CreateImage(int width, int height, String bgColor);

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
        ImageData TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor,
            float startPosX, float startPosY);

        /// <summary>
        /// Loads a font file (*.ttf) and processes it with the given font size.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="size">The font size.</param>
        /// <returns>An <see cref="IFont"/> containing all necessary information for further processing.</returns>
        IFont LoadFont(string filename, uint size);

        /// <summary>
        /// Fixes the kerning of a text (if possible).
        /// </summary>
        /// <param name="font">The <see cref="IFont"/> containing information about the font.</param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="text">The text.</param>
        /// <param name="scaleX">The scale x (OpenGL scaling factor).</param>
        /// <returns>The fixed vertices as an array of <see cref="float3"/>.</returns>
        float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX);

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
        /// <exception cref="System.ApplicationException"></exception>
        void SetVertices(IMeshImp mesh, float3[] vertices);

        /// <summary>
        /// Binds the normals onto the GL Rendercontext and assigns an NormalBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="normals">The normals.</param>
        /// <exception cref="System.ArgumentException">Normals must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetNormals(IMeshImp mr, float3[] normals);

        /// <summary>
        /// Binds the UV coordinates onto the GL Rendercontext and assigns an UVBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="uvs">The UV's.</param>
        /// <exception cref="System.ArgumentException">UVs must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetUVs(IMeshImp mr, float2[] uvs);

        /// <summary>
        /// Binds the colors onto the GL Rendercontext and assigns an ColorBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="System.ArgumentException">colors must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetColors(IMeshImp mr, uint[] colors);

        /// <summary>
        /// Binds the triangles onto the GL Rendercontext and assigns an ElementBuffer index to the passed <see cref="IMeshImp" /> instance.
        /// </summary>
        /// <param name="mr">The <see cref="IMeshImp" /> instance.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        /// <exception cref="System.ArgumentException">triangleIndices must not be null or empty</exception>
        /// <exception cref="System.ApplicationException"></exception>
        void SetTriangles(IMeshImp mr, ushort[] triangleIndices);

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
        /// Specify the View Frustum by settings its left,right,bottom,top,near and far planes. 
        /// Image the View frustum as a cubical form that determines the Cameras 3D view along its far plane. 
        /// </summary>
        /// <param name="left">The left plane.</param>
        /// <param name="right">The right plane.</param>
        /// <param name="bottom">The bottom plane.</param>
        /// <param name="top">The top plane.</param>
        /// <param name="zNear">The z near plane.</param>
        /// <param name="zFar">The z far plane.</param>
        void Frustum(double left, double right, double bottom, double top, double zNear, double zFar);

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

        void GetBufferContent(Rectangle quad, ITexture texId);

        /// <summary>
        /// Creates the mesh implementation.
        /// </summary>
        /// <returns>The <see cref="IMeshImp" /> instance.</returns>
        IMeshImp CreateMeshImp();
        void SetRenderState(RenderState renderState, uint value);

        uint GetRenderState(RenderState renderState);

        /*
         * NO tangent space normal maps at this time...
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
    }
}