using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Level
    {
        private readonly ShaderProgram _sp;

        public RollingCube RCube { get; private set; }
        public IShaderParam VColorObj { get; private set; }
        public RenderContext RContext { get; private set; }

        public Feld[] LevelFeld;
        private byte[,] _levelTpl;
        public int[] StartCxy;

        public float4x4 CamPosition = float4x4.LookAt(0, -500, 3000, 0, 0, 0, 0, 1, 0);
        public float4x4 CamTranslation;
        public float4x4 ObjectOrientation = float4x4.CreateRotationX((float)Math.PI / 2);

        public enum Directions
        {
            Left,
            Right,
            Forward,
            Backward
        };

        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp)
        {
            _sp = sp;
            RContext = rc;

            ConstructLevel(sizex, sizey, 0);
        }

        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp, byte id)
        {
            _sp = sp;
            RContext = rc;

            ConstructLevel(sizex, sizey, id);
        }

        private void ConstructLevel(byte sizex, byte sizey, byte id)
        {
            if (RCube == null)
            {
                RCube = new RollingCube(this);
                VColorObj = _sp.GetShaderParam("vColor");

                StartCxy = new int[2];
                LevelFeld = new Feld[sizex*sizey];
                _levelTpl = new byte[sizex,sizey];

                int cnt = 0;

                for (byte y = 0; y < sizey; y++)
                {
                    for (byte x = 0; x < sizex; x++)
                    {
                        LevelFeld[cnt] = new Feld(this) {X = x, Y = y, Type = 0};
                        cnt++;
                    }
                }

                CamTranslation = float4x4.CreateTranslation((float) (sizex - 1)*-100, (float) (sizey - 1)*-100, 0);

                ResetLevel();
            }
        }

        private void ResetLevel()
        {
            SetLevel(1);

            foreach (var t in LevelFeld)
                t.ResetFeld();

            RCube.ResetCube(StartCxy[0], StartCxy[1]);
        }

        private void SetLevel(int id)
        {
            //Dummy for a level file - level files need a check on read not to be bigger than the board
            byte[,] tmpLvl =
                {
                    {0, 0, 2, 2, 2, 0, 0},
                    {0, 0, 2, 0, 2, 0, 0},
                    {0, 0, 2, 0, 2, 2, 2},
                    {0, 0, 2, 0, 0, 0, 2},
                    {1, 2, 2, 0, 3, 2, 2}
                };

            var ctr = 0;

            for (var y = 0; y < tmpLvl.GetLength(1); y++)
            {
                for (var x = tmpLvl.GetLength(0) - 1; x >= 0; x--)
                {
                    _levelTpl[y, x] = tmpLvl[ctr, y];
                    ctr++;
                }
                ctr = 0;
            }

            /*   _levelTpl[0, 0] = 1;
            _levelTpl[0, 1] = 2;
            _levelTpl[0, 2] = 2;
            _levelTpl[0, 3] = 2;
            _levelTpl[1, 3] = 2;
            _levelTpl[2, 3] = 2;
            _levelTpl[3, 3] = 2;
            _levelTpl[3, 2] = 2;
            _levelTpl[3, 1] = 3;*/

            StartCxy[0] = 0;
            StartCxy[1] = 0;
            //Dummy end

            var cnt = 0;

            for (byte y = 0; y < _levelTpl.GetLength(0); y++)
            {
                for (byte x = 0; x < _levelTpl.GetLength(1); x++)
                {
                    LevelFeld[cnt].Type = (Feld.FieldTypes) _levelTpl[x, y];
                    cnt++;
                }
            }
        }

        public void MoveCube(Directions dir)
        {
            if (RCube.CheckField)
                CheckField();

            switch (dir)
            {
                case Directions.Left:
                    RCube.MoveCube(-1, 0);
                    break;
                case Directions.Right:
                    RCube.MoveCube(+1, 0);
                    break;
                case Directions.Forward:
                    RCube.MoveCube(0, +1);
                    break;
                case Directions.Backward:
                    RCube.MoveCube(0, -1);
                    break;
            }
        }

        private void CheckField()
        {
            var lastX = RCube._cubeLastCxy[0];
            var lastY = RCube._cubeLastCxy[1];

            var curX = (int) (RCube._cubeCxy[0]/200.0);
            var curY = (int) (RCube._cubeCxy[1]/200.0);

            if (curX < 0 || curX > _levelTpl.GetLength(0) || curY < 0 || curY > _levelTpl.GetLength(1))
                ResetLevel();
            else
            {
                var lastField = lastY*_levelTpl.GetLength(1) + lastX;
                var curField = curY*_levelTpl.GetLength(1) + curX;

                var curState = LevelFeld[curField].State;
                var curType = LevelFeld[curField].Type;

                LevelFeld[lastField].State = Feld.FieldStates.FsDead;

                if (curType == Feld.FieldTypes.FtVoid || curType == Feld.FieldTypes.FtEnd ||
                    curState == Feld.FieldStates.FsDead)
                    ResetLevel();
            }

            RCube.CheckField = false;
        }

        public void Render(float4x4 mtxRot, double deltaTime)
        {
            if (RCube.CheckField)
                CheckField();

            foreach (var feld in LevelFeld)
                feld.Render(CamPosition, CamTranslation, ObjectOrientation, mtxRot);

            RCube.RenderCube(CamPosition, CamTranslation, ObjectOrientation, mtxRot, deltaTime);
        }
    }
}