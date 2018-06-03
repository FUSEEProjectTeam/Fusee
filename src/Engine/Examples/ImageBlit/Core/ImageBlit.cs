#define GUI_SIMPLE

using System;
using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
#if GUI_SIMPLE
using Fusee.Engine.Core.GUI;
#endif

namespace Fusee.Engine.Examples.ImageBlit.Core
{

    [FuseeApplication(Name = "ImageBlit Example", Description = "ImageBlit example.")]
    public class ImageBlit : RenderCanvas
    {

        #region Billboard Shader and Mesh

        private ShaderEffect billBoardShaderEffect;

        private readonly string BillboardVs = @"
            uniform mat4 guiXForm;
            attribute vec3 fuVertex;
            attribute vec2 fuUV;
            attribute vec4 fuColor;

            varying vec2 vUV;
            varying vec4 vColor;

            void main()
            {
                vUV = fuUV;
                vColor = fuColor;

                gl_Position = guiXForm * vec4(fuVertex, 1);
            }";

        private readonly string BillboardPs = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            varying vec2 vUV;
            varying vec4 vColor;

            uniform sampler2D tex;
            //uniform vec4 uColor;

            void main(void) {
                //vec2 uv = vUV *vec2(-1,1);
            
                gl_FragColor = texture2D(tex, vUV);// + uColor; // just a texture mixed with color
            }";


        private Mesh _billboardMesh;
        private readonly float DesiredWidth = 270f;
        private readonly float DesiredHeight = 500f;

        private readonly float4 BillBoardRedColor = new float4(1, 0, 0, 0.3f);
        private readonly float4 BillBoardGreenColor = new float4(0, 1, 0, 0.3f);
        private readonly float4 BillBoardBlueColor = new float4(0, 0, 1, 0.3f);

        private float4x4 _leftImageFloat4X4 = float4x4.CreateTranslation(-0.5f, 0, 0);
        private float4x4 _centerImageFloat4X4 = float4x4.Identity;
        private float4x4 _rightImageFloat4X4 = float4x4.CreateTranslation(0.5f, 0, 0);

        private Texture blitCPUtoCPUDestinationTexture, blitCPUtoGPUDestinationTexture, blitGPUtoGPUDestinationTexture;
        private ImageData blitSource;
        /// <summary>
        /// Creates a mesh used for the screen
        /// </summary>
        private Mesh CreatePlaneMesh(float desiredWidth, float desiredHeight, float screenWidth, float screenHeight)
        {
            if (desiredWidth > screenWidth) desiredWidth = screenWidth; // crop width when it exceeds screen width
            if (desiredHeight > screenHeight) desiredHeight = screenHeight; // crop height when it exceeds screen height
            float relativeWidth = desiredWidth / screenWidth; // range 0f...1f
            float relativeHeight = desiredHeight / screenHeight; // range 0f...1f

            var mesh = new Mesh();
            var vertices = new[]
            {
                new float3 {x = -relativeWidth, y = -relativeHeight, z = +0.5f},
                new float3 {x = +relativeWidth, y = -relativeHeight, z = +0.5f},
                new float3 {x = -relativeWidth, y = +relativeHeight, z = +0.5f},
                new float3 {x = +relativeWidth, y = +relativeHeight, z = +0.5f}
            };

            var triangles = new ushort[]
            {
                // front face
                0,1,2,1,3,2
            };

            var normals = new[]
            {
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1)
            };
            var uVs = new[]
            {
                new float2(0, 0),
                new float2(1, 0),
                new float2(0, 1),
                new float2(1, 1)
            };

            mesh.Vertices = vertices;
            mesh.Triangles = triangles;
            mesh.Normals = normals;
            mesh.UVs = uVs;
            return mesh;
        }

        private float3[] OnResizeCalculateMeshVertices(float desiredWidth, float desiredHeight, float screenWidth, float screenHeight)
        {
            if (desiredWidth > screenWidth) desiredWidth = screenWidth; // crop width when it exceeds screen width
            if (desiredHeight > screenHeight) desiredHeight = screenHeight; // crop height when it exceeds screen height
            float relativeWidth = desiredWidth / screenWidth; // range 0f...1f
            float relativeHeight = desiredHeight / screenHeight; // range 0f...1f

            var vertices = new[]
            {
                new float3 {x = -relativeWidth, y = -relativeHeight, z = +0.5f},
                new float3 {x = +relativeWidth, y = -relativeHeight, z = +0.5f},
                new float3 {x = -relativeWidth, y = +relativeHeight, z = +0.5f},
                new float3 {x = +relativeWidth, y = +relativeHeight, z = +0.5f}
            };
            return vertices;
        }

        #endregion

        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private bool _keys;

        #if GUI_SIMPLE
        private GUIHandler _guiHandler;

        private GUIButton _guiFuseeLink;
        private GUIImage _guiFuseeLogo;
        private FontMap _guiLatoBlack;
        private GUIText _guiSubText;

        private GUIButton _bltButton;

        private float _subtextHeight;
        private float _subtextWidth;

        private string _text;
        #endif

        // Init is called on startup. 
        public override void Init()
        {
#if GUI_SIMPLE
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            fontLato.UseKerning = true;
            _guiLatoBlack = new FontMap(fontLato, 18);

            _guiFuseeLogo = new GUIImage(AssetStorage.Get<ImageData>("FuseeText.png"), 10, 10, -5, 174, 50);

            _guiFuseeLink = new GUIButton(6, 6, 182, 58);
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderColor = ColorUint.Tofloat4(ColorUint.Greenery);
            _guiFuseeLink.BorderWidth = 0;
            _guiFuseeLink.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            _guiFuseeLink.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            _guiFuseeLink.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            _guiHandler.Add(_guiFuseeLink);
            
            _guiHandler.Add(_guiFuseeLogo);

            _bltButton = new GUIButton("Blt CPU to GPU", _guiLatoBlack, 206, 6, 182, 58);
            _bltButton.ButtonColor = new float4(0.66f, 0.66f, 0.66f, 1);
            _bltButton.BorderColor = ColorUint.Tofloat4(ColorUint.Greenery);
            _bltButton.BorderWidth = 0;
            _bltButton.OnGUIButtonDown += BltTextureOnOnGuiButtonDown;
            _bltButton.OnGUIButtonEnter += _guiBlt_OnGUIButtonEnter;
            _bltButton.OnGUIButtonLeave += _guiBlt_OnGUIButtonLeave;
            _guiHandler.Add(_bltButton);


            _guiHandler.Add(_guiFuseeLogo);
            

            _text = "Image Blit Example: Demonstrates \"Bit Block Image Transfer\" in Modes CPU->CPU and CPU->GPU on button press.";

            _guiSubText = new GUIText(_text, _guiLatoBlack, 300, 100);
            _guiSubText.TextColor = ColorUint.Tofloat4(ColorUint.Black);
            _guiHandler.Add(_guiSubText);
            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);

#endif

            // BEGIN SETUP blit test
            this.blitSource = AssetStorage.Get<ImageData>("censored_79_16.png"); // init local blitSource for GuiButtonHandler
            // https://pixabay.com/en/road-transport-system-city-winter-3036620/ Creative Commons CC0
            // https://pixabay.com/en/bremen-town-musicians-donkey-dog-1651945/ Creative Commons CC0
            ImageData cpuBlitDestination = AssetStorage.Get<ImageData>("townmusicians.jpg");
            ImageData gpuBlitDestination = AssetStorage.Get<ImageData>("townmusicians.jpg");

            ImageData blitSource = AssetStorage.Get<ImageData>("censored_79_16.png");

            cpuBlitDestination.Blt(185, 230, blitSource, 0, 0, cpuBlitDestination.Width, cpuBlitDestination.Height); // CPU to CPU

            blitCPUtoCPUDestinationTexture = new Texture(cpuBlitDestination);
            blitCPUtoGPUDestinationTexture = new Texture(gpuBlitDestination);
            blitGPUtoGPUDestinationTexture = new Texture(gpuBlitDestination);

            //#if GUI_SIMPLE
            //            _guiBlitDestinationImage = new GUIImage(blitCPUtoGPUDestinationTexture, 10, 10, 0, 277, 500);
            //            _guiHandler.Add(_guiBlitDestinationImage);
            //
            //#endif
            // Create billboard Mesh
            _billboardMesh = CreatePlaneMesh(DesiredWidth, DesiredHeight, Width, Height); // billboard mesh will contain townmusicians.jpg

            float4x4 billboardMatrix = float4x4.CreateScale(1, 1, 1) * float4x4.CreateTranslation(-0.5f, 0, 0);

            // create shadereffect
            billBoardShaderEffect = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = BillboardVs,
                        PS = BillboardPs,
                        StateSet = new RenderStateSet
                        {
                            AlphaBlendEnable = true,
                            SourceBlend = Blend.SourceAlpha,
                            DestinationBlend = Blend.InverseSourceAlpha,
                            ZEnable = true
                        }
                    }
                },
                new[]
                {
                    new EffectParameterDeclaration {Name = "tex", Value = blitCPUtoGPUDestinationTexture},
                    //new EffectParameterDeclaration {Name = "uColor", Value = BillBoardRedColor},
                    new EffectParameterDeclaration {Name = "guiXForm", Value = billboardMatrix},
                });

            // END SETUP Blit Test

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

        }

        private void BltTextureOnOnGuiButtonDown(GUIButton sender, GUIButtonEventArgs ea)
        {
            Random random = new Random();
            int randomWidth = random.Next(0, blitCPUtoGPUDestinationTexture.Width - 79);
            int randomHeight = random.Next(0, blitCPUtoGPUDestinationTexture.Height - 16);
            
            blitCPUtoGPUDestinationTexture.Blt(randomWidth, randomHeight, blitSource);
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }             

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }


            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -600, 0, 150, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            // Render the scene loaded in Init()
            RC.SetShaderEffect(billBoardShaderEffect);
            billBoardShaderEffect.SetEffectParam("guiXForm", _leftImageFloat4X4);
            //billBoardShaderEffect.SetEffectParam("uColor", BillBoardRedColor);
            billBoardShaderEffect.SetEffectParam("tex", blitCPUtoCPUDestinationTexture);
            RC.Render(_billboardMesh);

            billBoardShaderEffect.SetEffectParam("guiXForm", _centerImageFloat4X4);
            //billBoardShaderEffect.SetEffectParam("uColor", BillBoardGreenColor);
            billBoardShaderEffect.SetEffectParam("tex", blitCPUtoGPUDestinationTexture);
            RC.Render(_billboardMesh);

            billBoardShaderEffect.SetEffectParam("guiXForm", _rightImageFloat4X4);
            //billBoardShaderEffect.SetEffectParam("uColor", BillBoardBlueColor);
            billBoardShaderEffect.SetEffectParam("tex", blitGPUtoGPUDestinationTexture);
            RC.Render(_billboardMesh);

#if GUI_SIMPLE
            _guiHandler.RenderGUI();
            #endif
            
            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width/(float) Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 2000);
            RC.Projection = projection;


            // Update Billboard Mesh vertices
            if (_billboardMesh != null)
            {
                _billboardMesh.Vertices = OnResizeCalculateMeshVertices(DesiredWidth, DesiredHeight, Width, Height);
            }


            #if GUI_SIMPLE
            _guiSubText.PosX = (int)((Width - _subtextWidth) / 2);
            _guiSubText.PosY = 90;
            _guiHandler.Refresh();
            #endif

        }

        #if GUI_SIMPLE
        private void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = new float4(0, 0, 0, 0);
            sender.BorderWidth = 0;
            SetCursor(CursorType.Standard);
        }

        private void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = new float4(0.533f, 0.69f, 0.3f, 0.4f);
            sender.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        private void _guiBlt_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = new float4(0.66f, 0.66f, 0.66f, 1);
            sender.TextColor = new float4(.0f, .0f, .0f, 1.0f);
            sender.BorderWidth = 1;
            SetCursor(CursorType.Standard);
        }

        private void _guiBlt_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = new float4(0.533f, 0.69f, 0.3f, 0.4f);
            sender.TextColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            sender.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea)
        {
            OpenLink("http://fusee3d.org");
        }
        #endif
    }
}