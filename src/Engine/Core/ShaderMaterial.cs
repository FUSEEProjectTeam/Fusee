using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class ShaderMaterial
    {
        private float _emission;
        private float4 _ambient;
        private float4 _specular;
        private float4 _diffuse;
        private float _shininess;
        private ShaderProgram _sp;
        public IShaderParam param1;
        public IShaderParam param2;
        public IShaderParam param3;
        public IShaderParam param4;

        public ShaderMaterial(ShaderProgram program)
        {
            _sp = program;
        }


        public ShaderProgram GetShader()
        {
            return _sp;
        }

        public float4 GetDiffuse()
        {
            return _diffuse;
        }

        public float4 GetAmbient()
        {
            return _ambient;
        }

        public float GetShininess()
        {
            return _shininess;
        }

        public float4 GetSpecular()
        {
            return _specular;
        }

        public float GetEmmision()
        {
            return _emission;
        }

        public void SetAmbient(float4 ambient)
        {
            _ambient = ambient;
        }

        public void SetDiffuse(float4 diffuse)
        {
            _diffuse = diffuse;
        }

        public void SetSpecular(float4 specular)
        {
            _specular = specular;
        }

        public void SetShininess(float shininess)
        {
            _shininess = shininess;
        }

        public void SetEmission(float emission)
        {
            _emission = emission;
        }
        public ShaderMaterial(string s, RenderContext rc)
        {
            if (s == "multiLight")
            {
                _sp = Shaders.GetShader("multiLight",rc);
                rc.SetShader(_sp);
                _shininess = 256.0f;
                UpdateMaterial(rc);
            }
            else if (s == "chess")
            {
                _sp = MoreShaders.GetShader("chess", rc);
                _shininess = 64;
                param1 = _sp.GetShaderParam("darkColor");
                param2 = _sp.GetShaderParam("brightColor");
                param3 = _sp.GetShaderParam("chessSize");
                param4 = _sp.GetShaderParam("smoothFactor");
                rc.SetShaderParam(param1, new float3(0 0, 0));
                rc.SetShaderParam(param2, new float3(1, 1, 0));
                rc.SetShaderParam(param3, 25);
                rc.SetShaderParam(param4, 1);
                rc.SetShader(_sp);
            }
            else
            {
                ShaderProgram shp = Shaders.GetShader("multiLight", rc);
                _shininess = 64;
                rc.SetShader(shp);
            }
        }
        public void UpdateMaterial(RenderContext rc)
        {
            IShaderParam sp;
            if ((sp = _sp.GetShaderParam("FUSEE_MAT_SHININESS")) != null)
                rc.SetShaderParam(sp, _shininess);
            if ((sp = _sp.GetShaderParam("FUSEE_MAT_SPECULAR")) != null)
                rc.SetShaderParam(sp, _specular);
        }
    }   
}
