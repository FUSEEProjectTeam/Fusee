using CommunityToolkit.Diagnostics;
using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    internal class TextureManager
    {
        private readonly IRenderContextImp _renderContextImp;

        private readonly Stack<ITextureHandle> _toBeDeletedTextureHandles = new();

        private readonly Dictionary<Guid, Tuple<ITextureHandle, ITextureBase>> _identifierToTextureHandleDictionary = new();

        private void Remove(ITextureHandle textureHandle)
        {
            _renderContextImp.RemoveTextureHandle(textureHandle);
        }

        private void TextureChanged(object? sender, TextureEventArgs textureDataEventArgs)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(textureDataEventArgs.Texture.UniqueIdentifier,
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
                    _identifierToTextureHandleDictionary.Remove(texture.UniqueIdentifier);
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
                    Guard.IsNotNull(sender);
                    _renderContextImp.SetTextureFilterMode(toBeUpdatedTextureTuple.Item1, ((ITextureBase)sender).FilterMode);
                    break;
                case TextureChangedEnum.WrapModeChanged:
                    Guard.IsNotNull(sender);
                    _renderContextImp.SetTextureWrapMode(toBeUpdatedTextureTuple.Item1, ((ITextureBase)sender).WrapMode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid argument: {textureDataEventArgs.ChangedEnum}");
            }
        }

        private ITextureHandle RegisterNewTexture(ExposedTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            // Generate the multi-sample texture, as well as the result texture where the final image is being blit to
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);
            texture.TextureHandle = textureHandle;

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;
            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(texture.TextureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableMultisampleTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            // Generate the multi-sample texture, as well as the result texture where the final image is being blit to
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);
            texture.InternalTextureHandle = textureHandle;

            ITextureHandle internalTexHandle = _renderContextImp.CreateTexture(texture.InternalResultTexture);
            texture.InternalResultTexture.TextureHandle = internalTexHandle;

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;
            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(texture.InternalTextureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableCubeMap texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);
            texture.TextureHandle = textureHandle;

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableArrayTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);
            texture.TextureHandle = textureHandle;

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(WritableTexture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);
            texture.TextureHandle = textureHandle;

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(Texture1D texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

            return textureHandle;
        }

        private ITextureHandle RegisterNewTexture(Texture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.UniqueIdentifier, new Tuple<ITextureHandle, ITextureBase>(textureHandle, texture));

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
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(ExposedTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableMultisampleTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableCubeMap texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableArrayTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureTouple))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureTouple.Item1;
        }

        public ITextureHandle GetTextureHandle(WritableTexture texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureItem))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureItem.Item1;
        }

        public ITextureHandle GetTextureHandle(Texture1D texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.UniqueIdentifier, out var foundTextureItem))
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
    }
}