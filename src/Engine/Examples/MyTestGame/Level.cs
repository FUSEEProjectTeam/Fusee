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

        public Field[,] LevelFeld;
        public int[] StartXy;
        private static int _curLvlId;

        public float4x4 CamPosition = float4x4.LookAt(0, 0, 3000, 0, 0, 0, 0, 1, 0);
        public float4x4 CamTranslation;
        public float4x4 ObjectOrientation = float4x4.CreateRotationX((float)Math.PI / 2);

        private float4x4 _mtxRot;
        internal float LvlDeltaTime { get; private set; }

        public readonly Mesh GlobalFieldMesh = MeshReader.LoadMesh("SampleObj/Tile.obj.model");
        public readonly Mesh GlobalCubeMesh = MeshReader.LoadMesh("SampleObj/Cube.obj.model");

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
                        {2, 2, 0, 0, 0, 0, 2},
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

            ConstructLevel(0);
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

                StartXy = new int[2];
                _curLvlId = id;

             //   for (int i = 0; i < 500; i++)
             //   {
                    LoadLevel(id);                  
              //  }

            }
        }

        private void LoadLevel(int id)
        {
            // if id > amount of levels, go to fist lvl
           id %= _lvlTmp.Length;

            // X and Y swapped and turned 90°
            var sizeX = _lvlTmp[id].GetLength(1);
            var sizeY = _lvlTmp[id].GetLength(0);

            LevelFeld = new Field[sizeX, sizeY];

            for (var y = 0; y < sizeY; y++)
                for (var x = 0; x < sizeX; x++)
                {
                    var fType = (Field.FieldTypes) _lvlTmp[id][sizeY - 1 - y, x];
                    LevelFeld[x, y] = new Field(this, x, y, fType);

                    // set start coordinates
                    if (fType == Field.FieldTypes.FtStart)
                    {
                        StartXy[0] = x;
                        StartXy[1] = y;
                    }
                }

            CamTranslation = float4x4.CreateTranslation((float) -(sizeX - 1)*100, (float) -(sizeY - 1)*100, 150);
            ResetLevel();
        }

        private void ResetLevel()
        {
            foreach (var feld in LevelFeld)
                if (feld != null)
                    feld.ResetField();

            if (RCube != null)
                RCube.ResetCube(StartXy[0], StartXy[1]);
        }

        public void CheckField(int[] posLastXy, int[] posCurXy)
        {
            if (OutOfBounds(posCurXy[0], posCurXy[1], LevelFeld))
                ResetLevel();
            else
            {
                LevelFeld[posLastXy[0], posLastXy[1]].DeadField();

                var curState = LevelFeld[posCurXy[0], posCurXy[1]].State;
                var curType = LevelFeld[posCurXy[0], posCurXy[1]].Type;

                if (curType == Field.FieldTypes.FtVoid || curState == Field.FieldStates.FsDead)
                    ResetLevel();

                if (curType == Field.FieldTypes.FtEnd)
                {
                    // LINQ could be used. However, JSIL can't do that
                    var actualNumCount = 0;
                    var targetNumCount = 0;

                    foreach (var field in LevelFeld)
                    {
                        if (field.State == Field.FieldStates.FsDead)
                            actualNumCount++;

                        if (field.Type == Field.FieldTypes.FtNormal)
                            targetNumCount++;
                    }

                    if (targetNumCount == actualNumCount - 1)
                    {
                        _curLvlId = ++_curLvlId % _lvlTmp.Length;
                        LoadLevel(_curLvlId);                       
                    } else
                    {
                        ResetLevel();
                    }
                }
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

        public float4x4 AddCameraTrans(float4x4 mod)
        {
            return mod * CamTranslation * _mtxRot * CamPosition;
        }

        public void Render(float4x4 mtxRot, double dTime)
        {
            LvlDeltaTime = (float) dTime;
            _mtxRot = mtxRot;

            foreach (var feld in LevelFeld)
                if (feld != null)
                    feld.Render(ObjectOrientation);

            if (RCube != null)
                RCube.RenderCube();
        }

        private static bool OutOfBounds(int x, int y, Field[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }     
    }
}