using NUnitLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.Tests.Render.Desktop
{
    internal static class Run
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Program.Example = new Fusee.Examples.Simple.Core.Simple();
            Program.Init("SimpleTest.png");

            Program.Example = new Fusee.Examples.AdvancedUI.Core.AdvancedUI() { rnd = new System.Random(12345) };
            Program.Init("AdvancedUITest.png");

            //Program.Example = new Fusee.Examples.Bone.Core.Bone();
            //Program.Init("BoneAnimationTest.png");

            Program.Example = new Fusee.Examples.GeometryEditing.Core.GeometryEditing();
            Program.Init("GeometryEditingTest.png");

            Program.Example = new Fusee.Examples.MeshingAround.Core.MeshingAround();
            Program.Init("MeshingAroundTest.png");

            Program.Example = new Fusee.Examples.Picking.Core.Picking();
            Program.Init("PickingTest.png");

            Program.Example = new Fusee.Examples.ThreeDFont.Core.ThreeDFont();
            Program.Init("ThreeDFontTest.png");

            Program.Example = new Fusee.Examples.UI.Core.UI();
            Program.Init("UITest.png");

            return new AutoRun().Execute(args);
        }

    }
}