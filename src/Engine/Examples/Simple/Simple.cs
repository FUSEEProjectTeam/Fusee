using System;
using Fusee.Engine;
using Fusee.Math;
using SlimDX.DirectInput;
using GameControllerState = SlimDX.DirectInput.JoystickState;


namespace Examples.Simple
{
    [FuseeApplication(Name = "Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        private GameController gameController;
        private DirectInput directInput;
       
        


        //private InputDevice _device;

        // is called on startup
        public override void Init()
        {
            //foreach (Device device in Input.Instance.Devices)
            //{
            //    if (device.Category == DeviceCategory.GameController)
            //    {
            //        _device = device;
            //        break;
            //    }
            //}	
           directInput = new DirectInput();
           gameController = new GameController(directInput, 0);
           Input.Instance.GetAllDevices();
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    if (_device.IsButtonDown(i))
            //    {
            //        System.Diagnostics.Debug.Write(i);
            //    }
            //}



            //GameControllerState gameControllerState = gameController.GetState();
            //for (int i = 0; i < 10; i++)
            //{
            //    if (gameControllerState.IsPressed(i) == true)
            //    {
            //        System.Diagnostics.Debug.Write(i);
            //    }
            //}
            //System.Diagnostics.Debug.WriteLine(gameControllerState.VelocityZ);

            Input.Instance.GetPressedButton(directInput, gameController);


        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new Simple();
            app.Run();
        }
    }
}