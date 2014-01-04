using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameEntity
    {
        protected float4x4 Position;
        protected float2 Rotation;
        protected float3 NRotXV;
        protected float3 NRotYV;
        protected float3 NRotZV;
        protected float Speed = 0;

        private readonly Mesh _mesh;

        private ShaderProgram _shaderProgram;
        private IShaderParam _shaderParam;
        private ITexture _iTexture;
        private float4 _color = new float4(0.5f,0.5f,0.5f,1);
        private bool _isTextureShader;

        private readonly RenderContext _rc;

        public GameEntity(String meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _rc = rc;

            Position = float4x4.Identity;
            Position = float4x4.CreateRotationX(angX) *
                        float4x4.CreateRotationY(angY) *
                        float4x4.CreateRotationZ(angZ) *
                        float4x4.CreateTranslation(posX, posY, posZ);

            UpdateNVectors();

            SetDiffuseShader();
        }

        public GameEntity(String meshPath, RenderContext rc, float3 posxyz, float3 angxyz)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _rc = rc;

            Position = float4x4.Identity;
            Position = float4x4.CreateRotationX(angxyz.x) *
                        float4x4.CreateRotationY(angxyz.y) *
                        float4x4.CreateRotationZ(angxyz.z) *
                        float4x4.CreateTranslation(posxyz.x, posxyz.y, posxyz.z);

            UpdateNVectors();
        }

        public float4x4 GetPosition()
        {
            return Position;
        }

        public void SetPosition(float4x4 position)
        {
            Position = position;
        }

        public void SetPosition(float3 position)
        {
            Position.Row3 = new float4(position, 1);
        }

        public float3 GetPositionVector()
        {
            return new float3(Position.M41, Position.M42, Position.M43);
        }

        public void SetShader(float4 color)
        {
            _color = color;
            SetDiffuseShader();
        }

        public void SetShader(float r,float g, float b, float a=1)
        {
            _color = new float4(r,g,b,a);
            SetDiffuseShader();
        }

        public void SetShader(String texturePath)
        {
            SetTextureShader(texturePath);
        }

        public void Render(float4x4 camMatrix)
        {
            _rc.SetShader(_shaderProgram);
            if (_isTextureShader)
            {
                _rc.SetShaderParamTexture(_shaderParam, _iTexture);
            }
            else
            {
                _rc.SetShaderParam(_shaderParam, _color);
            }

            _rc.ModelView = Position * camMatrix;
            _rc.Render(_mesh);
        }

        protected void UpdateNVectors()
        {
            NRotXV = float3.Normalize(new float3(Position.Row0));
            NRotYV = float3.Normalize(new float3(Position.Row1));
            NRotZV = float3.Normalize(new float3(Position.Row2));
        }

        protected void SetDiffuseShader()
        {
            _shaderProgram = MoreShaders.GetDiffuseColorShader(_rc);
            _shaderParam = _shaderProgram.GetShaderParam("color");

            _isTextureShader = false;
        }

        protected void SetTextureShader(String texturePath)
        {
            _shaderProgram = MoreShaders.GetTextureShader(_rc);
            _shaderParam = _shaderProgram.GetShaderParam("texture1");

            var imgData = _rc.LoadImage(texturePath);
            _iTexture = _rc.CreateTexture(imgData);

            _isTextureShader = true;
        }
    }
}
