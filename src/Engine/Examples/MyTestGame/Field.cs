using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Field
    {
        private readonly Level _curLevel;
        private readonly Mesh _feldMesh;
        private readonly int _fieldId;

        internal FieldTypes Type { get; private set; }
        internal FieldStates State { get; private set; }

        // x-y-z
        internal float[] CoordXyz { get; private set; }
        private readonly float[] _veloXyz;
        private float _curBright;

        // enums
        public enum FieldTypes
        {
            FtNull = 0,
            FtStart = 1,
            FtEnd = 3,
            FtNormal = 2
        }

        public enum FieldStates
        {
            FsLoading,
            FsAlive,
            FsDead
        }

        // constructor
        public Field(Level curLevel, int id, int x, int y, FieldTypes type)
        {
            _curLevel = curLevel;
            _feldMesh = _curLevel.GlobalFieldMesh;
            _fieldId = id;

            CoordXyz = new[] {x, y, 0.0f};
            _veloXyz = new[] {0.0f, 0.0f, 0.0f};
            _curBright = 1.0f;

            Type = type;
            State = FieldStates.FsLoading;
        }

        // methods
        public void ResetField()
        {
            State = FieldStates.FsLoading;

            CoordXyz[2] = -_fieldId/2.0f;
            _veloXyz[2] = 0.1f;

            // default brightness: z coord divided by maximum dist
            _curBright = 1 - (CoordXyz[2]/(-_curLevel.FieldCount/2.0f));
        }

        public void DeadField()
        {
            if (State != FieldStates.FsDead)
            {
                State = FieldStates.FsDead;

                CoordXyz[2] = 0;
                _veloXyz[2] = (Type == FieldTypes.FtEnd) ? -0.4f : -0.1f;
            }
        }

        private void LoadAnimation()
        {
            if (State != FieldStates.FsLoading) return;

            _veloXyz[2] = Math.Max(-0.01f, -CoordXyz[2] / 10.0f);
            CoordXyz[2] += _veloXyz[2];

            _curBright = 1 - (CoordXyz[2])/(-_curLevel.FieldCount/2.0f);

            if (CoordXyz[2] > -0.01f)
            {
                CoordXyz[2] = 0;
                _veloXyz[2] = 0;
                _curBright = 1.0f;

                State = FieldStates.FsAlive;
            }                
        }

        private void DeadAnimation()
        {
            if (State != FieldStates.FsDead) return;

            if (_curBright > 0.0f)
            {
                CoordXyz[2] += _veloXyz[2];
                _curBright -= .02f;
            }       
        }

        public void Render(float4x4 mtxObjRot)
        {
            LoadAnimation();
            DeadAnimation();

            // color fields
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

            // translate fields
            var mtxObjPos = float4x4.CreateTranslation(CoordXyz[0]*200, CoordXyz[1]*200, CoordXyz[2]*100);

            // set translation and color, then render
            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(mtxObjRot*mtxObjPos);
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(vColor, _curBright * val));

            _curLevel.RContext.Render(_feldMesh);
        }
    }
}
