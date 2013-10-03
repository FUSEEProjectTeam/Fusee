using System.Diagnostics;
using Fusee.Math;

namespace Fusee.Engine
{
    public enum Stereo3DEye
    {
        Left,
        Right
    }

    public enum Stereo3DMode
    {
        Anaglyph,
        Oculus
    }

    internal class Stereo3DParams
    {
        internal static float EyeDistance = 30f;
        internal static float Convergence = 0f;
    }

    public class Stereo3D
    {
        private readonly RenderContext _rc;

        private readonly Stereo3DMode _activeMode;
        private Stereo3DEye _currentEye;

        private readonly ShaderProgram _shaderProgram;
        private readonly IShaderParam _shaderTexture;

        private readonly Mesh _planeMesh;

        private float _calibrationX;
        private float _calibrationY;

        private readonly int _screenWidth;
        private readonly int _screenHeight;

        #region OculusRift

        // variables and shader for Oculus Rift
        public const float K0 = 1.0f;
        public const float K1 = 0.22f;
        public const float K2 = 0.24f;
        public const float K3 = 0.0f;

        private readonly IShaderParam _lensCenterParam;
        private readonly IShaderParam _screenCenterParam;
        private readonly IShaderParam _scaleParam;
        private readonly IShaderParam _scaleInParam;
        private readonly IShaderParam _hdmWarpParam;

        private readonly ITexture _contentLTex;
        private readonly ITexture _contentRTex;

        private const string OculusVs = @"
            attribute vec3 fuVertex;
            attribute vec2 fuUV;

            varying vec2 vUV;
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vUV = fuUV;
            }";

        private const string OculusPs = @"
            uniform sampler2D vTexture;

            uniform vec2 LensCenter;
            uniform vec2 ScreenCenter;
            uniform vec2 Scale;
            uniform vec2 ScaleIn;
            uniform vec4 HmdWarpParam;

            varying vec2 vUV;

            vec2 HmdWarp(vec2 texIn)
            {
                vec2 theta = (texIn - LensCenter) * ScaleIn;
                float rSq = theta.x * theta.x + theta.y * theta.y;
                vec2 theta1 = theta * (HmdWarpParam.x + HmdWarpParam.y * rSq + HmdWarpParam.z * rSq * rSq + HmdWarpParam.w * rSq * rSq * rSq);
                return LensCenter + Scale * theta1;
            }

            void main()
            {
                vec2 tc = HmdWarp(vUV.xy);
	            if (any(bvec2(clamp(tc,ScreenCenter-vec2(0.25,0.5), ScreenCenter+vec2(0.25,0.5)) - tc)))
	            {
		            gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
		            return;
	            }

	            gl_FragColor = texture(vTexture, tc);
            }";

        #endregion

        public Stereo3D(RenderContext rc, Stereo3DMode mode, int width, int height)
        {
            _rc = rc;
            _activeMode = mode;

            _screenWidth = width;
            _screenHeight = height;

            var imgData = _rc.CreateImage(width, height, "black");
            _contentLTex = _rc.CreateTexture(imgData);
            _contentRTex = _rc.CreateTexture(imgData);

            _planeMesh = MeshReader.LoadMesh("Assets/guiPlane.obj.model");

            switch (mode)
            {
                case Stereo3DMode.Oculus:
                    _shaderProgram = _rc.CreateShader(OculusVs, OculusPs);
                    _shaderTexture = _shaderProgram.GetShaderParam("vTexture");

                    _lensCenterParam = _shaderProgram.GetShaderParam("LensCenter");
                    _screenCenterParam = _shaderProgram.GetShaderParam("ScreenCenter");
                    _scaleParam = _shaderProgram.GetShaderParam("Scale");
                    _scaleInParam = _shaderProgram.GetShaderParam("ScaleIn");
                    _hdmWarpParam = _shaderProgram.GetShaderParam("HmdWarpParam");

                    break;
            }

            _calibrationX = 0;
            _calibrationY = 0;
        }

        public void Prepare(Stereo3DEye eye)
        {
            _currentEye = eye;
            const int cuttingEdge = 100;

            switch (eye)
            {
                case Stereo3DEye.Left:
                    _rc.Viewport(0, cuttingEdge, _screenWidth / 2, _screenHeight - cuttingEdge);
                    break;

                case Stereo3DEye.Right:
                    _rc.Viewport(_screenWidth / 2, cuttingEdge, _screenWidth / 2, _screenHeight - cuttingEdge);
                    break;
            }

            _rc.ClearColor = new float4(0.1f, 0.1f, 0.1f, 0);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
        }

        public void Save()
        {
            //var picTrans = (int) System.Math.Round(_calibrationX);
            const int picTrans = 81;

            switch (_currentEye)
            {
                case Stereo3DEye.Left:
                    _rc.GetBufferContent(new Rectangle(-picTrans, 0, _screenWidth - picTrans, _screenHeight), _contentLTex);
                    break;
                case Stereo3DEye.Right:
                    _rc.GetBufferContent(new Rectangle(+picTrans, 0, _screenWidth + picTrans, _screenHeight), _contentRTex);
                    break;
            }
        }

        public void Display()
        {
            _rc.ClearColor = new float4(0.0f, 0.0f, 0.0f, 1);
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    _rc.SetShader(_shaderProgram);

                    RenderDistortedEye(Stereo3DEye.Left, 0, 0, 0.5f, 1.0f);
                    RenderDistortedEye(Stereo3DEye.Right, 0.5f, 0.0f, 0.5f, 1.0f);

                    break;
            }

            _rc.Viewport(0, 0, _screenWidth, _screenHeight);

            #region Calibration
            /*
                if (Input.Instance.IsKeyPressed(KeyCodes.Left))
                    _calibrationX -= 1f;

                if (Input.Instance.IsKeyPressed(KeyCodes.Right))
                    _calibrationX += 1f;

                if (Input.Instance.IsKeyPressed(KeyCodes.Up))
                    _calibrationY += 1f;

                if (Input.Instance.IsKeyPressed(KeyCodes.Down))
                    _calibrationY -= 1f;

                if (Input.Instance.IsKeyDown(KeyCodes.Space))
                    Debug.WriteLine(_calibrationX + "/" + _calibrationY);

                _rc.Clear(ClearFlags.Depth);

                _rc.ModelView = float4x4.Scale(1, 1, 1);
                _rc.Projection = float4x4.CreateOrthographic(_screenWidth, _screenHeight, 0, 100000);

                _rc.DebugLine(new float3(_calibrationX, -1000, 0), new float3(_calibrationX, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(_calibrationX - 2, -1000, 0), new float3(_calibrationX - 2, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(_calibrationX + 2, -1000, 0), new float3(_calibrationX + 2, 1000, 0), new float4(0, 1, 0, 1));

                _rc.DebugLine(new float3(-10000, _calibrationY, 0), new float3(10000, _calibrationY, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, _calibrationY - 2, 0), new float3(10000, _calibrationY - 2, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, _calibrationY + 2, 0), new float3(10000, _calibrationY + 2, 0), new float4(0, 1, 0, 1));
            
                // Right
                _rc.DebugLine(new float3(-150, -1000, 0), new float3(-150, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-150 - 2, -1000, 0), new float3(-150 - 2, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-150 + 2, -1000, 0), new float3(-150 + 2, 1000, 0), new float4(0, 1, 0, 1));
            
                // Left
                _rc.DebugLine(new float3(-1550, -1000, 0), new float3(-1550, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-1550 - 2, -1000, 0), new float3(-1550 - 2, 1000, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-1550 + 2, -1000, 0), new float3(-1550 + 2, 1000, 0), new float4(0, 1, 0, 1));
            
                _rc.DebugLine(new float3(-10000, 684, 0), new float3(10000, 684, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, 684 - 2, 0), new float3(10000, 684 - 2, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, 684 + 2, 0), new float3(10000, 684 + 2, 0), new float4(0, 1, 0, 1));

                _rc.DebugLine(new float3(-10000, -701, 0), new float3(10000, -701, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, -701 - 2, 0), new float3(10000, -701 - 2, 0), new float4(0, 1, 0, 1));
                _rc.DebugLine(new float3(-10000, -701 + 2, 0), new float3(10000, -701 + 2, 0), new float4(0, 1, 0, 1));
            */
            #endregion
        }

        private void RenderDistortedEye(Stereo3DEye eye, float x, float y, float w, float h)
        {
            var absX = (int)System.Math.Round(x * _screenWidth);
            var absY = (int)System.Math.Round(y * _screenHeight);
            var absW = (int)System.Math.Round(w * _screenWidth);
            var absH = (int)System.Math.Round(h * _screenHeight);

            _rc.Viewport(absX, absY, absW, absH);

            float aspectRatio = (float)absW / absH;
            _rc.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100000);

            var scale = new float2(0.1469278f, 0.2350845f);
            var scaleIn = new float2(4, 2.5f);
            var hdmWarp = new float4(K0, K1, K2, K3);

            float2 lensCenter;
            float2 screenCenter;
            
            if (eye == Stereo3DEye.Left)
            {
                _rc.SetShaderParamTexture(_shaderTexture, _contentLTex);

                lensCenter = new float2(0.3125f, 0.5f);
                screenCenter = new float2(0.25f, 0.5f);
            }
            else
            {
                _rc.SetShaderParamTexture(_shaderTexture, _contentRTex);

                lensCenter = new float2(0.6875f, 0.5f);
                screenCenter = new float2(0.75f, 0.5f);
            }

            _rc.SetShaderParam(_lensCenterParam, lensCenter);
            _rc.SetShaderParam(_screenCenterParam, screenCenter);
            _rc.SetShaderParam(_scaleParam, scale);
            _rc.SetShaderParam(_scaleInParam, scaleIn);
            _rc.SetShaderParam(_hdmWarpParam, hdmWarp);

            const int scaleW = 495;
            const int scaleH = 280;
            const int transOff = 223;

            if (eye == Stereo3DEye.Left)
            {
                _rc.ModelView = float4x4.Scale(scaleW, scaleH, 1) * float4x4.CreateTranslation(transOff, 0, 0);
                _rc.View = float4x4.LookAt(0, 0, 820, 0, 0, 0, 0, 1, 0);
            }
            else
            {
                _rc.ModelView = float4x4.Scale(scaleW, scaleH, 1) * float4x4.CreateTranslation(-transOff, 0, 0);
                _rc.View = float4x4.LookAt(0, 0, 820, 0, 0, 0, 0, 1, 0);
            }

            _rc.Render(_planeMesh);
        }
    }
}
