using System.Windows.Forms;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.WindowSizesDemo
{
    public class WindowSizesDemo : RenderCanvas
    {
        // Vars.
        private GUIHandler _guiHandler;
        private Mesh _cube;
        private ShaderProgram _spcolor;
        private IShaderParam _colorParam;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);

            // IMPORTANT: You are supposed to use either one SetWindowSize() or VideoWall(). You can't use both.
            // It's possible to resize a window to fit on a video wall or display.
            // Here are some different variants how to use this functionality. Uncomment to test.
            //SetWindowSize(800, 600);
            SetWindowSize(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height/9 * 2, true, 0, 0);
            //SetWindowSize(Screen.PrimaryScreen.Bounds.Width/9 * 2, Screen.PrimaryScreen.Bounds.Height, true, 0, 0);
            //SetWindowSize(640, 480, false, Screen.PrimaryScreen.Bounds.Width/2 - 640/2, Screen.PrimaryScreen.Bounds.Height/2 - 480/2);
            // IMPORTANT: For video Walls or projector matrices it's possible to let the system do the magic by
            // giving it the number of monitors/projectors per axis and specify some border information.
            // This only works in pseudo-fullscreen mode a.k.a borderless fullscreen-window.
            //VideoWall(0, 0, true, false);

            // This is the old method. More than this could not be done directly in a project code(?)
            //Width = 640;
            //Height = 480;

            // Initialize an ui.
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            // Create a new element.
            GUIElement element = new GUIImage("Assets/image.png", 0, 0, -10, 320, 130);

            // Add the element to the gui.
            _guiHandler.Add(element);

            // Add something to render.
            _cube = new Cube();
            _spcolor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spcolor.GetShaderParam("color");
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            
            // First render 3d elements
            RC.SetShader(MoreShaders.GetDiffuseColorShader(RC));
            RC.SetShaderParam(_colorParam, new float4(0.8f, 0.5f, 0, 1));
            RC.Render(_cube);

            // Projection stuff.
            var mtxCam = float4x4.LookAt(6, 3, 3, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam;

            // Pull user input.
            if (Input.Instance.IsKey(KeyCodes.Escape))
                CloseGameWindow(); // Call to opentk to close the application. TODO: Figure out how to prevent problems with existing projects. Perhaps a boolean to overwrite or so?

            // Second render gui
            _guiHandler.RenderGUI();

            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4/9*2, aspectRatio, 1, 10000);

            _guiHandler.Refresh();
        }

        public static void Main()
        {
            var app = new WindowSizesDemo();
            app.Run();
        }
    }
}
