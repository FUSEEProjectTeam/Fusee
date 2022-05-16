using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core
{
    internal class WritableMultisampleTexture : IWritableTexture, IDisposable
    {
        private bool disposedValue;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public ImagePixelFormat PixelFormat { get; private set; }

        public Multisample Multisample { get; private set; }

        public RenderTargetTextureTypes TextureType { get; private set; }

        public TextureCompareMode CompareMode { get; private set; }

        public Compare CompareFunc { get; private set; }

        public ITextureHandle TextureHandle { get; internal set; }

        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        public bool DoGenerateMipMaps => false;

        public TextureWrapMode WrapMode { get; private set; }

        public TextureFilterMode FilterMode { get; private set; }

        public event EventHandler<TextureEventArgs> TextureChanged;

        /// <summary>
        /// Fire dispose texture event
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TextureChanged?.Invoke(this, new TextureEventArgs(this, TextureChangedEnum.Disposed));
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose()"/> in order to fire TextureChanged event.
        /// </summary>
        ~WritableMultisampleTexture()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Fire dispose texture event
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
