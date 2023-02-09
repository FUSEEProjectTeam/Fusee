using NUnitLite;
using System;

namespace Fusee.Tests.Render.Desktop
{
    internal static class Run
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Start Render");

            Program.Example = new Fusee.Examples.AdvancedUI.Core.AdvancedUI() { rnd = new System.Random(12345) };
            Program.Init("AdvancedUI.png");

            Program.Example = new Fusee.Examples.Camera.Core.CameraExample() { };
            Program.Init("Camera.png");

            Program.Example = new Fusee.Examples.ComputeFractal.Core.ComputeFractal() { };
            Program.Init("Fractal.png");

            Program.Example = new Fusee.Examples.Deferred.Core.Deferred() { };
            Program.Init("Deferred.png");

            Program.Example = new Fusee.Examples.GeometryEditing.Core.GeometryEditing();
            Program.Init("GeometryEditing.png");

            Program.Example = new Fusee.Examples.Labyrinth.Core.Labyrinth() { };
            Program.Init("Labyrinth.png");

            Program.Example = new Fusee.Examples.Materials.Core.Materials { };
            Program.Init("Materials.png");

            Program.Example = new Fusee.Examples.MeshingAround.Core.MeshingAround();
            Program.Init("MeshingAround.png");

            Program.Example = new Fusee.Examples.Picking.Core.Picking();
            Program.Init("Picking.png");

            Program.Example = new Fusee.Examples.PointCloudPotree2.Core.PointCloudPotree2();
            Program.Init("PointCloudPotree2.png");

            Program.Example = new Fusee.Examples.RenderContextOnly.Core.RenderContextOnly();
            Program.Init("RenderContextOnly.png");

            Program.Example = new Fusee.Examples.RenderLayerEx.Core.RenderLayerExample();
            Program.Init("RenderLayer.png");

            Program.Example = new Fusee.Examples.Simple.Core.Simple();
            Program.Init("Simple.png");

            Program.Example = new Fusee.Examples.ThreeDFont.Core.ThreeDFont();
            Program.Init("ThreeDFont.png");

            Program.Example = new Fusee.Examples.UI.Core.UI();
            Program.Init("UI.png");

            return new AutoRun().Execute(args);
        }
    }
}