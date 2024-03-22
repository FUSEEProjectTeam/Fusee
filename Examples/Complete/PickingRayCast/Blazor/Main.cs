using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Blazor;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Blazor;
using Fusee.Serialization;
using Microsoft.JSInterop;
using ProtoBuf;
using Path = System.IO.Path;
using Stream = System.IO.Stream;

namespace Fusee.Examples.PickingRayCast.Blazor
{
    public class Main : BlazorBase
    {
        private RenderCanvasImp _canvasImp;
        private Core.PickingRayCast _app;

        public override void Run()
        {
            Console.WriteLine("Starting Blazor program");

            // Disable colored console ouput, not supported
            Diagnostics.UseConsoleColor(false);
            Diagnostics.SetMinDebugOutputLoggingSeverityLevel(Diagnostics.SeverityLevel.Verbose);

            base.Run();

            // Inject Fusee.Engine.Base InjectMe dependencies
            Base.Core.IO.IOImp = new Fusee.Base.Imp.Blazor.IO();

            #region FAP

            var fap = new Fusee.Base.Imp.Blazor.AssetProvider(Runtime);
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Base.Core.Font),
                    Decoder = (_, __) => throw new System.NotImplementedException("Non-async decoder isn't supported in Blazor builds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var font = new Base.Core.Font
                            {
                                _fontImp = new FontImp((Stream)storage)
                            };

                            return await Task.FromResult(font);
                        }

                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase);
                    }
                });

            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    Decoder = (_, __) => throw new System.NotImplementedException("Non-async decoder isn't supported in Blazor builds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).IndexOf("fus", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return await FusSceneConverter.ConvertFromAsync(Serializer.Deserialize<FusFile>((System.IO.Stream)storage));
                        }
                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase);
                    }
                });
            AssetStorage.RegisterProvider(fap);

            #endregion

            _app = new Core.PickingRayCast();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            _canvasImp = new RenderCanvasImp(canvas, Runtime, gl, canvasWidth, canvasHeight);
            _app.CanvasImplementor = _canvasImp;
            _app.ContextImplementor = new RenderContextImp(_app.CanvasImplementor);
            Input.AddDriverImp(new RenderCanvasInputDriverImp(_app.CanvasImplementor, Runtime));

            _app.LoadingCompleted += (s, e) =>
            {
                Console.WriteLine("Loading finished");
                ((IJSInProcessRuntime)Runtime).InvokeVoid("LoadingFinished");
            };

            _app.InitApp();

            // Start the app
            _app.Run();
        }

        public override void Update(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTimeUpdate = (float)(elapsedMilliseconds / 1000.0);

            _canvasImp?.DoUpdate();
        }

        public override void Draw(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTime = (float)(elapsedMilliseconds / 1000.0);

            _canvasImp?.DoRender();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            _canvasImp?.DoResize(width, height);
        }
    }
}