using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Xene;
using Fusee.Base.Core;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctantLoader<TPoint> where TPoint : new()
    {
        public RenderContext RC;
        public Texture VisibleOctreeHierarchyTex;

        public SceneNodeContainer RootNode { get; private set; }
        public Dictionary<Guid, SceneNodeContainer> VisibleNodes = new Dictionary<Guid, SceneNodeContainer>();          // Visible nodes.
        private readonly Dictionary<Guid, IEnumerable<Mesh>> LoadedMeshs;                                               // Visible AND Loaded nodes.

        private readonly SortedDictionary<double, SceneNodeContainer> _nodesOrderedByProjectionSize;                    // For traversal purposes only.
        public Dictionary<Guid, SceneNodeContainer> _determinedAsVisible = new Dictionary<Guid, SceneNodeContainer>();  // All visible nodes in screen projected size order - cleared in every traversal.
        private int _numberOfVisiblePoints;
        private readonly string _fileFolderPath;

        #region Traversal Properties

        private readonly int SceneUpdateTime = 200; // in ms
        private float _deltaTimeSinceLastUpdate;

        public bool IsUserMoving;

        // Maximal number of points that are visible in one frame - tradeoff between performance and quality
        public int PointThreshold = 1000000;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private readonly double _minScreenProjectedSize = 80;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        private readonly int _noOfLoadedNodes = 10;

        #endregion

        public PtOctantLoader(SceneNodeContainer rootNode, string fileFolderPath, RenderContext rc)
        {
            LoadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>(); // visible nodes ordered by screen-projected-size;
            RC = rc;
            RootNode = rootNode;                        
            _fileFolderPath = fileFolderPath;            
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        /// <param name="depthPassEf">Shader effect used in the depth pass in eye dome lighting.</param>
        /// <param name="colorPassEf">Shader effect that is accountable for rendering the color pass.</param>        
        /// <param name="GetMeshsForNode">User-given Function that defines how to create the mesh for a scene node.</param>
        /// <param name="ptAccessor">PointAccessor, needed to load the actual points.</param>       
        public void UpdateScene(PointSizeMode ptSizeMode, ShaderEffect depthPassEf, ShaderEffect colorPassEf, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor)
        {
            Diagnostics.Log(Time.FramePerSecond);

            if (_deltaTimeSinceLastUpdate < SceneUpdateTime /*&& !IsUserMoving*/)
                _deltaTimeSinceLastUpdate += Time.RealDeltaTimeMs * 1000;
            //else if (IsUserMoving)
            //{
            //    _deltaTimeSinceLastUpdate = 200;
            //}
            else
            {
                _deltaTimeSinceLastUpdate = 0;
                //if (IsUserMoving) return;

                TraverseByProjectedSizeOrder();
                TraverseToUpdateScene(RootNode, GetMeshsForNode, ptAccessor);
                
                if (ptSizeMode == PointSizeMode.ADAPTIVE_SIZE)
                {
                    TraverseBreadthFirstToCreate1DTex(RootNode, VisibleOctreeHierarchyTex);
                    depthPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                    colorPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                }
            }
        }

        /// <summary>
        /// Iterates the VisibleNodes list and sets the octant mesh for visible nodes.
        /// </summary>
        /// <param name="scene">The scene that contains the point cloud and the wireframe cubes. Only needed to visualize the octants.</param>
        /// <param name="wfc">A wireframe cube. Only needed to visualize the octants.</param>
        /// <param name="effect">Shader effect for rendering the wireframe cubes.</param>
        public void ShowOctants(SceneContainer scene, WireframeCube wfc, ShaderEffect effect)
        {
            scene.Children.RemoveAll(node => node.Name == "WireframeCube");

            foreach (var node in VisibleNodes.Values)
            {
                var ptOctantComp = node.GetComponent<PtOctantComponent>();

                if (LoadedMeshs.ContainsKey(ptOctantComp.Guid))
                {
                    scene.Children.Add(new SceneNodeContainer()
                    {
                        Name = "WireframeCube",
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Translation = (float3)ptOctantComp.Center,
                                Scale = float3.One * (float)ptOctantComp.Size
                            },
                            new ShaderEffectComponent()
                            {
                                Effect = effect
                            },
                            wfc
                        }
                    });
                }
                else
                {
                    throw new ArgumentException("Trying to set octant for node that is not loaded yet!");
                }
            }
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for nodes in screen-projected-size order.
        /// </summary>        
        private void TraverseByProjectedSizeOrder()
        {
            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            _determinedAsVisible.Clear();
            _nodesOrderedByProjectionSize.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;            

            ProcessNode(RootNode, fov);          

            while (_nodesOrderedByProjectionSize.Count > 0 && _numberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;
                _determinedAsVisible.Add(kvp.Value.GetComponent<PtOctantComponent>().Guid, kvp.Value);
                _nodesOrderedByProjectionSize.Remove(kvp.Key);                
                ProcessChildren(biggestNode, fov);
            }
        }

        private void ProcessChildren(SceneNodeContainer node, float fov)
        {
            var ptOctantComp = node.GetComponent<PtOctantComponent>();

            if (ptOctantComp.IsLeaf) return;

            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                ProcessNode(child, fov);
            }
        }

        private void ProcessNode(SceneNodeContainer node, float fov)
        {
            var ptOctantChildComp = node.GetComponent<PtOctantComponent>();

            //If node does not intersect the viewing frustum, remove it from loaded meshs and return.
            if (!ptOctantChildComp.Intersects(RC.Projection * RC.View)) 
                return;            

            var camPos = RC.View.Invert().Column3;
            var camPosD = new double3(camPos.x, camPos.y, camPos.z);

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

            //If the nodes screen projected size is too small, remove it from loaded meshs and return.
            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)                
                return;
            

            //Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            if (!_nodesOrderedByProjectionSize.ContainsKey(ptOctantChildComp.ProjectedScreenSize))            
                _nodesOrderedByProjectionSize.Add(ptOctantChildComp.ProjectedScreenSize, node);
            
        }

        /// <summary>
        /// Traverse the scene (octree) and remove all meshes so the scene can be filled with the newly visible ones.
        /// Does not check for differences in visibility between frames!
        /// </summary>
        /// <param name="node">The scene node the traversal starts with.</param>
        private void TraverseToUpdateScene(SceneNodeContainer node, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor)
        {
            var ptOctantComp = node.GetComponent<PtOctantComponent>();
            ptOctantComp.VisibleChildIndices = 0;
            if (!_determinedAsVisible.ContainsKey(ptOctantComp.Guid)) //Node isn't visible
            {
                RemoveNode(node, ptOctantComp);
                _numberOfVisiblePoints -= ptOctantComp.NumberOfPointsInNode;
                if (_numberOfVisiblePoints < 0)
                    _numberOfVisiblePoints = 0;
            }
            else
            {
                if (VisibleNodes.ContainsKey(ptOctantComp.Guid)) //is visible and was loaded
                {
                    if (LoadedMeshs.TryGetValue(ptOctantComp.Guid, out var loadedMeshes))
                    {
                        node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                        node.Components.AddRange(loadedMeshes);
                    }
                    else
                    {
                        throw new ArgumentException("Trying to set meshes that are not loaded yet!");
                    }
                }
                else //is visible and needs to be loaded
                {
                    //Load and add to VisibleNodes
                    if (!ptOctantComp.WasLoaded)
                    {
                        var pts = LoadPointsForNode(ptAccessor, ptOctantComp);
                        ptOctantComp.NumberOfPointsInNode = pts.Count;
                        var meshes = GetMeshsForNode(ptAccessor, pts);
                        LoadedMeshs.Add(ptOctantComp.Guid, meshes);

                        _numberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;
                    }
                    VisibleNodes.Add(ptOctantComp.Guid, node);
                }
            }

            foreach (var child in node.Children)
            {
                TraverseToUpdateScene(child, GetMeshsForNode, ptAccessor);
            }
        }

        private void RemoveNode(SceneNodeContainer node, PtOctantComponent ptOctantComponent)
        {
            node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
            
            LoadedMeshs.TryGetValue(ptOctantComponent.Guid, out var meshs);
            if (meshs != null)
            {
                foreach (var mesh in meshs)
                {
                    mesh.Dispose();
                }
            }
            LoadedMeshs.Remove(ptOctantComponent.Guid);
            VisibleNodes.Remove(ptOctantComponent.Guid);
            ptOctantComponent.WasLoaded = false;
            ptOctantComponent.VisibleChildIndices = 0;
        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, PtOctantComponent ptOctantComponent)
        {
            var pathToFile = _fileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using (BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read)))
            {
                // step to stream position
                //br.BaseStream.Position = node.StreamPosition;

                // read number of points
                var numberOfPoints = br.ReadInt32();
                var lengthOfPoint = br.ReadInt32();

                List<TPoint> points = new List<TPoint>(numberOfPoints);

                for (var i = 0; i < numberOfPoints; i++)
                {
                    var pt = new TPoint();
                    var ptBytes = br.ReadBytes(lengthOfPoint);

                    ptAccessor.SetRawPoint(ref pt, ptBytes);

                    points.Add(pt);
                }

                ptOctantComponent.WasLoaded = true;                

                return points;
            }

        }               

        //Traverse breadth first to create 1D texture that contains the visible octree hierarchy.        
        /// <summary>
        /// Traverses in breadth-first order.
        /// </summary>
        private void TraverseBreadthFirstToCreate1DTex(SceneNodeContainer node, Texture tex)
        {
            if (VisibleNodes.Count == 0) return;

            //clear texture
            tex.Blt(0, 0, new ImageData(new byte[tex.PixelData.Length], tex.Width, tex.Height, tex.PixelFormat));

            var visibleOctantsImgData = new ImageData(new byte[VisibleNodes.Count * tex.PixelFormat.BytesPerPixel], VisibleNodes.Count, 1, tex.PixelFormat);

            Queue<SceneNodeContainer> candidates = new Queue<SceneNodeContainer>();
            candidates.Enqueue(node);

            //The nodes' position in the texture
            byte nodePixelPos = 0;
            var rootPtOctantComp = node.GetComponent<PtOctantComponent>();
            rootPtOctantComp.PosInHierarchyTex = 0;

            while (candidates.Count > 0)
            {                
                node = candidates.Dequeue();
                var ptOctantComp = node.GetComponent<PtOctantComponent>();

                //check if octantcomp.guid is in VisibleNode
                //yes --> write to 1D tex
                if (VisibleNodes.ContainsKey(ptOctantComp.Guid))
                {
                    ptOctantComp.PosInHierarchyTex = nodePixelPos;

                    if (node.Parent != null)
                    {
                        var parentPtOctantComp = node.Parent.GetComponent<PtOctantComponent>();

                        //If parentPtOctantComp.VisibleChildIndices == 0 this child is the first visible one.
                        if (parentPtOctantComp.VisibleChildIndices == 0)
                        {
                            //Get the "green byte" (+1) and calculate the offset from the parent to this node (in px)
                            var parentBytePos = (parentPtOctantComp.PosInHierarchyTex * tex.PixelFormat.BytesPerPixel) + 1;
                            visibleOctantsImgData.PixelData[parentBytePos] = (byte)(nodePixelPos - parentPtOctantComp.PosInHierarchyTex);
                        }

                        //add the index of this node to VisibleChildIndices
                        byte indexNumber = (byte)System.Math.Pow(2, ptOctantComp.PosInParent);
                        parentPtOctantComp.VisibleChildIndices += indexNumber;
                        visibleOctantsImgData.PixelData[parentPtOctantComp.PosInHierarchyTex * tex.PixelFormat.BytesPerPixel] = parentPtOctantComp.VisibleChildIndices;
                    }

                    nodePixelPos++;
                }

                //enqueue all children
                foreach (var child in node.Children)
                {
                    candidates.Enqueue(child);
                }                
            }

            //replace PixelData with new contents
            tex.Blt(0, 0, visibleOctantsImgData);
        }

    }
}
