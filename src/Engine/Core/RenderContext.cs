using System;
using System.Collections.Generic;
using JSIL.Meta;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// The render context contains all functions necessary to manipulate the underlying rendering hardware. Use this class' elements
    /// to render geometry to the RenderCanvas associated with this context. If you are worked with OpenGL or DirectX before you will find
    /// many similarities in this class' methods and properties.
    /// </summary>
    public class RenderContext
    {
        private readonly IRenderContextImp _rci;

        private ShaderProgram _currentShader;
        private MatrixParamNames _currentShaderParams;
        private ShaderProgram _debugShader;
        private readonly Light[] _lightParams;
        private readonly LightParamNames[] _lightShaderParams;

        private bool _updatedShaderParams;

        internal struct MatrixParamNames
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
            // ReSharper restore InconsistentNaming
        };

        internal struct LightParamNames
        {
            // ReSharper disable InconsistentNaming
            public IShaderParam FUSEE_L_AMBIENT;
            public IShaderParam FUSEE_L_DIFFUSE;
            public IShaderParam FUSEE_L_SPECULAR;
            public IShaderParam FUSEE_L_POSITION;
            public IShaderParam FUSEE_L_DIRECTION;
            // ReSharper restore InconsistentNaming
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rci"></param>
        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            View = float4x4.Identity;
            ModelView = float4x4.Identity;
            Projection = float4x4.Identity;

            _lightParams = new Light[8];
            _lightShaderParams = new LightParamNames[8];
            _debugShader = MoreShaders.GetShader("oneColor",this);
            _updatedShaderParams = false;
        }

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


        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, String bgColor)
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

        /// <summary>
        /// Creates a new texture and binds it to the shader.
        /// </summary>
        /// <remarks>
        /// Method should be called after LoadImage method to process
        /// the BitmapData an make them available for the shader.
        /// </remarks>
        /// <param name="imgData">An ImageData struct, containing necessary information for the upload to the graphics card.</param>
        /// <returns>
        /// An ITexture that can be used for texturing in the shader.
        /// </returns>
        public ITexture CreateTexture(ImageData imgData)
        {
            return _rci.CreateTexture(imgData);
        }

        public ITexture DisableTexture()
        {
            return _rci.CreateTexture(CreateImage(1, 1, "white"));
        }

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
        public ImageData LoadImage(String filename)
        {
            return _rci.LoadImage(filename);
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

        public ShaderProgram CurrentShader
        {
            get { return _currentShader; }
        }
        //directional or Point- Light
        public void SetLight(float3 v3, float4 color, int type, int id)
        {
            switch (type)
            {
                case 0:
                    SetLightActive(id, 1);
                    SetLightAmbient(id, color);
                    SetLightDiffuse(id, color);
                    SetLightSpecular(id, color);
                    SetLightDirection(id, v3);
                    break;
                case 1:
                    SetLightActive(id, 1);
                    SetLightAmbient(id, color);
                    SetLightDiffuse(id, color);
                    SetLightSpecular(id, color);
                    SetLightPosition(id, v3);
                    break;
            }
        }

        //Spotlight with position AND direction
        public void SetLight(float3 position, float3 direction, float4 color, int type, int id)
        {
            SetLightActive(id, 1);
            SetLightAmbient(id, color);
            SetLightDiffuse(id, color);
            SetLightSpecular(id, color);
            SetLightPosition(id, position);
            SetLightDirection(id, direction);
        }

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
                _modelView = _model * _view;

                UpdateCurrentShader();

                _rci.ModelView = _modelView;
            }
        }

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
                _modelView = _model * _view;

                UpdateCurrentShader();
                _rci.ModelView = _modelView;

            } //TODO: Flags
        }

        /// <summary>
        /// The ModelView matrix used by the rendering pipeline.
        /// </summary>
        /// <value>
        /// The 4x4 ModelView matrix defining the transformation applied to model coordinates yielding view coordinates.
        /// </value>
        /// <remarks>
        /// Model coordinates are the coordinates directly taken from the model (the mesh geometry - <see cref="Fusee.Engine.Mesh"/>). The rendering pipeline
        /// transforms these coordinates into View coordinates. Further down the pipeline the coordinates will be transformed to screen coordinates to allow the
        /// geometry to be rendered to pixel positions on the screen. The ModelView matrix defines the transformations performed on the original model coordinates
        /// to yield view coordinates. In most cases the matrix is a composition of several translations, rotations, and scale operations.
        /// </remarks>
        public float4x4 ModelView
        {
            get
            {
                return _modelView;
            }
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

                _rci.ModelView = value;
            }
        }


        /// <summary>
        /// The projection matrix used by the rendering pipeline
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="Fusee.Engine.RenderContext.ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry wihth the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        public float4x4 Projection
        {
            get
            {
                return _projection;
            }
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

                _rci.Projection = value;
            }
        }


        /// <summary>
        /// The combination of the ModelView and Projection matrices.
        /// </summary>
        /// <value>
        /// The 4x4 matrix resulting from the matrix multiplaction of the ModelView and the Projection matrix.
        /// </value>
        /// <remarks>
        /// <see cref="Fusee.Engine.RenderContext.ModelView"/> and <see cref="Fusee.Engine.RenderContext.Projection"/>.
        /// </remarks>
        public float4x4 ModelViewProjection
        {
            get
            {
                if (!_modelViewProjectionOk)
                {
                    _modelViewProjection = ModelView * Projection;
                    _modelViewProjectionOk = true;
                }
                return _modelViewProjection;
            }
        }

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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelView"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransModelView"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.Projection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransProjection"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelViewProjection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransModelViewProjection"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelView"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvModelView"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.Projection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvProjection"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelViewProjection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvModelViewProjection"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelView"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvModelView"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransModelView"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.Projection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvProjection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransProjection"/>
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
        /// <seealso cref="Fusee.Engine.RenderContext.ModelViewProjection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.InvModelViewProjection"/>
        /// <seealso cref="Fusee.Engine.RenderContext.TransModelViewProjection"/>
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

        private void UpdateCurrentShader()
        {
            // Todo: Check if the respective matrix was changed since last accessed by the currently updated shader
            // Todo: and set only if matrix was changed.

            if (_currentShader == null)
                return;

            if (!_updatedShaderParams)
                UpdateShaderParams();

            // Normal versions of MV and P
            if (_currentShaderParams.FUSEE_M != null)
                SetShaderParam(_currentShaderParams.FUSEE_M, Model);

            if (_currentShaderParams.FUSEE_V != null)
                SetShaderParam(_currentShaderParams.FUSEE_V, View);

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

            for (var i = 0; i < 8; i++)
            {
                if (_lightShaderParams[i].FUSEE_L_AMBIENT != null)
                    SetShaderParam(_lightShaderParams[i].FUSEE_L_AMBIENT, _lightParams[i].AmbientColor);

                if (_lightShaderParams[i].FUSEE_L_DIFFUSE != null)
                    SetShaderParam(_lightShaderParams[i].FUSEE_L_DIFFUSE, _lightParams[i].DiffuseColor);

                if (_lightShaderParams[i].FUSEE_L_SPECULAR != null)
                    SetShaderParam(_lightShaderParams[i].FUSEE_L_SPECULAR, _lightParams[i].SpecularColor);

                if (_lightShaderParams[i].FUSEE_L_POSITION != null)
                    SetShaderParam(_lightShaderParams[i].FUSEE_L_POSITION, _lightParams[i].Position);

                if (_lightShaderParams[i].FUSEE_L_DIRECTION != null)
                    SetShaderParam(_lightShaderParams[i].FUSEE_L_DIRECTION, _lightParams[i].Direction);
            }
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

            for (int i = 0; i < 8; i++)
            {
                _lightShaderParams[i].FUSEE_L_AMBIENT = _currentShader.GetShaderParam("FUSEE_L" + i + "_AMBIENT");
                _lightShaderParams[i].FUSEE_L_DIFFUSE = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIFFUSE");
                _lightShaderParams[i].FUSEE_L_SPECULAR = _currentShader.GetShaderParam("FUSEE_L" + i + "_SPECULAR");
                _lightShaderParams[i].FUSEE_L_POSITION = _currentShader.GetShaderParam("FUSEE_L" + i + "_POSITION");
                _lightShaderParams[i].FUSEE_L_DIRECTION = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIRECTION");
            }

            _updatedShaderParams = true;
            UpdateCurrentShader();
        }

        /// <summary>
        /// Activates the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to activate. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="active">1 - activate the light. 0 - deactiv</param>
        public void SetLightActive(int lightInx, int active)
        {
            _lightParams[lightInx].Active = active;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_ACTIVE";

            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Active);
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
            _lightParams[lightInx].AmbientColor = ambientColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_AMBIENT";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].AmbientColor);
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
            _lightParams[lightInx].DiffuseColor = diffuseColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIFFUSE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].DiffuseColor);
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
            _lightParams[lightInx].SpecularColor = specularColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_SPECULAR";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].SpecularColor);
        }

        /// <summary>
        /// Sets the position of the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the position on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="position">The position of the light in 3D space.</param>
        public void SetLightPosition(int lightInx, float3 position)
        {
            _lightParams[lightInx].Position = position;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_POSITION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Position);
        }

        /// <summary>
        /// Sets the direction of the light with the given index.
        /// </summary>
        /// <param name="lightInx">The light to set the direction on. Can range from 0 to 7. Up to eight lights are supported.</param>
        /// <param name="direction">The direction vector into which the light emits rays.</param>
        public void SetLightDirection(int lightInx, float3 direction)
        {
            _lightParams[lightInx].Direction = direction;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIRECTION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Direction);
        }

        /// <summary>
        /// Creates a shader object from vertex shader source code and pixel shader source code.
        /// </summary>
        /// <param name="vs">A string containing the vertex shader source.</param>
        /// <param name="ps">A string containing the pixel (fragment) shader source code.</param>
        /// <returns>A shader program object identifying the combination of the given vertex and pixel shader.</returns>
        /// <remarks>
        /// Currently only shaders in GLSL (or rather GLSL/ES) source language(s) are supported.
        /// The result is already compiled to code executable on the GPU. <see cref="Fusee.Engine.RenderContext.SetShader(ShaderProgram)"/>
        /// to activate the result as the current shader used for rendering geometry passed to the RenderContext.
        /// </remarks>
        public ShaderProgram CreateShader(string vs, string ps)
        {
            var sp = new ShaderProgram(_rci, _rci.CreateShader(vs, ps)) { _spi = _rci.CreateShader(vs, ps) };
            /*
sp.ShaderParamHandlesImp = new ShaderParamHandleImp[MatrixParamNames.Length];
for (int i=0; i < MatrixParamNames.Length; i++)
{
sp.ShaderParamHandlesImp[i] = _rci.GetShaderParamHandle(sp.Spi, MatrixParamNames[i]);
}
* */
            return sp;
        }

        /// <summary>
        /// Activates the passed shader program as the current shader for geometry rendering.
        /// </summary>
        /// <param name="program">The shader to apply to mesh geometry subsequently passed to the RenderContext</param>
        /// <seealso cref="Fusee.Engine.RenderContext.CreateShader"/>
        /// <seealso cref="Fusee.Engine.RenderContext.Render(Mesh)"/>
        public void SetShader(ShaderProgram program)
        {
            _updatedShaderParams = false;

            _currentShader = program;
            _rci.SetShader(program._spi);

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
        /// <param name="program">The shader program using the parameter.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns>A handle object to identify the given parameter in subsequent calls to SetShaderParam.</returns>
        /// <remarks>
        /// The returned handle can be used to assign values to a (uniform) shader paramter.
        /// </remarks>
        /// <seealso cref="SetShaderParam(IShaderParam,float)"/>
        public IShaderParam GetShaderParam(ShaderProgram program, string paramName)
        {
            return _rci.GetShaderParam(program._spi, paramName);
        }

        /// <summary>
        /// Gets the value of a shader parameter.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="handle">The handle.</param>
        /// <returns>The float value.</returns>
        public float GetParamValue(ShaderProgram program, IShaderParam handle)
        {
            return _rci.GetParamValue(program._spi, handle);
        }

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
        public void SetShaderParam(IShaderParam param, float val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            _rci.SetShaderParam(param, val);
        }

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
        public void SetShaderParam(IShaderParam param, int val)
        {
            _rci.SetShaderParam(param, val);
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

            if (m.Vertices != null && m.Vertices.Length != 0 && !m.VerticesSet)
                _rci.SetVertices(m._meshImp, m.Vertices);

            if (m.Colors != null && m.Colors.Length != 0 && !m.ColorsSet)
                _rci.SetColors(m._meshImp, m.Colors);

            if (m.UVs != null && m.UVs.Length != 0 && !m.NormalsSet)
                _rci.SetUVs(m._meshImp, m.UVs);

            if (m.Normals != null && m.Normals.Length != 0 && !m.NormalsSet)
                _rci.SetNormals(m._meshImp, m.Normals);

            if (m.Triangles != null && m.Triangles.Length != 0 && !m.TrianglesSet)
                _rci.SetTriangles(m._meshImp, m.Triangles);

            _rci.Render(m._meshImp);
        }

        public void DebugLine(float3 start, float3 end, float3 color)
        {
            SetShader(_debugShader);
            SetShaderParam(_currentShaderParams.FUSEE_MVP, ModelViewProjection);
            IShaderParam col = _debugShader.GetShaderParam("Col");
            SetShaderParam(col, color);
            _rci.DebugLine(start, end, color);
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
            _rci.Viewport(x, y, width, height);
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            _rci.ColorMask(red, green, blue, alpha);
        }

        public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            _rci.Frustum(left, right, bottom, top, zNear, zFar);
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
    }

}