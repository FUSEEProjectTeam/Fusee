using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Potree.V2.Data;

namespace Fusee.PointCloud.Potree.V2
{
    internal class Potree2Helpers
    {
        internal static PotreeNode FindNode(ref PotreeHierarchy potreeHierarchy, OctantId id)
        {
            return potreeHierarchy.Nodes.Find(n => n.Name == OctantId.OctantIdToPotreeName(id));
        }

        internal static void MapChildNodesRecursive(IPointCloudOctant octreeNode, PotreeNode potreeNode)
        {
            octreeNode.NumberOfPointsInNode = (int)potreeNode.NumPoints;

            for (int i = 0; i < potreeNode.Children.Length; i++)
            {
                if (potreeNode.Children[i] != null)
                {
                    var octant = new PointCloudOctant(potreeNode.Children[i].Aabb.Center, potreeNode.Children[i].Aabb.Size.y, new OctantId(potreeNode.Name));

                    if (potreeNode.Children[i].NodeType == NodeType.LEAF)
                    {
                        octant.IsLeaf = true;
                    }

                    MapChildNodesRecursive(octant, potreeNode.Children[i]);

                    octreeNode.Children[i] = octant;
                }
            }
        }
    }
}