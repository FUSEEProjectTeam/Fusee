using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.ShaderDemo
{
    // ShaderDemo
    public class ShaderDemo : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // Meshes
        private Mesh _currentMesh;
        private Mesh _meshCube;
        private Mesh _meshSphere;
        private Mesh _meshTeapot;
        private string _textCurrentMesh;
        // ShaderPrograms
        private ShaderProgram _currentShader;
        private ShaderProgram _shaderDiffuseColor;
        private ShaderProgram _shaderTexture;
        private ShaderProgram _shaderDiffuseTexture;
        private ShaderProgram _shaderDiffuseBumpTexture;
        private ShaderProgram _shaderSpecularTexture;
        private string _textShaderName = "Diffuse Color";
        // ShaderParams
        private IShaderParam _paramColor;
        private IShaderParam _paramTexture;
        private IShaderParam _paramBumpTexture;
        private IShaderParam _paramSpecular;
        private IShaderParam _paramShininess;

        // iTextures
        private ITexture _texCube;
        private ITexture _texBumpCube;
        private ITexture _texSphere;
        private ITexture _texBumpSphere;
        private ITexture _texTeapot;
        private ITexture _texBumpTeapot;
        private ITexture _currentTexture;
        private ITexture _currentBumpTexture;

        // Toon Shader Effect
        private ShaderEffect _shaderEffect = new ShaderEffect(new[]
            {
               new EffectPassDeclaration
                   {
          VS = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = normalize(vNormal);
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) ) + vec4(5.0 * vNormal.x, 5.0 * vNormal.y, 0, 0);
                vUV = fuUV;
            }",

        PS = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vec4(0, 0, 0, 1);
            }",

          StateSet = new RenderStateSet()
               {
                    AlphaBlendEnable = false,
                    ZEnable = false
                }
             },

         new EffectPassDeclaration
             {
                   VS = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) ) * vec4(1, 1, 1, 1);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }",

        PS = @"

            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                // vec4 result = vec4(0.3, 1, 0.7, 1) * dot(vNormal, vec3(0, 0, 1));
                vec4 result = vColor * dot(vNormal, vec3(0, 0, 1));
                result = vec4(floor(result.r * 3.0 + 0.5)/3.0, floor(result.g * 3.0 + 0.5)/3.0, floor(result.b* 3.0 + 0.5)/3.0, result.a); 
                gl_FragColor = result;
                // gl_FragColor = vec4(1, 0, 0, 1);
            }",

         StateSet = new RenderStateSet()
               {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                    //BlendFactor = new float4(0.5f, 0.5f, 0.5f, 0.5f),
                    //BlendOperation = BlendOperation.Add,
                    //SourceBlend = Blend.BlendFactor,
                    //DestinationBlend = Blend.InverseBlendFactor
                }
             },
            },

            new[]
                {
                    new EffectParameterDeclaration {Name = "vColor", Value = new float4(1, 0.3f, 0.7f, 1)}, 
                }); 


        /// GUI AREA ///
        // Overall GUI handler
        private GUIHandler _guiHandler;

        // GUI Images
        private GUIImage _guiImage;
        private GUIImage _borderImage;
        // Fonts
        private IFont _guiFont_Cabin18;
        private IFont _guiFont_Cabin24;

        // Repeatable Background Dimensions
        private static float bg_xmax = 128;
        private static float bg_ymax = 128;

        // Button Background Colors
        private static float4 colorDefaultButton = new float4(1, 1, 1, 1); // White
        private static float4 colorHighlightedButton = new float4(1, 0.588f, 0, 1); // Orange
        // Application Title Text
        private GUIText _guiText;

        //* Menu 1: Select Shader
        private GUIPanel _panelSelectShader;
        //** Possible Shader Buttons
        private GUIButton btn_DiffuseColorShader;
        private GUIButton btn_TextureShader;
        private GUIButton btn_DiffuseTextureShader;
        private GUIButton btn_DiffuseBumpTextureShader;
        private GUIButton btn_SpecularTexture;
        private GUIButton btn_Toon;

        //* Menu 2: Light Settings
        private GUIPanel _panelLightSettings;
        //** Possible Light Settings
        private GUIButton btn_DirectionalLight;
        private GUIButton btn_PointLight;
        private GUIButton btn_SpotLight;

        //* Menu 3: Select Mesh
        private GUIPanel _panelSelectMesh;
        //** Possible selectable Meshes
        private GUIButton btn_Cube;
        private GUIButton btn_Sphere;
        private GUIButton btn_Teapot;

        public override void Init()
        {
            // Set ToonShaderEffect
            _shaderEffect.AttachToContext(RC);

            // Setup GUI

            // GUIHandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            // font + text
            _guiFont_Cabin18 = RC.LoadFont("Assets/Cabin.ttf", 18);
            _guiFont_Cabin24 = RC.LoadFont("Assets/Cabin.ttf", 24);

            _guiText = new GUIText("FUSEE Shader Demo", _guiFont_Cabin24, 30, Height-30);
            _guiText.TextColor = new float4(1, 1, 1, 1);

            _guiHandler.Add(_guiText);

            // image
            _guiImage = new GUIImage("Assets/repbg.jpg", 0, 0, -5, Width, Height);
            _guiHandler.Add(_guiImage);
            _borderImage = new GUIImage("Assets/redbg.png", 0, 0, -4, 230, 32);
            //_guiHandler.Add(_borderImage);

            //* Menu1: Select Shader
            _panelSelectShader = new GUIPanel("Select Shader", _guiFont_Cabin24, 10, 10, 230, 230);
            _panelSelectShader.ChildElements.Add(_borderImage);
            _guiHandler.Add(_panelSelectShader);
            //** Possible Shader Buttons
            btn_DiffuseColorShader = new GUIButton("Diffuse Color", _guiFont_Cabin18, 25, 40, 180, 25);
            btn_TextureShader = new GUIButton("Texture Only", _guiFont_Cabin18, 25, 70, 180, 25);
            btn_DiffuseTextureShader = new GUIButton("Diffuse Texture", _guiFont_Cabin18, 25, 100, 180, 25);
            btn_DiffuseBumpTextureShader = new GUIButton("Diffuse Bump Texture", _guiFont_Cabin18, 25, 130, 180, 25);
            btn_SpecularTexture = new GUIButton("Specular Texture", _guiFont_Cabin18, 25, 160, 180, 25);
            btn_Toon = new GUIButton("Toon", _guiFont_Cabin18, 25, 190, 180, 25);
            //*** Add Handlers
            btn_DiffuseColorShader.OnGUIButtonDown += OnMenuButtonDown;
            btn_DiffuseColorShader.OnGUIButtonUp += OnMenuButtonUp;
            btn_DiffuseColorShader.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_DiffuseColorShader.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_TextureShader.OnGUIButtonDown += OnMenuButtonDown;
            btn_TextureShader.OnGUIButtonUp += OnMenuButtonUp;
            btn_TextureShader.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_TextureShader.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_DiffuseTextureShader.OnGUIButtonDown += OnMenuButtonDown;
            btn_DiffuseTextureShader.OnGUIButtonUp += OnMenuButtonUp;
            btn_DiffuseTextureShader.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_DiffuseTextureShader.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_DiffuseBumpTextureShader.OnGUIButtonDown += OnMenuButtonDown;
            btn_DiffuseBumpTextureShader.OnGUIButtonUp += OnMenuButtonUp;
            btn_DiffuseBumpTextureShader.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_DiffuseBumpTextureShader.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_SpecularTexture.OnGUIButtonDown += OnMenuButtonDown;
            btn_SpecularTexture.OnGUIButtonUp += OnMenuButtonUp;
            btn_SpecularTexture.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_SpecularTexture.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_Toon.OnGUIButtonDown += OnMenuButtonDown;
            btn_Toon.OnGUIButtonUp += OnMenuButtonUp;
            btn_Toon.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_Toon.OnGUIButtonLeave += OnMenuButtonLeave;
            //**** Add Buttons to Panel
            _panelSelectShader.ChildElements.Add(btn_DiffuseColorShader);
            _panelSelectShader.ChildElements.Add(btn_TextureShader);
            _panelSelectShader.ChildElements.Add(btn_DiffuseTextureShader);
            _panelSelectShader.ChildElements.Add(btn_DiffuseBumpTextureShader);
            _panelSelectShader.ChildElements.Add(btn_SpecularTexture);
            _panelSelectShader.ChildElements.Add(btn_Toon);
            //* Menu3: Select Mesh
            _panelSelectMesh = new GUIPanel("Select Mesh", _guiFont_Cabin24, 270, 10, 230, 130);
            _panelSelectMesh.ChildElements.Add(_borderImage);
            _guiHandler.Add(_panelSelectMesh);
            //** Possible Meshes
            btn_Cube = new GUIButton("Cube", _guiFont_Cabin18, 25, 40, 180, 25);
            btn_Sphere = new GUIButton("Sphere", _guiFont_Cabin18, 25, 70, 180, 25);
            btn_Teapot = new GUIButton("Teapot", _guiFont_Cabin18, 25, 100, 180, 25);
            _panelSelectMesh.ChildElements.Add(btn_Cube);
            _panelSelectMesh.ChildElements.Add(btn_Sphere);
            _panelSelectMesh.ChildElements.Add(btn_Teapot);

            //** Add handlers 
            btn_Cube.OnGUIButtonDown += OnMenuButtonDown;
            btn_Cube.OnGUIButtonUp += OnMenuButtonUp;
            btn_Cube.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_Cube.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_Sphere.OnGUIButtonDown += OnMenuButtonDown;
            btn_Sphere.OnGUIButtonUp += OnMenuButtonUp;
            btn_Sphere.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_Sphere.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_Teapot.OnGUIButtonDown += OnMenuButtonDown;
            btn_Teapot.OnGUIButtonUp += OnMenuButtonUp;
            btn_Teapot.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_Teapot.OnGUIButtonLeave += OnMenuButtonLeave;

            //* Menu2: Light Settings
            _panelLightSettings = new GUIPanel("Light Settings", _guiFont_Cabin24, 530, 10, 230, 130);
            _panelLightSettings.ChildElements.Add(_borderImage);
            _guiHandler.Add(_panelLightSettings);
            //** Possible Light Settings
            btn_DirectionalLight = new GUIButton("Directional Light", _guiFont_Cabin18, 25, 40, 180, 25);
            btn_PointLight = new GUIButton("Point Light", _guiFont_Cabin18, 25, 70, 180, 25);
            btn_SpotLight = new GUIButton("Spot Light", _guiFont_Cabin18, 25, 100, 180, 25);
            _panelLightSettings.ChildElements.Add(btn_DirectionalLight);
            _panelLightSettings.ChildElements.Add(btn_PointLight);
            _panelLightSettings.ChildElements.Add(btn_SpotLight);

            //*** Add Handlers
            btn_DirectionalLight.OnGUIButtonDown += OnMenuButtonDown;
            btn_DirectionalLight.OnGUIButtonUp += OnMenuButtonUp;
            btn_DirectionalLight.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_DirectionalLight.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_PointLight.OnGUIButtonDown += OnMenuButtonDown;
            btn_PointLight.OnGUIButtonUp += OnMenuButtonUp;
            btn_PointLight.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_PointLight.OnGUIButtonLeave += OnMenuButtonLeave;

            btn_SpotLight.OnGUIButtonDown += OnMenuButtonDown;
            btn_SpotLight.OnGUIButtonUp += OnMenuButtonUp;
            btn_SpotLight.OnGUIButtonEnter += OnMenuButtonEnter;
            btn_SpotLight.OnGUIButtonLeave += OnMenuButtonLeave;

            // Setup 3D Scene
            // Load Images and Assign iTextures
            var imgTexture = RC.LoadImage("Assets/crateTexture.jpg");
            var imgBumpTexture = RC.LoadImage("Assets/crateNormal.jpg");
            _texCube = RC.CreateTexture(imgTexture);
            _texBumpCube = RC.CreateTexture(imgBumpTexture);

            imgTexture = RC.LoadImage("Assets/earthTexture.jpg");
            imgBumpTexture = RC.LoadImage("Assets/earthNormal.jpg");
            _texSphere = RC.CreateTexture(imgTexture);
            _texBumpSphere = RC.CreateTexture(imgBumpTexture);

            imgTexture = RC.LoadImage("Assets/porcelainTexture.png");
            imgBumpTexture = RC.LoadImage("Assets/normalRust.jpg");
            _texTeapot = RC.CreateTexture(imgTexture);
            _texBumpTeapot = RC.CreateTexture(imgBumpTexture);

            _currentTexture = _texCube;
            _currentBumpTexture = _texBumpCube;
            // Load Meshes
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            _meshSphere = MeshReader.LoadMesh(@"Assets/Sphere.obj.model");
            _meshTeapot = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");

            // Set current Mesh and Update GUI
            _currentMesh = _meshCube;
            btn_Cube.ButtonColor = colorHighlightedButton;

            // Setup Shaderprograms and Update GUI
            _shaderDiffuseColor = MoreShaders.GetDiffuseColorShader(RC);
            _shaderDiffuseTexture = MoreShaders.GetDiffuseTextureShader(RC);
            _shaderTexture = MoreShaders.GetTextureShader(RC);
            _shaderDiffuseBumpTexture = MoreShaders.GetBumpDiffuseShader(RC);
            _shaderSpecularTexture = MoreShaders.GetSpecularShader(RC);
            btn_DiffuseColorShader.ButtonColor = colorHighlightedButton;
            _currentShader = _shaderDiffuseColor;
            RC.SetShader(_shaderDiffuseColor);
            // Setup ShaderParams
            _paramColor = _shaderDiffuseColor.GetShaderParam("color");
            // Setup Light and Update GUI
            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(0, new float4(0.2f, 0.2f, 0.2f, 1.0f));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(0, new float4(0.8f, 0.8f, 0.8f, 1.0f));
            RC.SetLightDirection(0, new float3(-1.0f, 0.0f, 0.0f));
            RC.SetLightSpotAngle(0, 10);
            btn_DirectionalLight.ButtonColor = colorHighlightedButton;
        }

     

        private void OnMenuButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;
            //sender.ButtonColor = colorHighlightedButton;
            
        }

        private void OnMenuButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            if (!isButtonSelected(sender))
            {
                sender.ButtonColor = colorHighlightedButton;
            }
            
            switch (sender.Text)
            {
                case "Diffuse Color":
                    _textShaderName = sender.Text;
                        deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                        _currentShader = _shaderDiffuseColor;
                        _paramColor = _currentShader.GetShaderParam("color");
                    break;
                case "Texture Only":
                    _textShaderName = sender.Text;
                    deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                    _currentShader = _shaderTexture;
                    _paramTexture = _currentShader.GetShaderParam("texture1");
                    break;
                case "Diffuse Texture":
                    _textShaderName = sender.Text;
                    deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                    _currentShader = _shaderDiffuseTexture;
                    _paramTexture = _currentShader.GetShaderParam("texture1");
                    break;
                case "Diffuse Bump Texture":
                    _textShaderName = sender.Text;
                    deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                    _currentShader = _shaderDiffuseBumpTexture;
                    _paramTexture = _currentShader.GetShaderParam("texture1");
                    _paramBumpTexture = _currentShader.GetShaderParam("normalTex");
                    _paramSpecular = _currentShader.GetShaderParam("specularLevel");
                    _paramShininess = _currentShader.GetShaderParam("shininess");
                    break;
                case "Specular Texture":
                    _textShaderName = sender.Text;
                    deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                    _currentShader = _shaderSpecularTexture;
                    _paramTexture = _currentShader.GetShaderParam("texture1");
                    _paramSpecular = _currentShader.GetShaderParam("specularLevel");
                    _paramShininess = _currentShader.GetShaderParam("shininess");
                    break;
                case "Toon":
                    _textShaderName = sender.Text;
                    deselectOtherButtonsFromPanel(_panelSelectShader, sender);
                    break;
                case "Cube":
                    deselectOtherButtonsFromPanel(_panelSelectMesh, sender);
                    setMesh(_meshCube);
                    _currentTexture = _texCube;
                    _currentBumpTexture = _texBumpCube;
                    _textCurrentMesh = "Cube";
                    break;
                case "Sphere":
                    deselectOtherButtonsFromPanel(_panelSelectMesh, sender);
                    setMesh(_meshSphere);
                    _currentTexture = _texSphere;
                    _currentBumpTexture = _texBumpSphere;
                    _textCurrentMesh = "Sphere";
                    break;
                case "Teapot":
                    deselectOtherButtonsFromPanel(_panelSelectMesh, sender);
                    setMesh(_meshTeapot);
                    _currentTexture = _texTeapot;
                    _currentBumpTexture = _texBumpTeapot;
                    _textCurrentMesh = "Teapot";
                    break;
                case "Directional Light":
                    deselectOtherButtonsFromPanel(_panelLightSettings, sender);
                    RC.SetLightActive(0, 1);
                    break;
                case "Point Light":
                    deselectOtherButtonsFromPanel(_panelLightSettings, sender);
                    RC.SetLightActive(0, 2);
                    break;
                case "Spot Light":
                    deselectOtherButtonsFromPanel(_panelLightSettings, sender);
                    RC.SetLightActive(0, 3);
                    break;
            }
        }

        private bool isButtonSelected(GUIButton btn)
        {
            return btn.ButtonColor == colorHighlightedButton ? true : false;
        }

        private void deselectOtherButtonsFromPanel(GUIPanel panel, GUIButton newselected_btn)
        {
            for (int i = 0; i < panel.ChildElements.Count; i++)
            {
                if (panel.ChildElements[i].GetType() == typeof(GUIButton))
                {
                    if (panel.ChildElements[i] == newselected_btn) { continue; }
                    GUIButton temp = (GUIButton)panel.ChildElements[i];
                    if (temp.ButtonColor == colorHighlightedButton)
                    {
                        temp.ButtonColor = colorDefaultButton;
                    }
                }
            }
        }

        private void setMesh(Mesh newmesh)
        {
            if (_currentMesh != newmesh)
            {
                _currentMesh = newmesh;
            }
        }

        private static void OnMenuButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
                sender.BorderWidth = 2;

            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
        }

        private static void OnMenuButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            sender.TextColor = new float4(0f, 0f, 0f, 1);
        }

        private void HandleMouseRotations()
        {
            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)System.Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKey(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * mtxCam;
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _guiHandler.RenderGUI();
            HandleMouseRotations();
            SetShaderValues(_currentShader);
            Present();
        }

        private void SetShaderValues(ShaderProgram sp)
        {
            RC.SetShader(sp);
            RenderStateSet myset = new RenderStateSet()
               {
                   AlphaBlendEnable = false,
                   ZEnable = true
               };
            RC.SetRenderState(myset);
            switch (_textShaderName)
            {
                case "Diffuse Color":
                    RC.SetShaderParam(_paramColor,new float4(1,0,0,1));
                    RC.Render(_currentMesh);
                    break;
                case "Texture Only":
                    RC.SetShaderParamTexture(_paramTexture, _currentTexture);
                    RC.Render(_currentMesh);
                    break;
                case "Diffuse Texture":
                    RC.SetShaderParamTexture(_paramTexture, _currentTexture);
                    RC.Render(_currentMesh);
                    break;
                case "Diffuse Bump Texture":
                    RC.SetShaderParamTexture(_paramTexture, _currentTexture);
                    RC.SetShaderParamTexture(_paramBumpTexture, _currentBumpTexture);
                    RC.SetShaderParam(_paramSpecular, 64.0f);
                    RC.SetShaderParam(_paramShininess, 0.5f);
                    RC.Render(_currentMesh);
                    break;
                case "Specular Texture":
                    RC.SetShaderParamTexture(_paramTexture, _currentTexture);
                    RC.SetShaderParam(_paramSpecular, 64.0f);
                    RC.SetShaderParam(_paramShininess, 0.5f);
                    RC.Render(_currentMesh);
                    break;
                case "Toon":
                    RC.SetRenderState(new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        BlendFactor = new float4(0.5f, 0.5f, 0.5f, 0.5f),
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.BlendFactor,
                        DestinationBlend = Blend.InverseBlendFactor
                    });
                    _shaderEffect.RenderMesh(_currentMesh);
                    break;
            }

        }

        private void setTextureToMesh(int diffuseOrBump)
        {

        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            // refresh all elements
            _guiHandler.Refresh();
            // Repeating Texture: xmax==Maximum width size of textureimage, ymax==Maximum height size of textureimage
            float xmax = Width / bg_xmax;
            float ymax = Width / bg_ymax;
            //if (btn_Cube.GUIMesh != null)
            //{
            //    btn_Cube.Refresh();
            //    btn_Cube.GUIMesh.Colors = new uint[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1 };
            //}
            _guiImage.Refresh();
            _guiImage.GUIMesh.UVs = new float2[] { new float2(0, 0), new float2(xmax, 0), new float2(0, ymax), new float2(xmax, ymax) };
        }

        public static void Main()
        {
            var app = new ShaderDemo();
            app.Run();
        }

    }
}