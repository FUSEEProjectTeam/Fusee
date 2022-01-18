using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.PointCloud.PotreeReader.V2.Data
{
    public class PotreeNode
    {
        public PotreeNode()
        {
        }

        public string Name = "";
        public AABBd Aabb;
        public PotreeNode Parent;
        public PotreeNode[] children = new PotreeNode[8];
        public NodeType NodeType = NodeType.UNSET;
        public long ByteOffset;
        public long ByteSize;
        public long NumPoints;

        public bool IsLoaded = false;

        public int Level()
        {
            return Name.Length - 1;
        }

        public void Traverse(Action<PotreeNode> callback)
        {
            callback(this);

            foreach (var child in children)
            {
                if (child != null)
                {
                    child.Traverse(callback);
                }
            }
        }
    }

    public enum NodeType : int
    {
        NORMAL = 0,
        LEAF = 1,
        PROXY = 2,
        UNSET = -1
    }
}
