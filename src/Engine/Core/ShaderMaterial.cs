using Fusee.Math;
using Fusee.Engine;

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
    }   
}
