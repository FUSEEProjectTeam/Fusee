using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Feld
    {
        private readonly Level _curLevel;

        public FieldTypes Type;
        public FieldStates State;

        public int X;
        public int Y;
        private static Mesh _feldMesh;

        public enum FieldTypes
        {
            FtVoid = 0,
            FtStart = 1,
            FtEnd = 3,
            FtNormal = 2
        }

        public enum FieldStates
        {
            FsDead,
            FsAlive
        }

        public Feld(Level curLevel)
        {
            _curLevel = curLevel;
            _feldMesh = MeshReader.LoadMesh("SampleObj/Tile.obj.model");

            ResetFeld();
        }

        public void ResetFeld()
        {
            State = FieldStates.FsAlive;
        }

        public void Render(float4x4 mtxObjRot)
        {
            if (Type == FieldTypes.FtVoid)
                return;

            var vColor = new float3(0.0f, 0.0f, 0.0f);
            var val = 1.0f;

            switch (Type)
            {
                case FieldTypes.FtStart:
                    val = 0.8f;
                    vColor = new float3(0.0f, 1.0f, 0.0f);
                    break;

                case FieldTypes.FtEnd:
                    val = 0.8f;
                    vColor = new float3(1.0f, 0.0f, 0.0f);
                    break;

                case FieldTypes.FtNormal:
                    vColor = new float3(0.8f, 0.8f, 0.8f);
                    break;
            }

            switch (State)
            {
                case FieldStates.FsDead:
                    val = 0.2f;
                    break;
            }

            var mtxObjPos = float4x4.CreateTranslation(X*200, Y*200, 0);

            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(mtxObjRot*mtxObjPos);
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(vColor, val));


            _curLevel.RContext.Render(_feldMesh);
        }
    }
}
