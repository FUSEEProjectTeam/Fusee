using System;
using System.Collections;
using System.Collections.Generic;
using Fusee.Base.Common;

namespace Fusee.Base.Core
{
    internal class ScanLineEnumerator : IEnumerator<ScanLine>
    {
        private readonly byte[] _pixelData;
        private readonly ImagePixelFormat _pixelFormat;
        private readonly int _xSrc;
        private readonly int _ySrc;
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxWidth;

        private int _currentPosition;
        public ScanLineEnumerator(byte[] pixelData, ImagePixelFormat pixelFormat, int xSrc, int ySrc, int width, int height, int maxWidth)
        {
            _pixelData = pixelData;
            _pixelFormat = pixelFormat;
            _xSrc = xSrc;
            _ySrc = ySrc;
            _width = width;
            _height = height;
            _maxWidth = maxWidth;

            _currentPosition = _ySrc - 1; // decreased by one for initial call of MoveNext
        }

        public bool MoveNext()
        {
            int totalHeight = _ySrc + _height;
            if(_currentPosition < totalHeight)
            {
                _currentPosition++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _currentPosition = _ySrc - 1; // decreased by one for initial call of MoveNext
        }

        public ScanLine Current
        {
            get { return GetCurrentScanLine(); }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
        private ScanLine GetCurrentScanLine()
        {
            int bytesPerLine = _width * _pixelFormat.BytesPerPixel;
            byte[] lineByteBuffer = new byte[bytesPerLine];

            // 1D offsetCoordinate that represents location in PixelData byte array (sizeof PixelData is Width*Height*BytesPerPixel)
            int srcOffset = ((_pixelFormat.BytesPerPixel * _maxWidth) * _currentPosition) + _xSrc * _pixelFormat.BytesPerPixel; // go down vertical along i by width times BytesPerPixel and then add horizontal width offset times BytesPerPixel
            Buffer.BlockCopy(_pixelData, srcOffset, lineByteBuffer, 0, bytesPerLine); // copy amount of bytesPerLine from PixelData to lineByteBuffer == grab the ScanLine
            ScanLine scanLine = new ScanLine(lineByteBuffer, 0, _width, _pixelFormat);
            return scanLine;
        }
    }
}