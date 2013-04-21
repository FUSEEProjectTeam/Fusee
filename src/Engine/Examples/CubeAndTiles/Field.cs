using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Field
    {
        private readonly Level _curLevel;
        private readonly Mesh _fieldMesh;
        private readonly int _fieldId;

        internal int[] CoordXY { get; private set; }

        private float _posZ;
        private float _veloZ;
        private float _curBright;

        private readonly float _fieldBright;
        private readonly float3 _fieldColor;
        private readonly float _randomRotZ;

        internal FieldTypes Type { get; private set; }
        internal FieldStates State { get; private set; }

        private float4x4 _modelView;
        private bool _dirtyFlag;

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
            _fieldMesh = _curLevel.GlobalFieldMesh;
            _fieldId = id;

            CoordXY = new[] {x, y};
            Type = type;

            _posZ = 0.0f;
            _veloZ = 0.0f;
            _curBright = 0.0f;

            // color and brightness
            _fieldBright = 1.0f;

            switch (Type)
            {
                case FieldTypes.FtStart:
                    _fieldBright = 0.8f;
                    _fieldColor = new float3(0.0f, 1.0f, 0.0f);
                    break;

                case FieldTypes.FtEnd:
                    _fieldBright = 1.0f;
                    _fieldColor = new float3(1.0f, 0.1f, 0.1f);
                    break;

                case FieldTypes.FtNormal:
                    _fieldColor = new float3(0.8f, 0.8f, 0.8f);
                    break;

                default:
                    _fieldColor = new float3(0.0f, 0.0f, 0.0f);
                    break;
            }

            _randomRotZ = curLevel.ObjRandom.Next(0, 4);

            State = FieldStates.FsLoading;
            _dirtyFlag = true;
        }

        // methods
        public void ResetField()
        {
            State = FieldStates.FsLoading;

            _posZ = -_fieldId/2.0f;
            _veloZ = 6f;

            // default brightness: z coord divided by maximum dist
            _curBright = 1 - (_posZ/(-_curLevel.FieldCount/2.0f));
        }

        public void DeadField()
        {
            if (State != FieldStates.FsDead)
            {
                State = FieldStates.FsDead;

                _posZ = 0;
                _veloZ = (Type == FieldTypes.FtEnd) ? -24f : -6f;
            }
        }

        private void LoadAnimation()
        {
            if (State != FieldStates.FsLoading) return;

            _veloZ = System.Math.Max(-0.6f, -_posZ/0.17f);
            _posZ += _veloZ * (float) Time.Instance.DeltaTime;

            _curBright = 1 - (_posZ)/(-_curLevel.FieldCount/2.0f);

            if (_posZ > -0.01f)
            {
                _posZ = 0;
                _veloZ = 0;
                _curBright = 1.0f;

                State = FieldStates.FsAlive;
            }

            _dirtyFlag = true;
        }

        private void DeadAnimation()
        {
            if (State != FieldStates.FsDead) return;

            if (_curBright > 0.0f)
            {
                _posZ += _veloZ*(float) Time.Instance.DeltaTime;
                _curBright -= 1.2f*(float) Time.Instance.DeltaTime;
            }
            else
                _curBright = 0;

            _dirtyFlag = true;
        }

        public void Render(float4x4 mtxObjRot, bool onlyRender = false)
        {
            // do not render dead fields with brightness <= 0
            if ((_curBright <= MathHelper.EpsilonFloat) && (State == FieldStates.FsDead))
                return;

            if (!onlyRender)
            {
                LoadAnimation();
                DeadAnimation();
            }

            if (_dirtyFlag)
            {
                // translate fields
                var mtxFieldRot = float4x4.CreateRotationZ((float) (_randomRotZ*System.Math.PI/2));

                var mtxObjPos = float4x4.CreateTranslation(CoordXY[0]*200, CoordXY[1]*200,
                                                           _posZ*100 - (RollingCube.CubeSize/2.0f + 15));

                // set translation and color, then render
                _modelView = mtxObjRot*mtxFieldRot*mtxObjPos;

                _dirtyFlag = false;
            }

            _curLevel.RContext.ModelView = _modelView*_curLevel.CamTrans;

            // TODO: SetShaderParam shouldn't set this if it's already set
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(_fieldColor, _curBright*_fieldBright));

            _curLevel.RContext.Render(_fieldMesh);
        }
    }
}
