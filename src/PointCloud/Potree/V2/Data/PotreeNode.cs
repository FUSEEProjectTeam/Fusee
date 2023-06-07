using CommunityToolkit.Diagnostics;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Newtonsoft.Json;
using System;

#pragma warning disable CS1591

namespace Fusee.PointCloud.Potree.V2.Data
{
    public class PotreeNode
    {
        public PotreeNode()
        {
        }

        [JsonIgnore]
        private string? _name;

        public string? Name
        {
            get { return _name; }
            set
            {
                Guard.IsNotNull(value);
                _name = value;
                OctantId = new OctantId(value);
            }
        }

        [JsonIgnore]
        public OctantId OctantId;

        public AABBd Aabb { get; set; } = new();
        public PotreeNode? Parent { get; set; }
        public PotreeNode[] Children = new PotreeNode[8];
        public NodeType NodeType = NodeType.UNSET;
        public long ByteOffset { get; set; }
        public long ByteSize { get; set; }
        public long NumPoints { get; set; }

        public bool IsLoaded = false;

        public int? Level()
        {
            return Name?.Length - 1;
        }

        public void Traverse(Action<PotreeNode> callback)
        {
            callback(this);

            foreach (var child in Children)
            {
                child?.Traverse(callback);
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

#pragma warning restore CS1591