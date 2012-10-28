using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Feld
    {
        private readonly RenderContext _rc;
        public byte Type;
        public byte X;
        public byte Y;
        private static Mesh _feldMesh;
        private static IShaderParam _vColorObj;
        private static float4 _vColor;


        public Feld(RenderContext rc, ShaderProgram sp)
        {
            _rc = rc;
            _vColorObj = sp.GetShaderParam("vColor");

            _feldMesh = MeshReader.LoadMesh("SampleObj/Tile.obj.model");
        }

        public void Render(float4x4 camPosition, float4x4 camTranslation, float4x4 objectOrientation, float4x4 mtxRot)
        {
            if (Type == 0)
                return;

            switch (Type)
            {
                case 1:
                    _vColor = new float4(0, 1, 0, 0.5f);
                    break;
                case 2:
                    _vColor = new float4(0.8f, 0.8f, 0.8f, 1);
                    break;
                case 3:
                    _vColor = new float4(1, 0, 0, 0.5f);
                    break;

                default:
                    _vColor = new float4(0, 0, 0, 1);
                    break;
            }
            _rc.SetShaderParam(_vColorObj, _vColor);

            _rc.ModelView = objectOrientation * float4x4.CreateTranslation(X * 200, Y * 200, 0) * camTranslation * mtxRot * camPosition;

            _rc.Render(_feldMesh);
        }
    }
}
