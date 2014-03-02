using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;
using Fusee.Serialization;

namespace Examples.SceneViewer
{
    public class SceneViewer : RenderCanvas
    {
        // is called on startup
        public override void Init()
        {
            //TestSerialize();
            TestDeserialize();
            RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        private void TestSerialize()
        {
            /**/
            var aMesh = new MeshContainer
            {
                Vertices = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Normals = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Triangles = new ushort[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 1,
                    4, 5, 6,
                    4, 6, 7,
                    4, 7, 5,
                }
            };

            var aChild = new SceneObjectContainer()
            {
                Mesh = aMesh,
                Transform = float4x4.CreateTranslation(0.11f, 0.11f, 0)
            };

            var parent = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    Version = 1,
                    Generator = "Fusee.SceneViewer",
                    CreatedBy = "FuseeProjetTeam"
                },
                Children = new List<SceneObjectContainer>(new SceneObjectContainer[]
                {
                    aChild,
                    aChild,
                    new SceneObjectContainer()
                    {
                        Mesh = aMesh,
                        Transform = float4x4.CreateTranslation(0.22f, 0.22f, 0)
                    },
                }),
            };
            var ser = new Serializer();
            using (var file = File.Create(@"Assets/Test.fus"))
            {
                ser.Serialize(file, parent);
            }
        }

        private void TestDeserialize()
        {
            var ser = new Serializer();
            SceneContainer mc2;
            using (var file = File.OpenRead(@"Assets/Test.fus"))
            {
                mc2 = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            /**/            
            Diagnostics.Log(mc2.ToString());
        }

        public static void Main()
        {
            var app = new SceneViewer();
            app.Run();
        }
    }
}
