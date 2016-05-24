using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class Curve
    {
        private readonly Font _font;

        public Curve(Font font)
        {
            _font = font;
        }

        public IList<float3> PointCoordinates(string text)
        {
            IList<float3> cp = new List<float3>();
            foreach (var c in text)
            {
                uint i = c;

                var gp = _font.GetGlyphPoints(i);
                if (gp.PointCoords == null) continue;

                foreach (float2 t in gp.PointCoords)
                {
                    var point  = new float3(t.x, t.y, 0);
                    Debug.WriteLine(c + " " + t);
                    cp.Add(point);
                }
            }
            return cp;
        }

        public IList<IDictionary<float3, int[]>> ControlPoints(string text)
        {
            IList<IDictionary<float3, int[]>> stringControlPoints = new List<IDictionary<float3, int[]>>();
            foreach (char c in text)
            {
                IDictionary<float3, int[]> cp = new Dictionary<float3, int[]>();
                var i = (uint)c;

                var gp = _font.GetGlyphPoints(i);

                if (gp.PointCoords == null) continue;
                for (var j = 0; j < gp.PointCoords.Count; j++)
                {
                    var point = new float3(gp.PointCoords[j].x, gp.PointCoords[j].y, 0);
                    cp.Add(point, gp.PointFlags[j]);
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