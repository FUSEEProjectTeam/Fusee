using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Engine.SimpleScene;
using Fusee.SceneManagement;
using Fusee.Math;
using Fusee.Serialization;

namespace Examples.MasterFuseeTest
{
    public class MasterFuseeTest : RenderCanvas
    {
        public static SceneObjectContainer FindByName(string name, IEnumerable<SceneObjectContainer> list)
        {
            foreach (SceneObjectContainer soc in list)
            {
                if (name == soc.Name)
                    return soc;

                SceneObjectContainer found = FindByName(name, soc.Children);
                if (found != null)
                    return found;
            }
            return null;
        }

        private ShaderProgram _myShader;
        private SceneRenderer _sceneRenderer;
        private SceneObjectContainer _sceneObject;
        private float _yAngle;
        // is called on startup
        public override void Init()
        {
            // _myMesh = new Cube();
            SceneContainer scene;
            var ser = new Serializer();
            using (var file = File.OpenRead(@"Assets/RGB.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }

            _sceneRenderer = new SceneRenderer(scene, "Assets");

            // Nach HierWasAendern suchen
            _sceneObject = FindByName("HierWasAendern", scene.Children);
 

            RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);
            _yAngle = 0;
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.ModelView = float4x4.LookAt(-4, 2, -8, 0, 0, 0, 0, 1, 0) * float4x4.CreateRotationY(_yAngle);

            if (_sceneObject != null)
            {
                _sceneObject.Transform.Rotation.x = _yAngle;
            }
            
            _yAngle += 1.0f * (float) Time.Instance.DeltaTime;
            _sceneRenderer.Render(RC);        
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new MasterFuseeTest();
            app.Run();
        }
    }
}
