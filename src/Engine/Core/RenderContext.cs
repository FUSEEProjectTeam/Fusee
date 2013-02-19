using System;
using System.Collections.Generic;
using Fusee.Engine;
using JSIL.Meta;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderContext
    {
       
        private IRenderContextImp _rci;
        private ShaderProgram _currentShader;
        private Light[] _lightParams = new Light[8];

        /*
        public static readonly string[] MatrixParamNames  = {
            "FUSEE_MV",
            "FUSEE_P",
            "FUSEE_MVP",

            "FUSEE_I_MV",
            "FUSEE_I_P",
            "FUSEE_I_MVP",

            "FUSEE_T_MV",
            "FUSEE_T_P",
            "FUSEE_T_MVP",

            "FUSEE_IT_MV",
            "FUSEE_IT_P",
            "FUSEE_IT_MVP",
        };
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext" /> class.
        /// </summary>
        /// <param name="rci">The rci.</param>
        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            ModelView = float4x4.Identity;
            Projection = float4x4.Identity;
        }

        /// <summary>
        /// The camera
        /// </summary>
        public float4x4 Camera; // TODO: Implement Camera. Temporary solution!!
        private float4x4 _modelView;
        private float4x4 _projection;
        private float4x4 _modelViewProjection;
        private float4x4 _invModelView;
        private float4x4 _invProjection;
        private float4x4 _invModelViewProjection;
        private float4x4 _invTransModelView;
        private float4x4 _invTransProjection;
        private float4x4 _invTransModelViewProjection;
        private float4x4 _transModelView;
        private float4x4 _transProjection;
        private float4x4 _transModelViewProjection;
        private bool _modelViewProjectionOk;
        private bool _invModelViewOk;
        private bool _invProjectionOk;
        private bool _invModelViewProjectionOk;
        private bool _invTransModelViewOk;
        private bool _invTransProjectionOk;
        private bool _invTransModelViewProjectionOk;
        private bool _transModelViewOk;
        private bool _transProjectionOk;
        private bool _transModelViewProjectionOk;


        
        public ITexture CreateTexture(String filename)
        {
            return _rci.CreateTexture(filename);
        }

        /// <summary>
        /// Creates a new texture and binds it to the shader
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

        /// <summary>
        /// Loads an image file from disk and creates a new Bitmap-object out of it.
        /// </summary>
        /// <remarks>
        /// This is the first step for the texturing Process.
        /// The Bitmap-bits get locked in the memory and are made available for
        /// further processing. The returned ImageData-Struct can be used in the 
        /// CreateTexture method
        /// </remarks>
        /// <param name="filename">Path to the image file</param>
        /// <returns>
        /// An ImageData struct with all necessary information for the texture-binding process
        /// </returns>
        public ImageData LoadImage(String filename)
        {
            return _rci.LoadImage(filename);
        }

        /// <summary>
        /// Sets a Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture() method</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
            _rci.SetShaderParamTexture(param, texId);
        }



        /// <summary>
        /// Gets or sets the model view.
        /// </summary>
        /// <value>
        /// The model view.
        /// </value>
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

                // Invalidate derived matrices
                _modelViewProjectionOk = false;
                _invModelViewOk = false;
                _invModelViewProjectionOk = false;
                _invTransModelViewOk = false;
                _invTransModelViewProjectionOk = false;
                _transModelViewOk = false;
                _transModelViewProjectionOk = false;

                UpdateCurrentShader();

                _rci.ModelView = value;
            }
        }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
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
        /// Gets the model view projection.
        /// </summary>
        /// <value>
        /// The model view projection.
        /// </value>
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

        /// <summary>
        /// Gets the inv model view.
        /// </summary>
        /// <value>
        /// The inv model view.
        /// </value>
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
        /// Gets the inv projection.
        /// </summary>
        /// <value>
        /// The inv projection.
        /// </value>
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
        /// Gets the inv model view projection.
        /// </summary>
        /// <value>
        /// The inv model view projection.
        /// </value>
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
        /// Gets the trans model view.
        /// </summary>
        /// <value>
        /// The trans model view.
        /// </value>
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
        /// Gets the trans projection.
        /// </summary>
        /// <value>
        /// The trans projection.
        /// </value>
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
        /// Gets the trans model view projection.
        /// </summary>
        /// <value>
        /// The trans model view projection.
        /// </value>
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
        /// Gets the inv trans model view.
        /// </summary>
        /// <value>
        /// The inv trans model view.
        /// </value>
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
        /// Gets the inv trans projection.
        /// </summary>
        /// <value>
        /// The inv trans projection.
        /// </value>
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
        /// Gets the inv trans model view projection.
        /// </summary>
        /// <value>
        /// The inv trans model view projection.
        /// </value>
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


        /// <summary>
        /// Updates the current shader.
        /// </summary>
        private void UpdateCurrentShader()
        {
            // Todo: Check if the respective matrix was changed since last accessed by the currently updated shader
            // Todo: and set only if matrix was changed.
            if (_currentShader == null)
            {
                // TODO: log that no shader was set
                return;
            }

            IShaderParam sp;
            // Normal versions of MV and P       
            if ((sp = _currentShader.GetShaderParam("FUSEE_MV")) != null)
                SetShaderParam(sp, ModelView);

            if ((sp = _currentShader.GetShaderParam("FUSEE_P")) != null)
                SetShaderParam(sp, Projection);

            if ((sp = _currentShader.GetShaderParam("FUSEE_MVP")) != null)
                SetShaderParam(sp, ModelViewProjection);

            // Inverted versions
            if ((sp = _currentShader.GetShaderParam("FUSEE_IMV")) != null)
                SetShaderParam(sp, InvModelView);

            if ((sp = _currentShader.GetShaderParam("FUSEE_IP")) != null)
                SetShaderParam(sp, InvProjection);

            if ((sp = _currentShader.GetShaderParam("FUSEE_IMVP")) != null)
                SetShaderParam(sp, InvModelViewProjection);

            // Transposed versions
            if ((sp = _currentShader.GetShaderParam("FUSEE_TMV")) != null)
                SetShaderParam(sp, TransModelView);

            if ((sp = _currentShader.GetShaderParam("FUSEE_TP")) != null)
                SetShaderParam(sp, TransProjection);

            if ((sp = _currentShader.GetShaderParam("FUSEE_TMVP")) != null)
                SetShaderParam(sp, TransModelViewProjection);

            // Inverted and transposed versions
            if ((sp = _currentShader.GetShaderParam("FUSEE_ITMV")) != null)
                SetShaderParam(sp, InvTransModelView);

            if ((sp = _currentShader.GetShaderParam("FUSEE_ITP")) != null)
                SetShaderParam(sp, InvTransProjection);

            if ((sp = _currentShader.GetShaderParam("FUSEE_ITMVP")) != null)
                SetShaderParam(sp, InvTransModelViewProjection);

            for (int i = 0; i < 8; i++)
            {
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_AMBIENT")) != null)
                    SetShaderParam(sp, _lightParams[i].AmbientColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIFFUSE")) != null)
                    SetShaderParam(sp, _lightParams[i].DiffuseColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_SPECULAR")) != null)
                    SetShaderParam(sp, _lightParams[i].SpecularColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_POSITION")) != null)
                    SetShaderParam(sp, _lightParams[i].Position);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIRECTION")) != null)
                    SetShaderParam(sp, _lightParams[i].Direction);
            }
        }

        /// <summary>
        /// Sets active light.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="active">The active.</param>
        public void SetLightActive(int lightInx, float active)
        {
            _lightParams[lightInx].Active = active;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_ACTIVE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Active);
        }

        /// <summary>
        /// Sets the ambient light.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="ambientColor">Color of the ambient.</param>
        public void SetLightAmbient(int lightInx, float4 ambientColor)
        {
            _lightParams[lightInx].AmbientColor = ambientColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_AMBIENT";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].AmbientColor);
        }

        /// <summary>
        /// Sets the diffuse light.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="diffuseColor">Color of the diffuse.</param>
        public void SetLightDiffuse(int lightInx, float4 diffuseColor)
        {
            _lightParams[lightInx].DiffuseColor= diffuseColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIFFUSE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].DiffuseColor);
        }

        /// <summary>
        /// Sets the specular light.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="specularColor">Color of the specular.</param>
        public void SetLightSpecular(int lightInx, float4 specularColor)
        {
            _lightParams[lightInx].SpecularColor = specularColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_SPECULAR";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].SpecularColor);
        }

        /// <summary>
        /// Sets the light position.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="position">The position.</param>
        public void SetLightPosition(int lightInx, float3 position)
        {
            _lightParams[lightInx].Position = position;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_POSITION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Position);
        }

        /// <summary>
        /// Sets the light direction.
        /// </summary>
        /// <param name="lightInx">The light inx.</param>
        /// <param name="direction">The direction.</param>
        public void SetLightDirection(int lightInx, float3 direction)
        {
            _lightParams[lightInx].Direction = direction;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIRECTION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].Direction);
        }

        /// <summary>
        /// Creates the shader.
        /// </summary>
        /// <param name="vs">The vertex shader.</param>
        /// <param name="ps">The pixel shader.</param>
        /// <returns></returns>
        public ShaderProgram CreateShader(string vs, string ps)
        {
            ShaderProgram sp = new ShaderProgram(_rci, _rci.CreateShader(vs, ps));
            sp._spi = _rci.CreateShader(vs, ps);
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
        /// Sets the shader.
        /// </summary>
        /// <param name="program">The program.</param>
        public void SetShader(ShaderProgram program)
        {
            _currentShader = program;
            _rci.SetShader(program._spi);
            //UpdateCurrentShader();
        }

        /// <summary>
        /// Gets the shader param list.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <returns></returns>
        public IEnumerable<ShaderParamInfo> GetShaderParamList(ShaderProgram program)
        {
            return _rci.GetShaderParamList(program._spi);
        }

        // Pass thru
        /// <summary>
        /// Gets the shader parameter.
        /// </summary>
        /// <remarks>
        /// Can be used to assign an a shader paramter to an IShaderParam
        /// </remarks>
        /// <param name="program">The program.</param>
        /// <param name="paramName">Name of the shader parameter.</param>
        /// <returns></returns>
        public IShaderParam GetShaderParam(ShaderProgram program, string paramName)
        {
            return _rci.GetShaderParam(program._spi, paramName);
        }

        /// <summary>
        /// Gets the value of a shader parameter.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <param name="handle">The handle.</param>
        /// <returns></returns>
        public float GetParamValue(ShaderProgram program, IShaderParam handle)
        {
            return _rci.GetParamValue(program._spi, handle);
        }

        /// <summary>
        /// Sets the shader parameter to a float-value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The float-value that should be assigned to the shader parameter.</param>
        [JSChangeName("SetShaderParam1f")]
        public void SetShaderParam(IShaderParam param, float val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float2 value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The float2 value that should be assigned to the shader parameter.</param>
        [JSChangeName("SetShaderParam2f")]
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float3 value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The float3 value that should be assigned to the shader parameter.</param>
        [JSChangeName("SetShaderParam3f")]
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float4 value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The float4 value that should be assigned to the shader parameter.</param>
        [JSChangeName("SetShaderParam4f")]
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a float4x4 value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The float4x4 value that should be assigned to the shader parameter.</param>
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Sets the shader parameter to a Int value.
        /// </summary>
        /// <param name="param">The shader parameter name.</param>
        /// <param name="val">The Int value that should be assigned to the shader parameter.</param>
        [JSChangeName("SetShaderParamI")]
        public void SetShaderParam(IShaderParam param, int val)
        {
            _rci.SetShaderParam(param, val);
        }

        /// <summary>
        /// Clears the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public void Clear(ClearFlags flags)
        {
            _rci.Clear(flags);
        }

        /// <summary>
        /// Renders the specified mesh.
        /// </summary>
        /// <param name="m">The mesh that should be rendered.</param>
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

        /// <summary>
        /// Viewports the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Viewport(int x, int y, int width, int height)
        {
            _rci.Viewport(x, y, width, height);
        }

        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        public float4 ClearColor
        {
            set { _rci.ClearColor = value; }
            get { return _rci.ClearColor; }
        }

        /// <summary>
        /// Gets or sets the clear depth.
        /// </summary>
        /// <value>
        /// The clear depth.
        /// </value>
        public float ClearDepth
        {
            set { _rci.ClearDepth = value; }
            get { return _rci.ClearDepth; }
        }
    }

}
