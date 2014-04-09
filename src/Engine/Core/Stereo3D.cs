using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// The enum for the eye side selection.
    /// </summary>
    public enum Stereo3DEye
    {
        /// <summary>
        /// The left eye = 0.
        /// </summary>
        Left,
        /// <summary>
        /// The right eye = 1.
        /// </summary>
        Right
    }

    /// <summary>
    /// The enum for the 3D Mode selection.
    /// </summary>
    public enum Stereo3DMode
    {
        /// <summary>
        /// The anaglyph mode = 0.
        /// </summary>
        Anaglyph,
        /// <summary>
        /// The oculus rift mode = 1.
        /// </summary>
        Oculus
    }

    internal static class Stereo3DParams
    {
        internal static float EyeDistance = 30f;
        internal static float Convergence = 0f;
    }

    /// <summary>
    /// Rendering of stereo 3D graphics in anaglyph or oculus rift mode.
    /// </summary>
    public class Stereo3D
    {
        private RenderContext _rc;
        private float4 _clearColor;

        private readonly Stereo3DMode _activeMode;
        private Stereo3DEye _currentEye;

        /// <summary>
        /// Gets the current eye.
        /// </summary>
        /// <value>
        /// The current eye side enum. left=0, right=1.
        /// </value>
        public Stereo3DEye CurrentEye
        {
            get { return _currentEye; }
        }

        private GUIImage _guiLImage;
        private GUIImage _guiRImage;

        private ShaderProgram _shaderProgram;
        private IShaderParam _shaderTexture;

        private readonly int _screenWidth;
        private readonly int _screenHeight;

        #region Stereo3D Shaders

        private ITexture _contentLTex;
        private ITexture _contentRTex;

        #region OculusRift

        // variables and shader for Oculus Rift
        private const float K0 = 1.0f;
        private const float K1 = 0.22f;
        private const float K2 = 0.24f;
        private const float K3 = 0.0f;

        private IShaderParam _lensCenterParam;
        private IShaderParam _screenCenterParam;
        private IShaderParam _scaleParam;
        private IShaderParam _scaleInParam;
        private IShaderParam _hdmWarpParam;

        private const string OculusVs = @"
            attribute vec3 fuVertex;
            attribute vec2 fuUV;
            attribute vec4 fuColor;

            varying vec2 vUV;

            void main()
            {
                vUV = fuUV;
                gl_Position = vec4(fuVertex, 1);
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
		            gl_FragColor = vec4(0.2, 0.2, 0.2, 1.0);
		            return;
	            }

	            gl_FragColor = texture(vTexture, tc);
            }";

        #endregion

        #region Anaglyph

        // shader for anagylph mode
        private const string AnaglyphVs = @"
            attribute vec3 fuVertex;
            attribute vec2 fuUV;
            attribute vec4 fuColor;

            varying vec2 vUV;

            void main()
            {
                vUV = fuUV;
                gl_Position = vec4(fuVertex, 1);
            }";

        private const string AnaglyphPs = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D vTexture;
            varying vec2 vUV;

            void main()
            {
                vec4 colTex = texture2D(vTexture, vUV);
                vec4 _redBalance = vec4(0.1, 0.65, 0.25, 0);
                float _redColor = (colTex.r * _redBalance.r + colTex.g * _redBalance.g + colTex.b * _redBalance.b) * 1.5;
                gl_FragColor = vec4(_redColor, colTex.g, colTex.b, 1) * 1.4; // * dot(vNormal, vec3(0, 0, -1))  lefthanded change???
            }";

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Stereo3D"/> class.
        /// </summary>
        /// <param name="mode">The 3D rendering mode. anaglyph=0, oculus=1.</param>
        /// <param name="width">The width of the render output in pixels.</param>
        /// <param name="height">The height of the render output in pixels.</param>
        public Stereo3D(Stereo3DMode mode, int width, int height)
        {
            _activeMode = mode;

            _screenWidth = width;
            _screenHeight = height;
        }

        /// <summary>
        /// Attaches the object to a specific <see cref="RenderContext"/> object.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext"/> object to be used.</param>
        public void AttachToContext(RenderContext rc)
        {
            _rc = rc;
            _clearColor = rc.ClearColor;

            var imgData = _rc.CreateImage(_screenWidth, _screenHeight, "black");
            _contentLTex = _rc.CreateTexture(imgData);
            _contentRTex = _rc.CreateTexture(imgData);

            // initialize shader and image
            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    _guiLImage = new GUIImage(null, 0, 0, _screenWidth/2, _screenHeight);
                    _guiLImage.AttachToContext(rc);
                    _guiLImage.Refresh();

                    _guiRImage = new GUIImage(null, _screenWidth/2, 0, _screenWidth/2, _screenHeight);
                    _guiRImage.AttachToContext(rc);
                    _guiRImage.Refresh();

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

                    _guiLImage = new GUIImage(null, 0, 0, _screenWidth, _screenHeight);
                    _guiLImage.AttachToContext(rc);
                    _guiLImage.Refresh();

                    break;
            }
        }

        /// <summary>
        /// Prepares the specified eye side for 3D rendering.
        /// </summary>
        /// <param name="eye">The <see cref="Stereo3DEye"/>.</param>
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

        /// <summary>
        /// Saves this instance.
        /// </summary>
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

                    _rc.Viewport(0, 0, _screenWidth, _screenHeight);

                    break;

                case Stereo3DMode.Anaglyph:
                    _rc.GetBufferContent(new Rectangle(0, 0, _screenWidth, _screenHeight),
                        (_currentEye == Stereo3DEye.Left) ? _contentLTex : _contentRTex);

                    break;
            }
        }

        /// <summary>
        /// Displays the result as rendering output on the <see cref="RenderContext"/>.
        /// </summary>
        public void Display()
        {
            _rc.ClearColor = new float4(0, 0, 0, 0); // _clearColor
            _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

            var currShader = _rc.CurrentShader;

            switch (_activeMode)
            {
                case Stereo3DMode.Oculus:
                    _rc.SetShader(_shaderProgram);

                    RenderDistortedEye(Stereo3DEye.Left);
                    RenderDistortedEye(Stereo3DEye.Right);

                    break;

                case Stereo3DMode.Anaglyph:
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

        private void RenderDistortedEye(Stereo3DEye eye)
        {
            var scale = new float2(0.1469278f, 0.2350845f);
            var scaleIn = new float2(2, 2.5f);
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

            _rc.Render(eye == Stereo3DEye.Left ? _guiLImage.GUIMesh : _guiRImage.GUIMesh);
        }

        #endregion

        #region Anaglyph

        private void RenderColorMaskedEye(Stereo3DEye eye, bool red, bool green, bool blue, bool alpha)
        {          
            _rc.SetShaderParamTexture(_shaderTexture, eye == Stereo3DEye.Left ? _contentLTex : _contentRTex);
            _rc.ColorMask(red, green, blue, alpha);

			// change lookat ?? lefthanded change
            _rc.Render(_guiLImage.GUIMesh);
        }

        /// <summary>
        /// Aligns the <see cref="Stereo3DEye"/> to the target point.
        /// </summary>
        /// <param name="eye">The <see cref="Stereo3DEye"/>.</param>
        /// <param name="eyeV">The eye vector.</param>
        /// <param name="target">The target.</param>
        /// <param name="up">Up vector.</param>
        /// <returns>A Matrix that represents the current eye's orientation towards a target point.</returns>
        public float4x4 LookAt3D(Stereo3DEye eye, float3 eyeV, float3 target, float3 up)
        {
            var x = (eye == Stereo3DEye.Left)
                ? eyeV.x - Stereo3DParams.EyeDistance
                : eyeV.x + Stereo3DParams.EyeDistance;

            var newEye = new float3(x, eyeV.y, eyeV.z);
            var newTarget = new float3(target.x, target.y, target.z);

			// change lookat ?? lefthanded change
            return float4x4.LookAt(newEye, newTarget, up);
        }

        #endregion
    }
}