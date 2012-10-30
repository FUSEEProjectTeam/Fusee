using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Level
    {
        private readonly ShaderProgram _sp;

        internal RollingCube RCube { get; private set; }
        internal IShaderParam VColorObj { get; private set; }
        internal RenderContext RContext { get; private set; }

        private Field[,] _levelFeld;
        private int[] _startXy;
        private int _curLvlId;

        private float4x4 _camPosition;
        private float4x4 _camTranslation;
        private float4x4 _objOrientation;
        private float4x4 _mtxRot;

        internal Mesh GlobalFieldMesh { get; private set; }
        internal Mesh GlobalCubeMesh { get; private set; }

        internal float LvlDeltaTime { get; private set; }

        // dummies for level files
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
                GlobalFieldMesh = MeshReader.LoadMesh("SampleObj/Tile.obj.model");
                GlobalCubeMesh = MeshReader.LoadMesh("SampleObj/Cube.obj.model");

                _camPosition = float4x4.LookAt(0, 0, 3000, 0, 0, 0, 0, 1, 0);
                _objOrientation = float4x4.CreateRotationX((float)Math.PI / 2);

                RCube = new RollingCube(this);
                VColorObj = _sp.GetShaderParam("vColor");

                _startXy = new int[2];
                _curLvlId = id;

                LoadLevel(id);
            }
        }

        private void LoadLevel(int id)
        {
            // if id > amount of levels, go to fist lvl
           id %= _lvlTmp.Length;

            // X and Y swapped and turned 90°
            var sizeX = _lvlTmp[id].GetLength(1);
            var sizeY = _lvlTmp[id].GetLength(0);

            _levelFeld = new Field[sizeX, sizeY];

            for (var y = 0; y < sizeY; y++)
                for (var x = 0; x < sizeX; x++)
                {
                    var fType = (Field.FieldTypes) _lvlTmp[id][sizeY - 1 - y, x];
                    _levelFeld[x, y] = new Field(this, x, y, fType);

                    // set start coordinates
                    if (fType == Field.FieldTypes.FtStart)
                    {
                        _startXy[0] = x;
                        _startXy[1] = y;
                    }
                }

            _camTranslation = float4x4.CreateTranslation((float) -(sizeX - 1)*100, (float) -(sizeY - 1)*100, 150);
            ResetLevel();
        }

        private void ResetLevel()
        {
            foreach (var feld in _levelFeld)
                if (feld != null)
                    feld.ResetField();

            if (RCube != null)
                RCube.ResetCube(_startXy[0], _startXy[1]);
        }

        public void CheckField(int[] posLastXy, int[] posCurXy)
        {
            if (OutOfBounds(posCurXy[0], posCurXy[1], _levelFeld))
                ResetLevel();
            else
            {
                _levelFeld[posLastXy[0], posLastXy[1]].DeadField();

                var curState = _levelFeld[posCurXy[0], posCurXy[1]].State;
                var curType = _levelFeld[posCurXy[0], posCurXy[1]].Type;

                if (curType == Field.FieldTypes.FtVoid || curState == Field.FieldStates.FsDead)
                    ResetLevel();

                if (curType == Field.FieldTypes.FtEnd)
                {
                    // LINQ could be used. However, JSIL can't do that
                    var actualNumCount = 0;
                    var targetNumCount = 0;

                    foreach (var field in _levelFeld)
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
            return mod * _camTranslation * _mtxRot * _camPosition;
        }

        public void Render(float4x4 mtxRot, double dTime)
        {
            LvlDeltaTime = (float) dTime;
            _mtxRot = mtxRot;

            foreach (var feld in _levelFeld)
                if (feld != null)
                    feld.Render(_objOrientation);

            if (RCube != null)
                RCube.RenderCube();
        }

        private static bool OutOfBounds(int x, int y, Field[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }     
    }
}