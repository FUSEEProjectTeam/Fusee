using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    internal class TextureManager : IDisposable
    {
        private readonly IRenderContextImp _renderContextImp;

        private readonly Stack<ITextureHandle> _toBeDeletedTextureHandles = new();

        private readonly Dictionary<Suid, Tuple<ITextureHandle, ITextureBase>> _identifierToTextureHandleDictionary = new();

        private void Remove(ITextureHandle textureHandle)
        {
            _renderContextImp.RemoveTextureHandle(textureHandle);
        }

        private void TextureChanged(object sender, TextureEventArgs textureDataEventArgs)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(textureDataEventArgs.Texture.SessionUniqueIdentifier,
                out Tuple<ITextureHandle, ITextureBase> toBeUpdatedTextureTuple))
            {
                throw new KeyNotFoundException("Texture is not registered.");
            }

            ITextureBase texture = textureDataEventArgs.Texture;

            switch (textureDataEventArgs.ChangedEnum)
            {
                case TextureChangedEnum.Disposed:
                    // Add the TextureHandle to the toBeDeleted Stack...
                    _toBeDeletedTextureHandles.Push(toBeUpdatedTextureTuple.Item1);
                    // remove the TextureHandle from the dictionary, the TextureHandle data now only resides inside the gpu and will be cleaned up on bottom of Render(Mesh mesh)
                    _identifierToTextureHandleDictionary.Remove(texture.SessionUniqueIdentifier);
                    // add the identifier to the reusable identifiers stack
                    //_reusableIdentifiers.Push(textureDataEventArgs.Texture.Identifier);
                    break;
                case TextureChangedEnum.RegionChanged:
                    //TODO: An IWritableTexture has no implementation of UpdateTextureRegion (yet)
                    if (texture is ITexture iTexture)
                    {
                        _renderContextImp.UpdateTextureRegion(toBeUpdatedTextureTuple.Item1, iTexture,
                            textureDataEventArgs.XStart, textureDataEventArgs.YStart, textureDataEventArgs.Width,
                            textureDataEventArgs.Height);
                    }
                    break;
                case TextureChangedEnum.FilterModeChanged:
                    _renderContextImp.SetTextureFilterMode(toBeUpdatedTextureTuple.Item1, ((ITextureBase)sender).FilterMode);
                    break;
                case TextureChangedEnum.WrapModeChanged:
                    _renderContextImp.SetTextureWrapMode(toBeUpdatedTextureTuple.Item1, ((ITextureBase)sender).WrapMode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid argument: {textureDataEventArgs.ChangedEnum}");
            }
        }

        private ITextureHandle RegisterNewTexture(WritableCubeMap texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableArrayTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(Texture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        /// <summary>
        /// Creates a new Instance of TextureManager. Th instance is handling the memory allocation and deallocation on the GPU by observing Texture.cs objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation.</param>
        public TextureManager(IRenderContextImp renderContextImp)
        {
            _renderContextImp = renderContextImp;
        }

        public ITextureHandle GetTextureHandle(Texture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableCubeMap texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableArrayTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureItem))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureItem.Item1;
        }

        /// <summary>
        /// Call this method on the main thread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            if (_toBeDeletedTextureHandles == null || _toBeDeletedTextureHandles.Count == 0)
            {
                return;
            }
            while (_toBeDeletedTextureHandles.Count > 0)
            {
                ITextureHandle tobeDeletedTextureHandle = _toBeDeletedTextureHandles.Pop();
                Remove(tobeDeletedTextureHandle);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                for (int i = 0; i < _identifierToTextureHandleDictionary.Count; i++)
                {
                    var texItem = _identifierToTextureHandleDictionary.ElementAt(i);
                    Remove(texItem.Value.Item1);
                    texItem.Value.Item2.Dispose();
                    _identifierToTextureHandleDictionary.Remove(texItem.Key);
                }

                Cleanup();

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~TextureManager()
        {
            Dispose(disposing: false);
        }

    }
}