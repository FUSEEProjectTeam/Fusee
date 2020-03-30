using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
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
            set => _rci.ClearColor = value;
            get => _rci.ClearColor;
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
            set => _rci.ClearDepth = value;
            get => _rci.ClearDepth;
        }

        /// <summary>
        /// Contains the default state of the render context. can be used to reset this RenderContext to it's DefaultState.
        /// </summary>
        public RenderContextDefaultState DefaultState { get; private set; }

        /// <summary>
        /// The world space frustum planes, derived from the current view-projection matrix.
        /// </summary>
        public Frustum RenderFrustum { get; private set; }


        /// <summary>
        /// Saves all global shader parameters. "Global" are those which get updated by a SceneRenderer, e.g. the matrices or the parameters of the lights.
        /// </summary>
        internal readonly Dictionary<string, object> GlobalFXParams;

        private readonly MeshManager _meshManager;
        private readonly TextureManager _textureManager;

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
            get => _view;
            set
            {
                _view = value;

                _modelViewOK = false;
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

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.View, _view);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelView, ModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelViewProjection, ModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IView, InvView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelView, InvModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelViewProjection, InvModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITView, InvTransView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelView, InvTransModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelViewProjection, InvTransModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TView, TransView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModelView, TransModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModelViewProjection, TransModelViewProjection);

                var invZMat = float4x4.Identity;
                invZMat.M33 = -1;
                RenderFrustum.CalculateFrustumPlanes(_projection * View);
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
            get => _model;
            set
            {
                _model = value;

                _modelViewOK = false;
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

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.Model, _model);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelView, ModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelViewProjection, ModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModel, InvModel);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelView, InvModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IModelViewProjection, InvModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModel, InvTransModel);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelView, InvTransModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITModelViewProjection, InvTransModelViewProjection);

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModel, TransModel);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModelView, TransModelView);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TModelViewProjection, TransModelViewProjection);
            }
        }

        /// <summary>
        /// The projection matrix used by the rendering pipeline
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry with the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        public float4x4 Projection
        {
            get => _projection;
            set
            {
                // Update matrix
                _projection = value;

                // Invalidate derived matrices
                _modelViewProjectionOk = false;
                _invProjectionOk = false;
                _invTransProjectionOk = false;
                _transProjectionOk = false;

                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.Projection, _projection);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ModelViewProjection, ModelViewProjection);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.IProjection, InvProjection);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.ITProjection, InvTransProjection);
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.TProjection, TransProjection);

                var invZMat = float4x4.Identity;
                invZMat.M33 = -1;
                RenderFrustum.CalculateFrustumPlanes(_projection * View);
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
                {
                    _modelView = View * Model;
                    _modelViewOK = true;
                }
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
        /// <see cref="ModelView"/> and <see cref="Projection"/>.
        /// </remarks>
        public float4x4 ModelViewProjection
        {
            get
            {
                if (!_modelViewProjectionOk)
                {
                    // Column order notation
                    _modelViewProjection = Projection * ModelView;
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
        /// <seealso cref="View"/>
        /// <seealso cref="TransView"/>
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
        /// <seealso cref="Model"/>
        /// <seealso cref="TransModel"/>
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
        /// <seealso cref="ModelView"/>
        /// <seealso cref="TransModelView"/>
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
        /// <seealso cref="Projection"/>
        /// <seealso cref="TransProjection"/>
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
        /// <seealso cref="ModelViewProjection"/>
        /// <seealso cref="TransModelViewProjection"/>
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
        /// <seealso cref="View"/>
        /// <seealso cref="InvView"/>
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
        /// <seealso cref="Model"/>
        /// <seealso cref="InvModel"/>
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
        /// <seealso cref="ModelView"/>
        /// <seealso cref="InvModelView"/>
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
        /// <seealso cref="Projection"/>
        /// <seealso cref="InvProjection"/>
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
        /// <seealso cref="ModelViewProjection"/>
        /// <seealso cref="InvModelViewProjection"/>
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
            get => _bones;
            set
            {
                _bones = value;
                SetGlobalEffectParam(ShaderShards.UniformNameDeclarations.BonesArray, _bones);
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
            GlobalFXParams = new Dictionary<string, object>();

            RenderFrustum = new Frustum();

            View = DefaultState.View;
            Model = float4x4.Identity;
            Projection = DefaultState.Projection;

            // mesh management
            _meshManager = new MeshManager(_rci);

            // texture management
            _textureManager = new TextureManager(_rci);

            _shaderEffectManager = new ShaderEffectManager(this);
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
            var textureHandle = _textureManager.GetTextureHandleFromTexture(dstTexture);
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
            var textureHandle = _textureManager.GetTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.TEXTURE2D);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamWritableTexture(IShaderParam param, WritableTexture texture)
        {
            var textureHandle = _textureManager.GetWritableTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.TEXTURE2D);
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
                var textureHandle = _textureManager.GetWritableTextureHandleFromTexture(tex);
                texHandles.Add(textureHandle);
            }
            var handlesAsArray = texHandles.ToArray();
            _rci.SetShaderParamTextureArray(param, handlesAsArray, TextureType.TEXTURE2D);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamWritableCubeMap(IShaderParam param, WritableCubeMap texture)
        {
            var textureHandle = _textureManager.GetWritableCubeMapHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.TEXTURE_CUBE_MAP);
        }

        #endregion

        #region Shader related methods

        /// <summary>
        /// Activates the passed shader effect as the current shader for geometry rendering.
        /// Will compile a shader by calling <see cref="IRenderContextImp.CreateShaderProgram(string, string, string)"/> if it hasn't been compiled yet.
        /// </summary>
        /// <param name="ef">The shader effect.</param>
        /// <remarks>A ShaderEffect must be attached to a context before you can render geometry with it. The main
        /// task performed in this method is compiling the provided shader source code and uploading the shaders to
        /// the gpu.</remarks>
        public void SetShaderEffect(ShaderEffect ef)
        {
            if (_rci == null)
                throw new NullReferenceException("No render context Implementation found!");

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

            //Minimal list of uniforms of the shader source code over all ShaderEffect passes
            var activeUniforms = new Dictionary<string, ShaderParamInfo>();

            try // to compile all the shaders
            {
                for (i = 0; i < nPasses; i++)
                {
                    var shaderOnGpu = _rci.CreateShaderProgram(ef.VertexShaderSrc[i], ef.PixelShaderSrc[i], ef.GeometryShaderSrc[i]);
                    var shaderParams = _rci.GetShaderParamList(shaderOnGpu).ToDictionary(info => info.Name, info => info);

                    foreach (var param in shaderParams)
                    {
                        if (!activeUniforms.ContainsKey(param.Key))
                            activeUniforms.Add(param.Key, param.Value);
                    }

                    compiledEffect.ShaderPrograms[i] = new ShaderProgram(shaderParams, shaderOnGpu);
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Error("Error while compiling shader for pass ", ex, new string[] { ef.VertexShaderSrc[0], ef.PixelShaderSrc[0] });
                throw new Exception("Error while compiling shader for pass " + i, ex);
            }

            _allCompiledShaderEffects.Add(ef, compiledEffect);

            // register built shader effect
            _shaderEffectManager.RegisterShaderEffect(ef);

            CreateAllShaderEffectVariables(ef, activeUniforms);

            // register this shader effect as current shader
            _currentShaderEffect = ef;
        }

        /// <summary>
        /// Gets the <see cref="CompiledShaderEffect"/> from the RC's dictionary and creates all effect parameters. 
        /// </summary>
        /// <param name="ef">The ShaderEffect the parameters are created for.</param>
        /// <param name="activeUniforms">The active uniform parameters, as they are saved in the source shader on the gpu.</param>
        private void CreateAllShaderEffectVariables(ShaderEffect ef, Dictionary<string, ShaderParamInfo> activeUniforms)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out var compiledEffect))
                throw new ArgumentException("ShaderEffect isn't build yet - no compiled effect found!");

            if (compiledEffect.Parameters.Count != 0)
                throw new ArgumentException("The compiled effect already has parameters!");

            for (var i = 0; i < compiledEffect.ShaderPrograms.Length; i++)
                compiledEffect.ParamsPerPass.Add(new Dictionary<string, EffectParam>());

            //Iterate source shader's active uniforms and create a EffectParam for each one.
            foreach (var param in activeUniforms)
            {
                if (!ef.ParamDecl.TryGetValue(param.Key, out var initialValue))
                {
                    Diagnostics.Error(initialValue, new NullReferenceException("Found uniform declaration in source shader that doesn't have a corresponding Parameter Declaration in the ShaderEffect!"));
                    continue;
                }

                var effectParam = new EffectParam()
                {
                    Info = param.Value
                };

                // Set the initial values as they are saved in the "globals" list
                if (GlobalFXParams.TryGetValue(param.Key, out var globalFXValue))
                    effectParam.Value = globalFXValue;
                else
                    effectParam.Value = initialValue;

                compiledEffect.Parameters.Add(param.Key, effectParam);

                //For each pass (== ShaderProgram) add the same(!) EffectParam to the ParamsPerPass Dictionary
                for (var i = 0; i < compiledEffect.ShaderPrograms.Length; i++)
                {
                    var shaderProgram = compiledEffect.ShaderPrograms[i];
                    if (shaderProgram.ParamsByName.ContainsKey(param.Key))
                        compiledEffect.ParamsPerPass[i].Add(param.Key, effectParam);
                }
            }
        }

        /// <summary>
        /// Sets global effect parameters by updating or adding them in the GlobalFXParams list.
        /// Changes will only have an effect when rendering.
        /// </summary>
        /// <param name="name">Effect parameter name.</param>
        /// <param name="value">Effect parameter value.</param>        
        internal void SetGlobalEffectParam(string name, object value)
        {
            if (GlobalFXParams.TryGetValue(name, out var currentValue))
            {
                if (currentValue.Equals(value)) return; // no new value
                GlobalFXParams[name] = value;
            }
            else if (value != null)
            {
                GlobalFXParams.Add(name, value);
            }
        }

        /// <summary>
        /// Called from the <see cref="ShaderEffect.ShaderEffectChanged"/> event. Will lookup the CompiledShaderEffect and change the value of the parameter there.
        /// </summary>
        /// <param name="ef">The ShaderEffect.</param>
        /// <param name="name">The parameter's name</param>
        /// <param name="paramValue">The parameter's value.</param>
        internal void UpdateParameterInCompiledEffect(ShaderEffect ef, string name, object paramValue)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out var compiledEffect)) throw new ArgumentException("ShaderEffect isn't build yet!");

            //We only need to look the parameter in the "all" parameters collection because EffectParam is a reference type.
            //Because of this we do not need to take about which passes this effect belongs to.
            if (compiledEffect.Parameters.TryGetValue(name, out var effectParam))
            {
                effectParam.Value = paramValue;
                effectParam.HasValueChanged = true;
            }
            else
            {
                Diagnostics.Warn($"Parameter {name} is declared in ShaderEffect but currently not used by the shader.");
            }
        }

        /// <summary>
        /// Removes given shader program from GPU. Should ONLY be used by the <see cref="ShaderEffectManager"/>!
        /// </summary>
        /// <param name="ef">The ShaderEffect.</param>
        internal void RemoveShader(ShaderEffect ef)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out var sFxParam)) return;

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
            if (_currentShaderProgram != program)
            {
                _currentShaderProgram = program;
                _rci.SetShader(program.GpuHandle);
            }
        }

        /// <summary>
        /// Sets the shaderParam, works with every type.
        /// </summary>
        /// <param name="param"></param>
        private void SetShaderParamT(EffectParam param)
        {
            if (param.HasValueChanged)
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
                        // parameter is an array
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
                        // parameter is an array
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
                        // parameter is an array
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
            else
            {
                if (param.Value is ITextureBase)
                {
                    if (param.Value is IWritableCubeMap)
                    {
                        var textureHandle = _textureManager.GetWritableCubeMapHandleFromTexture((WritableCubeMap)param.Value);
                        _rci.SetActiveAndBindTexture(param.Info.Handle, textureHandle, TextureType.TEXTURE_CUBE_MAP);
                    }
                    else if (param.Value is IWritableTexture)
                    {
                        var textureHandle = _textureManager.GetWritableTextureHandleFromTexture((WritableTexture)param.Value);
                        _rci.SetActiveAndBindTexture(param.Info.Handle, textureHandle, TextureType.TEXTURE2D);
                    }
                    else if (param.Value is ITexture)
                    {
                        var textureHandle = _textureManager.GetTextureHandleFromTexture((Texture)param.Value);
                        _rci.SetActiveAndBindTexture(param.Info.Handle, textureHandle, TextureType.TEXTURE2D);
                    }
                    else if (param.Value is IWritableTexture[])
                    {
                        foreach (var tex in (WritableTexture[])param.Value)
                        {
                            var textureHandle = _textureManager.GetWritableTextureHandleFromTexture(tex);
                            _rci.SetActiveAndBindTexture(param.Info.Handle, textureHandle, TextureType.TEXTURE2D);
                        }
                    }
                }
            }

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

            for (var i = 0; i < LockedStates.Count; i++)
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
                    {
                        Diagnostics.Warn("Render state " + renderState + " was locked and will remain its old value.\n Call UndoLockRenderState(renderState) to undo it.");
                    }

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

                for (var i = 0; i < renderTarget.RenderTextures.Length; i++)
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
        /// <param name="width">The width in pixel.</param>
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

            var compiledShaderEffect = _allCompiledShaderEffects[_currentShaderEffect];

            for (var i = 0; i < compiledShaderEffect.ShaderPrograms.Length; i++)
            {
                try
                {
                    SetShaderProgram(compiledShaderEffect.ShaderPrograms[i]);
                    SetRenderStateSet(_currentShaderEffect.States[i]);

                    foreach (var paramItem in compiledShaderEffect.ShaderPrograms[i].ParamsByName)
                    {
                        if (!_currentShaderEffect.ParamDecl.TryGetValue(paramItem.Key, out var currentValue))
                        {
                            Diagnostics.Error(currentValue, new NullReferenceException("Found uniform declaration in source shader that doesn't have a corresponding Parameter Declaration in the ShaderEffect!"));
                            continue;
                        }

                        // OVERWRITE Values in the ShaderEffect with the newest ones from the GlobalFXParams collection.
                        if (GlobalFXParams.TryGetValue(paramItem.Key, out var globalFXValue))
                        {
                            if (!currentValue.Equals(globalFXValue)) //TODO: does NOT work for matrices some times because of rounding (?) errors
                                _currentShaderEffect.SetEffectParam(paramItem.Key, globalFXValue);
                        }

                        var param = compiledShaderEffect.ParamsPerPass[i][paramItem.Key];
                        SetShaderParamT(param);
                        param.HasValueChanged = false;
                    }

                    // TODO: split up RenderContext.Render into a preparation and a draw call so that we can prepare a mesh once and draw it for each pass.
                    var meshImp = _meshManager.GetMeshImpFromMesh(m);
                    _rci.Render(meshImp);

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
}