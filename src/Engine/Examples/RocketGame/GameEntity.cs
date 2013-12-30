using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameEntity
    {
        private float _posX;
        private float _posY;
        private float _posZ;
        private float _angX;
        private float _angY;
        private float _angZ;

        private readonly Mesh _mesh;
        private readonly ShaderMaterial _material;

        private readonly RenderContext _rc;

        public GameEntity(String meshPath, ShaderMaterial material, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _material = material;
            _rc = rc;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            AngX = angX;
            AngY = angY;
            AngZ = angZ;
        }

        public GameEntity(String meshPath, ShaderMaterial material, RenderContext rc, float3 posxyz, float3 angxyz)
        {
            _mesh = MeshReader.LoadMesh(meshPath);
            _material = material;
            _rc = rc;
            PosX = posxyz.x;
            PosY = posxyz.y;
            PosZ = posxyz.z;
            AngX = angxyz.x;
            AngY = angxyz.y;
            AngZ = angxyz.z;
        }

        public float PosX
        {
            get { return _posX; }
            set { _posX = value; }
        }

        public float PosY
        {
            get { return _posY; }
            set { _posY = value; }
        }

        public float PosZ
        {
            get { return _posZ; }
            set { _posZ = value; }
        }

        public float AngX
        {
            get { return _angX; }
            set { _angX = value; }
        }

        public float AngY
        {
            get { return _angY; }
            set { _angY = value; }
        }

        public float AngZ
        {
            get { return _angZ; }
            set { _angZ = value; }
        }

        public Mesh Mesh
        {
            get { return _mesh; }
        }

// ReSharper disable once InconsistentNaming
        public float3 PosXYZ
        {
            get
            {
                return new float3(PosX, PosY, PosZ);
            }
            set
            {
                _posX = value.x;
                _posY = value.y;
                _posZ = value.z;
            }
        }
// ReSharper disable once InconsistentNaming
        public float3 AngXYZ
        {
            get
            {
                return new float3(AngX, AngY, AngZ);
            }
            set
            {
                _angX = value.x;
                _angY = value.y;
                _angZ = value.z;
            }
        }

        public ShaderProgram GetShader()
        {
            _material.UpdateMaterial(_rc);
            return _material.GetShader();
        }

        public void Render(float4x4 camMatrix)
        {
            _rc.SetShader(this.GetShader());

            var mtxRot = float4x4.CreateRotationY(this.AngX) * float4x4.CreateRotationX(-this.AngY);
            var mtxTrans = float4x4.CreateTranslation(this.PosX, this.PosY, this.PosZ);

            _rc.ModelView = mtxRot * mtxTrans * camMatrix;
            _rc.Render(this._mesh);
        }
    }
}
