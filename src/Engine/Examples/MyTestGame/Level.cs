using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Level
    {
        private readonly RenderContext _rc;
        private readonly ShaderProgram _sp;

        public Feld[] LevelFeld;
        private byte[,] _levelTpl;
        public int[] StartCxy;
        private RollingCube _rollingCube;


        public float4x4 CamPosition = float4x4.LookAt(0, 0, 4000, 0, 0, 0, 0, 1, 0);
        public float4x4 CamTranslation;
        public float4x4 ObjectOrientation = float4x4.CreateRotationX((float)Math.PI / 2);

        public enum Directions
        {
            Left,
            Right,
            Forward,
            Backward
        };

        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp, RollingCube rCube)
        {
            _sp = sp;
            _rc = rc;

            _rollingCube = rCube;
            StartCxy = new int[2];
            LevelFeld = new Feld[sizex * sizey];
            _levelTpl = new byte[sizex, sizey];

            int cnt = 0;

            for (byte y = 0; y < sizey; y++)
            {
                for (byte x = 0; x < sizex; x++)
                {
                    LevelFeld[cnt] = new Feld(rc, sp) { X = x, Y = y, Type = 0 };
                    cnt++;
                }
            }

            CamTranslation = float4x4.CreateTranslation((float)(sizex - 1) * -100, (float)(sizey - 1) * -100, 0);

            ResetLevel();
        }
        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp, byte id)
        {
            LevelFeld = new Feld[sizex * sizey];
            _levelTpl = new byte[sizex, sizey];
            SetLevel(id, rc, sp);
        }

        private void ResetLevel()
        {
            SetLevel(1, _rc, _sp);

            _rollingCube.ResetCube(StartCxy[0], StartCxy[1]);
        }

        private void SetLevel(int id, RenderContext rc, ShaderProgram sp)
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

            int ctr = 0;

            for (int y = 0; y < tmpLvl.GetLength(1); y++)
            {
                for (int x = tmpLvl.GetLength(0) - 1; x >= 0; x--)
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

            int cnt = 0;

            for (byte y = 0; y < _levelTpl.GetLength(0); y++)
            {
                for (byte x = 0; x < _levelTpl.GetLength(1); x++)
                {
                    LevelFeld[cnt].Type = _levelTpl[x, y];
                    cnt++;
                }
            }
        }

        public void MoveCube(Directions dir)
        {
            bool ret = false;

            switch (dir)
            {
                case Directions.Left:
                    ret = _rollingCube.MoveCube(-1, 0);
                    break;
                case Directions.Right:
                    ret = _rollingCube.MoveCube(+1, 0);
                    break;
                case Directions.Forward:
                    ret = _rollingCube.MoveCube(0, +1);
                    break;
                case Directions.Backward:
                    ret = _rollingCube.MoveCube(0, -1);
                    break;
            }
        }

        public void Render(float4x4 mtxRot, double deltaTime)
        {
            if (_rollingCube.CheckColl)
            {
                int lastX = _rollingCube._cubeLastCxy[0];
                int lastY = _rollingCube._cubeLastCxy[1];

                int curX = (int)(_rollingCube._cubeCxy[0] / 200.0);
                int curY = (int)(_rollingCube._cubeCxy[1] / 200.0);

                LevelFeld[lastY * _levelTpl.GetLength(1) + lastX].Type = 0;
                byte curType = LevelFeld[curY * _levelTpl.GetLength(1) + curX].Type;

                if (curX < 0 || curX > _levelTpl.GetLength(0) || curY < 0 || curY > _levelTpl.GetLength(1))
                    ResetLevel();
                else if (curType == 0 || curType == 3)
                    ResetLevel();

                _rollingCube.CheckColl = false;
            }

            foreach (Feld feld in LevelFeld)
            {
                feld.Render(CamPosition, CamTranslation, ObjectOrientation, mtxRot);
            }
            _rollingCube.RenderCube(CamPosition, CamTranslation, ObjectOrientation, mtxRot, deltaTime);
        }
    }
}