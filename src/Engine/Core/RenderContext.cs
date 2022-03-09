using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
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
    public class RenderContext : IDisposable
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
            set => _rci.ClearColor = float4.SRgbFromLinearColor(value);
            get => float4.LinearColorFromSRgb(_rci.ClearColor);
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
        public FrustumF RenderFrustum { get; private set; }


        /// <summary>
        /// Saves all global shader parameters. "Global" are those which get updated by a SceneRenderer, e.g. the matrices or the parameters of the lights.
        /// </summary>
        internal readonly Dictionary<int, GlobalFxParam> GlobalFXParams;

        private readonly MeshManager _meshManager;
        private readonly TextureManager _textureManager;
        private bool _disposed;

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

        private readonly EffectManager _effectManager;
        private readonly Dictionary<Effect, CompiledEffects> _allCompiledEffects = new();

        /// <summary>
        /// The default <see cref="Effect"/>, that is used if a <see cref="SceneNode"/> has a mesh but no effect.
        /// </summary>
        public SurfaceEffectBase DefaultEffect;

        /// <summary>
        /// The currently used <see cref="Effect"/> is set in <see cref="SetEffect(Effect, bool)"/>.
        /// </summary>
        private Effect _currentEffect;

        /// <summary>
        /// The currently bound shader program.
        /// </summary>
        private IShaderHandle _currentShaderProgram;

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
                if (_view == value) return;
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

                SetGlobalEffectParam(UniformNameDeclarations.View, UniformNameDeclarations.ViewHash, _view);
                SetGlobalEffectParam(UniformNameDeclarations.ModelView, UniformNameDeclarations.ModelViewHash, ModelView);
                SetGlobalEffectParam(UniformNameDeclarations.ModelViewProjection, UniformNameDeclarations.ModelViewProjectionHash, ModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.IView, UniformNameDeclarations.IViewHash, InvView);
                SetGlobalEffectParam(UniformNameDeclarations.IModelView, UniformNameDeclarations.IModelViewHash, InvModelView);
                SetGlobalEffectParam(UniformNameDeclarations.IModelViewProjection, UniformNameDeclarations.IModelViewProjectionHash, InvModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.ITView, UniformNameDeclarations.ITViewHash, InvTransView);
                SetGlobalEffectParam(UniformNameDeclarations.ITModelView, UniformNameDeclarations.ITModelViewHash, InvTransModelView);
                SetGlobalEffectParam(UniformNameDeclarations.ITModelViewProjection, UniformNameDeclarations.ITModelViewProjectionHash, InvTransModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.TView, UniformNameDeclarations.TViewHash, TransView);
                SetGlobalEffectParam(UniformNameDeclarations.TModelView, UniformNameDeclarations.TModelViewHash, TransModelView);
                SetGlobalEffectParam(UniformNameDeclarations.TModelViewProjection, UniformNameDeclarations.TModelViewProjectionHash, TransModelViewProjection);

                var invZMat = float4x4.Identity;
                invZMat.M33 = -1;
                RenderFrustum.CalculateFrustumPlanes(_projection * _view);
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
                if (_model == value) return;

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

                SetGlobalEffectParam(UniformNameDeclarations.Model, UniformNameDeclarations.ModelHash, _model);
                SetGlobalEffectParam(UniformNameDeclarations.ModelView, UniformNameDeclarations.ModelViewHash, ModelView);
                SetGlobalEffectParam(UniformNameDeclarations.ModelViewProjection, UniformNameDeclarations.ModelViewProjectionHash, ModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.IModel, UniformNameDeclarations.IModelHash, InvModel);
                SetGlobalEffectParam(UniformNameDeclarations.IModelView, UniformNameDeclarations.IModelViewHash, InvModelView);
                SetGlobalEffectParam(UniformNameDeclarations.IModelViewProjection, UniformNameDeclarations.IModelViewProjectionHash, InvModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.ITModel, UniformNameDeclarations.ITModelHash, InvTransModel);
                SetGlobalEffectParam(UniformNameDeclarations.ITModelView, UniformNameDeclarations.ITModelViewHash, InvTransModelView);
                SetGlobalEffectParam(UniformNameDeclarations.ITModelViewProjection, UniformNameDeclarations.ITModelViewProjectionHash, InvTransModelViewProjection);

                SetGlobalEffectParam(UniformNameDeclarations.TModel, UniformNameDeclarations.TModelHash, TransModel);
                SetGlobalEffectParam(UniformNameDeclarations.TModelView, UniformNameDeclarations.TModelViewHash, TransModelView);
                SetGlobalEffectParam(UniformNameDeclarations.TModelViewProjection, UniformNameDeclarations.TModelViewProjectionHash, TransModelViewProjection);
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
                if (_projection == value) return;
                // Update matrix
                _projection = value;

                // Invalidate derived matrices
                _modelViewProjectionOk = false;
                _invProjectionOk = false;
                _invTransProjectionOk = false;
                _transProjectionOk = false;

                SetGlobalEffectParam(UniformNameDeclarations.Projection, UniformNameDeclarations.ProjectionHash, _projection);
                SetGlobalEffectParam(UniformNameDeclarations.ModelViewProjection, UniformNameDeclarations.ModelViewProjectionHash, ModelViewProjection);
                SetGlobalEffectParam(UniformNameDeclarations.IProjection, UniformNameDeclarations.IProjectionHash, InvProjection);
                SetGlobalEffectParam(UniformNameDeclarations.ITProjection, UniformNameDeclarations.ITProjectionHash, InvTransProjection);
                SetGlobalEffectParam(UniformNameDeclarations.TProjection, UniformNameDeclarations.TProjectionHash, TransProjection);

                var invZMat = float4x4.Identity;
                invZMat.M33 = -1;
                RenderFrustum.CalculateFrustumPlanes(_projection * View);
                SetGlobalEffectParam(UniformNameDeclarations.ClippingPlanes, UniformNameDeclarations.ClippingPlanesHash, CalculateClippingPlanesFromProjection());
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
                SetGlobalEffectParam(UniformNameDeclarations.BonesArray, UniformNameDeclarations.BonesArrayHash, _bones);
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
            DefaultEffect = MakeEffect.Default();
            GlobalFXParams = new Dictionary<int, GlobalFxParam>();

            SetGlobalEffectParam(UniformNameDeclarations.FuseePlatformId, UniformNameDeclarations.FuseePlatformIdHash, (int)_rci.FuseePlatformId);

            RenderFrustum = new FrustumF();

            View = DefaultState.View;
            Model = float4x4.Identity;
            Projection = DefaultState.Projection;

            // mesh, texture and effect management
            _meshManager = new MeshManager(_rci);
            _textureManager = new TextureManager(_rci);
            _effectManager = new EffectManager(this);

            ModuleExtensionPoint.CreateGpuMesh = CreateGpuMesh;
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
            SetGlobalEffectParam(UniformNameDeclarations.ViewportPx, UniformNameDeclarations.ViewportPxHash, new int2(width, height));
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
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(dstTexture);
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
        private void SetShaderParamTexture(IUniformHandle param, Texture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.Texture2D);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamImage(IUniformHandle param, WritableTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamImage(param, textureHandle, TextureType.Image2D, texture.PixelFormat);

        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamTexture(IUniformHandle param, WritableTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.Texture2D);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="textures">A texture array.</param>
        private void SetShaderParamWritableTextureArray(IUniformHandle param, WritableTexture[] textures)
        {
            var texHandles = new List<ITextureHandle>();
            foreach (var tex in textures)
            {
                ITextureHandle textureHandle = _textureManager.GetTextureHandle(tex);
                texHandles.Add(textureHandle);
            }
            var handlesAsArray = texHandles.ToArray();
            _rci.SetShaderParamTextureArray(param, handlesAsArray, TextureType.Texture2D);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamTexture(IUniformHandle param, WritableCubeMap texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.TextureCubeMap);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamTexture(IUniformHandle param, WritableArrayTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.ArrayTexture);
        }

        private void ConnectBufferToShaderStorage(IStorageBuffer buffer, string ssboName)
        {
            _rci.ConnectBufferToShaderStorage(_currentShaderProgram, buffer, ssboName);
        }

        #endregion

        #region Shader related methods

        /// <summary>
        /// Activates the passed shader effect as the current shader for geometry rendering.
        /// Will compile a shader by calling <see cref="IRenderContextImp.CreateShaderProgram(string, string, string)"/> if it hasn't been compiled yet.
        /// </summary>
        /// <param name="ef">The effect.</param>
        /// <param name="renderForward"></param>
        /// <remarks>A Effect must be attached to a context before you can render geometry with it. The main
        /// task performed in this method is compiling the provided shader source code and uploading the shaders to
        /// the gpu.</remarks>
        public void SetEffect(Effect ef, bool renderForward = true)
        {
            if (_rci == null)
                throw new NullReferenceException("No render context Implementation found!");

            if (ef == null)
                throw new NullReferenceException("No Effect found!");

            // Is this shader effect already built?
            if (_effectManager.GetEffect(ef) == null)
            {
                CreateShaderProgram(ef, renderForward);
            }
            _currentEffect = ef;
            return;
        }

        /// <summary>
        /// Creates a shader program on the gpu. Needs to be called before <see cref="SetEffect(Effect, bool)"/>.
        /// </summary>
        /// <param name="renderForward">Is a forward or deferred renderer used? Will create the proper shader for the render method.</param>
        /// <param name="ef">The effect.</param>
        internal void CreateShaderProgram(Effect ef, bool renderForward = true)
        {
            if (ef == null)
                throw new NullReferenceException("No Effect found!");

            // Is this shader effect already built?
            var fx = _effectManager.GetEffect(ef);
            if (fx != null)
            {
                if (renderForward && _allCompiledEffects[fx].ForwardFx != null)
                    return;
                if (!renderForward && _allCompiledEffects[fx].DeferredFx != null)
                    return;
            }

            if (_rci == null)
                throw new NullReferenceException("No render context Implementation found!");

            switch (ef)
            {
                case ComputeEffect cFx:
                    CreateShaderForComputeEffect(cFx);
                    break;
                case ShaderEffect shFx:
                    CreateShaderForShaderEffect(shFx);
                    break;
                case SurfaceEffectBase surfFx:
                    if (surfFx is SurfaceEffect || surfFx is SurfaceEffectPointCloud)
                        CreateShaderForSurfaceEffect(surfFx);
                    else if (surfFx is SurfaceEffectInstanced)
                    {

                    }
                    else
                        throw new ArgumentException($"{surfFx} is of unknown type {surfFx.GetType().Name}");
                    break;
            }

            // register built shader effect
            _effectManager.RegisterEffect(ef);
        }

        private CompiledEffect CompileEffect(Effect ef, string vert, string geom, string frag)
        {
            var shaderOnGpu = _rci.CreateShaderProgram(vert, frag, geom);
            var activeUniforms = _rci.GetActiveUniformsList(shaderOnGpu).ToDictionary(info => info.Hash, info => info);

            if (activeUniforms.Count == 0)
            {
                var ex = new Exception();
                Diagnostics.Error("Error while compiling shader for pass - couldn't get parameters form the gpu!", ex, new string[] { vert, geom, frag }); ;
                throw new Exception("Error while compiling shader for pass.", ex);
            }
            
            foreach (var shaderParam in activeUniforms)
            {
                if (!ef.ParamDecl.TryGetValue(shaderParam.Key, out IFxParamDeclaration dcl))
                {
                    Diagnostics.Error(shaderParam.Value.Name, new NullReferenceException("Found uniform declaration in source shader that doesn't have a corresponding Parameter Declaration in the Effect!"));
                    continue;
                }

                shaderParam.Value.Value = dcl.GetType().GetField("Value").GetValue(dcl);
            }

            var compiledEffect = new CompiledEffect
            {
                GpuHandle = shaderOnGpu,
                ActiveUniforms = activeUniforms
            };

            return compiledEffect;
        }

        private void CreateShaderForShaderEffect(ShaderEffect ef)
        {
            var vert = ef.VertexShaderSrc;
            var geom = ef.GeometryShaderSrc;
            var frag = ef.PixelShaderSrc;

            var compiledEffect = CompileEffect(ef, vert, geom, frag);

            var cFx = new CompiledEffects() { ForwardFx = compiledEffect };
            _allCompiledEffects.Add(ef, cFx);
        }

        private void CreateShaderForComputeEffect(ComputeEffect ef)
        {
            var cs = ef.ComputeShaderSrc;

            var shaderOnGpu = _rci.CreateShaderProgramCompute(cs);
            var activeUniforms = _rci.GetActiveUniformsList(shaderOnGpu).ToDictionary(info => info.Hash, info => info);

            var shaderStorageBuffers = _rci.GetShaderStorageBufferList(shaderOnGpu).ToDictionary(info => info.Hash, info => info);

            if (activeUniforms.Count == 0)
            {
                var ex = new Exception();
                Diagnostics.Error("Error while compiling shader for pass - couldn't get parameters form the gpu!", ex, new string[] { cs }); ;
                throw new Exception("Error while compiling shader for pass.", ex);
            }

            var compiledEffect = new CompiledEffect
            {
                GpuHandle = shaderOnGpu,
                ActiveUniforms = activeUniforms
            };
            var cFx = new CompiledEffects() { ForwardFx = compiledEffect };
            _allCompiledEffects.Add(ef, cFx);
        }

        private void CreateShaderForSurfaceEffect(SurfaceEffectBase ef)
        {
            var renderDependentShardsForward = new List<KeyValuePair<ShardCategory, string>>(5);
            var renderDependentShardsDeferred = new List<KeyValuePair<ShardCategory, string>>(3);

            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Method, ShaderShards.Fragment.Lighting.AssembleLightingMethods(ef.SurfaceInput.ShadingModel)));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.ForwardLighting(ef.SurfaceInput.ShadingModel, nameof(ef.SurfaceInput), SurfaceOut.StructName)));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.Lighting.LightStructDeclaration));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.FixedNumberLightArray));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.ColorOut()));

            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.GBufferOut()));
            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Method, ShaderShards.Fragment.Lighting.ColorManagementMethods()));
            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.RenderToGBuffer(ef.SurfaceInput.ShadingModel, nameof(ef.SurfaceInput), SurfaceOut.StructName)));

            string vert = SurfaceEffectBase.JoinShards(ef.VertexShaderSrc);
            string geom = SurfaceEffectBase.JoinShards(ef.GeometryShaderSrc);

            //Forward
            string frag = SurfaceEffectBase.JoinShards(ef.FragmentShaderSrc, renderDependentShardsForward);
            var compiledForward = CompileEffect(ef, vert, geom, frag);
            var compiledEffects = new CompiledEffects() { ForwardFx = compiledForward };

            //Deferred
            frag = SurfaceEffectBase.JoinShards(ef.FragmentShaderSrc, renderDependentShardsDeferred);
            var compiledDeferred = CompileEffect(ef, vert, geom, frag);
            compiledEffects.DeferredFx = compiledDeferred;

            _allCompiledEffects.Add(ef, compiledEffects);
        }

        /// <summary>
        /// Sets global effect parameters by updating or adding them in the GlobalFXParams list.
        /// Changes will only have an effect when rendering.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="hash">Effect parameter hash (generated from its name).</param>
        /// <param name="value">Effect parameter value.</param>
        public void SetGlobalEffectParam(string name, int hash, object value)
        {
            if (GlobalFXParams.TryGetValue(hash, out var param))
            {
                if (param.Value == value) return; // no new value
                param.Value = value;
                param.HasValueChanged = true;
            }
            else if (value != null)
            {
                param = new GlobalFxParam()
                {
                    Name = name,
                    Value = value,
                    HasValueChanged = true
                };
                GlobalFXParams.Add(hash, param);
            }

            GlobalFXParams[hash] = param;
        }

        internal void ClearGlobalEffectParamsDirtyFlag()
        {
            foreach (var key in GlobalFXParams.Keys)
            {
                var param = GlobalFXParams[key];
                param.HasValueChanged = false;
                GlobalFXParams[key] = param;
            }
        }

        /// <summary>
        /// Called from the <see cref="Effect.EffectChanged"/> event. Will lookup the CompiledEffect and change the value of the parameter there.
        /// </summary>
        /// <param name="ef">The Effect.</param>
        /// <param name="hash">The parameter's hash (generated from its name).</param>
        /// <param name="paramValue">The parameter's value.</param>
        internal void UpdateParameterInCompiledEffect(Effect ef, int hash, object paramValue)
        {
            var compiledEffects = _allCompiledEffects[ef];
            var forwardFx = compiledEffects.ForwardFx;
            if (forwardFx != null)
            {
                if (forwardFx.ActiveUniforms.TryGetValue(hash, out var effectParamFw))
                {
                    effectParamFw.Value = paramValue;
                    effectParamFw.HasValueChanged = true;
                }
            }

            var deferredFx = compiledEffects.DeferredFx;
            if (deferredFx != null)
            {
                if (deferredFx.ActiveUniforms.TryGetValue(hash, out var effectParamDf))
                {
                    effectParamDf.Value = paramValue;
                    effectParamDf.HasValueChanged = true;
                }
            }
        }

        /// <summary>
        /// Removes given shader program from GPU. Should ONLY be used by the <see cref="EffectManager"/>!
        /// </summary>
        /// <param name="ef">The Effect.</param>
        internal void RemoveShader(Effect ef)
        {
            var compiledEffects = _allCompiledEffects[ef];
            _allCompiledEffects.Remove(ef);

            if (compiledEffects.ForwardFx != null)
                _rci.RemoveShader(compiledEffects.ForwardFx?.GpuHandle);

            if (compiledEffects.DeferredFx != null)
                _rci.RemoveShader(compiledEffects.DeferredFx?.GpuHandle);
        }

        private void UpdateAllActiveFxParams(CompiledEffect cFx)
        {
            foreach (var fxParam in cFx.ActiveUniforms.Values)
            {
                if (fxParam.Value == null) continue;
                SetShaderParamT(in fxParam);
                fxParam.HasValueChanged = false;
            }
        }

        private void SetGlobalParamsInCurrentFx(CompiledEffect cFx)
        {
            foreach (var key in GlobalFXParams.Keys)
            {
                var globalFxParam = GlobalFXParams[key];

                if (cFx.ActiveUniforms.TryGetValue(key, out var activeParam))
                {
                    if (globalFxParam.HasValueChanged || globalFxParam.Value != activeParam.Value)
                        activeParam.Value = globalFxParam.Value;
                }
            }
        }

        private CompiledEffect GetCompiledFxForRenderMethod(bool renderForward)
        {
            var compiledEffect = _allCompiledEffects[_currentEffect];
            return renderForward ? compiledEffect.ForwardFx : compiledEffect.DeferredFx;
        }

        /// <summary>
        /// Activates the passed shader program as the current shader for rendering.
        /// </summary>
        /// <param name="program">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        private void SetCompiledFx(IShaderHandle program)
        {
            if (_currentShaderProgram != program)
            {
                _currentShaderProgram = program;
                _rci.SetShader(program);
            }
        }

        /// <summary>
        /// Sets the value for the given shader parameter, works with every type.
        /// Note that this will change the parameter value in the currently bound shader.
        /// </summary>
        /// <param name="param">The shader parameter.</param>
        private void SetShaderParamT(in IFxParam param)
        {
            if (param.HasValueChanged)
            {
                if (param.Value is bool boolVal)
                {
                    _rci.SetShaderParam(param.Handle, boolVal ? 1 : 0);
                }
                else if (param.Value is int intVal)
                {
                    _rci.SetShaderParam(param.Handle, in intVal);
                }
                else if (param.Value is float floatVal)
                {
                    _rci.SetShaderParam(param.Handle, in floatVal);
                }
                else if (param.Value is double doubleVal)
                {
                    _rci.SetShaderParam(param.Handle, in doubleVal);
                }
                else if (param.Value is float2 float2Val)
                {
                    _rci.SetShaderParam(param.Handle, in float2Val);
                }
                else if (param.Value is float3 float3Val)
                {
                    _rci.SetShaderParam(param.Handle, in float3Val);
                }
                else if (param.Value is float4 float4Val)
                {
                    _rci.SetShaderParam(param.Handle, in float4Val);
                }
                else if (param.Value is float4x4 float4x4Val)
                {
                    _rci.SetShaderParam(param.Handle, float4x4Val);
                }
                else if (param.Value is int2 int2Val)
                {
                    _rci.SetShaderParam(param.Handle, in int2Val);
                }

                else if (param.Value is IWritableArrayTexture)
                {
                    SetShaderParamTexture(param.Handle, (WritableArrayTexture)param.Value);
                }
                else if (param.Value is IWritableCubeMap)
                {
                    SetShaderParamTexture(param.Handle, ((WritableCubeMap)param.Value));
                }
                else if (param.Value is IWritableTexture[])
                {
                    SetShaderParamWritableTextureArray(param.Handle, (WritableTexture[])param.Value);
                }
                else if (param.Value is IWritableTexture)
                {
                    var wt = ((WritableTexture)param.Value);
                    if (wt.AsImage)
                        SetShaderParamImage(param.Handle, wt);
                    else
                        SetShaderParamTexture(param.Handle, wt);
                }
                else if (param.Value is ITexture)
                {
                    SetShaderParamTexture(param.Handle, (Texture)param.Value);
                }
                else if (param.Value is IStorageBuffer buffer)
                {
                    ConnectBufferToShaderStorage(buffer, param.Name);
                }

                else if (param.Value is float4x4[] float4x4ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float4x4ArrayVal);
                }
                else if (param.Value is float2[] float2ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float2ArrayVal);
                }
                else if (param.Value is float3[] float3ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float3ArrayVal);
                }
                else if (param.Value is float4[] float4ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float4ArrayVal);
                }

                else
                {
                    throw new ArgumentException($"{param} has an unknown type {param.Value.GetType().Name}.");
                }
            }
            else
            {
                if (param.Value is ITextureBase)
                {
                    if (param.Value is IWritableArrayTexture)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableArrayTexture)param.Value);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.ArrayTexture);
                    }
                    else if (param.Value is IWritableCubeMap)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableCubeMap)param.Value);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.TextureCubeMap);
                    }
                    else if (param.Value is IWritableTexture)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableTexture)param.Value);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture2D);
                    }
                    else if (param.Value is ITexture)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((Texture)param.Value);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture2D);
                    }
                    else if (param.Value is IWritableTexture[])
                    {
                        foreach (var tex in (WritableTexture[])param.Value)
                        {
                            ITextureHandle textureHandle = _textureManager.GetTextureHandle(tex);
                            _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture2D);
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"{param} has a unknown type.");
                    }
                }
            }
        }

        #endregion

        #region Render related methods

        /// <summary>
        /// Creates a <see cref="IRenderTarget"/> with the purpose of being used as CPU GBuffer representation.
        /// </summary>
        /// <param name="res">The texture resolution.</param>
        public IRenderTarget CreateGBufferTarget(TexRes res)
        {
            return _rci.CreateGBufferTarget(res);
        }

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
                    }

                    return;
                }
            }

            var currentVal = CurrentRenderState.GetRenderState(renderState);
            if (doLockState)
            {
                LockedStates[renderState] = new KeyValuePair<bool, uint>(true, (uint)currentVal);
            }
            if (currentVal != value)
            {
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
            return CurrentRenderState.GetRenderState(renderState);
        }

        /// <summary>
        /// Sets the RenderTarget, if texture is null render target is the main screen, otherwise the picture will be rendered onto given texture
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        public void SetRenderTarget(IRenderTarget renderTarget = null)
        {
            ITextureHandle[] texHandles = null;
            if (renderTarget != null)
            {
                texHandles = new ITextureHandle[renderTarget.RenderTextures.Length];

                for (var i = 0; i < renderTarget.RenderTextures.Length; i++)
                {
                    var tex = renderTarget.RenderTextures[i];
                    if (renderTarget.RenderTextures[i] == null) continue;
                    texHandles[i] = _textureManager.GetTextureHandle((WritableTexture)tex);
                }
            }

            _rci.SetRenderTarget(renderTarget, texHandles);
        }

        /// <summary>
        ///  Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        /// <param name="layer">The layer of the array texture that is set as render target.</param>
        public void SetRenderTarget(IWritableArrayTexture tex, int layer)
        {
            var texHandle = _textureManager.GetTextureHandle((WritableArrayTexture)tex);
            _rci.SetRenderTarget(tex, layer, texHandle);
        }

        /// <summary>
        ///  Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(IWritableTexture tex)
        {
            var texHandle = _textureManager.GetTextureHandle((WritableTexture)tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(IWritableCubeMap tex)
        {
            var texHandle = _textureManager.GetTextureHandle((WritableCubeMap)tex);
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
        /// Defines a barrier ordering memory transactions. At the moment it will insert all supported barriers.
        /// </summary>
        public void MemoryBarrier()
        {
            _rci.MemoryBarrier();
        }

        /// <summary>
        /// Launch the bound Compute Shader Program.
        /// </summary>
        /// <param name="kernelIndex"></param>
        /// <param name="threadGroupsX">The number of work groups to be launched in the X dimension.</param>
        /// <param name="threadGroupsY">The number of work groups to be launched in the Y dimension.</param>
        /// <param name="threadGroupsZ">he number of work groups to be launched in the Z dimension.</param>
        public void DispatchCompute(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
        {
            if (_currentEffect == null) throw new NullReferenceException("No Compute Shader bound.");
            if (_currentEffect.GetType() != typeof(ComputeEffect)) throw new NullReferenceException("Bound Effect isn't a Compute Shader.");

            var cFx = GetCompiledFxForRenderMethod(true);
            SetCompiledFx(cFx.GpuHandle);
            SetRenderStateSet(_currentEffect.RendererStates);
            SetGlobalParamsInCurrentFx(cFx);
            UpdateAllActiveFxParams(cFx);

            _rci.DispatchCompute(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);

            // After rendering always cleanup pending meshes, textures and shader effects
            _meshManager.Cleanup();
            _textureManager.Cleanup();
            _effectManager.Cleanup();

        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh that should be rendered.</param>
        /// <param name="doRenderForward">Is a forward or deferred renderer used? Will fetch the proper shader for the render method.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        public void Render(Mesh mesh, InstanceData instanceData, bool doRenderForward = true)
        {
            var cFx = GetCompiledFxForRenderMethod(doRenderForward);
            SetCompiledFx(cFx.GpuHandle);
            SetRenderStateSet(_currentEffect.RendererStates);
            SetGlobalParamsInCurrentFx(cFx);
            UpdateAllActiveFxParams(cFx);

            var meshImp = _meshManager.GetMeshImpFromMesh(mesh);
            _rci.Render(meshImp);

            // After rendering always cleanup pending meshes, textures and shader effects
            _meshManager.Cleanup();
            _textureManager.Cleanup();
            _effectManager.Cleanup();
        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh that should be rendered.</param>
        /// <param name="doRenderForward">Is a forward or deferred renderer used? Will fetch the proper shader for the render method.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        public void Render(Mesh mesh, bool doRenderForward = true)
        {
            var cFx = GetCompiledFxForRenderMethod(doRenderForward);
            SetCompiledFx(cFx.GpuHandle);
            SetRenderStateSet(_currentEffect.RendererStates);
            SetGlobalParamsInCurrentFx(cFx);
            UpdateAllActiveFxParams(cFx);

            var meshImp = _meshManager.GetMeshImpFromMesh(mesh);
            _rci.Render(meshImp);

            // After rendering always cleanup pending meshes, textures and shader effects
            _meshManager.Cleanup();
            _textureManager.Cleanup();
            _effectManager.Cleanup();
        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh that should be rendered.</param>
        /// <param name="doRenderForward">Is a forward or deferred renderer used? Will fetch the proper shader for the render method.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        public void Render(GpuMesh mesh, bool doRenderForward = true)
        {
            var cFx = GetCompiledFxForRenderMethod(doRenderForward);
            SetCompiledFx(cFx.GpuHandle);
            SetRenderStateSet(_currentEffect.RendererStates);
            SetGlobalParamsInCurrentFx(cFx);
            UpdateAllActiveFxParams(cFx);

            var meshImp = _meshManager.GetMeshImpFromMesh(mesh);
            _rci.Render(meshImp);

            // After rendering always cleanup pending meshes, textures and shader effects
            _meshManager.Cleanup();
            _textureManager.Cleanup();
            _effectManager.Cleanup();
        }

        private float2 CalculateClippingPlanesFromProjection()
        {
            var C = Projection.M33;
            var D = Projection.M34;
            float f = D / (C - 1.0f) * -1;
            float n = D / (C + 1.0f) * -1;
            return new float2(n, f);
        }

        /// <summary>
        /// Creates a platform specific <see cref="IMeshImp"/>.
        /// </summary>
        /// <returns></returns>
        public IMeshImp CreateMeshImp()
        {
            return _rci.CreateMeshImp();
        }

        /// <summary>
        /// Creates a <see cref="GpuMesh"/>, registers it in the <see cref="MeshManager"/> and uploads the data to the gpu.
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <param name="vertices">The vertex data of the mesh.</param>
        /// <param name="triangles">The triangle indices of the mesh.</param>
        /// <param name="normals">The normal vectors of the mesh.</param>
        /// <param name="colors">The first color set of the mesh.</param>
        /// <param name="colors1">The second color set of the mesh.</param>
        /// <param name="colors2">The third color set of the mesh.</param>
        /// <param name="uvs">The uv coordinates of the mesh.</param>
        /// <param name="tangents">The tangent vectors of the mesh.</param>
        /// <param name="bitangents">The bitangent vectors of the mesh.</param>
        /// <param name="boneIndices">The bone indices of the mesh.</param>
        /// <param name="boneWeights">The bone weights of the mesh.</param>
        /// <returns></returns>
        public GpuMesh CreateGpuMesh(PrimitiveType primitiveType, float3[] vertices, ushort[] triangles = null,
            float3[] normals = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null, float2[] uvs = null,
            float4[] tangents = null, float3[] bitangents = null, float4[] boneIndices = null, float4[] boneWeights = null)
        {
            var mesh = new GpuMesh
            {
                MeshType = primitiveType,
                BoundingBox = new AABBf(vertices)
            };
            _meshManager.RegisterNewMesh(mesh, vertices, triangles, uvs,
            normals, colors, colors1, colors2,
            tangents, bitangents, boneIndices, boneWeights);
            return mesh;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                _effectManager.Dispose();
                _textureManager.Dispose();
                _meshManager.Dispose();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~RenderContext()
        {
            Dispose(disposing: false);
        }
    }
}