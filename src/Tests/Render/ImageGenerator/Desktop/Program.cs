using System;
using System.IO;
using System.Runtime.InteropServices;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using FileMode = Fusee.Base.Common.FileMode;
using Path = Fusee.Base.Common.Path;
using System.Reflection;
using CommandLine;

namespace Fusee.Engine.Examples.ImageGenerator.Desktop
{
    enum ErrorCode : int
    {
        Success,
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
        PlatformNotHandled = -5,

        InternalError = -42,
    }

    [Verb("shoot", HelpText = "Generate a single image and store it to file")]
    public class ShootOptions
    {
        [Value(0,
            HelpText =
                "Path to image file to generate",
            MetaName = "Output", Required = true)]
        public string Output { get; set; }

        [Option('f', "format",
            HelpText =
                "Output file format overriding the file extension (if any). For example 'png' or 'jpg'."
        )]
        public string Format { get; set; }

        [Option('w', "width", Default = 512, HelpText = "Width of the image to be generated in pixels. Default is 512")]
        public int Width { get; set; }

        [Option('h', "height", Default = 512, HelpText = "Height of the image to be generated in pixels. Default is 512.")]
        public int Height { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<ShootOptions>(args)
            
            // Called with the SHOOT verb
            .WithParsed<ShootOptions>(opts =>
            {
                // Inject Fusee.Engine.Base InjectMe dependencies
                IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

                var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(Font),
                        Decoder = delegate (string id, object storage)
                        {
                            if (!Path.GetExtension(id).ToLower().Contains("ttf")) return null;
                            return new Font { _fontImp = new FontImp((Stream)storage) };
                        },
                        Checker = id => Path.GetExtension(id).ToLower().Contains("ttf")
                    });
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(SceneContainer),
                        Decoder = delegate (string id, object storage)
                        {
                            if (!Path.GetExtension(id).ToLower().Contains("fus")) return null;
                            return new ConvertSceneGraph().Convert(ProtoBuf.Serializer.Deserialize<SceneContainer>((Stream)storage));
                        },
                        Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                    });

                AssetStorage.RegisterProvider(fap);

                var app = new Generator();

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                var cimp = new RenderCanvasImpIG(opts.Width, opts.Height);
                cimp.EnableBlending = true;
                app.CanvasImplementor = cimp;
                app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(cimp);
                // Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
                // Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
                // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
                // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Desktop.AudioImp();
                // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Desktop.NetworkImp();
                // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
                // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

                /*
                var mode = new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 0, ColorFormat.Empty, 1);
                var win = new OpenTK.GameWindow(640, 480, mode, "", OpenTK.GameWindowFlags.Default, OpenTK.DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default);
                */

                //app.Run();

                // Initialize canvas/app and canvas implementor
                app.DoInit();
                cimp.DoInit();

                // Render a single frame and save it
                cimp.DoResize(opts.Width, opts.Height);
                cimp.DoRender();
                var bmp = cimp.ShootCurrentFrame();
                bmp.Save(opts.Output, System.Drawing.Imaging.ImageFormat.Png);

                // Done
                Console.Error.WriteLine($"SUCCESS: Image {opts.Output} generated.");
                Environment.Exit((int)ErrorCode.Success);
            })
            // ERROR on the command line
            .WithNotParsed(errs =>
            {
                /*foreach (var error in errs)
                {
                    Console.Error.WriteLine(error);
                }
                */
                Environment.Exit((int)ErrorCode.CommandLineSyntax);
            });
            // Do not Start the app
            // Don't call 
            //app.Run();
        }
    }
}
