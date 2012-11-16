using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Level
    {
        internal IShaderParam VColorObj { get; private set; }
        internal RenderContext RContext { get; private set; }

        private RollingCube _rCube;
        private Field[,] _levelFeld;

        internal int FieldCount { get; private set; }

        private int[] _startXy;
        private int _curLvlId;
        private LevelStates _lvlState;

        private float4x4 _camPosition;
        private float4x4 _camTranslation;
        private float4x4 _objOrientation;
        private float4x4 _mtxRot;

        internal Mesh GlobalFieldMesh { get; private set; }
        internal Mesh GlobalCubeMesh { get; private set; }

        internal float LvlDeltaTime { get; private set; }

        // array for level files
        private int[][,] _lvlTmp;

        public enum LevelStates
        {
            LsLoadFields,
            LsLoadCube,
            LsPlaying,
            LsWinning,
            LsDying
        }
        
        public enum Directions
        {
            Left,
            Right,
            Forward,
            Backward
        };

        public Level(RenderContext rc, ShaderProgram sp)
        {
            VColorObj = sp.GetShaderParam("vColor");
            RContext = rc;

            ConstructLevel(0);
        }

        public Level(RenderContext rc, ShaderProgram sp, int id)
        {
            VColorObj = sp.GetShaderParam("vColor");
            RContext = rc;

            ConstructLevel(id);
        }

        private void ConstructLevel(int id)
        {
            if (_rCube == null)
            {
                _lvlTmp = LevelTemplates.LvlTmp;

                GlobalFieldMesh = MeshReader.LoadMesh("Files/Tile.obj.model");
                GlobalCubeMesh = MeshReader.LoadMesh("Files/Cube.obj.model");

                _camPosition = float4x4.LookAt(0, 0, 3000, 0, 0, 0, 0, 1, 0);
                _objOrientation = float4x4.CreateRotationX((float)MathHelper.Pi / 2);

                _rCube = new RollingCube(this);

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
            FieldCount = 0;

            for (var y = 0; y < sizeY; y++)
                for (var x = 0; x < sizeX; x++)
                {
                    var fType = (Field.FieldTypes) _lvlTmp[id][sizeY - 1 - y, x];
                    if (fType == Field.FieldTypes.FtNull) continue;

                    _levelFeld[x, y] = new Field(this, ++FieldCount, x, y, fType);

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
            _lvlState = LevelStates.LsLoadFields;

            foreach (var feld in _levelFeld)
                if (feld != null)
                    feld.ResetField();

            if (_rCube != null)
                _rCube.ResetCube(_startXy[0], _startXy[1]);
        }

        private void WinLevel()
        {
            _lvlState = LevelStates.LsWinning;
            _rCube.WinningCube();
        }

        private void DeadLevel()
        {
            _lvlState = LevelStates.LsDying;
            _rCube.DeadCube();
        }

        internal void SetDeadField(int x, int y)
        {
            _levelFeld[x, y].DeadField();
        }

        internal void CheckField(int[] posCurXy)
        {
            var curX = posCurXy[0];
            var curY = posCurXy[1];

            if (OutOfBounds(curX, curY, _levelFeld))
                DeadLevel();
            else
            {
                if (_levelFeld[curX, curY] == null)
                {
                    DeadLevel();
                    return;
                }

                var curState = _levelFeld[curX, curY].State;
                var curType = _levelFeld[curX, curY].Type;

                if (curState == Field.FieldStates.FsDead)
                    DeadLevel();

                if (curType == Field.FieldTypes.FtEnd)
                {
                    // LINQ could be used. However, JSIL can't do that
                    var actualNumCount = 0;
                    var targetNumCount = 0;

                    foreach (var field in _levelFeld)
                    {
                        if (field == null) continue;

                        if (field.State == Field.FieldStates.FsDead)
                            actualNumCount++;

                        if (field.Type == Field.FieldTypes.FtNormal)
                            targetNumCount++;
                    }

                    if (targetNumCount == actualNumCount - 1)
                        WinLevel();
                    else
                    {
                        DeadLevel();
                        SetDeadField(curX, curY);
                    }

                }
            }
        }

        public void MoveCube(Directions dir)
        {
            if (_lvlState != LevelStates.LsPlaying)
                return;

            switch (dir)
            {
                case Directions.Left:
                    _rCube.MoveCube(-1, 0);
                    break;
                case Directions.Right:
                    _rCube.MoveCube(+1, 0);
                    break;
                case Directions.Forward:
                    _rCube.MoveCube(0, +1);
                    break;
                case Directions.Backward:
                    _rCube.MoveCube(0, -1);
                    break;
            }
        }

        private void LoadAnimation()
        {
            if (_lvlState != LevelStates.LsLoadFields)
                return;
            
            var allReady = true;

            allReady &= _levelFeld[_startXy[0], _startXy[1]].State == Field.FieldStates.FsAlive;
            allReady &= _rCube.State == RollingCube.CubeStates.CsAlive;

            if (allReady)
                _lvlState = LevelStates.LsPlaying;
        }

        private bool WonDeadAnimation()
        {
            // cube is dying, wait for it
            if (_lvlState == LevelStates.LsDying)
                if (_rCube.State == RollingCube.CubeStates.CsDead)
                {
                    ResetLevel();
                    return true;
                }

            // cube is winning, wait for it
            if (_lvlState == LevelStates.LsWinning)
                if (_rCube.State == RollingCube.CubeStates.CsWon)
                {
                    _curLvlId = ++_curLvlId % _lvlTmp.Length;
                    LoadLevel(_curLvlId);
                    return true;
                }

            return false;
        }

        public void Render(float4x4 mtxRot, double dTime)
        {
            if (WonDeadAnimation())
                return;

            LoadAnimation();

            LvlDeltaTime = (float) dTime;
            _mtxRot = mtxRot;

            foreach (var feld in _levelFeld)
                if (feld != null)
                    feld.Render(_objOrientation);

            if (_rCube != null)
                _rCube.RenderCube();
        }

        public float4x4 AddCameraTrans(float4x4 mod)
        {
            return mod * _camTranslation * _mtxRot * _camPosition;
        }

        private static bool OutOfBounds(int x, int y, Field[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }     
    }
}