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
        public List<IShaderParam> ParamListe;

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
                ShaderProgram sp = Shaders.GetShader("multiLight",rc);
                _shininess = 64;
                rc.SetShader(sp);
            }
            if (s == "chess")
            {
                ShaderProgram sp = Shaders.GetShader("chess", rc);
                _shininess = 64;
                ParamListe.Add(sp.GetShaderParam("darkColor"));
                ParamListe.Add(sp.GetShaderParam("brightColor"));
                ParamListe.Add(sp.GetShaderParam("chessSize"));
                ParamListe.Add(sp.GetShaderParam("smoothFactor"));
                rc.SetShaderParam(ParamListe[0], new float3(1,1,0));
                rc.SetShaderParam(ParamListe[1], new float3(1, 1, 0));
                rc.SetShaderParam(ParamListe[2], 1.0f);
                rc.SetShaderParam(ParamListe[3], 1.0f);
                rc.SetShader(sp);
            }

            ShaderProgram shp = Shaders.GetShader("multiLight", rc);
            _shininess = 64;
            rc.SetShader(shp);
        }
    }   
}
