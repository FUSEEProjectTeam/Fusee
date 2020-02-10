using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// The render context contains all functions necessary to manipulate the underlying rendering hardware. Use this class' elements
    /// to render geometry to the RenderCanvas associated with this context. If you have worked with OpenGL or DirectX before you will find
    /// many similarities in this class' methods and properties.
    /// </summary>
    public class RenderContext
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
        public float4 ClearColor
        {
            set { _rci.ClearColor = value; }
            get { return _rci.ClearColor; }
        }

        /// <summary>
        /// The depth value to use when clearing the color buffer.
        /// </summary>
        /// <value>
        /// Typically set to the highest possible depth value. Typically ranges between 0 and 1.
        /// </value>
        /// <remarks>
        /// This is the depth (z-) value that will be copied to all pixels in the depth (z-) buffer when Clear is called on the render context.
        /// </remarks>
        public float ClearDepth
        {
            set { _rci.ClearDepth = value; }
            get { return _rci.ClearDepth; }
        }

        /// <summary>
        /// Contains the default state of the render context. can be used to reset this RenderContext to it's DefaultState.
        /// </summary>
        public RenderContextDefaultState DefaultState { get; private set; }

        /// <summary>
        /// Saves all global shader parameters. "Global" are those which get updated by a SceneRenderer, e.g. the matrices or the parameters of the lights.
        /// </summary>
        internal readonly Dictionary<string, object> GlobalFXParams = new Dictionary<string, object>();

        private readonly MeshManager _meshManager;
        private readonly TextureManager _textureManager;

        private bool _updatedShaderParams;

        #region RenderState management properties

        /// <summary>
        /// Saves the current RenderState.
        /// </summary>
        public RenderStateSet CurrentRenderState { get; private set; } = new RenderStateSet();

        /// <summary>
        /// If a state is forced it will remain the value currently set in <see cref="CurrentRenderState"/>.
        /// </summary>
        public Dictionary<RenderState, KeyValuePair<bool, uint>> LockedStates { get; private set; } = new Dictionary<RenderState, KeyValuePair<bool, uint>>();

        #endregion

        #region Viewport properties

        /// <summary>
        /// Gets and sets the viewport width.
        /// </summary>
        public int ViewportWidth { get; private set; }

        /// <summary>
        /// Gets and sets the viewport height.
        /// </summary>
        public int ViewportHeight { get; private set; }

        /// <summary>
        /// Gets and sets the x coordinate of viewport's lower left (starting) point.
        /// </summary>
        public int ViewportXStart { get; private set; }

        /// <summary>
        /// Gets and sets the y coordinate of viewport's lower left (starting) point.
        /// </summary>
        public int ViewportYStart { get; private set; }

        #endregion

        #region Shader Management fields

        private readonly IRenderContextImp _rci;

        private readonly ShaderEffectManager _shaderEffectManager;
        private readonly Dictionary<ShaderEffect, CompiledShaderEffect> _allCompiledShaderEffects = new Dictionary<ShaderEffect, CompiledShaderEffect>();

        /// <summary>
        /// Caches matrices of the current shader to minimize <see cref="SetShaderEffect(ShaderEffect)"/> calls.
        /// </summary>
        private readonly MatrixParams _currentShaderMatrixParams;

        /// <summary>
        /// The currently used <see cref="ShaderEffect"/> is set in <see cref="SetShaderEffect(ShaderEffect)"/>.
        /// </summary>
        private ShaderEffect _currentShaderEffect;

        /// <summary>
        /// The currently bound shader program. One <see cref="ShaderEffect"/> can result in one to _n_ <see cref="ShaderProgram"/>s, one for each pass.
        /// Is set in <see cref="Render"/> --> <see cref="SetShaderProgram(ShaderProgram)"/>.
        /// </summary>
        private ShaderProgram _currentShaderProgram;

        #endregion

        #region Matrix backing fields and flags

        // Settable matrices
        private float4x4 _modelView;
        private float4x4 _projection;
        private float4x4 _view;
        private float4x4 _model;

        private float4x4[] _bones;

        // Derived matrices
        private float4x4 _modelViewProjection;

        private float4x4 _invView;
        private float4x4 _invModel;
        private float4x4 _invModelView;
        private float4x4 _invProjection;
        private float4x4 _invModelViewProjection;

        private float4x4 _invTransView;
        private float4x4 _invTransModel;
        private float4x4 _invTransModelView;
        private float4x4 _invTransProjection;
        private float4x4 _invTransModelViewProjection;

        private float4x4 _transView;
        private float4x4 _transModel;
        private float4x4 _transModelView;
        private float4x4 _transProjection;
        private float4x4 _transModelViewProjection;

        private bool _modelViewProjectionOk;

        private bool _modelViewOK;

        private bool _invViewOk;
        private bool _invModelOk;
        private bool _invModelViewOk;
        private bool _invProjectionOk;
        private bool _invModelViewProjectionOk;

        private bool _invTransViewOk;
        private bool _invTransModelOk;
        private bool _invTransModelViewOk;
        private bool _invTransProjectionOk;
        private bool _invTransModelViewProjectionOk;

        private bool _transViewOk;
        private bool _transModelOk;
        private bool _transModelViewOk;
        private bool _transProjectionOk;
        private bool _transModelViewProjectionOk;

        #endregion

        #region Matrix Properties

        /// <summary>
        /// The View matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        /// <remarks>
        /// This matrix is also referred often as the camera transformation(not the projection). 
        /// It describes the orientation of the view that is used to render a scene.
        /// You can use <see cref="float4x4.LookAt(float3, float3, float3)"/> to create a valid view matrix and analyze how it is build up.
        /// </remarks>
        public float4x4 View
        {
            get { return _view; }
            set
            {
                _view = value;

                _modelViewProjectionOk = false;
                _invViewOk = false;
                _invModelViewOk = false;
                _invModelViewProjectionOk = false;

                _invTransViewOk = false;
                _invTransModelViewOk = false;
                _invTransModelViewProjectionOk = false;

                _transViewOk = false;
                _transModelViewOk = false;
                _transModelViewProjectionOk = false;
                _modelViewOK = false;

                UpdateCurrentShader();
            }
        }

        /// <summary>
        /// The Model matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The model matrix.
        /// </value>
        /// <remarks>
        /// Model coordinates are the coordinates directly taken from the model (the mesh geometry - <see cref="Mesh"/>).
        /// </remarks>
        public float4x4 Model
        {

            get { return _model; }
            set
            {
                _model = value;

                _modelViewProjectionOk = false;
                _invModelOk = false;
                _invModelViewOk = false;
                _invModelViewProjectionOk = false;

                _invTransModelOk = false;
                _invTransModelViewOk = false;
                _invTransModelViewProjectionOk = false;

                _transModelOk = false;
                _transModelViewOk = false;
                _transModelViewProjectionOk = false;
                _modelViewOK = false;

                UpdateCurrentShader();
            }
        }

        /// <summary>
        /// The projection matrix used by the rendering pipeline
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="RenderContext.ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry with the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        public float4x4 Projection
        {
            get { return _projection; }
            set
            {
                // Update matrix
                _projection = value;

                // Invalidate derived matrices
                _modelViewProjectionOk = false;
                _invProjectionOk = false;
                _invProjectionOk = false;
                _invTransProjectionOk = false;
                _invTransProjectionOk = false;
                _transProjectionOk = false;
                _transProjectionOk = false;

                UpdateCurrentShader();
            }
        }

        /// <summary>
        /// The ModelView matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The 4x4 ModelView matrix defining the transformation applied to model coordinates yielding view coordinates.
        /// </value>
        /// <remarks>
        /// Model coordinates are the coordinates directly taken from the model (the mesh geometry - <see cref="Mesh"/>). The rendering pipeline
        /// transforms these coordinates into View coordinates. Further down the pipeline the coordinates will be transformed to screen coordinates to allow the
        /// geometry to be rendered to pixel positions on the screen. The ModelView matrix defines the transformations performed on the original model coordinates
        /// to yield view coordinates. In most cases the matrix is a composition of several translations, rotations, and scale operations.
        /// </remarks>
        public float4x4 ModelView
        {
            get
            {
                if (!_modelViewOK)
                    _modelView = View * Model;
                return _modelView;
            }
        }


        /// <summary>
        /// The combination of the ModelView and Projection matrices.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix multiplication of the ModelView and the Projection matrix.
        /// </value>
        /// <remarks>
        /// <see cref="RenderContext.ModelView"/> and <see cref="RenderContext.Projection"/>.
        /// </remarks>
        public float4x4 ModelViewProjection
        {
            get
            {
                if (!_modelViewProjectionOk)
                {
                    // Row order notation
                    // _modelViewProjection = float4x4.Mult(ModelView, Projection);

                    // Column order notation
                    _modelViewProjection = float4x4.Mult(Projection, ModelView);
                    _modelViewProjectionOk = true;
                }
                return _modelViewProjection;
            }
        }

        /// <summary>
        /// Gets the inverted View matrix.
        /// </summary>
        /// <value>
        /// The inverted view matrix.
        /// </value>
        /// <remarks>
        /// If the View matrix is orthogonal (i.e. contains no scale component), its inverse matrix
        /// is equal to its transpose matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.View"/>
        /// <seealso cref="RenderContext.TransView"/>
        public float4x4 InvView
        {
            get
            {
                if (!_invViewOk)
                {
                    _invView = float4x4.Invert(View);
                    _invViewOk = true;
                }
                return _invView;
            }
        }

        /// <summary>
        /// Gets the inverted Model matrix.
        /// </summary>
        /// <value>
        /// The inverted Model matrix.
        /// </value>
        /// <remarks>
        /// If the Model matrix is orthogonal (i.e. contains no scale component), its inverse matrix
        /// is equal to its transpose matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.Model"/>
        /// <seealso cref="RenderContext.TransModel"/>
        public float4x4 InvModel
        {
            get
            {
                if (!_invModelOk)
                {
                    _invModel = float4x4.Invert(Model);
                    _invModelOk = true;
                }
                return _invModel;
            }
        }

        /// <summary>
        /// The inverse of the ModelView matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion applied to the ModelView matrix.
        /// </value>
        /// <remarks>
        /// If the ModelView matrix is orthogonal (i.e. contains no scale component), its inverse matrix
        /// is equal to its transpose matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.ModelView"/>
        /// <seealso cref="RenderContext.TransModelView"/>
        public float4x4 InvModelView
        {
            get
            {
                if (!_invModelViewOk)
                {
                    _invModelView = float4x4.Invert(ModelView);
                    _invModelViewOk = true;
                }
                return _invModelView;
            }
        }


        /// <summary>
        /// The inverse of the Projection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion applied to the Projection matrix.
        /// </value>
        /// <remarks>
        /// If the Projection matrix is orthogonal (i.e. contains no scale component), its inverse matrix
        /// is equal to its transpose matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.Projection"/>
        /// <seealso cref="RenderContext.TransProjection"/>
        public float4x4 InvProjection
        {
            get
            {
                if (!_invProjectionOk)
                {
                    _invProjection = float4x4.Invert(Projection);
                    _invProjectionOk = true;
                }
                return _invProjection;
            }
        }


        /// <summary>
        /// The inverse of the ModelViewProjection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion applied to the ModelViewProjection matrix.
        /// </value>
        /// <remarks>
        /// If the ModelViewProjection matrix is orthogonal (i.e. contains no scale component), its inverse matrix
        /// is equal to its transpose matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.ModelViewProjection"/>
        /// <seealso cref="RenderContext.TransModelViewProjection"/>
        public float4x4 InvModelViewProjection
        {
            get
            {
                if (!_invModelViewProjectionOk)
                {
                    _invModelViewProjection = float4x4.Invert(ModelViewProjection);
                    _invModelViewProjectionOk = true;
                }
                return _invModelViewProjection;
            }
        }

        /// <summary>
        /// The transpose of the View matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix transpose applied to the View matrix.
        /// </value>
        /// <remarks>
        /// If the View matrix is orthogonal (i.e. contains no scale component), its transpose matrix
        /// is equal to its inverse matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.View"/>
        /// <seealso cref="RenderContext.InvView"/>
        public float4x4 TransView
        {
            get
            {
                if (!_transViewOk)
                {
                    _transView = float4x4.Transpose(View);
                    _transViewOk = true;
                }
                return _transView;
            }
        }

        /// <summary>
        /// The transpose of the Model matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix transpose applied to the Model matrix.
        /// </value>
        /// <remarks>
        /// If the Model matrix is orthogonal (i.e. contains no scale component), its transpose matrix
        /// is equal to its inverse matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.Model"/>
        /// <seealso cref="RenderContext.InvModel"/>
        public float4x4 TransModel
        {
            get
            {
                if (!_transModelOk)
                {
                    _transModel = float4x4.Transpose(Model);
                    _transModelOk = true;
                }
                return _transModel;
            }
        }

        /// <summary>
        /// The transpose of the ModelView matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix transpose applied to the ModelView matrix.
        /// </value>
        /// <remarks>
        /// If the ModelView matrix is orthogonal (i.e. contains no scale component), its transpose matrix
        /// is equal to its inverse matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.ModelView"/>
        /// <seealso cref="RenderContext.InvModelView"/>
        public float4x4 TransModelView
        {
            get
            {
                if (!_transModelViewOk)
                {
                    _transModelView = float4x4.Transpose(ModelView);
                    _transModelViewOk = true;
                }
                return _transModelView;
            }
        }


        /// <summary>
        /// The transpose of the Projection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix transpose applied to the Projection matrix.
        /// </value>
        /// <remarks>
        /// If the Projection matrix is orthogonal (i.e. contains no scale component), its transpose matrix
        /// is equal to its inverse matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.Projection"/>
        /// <seealso cref="RenderContext.InvProjection"/>
        public float4x4 TransProjection
        {
            get
            {
                if (!_transProjectionOk)
                {
                    _transProjection = float4x4.Transpose(Projection);
                    _transProjectionOk = true;
                }
                return _transProjection;
            }
        }


        /// <summary>
        /// The transpose of the ModelViewProjection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix transpose applied to the ModelViewProjection matrix.
        /// </value>
        /// <remarks>
        /// If the ModelViewProjection matrix is orthogonal (i.e. contains no scale component), its transpose matrix
        /// is equal to its inverse matrix.
        /// </remarks>
        /// <seealso cref="RenderContext.ModelViewProjection"/>
        /// <seealso cref="RenderContext.InvModelViewProjection"/>
        public float4x4 TransModelViewProjection
        {
            get
            {
                if (!_transModelViewProjectionOk)
                {
                    _transModelViewProjection = float4x4.Transpose(ModelViewProjection);
                    _transModelViewProjectionOk = true;
                }
                return _transModelViewProjection;
            }
        }

        /// <summary>
        /// The inverse transpose of the View matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion and transpose applied to the View matrix.
        /// </value>
        /// <remarks>
        /// If the View matrix is orthogonal (i.e. contains no scale component), its inverse transpose matrix
        /// is the same as the original View matrix.
        /// </remarks>
        /// <seealso cref="View"/>
        /// <seealso cref="InvView"/>
        /// <seealso cref="TransView"/>
        public float4x4 InvTransView
        {
            get
            {
                if (!_invTransViewOk)
                {
                    _invTransView = float4x4.Invert(TransView);
                    _invTransViewOk = true;
                }
                return _invTransView;
            }
        }

        /// <summary>
        /// The inverse transpose of the Model matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion and transpose applied to the Model matrix.
        /// </value>
        /// <remarks>
        /// If the Model matrix is orthogonal (i.e. contains no scale component), its inverse transpose matrix
        /// is the same as the original Model matrix.
        /// </remarks>
        /// <seealso cref="Model"/>
        /// <seealso cref="InvModel"/>
        /// <seealso cref="TransModel"/>
        public float4x4 InvTransModel
        {
            get
            {
                if (!_invTransModelOk)
                {
                    _invTransModel = float4x4.Invert(TransModel);
                    _invTransModelOk = true;
                }
                return _invTransModel;
            }
        }

        /// <summary>
        /// The inverse transpose of the ModelView matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion and transpose applied to the ModelView matrix.
        /// </value>
        /// <remarks>
        /// If the ModelView matrix is orthogonal (i.e. contains no scale component), its inverse transpose matrix
        /// is the same as the original ModelView matrix.
        /// </remarks>
        /// <seealso cref="ModelView"/>
        /// <seealso cref="InvModelView"/>
        /// <seealso cref="TransModelView"/>
        public float4x4 InvTransModelView
        {
            get
            {
                if (!_invTransModelViewOk)
                {
                    _invTransModelView = float4x4.Invert(TransModelView);
                    _invTransModelViewOk = true;
                }
                return _invTransModelView;
            }
        }


        /// <summary>
        /// The inverse transpose of the Projection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion and transpose applied to the Projection matrix.
        /// </value>
        /// <remarks>
        /// If the Projection matrix is orthogonal (i.e. contains no scale component), its inverse transpose matrix
        /// is the same as the original Projection matrix.
        /// </remarks>
        /// <seealso cref="Projection"/>
        /// <seealso cref="InvProjection"/>
        /// <seealso cref="TransProjection"/>
        public float4x4 InvTransProjection
        {
            get
            {
                if (!_invTransProjectionOk)
                {
                    _invTransProjection = float4x4.Invert(TransProjection);
                    _invTransProjectionOk = true;
                }
                return _invTransProjection;
            }
        }


        /// <summary>
        /// The inverse transpose of the ModelViewProjection matrix.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix inversion and transpose applied to the ModelViewProjection matrix.
        /// </value>
        /// <remarks>
        /// If the ModelViewProjection matrix is orthogonal (i.e. contains no scale component), its inverse transpose matrix
        /// is the same as the original ModelViewProjection matrix.
        /// </remarks>
        /// <seealso cref="ModelViewProjection"/>
        /// <seealso cref="InvModelViewProjection"/>
        /// <seealso cref="TransModelViewProjection"/>
        public float4x4 InvTransModelViewProjection
        {
            get
            {
                if (!_invTransModelViewProjectionOk)
                {
                    _invTransModelViewProjection = float4x4.Invert(TransModelViewProjection);
                    _invTransModelViewProjectionOk = true;
                }
                return _invTransModelViewProjection;
            }
        }

        #endregion

        /// <summary>
        /// Array of bone matrices.
        /// </summary>
        public float4x4[] Bones
        {
            get { return _bones; }
            set
            {
                _bones = value;
                UpdateCurrentShader();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="rci">The <see cref="IRenderContextImp"/>.</param>
        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            DefaultState = new RenderContextDefaultState();

            View = DefaultState.View;
            Model = float4x4.Identity;
            Projection = DefaultState.Projection;

            // mesh management
            _meshManager = new MeshManager(_rci);

            // texture management
            _textureManager = new TextureManager(_rci);

            _shaderEffectManager = new ShaderEffectManager(this);

            _currentShaderMatrixParams = new MatrixParams();
            _updatedShaderParams = false;
        }

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
        public void Clear(ClearFlags flags)
        {
            _rci.Clear(flags);
        }

        /// <summary>
        /// Sets the rectangular output region within the output buffer(s).
        /// </summary>
        /// <param name="x">leftmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="y">topmost pixel of the rectangular output region within the output buffer.</param>
        /// <param name="width">horizontal size (in pixels) of the output region.</param>
        /// <param name="height">vertical size (in pixels) of the output region.</param>       
        /// <remarks>
        /// Setting the Viewport limits the rendering output to the specified rectangular region.
        /// </remarks>
        public void Viewport(int x, int y, int width, int height)
        {
            if (ViewportXStart == x && ViewportYStart == y && ViewportWidth == width && ViewportHeight == height)
                return;

            _rci.Scissor(x, y, width, height);
            _rci.Viewport(x, y, width, height);

            ViewportWidth = width;
            ViewportHeight = height;
            ViewportXStart = x;
            ViewportYStart = y;

        }

        #region Image Data related methods

        /// <summary>
        /// Updates a rectangular region of a given Texture (dstTexture) by copying a rectangular block from another texture (srcTexture).
        /// </summary>
        /// <param name="dstTexture">This Textures region will be updated.</param>
        /// <param name="srcTexture">This is the source from which the region will be copied.</param>
        /// <param name="startX">x offset in pixels.</param>
        /// <param name="startY">y offset in pixels.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        internal void UpdateTextureRegion(Texture dstTexture, Texture srcTexture, int startX, int startY, int width, int height)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandleFromTexture(dstTexture);
            _rci.UpdateTextureRegion(textureHandle, srcTexture, startX, startY, width, height);
        }

        /// <summary>
        /// Free all allocated gpu memory that belongs to a frame-buffer object.
        /// </summary>
        /// <param name="bufferHandle">The platform dependent abstraction of the gpu buffer handle.</param>
        internal void DeleteFrameBuffer(IBufferHandle bufferHandle)
        {
            _rci.DeleteFrameBuffer(bufferHandle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belongs to a render-buffer object.
        /// </summary>
        /// <param name="bufferHandle">The platform dependent abstraction of the gpu buffer handle.</param>
        internal void DeleteRenderBuffer(IBufferHandle bufferHandle)
        {
            _rci.DeleteRenderBuffer(bufferHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamTexture(IShaderParam param, Texture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamWritableTexture(IShaderParam param, WritableTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetWritableTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="textures">A texture array.</param>
        private void SetShaderParamWritableTextureArray(IShaderParam param, WritableTexture[] textures)
        {
            var texHandles = new List<ITextureHandle>();
            foreach (var tex in textures)
            {
                ITextureHandle textureHandle = _textureManager.GetWritableTextureHandleFromTexture(tex);
                texHandles.Add(textureHandle);
            }
            _rci.SetShaderParamTextureArray(param, texHandles.ToArray());
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamWritableCubeMap(IShaderParam param, WritableCubeMap texture)
        {
            ITextureHandle textureHandle = _textureManager.GetWritableCubeMapHandleFromTexture(texture);
            _rci.SetShaderParamCubeTexture(param, textureHandle);
        }

        #endregion

        #region Shader related methods

        /// <summary>
        /// Activates the passed shader effect as the current shader for geometry rendering.
        /// Will compile a shader by calling <see cref="IRenderContextImp.CreateShaderProgram(string, string, string)"/> if it hasn't been yet.
        /// </summary>
        /// <param name="ef">The shader effect use.</param>
        /// <remarks>A ShaderEffect must be attached to a context before you can render geometry with it. The main
        /// task performed in this method is compiling the provided shader source code and uploading the shaders to
        /// the gpu.</remarks>
        public void SetShaderEffect(ShaderEffect ef)
        {
            if (_rci == null)
                throw new ArgumentNullException("rc", "must pass a valid render context.");

            if (ef == null)
                return;

            // Is this shader effect already built?
            if (_shaderEffectManager.GetShaderEffect(ef) != null)
            {
                _currentShaderEffect = ef;
                return;
            }

            int i = 0, nPasses = ef.VertexShaderSrc.Length;

            var compiledEffect = new CompiledShaderEffect
            {
                ShaderPrograms = new ShaderProgram[nPasses]
            };

            try // to compile all the shaders
            {
                for (i = 0; i < nPasses; i++)
                {
                    var shaderOnGpu = _rci.CreateShaderProgram(ef.VertexShaderSrc[i], ef.PixelShaderSrc[i], ef.GeometryShaderSrc[i]);
                    var shaderParams = _rci.GetShaderParamList(shaderOnGpu).ToDictionary(info => info.Name, info => info);
                    compiledEffect.ShaderPrograms[i] = new ShaderProgram(shaderParams, shaderOnGpu);
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Error("Error while compiling shader for pass ", ex, new string[] { ef.VertexShaderSrc[0], ef.PixelShaderSrc[0] });
                throw new Exception("Error while compiling shader for pass " + i, ex);
            }

            _allCompiledShaderEffects.Add(ef, compiledEffect);

            CreateAllShaderEffectVariables(ef);

            // register built shader effect
            _shaderEffectManager.RegisterShaderEffect(ef);

            // register this shader effect as current shader
            _currentShaderEffect = ef;
        }

        /// <summary>
        /// Sets global effect parameters.
        /// Overwrites values with the same name in current ShaderEffect
        /// </summary>
        /// <param name="name">Effect parameter name.</param>
        /// <param name="value">Effect parameter value.</param>        
        internal void SetGlobalEffectParam(string name, object value)
        {
            if (GlobalFXParams.ContainsKey(name))
            {
                if (GlobalFXParams[name].Equals(value)) return; // no new value

                GlobalFXParams[name] = value;
            }
            else
                GlobalFXParams.Add(name, value);

            // Update ShaderEffect
            _currentShaderEffect.SetEffectParam(name, value);
        }                

        /// <summary>
        /// Called on effect param changed event. Should ONLY be used by the <see cref="ShaderEffectManager"/>!
        /// </summary>
        internal void CreateOrUpdateParameter(ShaderEffect ef, string name, object paramValue)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out CompiledShaderEffect compiledEffect)) throw new ArgumentException("ShaderEffect isn't build yet!");

            for (int i = 0; i < compiledEffect.ParamsPerPass.Count; i++)
            {
                var passParams = compiledEffect.ParamsPerPass[i];
                if (passParams.ContainsKey(name))
                    passParams[name].Value = paramValue;
                else
                {
                    var info = compiledEffect.ShaderPrograms[i].GetShaderParamInfo(name);
                    var newParam = new EffectParam()
                    {
                        Info = info,
                        Value = paramValue
                    };
                    passParams.Add(name, newParam);
                }
            }
        }

        /// <summary>
        /// Removes given shader program from GPU. Should ONLY be used by the <see cref="ShaderEffectManager"/>!
        /// </summary>
        /// <param name="ef">The ShaderEffect.</param>
        internal void RemoveShader(ShaderEffect ef)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out CompiledShaderEffect sFxParam)) return;

            foreach (var program in sFxParam.ShaderPrograms)
            {
                _rci.RemoveShader(program.GpuHandle);
            }
        }

        /// <summary>
        /// Activates the passed shader program as the current shader for rendering.
        /// </summary>
        /// <param name="program">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>       
        private void SetShaderProgram(ShaderProgram program)
        {
            _updatedShaderParams = false;

            if (_currentShaderProgram != program)
            {
                _currentShaderProgram = program;
                _rci.SetShader(program.GpuHandle);
            }

            UpdateShaderParams();
        }

        /// <summary>
        /// Gets the <see cref="CompiledShaderEffect"/> from the RC's dictionary and creates all effect parameters. 
        /// </summary>
        /// <param name="ef">The ShaderEffect the parameters are created for.</param>
        private void CreateAllShaderEffectVariables(ShaderEffect ef)
        {
            int nPasses = ef.VertexShaderSrc.Length;
            
            if (!_allCompiledShaderEffects.TryGetValue(ef, out var compiledEffect))
                throw new ArgumentException("ShaderEffect isn't build yet!");

            // Enumerate all shader parameters of all passes and enlist them in lookup tables
            for (var i = 0; i < nPasses; i++)
            {
                if (compiledEffect.ParamsPerPass.Count <= i)
                    compiledEffect.ParamsPerPass.Add(new Dictionary<string, EffectParam>());

                foreach (var paramNew in compiledEffect.ShaderPrograms[i].ParamInfosByName)
                {
                    if (ef.ParamDecl.TryGetValue(paramNew.Key, out object initValue))
                    {
                        if (initValue == null)
                            continue;

                        // OVERWRITE VARS WITH GLOBAL FXPARAMS
                        if (GlobalFXParams.TryGetValue(paramNew.Key, out object globalFXValue))
                        {
                            if (!initValue.Equals(globalFXValue))
                            {
                                // Diagnostics.Debug($"Global Overwrite {paramNew.Name},  with {globalFXValue}");

                                initValue = globalFXValue;
                                // update var in ParamDecl
                                ef.ParamDecl[paramNew.Key] = globalFXValue;
                            }
                        }

                        // IsAssignableFrom the boxed initValue object will cause JSIL to give an answer based on the value of the contents
                        // If the type originally was float but contains an integral value (e.g. 3), JSIL.GetType() will return Integer...
                        // Thus for primitive types (float, int, ) we hack a check ourselves. For other types (float2, ..) IsAssignableFrom works well.

                        // ReSharper disable UseMethodIsInstanceOfType
                        // ReSharper disable OperatorIsCanBeUsed
                        var initValType = initValue.GetType();

                        if (!(((paramNew.Value.Type == typeof(int) || paramNew.Value.Type == typeof(float))
                                  &&
                                  (initValType == typeof(int) || initValType == typeof(float) || initValType == typeof(double))
                                )
                                ||
                                (paramNew.Value.Type.IsAssignableFrom(initValType))
                             )
                            && (!paramNew.Key.Contains("BONES") && !paramNew.Key.Contains("[0]"))
                        )
                        {
                            throw new Exception("Error preparing effect pass " + i + ". Shader parameter " + paramNew.Value.Type.ToString() + " " + paramNew.Key +
                                                " was defined as " + initValType.ToString() + " " + paramNew.Key + " during initialization (different types).");
                        }
                        // ReSharper restore OperatorIsCanBeUsed
                        // ReSharper restore UseMethodIsInstanceOfType

                        // Parameter was declared by user and type is correct in shader - carry on.
                        EffectParam paramExisting;
                        if (compiledEffect.Parameters.TryGetValue(paramNew.Key, out object paramExistingTmp))
                        {
                            paramExisting = (EffectParam)paramExistingTmp;
                            // The parameter is already there from a previous pass.
                            if (paramExisting.Info.Size != paramNew.Value.Size || paramExisting.Info.Type != paramNew.Value.Type)
                            {
                                // This should never happen due to the previous error check. Check it anyway...
                                throw new Exception("Error preparing effect pass " + i + ". Shader parameter " +
                                                    paramNew.Key +
                                                    " already defined with a different type in effect pass");
                            }                            
                        }
                        else
                        {
                            paramExisting = new EffectParam()
                            {
                                Info = paramNew.Value,
                                Value = initValue
                            };
                            compiledEffect.Parameters.Add(paramNew.Key, paramExisting);
                        }
                        //sFxParam.ParamsPerPass[i].Add(paramExisting);
                        if (!compiledEffect.ParamsPerPass[i].ContainsKey(paramExisting.Info.Name))
                            compiledEffect.ParamsPerPass[i].Add(paramExisting.Info.Name, paramExisting);

                    }
                    else
                    {
                        // This should not happen due to shader compiler optimization
                        Diagnostics.Warn($"Uniform variable {paramNew.Key} found but no value is given. Please add this variable to ParamDecl of current ShaderEffect.");
                    }
                }
            }
        }

        /// <summary>
        /// Sets the shaderParam, works with every type.
        /// </summary>
        /// <param name="param"></param>
        private void SetShaderParamT(EffectParam param)
        {
            if (param.Info.Type == typeof(int))
            {
                _rci.SetShaderParam(param.Info.Handle, (int)param.Value);
            }
            else if (param.Info.Type == typeof(float))
            {
                _rci.SetShaderParam(param.Info.Handle, (float)param.Value);
            }
            else if (param.Info.Type == typeof(float2))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float2[])param.Value;
                    _rci.SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                _rci.SetShaderParam(param.Info.Handle, (float2)param.Value);
            }
            else if (param.Info.Type == typeof(float3))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float3[])param.Value;
                    _rci.SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                _rci.SetShaderParam(param.Info.Handle, (float3)param.Value);
            }
            else if (param.Info.Type == typeof(float4))
            {
                _rci.SetShaderParam(param.Info.Handle, (float4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float4x4[])param.Value;
                    _rci.SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                _rci.SetShaderParam(param.Info.Handle, (float4x4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4[]))
            {
                _rci.SetShaderParam(param.Info.Handle, (float4x4[])param.Value);
            }
            else if (param.Value is IWritableCubeMap)
            {
                SetShaderParamWritableCubeMap(param.Info.Handle, ((WritableCubeMap)param.Value));
            }
            else if (param.Value is IWritableTexture[])
            {
                SetShaderParamWritableTextureArray(param.Info.Handle, (WritableTexture[])param.Value);
            }
            else if (param.Value is IWritableTexture)
            {
                SetShaderParamWritableTexture(param.Info.Handle, ((WritableTexture)param.Value));
            }
            else if (param.Value is ITexture)
            {
                SetShaderParamTexture(param.Info.Handle, (Texture)param.Value);
            }

        }       

        /// <summary>
        /// Checks the matrix cache and updates the found parameters in the _currentShaderEffect.
        /// This method is called whenever a base matrix (model, view or projection is changed).
        /// The derived matrices are updated due to the use of the Getters of the derived matrices here.
        /// </summary>
        private void UpdateCurrentShader()
        {
            if (_currentShaderProgram == null)
                return;

            if (!_updatedShaderParams)
                UpdateShaderParams();

            // Normal versions of MV and P
            if (_currentShaderMatrixParams.FUSEE_M != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.Model, Model);

            if (_currentShaderMatrixParams.FUSEE_V != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.View, View);

            if (_currentShaderMatrixParams.FUSEE_IV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IView, InvView);

            if (_currentShaderMatrixParams.FUSEE_MV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelView, ModelView);

            if (_currentShaderMatrixParams.FUSEE_P != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.Projection, Projection);

            if (_currentShaderMatrixParams.FUSEE_MVP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelViewProjection, ModelViewProjection);

            // Inverted versions
            // Todo: Add inverted versions for M and V
            if (_currentShaderMatrixParams.FUSEE_IMV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelView, InvModelView);

            if (_currentShaderMatrixParams.FUSEE_ITV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITView, InvTransView);

            if (_currentShaderMatrixParams.FUSEE_IP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IProjection, InvProjection);

            if (_currentShaderMatrixParams.FUSEE_IMVP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelViewProjection, InvModelViewProjection);

            // Transposed versions
            // Todo: Add transposed versions for M and V
            if (_currentShaderMatrixParams.FUSEE_TMV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModelView, TransModelView);

            if (_currentShaderMatrixParams.FUSEE_TP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TProjection, TransProjection);

            if (_currentShaderMatrixParams.FUSEE_TMVP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelViewProjection, TransModelViewProjection);

            // Inverted and transposed versions
            // Todo: Add inverted & transposed versions for M and V
            if (_currentShaderMatrixParams.FUSEE_ITMV != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelView, InvTransModelView);

            if (_currentShaderMatrixParams.FUSEE_ITP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITProjection, InvTransProjection);

            if (_currentShaderMatrixParams.FUSEE_ITMVP != null)
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelViewProjection, InvTransModelViewProjection);

            // Bones (if any)
            if (_currentShaderMatrixParams.FUSEE_BONES != null && Bones != null)
                SetGlobalEffectParam("FUSEE_BONES[0]", Bones);

        }

        private void UpdateShaderParams()
        {
            if (_currentShaderProgram == null)
            {
                Diagnostics.Warn("No shader is currently set! Use RenderContext.SetShader first!");
                return;
            }

            // Normal versions of MV and P
            _currentShaderMatrixParams.FUSEE_M = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.Model);
            _currentShaderMatrixParams.FUSEE_V = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.View);
            _currentShaderMatrixParams.FUSEE_MV = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.ModelView);
            _currentShaderMatrixParams.FUSEE_P = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.Projection);
            _currentShaderMatrixParams.FUSEE_MVP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.ModelViewProjection);

            // Inverted versions
            _currentShaderMatrixParams.FUSEE_IMV = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.IModelView);
            _currentShaderMatrixParams.FUSEE_IP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.IProjection);
            _currentShaderMatrixParams.FUSEE_IMVP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.IModelViewProjection);
            _currentShaderMatrixParams.FUSEE_IV = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.IView);

            // Transposed versions
            _currentShaderMatrixParams.FUSEE_TMV = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.TModelView);
            _currentShaderMatrixParams.FUSEE_TP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.TProjection);
            _currentShaderMatrixParams.FUSEE_TMVP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.TModelViewProjection);

            // Inverted and transposed versions
            _currentShaderMatrixParams.FUSEE_ITMV = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.ITModelView);
            _currentShaderMatrixParams.FUSEE_ITP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.ITProjection);
            _currentShaderMatrixParams.FUSEE_ITMVP = _currentShaderProgram.GetShaderParam(ShaderShards.UniformNameDeclarations.ITModelViewProjection);

            // Bones
            _currentShaderMatrixParams.FUSEE_BONES = _currentShaderProgram.GetShaderParam("FUSEE_BONES[0]");

            _updatedShaderParams = true;
            UpdateCurrentShader();
        }
        #endregion

        #region Render related methods

        /// <summary>
        /// The clipping behavior against the Z position of a vertex can be turned off by activating depth clamping. 
        /// This is done with glEnable(GL_DEPTH_CLAMP). This will cause the clip-space Z to remain unclipped by the front and rear viewing volume.
        /// See: https://www.khronos.org/opengl/wiki/Vertex_Post-Processing#Depth_clamping
        /// </summary>
        public void EnableDepthClamp()
        {
            _rci.EnableDepthClamp();
        }

        /// <summary>
        /// Disables depths clamping. <seealso cref="EnableDepthClamp"/>
        /// </summary>
        public void DisableDepthClamp()
        {
            _rci.DisableDepthClamp();
        }

        /// <summary>
        /// Returns the hardware capabilities.
        /// </summary>
        /// <param name="capability"></param>
        /// <returns></returns>
        public uint GetHardwareCapabilities(HardwareCapability capability)
        {
            return _rci.GetHardwareCapabilities(capability);
        }

        /// <summary>
        /// Returns a human readable description of the underlying graphics hardware
        /// </summary>
        /// <returns></returns>
        public string GetHardwareDescription()
        {
            return _rci.GetHardwareDescription();
        }

        /// <summary>
        /// Unlocks the given <see cref="RenderState"/>. And sets it to the value it had before it was locked.
        /// After this call the state can be set to a new value again.
        /// </summary>
        /// <param name="state">The state to unlock.</param>
        /// <param name="resetValue">True by default. Defines if the state gets reset to its pre-locked value.</param>
        public void UnlockRenderState(RenderState state, bool resetValue = true)
        {
            if (LockedStates.ContainsKey(state))
            {
                var resetToVal = LockedStates[state].Value;
                LockedStates[state] = new KeyValuePair<bool, uint>(false, resetToVal);

                if (resetValue)
                    SetRenderState(state, resetToVal);
            }
        }

        /// <summary>
        /// Unlocks all previously locked <see cref="RenderState"/>s.
        /// <param name="resetValue">True by default. Defines if the states get reset to their pre-locked value.</param>
        /// </summary>
        public void UnlockAllRenderStates(bool resetValue = true)
        {
            if (LockedStates.Count == 0) return;

            for (int i = 0; i < LockedStates.Count; i++)
                UnlockRenderState(LockedStates.ElementAt(i).Key, resetValue);
        }

        /// <summary>
        /// Apply a single render state to the render context. All subsequent rendering will be
        /// performed using the currently set state unless it is changed to a different value.
        /// </summary>
        /// <param name="renderState">One of the <see cref="RenderState"/> enumeration values.</param>
        /// <param name="value">An unsigned integer value representing the value the state should be set to.
        ///  Depending on the renderState, this value can be interpreted as an integer value, a float value, a
        /// boolean value, or even a color.  </param>
        /// <param name="doLockState">Forces this state to have the given value and locks the state. Unlock it by calling <see cref="UnlockRenderState(RenderState, bool)"/></param>
        /// <remarks>This method is close to the underlying implementation layer and might be awkward to use
        /// due to the ambiguity of the value parameter type. If you want type-safe state values and also 
        /// want to set a couple of states at the same time, try the more 
        /// elaborate <see cref="SetRenderStateSet(RenderStateSet, bool)"/> method.</remarks>
        public void SetRenderState(RenderState renderState, uint value, bool doLockState = false)
        {
            if (LockedStates.TryGetValue(renderState, out var lockedState))
            {
                if (lockedState.Key)
                {
                    if (doLockState)
                    {
                        CurrentRenderState.SetRenderState(renderState, value);
                        _rci.SetRenderState(renderState, value);
                        Diagnostics.Warn("PREVIOUSLY LOCKED STATE WAS OVERWRITTEN: Render state " + renderState + " was locked and will remain its old value.\n Call UnlockRenderState(renderState) to undo it.");
                    }
                    else
                        Diagnostics.Warn("Render state " + renderState + " was locked and will remain its old value.\n Call UndoLockRenderState(renderState) to undo it.");

                    return;
                }
            }

            var currentVal = CurrentRenderState.GetRenderState(renderState);
            if (currentVal != value)
            {
                if (doLockState)
                {
                    if (currentVal != null)
                        LockedStates[renderState] = new KeyValuePair<bool, uint>(true, (uint)currentVal);
                    else
                        LockedStates[renderState] = new KeyValuePair<bool, uint>(true, (uint)RenderStateSet.Default.GetRenderState(renderState));
                }

                CurrentRenderState.SetRenderState(renderState, value);
                _rci.SetRenderState(renderState, value);
            }
        }

        /// <summary>
        /// Apply a number of render states to this render context. All subsequent rendering will be
        /// performed using the currently set state set unless one of its values it is changed. Use this 
        /// method to change more than one render state at once. 
        /// </summary>
        /// <param name="renderStateSet">A set of render states with their respective values to be set.</param>
        /// <param name="doLockState">Forces all states that are set in this <see cref="RenderStateSet"/> to have the given value and locks them. Unlock them by calling <see cref="UnlockRenderState(RenderState, bool)"/></param>
        public void SetRenderStateSet(RenderStateSet renderStateSet, bool doLockState = false)
        {
            foreach (var state in renderStateSet.States)
            {
                SetRenderState(state.Key, state.Value, doLockState);
            }
        }

        /// <summary>
        /// Returns a current render state.
        /// </summary>
        /// <param name="renderState"></param>
        /// <returns></returns>
        public uint GetRenderState(RenderState renderState)
        {
            var currentState = CurrentRenderState.GetRenderState(renderState);

            if (currentState != null)
                return (uint)currentState;
            else
                return (uint)RenderStateSet.Default.GetRenderState(renderState);
        }

        /// <summary>
        /// Sets the RenderTarget, if texture is null render target is the main screen, otherwise the picture will be rendered onto given texture
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        public void SetRenderTarget(RenderTarget renderTarget = null)
        {
            ITextureHandle[] texHandles = null;
            if (renderTarget != null)
            {
                texHandles = new ITextureHandle[renderTarget.RenderTextures.Length];

                for (int i = 0; i < renderTarget.RenderTextures.Length; i++)
                {
                    var tex = renderTarget.RenderTextures[i];
                    if (renderTarget.RenderTextures[i] == null) continue;
                    texHandles[i] = _textureManager.GetWritableTextureHandleFromTexture((WritableTexture)tex);
                }
            }

            _rci.SetRenderTarget(renderTarget, texHandles);
        }

        /// <summary>
        ///  Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(IWritableTexture tex)
        {
            var texHandle = _textureManager.GetWritableTextureHandleFromTexture((WritableTexture)tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(IWritableCubeMap tex)
        {
            var texHandle = _textureManager.GetWritableCubeMapHandleFromTexture((WritableCubeMap)tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        /// Specifies the rasterized width of both aliased and antialiased lines.
        /// </summary>
        /// <param name="width">The width in px.</param>
        public void SetLineWidth(float width)
        {
            _rci.SetLineWidth(width);
        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="m">The mesh that should be rendered.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        internal void Render(Mesh m)
        {
            if (_currentShaderEffect == null) return;

            // Update global shader parameters in current shader (light and matrices)
            foreach (var fxParam in GlobalFXParams)            
                _currentShaderEffect.SetEffectParam(fxParam.Key, fxParam.Value);            

            int i = 0, nPasses = _currentShaderEffect.VertexShaderSrc.Length;
            try
            {
                for (i = 0; i < nPasses; i++)
                {
                    var compiledShaderEffect = _allCompiledShaderEffects[_currentShaderEffect];
                    
                    SetShaderProgram(compiledShaderEffect.ShaderPrograms[i]);

                    // TODO: Use shared uniform parameters - currently SetShader will query the shader params and set all the common uniforms (like matrices and light)
                    foreach (var param in compiledShaderEffect.ParamsPerPass[i])
                        SetShaderParamT(param.Value);

                    SetRenderStateSet(_currentShaderEffect.States[i]);

                    // TODO: split up RenderContext.Render into a preparation and a draw call so that we can prepare a mesh once and draw it for each pass.
                    var meshImp = _meshManager.GetMeshImpFromMesh(m);
                    _rci.Render(meshImp);
                }

                // After rendering always cleanup pending meshes
                _meshManager.Cleanup();
                _textureManager.Cleanup();

                // After rendering all passes cleanup shader effect
                _shaderEffectManager.Cleanup();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while rendering pass " + i, ex);
            }
        }      

        #endregion        

        /// <summary>
        /// Resets the RenderContexts View, Projection and Viewport to the values defined in <see cref="DefaultState"/>.
        /// Must be called after every visitation of the Scene Graph that changed these values.
        /// </summary>
        internal void ResetToDefaultRenderContextState()
        {
            Viewport(0, 0, DefaultState.CanvasWidth, DefaultState.CanvasHeight);
            View = DefaultState.View;
            Projection = DefaultState.Projection;
        }
    }

    internal sealed class MatrixParams
    {
        // ReSharper disable InconsistentNaming
        public IShaderParam FUSEE_M;
        public IShaderParam FUSEE_V;
        public IShaderParam FUSEE_MV;

        public IShaderParam FUSEE_P;
        public IShaderParam FUSEE_MVP;

        public IShaderParam FUSEE_IV;
        public IShaderParam FUSEE_IMV;
        public IShaderParam FUSEE_ITV;
        public IShaderParam FUSEE_IP;
        public IShaderParam FUSEE_IMVP;

        public IShaderParam FUSEE_TMV;
        public IShaderParam FUSEE_TP;
        public IShaderParam FUSEE_TMVP;

        public IShaderParam FUSEE_ITMV;
        public IShaderParam FUSEE_ITP;
        public IShaderParam FUSEE_ITMVP;

        public IShaderParam FUSEE_BONES;
        // ReSharper restore InconsistentNaming
    };

    internal sealed class EffectParam
    {
        public ShaderParamInfo Info;
        public object Value;
    }

    /// <summary>
    /// All compiled information of one ShaderEffect.
    /// A <see cref="ShaderEffect"/> can have more than one Pass where each pass contains another shader.
    /// Shaders that where created on the gpu are saved as <see cref="ShaderProgram"/>.
    /// </summary>
    internal class CompiledShaderEffect
    {
        /// <summary>
        /// The compiled vertex and pixel shaders for every pass.
        /// </summary>
        internal ShaderProgram[] ShaderPrograms;

        /// <summary>
        /// All parameter saved per pass with uniform handle, type and info (name, etc.) as lookup table
        /// </summary>
        internal List<Dictionary<string, EffectParam>> ParamsPerPass = new List<Dictionary<string, EffectParam>>();

        /// <summary>
        /// All shader parameters of all passes
        /// </summary>
        internal Dictionary<string, object> Parameters = new Dictionary<string, object>();

    }

    /// <summary>
    /// After every Render call the values are reset to the ones saved here.
    /// This ensures that we do not necessarily need a Camera in the Scene Graph.
    /// The viewport width and height is updated with every resize.
    /// </summary>
    public class RenderContextDefaultState
    {
        /// <summary>
        /// This value should be equal to the window/canvas width and is set at every resize.
        /// If this value is changed the default Projection matrix is recalculated.
        /// </summary>
        public int CanvasWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                _aspect = (float)_width / _height;
                if (_aspect != 0)
                    Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
            }
        }

        /// <summary>
        /// This value should be equal to the window/canvas height and is set at every resize.
        /// If this value is changed the default Projection matrix is recalculated.
        /// </summary>
        public int CanvasHeight
        {
            get { return _height; }
            set
            {
                _height = value;
                _aspect = (float)_width / _height;
                if (_aspect != 0)
                    Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
            }
        }

        /// <summary>
        /// The view matrix.
        /// </summary>
        public readonly float4x4 View = float4x4.Identity;

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public float4x4 Projection { get; private set; }

        /// <summary>
        /// The default distance to the near clipping plane.
        /// </summary>
        public readonly float ZNearDefautlt = 0.1f;

        /// <summary>
        /// The default distance to the far clipping plane.
        /// </summary>
        public readonly float ZFarDefault = 3000;

        /// <summary>
        /// The default distance field of view.
        /// </summary>
        public readonly float FovDefault = M.DegreesToRadians(45);

        private int _height = 9;
        private int _width = 16;
        private float _aspect;

        /// <summary>
        /// Creates a new instance of type RenderContextDefaultState.
        /// </summary>
        public RenderContextDefaultState()
        {
            _aspect = (float)_width / _height;
            Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
        }
    }

}