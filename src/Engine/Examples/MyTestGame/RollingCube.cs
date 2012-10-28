using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class RollingCube
    {
        // Variablen
        private readonly Mesh _rCubeMesh;
        private readonly Level _curLevel;

        public int[] CubeCxy;
        public int[] CubeLastCxy;

        private static int[] _orientCyx;
        private static float[] _rotateCyx;
        private static int[] _curDirCxy;

        public bool CheckField;

        // Konstanten
        private const float PiHalf = (float) Math.PI / 2.0f;
      

        // Constructor
        public RollingCube(Level curLevel)
        {
            _curLevel = curLevel;
            _rCubeMesh = MeshReader.LoadMesh("SampleObj/Cube.obj.model");

            CubeCxy = new int[2];
            CubeLastCxy = new int[2];

            _orientCyx = new int[2];
            _rotateCyx = new float[2];
            _curDirCxy = new int[2];

            ResetCube(0, 0);
        }

        // Funktionen
        public void ResetCube(int x, int y)
        {
            CubeCxy[0] = x * 200;
            CubeCxy[1] = y * 200;

            for (int i = 0; i < 1; i++)
            {
                _orientCyx[i] = 0;
                _rotateCyx[i] = 0.0f;
                _curDirCxy[i] = 0;
            }

            CheckField = false;
        }

        public bool MoveCube(sbyte dirX, sbyte dirY)
        {
            if (_curDirCxy[0] + _curDirCxy[1] == 0)
            {
                CubeLastCxy[0] = (int)(CubeCxy[0] / 200.0);
                CubeLastCxy[1] = (int)(CubeCxy[1] / 200.0);

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
            var cubeXYtmp = new float[] {CubeCxy[0], CubeCxy[1]};
            var useRotCyx = new[] {0.0f, 0.0f};

            for (int i = 0; i <= 1; i++)
            {
                // 1st: moving in x direction? - 2nd: moving in y direction?
                if (_curDirCxy[i] != 0)
                {
                    _rotateCyx[i] += _curDirCxy[i] * 2.0f * (float)deltaTime;

                    float progr = 1 - (Math.Abs(rotateTargCyx[i] - _rotateCyx[i]) / PiHalf);
                    float curPos = progr * 200 * _curDirCxy[i];

                    cubeXYtmp[i] = CubeCxy[i] + (int)curPos;

                    // rotation target reached?
                    if ((rotateTargCyx[i] - _rotateCyx[i]) * _curDirCxy[i] < 0)
                    {
                        _rotateCyx[i] = _orientCyx[i] * PiHalf;

                        CubeCxy[i] += 200 * _curDirCxy[i];
                        cubeXYtmp[i] = CubeCxy[i];

                        _curDirCxy[i] = 0;
                        CheckField = true;
                    }

                    useRotCyx[i] = _rotateCyx[i];
                }
            }

            // Objekt ausrichten
            var mtxObjRot = float4x4.CreateRotationY(useRotCyx[0]) * float4x4.CreateRotationX(-useRotCyx[1]);
            var mtxObjPos = float4x4.CreateTranslation(cubeXYtmp[0], cubeXYtmp[1], 110);

            // Rendern
            _curLevel.RContext.ModelView = mtxObjRot * mtxObjPos * camTranslation * mtxRot * camPosition;

            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(0.5f, 0.15f, 0.17f, 1.0f));
            _curLevel.RContext.Render(_rCubeMesh);
        }
    }
}