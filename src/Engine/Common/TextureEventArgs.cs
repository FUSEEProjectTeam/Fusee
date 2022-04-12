using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// This enum describes if a Texture has been disposed or if the texture's region changed. Used inside the TextureManager./>.
    /// </summary>
    public enum TextureChangedEnum
    {
        /// <summary>
        /// The texture has been disposed.
        /// </summary>
        Disposed = 0,

        /// <summary>
        /// The texture's region has changed
        /// </summary>
        RegionChanged = 1,

        /// <summary>
        /// The texture's <see cref="TextureWrapMode"/> has changed.
        /// </summary>
        WrapModeChanged = 2,

        /// <summary>
        /// The texture's <see cref="TextureFilterMode"/> has changed.
        /// </summary>
        FilterModeChanged = 3,
    }

    /// <summary>
    /// EventArgs to propagate changes of a <see cref="ITextureBase"/> object's life cycle and property changes.
    /// Used inside the TextureManager./>.
    /// </summary>
    public class TextureEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="Texture"/> that triggered the event.
        /// </summary>
        public ITextureBase Texture { get; }

        /// <summary>
        /// Description enum providing details about what property of the Texture changed.
        /// </summary>
        public TextureChangedEnum ChangedEnum { get; }

        /// <summary>
        /// x offset -> where does the changed region start along x (from left to right)?
        /// </summary>
        public int XStart { get; }

        /// <summary>
        /// y offset -> where does the changed region start along y (from top to bottom)?
        /// </summary>
        public int YStart { get; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Constructor takes a Texture and a description which property of the Texture changed.
        /// </summary>
        /// <param name="texture">The Texture which property of life cycle has changed.</param>
        /// <param name="textureChangedEnum">The <see cref="TextureChangedEnum"/> describing which property of the Texture changed.</param>
        /// <param name="xStart">(optional) x offset -> where does the changed region start along x (from left to right)?</param>
        /// <param name="yStart">(optional) y offset -> where does the changed region start along y (from top to bottom)?</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public TextureEventArgs(ITextureBase texture, TextureChangedEnum textureChangedEnum, int xStart = 0, int yStart = 0, int width = 0, int height = 0)
        {
            Texture = texture;
            ChangedEnum = textureChangedEnum;
            XStart = xStart;
            YStart = yStart;
            Width = width;
            Height = height;

        }
    }
}