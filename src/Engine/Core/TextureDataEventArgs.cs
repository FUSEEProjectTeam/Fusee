using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// EventArgs to propagate changes of a <see cref="Texture"/> object's life cycle and property changes.
    /// Used inside <see cref="TextureManager"/>.
    /// </summary>
    public class TextureDataEventArgs : EventArgs
    {
        private readonly Texture _texture;
        private readonly TextureChangedEnum _textureChangedEnum;

        /// <summary>
        /// The <see cref="Texture"/> that triggered the event.
        /// </summary>
        public Texture Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Description enum providing details about what property of the Texture changed.
        /// </summary>
        public TextureChangedEnum ChangedEnum
        {
            get { return _textureChangedEnum; }
        }

        private int _xStart, _yStart, _width, _height;

        /// <summary>
        /// x offset -> where does the changed region start along x (from left to right)?
        /// </summary>
        public int XStart
        {
            get { return _xStart; }
        }

        /// <summary>
        /// y offset -> where does the changed region start along y (from top to bottom)?
        /// </summary>
        public int YStart
        {
            get { return _yStart; }
        }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Constructor takes a Texture and a description which property of the Texture changed.
        /// </summary>
        /// <param name="texture">The Texture which property of life cycle has changed.</param>
        /// <param name="textureChangedEnum">The <see cref="TextureChangedEnum"/> describing which property of the Texture changed.</param>
        /// <param name="xStart">(optional) x offset -> where does the changed region start along x (from left to right)?</param>
        /// <param name="yStart">(optional) y offset -> where does the changed region start along y (from top to bottom)?</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public TextureDataEventArgs(Texture texture, TextureChangedEnum textureChangedEnum, int xStart = 0, int yStart = 0, int width = 0, int height = 0)
        {
            _texture = texture;
            _textureChangedEnum = textureChangedEnum;
            _xStart = xStart;
            _yStart = yStart;
            _width = width;
            _height = height;

        }
    }
}