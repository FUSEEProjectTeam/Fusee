using System;

namespace Fusee.Engine.Core
{
    public class TextureDataEventArgs : EventArgs
    {
        private readonly Texture _texture;
        private readonly TextureChangedEnum _textureChangedEnum;

        public Texture Texture
        {
            get { return _texture; }
        }

        public TextureChangedEnum ChangedEnum
        {
            get { return _textureChangedEnum; }
        }

        private int _xStart, _yStart, _width, _height;

        public int XStart
        {
            get { return _xStart; }
        }

        public int YStart
        {
            get { return _yStart; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

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