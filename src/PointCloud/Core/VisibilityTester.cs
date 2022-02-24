using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Class that manages the on demand loading of point cloud points.
    /// </summary>
    public class VisibilityTester
    {
        /// <summary>
        ///All nodes that are visible in this frame.
        /// </summary>
        public List<string> VisibleNodes { get; private set; }

        /// <summary>
        /// Current Field of View - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float Fov { get; set; }

        /// <summary>
        /// Current camera position - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float3 CamPos { get; set; }

        /// <summary>
        /// Current height of the viewport - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public int ViewportHeight { get; set; }

        /// <summary>
        /// Current camera frustum - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public FrustumF RenderFrustum { get; set; }

        /// <summary>
        /// The octree structure of the point cloud.
        /// </summary>
        public IPointCloudOctree Octree
        {
            get;
            set;
        }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints { get; private set; }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => _minProjSizeModifier;
            set
            {
                _minProjSizeModifier = value;
                if (((PointCloudOctree)Octree).Root != null)
                    _minScreenProjectedSize = ((PointCloudOctant)Octree.Root).ProjectedScreenSize * _minProjSizeModifier;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - trade off between performance and quality.
        /// </summary>
        public int PointThreshold { get; set; } = 2000000;

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public float UpdateRate { get; set; } = 1 / 30f;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private double _minScreenProjectedSize;

        // Allows traversal in order of screen projected size.
        private readonly SortedDictionary<double, PointCloudOctant> _visibleNodesOrderedByProjectionSize;

        private double3 _camPosD;
        private float _fov;
        private float _minProjSizeModifier = 0.1f;
        private readonly TriggerPointLoading _triggerPointLoading;
        private float _deltaTimeSinceLastUpdate;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="octree">The octree the visibility algorithm is working on.</param>
        /// <param name="tryAddPointsToCacheHandler">Inject a method that loads the points and adds the loaded points in to cache, if needed.</param>
        public VisibilityTester(IPointCloudOctree octree, TriggerPointLoading tryAddPointsToCacheHandler)
        {
            _visibleNodesOrderedByProjectionSize = new SortedDictionary<double, PointCloudOctant>();
            var numberOfNodes = ((int)System.Math.Pow(8, octree.MaxLevel + 1) - 1) / 7 - 1;
            VisibleNodes = new(numberOfNodes / 4);
            Octree = octree;
            _triggerPointLoading = tryAddPointsToCacheHandler;
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        public void Update()
        {
            _camPosD = new double3(CamPos.x, CamPos.y, CamPos.z);
            _fov = Fov;

            SetMinScreenProjectedSize(_camPosD, _fov);

            if (_deltaTimeSinceLastUpdate < UpdateRate)
                _deltaTimeSinceLastUpdate += Time.DeltaTime;
            else
            {
                _deltaTimeSinceLastUpdate = 0;
                //Traverses ordered by projected size.
                DetermineVisibility();
            }
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// Recursive traversal stops if: the screen-projected size is too small, a certain "global" point threshold is reached.
        /// </summary>
        private void DetermineVisibility()
        {
            NumberOfVisiblePoints = 0;
            _visibleNodesOrderedByProjectionSize.Clear();
            VisibleNodes.Clear();

            DetermineVisibilityForNode((PointCloudOctant)Octree.Root);

            while (_visibleNodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _visibleNodesOrderedByProjectionSize.Last();
                var octant = kvp.Value;

                _triggerPointLoading(octant.Guid);

                NumberOfVisiblePoints += octant.NumberOfPointsInNode;
                VisibleNodes.Add(octant.Guid);
                _visibleNodesOrderedByProjectionSize.Remove(kvp.Key);
                DetermineVisibilityForChildren(kvp.Value);
            }
        }

        private void DetermineVisibilityForChildren(PointCloudOctant node)
        {
            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                DetermineVisibilityForNode((PointCloudOctant)child);
            }
        }

        private void DetermineVisibilityForNode(PointCloudOctant node)
        {
            // gets pixel radius of the node
            node.ComputeScreenProjectedSize(_camPosD, ViewportHeight, _fov);

            //If node does not intersect the viewing frustum or is smaller than the minimal projected size:
            //Return -> will not be added to _visibleNodesOrderedByProjectionSize -> traversal of this branch stops.
            if (!node.InsideOrIntersectingFrustum(RenderFrustum) || node.ProjectedScreenSize < _minScreenProjectedSize)
            {
                return;
            }

            // Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            _visibleNodesOrderedByProjectionSize.TryAdd(node.ProjectedScreenSize, node);
        }

        private void SetMinScreenProjectedSize(double3 camPos, float fov)
        {
            ((PointCloudOctant)Octree.Root).ComputeScreenProjectedSize(camPos, ViewportHeight, fov);
            _minScreenProjectedSize = ((PointCloudOctant)Octree.Root).ProjectedScreenSize * _minProjSizeModifier;
        }
    }
}