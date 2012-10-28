using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class RollingCube
    {
        // Variablen
        private readonly Mesh _rCubeMesh;
        private readonly RenderContext _rc;

        public int[] _cubeCxy;
        public int[] _cubeLastCxy;

        private static int[] _orientCyx;
        private static float[] _rotateCyx;
        private static int[] _curDirCxy;

        private static IShaderParam _vColorObj;

        public bool CheckColl;

        // Konstanten
        private const float PiHalf = (float) Math.PI / 2.0f;
      

        // Constructor
        public RollingCube(RenderContext rContext, ShaderProgram sProg)
        {
            _rc = rContext;
            _rCubeMesh = MeshReader.LoadMesh("SampleObj/Cube.obj.model");
            _vColorObj = sProg.GetShaderParam("vColor");

            // Warum geht das nicht??
            // rContext.GetShaderParam();

            _cubeCxy = new int[2];
            _cubeLastCxy = new int[2];

            _orientCyx = new int[2];
            _rotateCyx = new float[2];
            _curDirCxy = new int[2];

            ResetCube(0, 0);
        }

        // Funktionen
        public void ResetCube(int x, int y)
        {
            _cubeCxy[0] = x * 200;
            _cubeCxy[1] = y * 200;

            for (int i = 0; i < 1; i++)
            {
                _orientCyx[i] = 0;
                _rotateCyx[i] = 0.0f;
                _curDirCxy[i] = 0;
            }

            CheckColl = false;
        }

        public bool MoveCube(sbyte dirX, sbyte dirY)
        {
            if (_curDirCxy[0] + _curDirCxy[1] == 0)
            {
                _cubeLastCxy[0] = (int)(_cubeCxy[0] / 200.0);
                _cubeLastCxy[1] = (int)(_cubeCxy[1] / 200.0);

                _curDirCxy[0] = dirX;
                _curDirCxy[1] = dirY;

                for (int i = 0; i <= 1; i++)
                {
                    _orientCyx[i] += _curDirCxy[i];
                    _orientCyx[i] = (int)(_orientCyx[i] - Math.Floor(_orientCyx[i] / 4.0) * 4);
                    _rotateCyx[i] = (_orientCyx[i] - _curDirCxy[i])*PiHalf;
                }

                return true;
            }

            return false;   
        }

        public void RenderCube(float4x4 camPosition, float4x4 camTranslation, float4x4 objectOrientation, float4x4 mtxRot, double deltaTime)
        {
            var rotateTargCyx = new[] {_orientCyx[0]*PiHalf, _orientCyx[1]*PiHalf};
            var cubeXYtmp = new float[] {_cubeCxy[0], _cubeCxy[1]};
            var useRotCyx = new[] {0.0f, 0.0f};

            for (int i = 0; i <= 1; i++)
            {
                // 1st: moving in x direction? - 2nd: moving in y direction?
                if (_curDirCxy[i] != 0)
                {
                    _rotateCyx[i] += _curDirCxy[i] * 2.0f * (float)deltaTime;

                    float progr = 1 - (Math.Abs(rotateTargCyx[i] - _rotateCyx[i]) / PiHalf);
                    float curPos = progr * 200 * _curDirCxy[i];

                    cubeXYtmp[i] = _cubeCxy[i] + (int)curPos;

                    // rotation target reached?
                    if ((rotateTargCyx[i] - _rotateCyx[i]) * _curDirCxy[i] < 0)
                    {
                        _rotateCyx[i] = _orientCyx[i] * PiHalf;

                        _cubeCxy[i] += 200 * _curDirCxy[i];
                        cubeXYtmp[i] = _cubeCxy[i];

                        _curDirCxy[i] = 0;
                        CheckColl = true;
                    }

                    useRotCyx[i] = _rotateCyx[i];
                }
            }

            // Objekt ausrichten
            var mtxObjRot = float4x4.CreateRotationY(useRotCyx[0]) * float4x4.CreateRotationX(-useRotCyx[1]);
            var mtxObjPos = float4x4.CreateTranslation(cubeXYtmp[0], cubeXYtmp[1], 110);

            // Rendern
            _rc.ModelView = mtxObjRot * mtxObjPos * camTranslation * mtxRot * camPosition;

            _rc.SetShaderParam(_vColorObj, new float4(0.5f, 0.15f, 0.17f, 1.0f));
            _rc.Render(_rCubeMesh);
        }
    }
}