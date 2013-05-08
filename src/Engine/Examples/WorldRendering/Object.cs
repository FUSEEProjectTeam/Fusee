using Fusee.Engine;

namespace Examples.WorldRendering
{
    internal class Object
    {
        private float _posX, _posY, _posZ;
        private float _angX, _angY, _angZ;

        private readonly Mesh _mesh;
        private readonly ShaderMaterial _material;

        public Object(Geometry geo, ShaderMaterial m, int x, int y, int z)
        {
            _mesh = geo.ToMesh();
            _material = m;
            _posX = x;
            _posY = y;
            _posZ = z;
            _angX = .0f;
            _angY = .0f;
            _angY = .0f;
        }

        public Mesh GetMesh()
        {
            return _mesh;
        }

        public ShaderProgram GetShader(RenderContext rc)
        {
            _material.UpdateMaterial(rc);
            return _material.GetShader();
        }

        public float GetPosX()
        {
            return _posX;
        }

        public float GetPosY()
        {
            return _posY;
        }

        public float GetPosZ()
        {
            return _posZ;
        }

        public float GetAngleX()
        {
            return _angX;
        }

        public float GetAngleY()
        {
            return _angY;
        }

        public float GetAngleZ()
        {
            return _angZ;
        }

        public void SetX(float x)
        {
            _posX = x;
        }

        public void SetY(float y)
        {
            _posY = y;
        }

        public void SetZ(float z)
        {
            _posZ = z;
        }

        public void SetAngleX(float angle)
        {
            _angX = angle;
        }

        public void SetAngleY(float angle)
        {
            _angY = angle;
        }

        public void SetAngleZ(float angle)
        {
            _angZ = angle;
        }
    }
}