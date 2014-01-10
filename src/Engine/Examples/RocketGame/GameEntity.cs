using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameEntity
    {
        protected float4x4 Position;
        protected float3 Rotation;
        protected float3 NRotXV;
        protected float3 NRotYV;
        protected float3 NRotZV;
        protected float Scale = 1;
        protected float Speed = 0;
        protected float4x4 CorrectionMatrix;

        private readonly Mesh _mesh;

        private ShaderEffect _shaderEffect;
        private ITexture _iTexture;
        private float4 _color = new float4(0.5f,0.5f,0.5f,1);

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

            CorrectionMatrix = float4x4.Identity;

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

            CorrectionMatrix = float4x4.Identity;

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
            //urks ...
            Position.Row3 = new float4(position, 1);
        }

        //Setscale and SetCorrectionMatrix are redundant but affect different parameters ... might wanna try to unify them
        public void SetScale(float scale)
        {
            Scale = scale;
        }

        //Setscale and SetCorrectionMatrix are redundant but affect different parameters ... might wanna try to unify them
        public void SetCorrectionMatrix(float4x4 corrMatrix)
        {
            CorrectionMatrix = corrMatrix;
        }

        public float3 GetPositionVector()
        {
            //urks ....
            return new float3(Position.M41, Position.M42, Position.M43);
        }

        public void SetShader(float4 color)
        {
            _color = color;
            SetDiffuseShader();
        }

        public void SetShader(float r,float g, float b, float a=1)
        {
            //urks ...
            _color = new float4(r,g,b,a);
            SetDiffuseShader();
        }

        public void SetShader(String texturePath)
        {
            SetTextureShader(texturePath);
        }
        public void SetShader(String texturePath, float4 baseColor, float4 lineColor, float2 lineWidth)
        {
            SetTextureShader(texturePath, baseColor, lineColor, lineWidth);
        }

        public void Render(float4x4 camMatrix)
        {
            _rc.ModelView = CorrectionMatrix * float4x4.Scale(Scale, Scale, Scale) * Position * camMatrix;

            _shaderEffect.RenderMesh(_mesh);
        }

        protected void UpdateNVectors()
        {
            //urks ...
            NRotXV = float3.Normalize(new float3(Position.Row0));
            NRotYV = float3.Normalize(new float3(Position.Row1));
            NRotZV = float3.Normalize(new float3(Position.Row2));
        }

        protected void SetDiffuseShader()
        {
            _shaderEffect = Shader.GetShaderEffect(_rc, _color);
        }

        protected void SetTextureShader(String texturePath)
        {
            var imgData = _rc.LoadImage(texturePath);
            _iTexture = _rc.CreateTexture(imgData);

            _shaderEffect = Shader.GetShaderEffect(_rc, _iTexture);
        }
        protected void SetTextureShader(String texturePath, float4 baseColor, float4 lineColor, float2 lineWidth)
        {
            var imgData = _rc.LoadImage(texturePath);
            _iTexture = _rc.CreateTexture(imgData);

            _shaderEffect = Shader.GetShaderEffect(_rc, _iTexture, baseColor, lineColor, lineWidth);
        }
    }
}
