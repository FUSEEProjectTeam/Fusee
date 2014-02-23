using System;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameEntity
    {
        private readonly Mesh _mesh;

        private readonly RenderContext _rc;

        protected float4x4 Position;
        protected float4x4 CorrectionMatrix = float4x4.Identity;

        private ShaderEffect _shaderEffect;
        private ITexture _iTexture1;
        private ITexture _iTexture2;
        private float4 _color = new float4(0.5f, 0.5f, 0.5f, 1);

        private FuseeSerializer _ser;

        public GameEntity(String meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
        {
            if (meshPath.Contains("protobuf"))
            {
                _ser = new FuseeSerializer();
                using (var file = File.OpenRead(meshPath))
                {
                    _mesh = _ser.Deserialize(file, null, typeof(Mesh)) as Mesh;
                }
            }
            else
            {
                _mesh = MeshReader.LoadMesh(meshPath);
            }

            _rc = rc;

            Position = float4x4.CreateRotationX(angX) *
                        float4x4.CreateRotationY(angY) *
                        float4x4.CreateRotationZ(angZ) *
                        float4x4.CreateTranslation(posX, posY, posZ);

            SetShader(_color);
        }

        public void Render(float4x4 camMatrix)
        {
            _rc.ModelView = CorrectionMatrix * Position * camMatrix;

            _shaderEffect.RenderMesh(_mesh);
        }

        public float4x4 GetPosition()
        {
            return Position;
        }
        public float3 GetPositionVector()
        {
            return new float3(Position.M41, Position.M42, Position.M43);
        }

        public void SetPosition(float4x4 position)
        {
            Position = position;
        }

        public void SetCorrectionMatrix(float4x4 corrMatrix)
        {
            CorrectionMatrix = corrMatrix;
        }

        public void SetShader(float4 color)
        {
            _color = color;
            _shaderEffect = Shader.GetShaderEffect(_rc, _color);
        }

        public void SetShader(String texturePath)
        {
            var imgData = _rc.LoadImage(texturePath);
            _iTexture1 = _rc.CreateTexture(imgData);

            _shaderEffect = Shader.GetShaderEffect(_rc, _iTexture1);

        }

        public void SetShader(float4 baseColor, String colorMapTexturePath, float4 lineColor, float2 lineWidth)
        {
            var imgData = _rc.LoadImage(colorMapTexturePath);
            _iTexture1 = _rc.CreateTexture(imgData);

            _shaderEffect = Shader.GetShaderEffect(_rc, baseColor, _iTexture1, lineColor, lineWidth);
        }
        public void SetShader(String baseTexturePath, String colorMapTexturePath, float4 lineColor, float2 lineWidth)
        {
            var imgData = _rc.LoadImage(baseTexturePath);
            _iTexture1 = _rc.CreateTexture(imgData);
            imgData = _rc.LoadImage(colorMapTexturePath);
            _iTexture2 = _rc.CreateTexture(imgData);

            _shaderEffect = Shader.GetShaderEffect(_rc, _iTexture1, _iTexture2, lineColor, lineWidth);
        }
    }
}
