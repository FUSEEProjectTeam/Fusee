using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class Curves
    {
        private Font _font;
        private uint _pixelHeight;

        public Curves(Font font, uint pixelHeight)
        {
            _font = font;
            _pixelHeight = pixelHeight;
        }

        public List<float2> ControlPoints(string text)
        {
            List<float2> cp = new List<float2>();
            foreach (char c in text)
            {
                uint i = (uint) c;

                GlyphPoints gp = _font.GetGlyphPoints(i);
                if (gp.Points != null)
                {
                    foreach (float2 t in gp.Points)
                    {
                        Debug.WriteLine(c + " " + t);
                        cp.Add(t);
                    }
                }
            }
            return cp;
        }
    }
}