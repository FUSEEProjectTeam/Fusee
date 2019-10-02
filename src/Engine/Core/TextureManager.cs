using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    internal class TextureManager
    {
        private readonly IRenderContextImp _renderContextImp;

        private Stack<ITextureHandle> _toBeDeletedTextureHandles = new Stack<ITextureHandle>();

        private Dictionary<Suid, ITextureHandle> _identifierToTextureHandleDictionary = new Dictionary<Suid, ITextureHandle>();       

        private void Remove(ITextureHandle textureHandle)
        {
            _renderContextImp.RemoveTextureHandle(textureHandle);
        }

        private void TextureChanged(object sender, TextureDataEventArgs textureDataEventArgs)
        {
            ITextureHandle toBeUpdatedTextureHandle;
            if (!_identifierToTextureHandleDictionary.TryGetValue(textureDataEventArgs.Texture.SessionUniqueIdentifier,
                out toBeUpdatedTextureHandle))
            {
                throw new KeyNotFoundException("Texture is not registered.");
            }

            Texture texture = textureDataEventArgs.Texture;

            switch (textureDataEventArgs.ChangedEnum)
            {
                case TextureChangedEnum.Disposed:
                    // Add the TextureHandle to the toBeDeleted Stack...
                    _toBeDeletedTextureHandles.Push(toBeUpdatedTextureHandle);
                    // remove the TextureHandle from the dictionary, the TextureHandle data now only resides inside the gpu and will be cleaned up on bottom of Render(Mesh mesh)
                    _identifierToTextureHandleDictionary.Remove(texture.SessionUniqueIdentifier);
                    // add the identifier to the reusable identifiers stack
                    //_reusableIdentifiers.Push(textureDataEventArgs.Texture.Identifier);
                    break;
                case TextureChangedEnum.RegionChanged:
                    _renderContextImp.UpdateTextureRegion(toBeUpdatedTextureHandle, texture,
                        textureDataEventArgs.XStart, textureDataEventArgs.YStart, textureDataEventArgs.Width,
                        textureDataEventArgs.Height);
                    break;
            }
        }

        private ITextureHandle RegisterNewWritableTexture(WritableCubeMap texture)
        {
            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, texture.TextureHandle);

            return texture.TextureHandle;

        }

        private ITextureHandle RegisterNewWritableTexture(WritableTexture texture)
        {
            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, texture.TextureHandle);

            return texture.TextureHandle;

        }

        private ITextureHandle RegisterNewTexture(Texture texture)
        {
            // Configure newly created TextureHandle to reflect Texture's properties on GPU (allocate buffers)
            ITextureHandle textureHandle = _renderContextImp.CreateTexture(texture);

            // Setup handler to observe changes of the texture data and dispose event (deallocation)
            texture.TextureChanged += TextureChanged;

            _identifierToTextureHandleDictionary.Add(texture.SessionUniqueIdentifier, textureHandle);

            return textureHandle;
        }

        /// <summary>
        /// Creates a new Instance of TextureManager. Th instance is handling the memory allocation and deallocation on the GPU by observing Texture.cs objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation. See <see cref="RegisterNewTexture"/>.</param>
        public TextureManager(IRenderContextImp renderContextImp)
        {
            _renderContextImp = renderContextImp;
        }

        public ITextureHandle GetTextureHandleFromTexture(Texture texture)
        {
            ITextureHandle foundTextureHandle;
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out foundTextureHandle))
            {
                return RegisterNewTexture(texture);
            }
            return foundTextureHandle;
        }

        public ITextureHandle GetWritableTextureHandleFromTexture(WritableCubeMap texture)
        {
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureHandle))
            {
                return RegisterNewWritableTexture(texture);
            }
            return foundTextureHandle;
        }

        public ITextureHandle GetWritableTextureHandleFromTexture(WritableTexture texture)
        {    
            if (!_identifierToTextureHandleDictionary.TryGetValue(texture.SessionUniqueIdentifier, out var foundTextureHandle))
            {
                return RegisterNewWritableTexture(texture);
            }
            return foundTextureHandle;
        }

        /// <summary>
        /// Call this method on the mainthread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
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