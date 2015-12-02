using System;
using System.Diagnostics;
using System.Threading;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Level
    {
        internal IShaderParam VColorObj { get; private set; }
        internal IShaderParam VTextureObj { get; private set; }

        internal RenderContext RContext { get; private set; }

        private readonly Stereo3D _stereo3D;
        internal bool UseStereo3D;

        private RollingCube _rCube;
        private Field[,] _levelFeld;

        internal int FieldCount { get; private set; }

        private int[] _startXy;
        private int _curLvlId;
        private LevelStates _lvlState;

        private int _camPosition;
        private float4x4 _camTranslation;
        private float4x4 _objOrientation;

        internal float4x4 CamTrans { get; private set; }

        internal Mesh GlobalFieldMesh { get; private set; }
        internal ITexture TextureField { get; private set; }

        internal Mesh GlobalCubeMesh { get; private set; }
        internal ITexture TextureCube { get; private set; }

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
            Left,
            Right,
            Forward,
            Backward,
        };

        public Level(RenderContext rc, ShaderProgram sp, Stereo3D stereo3D)
            : this(rc, sp, 0, stereo3D)
        {
        }

        public Level(RenderContext rc, ShaderProgram sp, int id, Stereo3D stereo3D)
        {
            ObjRandom = new Random();

            VColorObj = sp.GetShaderParam("vColor");
            VTextureObj = sp.GetShaderParam("vTexture");

            RContext = rc;

            _stereo3D = stereo3D;
            UseStereo3D = false;

            ConstructLevel(id);
        }

        private void ConstructLevel(int id)
        {
            if (_rCube != null)
                return;

            _lvlTmp = LevelTemplates.LvlTmp;

            // load meshes
            GlobalFieldMesh = MeshReader.LoadMesh("Assets/Tile.obj.model");
            GlobalCubeMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");

            // load textures
            var imgData = RContext.LoadImage("Assets/tex_stone.jpg");
            TextureField = RContext.CreateTexture(imgData);

            imgData = RContext.LoadImage("Assets/tex_cube.jpg");
            TextureCube = RContext.CreateTexture(imgData);

            // camera
            _camPosition = -3000; // colh
            _objOrientation = float4x4.CreateRotationX(MathHelper.Pi/2);

            // create cube and set vars
            _rCube = new RollingCube(this);

            _startXy = new int[2];
            _curLvlId = id;

            // load level
            LoadLevel(id);
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
                    // colh
                    // var fType = (Field.FieldTypes) _lvlTmp[id][sizeY - 1 - y, x];
                    var fType = (Field.FieldTypes)_lvlTmp[id][y, x];
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

                if (curType == Field.FieldTypes.FtTele)
                {
                    foreach (var field in _levelFeld)
                    {
                        if (field == null) continue;

                        if (field.Type == Field.FieldTypes.FtTele &&
                            (field.CoordXY[0] != curX || field.CoordXY[1] != curY))
                            TeleportCube(field.CoordXY[0], field.CoordXY[1]);
                    }
                }
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

                        if (field.Type != Field.FieldTypes.FtStart && field.Type != Field.FieldTypes.FtEnd)
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
                    _rCube.MoveCube(0, -1);
                    break;
                case Directions.Backward:
                    _rCube.MoveCube(0, +1);
                    break;
            }
        }

        public void TeleportCube(int x, int y)
        {
            _rCube.TeleportCube(x, y);
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
                    _curLvlId = ++_curLvlId%_lvlTmp.Length;
                    LoadLevel(_curLvlId);
                    return true;
                }

            return false;
        }

        public void Render(float4x4 mtxRot)
        {
            if (WonDeadAnimation())
                return;

            LoadAnimation();

            var eyeF = new float3(0, 0, _camPosition);
            var targetF = new float3(0, 0, 0);
            var upF = new float3(0, 1, 0);

            if (!UseStereo3D)
            {
                // normal mode
                var lookAt = float4x4.LookAt(eyeF, targetF, upF);
                // colh CamTrans = _camTranslation*mtxRot*lookAt;
                CamTrans = lookAt * mtxRot * _camTranslation;

                RContext.SetShaderParamTexture(VTextureObj, TextureField);

                foreach (var feld in _levelFeld)
                    if (feld != null)
                        feld.Render(_objOrientation);

                RContext.SetShaderParamTexture(VTextureObj, TextureCube);

                if (_rCube != null)
                    _rCube.RenderCube();
            }
            else
            {
                // 3d mode
                _stereo3D.Prepare(Stereo3DEye.Left);

                for (var x = 0; x < 2; x++)
                {
                    var lookAt = _stereo3D.LookAt3D(_stereo3D.CurrentEye, eyeF, targetF, upF);
                    // colh CamTrans = _camTranslation*mtxRot*lookAt;
                    CamTrans = lookAt * mtxRot * _camTranslation;

                    var renderOnly = (_stereo3D.CurrentEye == Stereo3DEye.Left);

                    RContext.SetShaderParamTexture(VTextureObj, TextureField);

                    foreach (var feld in _levelFeld)
                        if (feld != null)
                            feld.Render(_objOrientation, renderOnly);

                    RContext.SetShaderParamTexture(VTextureObj, TextureCube);

                    if (_rCube != null)
                        _rCube.RenderCube(renderOnly);

                    _stereo3D.Save();

                    if (x == 0) _stereo3D.Prepare(Stereo3DEye.Right);
                }
            }
        }

        public void ZoomCamera(int val)
        {
            // colh _camPosition = Math.Min(5000, Math.Max(1500, _camPosition - val));
            _camPosition = Math.Max(-5000, Math.Min(-1500, _camPosition + val));
        }

        private static bool OutOfBounds(int x, int y, Field[,] array)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }
    }
}