using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using Fusee.Xirkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;



namespace Fusee.Examples.Starkiller.Core
{
    [FuseeApplication(Name = "Starkiller", Description = "Yet another FUSEE App.")]
    public class Starkiller : RenderCanvas
    {
        private SceneRendererForward _sceneRenderer;

        private SceneNode _meteors;
        private SceneNode _projectiles;
        private SceneNode _schiff;

        private float MeteorSpeedFactor = 2;

        private bool[] abgefeuert;

        private SceneContainer _scene;


        float Highscore = 0;
        float Leben = 0;
        bool gamestart = false;


        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;



        private SceneContainer CreateScene()
        {
            SceneContainer sc = new SceneContainer();
            SceneContainer _starkillerScene = AssetStorage.Get<SceneContainer>("StarkillerAssets.fus");

            if (_starkillerScene != null)
            {
                _meteors = AddHierarchy(_starkillerScene, "Meteorit", "Meteors");
                sc.Children.Add(_meteors);

                _projectiles = AddHierarchy(_starkillerScene, "AP", "Projectiles");
                sc.Children.Add(_projectiles);

                abgefeuert = new bool[_projectiles.Children.Count];

                _schiff = _starkillerScene.Children.FindNodes(n => n.Name == "Schiff").First();
                sc.Children.Add(_schiff);
            }

            return sc;
        }


        private SceneNode AddHierarchy(SceneContainer searchTarget, string searchName, string hierarchyName)
        {
            List<SceneNode> projectiles = searchTarget.Children.FindNodes(n => n.Name.Contains(searchName)).ToList();

            var sn = new SceneNode() { Name = hierarchyName };

            foreach (var p in projectiles)
            {
                sn.Children.Add(p);
            }

            return sn;
        }


        public override async Task<bool> Init()
        {
            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;
            //Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 0, 0, 0);

            //Wrap a SceneRenderer around the model.


            _gui = await CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
            _scene = CreateScene();

            _sceneRenderer = new SceneRendererForward(_scene);

            return true;
        }



        //RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);


            RC.View = float4x4.CreateTranslation(0, -20, 50) * float4x4.CreateRotationX(-5 * M.Pi / 180);

            if (Keyboard.IsKeyDown(KeyCodes.Enter))
            {
                if (!gamestart)
                    gamestart = true;
                Leben = 3;
            }
            if (Leben > 0 && gamestart)
            {
                ////Bewegung des Schiffs
                float bewegungHorizontal = _schiff.GetTransform().Translation.x;
                bewegungHorizontal += 0.7f * Keyboard.ADAxis;
                float bewegungVertikal = _schiff.GetTransform().Translation.y;
                bewegungVertikal += 0.7f * Keyboard.WSAxis;

                _schiff.GetTransform().Translation.x = bewegungHorizontal;
                _schiff.GetTransform().Translation.y = bewegungVertikal;
                _schiff.GetTransform().Translation.z = 15;

                //Bewegung der Meteoriten
                foreach (var m in _meteors.Children)
                {

                    var trans = m.GetTransform();

                    if (trans.Translation.z < -200)
                    {
                        trans.Translation.z = 2000;
                    }

                    trans.Translation.z -= (430 - (m.GetMesh().BoundingBox.Size.Length * MeteorSpeedFactor)) * DeltaTime;
                    trans.Rotation.y += 1 * DeltaTime;
                    trans.Rotation.z += 1 * DeltaTime;
                }

                //abfeuern des einzelnen Projektils + Platzierung vor dem Raumschiff
                if (Keyboard.IsKeyDown(KeyCodes.Space))
                {
                    for (var i = 0; i < _projectiles.Children.Count; i++)
                    {
                        if (!abgefeuert[i])
                        {
                            abgefeuert[i] = true;
                            _projectiles.Children[i].GetTransform().Translation.x = _schiff.GetTransform().Translation.x;
                            _projectiles.Children[i].GetTransform().Translation.y = _schiff.GetTransform().Translation.y;
                            _projectiles.Children[i].GetTransform().Translation.z = _schiff.GetTransform().Translation.z + 5;
                            break;
                        }
                    }
                }

                // Bewegung des Projektils
                for (var i = 0; i < _projectiles.Children.Count; i++)
                {
                    if (abgefeuert[i])
                    {
                        _projectiles.Children[i].GetTransform().Translation.z += DeltaTime * 300;

                        if (_projectiles.Children[i].GetTransform().Translation.z > 500)
                        {
                            _projectiles.Children[i].GetTransform().Translation.z = -50;
                            abgefeuert[i] = false;
                        }
                    }

                }

                //Kollisions abfrage
                for (var i = 0; i < _projectiles.Children.Count; i++)
                {
                    if (abgefeuert[i])
                    {
                        var centerX = _projectiles.Children[i].GetMesh().BoundingBox.Center.x + _projectiles.Children[i].GetTransform().Translation.x;
                        var centerY = _projectiles.Children[i].GetMesh().BoundingBox.Center.y + _projectiles.Children[i].GetTransform().Translation.y;
                        var centerZ = _projectiles.Children[i].GetMesh().BoundingBox.Center.z + _projectiles.Children[i].GetTransform().Translation.z;

                        for (var j = 0; j < _meteors.Children.Count; j++)
                        {

                            var minX = _meteors.Children[j].GetMesh().BoundingBox.min.x + _meteors.Children[j].GetTransform().Translation.x;
                            var maxX = _meteors.Children[j].GetMesh().BoundingBox.max.x + _meteors.Children[j].GetTransform().Translation.x;
                            var minY = _meteors.Children[j].GetMesh().BoundingBox.min.y + _meteors.Children[j].GetTransform().Translation.y;
                            var maxY = _meteors.Children[j].GetMesh().BoundingBox.max.y + _meteors.Children[j].GetTransform().Translation.y;
                            var minZ = _meteors.Children[j].GetMesh().BoundingBox.min.z + _meteors.Children[j].GetTransform().Translation.z;
                            var maxZ = _meteors.Children[j].GetMesh().BoundingBox.max.z + _meteors.Children[j].GetTransform().Translation.z;

                            if (minX <= centerX && centerX <= maxX && minY <= centerY && centerY <= maxY && minZ <= centerZ && centerZ <= maxZ)
                            {
                                abgefeuert[i] = false;
                                _projectiles.Children[i].GetTransform().Translation.z = -50;
                                _meteors.Children[j].GetTransform().Translation.z = -100;
                                Highscore += 100;
                            }
                        }
                    }

                    //Schiff Kollision 
                    for (var k = 0; k < _meteors.Children.Count; k++)
                    {

                        var SchiffAABBf = _schiff.GetTransform().Matrix() * _schiff.GetMesh().BoundingBox;



                        var MeteorsAABBf = _meteors.Children[k].GetTransform().Matrix() * _meteors.Children[k].GetMesh().BoundingBox;



                        if (MeteorsAABBf.Intersects(SchiffAABBf))
                        {
                            _schiff.GetTransform().Translation.x = 0;
                            _schiff.GetTransform().Translation.y = 0;
                            _schiff.GetTransform().Translation.z = -100;
                            Leben -= 1;
                            if (Leben == 0)
                            {
                                gamestart = false;
                                Highscore = 0;
                            }

                        }

                    }



                }
            }


            //Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            //Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
        private async Task<SceneContainer> CreateGui()
        {
            var vsTex = await AssetStorage.GetAsync<string>("texture.vert");
            var psTex = await AssetStorage.GetAsync<string>("texture.frag");

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(await AssetStorage.GetAsync<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _initCanvasHeight - 0.5f), _initCanvasHeight, _initCanvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            // Initialize the information text line.
            var textToDisplay = "FUSEE 3D Scene";
            if (_scene.Header.CreatedBy != null || _scene.Header.CreationDate != null)
            {
                textToDisplay += " created";
                if (_scene.Header.CreatedBy != null)
                    textToDisplay += " by " + _scene.Header.CreatedBy;

                if (_scene.Header.CreationDate != null)
                    textToDisplay += " on " + _scene.Header.CreationDate;
            }

            var fontLato = await AssetStorage.GetAsync<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                textToDisplay,
                "SceneDescriptionText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);


            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                });
            canvas.Children.Add(fuseeLogo);
            canvas.Children.Add(text);

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
            void BtnLogoEnter(CodeComponent sender)
            {
                _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", new float4(0.8f, 0.8f, 0.8f, 1f));
            }

            void BtnLogoExit(CodeComponent sender)
            {
                _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", float4.One);
            }

            void BtnLogoDown(CodeComponent sender)
            {
                OpenLink("http://fusee3d.org");
            }
        }
        public void SetProjectionAndViewport()

        {

            //Set the rendering area to the entire window size

            RC.Viewport(0, 0, Width, Height);



            //Create a new projection matrix generating undistorted images on the new aspect ratio.

            var aspectRatio = Width / (float)Height;



            //0.25*PI Rad -> 45� Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio

            //Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)

            //Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)

            var projection = float4x4.CreatePerspectiveFieldOfView(0, aspectRatio, 1, 20000);

            RC.Projection = projection;

        }


    }
}