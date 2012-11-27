using System;
using Fusee.Engine;
using JSIL.Meta;
using Fusee.Math;

namespace Fusee.Engine
{
    public class RenderContext
    {
        private IRenderContextImp _rci;
        private ShaderProgram _currentShader;
        private LightParams[] _lightParams = { new LightParams(), new LightParams() };

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

        public RenderContext(IRenderContextImp rci)
        {
            _rci = rci;
            ModelView = float4x4.Identity;
            Projection = float4x4.Identity;
        }

        // Settable matrices
        private float4x4 _modelView;
        private float4x4 _projection;

        // Derived matrices
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
                    SetShaderParam(sp, _lightParams[i].ambientColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIFFUSE")) != null)
                    SetShaderParam(sp, _lightParams[i].diffuseColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_SPECULAR")) != null)
                    SetShaderParam(sp, _lightParams[i].specularColor);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_POSITION")) != null)
                    SetShaderParam(sp, _lightParams[i].position);
                if ((sp = _currentShader.GetShaderParam("FUSEE_L" + i + "_DIRECTION")) != null)
                    SetShaderParam(sp, _lightParams[i].direction);
            }
        }

        public void SetLightActive(int lightInx, float active)
        {
            _lightParams[lightInx].active = active;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_ACTIVE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].active);
        }

        public void SetLightAmbient(int lightInx, float4 ambientColor)
        {
            _lightParams[lightInx].ambientColor = ambientColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_AMBIENT";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].ambientColor);
        }

        public void SetLightDiffuse(int lightInx, float4 diffuseColor)
        {
            _lightParams[lightInx].diffuseColor = diffuseColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIFFUSE";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].diffuseColor);
        }

        public void SetLightSpecular(int lightInx, float4 specularColor)
        {
            _lightParams[lightInx].specularColor = specularColor;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_SPECULAR";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].specularColor);
        }

        public void SetLightPosition(int lightInx, float3 position)
        {
            _lightParams[lightInx].position = position;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_POSITION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].position);
        }

        public void SetLightDirection(int lightInx, float3 direction)
        {
            _lightParams[lightInx].direction = direction;
            IShaderParam sp;
            string paramName = "FUSEE_L" + lightInx + "_DIRECTION";
            if ((sp = _currentShader.GetShaderParam(paramName)) != null)
                SetShaderParam(sp, _lightParams[lightInx].direction);
        }
        
        public ShaderProgram CreateShader(string vs, string ps)
        {
            ShaderProgram sp = new ShaderProgram(_rci, _rci.CreateShader(vs, ps));
            sp._spi = _rci.CreateShader(vs, ps);
            return sp;
        }

        public void SetShader(ShaderProgram program)
        {
            _currentShader = program;
            _rci.SetShader(program._spi);
            UpdateCurrentShader();
        }


        // Pass thru
        public IShaderParam GetShaderParam(ShaderProgram program, string paramName)
        {
            return _rci.GetShaderParam(program._spi, paramName);
        }

        [JSChangeName("SetShaderParam1f")]
        public void SetShaderParam(IShaderParam param, float val)
        {
            _rci.SetShaderParam(param, val);
        }

        [JSChangeName("SetShaderParam2f")]
        public void SetShaderParam(IShaderParam param, float2 val)
        {
            _rci.SetShaderParam(param, val);
        }

        [JSChangeName("SetShaderParam3f")]
        public void SetShaderParam(IShaderParam param, float3 val)
        {
            _rci.SetShaderParam(param, val);
        }

        [JSChangeName("SetShaderParam4f")]
        public void SetShaderParam(IShaderParam param, float4 val)
        {
            _rci.SetShaderParam(param, val);
        }

        [JSChangeName("SetShaderParamMtx4f")]
        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            _rci.SetShaderParam(param, val);
        }
       
        public void Clear(ClearFlags flags)
        {
            _rci.Clear(flags);
        }

        public void Render(Mesh m)
        {
            if (m._meshImp == null)
                m._meshImp = _rci.CreateMeshImp();

            if (m.Vertices != null && m.Vertices.Length != 0 && !m.VerticesSet)
                _rci.SetVertices(m._meshImp, m.Vertices);

            if (m.Colors != null && m.Colors.Length != 0 && !m.ColorsSet)
                _rci.SetColors(m._meshImp, m.Colors);
            
            if (m.Normals != null && m.Normals.Length != 0 && !m.NormalsSet)
                _rci.SetNormals(m._meshImp, m.Normals);

            if (m.Triangles != null && m.Triangles.Length != 0 && !m.TrianglesSet)
                _rci.SetTriangles(m._meshImp, m.Triangles);

            _rci.Render(m._meshImp);
        }

        public void Viewport(int x, int y, int width, int height)
        {
            _rci.Viewport(x, y, width, height);
        }

        public float4 ClearColor
        {
            set { _rci.ClearColor = value; }
            get { return _rci.ClearColor; }
        }

        public float ClearDepth
        {
            set { _rci.ClearDepth = value; }
            get { return _rci.ClearDepth; }
        }
    }
    class LightParams
    {
        public float active = 0.0f;
        public float4 ambientColor = new float4(1, 1, 1, 1);
        public float4 diffuseColor = new float4(1, 1, 1, 1);
        public float4 specularColor = new float4(1, 1, 1, 1);
        public float3 position = new float3(0, 0, 0);
        public float3 direction = new float3(1, 1, 1);
        public int type = 0;
    }
}
