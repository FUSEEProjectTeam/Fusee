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
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctantLoader<TPoint> where TPoint : new()
    {
        public bool WasSceneUpdated { get; private set; }

        public RenderContext RC;
        public Texture VisibleOctreeHierarchyTex;

        private SceneNodeContainer _rootNode;

        public SceneNodeContainer RootNode
        {
            get
            {
                return _rootNode;
            }

            set
            {
                _loadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();
                _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>(); // visible nodes ordered by screen-projected-size
                _determinedAsVisible = new Dictionary<Guid, SceneNodeContainer>();

                var fov = (float)RC.ViewportWidth / RC.ViewportHeight;
                var camPos = RC.View.Invert().Column3;
                var camPosD = new double3(10, 10, -30);

                _rootNode = value;

                // gets pixel radius of the node
                RootNode.GetComponent<PtOctantComponent>().ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);
                _minScreenProjectedSize = (float)RootNode.GetComponent<PtOctantComponent>().ProjectedScreenSize * 1/2f;
            } 
        }

        public ConcurrentDictionary<Guid, SceneNodeContainer> VisibleNodes                                                                  // Visible AND loaded nodes.
        {
            get;
            private set;

        } = new ConcurrentDictionary<Guid, SceneNodeContainer>();    
        
        private Dictionary<Guid, IEnumerable<Mesh>> _loadedMeshs;                                                                  // Visible AND loaded meshes.

        private SortedDictionary<double, SceneNodeContainer> _nodesOrderedByProjectionSize;                                        // For traversal purposes only.
        private Dictionary<Guid, SceneNodeContainer> _determinedAsVisible = new Dictionary<Guid, SceneNodeContainer>();            // All visible nodes in screen projected size order - cleared in every traversal.
        private readonly Dictionary<Guid, SceneNodeContainer> _determinedAsVisibleAndUnloaded = new Dictionary<Guid, SceneNodeContainer>(); // Visible but unloaded nodes in screen projected size order - cleared in every traversal.

        private readonly WireframeCube wfc = new WireframeCube();
        private readonly ShaderEffect _wfcEffect = ShaderCodeBuilder.MakeShaderEffect(new float4(0, 0, 0, 1), new float4(1, 1, 1, 1), 10);

        public string FileFolderPath;

        #region Traversal Properties
        public int NumberOfVisiblePoints { get; private set; }

        // Maximal number of points that are visible in one frame - tradeoff between performance and quality
        public int PointThreshold = 1000000;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        public float _minScreenProjectedSize;

        //Scene is only updated if the user is moving.
        public bool IsUserMoving;

        private readonly int SceneUpdateTime = 200; // in ms
        private float _deltaTimeSinceLastUpdate;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _noOfLoadedNodes = 5;

        #endregion

        public PtOctantLoader(float3 initialCamPos, string fileFolderPath, RenderContext rc)
        {
            _loadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>(); // visible nodes ordered by screen-projected-size;
            RC = rc;
            //RootNode = rootNode;                        
            FileFolderPath = fileFolderPath;
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        /// <param name="depthPassEf">Shader effect used in the depth pass in eye dome lighting.</param>
        /// <param name="colorPassEf">Shader effect that is accountable for rendering the color pass.</param>        
        /// <param name="GetMeshsForNode">User-given Function that defines how to create the mesh for a scene node.</param>
        /// <param name="ptAccessor">PointAccessor, needed to load the actual point data.</param>       
        public void UpdateScene(PointSizeMode ptSizeMode, ShaderEffect depthPassEf, ShaderEffect colorPassEf, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor)
        {
            WasSceneUpdated = false;

            //Diagnostics.Log(NumberOfVisiblePoints);
            
            if (_deltaTimeSinceLastUpdate < SceneUpdateTime )
                _deltaTimeSinceLastUpdate += Time.RealDeltaTimeMs * 1000;
            
            else
            {
                _deltaTimeSinceLastUpdate = 0;
                if (IsUserMoving) return;

                TraverseByProjectedSizeOrder();
                LoadNodesAsync(GetMeshsForNode, ptAccessor);

                var nodesToRender = _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);                
                TraverseToUpdateScene(nodesToRender, _rootNode);
                
                if (ptSizeMode == PointSizeMode.ADAPTIVE_SIZE)
                {
                    TraverseBreadthFirstToCreate1DTex(_rootNode, VisibleOctreeHierarchyTex);
                    depthPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                    colorPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                }
            }

            WasSceneUpdated = true;
        }

        /// <summary>
        /// Iterates the VisibleNodes list and sets the octant mesh for visible nodes.
        /// </summary>
        /// <param name="scene">The scene that contains the point cloud and the wireframe cubes. Only needed to visualize the octants.</param>
        /// <param name="wfc">A wireframe cube. Only needed to visualize the octants.</param>
        /// <param name="effect">Shader effect for rendering the wireframe cubes.</param>
        public void ShowOctants(SceneContainer scene)
        {
            WasSceneUpdated = false;
            DeleteOctants(scene);
            foreach (var node in VisibleNodes.Values)
            {
                var ptOctantComp = node.GetComponent<PtOctantComponent>();

                if (_loadedMeshs.ContainsKey(ptOctantComp.Guid))
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
                                Effect = _wfcEffect
                            },
                            wfc
                        }
                    });
                }                
            }

            WasSceneUpdated = true;
        }

        public void DeleteOctants(SceneContainer scene)
        {
            scene.Children.RemoveAll(node => node.Name == "WireframeCube");
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// </summary>        
        private void TraverseByProjectedSizeOrder()
        {
            NumberOfVisiblePoints = 0;

            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            _determinedAsVisible.Clear();
            _determinedAsVisibleAndUnloaded.Clear();
            _nodesOrderedByProjectionSize.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;            

            ProcessNode(_rootNode, fov);          

            while (_nodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;
                _determinedAsVisible.Add(kvp.Value.GetComponent<PtOctantComponent>().Guid, kvp.Value);

                var ptOctantComp = kvp.Value.GetComponent<PtOctantComponent>();
                if (!ptOctantComp.WasLoaded)
                {
                    if(ptOctantComp.NumberOfPointsInNode == 0)
                        NumberOfVisiblePoints += GetPtCountFromFile(ptOctantComp);
                    else
                        NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                    _determinedAsVisibleAndUnloaded.Add(ptOctantComp.Guid, kvp.Value);
                }
                else                
                    NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;                

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
            if (!ptOctantChildComp.Intersects3D3(RC.Projection * RC.View)) 
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

        private async void LoadNodesAsync(Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor)
        {
            //using (var cancellationTokenSource = new CancellationTokenSource())
            //{           
            
                //var userMovingTask = Task.Run(() =>
                //{
                //    if (IsUserMoving)// Cancel the task
                //        cancellationTokenSource.Cancel();
                //});               
                    
                await Task.Run(() =>
                {
                    //try
                    //{
                    //    // Were we already canceled?
                    //    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var loopLength = _determinedAsVisibleAndUnloaded.Count < _noOfLoadedNodes ? _determinedAsVisibleAndUnloaded.Count : _noOfLoadedNodes;

                        for (int i = 0; i < loopLength; i++)
                        {
                            //if (cancellationTokenSource.Token.IsCancellationRequested)
                            //    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var node = _determinedAsVisibleAndUnloaded.ElementAt(i).Value;
                            var ptOctantComp = node.GetComponent<PtOctantComponent>();
                            if (!ptOctantComp.WasLoaded)
                            {
                                var pts = LoadPointsForNode(ptAccessor, ptOctantComp);
                                ptOctantComp.NumberOfPointsInNode = pts.Count;
                                var meshes = GetMeshsForNode(ptAccessor, pts);
                                _loadedMeshs.Add(ptOctantComp.Guid, meshes);
                            }

                            //if (VisibleNodes.ContainsKey(ptOctantComp.Guid)) continue;                            
                            VisibleNodes.AddOrUpdate(ptOctantComp.Guid, node, (key, val) => val);
                        }
                    //}
                    //catch (OperationCanceledException e)
                    //{
                    //    Diagnostics.Log("Loading was canceled!");
                    //}                                       
                }); 

                //await userMovingTask;
            //}
        }

        private void LoadNodes(Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor)
        {
            foreach (var kvp in _determinedAsVisibleAndUnloaded)
            {
                var node = kvp.Value;
                var ptOctantComp = node.GetComponent<PtOctantComponent>();
                if (!ptOctantComp.WasLoaded)
                {
                    var pts = LoadPointsForNode(ptAccessor, ptOctantComp);
                    ptOctantComp.NumberOfPointsInNode = pts.Count;
                    var meshes = GetMeshsForNode(ptAccessor, pts);
                    _loadedMeshs.Add(ptOctantComp.Guid, meshes);                    
                }

                VisibleNodes.TryAdd(ptOctantComp.Guid, node);
            }

        }

        /// <summary>
        /// Traverse and updates the scene (octree) according to the _determinedAsVisible list.
        /// </summary>
        /// <param name="nodesToRender">Nodes that are visible AND loaded - corresponds to the ones that can be rendered.</param>
        /// <param name="node">Node that is processed in this step of the traversal.</param>        
        private void TraverseToUpdateScene(Dictionary<Guid, SceneNodeContainer> nodesToRender, SceneNodeContainer node)
        {
            var ptOctantComp = node.GetComponent<PtOctantComponent>();
            ptOctantComp.VisibleChildIndices = 0;

            if (!_determinedAsVisible.ContainsKey(ptOctantComp.Guid)) //Node isn't visible
            {
                RemoveNode(node, ptOctantComp); 
            }
            else
            {
                if (nodesToRender.ContainsKey(ptOctantComp.Guid)) //is visible and was loaded
                {
                    if (_loadedMeshs.TryGetValue(ptOctantComp.Guid, out var loadedMeshes))
                    {
                        node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                        node.Components.AddRange(loadedMeshes);
                    }
                    else
                    {
                        throw new ArgumentException("Trying to set meshes that are not loaded yet!");
                    }
                }                
            }

            foreach (var child in node.Children)
            {
                TraverseToUpdateScene(nodesToRender, child);
            }
        }

        private void RemoveNode(SceneNodeContainer node, PtOctantComponent ptOctantComponent)
        {
            node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
            
            _loadedMeshs.TryGetValue(ptOctantComponent.Guid, out var meshs);
            if (meshs != null)
            {
                //_numberOfVisiblePoints -= ptOctantComponent.NumberOfPointsInNode;
                foreach (var mesh in meshs)
                {
                    mesh.Dispose();
                }
            }
            _loadedMeshs.Remove(ptOctantComponent.Guid);
            VisibleNodes.Remove(ptOctantComponent.Guid, out var val);
            ptOctantComponent.WasLoaded = false;
            ptOctantComponent.VisibleChildIndices = 0;
        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, PtOctantComponent ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

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

        private int GetPtCountFromFile(PtOctantComponent ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using (BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read)))
            {
                // step to stream position
                //br.BaseStream.Position = node.StreamPosition;

                // read number of points
                return br.ReadInt32();
                
            }

        }

        /// <summary>
        /// Traverse breadth first to create 1D texture that contains the visible octree hierarchy.
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
