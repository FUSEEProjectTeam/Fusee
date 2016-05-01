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

        public List<float2> PointCoordinates(string text)
        {
            List<float2> cp = new List<float2>();
            foreach (char c in text)
            {
                uint i = (uint)c;

                var gp = _font.GetGlyphPoints(i);
                if (gp.PointCoords == null) continue;

                foreach (float2 t in gp.PointCoords)
                {
                    Debug.WriteLine(c + " " + t);
                    cp.Add(t);
                }
            }
            return cp;
        }

        public List<Dictionary<float2, int[]>> ControlPoints(string text)
        {
            var stringControlPoints = new List<Dictionary<float2, int[]>>();
            foreach (char c in text)
            {
                var cp = new Dictionary<float2, int[]>();
                var i = (uint)c;

                var gp = _font.GetGlyphPoints(i);

                if (gp.PointCoords == null) continue;
                for (int j = 0; j < gp.PointCoords.Count; j++)
                {
                    cp.Add(gp.PointCoords[j], gp.PointFlags[j]);
                }
                foreach (var obj in cp)
                {
                    Debug.WriteLine(c + " " + obj.Key);
                    foreach (var ar in obj.Value)
                    {
                        Debug.WriteLine(ar);
                    }
                }
            }
            return stringControlPoints;
        }
    }
}