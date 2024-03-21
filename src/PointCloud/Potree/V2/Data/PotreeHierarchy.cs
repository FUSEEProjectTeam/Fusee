using System.Collections.Generic;

#pragma warning disable CS1591

namespace Fusee.PointCloud.Potree.V2.Data
{
    public class PotreeHierarchy
    {
        public PotreeNode? Root = null;
        public List<PotreeNode>? Nodes;
    }
}

#pragma warning restore CS1591