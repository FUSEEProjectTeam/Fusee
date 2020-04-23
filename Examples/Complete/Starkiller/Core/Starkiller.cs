using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Engine.Core.Scene;

namespace Fusee.Examples.Starkiller.Core
{
    [FuseeApplication(Name = "Starkiller", Description = "Yet another FUSEE App.")]
    public class Starkiller : RenderCanvas
    {
        private SceneContainer _starkillerScene;
        private SceneRendererForward _sceneRenderer;
        private Transform _Rumpf;
        private Transform _TwLinks;
        private Transform _TwRechts;
        private Transform _starkillerTransform = new Transform();
        private SceneContainer _scene;


        private SceneContainer CreateScene()
        {
            //load the Nodes in
            SceneContainer _starkillerScene = AssetStorage.Get<SceneContainer>("StarkillerAssets.fus");
            SceneNode meteorit1 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit1").First();
            SceneNode visier = _starkillerScene.Children.FindNodes(n => n.Name == "Visier").First();
            SceneNode rumpf = _starkillerScene.Children.FindNodes(n => n.Name == "Rumpf").First();
            SceneNode tw_links = _starkillerScene.Children.FindNodes(n => n.Name == "TW_Links").First();
            SceneNode tw_rechts = _starkillerScene.Children.FindNodes(n => n.Name == "TW_Links").First();

            SceneNode starkiller = new SceneNode
            {
                Components = new List<SceneComponent>
                {
                    // TRANSFROM COMPONENT
                    _starkillerTransform,
                    // SHADER EFFECT COMPONENT
                    rumpf.GetComponent<ShaderEffect>(),
                },
                Children = new ChildList()
            };

            starkiller.Children.Add(new SceneNode
            {
                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Translation = new float3(0,0,40),
                                    },
                                    visier.GetComponent<ShaderEffect>(),
                                    visier.GetComponent<Mesh>()
                                },
                Name = "Schiff",

                Children = new ChildList
                            {
                                new SceneNode
                                {
                                    Components = new List<SceneComponent>
                                    {
                                    new Transform
                                        {
                                            Translation = new float3(0,0,0)
                                        },
                                           rumpf.GetComponent<ShaderEffect>(),
                                           rumpf.GetComponent<Mesh>()
                                    },
                                Name = "Rumpf",

                                Children = new ChildList
                                {
                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                        new Transform
                                            {
                                                Translation = new float3(0,0,0)
                                            },
                                            tw_links.GetComponent<ShaderEffect>(),
                                            tw_links.GetComponent<Mesh>()
                                        },
                                    Name = "TwLinks",
                                    },

                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                        new Transform
                                            {
                                                Translation = new float3(0,0,0)
                                            },
                                            tw_rechts.GetComponent<ShaderEffect>(),
                                            tw_rechts.GetComponent<Mesh>()
                                        },
                                    Name = "TwRechts",
                                    }
                                }
                            },
                        }
                    }
                    );

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    starkiller
                }
            };
        }


        public override async Task<bool> Init()
        {
            //Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 0, 0, 0);

          





            //Wrap a SceneRenderer around the model.
            _scene = CreateScene();
            _sceneRenderer = new SceneRendererForward(_scene);

            _Rumpf = _scene.Children.FindNodes(n => n.Name == "Rumpf")?.FirstOrDefault()?.GetTransform();
            _TwLinks = _scene.Children.FindNodes(n => n.Name == "TwLinks")?.FirstOrDefault()?.GetTransform();
            _TwRechts = _scene.Children.FindNodes(n => n.Name == "TwRechts")?.FirstOrDefault()?.GetTransform();

            return true;
        }




        //RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);


            RC.View = float4x4.CreateTranslation(0, -20, 50) * float4x4.CreateRotationX(-5 * M.Pi / 180);

            //Bewegung des Rumpfs auf x und y
            float rumpfBewegungHorizontal = _Rumpf.Translation.x;
            rumpfBewegungHorizontal += 0.5f * Keyboard.ADAxis;

            float rumpfBewegungVertikal = _Rumpf.Translation.y;
            rumpfBewegungVertikal += 0.5f * Keyboard.WSAxis;

            _Rumpf.Translation = new float3(rumpfBewegungHorizontal, rumpfBewegungVertikal, 0);

            //Barrelroll

            if (Keyboard.IsKeyDown(KeyCodes.Q))
            {
                float barrelroll = _Rumpf.Rotation.z;
                barrelroll += 5 * M.Pi / 180;
                _Rumpf.Rotation = new float3(0, 0, barrelroll);
            }
            else if (Keyboard.IsKeyDown(KeyCodes.E))
            {
                float barrelroll = _Rumpf.Rotation.z;
                barrelroll -= 5 * M.Pi / 180;
                _Rumpf.Rotation = new float3(0, 0, barrelroll);
            }


            //Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            //Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
        public void SetProjectionAndViewport()

        {

            //Set the rendering area to the entire window size

            RC.Viewport(0, 0, Width, Height);



            //Create a new projection matrix generating undistorted images on the new aspect ratio.

            var aspectRatio = Width / (float)Height;



            //0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio

            //Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)

            //Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)

            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);

            RC.Projection = projection;

        }


    }
}
