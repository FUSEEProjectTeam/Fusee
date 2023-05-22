using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fusee.ImGuiImp.Desktop")]
namespace Fusee.Engine.Core
{
    internal struct GlobalUniform
    {
        /// <summary>
        /// Delegate that points to a method that can return the uniform value.
        /// </summary>
        public GetUniformValue Getter;

        /// <summary>
        /// The name of the uniform parameter.r
        /// </summary>
        public string Name;

        /// <summary>
        /// Uniform arrays that contain structs (eg. the "allLights" array) are a special case
        /// because every field of the struct needs to have its own Getter but only there is only one uniform parameter declared in the shader code.
        /// Therefore these getters cannot be used to generate the uniform declaration in glsl.
        /// </summary>
        public bool IsStructArray;
    }

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

        private readonly MeshManager _meshManager;
        private readonly TextureManager _textureManager;

        /// <summary>
        /// Saves all global shader parameters. "Global" are those which get updated by a SceneRenderer, e.g. the matrices or the parameters of the lights.
        ///</summary>
        internal Dictionary<int, GlobalUniform> GlobalUniforms;

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

        /// <summary>
        /// Gets the window width.
        /// </summary>
        public Func<int> GetWindowWidth { get; internal set; }

        /// <summary>
        /// Sets the window width.
        /// </summary>
        public Func<int> GetWindowHeight { get; internal set; }

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
        internal IShaderHandle CurrentShaderProgram;

        #endregion

        #region Matrix backing fields and flags

        // Settable matrices
        private float4x4 _modelView;
        private float4x4 _projection;
        private float4x4 _view;
        private float4x4 _model;

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

                var invZMat = float4x4.Identity;
                invZMat.M33 = -1;
                RenderFrustum.CalculateFrustumPlanes(_projection * View);
                CalculateClippingPlanesFromProjection(out _clippingPlanes);
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

        private int2 _viewportInPx = int2.One;
        private float2 _clippingPlanes = float2.One;

        /// <summary>
        /// Global Uniform array of <see cref="LightResult"/>s. Updated by a SceneRenderer.
        /// </summary>
        public LightResult[] ForwardLights = new LightResult[ModuleExtensionPoint.NumberOfLightsForward];

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="rci">The <see cref="IRenderContextImp"/>.</param>
        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            ModuleExtensionPoint.CreateGpuMesh = CreateGpuMesh;
            ModuleExtensionPoint.PlatformId = _rci.FuseePlatformId;
            DefaultState = new RenderContextDefaultState();
            DefaultEffect = MakeEffect.Default();

            RenderFrustum = new FrustumF();

            View = DefaultState.View;
            Model = float4x4.Identity;
            Projection = DefaultState.Projection;

            // mesh, texture and effect management
            _meshManager = new MeshManager(_rci);
            _textureManager = new TextureManager(_rci);
            _effectManager = new EffectManager(this);

            GlobalUniforms = new()
            {
                {
                    UniformNameDeclarations.ViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.View,
                        Getter = () => View,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ModelHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.Model,
                        Getter = () => Model,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.Projection,
                        Getter = () => Projection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ModelViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ModelView,
                        Getter = () => ModelView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ModelViewProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ModelViewProjection,
                        Getter = () => ModelViewProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.IViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.IView,
                        Getter = () => InvView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.IModelHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.IModel,
                        Getter = () => InvModel,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.IModelViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.IModelView,
                        Getter = () => InvModelView,
                        IsStructArray = false
                    }
                },
                {

                    UniformNameDeclarations.IProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.IProjection,
                        Getter = () => InvProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.IModelViewProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.IModelViewProjection,
                        Getter = () => InvModelViewProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.TViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.TView,
                        Getter = () => TransView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.TModelHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.TModel,
                        Getter = () => TransModel,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.TModelViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.TModelView,
                        Getter = () => TransModelView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.TProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.TProjection,
                        Getter = () => TransProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.TModelViewProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.TModelViewProjection,
                        Getter = () => TransModelViewProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ITViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ITView,
                        Getter = () => InvTransView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ITModelHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ITModel,
                        Getter = () => InvTransModel,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ITModelViewHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ITModelView,
                        Getter = () => InvTransModelView,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ITProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ITProjection,
                        Getter = () => InvTransProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ITModelViewProjectionHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ITModelViewProjection,
                        Getter = () => InvTransModelViewProjection,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.FuseePlatformIdHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.FuseePlatformId,
                        Getter = () => (int)_rci.FuseePlatformId,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ClippingPlanesHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ClippingPlanes,
                        Getter = () => _clippingPlanes,
                        IsStructArray = false
                    }
                },
                {
                    UniformNameDeclarations.ViewportPxHash,
                    new GlobalUniform
                    {
                        Name = UniformNameDeclarations.ViewportPx,
                        Getter = () => _viewportInPx,
                        IsStructArray = false
                    }
                }
            };

            for (var i = 0; i < ModuleExtensionPoint.NumberOfLightsForward; i++)
            {
                ForwardLights[i] = new LightResult();
                AddForwardLightGetter(i);
            }


        }

        private void AddForwardLightGetter(int arrayPos)
        {
            var lightPos = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetPosName(arrayPos),
                Getter = () => View * ForwardLights[arrayPos].WorldSpacePos,
                IsStructArray = true
            };

            var lightColor = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetIntensitiesName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.Color,
                IsStructArray = true
            };

            var lightMaxDistance = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetMaxDistName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.MaxDistance,
                IsStructArray = true
            };

            var lightStrength = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetStrengthName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.Strength,
                IsStructArray = true
            };

            var lightOuterConeAngle = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetOuterConeAngleName(arrayPos),
                Getter = () => M.DegreesToRadians(ForwardLights[arrayPos].Light.OuterConeAngle),
                IsStructArray = true
            };
            var lightInnerConeAngle = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetInnerConeAngleName(arrayPos),
                Getter = () => M.DegreesToRadians(ForwardLights[arrayPos].Light.InnerConeAngle),
                IsStructArray = true
            };
            var lightDirection = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetDirectionName(arrayPos),
                Getter = () => (View * ForwardLights[arrayPos].Rotation * float4.UnitZ).xyz.Normalize(),
                IsStructArray = true
            };
            var lightType = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetTypeName(arrayPos),
                Getter = () => (int)ForwardLights[arrayPos].Light.Type,
                IsStructArray = true
            };
            var lightIsActive = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetIsActiveName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.Active ? 1 : 0,
                IsStructArray = true
            };
            var lightIsCastingShadows = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetIsCastingShadowsName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.IsCastingShadows ? 1 : 0,
                IsStructArray = true
            };
            var lightBias = new GlobalUniform
            {
                Name = UniformNameDeclarations.GetBiasName(arrayPos),
                Getter = () => ForwardLights[arrayPos].Light.Bias,
                IsStructArray = true
            };

            GlobalUniforms.Add(lightPos.Name.GetHashCode(), lightPos);
            GlobalUniforms.Add(lightColor.Name.GetHashCode(), lightColor);
            GlobalUniforms.Add(lightMaxDistance.Name.GetHashCode(), lightMaxDistance);
            GlobalUniforms.Add(lightStrength.Name.GetHashCode(), lightStrength);
            GlobalUniforms.Add(lightOuterConeAngle.Name.GetHashCode(), lightOuterConeAngle);
            GlobalUniforms.Add(lightInnerConeAngle.Name.GetHashCode(), lightInnerConeAngle);
            GlobalUniforms.Add(lightDirection.Name.GetHashCode(), lightDirection);
            GlobalUniforms.Add(lightType.Name.GetHashCode(), lightType);
            GlobalUniforms.Add(lightIsActive.Name.GetHashCode(), lightIsActive);
            GlobalUniforms.Add(lightIsCastingShadows.Name.GetHashCode(), lightIsCastingShadows);
            GlobalUniforms.Add(lightBias.Name.GetHashCode(), lightBias);
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
            _viewportInPx.x = width;
            _viewportInPx.y = height;
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
        /// This method enables an external <see cref="Texture"/> to be registered to the current <see cref="RenderContext"/>
        /// without the need to be rendered first. This procedure is needed for image rendering with ImGui
        /// </summary>
        /// <param name="tex">Texture to register</param>
        public void RegisterTexture(ExposedTexture tex)
        {
            _ = _textureManager.GetTextureHandle(tex);
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
        private void SetShaderParamTexture(IUniformHandle param, WritableMultisampleTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.TextureMultisample);
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
        /// <param name="texture">An ITexture.</param>
        private void SetShaderParamTexture(IUniformHandle param, Texture1D texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
            _rci.SetShaderParamTexture(param, textureHandle, TextureType.Texture1D);
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
            _rci.ConnectBufferToShaderStorage(CurrentShaderProgram, buffer, ssboName);
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
                    CreateShaderForSurfaceEffect(surfFx);
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

            AssignUniformGetter(ef, activeUniforms);

            var compiledEffect = new CompiledEffect
            {
                GpuHandle = shaderOnGpu,
                ActiveUniforms = activeUniforms
            };

            return compiledEffect;
        }

        private void AssignUniformGetter(Effect ef, Dictionary<int, IActiveUniform> activeUniforms)
        {
            foreach (var shaderParam in activeUniforms)
            {
                if (GlobalUniforms.TryGetValue(shaderParam.Key, out var globalUniform))
                {
                    shaderParam.Value.UniformValueGetter = globalUniform.Getter;
                    shaderParam.Value.IsGlobal = true;
                }
                else
                {
                    if (!ef.UniformParameters.TryGetValue(shaderParam.Key, out IFxParamDeclaration dcl))
                    {
                        Diagnostics.Error(shaderParam.Value.Name, new NullReferenceException("Found uniform declaration in source shader that doesn't have a corresponding Parameter Declaration in the Effect!"));
                        continue;
                    }

                    shaderParam.Value.UniformValueGetter = () => ef.UniformParameters[shaderParam.Key].GetValue();
                    shaderParam.Value.IsGlobal = false;
                }
            }
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
            foreach (var fxParam in shaderStorageBuffers)
            {
                activeUniforms.Add(fxParam.Key, fxParam.Value);
            }
            shaderStorageBuffers.Clear();

            if (activeUniforms.Count == 0)
            {
                var ex = new Exception();
                Diagnostics.Error("Error while compiling shader for pass - couldn't get parameters form the gpu!", ex, new string[] { cs }); ;
                throw new Exception("Error while compiling shader for pass.", ex);
            }

            AssignUniformGetter(ef, activeUniforms);

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
            //Add the shader code for the global uniforms
            foreach (var key in GlobalUniforms.Keys)
            {
                ShaderCategory shaderCategory = ShaderCategory.Vertex | ShaderCategory.Fragment;
                if (ef.GeometryShaderSrc.Count != 0)
                    shaderCategory |= ShaderCategory.Geometry;
                if (!GlobalUniforms[key].IsStructArray)
                    ef.HandleUniform(shaderCategory, GlobalUniforms[key].Name, GlobalUniforms[key].Getter().GetType(), ShardCategory.InternalUniform);
            }

            var renderDependentShardsForward = new List<KeyValuePair<ShardCategory, string>>(5);
            var renderDependentShardsDeferred = new List<KeyValuePair<ShardCategory, string>>(3);

            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Method, ShaderShards.Fragment.Lighting.AssembleLightingMethods(ef.SurfaceInput.ShadingModel)));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.ForwardLighting(ef.SurfaceInput.ShadingModel, nameof(ef.SurfaceInput))));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.Lighting.LightStructDeclaration));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.FixedNumberLightArray));
            renderDependentShardsForward.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.ColorOut()));

            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.GBufferOut()));
            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Method, ShaderShards.Fragment.Lighting.ColorManagementMethods()));
            renderDependentShardsDeferred.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.RenderToGBuffer(ef.SurfaceInput.ShadingModel, nameof(ef.SurfaceInput))));

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
        /// Called from the <see cref="Effect.EffectChanged"/> event. Will lookup the CompiledEffect and change the value of the parameter there.
        /// </summary>
        /// <param name="ef">The Effect.</param>
        /// <param name="hash">The parameter's hash (generated from its name).</param>
        internal void MarkShaderUniformForUpdate(Effect ef, int hash)
        {
            var compiledEffects = _allCompiledEffects[ef];
            var forwardFx = compiledEffects.ForwardFx;
            if (forwardFx != null)
            {
                if (forwardFx.ActiveUniforms.TryGetValue(hash, out var effectParamFw))
                    effectParamFw.HasValueChanged = true;
            }

            var deferredFx = compiledEffects.DeferredFx;
            if (deferredFx != null)
            {
                if (deferredFx.ActiveUniforms.TryGetValue(hash, out var effectParamDf))
                    effectParamDf.HasValueChanged = true;
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
                SetShaderParamT(in fxParam);
                fxParam.HasValueChanged = false;
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
            if (CurrentShaderProgram != program)
            {
                CurrentShaderProgram = program;
                _rci.SetShader(program);
            }
        }

        /// <summary>
        /// Sets the value for the given shader parameter, works with every type.
        /// Note that this will change the parameter value in the currently bound shader.
        /// </summary>
        /// <param name="param">The shader parameter.</param>
        private void SetShaderParamT(in IActiveUniform param)
        {
            var val = param.UniformValueGetter();
            if (val == null) return;

            if (param.IsGlobal || param.HasValueChanged)
            {
                if (val is bool boolVal)
                {
                    _rci.SetShaderParam(param.Handle, boolVal ? 1 : 0);
                }
                else if (val is int intVal)
                {
                    _rci.SetShaderParam(param.Handle, intVal);
                }
                else if (val is float floatVal)
                {
                    _rci.SetShaderParam(param.Handle, floatVal);
                }
                else if (val is double doubleVal)
                {
                    _rci.SetShaderParam(param.Handle, doubleVal);
                }
                else if (val is float2 float2Val)
                {
                    _rci.SetShaderParam(param.Handle, float2Val);
                }
                else if (val is float3 float3Val)
                {
                    _rci.SetShaderParam(param.Handle, float3Val);
                }
                else if (val is float4 float4Val)
                {
                    _rci.SetShaderParam(param.Handle, float4Val);
                }
                else if (val is float4x4 float4x4Val)
                {
                    _rci.SetShaderParam(param.Handle, float4x4Val);
                }
                else if (val is int2 int2Val)
                {
                    _rci.SetShaderParam(param.Handle, int2Val);
                }

                else if (val is IWritableArrayTexture writableArrayTex)
                {
                    SetShaderParamTexture(param.Handle, (WritableArrayTexture)writableArrayTex);
                }
                else if (val is IWritableCubeMap writableCubeTex)
                {
                    SetShaderParamTexture(param.Handle, (WritableCubeMap)writableCubeTex);
                }
                else if (val is IWritableTexture[] writableTexArray)
                {
                    SetShaderParamWritableTextureArray(param.Handle, (WritableTexture[])writableTexArray);
                }
                else if (val is IWritableTexture writableTex)
                {
                    var wt = (WritableTexture)writableTex;
                    if (wt.AsImage)
                        SetShaderParamImage(param.Handle, wt);
                    else
                        SetShaderParamTexture(param.Handle, wt);

                }
                else if (val is WritableMultisampleTexture wmst)
                {
                    SetShaderParamTexture(param.Handle, wmst);
                }
                else if (val is ITexture tex)
                {
                    if (tex.GetType() == typeof(Texture1D))
                        SetShaderParamTexture(param.Handle, (Texture1D)tex);
                    else
                        SetShaderParamTexture(param.Handle, (Texture)tex);
                }
                else if (val is IStorageBuffer buffer)
                {
                    ConnectBufferToShaderStorage(buffer, param.Name);
                }

                else if (val is float4x4[] float4x4ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float4x4ArrayVal);
                }
                else if (val is float2[] float2ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float2ArrayVal);
                }
                else if (val is float3[] float3ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float3ArrayVal);
                }
                else if (val is float4[] float4ArrayVal)
                {
                    _rci.SetShaderParam(param.Handle, float4ArrayVal);
                }

                else
                {
                    throw new ArgumentException($"{param} has an unknown type {val.GetType().Name}.");
                }
            }
            else
            {
                if (val is ITextureBase textureBase)
                {
                    if (textureBase is IWritableArrayTexture)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableArrayTexture)textureBase);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.ArrayTexture);
                    }
                    else if (val is IWritableCubeMap writableCubeTex)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableCubeMap)writableCubeTex);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.TextureCubeMap);
                    }
                    else if (val is IWritableTexture writableTex)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableTexture)writableTex);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture2D);

                    }
                    else if (val is WritableMultisampleTexture writableMultTex)
                    {
                        ITextureHandle textureHandle = _textureManager.GetTextureHandle((WritableMultisampleTexture)writableMultTex);
                        _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.TextureMultisample);
                    }
                    else if (val is ITexture tex)
                    {
                        if (tex.GetType() == typeof(Texture1D))
                        {
                            ITextureHandle textureHandle = _textureManager.GetTextureHandle((Texture1D)tex);
                            _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture1D);
                        }
                        else
                        {
                            ITextureHandle textureHandle = _textureManager.GetTextureHandle((Texture)tex);
                            _rci.SetActiveAndBindTexture(param.Handle, textureHandle, TextureType.Texture2D);
                        }
                    }
                    else if (val is IWritableTexture[] writableTexArray)
                    {
                        foreach (var texture in (WritableTexture[])writableTexArray)
                        {
                            ITextureHandle textureHandle = _textureManager.GetTextureHandle(texture);
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
        /// Retrieve pixels from bound framebuffer
        /// </summary>
        /// <param name="x">x pixel position</param>
        /// <param name="y">y pixel position</param>
        /// <param name="pixelFormat">format to retrieve, this has to match the current bound FBO!</param>
        /// <param name="width">how many pixel in x direction</param>
        /// <param name="height">how many pixel in y direction</param>
        /// <returns><see cref="ReadOnlySpan{T}"/> with pixel content</returns>
        /// <remarks>Does usually not throw on error (e. g. wrong pixel format, out of bounds, etc), uses GL.GetError() to retrieve
        /// potential error</remarks>
        public ReadOnlySpan<byte> ReadPixels(int x, int y, ImagePixelFormat pixelFormat, int width, int height)
        {
            return _rci.ReadPixels(x, y, pixelFormat, width, height);
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
                LockedStates[renderState] = new KeyValuePair<bool, uint>(true, currentVal);
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
        /// Takes a <see cref="WritableMultisampleTexture"/> and blits the result of all samples into an
        /// existing <see cref="WritableTexture"/> for further use (e. g. bind and use as Albedo texture)
        /// </summary>
        /// <param name="input">WritableMultisampleTexture</param>
        /// <param name="output">WritableTexture</param>
        public void BlitMultisample2DTextureToTexture(WritableMultisampleTexture input, WritableTexture output)
        {
            _rci.BlitMultisample2DTextureToTexture(input, output);
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
        public void SetRenderTarget(IWritableTexture? tex)
        {
            if (tex == null)
                SetRenderTarget();
            else if (tex is WritableTexture wt)
                SetRenderTarget(wt);
            else if (tex is WritableMultisampleTexture wmst)
                SetRenderTarget(wmst);
        }

        /// <summary>
        ///  Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(WritableTexture tex)
        {
            var texHandle = _textureManager.GetTextureHandle(tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        ///  Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        public void SetRenderTarget(WritableMultisampleTexture tex)
        {
            var texHandle = _textureManager.GetTextureHandle(tex);
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
            UpdateAllActiveFxParams(cFx);

            _rci.DispatchCompute(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh that should be rendered.</param>
        /// <param name="instanceData">Optional parameter in case gpu instancing is used to render the given mesh. See <see cref="InstanceData"/>.</param>
        /// <param name="doRenderForward">Is a forward or deferred renderer used? Will fetch the proper shader for the render method.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        public void Render(Mesh mesh, InstanceData instanceData = null, bool doRenderForward = true)
        {
            var cFx = GetCompiledFxForRenderMethod(doRenderForward);
            SetCompiledFx(cFx.GpuHandle);
            SetRenderStateSet(_currentEffect.RendererStates);
            UpdateAllActiveFxParams(cFx);

            var meshImp = _meshManager.GetImpFromMesh(mesh);

            // The dirty index functionality works after the initial call to the MeshManager
            // This is therefore the first possible place to catch und discard (pointcloud)-meshes with a dirty index
            if (mesh != null && mesh.Flags != null && mesh.Flags.DirtyIndex)
            {
                return;
            }

            if (instanceData != null)
            {
                var instanceDataImp = _meshManager.GetImpFromInstanceData(mesh, instanceData);
                _rci.Render(meshImp, instanceDataImp);
            }
            else
                _rci.Render(meshImp, null);
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
            UpdateAllActiveFxParams(cFx);

            var meshImp = _meshManager.GetImpFromMesh(mesh);
            _rci.Render(meshImp);
        }

        /// <summary>
        /// After rendering always cleanup pending meshes, textures and shader effects
        /// </summary>
        internal void CleanupResourceManagers()
        {
            _meshManager.Cleanup();
            _textureManager.Cleanup();
            _effectManager.Cleanup();
        }

        /// <summary>
        /// Calls the mesh manager which traverses all known meshes and updates GPU data if necessary
        /// </summary>
        internal void UpdateAllMeshes()
        {
            _meshManager.UpdateAllMeshes();
        }

        private void CalculateClippingPlanesFromProjection(out float2 clippingPlanes)
        {
            var C = Projection.M33;
            var D = Projection.M34;
            float f = D / (C - 1.0f) * -1;
            float n = D / (C + 1.0f) * -1;
            clippingPlanes.x = n;
            clippingPlanes.y = f;
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
        public GpuMesh CreateGpuMesh(PrimitiveType primitiveType, float3[] vertices, uint[] triangles = null,
            float3[] normals = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null, float2[] uvs = null,
            float4[] tangents = null, float3[] bitangents = null, float4[] boneIndices = null, float4[] boneWeights = null, uint[] flags = null)
        {
            var mesh = new GpuMesh
            {
                MeshType = primitiveType,
                BoundingBox = new AABBf(vertices)
            };
            _meshManager.RegisterNewMesh(mesh, vertices, triangles, uvs,
            normals, colors, colors1, colors2,
            tangents, bitangents, boneIndices, boneWeights, flags);
            return mesh;
        }

        #endregion
    }
}