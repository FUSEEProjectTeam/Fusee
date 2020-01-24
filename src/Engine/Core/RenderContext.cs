using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{   

    /// <summary>
    /// The render context contains all functions necessary to manipulate the underlying rendering hardware. Use this class' elements
    /// to render geometry to the RenderCanvas associated with this context. If you have worked with OpenGL or DirectX before you will find
    /// many similarities in this class' methods and properties.
    /// </summary>
    public class RenderContext
    {
        #region Fields

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


        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// All global FX Params
        /// Overwrites values with the same name in current ShaderEffect
        /// </summary>
        public readonly Dictionary<string, object> AllFXParams = new Dictionary<string, object>();

        /// <summary>
        /// The currently bound shader program.
        /// </summary>
        public ShaderProgram CurrentShaderProgram { get; private set; }

        /// <summary>
        /// Gets and sets a value indicating whether [debug lines enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug lines enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool DebugLinesEnabled { get; set; } = true;       

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
        /// Safes the default state of the render context. This is used to reset the render context to its default state after every render call.
        /// </summary>
        public RenderContextDefaultState DefaultState { get; private set; }

        #region Private Fields

        private readonly IRenderContextImp _rci;       

        private readonly MatrixParamNames _currentShaderParams;
        private ShaderEffect _currentShaderEffect;

        // Mesh Management
        private readonly MeshManager _meshManager;

        // Texture Management
        private readonly TextureManager _textureManager;

        // ShaderEffect Management
        private readonly ShaderEffectManager _shaderEffectManager;
        private readonly Dictionary<ShaderEffect, CompiledShaderEffect> _allCompiledShaderEffects = new Dictionary<ShaderEffect, CompiledShaderEffect>();

        private bool _updatedShaderParams;

        private readonly ShaderProgram _debugShader;
        private readonly IShaderParam _debugColor;

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

        #region Matrix Fields

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
                //value.M13 *= -1; // Correct Operation to make Coordinate System left handed
                //value.M23 *= -1;
                //value.M33 *= -1;
                //value.M43 *= -1;
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
                _modelView = _view * _model;

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
                _modelView = _view * _model;

                UpdateCurrentShader();
            } //TODO: Flags
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
            get { return _modelView; }
            set
            {
                // Update matrix
                _modelView = value;
                _view = float4x4.Identity;
                _model = value;

                // Invalidate derived matrices
                _modelViewProjectionOk = false;

                _invModelOk = false;
                _invViewOk = false;
                _invModelViewOk = false;
                _invModelViewProjectionOk = false;

                _invTransModelOk = false;
                _invTransViewOk = false;
                _invTransModelViewOk = false;
                _invTransModelViewProjectionOk = false;

                _transModelOk = false;
                _transViewOk = false;
                _transModelViewOk = false;
                _transModelViewProjectionOk = false;

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
        /// <seealso cref="RenderContext.View"/>
        /// <seealso cref="RenderContext.InvView"/>
        /// <seealso cref="RenderContext.TransView"/>
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
        /// <seealso cref="RenderContext.Model"/>
        /// <seealso cref="RenderContext.InvModel"/>
        /// <seealso cref="RenderContext.TransModel"/>
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
        /// <seealso cref="RenderContext.ModelView"/>
        /// <seealso cref="RenderContext.InvModelView"/>
        /// <seealso cref="RenderContext.TransModelView"/>
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
        /// <seealso cref="RenderContext.Projection"/>
        /// <seealso cref="RenderContext.InvProjection"/>
        /// <seealso cref="RenderContext.TransProjection"/>
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
        /// <seealso cref="RenderContext.ModelViewProjection"/>
        /// <seealso cref="RenderContext.InvModelViewProjection"/>
        /// <seealso cref="RenderContext.TransModelViewProjection"/>
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

        #endregion

        #region Constructors

        /// <summary>
        /// The color value.
        /// </summary>
        protected float3 _col;

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

            // Make JSIL run through this one time. 
            _col = ColorUint.White.Tofloat3();


            _currentShaderParams = new MatrixParamNames();
            _updatedShaderParams = false;

            //_debugShader = Shaders.GetColorShader(this);
            //_debugColor = _debugShader.GetShaderParam("color");
        }

        #endregion

        #region Members

        #region Private Members               

        private void UpdateCurrentShader()
        {
            if (CurrentShaderProgram == null)
                return;

            if (!_updatedShaderParams)
                UpdateShaderParams();

            // Normal versions of MV and P
            if (_currentShaderParams.FUSEE_M != null)
                SetFXParam("FUSEE_M", Model);

            if (_currentShaderParams.FUSEE_V != null)
                SetFXParam("FUSEE_V", View);

            if (_currentShaderParams.FUSEE_IV != null)
                SetFXParam("FUSEE_IV", InvView);

            if (_currentShaderParams.FUSEE_MV != null)
                SetFXParam("FUSEE_MV", ModelView);

            if (_currentShaderParams.FUSEE_P != null)
                SetFXParam("FUSEE_P", Projection);

            if (_currentShaderParams.FUSEE_MVP != null)
                SetFXParam("FUSEE_MVP", ModelViewProjection);

            // Inverted versions
            // Todo: Add inverted versions for M and V
            if (_currentShaderParams.FUSEE_IMV != null)
                SetFXParam("FUSEE_IMV", InvModelView);

            if (_currentShaderParams.FUSEE_ITV != null)
                SetFXParam("FUSEE_ITV", InvTransView);

            if (_currentShaderParams.FUSEE_IP != null)
                SetFXParam("FUSEE_IP", InvProjection);

            if (_currentShaderParams.FUSEE_IMVP != null)
                SetFXParam("FUSEE_IMVP", InvModelViewProjection);

            // Transposed versions
            // Todo: Add transposed versions for M and V
            if (_currentShaderParams.FUSEE_TMV != null)
                SetFXParam("FUSEE_TMV", TransModelView);

            if (_currentShaderParams.FUSEE_TP != null)
                SetFXParam("FUSEE_TP", TransProjection);

            if (_currentShaderParams.FUSEE_TMVP != null)
                SetFXParam("FUSEE_TMVP", TransModelViewProjection);

            // Inverted and transposed versions
            // Todo: Add inverted & transposed versions for M and V
            if (_currentShaderParams.FUSEE_ITMV != null)
                SetFXParam("FUSEE_ITMV", InvTransModelView);

            if (_currentShaderParams.FUSEE_ITP != null)
                SetFXParam("FUSEE_ITP", InvTransProjection);

            if (_currentShaderParams.FUSEE_ITMVP != null)
                SetFXParam("FUSEE_ITMVP", InvTransModelViewProjection);

            // Bones (if any)
            if (_currentShaderParams.FUSEE_BONES != null && Bones != null)
                SetFXParam("FUSEE_BONES[0]", Bones);

        }

        private void UpdateShaderParams()
        {
            if (CurrentShaderProgram == null)
            {
                // TODO: log that no shader was set
                return;
            }

            // Normal versions of MV and P
            _currentShaderParams.FUSEE_M = CurrentShaderProgram.GetShaderParam("FUSEE_M");
            _currentShaderParams.FUSEE_V = CurrentShaderProgram.GetShaderParam("FUSEE_V");
            _currentShaderParams.FUSEE_MV = CurrentShaderProgram.GetShaderParam("FUSEE_MV");
            _currentShaderParams.FUSEE_P = CurrentShaderProgram.GetShaderParam("FUSEE_P");
            _currentShaderParams.FUSEE_MVP = CurrentShaderProgram.GetShaderParam("FUSEE_MVP");

            // Inverted versions
            _currentShaderParams.FUSEE_IMV = CurrentShaderProgram.GetShaderParam("FUSEE_IMV");
            _currentShaderParams.FUSEE_IP = CurrentShaderProgram.GetShaderParam("FUSEE_IP");
            _currentShaderParams.FUSEE_IMVP = CurrentShaderProgram.GetShaderParam("FUSEE_IMVP");
            _currentShaderParams.FUSEE_IV = CurrentShaderProgram.GetShaderParam("FUSEE_IV");

            // Transposed versions
            _currentShaderParams.FUSEE_TMV = CurrentShaderProgram.GetShaderParam("FUSEE_TMV");
            _currentShaderParams.FUSEE_TP = CurrentShaderProgram.GetShaderParam("FUSEE_TP");
            _currentShaderParams.FUSEE_TMVP = CurrentShaderProgram.GetShaderParam("FUSEE_TMVP");

            // Inverted and transposed versions
            _currentShaderParams.FUSEE_ITMV = CurrentShaderProgram.GetShaderParam("FUSEE_ITMV");
            _currentShaderParams.FUSEE_ITP = CurrentShaderProgram.GetShaderParam("FUSEE_ITP");
            _currentShaderParams.FUSEE_ITMVP = CurrentShaderProgram.GetShaderParam("FUSEE_ITMVP");

            // Bones
            _currentShaderParams.FUSEE_BONES = CurrentShaderProgram.GetShaderParam("FUSEE_BONES[0]");            

            _updatedShaderParams = true;
            UpdateCurrentShader();
        }

        #endregion

        #region Public Members

        #region Image Data related Members

        /// <summary>
        /// Updates a rectangular region of a given Texture (dstTexture) by copying a rectangular block from another texture (srcTexture).
        /// </summary>
        /// <param name="dstTexture">This Textures region will be updated.</param>
        /// <param name="srcTexture">This is the source from which the region will be copied.</param>
        /// <param name="startX">x offset in pixels.</param>
        /// <param name="startY">y offset in pixels.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public void UpdateTextureRegion(Texture dstTexture, Texture srcTexture, int startX, int startY, int width, int height)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandleFromTexture(dstTexture);
            _rci.UpdateTextureRegion(textureHandle, srcTexture, startX, startY, width, height);
        }

        /// <summary>
        /// Gets or creates a new <see cref="ITextureHandle"/> for the given <see cref="Texture"/> by using the <see cref="TextureManager"/>.
        /// </summary>      
        /// <param name="tex">An <see cref="Texture"/>, containing necessary information for the upload to the graphics card.</param>     
        public ITextureHandle CreateTexture(Texture tex)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandleFromTexture(tex);
            return textureHandle;
        }

        /// <summary>
        /// Free all allocated gpu memory that belong to the given <see cref="ITextureHandle"/>.
        /// </summary>
        /// <param name="textureHandle">The <see cref="ITextureHandle"/> which gpu allocated memory will be freed.</param>
        public void RemoveTextureHandle(ITextureHandle textureHandle)
        {
            _rci.RemoveTextureHandle(textureHandle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belongs to a frame-buffer object.
        /// </summary>
        /// <param name="bufferHandle">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteFrameBuffer(IBufferHandle bufferHandle)
        {
            _rci.DeleteFrameBuffer(bufferHandle);
        }

        /// <summary>
        /// Free all allocated gpu memory that belongs to a render-buffer object.
        /// </summary>
        /// <param name="bufferHandle">The platform dependent abstraction of the gpu buffer handle.</param>
        public void DeleteRenderBuffer(IBufferHandle bufferHandle)
        {
            _rci.DeleteRenderBuffer(bufferHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        public void SetShaderParamTexture(IShaderParam param, Texture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texture">An ITexture.</param>
        public void SetShaderParamWritableTexture(IShaderParam param, WritableTexture texture)
        {
            ITextureHandle textureHandle = _textureManager.GetWritableTextureHandleFromTexture(texture);
            _rci.SetShaderParamTexture(param, textureHandle);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="textures">A texture array.</param>
        public void SetShaderParamWritableTextureArray(IShaderParam param, WritableTexture[] textures)
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
        public void SetShaderParamWritableCubeMap(IShaderParam param, WritableCubeMap texture)
        {
            ITextureHandle textureHandle = _textureManager.GetWritableCubeMapHandleFromTexture(texture);
            _rci.SetShaderParamCubeTexture(param, textureHandle);
        }

        #endregion

        #region Light related Members

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


        #endregion

        #region Shader related Members

        /// <summary>
        /// Sets global FX Params
        /// Overwrites values with the same name in current ShaderEffect
        /// </summary>
        /// <param name="name">FX Param name</param>
        /// <param name="value">FX Param value</param>
        // ReSharper disable once InconsistentNaming
        public void SetFXParam(string name, object value)
        {
            object tmpFXParam;

            if (AllFXParams.TryGetValue(name, out tmpFXParam)) // already in cache?
            {
                if (tmpFXParam.Equals(value)) return; // no new value

                AllFXParams[name] = value;

                // Update ShaderEffect
                _currentShaderEffect.SetEffectParam(name, value);
                return;
            }

            // cache miss
            AllFXParams.Add(name, value);

            // Update ShaderEffect
            _currentShaderEffect.SetEffectParam(name, value);
        }

        /// <summary>
        /// Creates a shader object from vertex shader source code and pixel shader source code.
        /// </summary>
        /// <param name="vs">A string containing the vertex shader source.</param>
        /// <param name="ps">A string containing the pixel (fragment) shader source code.</param>
        /// <param name="gs">A string containing the geometry shader source code (optional).</param>
        /// <returns>A shader program object identifying the combination of the given vertex and pixel shader.</returns>
        /// <remarks>
        /// Currently only shaders in GLSL (or rather GLSL/ES) source language(s) are supported.
        /// The result is already compiled to code executable on the GPU. <see cref="SetShader(ShaderProgram)"/>
        /// to activate the result as the current shader used for rendering geometry passed to the RenderContext.
        /// </remarks>
        private ShaderProgram CreateShader(string vs, string ps, string gs = null)
        {
            return new ShaderProgram(_rci, _rci.CreateShader(vs, ps, gs));
        }

        /// <summary>
        /// Removes given shader program from GPU
        /// </summary>
        /// <param name="ef">The ShaderEffect</param>
        internal void RemoveShader(ShaderEffect ef)
        {
            if (!_allCompiledShaderEffects.TryGetValue(ef, out CompiledShaderEffect sFxParam)) return;

            foreach (var program in sFxParam.CompiledShaders)
            {
                _rci.RemoveShader(program._spi);
            }
        }

        /// <summary>
        /// Activates the passed shader program as the current shader for geometry rendering.
        /// </summary>
        /// <param name="program">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        /// <seealso cref="CreateShader"/>
        /// <seealso cref="Render(Mesh)"/>
        private void SetShader(ShaderProgram program)
        {
            _updatedShaderParams = false;

            if (CurrentShaderProgram != program)
            {
                CurrentShaderProgram = program;
                _rci.SetShader(program._spi);
            }
            UpdateShaderParams(); // initial set
        }

        /// <summary>
        /// Activates the passed shader effect as the current shader for geometry rendering.
        /// </summary>
        /// <param name="ef">The shader effect to compile and use.</param>
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

            var compiledShader = new CompiledShaderEffect
            {
                CompiledShaders = new ShaderProgram[nPasses]
            };

            try // to compile all the shaders
            {
                for (i = 0; i < nPasses; i++)
                {
                    compiledShader.CompiledShaders[i] = CreateShader(ef.VertexShaderSrc[i], ef.PixelShaderSrc[i], ef.GeometryShaderSrc[i]);
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Error("Error while compiling shader for pass ", ex, new string[] { ef.VertexShaderSrc[0], ef.PixelShaderSrc[0] });
                throw new Exception("Error while compiling shader for pass " + i, ex);
            }

            _allCompiledShaderEffects.Add(ef, compiledShader);

            CreateAllShaderEffectVariables(ef);

            // register built shader effect
            _shaderEffectManager.RegisterShaderEffect(ef);

            // register this shader effect as current shader
            _currentShaderEffect = ef;
        }

        internal void HandleAndUpdateChangedButExisistingEffectVariable(ShaderEffect ef, string changedName, object changedValue)
        {
            CompiledShaderEffect sFxParam;
            if (!_allCompiledShaderEffects.TryGetValue(ef, out sFxParam)) return; // if "ef" not built -> return

            foreach (var passParams in sFxParam.ParamsPerPass)
            {
                foreach (var param in passParams)
                {
                    // if not found -> continue
                    if (!param.Info.Name.Equals(changedName)) continue;

                    // if not changed -> break
                    if (param.Value.Equals(changedValue))
                        return;

                    param.Value = changedValue;
                }
            }
        }

        /// <summary>
        /// Specifies the rasterized width of both aliased and antialiased lines.
        /// </summary>
        /// <param name="width">The width in px.</param>
        public void SetLineWidth(float width)
        {
            _rci.SetLineWidth(width);
        }

        internal void CreateAllShaderEffectVariables(ShaderEffect ef)
        {
            int nPasses = ef.VertexShaderSrc.Length;

            if (!_allCompiledShaderEffects.TryGetValue(ef, out CompiledShaderEffect sFxParam))
            {
                sFxParam = new CompiledShaderEffect();
                _allCompiledShaderEffects.Add(ef, sFxParam);
            }

            // Enumerate all shader parameters of all passes and enlist them in lookup tables
            sFxParam.Parameters = new Dictionary<string, object>();
            sFxParam.ParamsPerPass = new List<List<EffectParam>>();

            for (var i = 0; i < nPasses; i++)
            {
                var shaderParamInfos = GetShaderParamList(sFxParam.CompiledShaders[i]).ToList();
                sFxParam.ParamsPerPass.Add(new List<EffectParam>());

                foreach (var paramNew in shaderParamInfos)
                {
                    if (ef.ParamDecl.TryGetValue(paramNew.Name, out object initValue))
                    {
                        if (initValue == null)
                            continue;

                        // OVERWRITE VARS WITH GLOBAL FXPARAMS
                        if (AllFXParams.TryGetValue(paramNew.Name, out object globalFXValue))
                        {
                            if (!initValue.Equals(globalFXValue))
                            {
                                // Diagnostics.Log($"Global Overwrite {paramNew.Name},  with {globalFXValue}");

                                initValue = globalFXValue;
                                // update var in ParamDecl
                                ef.ParamDecl[paramNew.Name] = globalFXValue;
                            }
                        }

                        // IsAssignableFrom the boxed initValue object will cause JSIL to give an answer based on the value of the contents
                        // If the type originally was float but contains an integral value (e.g. 3), JSIL.GetType() will return Integer...
                        // Thus for primitive types (float, int, ) we hack a check ourselves. For other types (float2, ..) IsAssignableFrom works well.

                        // ReSharper disable UseMethodIsInstanceOfType
                        // ReSharper disable OperatorIsCanBeUsed
                        var initValType = initValue.GetType();

                        if (!(((paramNew.Type == typeof(int) || paramNew.Type == typeof(float))
                                  &&
                                  (initValType == typeof(int) || initValType == typeof(float) || initValType == typeof(double))
                                )
                                ||
                                (paramNew.Type.IsAssignableFrom(initValType))
                             )
                            && (!paramNew.Name.Contains("BONES") && !paramNew.Name.Contains("[0]"))
                        )
                        {
                            throw new Exception("Error preparing effect pass " + i + ". Shader parameter " + paramNew.Type.ToString() + " " + paramNew.Name +
                                                " was defined as " + initValType.ToString() + " " + paramNew.Name + " during initialization (different types).");
                        }
                        // ReSharper restore OperatorIsCanBeUsed
                        // ReSharper restore UseMethodIsInstanceOfType

                        // Parameter was declared by user and type is correct in shader - carry on.
                        EffectParam paramExisting;
                        object paramExistingTmp;
                        if (sFxParam.Parameters.TryGetValue(paramNew.Name, out paramExistingTmp))
                        {
                            paramExisting = (EffectParam)paramExistingTmp;
                            // The parameter is already there from a previous pass.
                            if (paramExisting.Info.Size != paramNew.Size || paramExisting.Info.Type != paramNew.Type)
                            {
                                // This should never happen due to the previous error check. Check it anyway...
                                throw new Exception("Error preparing effect pass " + i + ". Shader parameter " +
                                                    paramNew.Name +
                                                    " already defined with a different type in effect pass " +
                                                   paramExisting.ShaderInxs[0]);
                            }
                            // List the current pass to use this shader parameter
                            paramExisting.ShaderInxs.Add(i);
                        }
                        else
                        {
                            paramExisting = new EffectParam()
                            {
                                Info = paramNew,
                                ShaderInxs = new List<int>(new int[] { i }),
                                Value = initValue
                            };
                            sFxParam.Parameters.Add(paramNew.Name, paramExisting);
                        }
                        sFxParam.ParamsPerPass[i].Add(paramExisting);
                    }
                    else
                    {
                        // This should not happen due to shader compiler optimization
                        Diagnostics.Warn($"Uniform variable {paramNew.Name} found but no value is given. Please add this variable to ParamDecl of current ShaderEffect.");
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of (uniform) shader parameters accessed by the given shader.
        /// </summary>
        /// <param name="program">The shader program to query for parameters.</param>
        /// <returns>
        /// A list of shader parameters accessed by the shader code of the given shader program. The parameters listed here
        /// are the so-called uniform parameters of the shader (in contrast to the varying parameters). The list contains all
        /// uniform parameters that are accessed by either the vertex shader, the pixel shader, or both shaders compiled into
        /// the given shader.
        /// </returns>
        public IEnumerable<ShaderParamInfo> GetShaderParamList(ShaderProgram program)
        {
            return _rci.GetShaderParamList(program._spi);
        }

        /// <summary>
        /// Returns an identifier for the named (uniform) parameter used in the specified shader program.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> using the parameter.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns>A <see cref="IShaderParam"/> object to identify the given parameter in subsequent calls to SetShaderParam.</returns>
        /// <remarks>
        /// The returned handle can be used to assign values to a (uniform) shader parameter.
        /// </remarks>
        public IShaderParam GetShaderParam(ShaderProgram program, string paramName)
        {
            return _rci.GetShaderParam(program._spi, paramName);
        }

        /// <summary>
        /// Gets the value of a shader parameter.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/>.</param>
        /// <param name="handle">The <see cref="IShaderParam"/>.</param>
        /// <returns>The float value.</returns>
        public float GetParamValue(ShaderProgram program, IShaderParam handle)
        {
            return _rci.GetParamValue(program._spi, handle);
        }

        /// <summary>
        /// Sets the specified shader parameter to a float value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float2 value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float2 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float3 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float2 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>        
        public void SetShaderParam(IShaderParam param, float2[] val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float3 value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float3 value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float3 array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float3 array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>        
        public void SetShaderParam(IShaderParam param, float3[] val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float4 value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float4 value that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, float4[] val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float4x4 matrix that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float4x4 matrix array.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The float4x4 matrix array that should be assigned to the shader array parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, float4x4[] val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a integer value.
        /// </summary>
        /// <param name="param">The <see cref="IShaderParam"/> identifier.</param>
        /// <param name="val">The integer value that should be assigned to the shader parameter.</param>
        /// <remarks>
        /// <see cref="GetShaderParam"/> to see how to retrieve an identifier for
        /// a given uniform parameter name used in a shader program.
        /// </remarks>
        /// <seealso cref="GetShaderParamList"/>
        public void SetShaderParam(IShaderParam param, int val)
        {
            _rci.SetShaderParam(param, val);
        }
        #endregion

        #region Render related Members       

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
        /// Apply a single render state to the render context. All subsequent rendering will be
        /// performed using the currently set state unless it is changed to a different value.
        /// </summary>
        /// <param name="renderState">One of the <see cref="RenderState"/> enumeration values.</param>
        /// <param name="value">An unsigned integer value representing the value the state should be set to.
        ///  Depending on the renderState, this value can be interpreted as an integer value, a float value, a
        /// boolean value, or even a color.  </param>
        /// <remarks>This method is close to the underlying implementation layer and might be awkward to use
        /// due to the ambiguity of the value parameter type. If you want type-safe state values and also 
        /// want to set a couple of states at the same time, try the more 
        /// elaborate <see cref="SetRenderState(RenderStateSet)"/> method.</remarks>
        public void SetRenderState(RenderState renderState, uint value)
        {
            _rci.SetRenderState(renderState, value);
        }

        /// <summary>
        /// Apply a number of render states to this render context. All subsequent rendering will be
        /// performed using the currently set state set unless one of its values it is changed. Use this 
        /// method to change more than one render state at once. 
        /// </summary>
        /// <param name="renderStateSet">A set of render states with their respective values to be set.</param>
        public void SetRenderState(RenderStateSet renderStateSet)
        {
            foreach (var state in renderStateSet.States)
            {
                var theKey = state.Key;
                var theValue = state.Value;
                _rci.SetRenderState(theKey, theValue);
            }
        }

        /// <summary>
        /// Returns the current render state set.
        /// </summary>        
        /// <returns></returns>
        public RenderStateSet GetRenderStateSet()
        {
            return new RenderStateSet()
            {
                AlphaBlendEnable = _rci.GetRenderState(RenderState.AlphaBlendEnable) != 0,
                BlendFactor = (float4)(ColorUint)_rci.GetRenderState(RenderState.BlendFactor),
                BlendOperation = (BlendOperation)_rci.GetRenderState(RenderState.BlendOperation),
                BlendOperationAlpha = (BlendOperation)_rci.GetRenderState(RenderState.BlendOperationAlpha),
                DestinationBlend = (Blend)_rci.GetRenderState(RenderState.DestinationBlend),
                DestinationBlendAlpha = (Blend)_rci.GetRenderState(RenderState.DestinationBlendAlpha),
                SourceBlend = (Blend)_rci.GetRenderState(RenderState.SourceBlend),
                SourceBlendAlpha = (Blend)_rci.GetRenderState(RenderState.SourceBlendAlpha),

                CullMode = (Cull)_rci.GetRenderState(RenderState.CullMode),
                Clipping = _rci.GetRenderState(RenderState.Clipping) != 0,
                FillMode = (FillMode)_rci.GetRenderState(RenderState.FillMode),
                ZEnable = _rci.GetRenderState(RenderState.ZEnable) != 0,
                ZFunc = (Compare)_rci.GetRenderState(RenderState.ZEnable),
                ZWriteEnable = _rci.GetRenderState(RenderState.ZWriteEnable) != 0
            };            
        }

        /// <summary>
        /// Returns the current render state.
        /// </summary>
        /// <param name="renderState"></param>
        /// <returns></returns>
        public uint GetRenderState(RenderState renderState)
        {
            return _rci.GetRenderState(renderState);
        }

        /// <summary>
        /// Sets the RenderTarget, if texture is null render target is the main screen, otherwise the picture will be rendered onto given texture
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        internal void SetRenderTarget(RenderTarget renderTarget = null)
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
        internal void SetRenderTarget(IWritableTexture tex)
        {
            var texHandle = _textureManager.GetWritableTextureHandleFromTexture((WritableTexture)tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        /// Renders into the given texture.
        /// </summary>
        /// <param name="tex">The render texture.</param>
        internal void SetRenderTarget(IWritableCubeMap tex)
        {
            var texHandle = _textureManager.GetWritableCubeMapHandleFromTexture((WritableCubeMap)tex);
            _rci.SetRenderTarget(tex, texHandle);
        }

        /// <summary>
        /// Detaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="type">The texture to detach.</param>
        public void DetachTextureFromFbo(IRenderTarget renderTarget, RenderTargetTextureTypes type)
        {
            _rci.DetachTextureFromFbo(renderTarget, type);
        }

        /// <summary>
        /// Reattaches a texture from the frame buffer object, associated with the given render target.
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="type">The texture to detach.</param>
        public void ReattachTextureFromFbo(IRenderTarget renderTarget, RenderTargetTextureTypes type)
        {
            var texHandle = _textureManager.GetWritableTextureHandleFromTexture((WritableTexture)renderTarget.RenderTextures[(int)type]);
            _rci.ReatatchTextureFromFbo(renderTarget, type, texHandle);
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

            // GLOBAL OVERRIDE
            foreach (var fxParam in AllFXParams)
            {
                _currentShaderEffect.SetEffectParam(fxParam.Key, fxParam.Value);
            }

            int i = 0, nPasses = _currentShaderEffect.VertexShaderSrc.Length;
            try
            {
                for (i = 0; i < nPasses; i++)
                {
                    _allCompiledShaderEffects.TryGetValue(_currentShaderEffect, out var compiledShaderEffect);

                    // TODO: Use shared uniform parameters - currently SetShader will query the shader params and set all the common uniforms (like matrices and light)
                    SetShader(compiledShaderEffect.CompiledShaders[i]);

                    foreach (var param in compiledShaderEffect.ParamsPerPass[i])
                    {
                        SetShaderParamT(param);
                    }

                    SetRenderState(_currentShaderEffect.States[i]);
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

        /// <summary>
        /// Sets the shaderParam, works with every type.
        /// </summary>
        /// <param name="param"></param>
        internal void SetShaderParamT(EffectParam param)
        {
            if (param.Info.Type == typeof(int))
            {
                SetShaderParam(param.Info.Handle, (int)param.Value);
            }
            else if (param.Info.Type == typeof(float))
            {
                SetShaderParam(param.Info.Handle, (float)param.Value);
            }
            else if (param.Info.Type == typeof(float2))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float2[])param.Value;
                    SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                SetShaderParam(param.Info.Handle, (float2)param.Value);
            }
            else if (param.Info.Type == typeof(float3))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float3[])param.Value;
                    SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                SetShaderParam(param.Info.Handle, (float3)param.Value);
            }
            else if (param.Info.Type == typeof(float4))
            {
                SetShaderParam(param.Info.Handle, (float4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4))
            {
                if (param.Info.Size > 1)
                {
                    // param is an array
                    var paramArray = (float4x4[])param.Value;
                    SetShaderParam(param.Info.Handle, paramArray);
                    return;
                }
                SetShaderParam(param.Info.Handle, (float4x4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4[]))
            {
                SetShaderParam(param.Info.Handle, (float4x4[])param.Value);
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

        #endregion

        #region Other Members

        /// <summary>
        /// Resets the RenderContexts View, Projection and Viewport to the values defined in <see cref="DefaultState"/>.
        /// Must be called after every visitation of the Scene Graph that changed these values.
        /// </summary>
        internal void ResetToDefaultState()
        {
            Viewport(0, 0, DefaultState.CanvasWidth, DefaultState.CanvasHeight);
            View = DefaultState.View;
            Projection = DefaultState.Projection;
        }

        /// <summary>
        /// This method returns the color of one or more pixels from the back-buffer.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns>The requested rectangular area</returns>
        public IImageData GetPixelColor(int x, int y, int w, int h)
        {
            return _rci.GetPixelColor(x, y, w, h);
        }

        /// <summary>
        /// This method returns depth value from the depth-buffer at a given coordinate.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public float GetPixelDepth(int x, int y)
        {
            return _rci.GetPixelDepth(x, y);
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The start point of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            if (DebugLinesEnabled)
            {
                start /= 2;
                end /= 2;

                var oldShader = CurrentShaderProgram;
                SetShader(_debugShader);

                SetShaderParam(_currentShaderParams.FUSEE_MVP, ModelViewProjection);
                SetShaderParam(_debugColor, color);

                _rci.DebugLine(start, end, color);

                if (oldShader != null)
                    SetShader(oldShader);
            }
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
        /// <param name="renderToScreen">Determines if we render to screen or to a frame buffer object. Is true per default.</param>
        /// <remarks>
        /// Setting the Viewport limits the rendering output to the specified rectangular region.
        /// </remarks>
        public void Viewport(int x, int y, int width, int height, bool renderToScreen = true)
        {
            _rci.Scissor(x, y, width, height);
            _rci.Viewport(x, y, width, height);            

            if (!renderToScreen) return;

            ViewportWidth = width;
            ViewportHeight = height;
            ViewportXStart = x;
            ViewportYStart = y;
        }

        /// <summary>
        /// Enable or disable Color channels to be written to the frame buffer (final image).
        /// Use this function as a color channel filter for the final image.
        /// </summary>
        /// <param name="red">if set to <c>true</c> [red].</param>
        /// <param name="green">if set to <c>true</c> [green].</param>
        /// <param name="blue">if set to <c>true</c> [blue].</param>
        /// <param name="alpha">if set to <c>true</c> [alpha].</param>
        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            _rci.ColorMask(red, green, blue, alpha);
        }

        #endregion

        #endregion

        #endregion
    }

    internal sealed class MatrixParamNames
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
        public List<int> ShaderInxs;
    }

    /// <summary>
    /// All compiled information of one ShaderEffect
    /// </summary>
    internal class CompiledShaderEffect
    {
        /// <summary>
        /// The compiled vertex and pixel shaders
        /// </summary>
        internal ShaderProgram[] CompiledShaders;

        /// <summary>
        /// All parameter saved per pass with uniform handle, type and info (name, etc.) as lookup table
        /// </summary>
        internal List<List<EffectParam>> ParamsPerPass = new List<List<EffectParam>>();
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
                if(_aspect != 0)
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