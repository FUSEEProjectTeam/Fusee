using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.Desktop;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop
{

    public class ImGuiInputImp : IInputDriverImp
    {
        private readonly GameWindow _gameWindow;
        private readonly KeyboardDeviceImp _keyboard;
        private readonly MouseDeviceImp _mouse;
        private readonly GamePadDeviceImp _gamePad0;
        private readonly GamePadDeviceImp _gamePad1;
        private readonly GamePadDeviceImp _gamePad2;
        private readonly GamePadDeviceImp _gamePad3;

        public ImGuiInputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException(nameof(renderCanvas));

            if (renderCanvas is not ImGuiRenderCanvasImp)
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", nameof(renderCanvas));

            _gameWindow = ((ImGuiRenderCanvasImp)renderCanvas)._gameWindow;
            if (_gameWindow == null)
                throw new ArgumentNullException(nameof(_gameWindow));

            _keyboard = new KeyboardDeviceImp(_gameWindow);
            _mouse = new MouseDeviceImp(_gameWindow);
            //_gamePad0 = new GamePadDeviceImp(_gameWindow, 0);
            //_gamePad1 = new GamePadDeviceImp(_gameWindow, 1);
            //_gamePad2 = new GamePadDeviceImp(_gameWindow, 2);
            //_gamePad3 = new GamePadDeviceImp(_gameWindow, 3);
        }

        /// <summary>
        /// Devices supported by this driver: One mouse, one keyboard and up to four gamepads.
        /// </summary>
        public IEnumerable<IInputDeviceImp> Devices
        {
            get
            {
                yield return _keyboard;
                yield return _mouse;
                //yield return _gamePad0;
                //yield return _gamePad1;
                //yield return _gamePad2;
                //yield return _gamePad3;

            }
        }

        /// <summary>
        /// Returns a human readable description of this driver.
        /// </summary>
        public string DriverDesc
        {
            get
            {
                string pf = "ImGui";
                return "ImGui and OpenTk GameWindow Mouse and Keyboard input driver for " + pf;
            }
        }

        /// <summary>
        /// Returns a (hopefully) unique ID for this driver. Uniqueness is granted by using the
        /// full class name (including namespace).
        /// </summary>
        public string DriverId
        {
            get { return GetType()?.FullName ?? "Fusee.DImGui.Desktop"; }
        }

#pragma warning disable 0067

        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<DeviceImpDisconnectedArgs>? DeviceDisconnected;

        /// <summary>
        /// Not supported on this driver. Mouse and keyboard are considered to be connected all the time.
        /// You can register handlers but they will never get called.
        /// </summary>
        public event EventHandler<NewDeviceImpConnectedArgs>? NewDeviceConnected;

#pragma warning restore 0067

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Part of the Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RenderCanvasInputDriverImp() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Part of the dispose pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


}
