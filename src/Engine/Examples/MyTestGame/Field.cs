using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Field
    {
        private readonly Level _curLevel;

        internal Point Coord { get; private set; }
        internal FieldTypes Type { get; private set; }
        internal FieldStates State { get; private set; }

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

        public Field(Level curLevel, int x, int y, FieldTypes type)
        {
            _curLevel = curLevel;
            _feldMesh = MeshReader.LoadMesh("SampleObj/Tile.obj.model");

            Coord = new Point {x = x, y = y, z = 0};
            Type = type;

            ResetField();
        }

        public void ResetField()
        {
            State = FieldStates.FsAlive;
            Coord = new Point { x = Coord.x, y = Coord.y, z = 0 };
        }

        public void DeadField()
        {
            State = FieldStates.FsDead;
            Coord = new Point {x = Coord.x, y = Coord.y, z = -100};
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

            var mtxObjPos = float4x4.CreateTranslation(Coord.x*200, Coord.y*200, Coord.z);

            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(mtxObjRot*mtxObjPos);
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(vColor, val));


            _curLevel.RContext.Render(_feldMesh);
        }
    }
}
