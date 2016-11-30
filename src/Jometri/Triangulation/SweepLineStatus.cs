using Fusee.Jometri.DCEL;

namespace Fusee.Jometri.Triangulation
{
    //Internal extension methodes to BinarySearchTree //TODO: Make PreorderTraverseTreeNodes() private and access it via refelction?!
    internal static class SweepLineStatus
    {
        //Updates the nodes (key and value) according to the sweep lines intersection point with a certain status edge
        internal static void UpdateNodes(this BinarySearchTree<float, StatusEdge> tree, Geometry.Vertex eventPoint)
        {
            foreach (var node in tree.PreorderTraverseTreeNodes())
            {
                node.Value.SetKey(eventPoint);
                node.Key = node.Value.IntersectionPointX;
            }
        }

        //TODO: is there a more effective way to search for a node whose value has a certain half edge?
        internal static StatusEdge FindStatusEdgeWithHandle(this BinarySearchTree<float, StatusEdge> tree, HalfEdgeHandle handle)
        {
            foreach (var n in tree.PreorderTraverseTreeValues())
            {
                if (n.HalfEdge == handle)
                    return n;
            }
            return null;
        }
    }
}
