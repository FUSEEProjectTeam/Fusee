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
using Fusee.Xirkit;

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
        

        bool gamestart = true;
       
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
            //Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 0, 0, 0);

            //Wrap a SceneRenderer around the model.
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

            ////Bewegung des Schiffs
            float bewegungHorizontal = _schiff.GetTransform().Translation.x;
            bewegungHorizontal += 0.7f * Keyboard.ADAxis;
            float bewegungVertikal = _schiff.GetTransform().Translation.y;
            bewegungVertikal += 0.7f * Keyboard.WSAxis;

            _schiff.GetTransform().Translation.x = bewegungHorizontal;
            _schiff.GetTransform().Translation.y = bewegungVertikal;
            _schiff.GetTransform().Translation.z = 0;

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

            for (var i = 0; i < _projectiles.Children.Count; i++)
            {
                if (abgefeuert[i])
                {
                   _projectiles.Children[i].GetTransform().Translation.z += DeltaTime*300; 

                    if(_projectiles.Children[i].GetTransform().Translation.z > 500)
                    {
                        _projectiles.Children[i].GetTransform().Translation.z = -50;
                        abgefeuert[i] = false;
                    }
                }
               
            }

            //Geschoss abfeuern


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
