using System;
using System.Diagnostics;
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
        private readonly ShaderMaterial _material;

        private readonly RenderContext _rc;

        public GameEntity(String meshPath, ShaderMaterial material, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _material = material;
            _rc = rc;

            Position = float4x4.Identity;
            Position = float4x4.CreateRotationX(angX) *
                        float4x4.CreateRotationY(angY) *
                        float4x4.CreateRotationZ(angZ) *
                        float4x4.CreateTranslation(posX, posY, posZ);

            UpdateNVectors();
        }

        public GameEntity(String meshPath, ShaderMaterial material, RenderContext rc, float3 posxyz, float3 angxyz)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _material = material;
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

        public ShaderProgram GetShader()
        {
            _material.UpdateMaterial(_rc);
            return _material.GetShader();
        }

        public void Render(float4x4 camMatrix)
        {
            _rc.SetShader(this.GetShader());

            _rc.ModelView = Position * camMatrix;
            _rc.Render(this._mesh);
        }

        protected void UpdateNVectors()
        {
            NRotXV = float3.Normalize(new float3(Position.Row0));
            NRotYV = float3.Normalize(new float3(Position.Row1));
            NRotZV = float3.Normalize(new float3(Position.Row2));
        }
    }
}
