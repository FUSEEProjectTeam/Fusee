using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.RenderLayerEx.Core
{
    /// <summary>
    /// This example builds a 3x3 grid of rockets rendered by 2 cameras.
    /// - The left camera is on RenderLayers.Layer01 the right is on RenderLayers.Layer01.
    /// - The corner rockets are on RenderLayers.All => rendered by both cameras
    /// - The center-center rocket is on RenderLayers.None => not rendered by any camera
    /// - The center rockets on the top and bottom are on RenderLayers.Layer01 => only rendered by the left camera.
    /// - The center rockets to the left and right are on RenderLayers.Layer02 => only rendered by the right camera.
    /// </summary>
    [FuseeApplication(Name = "FUSEE RenderLayer Example", Description = "A very RenderLayer example.")]
    public class RenderLayerExample : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        // Init is called on startup.
        public override void Init()
        {
            _scene = new SceneContainer();

            // Loading the rocket and picking it apart for reuse
            var rocket = AssetStorage.Get<SceneContainer>("FUSEERocket.fus");
            
            var gray = rocket.Children[0].Children[0];
            var white = rocket.Children[0].Children[1];
            var green = rocket.Children[0].Children[2];

            var rocketTransform = rocket.Children[0].GetTransform();
            
            var grayShEf = gray.GetComponent<ShaderEffect>();
            var grayMesh = gray.GetComponent<Mesh>();
            var whiteShEf = white.GetComponent<ShaderEffect>();
            var whiteMesh = white.GetComponent<Mesh>();
            var greenShEf = green.GetComponent<ShaderEffect>();
            var greenMesh = green.GetComponent<Mesh>();

            // RenderLayer Components
            var rl1 = new RenderLayer { Layer = RenderLayers.Layer01 };
            var rl2 = new RenderLayer { Layer = RenderLayers.Layer02 };
            var rln = new RenderLayer { Layer = RenderLayers.None };
            var rla = new RenderLayer { Layer = RenderLayers.All };

            // 1st Level
            var n1 = new SceneNode();
            n1.AddComponent(new Transform { Translation = new float3(0, -4, 0) });
            _scene.Children.Add(n1);
            var n2 = new SceneNode();
            n2.AddComponent(new Transform { Translation = new float3(0, 0, 0) });
            _scene.Children.Add(n2);
            var n3 = new SceneNode();
            n3.AddComponent(new Transform { Translation = new float3(0, 4, 0) });
            _scene.Children.Add(n3);

            // Rockets Bottom Row
            var c11 = new SceneNode();
            c11.AddComponent(rla);
            c11.AddComponent(new Transform { Translation = new float3(-2, 0, 0) });
            c11.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n1.Children.Add(c11);
            var c12 = new SceneNode();
            c12.AddComponent(rl1);
            c12.AddComponent(new Transform { Translation = new float3(0, 0, 0) });
            c12.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n1.Children.Add(c12);
            var c13 = new SceneNode();
            c13.AddComponent(rla);
            c13.AddComponent(new Transform { Translation = new float3(2, 0, 0) });
            c13.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n1.Children.Add(c13);

            // Rockets Middle Row
            var c21 = new SceneNode();
            c21.AddComponent(rl2);
            c21.AddComponent(new Transform { Translation = new float3(-2, 0, 0) });
            c21.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n2.Children.Add(c21);
            var c22 = new SceneNode();
            c22.AddComponent(rln);
            c22.AddComponent(new Transform { Translation = new float3(0, 0, 0) });
            c22.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n2.Children.Add(c22);
            var c23 = new SceneNode();
            c23.AddComponent(rl2);
            c23.AddComponent(new Transform { Translation = new float3(2, 0, 0) });
            c23.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n2.Children.Add(c23);

            // Rockets Top Row
            var c31 = new SceneNode();
            c31.AddComponent(rla);
            c31.AddComponent(new Transform { Translation = new float3(-2, 0, 0) });
            c31.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n3.Children.Add(c31);
            var c32 = new SceneNode();
            c32.AddComponent(rl1);
            c32.AddComponent(new Transform { Translation = new float3(0, 0, 0) });
            c32.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n3.Children.Add(c32);
            var c33 = new SceneNode();
            c33.AddComponent(rla);
            c33.AddComponent(new Transform { Translation = new float3(2, 0, 0) });
            c33.Children.Add(new SceneNode { Components = { rocketTransform, grayShEf, grayMesh, greenShEf, greenMesh, whiteShEf, whiteMesh } });
            n3.Children.Add(c33);

            // Cameras
            var cam = new SceneNode();
            cam.AddComponent(new Transform { Translation = new float3(0, 1.7f, -16), Rotation = new float3(0, 0, 0) });

            var cam1 = new Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
            cam1.Viewport = new float4(0, 0, 50, 100);
            cam1.BackgroundColor = new float4(new float4(1, 0.8f, 0.8f, 1));
            cam1.Layer = 1;
            cam1.FrustumCullingOn = false;
            cam1.RenderLayer = RenderLayers.Layer01;
            cam.AddComponent(cam1);

            var cam2 = new Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
            cam2.Viewport = new float4(50, 0, 50, 100);
            cam2.BackgroundColor = new float4(new float4(0.8f, 1, 0.8f, 1));
            cam2.Layer = 1;
            cam2.FrustumCullingOn = false;
            cam2.RenderLayer = RenderLayers.Layer02;
            cam.AddComponent(cam2);

            _scene.Children.Add(cam);

            _sceneRenderer = new SceneRendererForward(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _sceneRenderer.Render(RC);

            Present();
        }
    }
}