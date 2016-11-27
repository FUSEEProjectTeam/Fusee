﻿using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using JSIL.Meta;
using Fusee.Math.Core;

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

        #region Private Fields

        private readonly IRenderContextImp _rci;

        internal int ViewportWidth { get; private set; }
        internal int ViewportHeight { get; private set; }

        private ShaderProgram _currentShader;
        private readonly MatrixParamNames _currentShaderParams;

        /* Removed Light support
        private readonly Light[] _lightParams;
        private readonly LightParamNames[] _lightShaderParams;
        */


        private bool _updatedShaderParams;

        private readonly ShaderProgram _debugShader;
        private readonly IShaderParam _debugColor;
        private bool _debugLinesEnabled = true;

        public bool HasPickingContext { get; private set; }

        // Settable matrices
        private float4x4 _modelView;
        private float4x4 _projection;
        private float4x4 _view;
        private float4x4 _model;
        private float4x4[] _bones;

        public float4x4[] Bones
        {
            get { return _bones; }
            set
            {
                _bones = value;
                UpdateCurrentShader();
            }
        }

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

        #region Internal Fields

        internal sealed class MatrixParamNames
        {
            // ReSharper disable InconsistentNaming
            public IShaderParam FUSEE_M;
            public IShaderParam FUSEE_V;
            public IShaderParam FUSEE_MV;

            public IShaderParam FUSEE_P;
            public IShaderParam FUSEE_MVP;

            public IShaderParam FUSEE_IMV;
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

        internal struct LightParamNames
        {
            /*
            // ReSharper disable InconsistentNaming
            public IShaderParam AMBIENT;
            public IShaderParam DIFFUSE;
            public IShaderParam SPECULAR;
            public IShaderParam POSITION;
            public IShaderParam DIRECTION;
            public IShaderParam SPOTANGLE;
            public IShaderParam ACTIVE;
            // ReSharper restore InconsistentNaming
            */
        }

        #endregion


        #region Matrix Fields

        /*/// <summary>
        /// The View matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        /// <remarks>
        /// This matrix is also reffered often as the camera transformation(not the projection). 
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
        */
        /*
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
                _modelView = _view*_model;

                UpdateCurrentShader();
            } //TODO: Flags
        } */

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
        /// view coordinate geometry wihth the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
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
        /// The 4x4 matrix resulting from the matrix multiplaction of the ModelView and the Projection matrix.
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

        /*
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
        */

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

        /*
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
        */

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

        /*
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
        */

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
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="rci">The <see cref="IRenderContextImp"/>.</param>
        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            // View = float4x4.Identity;
            ModelView = float4x4.Identity;
            Projection = float4x4.Identity;

            /* Removed Light support
            // Todo: Remove multiple Lights per shader !!!
            _lightParams = new Light[8];
            _lightShaderParams = new LightParamNames[8];
            */
            _currentShaderParams = new MatrixParamNames();
            _updatedShaderParams = false;

            _debugShader = Shaders.GetColorShader(this);
            _debugColor = _debugShader.GetShaderParam("color");
        }

        #endregion

        #region Members

        #region Private Members

        private void UpdateCurrentShader()
        {
            // Todo: Check if the respective matrix was changed since last accessed by the currently updated shader
            // Todo: and set only if matrix was changed.

            if (_currentShader == null)
                return;

            if (!_updatedShaderParams)
                UpdateShaderParams();

            // Normal versions of MV and P
            //  if (_currentShaderParams.FUSEE_M != null)
            //     SetShaderParam(_currentShaderParams.FUSEE_M, Model);

            //  if (_currentShaderParams.FUSEE_V != null)
            //     SetShaderParam(_currentShaderParams.FUSEE_V, View);

            if (_currentShaderParams.FUSEE_MV != null)
                SetShaderParam(_currentShaderParams.FUSEE_MV, ModelView);

            if (_currentShaderParams.FUSEE_P != null)
                SetShaderParam(_currentShaderParams.FUSEE_P, Projection);

            if (_currentShaderParams.FUSEE_MVP != null)
                SetShaderParam(_currentShaderParams.FUSEE_MVP, ModelViewProjection);

            // Inverted versions
            if (_currentShaderParams.FUSEE_IMV != null)
                SetShaderParam(_currentShaderParams.FUSEE_IMV, InvModelView);

            if (_currentShaderParams.FUSEE_IP != null)
                SetShaderParam(_currentShaderParams.FUSEE_IP, InvProjection);

            if (_currentShaderParams.FUSEE_IMVP != null)
                SetShaderParam(_currentShaderParams.FUSEE_IMVP, InvModelViewProjection);

            // Transposed versions
            if (_currentShaderParams.FUSEE_TMV != null)
                SetShaderParam(_currentShaderParams.FUSEE_TMV, TransModelView);

            if (_currentShaderParams.FUSEE_TP != null)
                SetShaderParam(_currentShaderParams.FUSEE_TP, TransProjection);

            if (_currentShaderParams.FUSEE_TMVP != null)
                SetShaderParam(_currentShaderParams.FUSEE_TMVP, TransModelViewProjection);

            // Inverted and transposed versions
            if (_currentShaderParams.FUSEE_ITMV != null)
                SetShaderParam(_currentShaderParams.FUSEE_ITMV, InvTransModelView);

            if (_currentShaderParams.FUSEE_ITP != null)
                SetShaderParam(_currentShaderParams.FUSEE_ITP, InvTransProjection);

            if (_currentShaderParams.FUSEE_ITMVP != null)
                SetShaderParam(_currentShaderParams.FUSEE_ITMVP, InvTransModelViewProjection);

            // Bones (if any)
            if (_currentShaderParams.FUSEE_BONES != null && Bones != null)
                SetShaderParam(_currentShaderParams.FUSEE_BONES, Bones);


            /* Removed light support
            // Todo: Remove multiple Lights per shader !!!
            for (var i = 0; i < 8; i++)
            {
                if (_lightShaderParams[i].AMBIENT != null)
                    SetShaderParam(_lightShaderParams[i].AMBIENT, _lightParams[i].AmbientColor);

                if (_lightShaderParams[i].DIFFUSE != null)
                    SetShaderParam(_lightShaderParams[i].DIFFUSE, _lightParams[i].DiffuseColor);

                if (_lightShaderParams[i].SPECULAR != null)
                    SetShaderParam(_lightShaderParams[i].SPECULAR, _lightParams[i].SpecularColor);

                if (_lightShaderParams[i].POSITION != null)
                    SetShaderParam(_lightShaderParams[i].POSITION, _lightParams[i].Position);

                if (_lightShaderParams[i].DIRECTION != null)
                    SetShaderParam(_lightShaderParams[i].DIRECTION, _lightParams[i].Direction);

                if (_lightShaderParams[i].ACTIVE != null)
                    SetShaderParam(_lightShaderParams[i].ACTIVE, _lightParams[i].Active);

                if (_lightShaderParams[i].SPOTANGLE != null)
                    SetShaderParam(_lightShaderParams[i].SPOTANGLE, _lightParams[i].Angle);
            }
            */

        }

        private void UpdateShaderParams()
        {
            if (_currentShader == null)
            {
                // TODO: log that no shader was set
                return;
            }

            // Normal versions of MV and P
            _currentShaderParams.FUSEE_M = _currentShader.GetShaderParam("FUSEE_M");
            _currentShaderParams.FUSEE_V = _currentShader.GetShaderParam("FUSEE_V");
            _currentShaderParams.FUSEE_MV = _currentShader.GetShaderParam("FUSEE_MV");
            _currentShaderParams.FUSEE_P = _currentShader.GetShaderParam("FUSEE_P");
            _currentShaderParams.FUSEE_MVP = _currentShader.GetShaderParam("FUSEE_MVP");

            // Inverted versions
            _currentShaderParams.FUSEE_IMV = _currentShader.GetShaderParam("FUSEE_IMV");
            _currentShaderParams.FUSEE_IP = _currentShader.GetShaderParam("FUSEE_IP");
            _currentShaderParams.FUSEE_IMVP = _currentShader.GetShaderParam("FUSEE_IMVP");

            // Transposed versions
            _currentShaderParams.FUSEE_TMV = _currentShader.GetShaderParam("FUSEE_TMV");
            _currentShaderParams.FUSEE_TP = _currentShader.GetShaderParam("FUSEE_TP");
            _currentShaderParams.FUSEE_TMVP = _currentShader.GetShaderParam("FUSEE_TMVP");

            // Inverted and transposed versions
            _currentShaderParams.FUSEE_ITMV = _currentShader.GetShaderParam("FUSEE_ITMV");
            _currentShaderParams.FUSEE_ITP = _currentShader.GetShaderParam("FUSEE_ITP");
            _currentShaderParams.FUSEE_ITMVP = _currentShader.GetShaderParam("FUSEE_ITMVP");

            // Bones
            _currentShaderParams.FUSEE_BONES = _currentShader.GetShaderParam("FUSEE_BONES[0]");

            /* Removed Light support
            for (int i = 0; i < 8; i++)
            {
                _lightShaderParams[i].AMBIENT = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].ambient");
                _lightShaderParams[i].DIFFUSE = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].diffuse");
                _lightShaderParams[i].SPECULAR = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].specular");
                _lightShaderParams[i].POSITION = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].position");
                _lightShaderParams[i].DIRECTION = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].direction");
                _lightShaderParams[i].SPOTANGLE = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].spotAngle");
                _lightShaderParams[i].ACTIVE = _currentShader.GetShaderParam("FUSEE_LIGHTS[" + i + "].active");
            }
            */

            _updatedShaderParams = true;
            UpdateCurrentShader();
        }

        #endregion

        #region Public Members

        #region Image Data related Members

        public void UpdateTextureFromVideoStream(IVideoStreamImp stream, ITexture tex)
        {
            _rci.UpdateTextureFromVideoStream(stream, tex);
        }

        public void UpdateTextureRegion(ITexture tex, ImageData img, int startX, int startY, int width, int height)
        {
            _rci.UpdateTextureRegion(tex, img, startX, startY, width, height);
        }

        /*
        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, ColorUint bgColor)
        {
            return _rci.CreateImage(width, height, bgColor);
        }

        /// <summary>
        /// Maps a specified text with on an image.
        /// </summary>
        /// <param name="imgData">The ImageData struct with the PixelData from the image.</param>
        /// <param name="fontName">The name of the text-font.</param>
        /// <param name="fontSize">The size of the text-font.</param>
        /// <param name="text">The text that sould be mapped on the iamge.</param>
        /// <param name="textColor">The color of the text-font.</param>
        /// <param name="startPosX">The horizontal start-position of the text on the image.</param>
        /// <param name="startPosY">The vertical start-position of the text on the image.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing</returns>
        public ImageData TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor, float startPosX, float startPosY)
        {
            return _rci.TextOnImage(imgData, fontName, fontSize, text, textColor, startPosX, startPosY);
        }
        */

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>
        /// <remarks>
        /// Method should be called after LoadImage method to process
        /// the BitmapData an make them available for the shader.
        /// </remarks>
        /// <param name="imgData">An ImageData struct, containing necessary information for the upload to the graphics card.</param>
        /// <param name="repeat">Indicating if the texture should be clamped or repeated.</param>
        /// <returns>
        /// An <see cref="ITexture"/> that can be used for texturing in the shader.
        /// </returns>
        public ITexture CreateTexture(ImageData imgData, bool repeat = false)
        {
            return _rci.CreateTexture(imgData, repeat);
        }

        /// <summary>
        /// Creates a new writable texture and binds it to the shader.
        /// This is done by creating a framebuffer and a renderbuffer (if needed).
        /// All bufferhandles are returned with the texture.
        /// For binding this texture call <see cref="SetRenderTarget"/>SetRenderTarget
        /// <param name="textureFormat">The format of writable texture (e.g. Depthbuffer, G-Buffer, ...)</param>
        /// </summary>
        /// <returns>
        /// An <see cref="ITexture"/>ITexture that can be used for of screen rendering
        /// </returns>
        public ITexture CreateWritableTexture(int width, int height, WritableTextureFormat textureFormat)
        {
            return _rci.CreateWritableTexture(width, height, textureFormat);
        }
  

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method.</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
            _rci.SetShaderParamTexture(param, texId);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture.
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding.</param>
        /// <param name="texId">An ITexture probably returned from CreateWritableTexture() method.</param>
        /// <param name="gHandle">The desired gBuffer texture</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId, GBufferHandle gHandle)
        {
            _rci.SetShaderParamTexture(param, texId, gHandle);
        }


        #endregion

        #region Text related Members

        /*
        /// <summary>
        /// Loads a font file (*.ttf) and processes it with the given font size.
        /// </summary>
        /// <param name="stream">The stream containting the font data.</param>
        /// <param name="size">The font size.</param>
        /// <returns>An <see cref="IFont"/> containing all necessary information for further processing.</returns>
        /// <exception cref="System.Exception">Font not found: "filename"</exception>
        public IFont LoadFont(Stream stream, uint size)
        {
            return _rci.LoadFont(stream, size);
        }

        /// <summary>
        /// Fixes the kerning of a text (if possible).
        /// </summary>
        /// <param name="font">The <see cref="IFont"/> containing information about the font.</param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="text">The text.</param>
        /// <param name="scaleX">The scale x (OpenGL scaling factor).</param>
        /// <returns>The fixed vertices as an array of <see cref="float3"/>.</returns>
        internal float3[] FixTextKerning(IFont font, float3[] vertices, string text, float scaleX)
        {
            return _rci.FixTextKerning(font, vertices, text, scaleX);
        }
        */

        #endregion

        #region Light related Members

        /// <summary>
        /// Sets the directional or point lights information.
        /// </summary>
        /// <param name="v3">The lights direction or position. This depends on the light type.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="type">The type of the light. 0=directional, 1=point.</param>
        /// <param name="id">The identifier. A maximum of 8 lights is recommended due to portability.</param>
        public void SetLight(float3 v3, float4 diffuse, float4 ambient, float4 specular, int type, int id)
        {
            switch (type)
            {
                case 1:
                    SetLightActive(id, type);
                    SetLightAmbient(id, ambient);
                    SetLightDiffuse(id, diffuse);
                    SetLightSpecular(id, specular);
                    SetLightDirection(id, v3);
                    break;
                case 2:
                    SetLightActive(id, type);
                    SetLightAmbient(id, ambient);
                    SetLightDiffuse(id, diffuse);
                    SetLightSpecular(id, specular);
                    SetLightPosition(id, v3);
                    break;
            }
        }

        /// <summary>
        /// Sets the spotlights information.
        /// </summary>
        /// <param name="position">The light position.</param>
        /// <param name="direction">The light direction.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="type">The light type.</param>
        /// <param name="id">The identifier.A maximum of 8 lights is recommended due to portability.</param>
        public void SetLight(float3 position, float3 direction, float4 diffuse, float4 ambient, float4 specular,
            int type, int id)
        {
            SetLightActive(id, type);
            SetLightAmbient(id, ambient);
            SetLightDiffuse(id, diffuse);
            SetLightSpecular(id, specular);
            SetLightPosition(id, position);
            SetLightDirection(id, direction);
        }

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
        /// Activates the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to activate. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="active">1 - activate the light. 0 - deactiv</param>
        public void SetLightActive(int lightInx, float active)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].Active = active;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_ACTIVE";

            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Active);
           */
        }

        /// <summary>
        /// Sets the ambient color component on the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the ambient color on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="ambientColor">
        /// The ambient color to be emitted by the given light. The value is interpreted as a (Red, Green, Blue, Alpha) quadruple with
        /// component values ranging from 0.0f to 1.0f. The Alpha component will be ignored.
        /// </param>
        /// <remarks>
        /// An ambient light component represents a fixed-intensity and fixed-color light that affects all parts of all objects in the scene equally.
        /// </remarks>
        public void SetLightAmbient(int lightInx, float4 ambientColor)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].AmbientColor = ambientColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_AMBIENT";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].AmbientColor);
            */
        }

        /// <summary>
        /// Sets the diffuse color component on the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the diffuse color on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="diffuseColor">
        /// The diffuse color to be emitted by the given light. The value is interpreted as a (Red, Green, Blue, Alpha) quadruple with
        /// component values ranging from 0.0f to 1.0f. The Alpha component will be ignored.
        /// </param>
        /// <remarks>
        /// A diffuse light component results in different parts of objects shaded with different intensites based on the angle of the incoming
        /// light ray at each given spot on the object surface. This component is what makes objects look "3D" - e.g. coloring the different faces of a cube with
        /// different intensities or creating brightness gradients on curved surfaces like a sphere.
        /// </remarks>
        public void SetLightDiffuse(int lightInx, float4 diffuseColor)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].DiffuseColor = diffuseColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIFFUSE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].DiffuseColor);
            */
        }

        /// <summary>
        /// Sets the specular color component on the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the specular color on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="specularColor">
        /// The specular color to be emitted by the given light. The value is interpreted as a (Red, Green, Blue, Alpha) quadruple with
        /// component values ranging from 0.0f to 1.0f. The Alpha component will be ignored.
        /// </param>
        /// <remarks>
        /// A specular light component results in highlights created on the lit surfaces where the light source is mirrored into the viewers' eye.
        /// Bright highlights with small radii make objects' materials look glossy. The specular light component adds realism to 3D scenes in
        /// walk-through animations because the specualar light's intensity at a given point on an object's surface depends not only on the
        /// incoming light ray angle but also on the positon of the viewer. With a moving camera, also the specular highlights move on the
        /// objects' surfaces.
        /// </remarks>
        public void SetLightSpecular(int lightInx, float4 specularColor)
        {
            /*
            _lightParams[lightInx].SpecularColor = specularColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_SPECULAR";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].SpecularColor);
           */
        }

        /// <summary>
        /// Sets the position of the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the position on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="position">The position of the light in 3D space.</param>
        public void SetLightPosition(int lightInx, float3 position)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].Position = position;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_POSITION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Position);
            */
        }

        /// <summary>
        /// Sets the direction of the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the direction on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="direction">The direction vector into which the light emits rays.</param>
        public void SetLightDirection(int lightInx, float3 direction)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].Direction = direction;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIRECTION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Direction);
            */
        }

        /// <summary>
        /// Sets the opening angle of the spot light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the direction on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="angle">The opening angle of the spotlight in degree.</param>
        public void SetLightSpotAngle(int lightInx, float angle)
        {
            throw new NotImplementedException("Removed Single Pass Light Support");
            /*
            _lightParams[lightInx].Angle = -(float)System.Math.Cos(angle);
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_SPOTANGLE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Angle);
            */
        }

        #endregion

        #region Shader related Members

        /// <summary>
        /// Gets the current shader.
        /// </summary>
        /// <value>
        /// The current shader.
        /// </value>
        public ShaderProgram CurrentShader
        {
            get { return _currentShader; }
        }

        /// <summary>
        /// Creates a shader object from vertex shader source code and pixel shader source code.
        /// </summary>
        /// <param name="vs">A string containing the vertex shader source.</param>
        /// <param name="ps">A string containing the pixel (fragment) shader source code.</param>
        /// <returns>A shader program object identifying the combination of the given vertex and pixel shader.</returns>
        /// <remarks>
        /// Currently only shaders in GLSL (or rather GLSL/ES) source language(s) are supported.
        /// The result is already compiled to code executable on the GPU. <see cref="RenderContext.SetShader(ShaderProgram)"/>
        /// to activate the result as the current shader used for rendering geometry passed to the RenderContext.
        /// </remarks>
        public ShaderProgram CreateShader(string vs, string ps)
        {
            var sp = new ShaderProgram(_rci, _rci.CreateShader(vs, ps));

            /*
                sp.ShaderParamHandlesImp = new ShaderParamHandleImp[MatrixParamNames.Length];
                for (int i=0; i < MatrixParamNames.Length; i++)
                {
                sp.ShaderParamHandlesImp[i] = _rci.GetShaderParamHandle(sp.Spi, MatrixParamNames[i]);
                }
            */

            return sp;
        }

        /// <summary>
        /// Activates the passed shader program as the current shader for geometry rendering.
        /// </summary>
        /// <param name="program">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        /// <seealso cref="RenderContext.CreateShader"/>
        /// <seealso cref="RenderContext.Render(Mesh)"/>
        public void SetShader(ShaderProgram program)
        {
            _updatedShaderParams = false;

            if (_currentShader != program)
            {
                _currentShader = program;
                _rci.SetShader(program._spi);
            }
            UpdateShaderParams();
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

        // Pass thru
        /// <summary>
        /// Returns an identifiyer for the named (uniform) parameter used in the specified shader program.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> using the parameter.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns>A <see cref="IShaderParam"/> object to identify the given parameter in subsequent calls to SetShaderParam.</returns>
        /// <remarks>
        /// The returned handle can be used to assign values to a (uniform) shader paramter.
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
        [JSChangeName("SetShaderParam1f")]
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
        [JSChangeName("SetShaderParam2f")]
        public void SetShaderParam(IShaderParam param, float2 val)
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
        [JSChangeName("SetShaderParam3f")]
        public void SetShaderParam(IShaderParam param, float3 val)
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
        [JSChangeName("SetShaderParam4f")]
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
        [JSChangeName("SetShaderParam4fArray")]
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
        [JSChangeName("SetShaderParamfloat4x4")]
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
        [JSChangeName("SetShaderParamMtx4fArray")]
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
        [JSChangeName("SetShaderParamI")]
        public void SetShaderParam(IShaderParam param, int val)
        {
            _rci.SetShaderParam(param, val);
        }

        #endregion

        #region Render releated Members

        /// <summary>
        /// Apply a single render state to the render context. All subsequent rendering will be
        /// performed using the currently set state unless it is changed to a different value.
        /// </summary>
        /// <param name="renderState">One of the <see cref="RenderState"/> enumaration values.</param>
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

        public uint GetRenderState(RenderState renderState)
        {
            return _rci.GetRenderState(renderState);
        }


        /// <summary>
        /// Sets the RenderTarget, if texture is null rendertarget is the main screen, otherwise the picture will be rendered onto given texture
        /// </summary>
        /// <param name="texture">The texture as target</param>
        /// <param name="deferredNormalPass">If this is true, the rendertarget is set to the main screen,
        /// but before the content of the first pass's z-Buffer is copied to main screen buffer (needed for G-Buffer).</param>
        public void SetRenderTarget(ITexture texture, bool deferredNormalPass = false)
        {
            _rci.SetRenderTarget(texture, deferredNormalPass);
        }



        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="m">The mesh that should be rendered.</param>
        /// <remarks>
        /// Passes geometry to be pushed through the rendering pipeline. <see cref="Mesh"/> for a description how geometry is made up.
        /// The geometry is transformed and rendered by the currently active shader program.
        /// </remarks>
        public void Render(Mesh m)
        {
            if (m._meshImp == null)
                m._meshImp = _rci.CreateMeshImp();

            if (m.Colors != null && m.Colors.Length != 0 && !m.ColorsSet)
                _rci.SetColors(m._meshImp, m.Colors);

            /*
             * Not using tangent space normals right now
             * 
             * if (NeedTangents
                && (m.Vertices != null && m.UVs != null && m.Normals != null)
                && (m.Vertices.Length != 0 && m.UVs.Length != 0 && m.Normals.Length != 0)
                && !(m.VerticesSet && m.UVsSet && m.NormalsSet))
            {
                // This will set vertices, uvs and normals and also calculate tangents and bitangents
                _rci.SetVertexData(m._meshImp, m.Vertices, m.UVs, m.Normals);
            }
            else*/
            {
                if (m.Vertices != null && m.Vertices.Length != 0 && !m.VerticesSet)
                    _rci.SetVertices(m._meshImp, m.Vertices);

                if (m.UVs != null && m.UVs.Length != 0 && !m.UVsSet)
                    _rci.SetUVs(m._meshImp, m.UVs);

                if (m.Normals != null && m.Normals.Length != 0 && !m.NormalsSet)
                    _rci.SetNormals(m._meshImp, m.Normals);
            }

            if (m.BoneIndices != null && m.BoneIndices.Length != 0 && !m.BoneIndicesSet)
                _rci.SetBoneIndices(m._meshImp, m.BoneIndices);

            if (m.BoneWeights != null && m.BoneWeights.Length != 0 && !m.BoneWeightsSet)
                _rci.SetBoneWeights(m._meshImp, m.BoneWeights);

            if (m.Triangles != null && m.Triangles.Length != 0 && !m.TrianglesSet)
                _rci.SetTriangles(m._meshImp, m.Triangles);

            _rci.Render(m._meshImp);
        }
      
        public uint GetHardwareCapabilities(HardwareCapability capability)
        {
            return _rci.GetHardwareCapabilities(capability);
        }

        #endregion

        #region Other Members
        /// <summary>
        /// This method returns the color of one or more pixels from the backbuffer.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns>The requested rectangular area</returns>
        public ImageData GetPixelColor(int x, int y, int w, int h)
        {
            return _rci.GetPixelColor(x, y, w, h);
        }

        /// <summary>
        /// This method returns depth value from the depthbuffer at a given coordinate.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <returns></returns>
        public float GetPixelDepth(int x, int y)
        {
            return _rci.GetPixelDepth(x, y);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug lines enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug lines enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool DebugLinesEnabled
        {
            get { return _debugLinesEnabled; }
            set { _debugLinesEnabled = value; }
        }

        /// <summary>
        /// Draws a Debug Line in 3D Space by using a start and end point (float3).
        /// </summary>
        /// <param name="start">The startpoint of the DebugLine.</param>
        /// <param name="end">The endpoint of the DebugLine.</param>
        /// <param name="color">The color of the DebugLine.</param>
        public void DebugLine(float3 start, float3 end, float4 color)
        {
            if (_debugLinesEnabled)
            {
                start /= 2;
                end /= 2;

                var oldShader = _currentShader;
                SetShader(_debugShader);

                SetShaderParam(_currentShaderParams.FUSEE_MVP, ModelViewProjection);
                SetShaderParam(_debugColor, color);

                _rci.DebugLine(start, end, color);

                if (oldShader != null)
                    SetShader(oldShader);
            }
        }

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
        public void Clear(ClearFlags flags)
        {
            _rci.Clear(flags);
        }

        /// <summary>
        /// Gets the content of the buffer and passes it to the <see cref="IRenderCanvasImp"/>.
        /// </summary>
        /// <param name="quad">The <see cref="Rectangle"/>.</param>
        /// <param name="texId">The <see cref="ITexture"/>.</param>
        public void GetBufferContent(Rectangle quad, ITexture texId)
        {
            _rci.GetBufferContent(quad, texId);
        }

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
        public void Viewport(int x, int y, int width, int height)
        {
            ViewportWidth = width;
            ViewportHeight = height;

            _rci.Viewport(x, y, width, height);
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

}
