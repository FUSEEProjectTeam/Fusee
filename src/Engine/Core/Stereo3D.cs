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
        private readonly float4 _clearColor;

        private readonly Stereo3DMode _activeMode;
        private Stereo3DEye _currentEye;

        public Stereo3DEye CurrentEye
        {
            get { return _currentEye; }
        }

        private readonly ShaderProgram _shaderProgram;
        private readonly IShaderParam _shaderTexture;

        private readonly Mesh _planeMesh;

        private readonly int _screenWidth;
        private readonly int _screenHeight;

        #region Stereo3D Shaders

        private readonly ITexture _contentLTex;
        private readonly ITexture _contentRTex;

        #region OculusRift

        // variables and shader for Oculus Rift
        private const float K0 = 1.0f;
        private const float K1 = 0.22f;
        private const float K2 = 0.24f;
        private const float K3 = 0.0f;

        private readonly IShaderParam _lensCenterParam;
        private readonly IShaderParam _screenCenterParam;
        private readonly IShaderParam _scaleParam;
        private readonly IShaderParam _scaleInParam;
        private readonly IShaderParam _hdmWarpParam;

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

        #region Anaglyph

        // shader for anagylph mode
        private const string AnaglyphVs = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
        
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                mat4 FUSEE_MVP = FUSEE_P * FUSEE_MV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        private const string AnaglyphPs = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D vTexture;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                vec4 colTex = texture2D(vTexture, vUV);
                vec4 _redBalance = vec4(0.1, 0.65, 0.25, 0);
                float _redColor = (colTex.r * _redBalance.r + colTex.g * _redBalance.g + colTex.b * _redBalance.b) * 1.5;
                gl_FragColor = vec4(_redColor, colTex.g, colTex.b, 1) * dot(vNormal, vec3(0, 0, 1)) * 1.4;
            }";

        #endregion

        #endregion

        public Stereo3D(RenderContext rc, Stereo3DMode mode, int width, int height)
        {
            _rc = rc;
            _clearColor = rc.ClearColor;

            _activeMode = mode;

            _screenWidth = width;
            _screenHeight = height;

            var imgData = _rc.CreateImage(width, height, "black");
            _contentLTex = _rc.CreateTexture(imgData);
            _contentRTex = _rc.CreateTexture(imgData);

            _planeMesh = new Cube();

            // initialize shader
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

                case Stereo3DMode.Anaglyph:
                    _shaderProgram = _rc.CreateShader(AnaglyphVs, AnaglyphPs);
                    _shaderTexture = _shaderProgram.GetShaderParam("vTexture");

                    break;
            }
        }

        public void Prepare(Stereo3DEye eye)
        {
            _currentEye = eye;

            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    _currentEye = eye;
                    const int cuttingEdge = 100;

                    switch (eye)
                    {
                        case Stereo3DEye.Left:
                            _rc.Viewport(0, cuttingEdge, _screenWidth/2, _screenHeight - cuttingEdge);
                            break;

                        case Stereo3DEye.Right:
                            _rc.Viewport(_screenWidth/2, cuttingEdge, _screenWidth/2, _screenHeight - cuttingEdge);
                            break;
                    }

                    break;
            }

            _rc.ClearColor = _clearColor;
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);
        }

        public void Save()
        {
            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    const int picTrans = 81;

                    switch (_currentEye)
                    {
                        case Stereo3DEye.Left:
                            _rc.GetBufferContent(new Rectangle(-picTrans, 0, _screenWidth - picTrans, _screenHeight),
                                _contentLTex);
                            break;
                        case Stereo3DEye.Right:
                            _rc.GetBufferContent(new Rectangle(+picTrans, 0, _screenWidth + picTrans, _screenHeight),
                                _contentRTex);
                            break;
                    }

                    break;

                case Stereo3DMode.Anaglyph:
                    _rc.GetBufferContent(new Rectangle(0, 0, _screenWidth, _screenHeight),
                        (_currentEye == Stereo3DEye.Left) ? _contentLTex : _contentRTex);

                    break;
            }
        }

        public void Display()
        {
            _rc.ClearColor = new float4(0, 0, 0, 0); // _clearColor
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

            var currShader = _rc.CurrentShader;

            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    _rc.SetShader(_shaderProgram);

                    RenderDistortedEye(Stereo3DEye.Left, 0, 0, 0.5f, 1.0f);
                    RenderDistortedEye(Stereo3DEye.Right, 0.5f, 0.0f, 0.5f, 1.0f);

                    _rc.Viewport(0, 0, _screenWidth, _screenHeight);

                    break;

                case Stereo3DMode.Anaglyph:
                    _rc.Viewport(0, 0, _screenWidth, _screenHeight);

                    float aspectRatio = (float) _screenWidth/_screenHeight;
                    _rc.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100000);

                    _rc.SetShader(_shaderProgram);

                    RenderColorMaskedEye(Stereo3DEye.Left, true, false, false, false);
                    _rc.Clear(ClearFlags.Depth);
                    RenderColorMaskedEye(Stereo3DEye.Right, false, true, true, false);

                    _rc.ColorMask(true, true, true, false);

                    break;
            }

            _rc.SetShader(currShader);
        }

        #region OculusRift

        private void RenderDistortedEye(Stereo3DEye eye, float x, float y, float w, float h)
        {
            var absX = (int) System.Math.Round(x*_screenWidth);
            var absY = (int) System.Math.Round(y*_screenHeight);
            var absW = (int) System.Math.Round(w*_screenWidth);
            var absH = (int) System.Math.Round(h*_screenHeight);

            _rc.Viewport(absX, absY, absW, absH);

            float aspectRatio = (float) absW/absH;
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

            const int scaleW = 2*495;
            const int scaleH = 2*280;
            const int transOff = 223;

            if (eye == Stereo3DEye.Left)
            {
                _rc.ModelView = float4x4.Scale(scaleW, scaleH, 1)*float4x4.CreateTranslation(transOff, 0, 0);
                _rc.View = float4x4.LookAt(0, 0, 820, 0, 0, 0, 0, 1, 0);
            }
            else
            {
                _rc.ModelView = float4x4.Scale(scaleW, scaleH, 1)*float4x4.CreateTranslation(-transOff, 0, 0);
                _rc.View = float4x4.LookAt(0, 0, 820, 0, 0, 0, 0, 1, 0);
            }

            _rc.Render(_planeMesh);
        }

        #endregion

        #region Anaglyph

        private void RenderColorMaskedEye(Stereo3DEye eye, bool red, bool green, bool blue, bool alpha)
        {          
            _rc.SetShaderParamTexture(_shaderTexture, eye == Stereo3DEye.Left ? _contentLTex : _contentRTex);
            _rc.ColorMask(red, green, blue, alpha);

            var scaleW = _screenWidth;
            var scaleH = _screenHeight;

            _rc.ModelView = float4x4.Scale(scaleW, scaleH, 1);
            _rc.View = float4x4.LookAt(0, 0, 870, 0, 0, 0, 0, 1, 0);

            _rc.Render(_planeMesh);
        }

        public float4x4 LookAt3D(Stereo3DEye eye, float3 eyeV, float3 target, float3 up)
        {
            var x = (eye == Stereo3DEye.Left)
                ? eyeV.x - Stereo3DParams.EyeDistance
                : eyeV.x + Stereo3DParams.EyeDistance;

            var newEye = new float3(x, eyeV.y, eyeV.z);
            var newTarget = new float3(target.x, target.y, target.z);

            return float4x4.LookAt(newEye, newTarget, up);
        }

        #endregion
    }
}