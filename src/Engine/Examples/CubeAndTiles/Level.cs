using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Level
    {
        internal IShaderParam VColorObj { get; private set; }
        internal IShaderParam VTextureObj { get; private set; }
        private readonly IShaderParam _vUseAnaglyph;

        internal RenderContext RContext { get; private set; }

        private readonly Anaglyph3D _anaglyph3D;
        internal bool UseAnaglyph3D;

        internal RollingCube RCube { get; set; }
        private Field[,] _levelFeld;

        internal int FieldCount { get; private set; }

        private int[] _startXy;
        private int _curLvlId;
        private LevelStates _lvlState;

        private int _camPosition;
        private float4x4 _camTranslation;
        private float4x4 _objOrientation;
        private float4x4 _mtxRot;
        
        internal Mesh GlobalFieldMesh { get; private set; }
        internal ITexture TextureField { get; private set; }

        internal Mesh GlobalCubeMesh { get; private set; }
        internal ITexture TextureCube { get; private set; }
        internal float3 CubeColor { get; set; }

        internal float LvlDeltaTime { get; private set; }
        internal Random ObjRandom { get; private set; }

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
            None,
            Left,
            Right,
            Forward,
            Backward
        };

        public Level(RenderContext rc, ShaderProgram sp, Anaglyph3D anaglyph3D)
            : this(rc, sp, 0, anaglyph3D)
        {
        }

        public Level(RenderContext rc, ShaderProgram sp, int id, Anaglyph3D anaglyph3D)
        {
            ObjRandom = new Random();

            VColorObj = sp.GetShaderParam("vColor");
            VTextureObj = sp.GetShaderParam("vTexture");

            RContext = rc;

            _anaglyph3D = anaglyph3D;
            UseAnaglyph3D = false;

            _vUseAnaglyph = sp.GetShaderParam("vUseAnaglyph");
            RContext.SetShaderParam(_vUseAnaglyph, UseAnaglyph3D ? 1 : 0);
            
            ConstructLevel(id);
        }

        private void ConstructLevel(int id)
        {
            if (RCube != null)
                return;

            _lvlTmp = LevelTemplates.LvlTmp;

            // load meshes
            GlobalFieldMesh = MeshReader.LoadMesh("Assets/Tile.obj.model");
            GlobalCubeMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");

            // load textures
            ImageData imgData = RContext.LoadImage("Assets/tex_stone.jpg");
            TextureField = RContext.CreateTexture(imgData);

            imgData = RContext.LoadImage("Assets/tex_cube.jpg");
            TextureCube = RContext.CreateTexture(imgData);

            // camera
            _camPosition = 3000;
            _objOrientation = float4x4.CreateRotationX(MathHelper.Pi/2);

            // create cube and set vars
            RCube = new RollingCube(this);

            _startXy = new int[2];
            _curLvlId = id;

            // load level
            LoadLevel(id);
        }

        public void LoadLevel(int id)
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

            if (RCube != null)
                RCube.ResetCube(_startXy[0], _startXy[1]);
        }

        private void WinLevel()
        {
            _lvlState = LevelStates.LsWinning;
            RCube.WinningCube();
        }

        private void DeadLevel()
        {
            _lvlState = LevelStates.LsDying;
            RCube.DeadCube();
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

        private void LoadAnimation()
        {
            if (_lvlState != LevelStates.LsLoadFields)
                return;
            
            var allReady = true;

            allReady &= _levelFeld[_startXy[0], _startXy[1]].State == Field.FieldStates.FsAlive;
            allReady &= RCube.State == RollingCube.CubeStates.CsAlive;

            if (allReady)
                _lvlState = LevelStates.LsPlaying;
        }

        private bool WonDeadAnimation()
        {
            // cube is dying, wait for it
            if (_lvlState == LevelStates.LsDying)
                if (RCube.State == RollingCube.CubeStates.CsDead)
                {
                    ResetLevel();
                    return true;
                }

            // cube is winning, wait for it
            if (_lvlState == LevelStates.LsWinning)
                if (RCube.State == RollingCube.CubeStates.CsWon)
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

            RContext.SetShaderParam(_vUseAnaglyph, UseAnaglyph3D ? 1 : 0);

            for (int x = 0; x < 2; x++)
            {
                if (UseAnaglyph3D)
                    _anaglyph3D.SwitchEye();

                var renderOnly = UseAnaglyph3D && _anaglyph3D.IsLeftEye;

                foreach (var feld in _levelFeld)
                    if (feld != null)
                        feld.Render(_objOrientation, renderOnly);

                if (RCube != null)
                    RCube.RenderCube(renderOnly);

                if (!UseAnaglyph3D)
                    break;
            }

            _anaglyph3D.NormalMode();
        }

        public void ZoomCamera(int val)
        {
            _camPosition = Math.Min(3000, Math.Max(1500, _camPosition - val));
        }

        public float4x4 AddCameraTrans(float4x4 mod)
        {
            var lookAt = UseAnaglyph3D
                         ? _anaglyph3D.LookAt3D(0, 0, _camPosition, 0, 0, 0, 0, 1, 0)
                         : float4x4.LookAt(0, 0, _camPosition, 0, 0, 0, 0, 1, 0);

            return mod*_camTranslation*_mtxRot*lookAt;
        }

        private static bool OutOfBounds(int x, int y, Field[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }     
    }
}