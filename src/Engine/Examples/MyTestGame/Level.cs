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

        public Feld[,] LevelFeld;
        public int[] StartCxy;

        public float4x4 CamPosition = float4x4.LookAt(0, 0, 3000, 0, 0, 0, 0, 1, 0);
        public float4x4 CamTranslation;
        public float4x4 ObjectOrientation = float4x4.CreateRotationX((float)Math.PI / 2);

        internal float DeltaTime { get; private set; }


        //Dummies for level files - level files need a check on read not to be bigger than the board
        private readonly int[][,] _lvlTmp =
            {
                new[,]
                    {
                        {0, 0, 2, 2, 2, 0, 0},
                        {0, 0, 2, 0, 2, 0, 0},
                        {0, 0, 2, 0, 2, 2, 2},
                        {0, 0, 2, 0, 0, 0, 2},
                        {1, 2, 2, 0, 3, 2, 2}
                    },

                new[,]
                    {
                        {2, 2, 2, 2, 2, 2, 2},
                        {2, 0, 0, 0, 0, 0, 2},
                        {2, 0, 3, 2, 2, 0, 2},
                        {2, 0, 0, 0, 2, 0, 2},
                        {2, 2, 2, 2, 2, 0, 2},
                        {0, 0, 0, 0, 0, 0, 2},
                        {1, 2, 2, 2, 2, 2, 2}
                    }
            };

        public enum Directions
        {
            Left,
            Right,
            Forward,
            Backward
        };

        public Level(RenderContext rc, ShaderProgram sp)
        {
            _sp = sp;
            RContext = rc;

            ConstructLevel(1);
        }

        public Level(RenderContext rc, ShaderProgram sp, int id)
        {
            _sp = sp;
            RContext = rc;

            ConstructLevel(id);
        }

        private void ConstructLevel(int id)
        {
            if (RCube == null)
            {
                RCube = new RollingCube(this);
                VColorObj = _sp.GetShaderParam("vColor");

                StartCxy = new int[2];

                LoadLevel(id);
            }
        }

        private void LoadLevel(int id)
        {
            // X and Y swapped + turned 90°
            var sizeX = _lvlTmp[id].GetLength(1);
            var sizeY = _lvlTmp[id].GetLength(0);

            LevelFeld = new Feld[sizeX, sizeY];

            for (var y = 0; y < sizeY; y++)
                for (var x = 0; x < sizeX; x++)
                {
                    var fType = (Feld.FieldTypes) _lvlTmp[id][sizeY - 1 - y, x];
                    LevelFeld[x, y] = new Feld(this) { X = x, Y = y, Type = fType };

                    // set start coordinates
                    if (fType == Feld.FieldTypes.FtStart)
                    {
                        StartCxy[0] = x;
                        StartCxy[1] = y;
                    }
                }

            CamTranslation = float4x4.CreateTranslation((float) -(sizeX - 1)*100, (float) -(sizeY - 1)*100, 150);
            ResetLevel();
        }

        private void ResetLevel()
        {
            foreach (var feld in LevelFeld)
                if (feld != null)
                    feld.ResetFeld();

            if (RCube != null)
                RCube.ResetCube(StartCxy[0], StartCxy[1]);
        }

        public void CheckField(int[] posLastXy, int[] posCurXy)
        {
            if (OutOfBounds(posCurXy[0], posCurXy[1], LevelFeld))
                ResetLevel();
            else
            {
                var curState = LevelFeld[posCurXy[0], posCurXy[1]].State;
                var curType = LevelFeld[posCurXy[0], posCurXy[1]].Type;

                LevelFeld[posLastXy[0], posLastXy[1]].State = Feld.FieldStates.FsDead;

                if (curType == Feld.FieldTypes.FtVoid || curType == Feld.FieldTypes.FtEnd ||
                    curState == Feld.FieldStates.FsDead)
                    ResetLevel();
            }
        }

        public void MoveCube(Directions dir)
        {
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

        public void Render(float4x4 mtxRot, double dTime)
        {
            DeltaTime = (float) dTime;

            foreach (var feld in LevelFeld)
                if (feld != null)
                    feld.Render(CamPosition, CamTranslation, ObjectOrientation, mtxRot);

            if (RCube != null)
                RCube.RenderCube(CamPosition, CamTranslation, ObjectOrientation, mtxRot);
        }

        private static bool OutOfBounds(int x, int y, Feld[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }

/*
        private static int LinCoords(int x, int y, int[,] array)
        {
            return (array.GetLength(1)*y + x);
        }
*/
         
    }
}