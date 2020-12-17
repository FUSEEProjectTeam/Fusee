using Fusee.Math.Core;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.OoCReaderWriter
{
    public class PtOctantWrite<TPoint> : OctantD<TPoint>
    {
        public PtGrid<TPoint> Grid;

        public PtOctantWrite(double3 center, double size, IOctant<double3, double, TPoint>[] children = null)
        {
            Guid = Guid.NewGuid();

            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<double3, double, TPoint>[8];
            else
                Children = children;

            Payload = new List<TPoint>();
        }

        public override IOctant<double3, double, TPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctantWrite<TPoint>(childCenter, childRes)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };

            child.Grid = new PtGrid<TPoint>(child.Center, new double3(child.Size, child.Size, child.Size));

            return child;
        }
    }
}