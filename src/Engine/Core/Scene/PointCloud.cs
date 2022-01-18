using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V1;
using Fusee.PointCloud.PotreeReader.V2;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Will render a Potree 1.0 Point Cloud if visited by the SceneRenderer.
    /// </summary>
    public class PointCloud : SceneComponent, IDisposable
    {
        /// <summary>
        /// Caches already created meshes.
        /// </summary>
        public MemoryCache<IEnumerable<GpuMesh>> MeshCache { get; private set; }

        /// <summary>
        /// The <see cref="PointType"/> of this PointCloud.
        /// </summary>
        public PointType Type { get; private set; }

        private IPointAccessor _pointAccessor;

        private readonly List<IEnumerable<GpuMesh>> _disposeQueue;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        /// <param name="fileFolderPath"></param>
        /// <param name="pointType"></param>
        public PointCloud(string fileFolderPath, PointType pointType)
        {
            var noOfOctants = (8 ^ 8) / 4;// Directory.GetFiles($"{fileFolderPath}\\Octants").Length;
            IPointAccessor pointAccessor;
            switch (pointType)
            {
                default:
                case PointType.Undefined:
                    throw new ArgumentException("Invalid PointType!");
                case PointType.Pos64:
                    pointAccessor = new Pos64Accessor();
                    PointCloudLoader = new PointCloudLoader<Pos64>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Col32IShort:
                    pointAccessor = new Pos64Col32IShortAccessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Col32IShort>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Col32IShort>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Col32IShort>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Col32IShort>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64IShort:
                    pointAccessor = new Pos64IShortAccessor();
                    PointCloudLoader = new PointCloudLoader<Pos64IShort>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64IShort>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64IShort>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64IShort>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Col32:
                    pointAccessor = new Pos64Col32Accessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Col32>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Col32>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Col32>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Col32>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Label8:
                    pointAccessor = new Pos64Label8Accessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Label8>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Label8>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Label8>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Label8>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Nor32Col32IShort:
                    pointAccessor = new Pos64Nor32Col32IShortAccessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Nor32Col32IShort>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Nor32Col32IShort>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Nor32Col32IShort>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Nor32Col32IShort>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Nor32IShort:
                    pointAccessor = new Pos64Nor32IShortAccessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Nor32IShort>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Nor32IShort>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Nor32IShort>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Nor32IShort>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Pos64Nor32Col32:
                    pointAccessor = new Pos64Nor32Col32Accessor();
                    PointCloudLoader = new PointCloudLoader<Pos64Nor32Col32>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotreeData.GetOctree<Pos64Nor32Col32>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,

                    };
                    Center = new float3(((PointCloudLoader<Pos64Nor32Col32>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Pos64Nor32Col32>)PointCloudLoader).Octree.Root.Size;
                    break;
                case PointType.Position_double__Color_float__Label_byte:
                    pointAccessor = new Position_double__Color_float__Label_byte___Accessor();
                    PointCloudLoader = new PointCloudLoader<Position_double__Color_float__Label_byte>(fileFolderPath, noOfOctants)
                    {
                        Octree = ReadPotree2Data.GetOctree<Position_double__Color_float__Label_byte>(pointAccessor, fileFolderPath),
                        FileFolderPath = fileFolderPath,
                        PtAccessor = pointAccessor,
                    };
                    Center = new float3(((PointCloudLoader<Position_double__Color_float__Label_byte>)PointCloudLoader).Octree.Root.Center);
                    Size = (float)((PointCloudLoader<Position_double__Color_float__Label_byte>)PointCloudLoader).Octree.Root.Size;
                    break;
            }

            MeshCache = new();
            MeshCache.AddItem += OnCreateMesh;
            MeshCache.HandleEvictedItem = OnItemEvictedFromCache;
            Type = pointType;
            _pointAccessor = pointAccessor;
            _disposeQueue = new List<IEnumerable<GpuMesh>>(noOfOctants);
        }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get; private set; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => PointCloudLoader.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => PointCloudLoader.MinProjSizeModifier;
            set
            {
                PointCloudLoader.MinProjSizeModifier = value;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get => PointCloudLoader.PointThreshold;
            set
            {
                PointCloudLoader.PointThreshold = value;
            }
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public double UpdateRate
        {
            get => PointCloudLoader.UpdateRate;
            set
            {
                PointCloudLoader.UpdateRate = value;
            }
        }

        internal float Fov;
        internal float3 CamPos;
        internal FrustumF RenderFrustum;
        internal int ViewportHeight;
        internal IPointCloudLoader PointCloudLoader;

        private int _maxNumberOfDisposals = 3;

        private static object _lockDisposeQueue = new object();

        internal void Update()
        {
            lock (_lockDisposeQueue)
            {
                if (_disposeQueue.Count > 0)
                {
                    var nodesInQueue = _disposeQueue.Count;
                    var count = nodesInQueue < _maxNumberOfDisposals ? nodesInQueue : _maxNumberOfDisposals;
                    for (int i = 0; i < count; i++)
                    {
                        var meshes = _disposeQueue.Last();
                        _disposeQueue.RemoveAt(_disposeQueue.Count - 1);
                        foreach (var mesh in meshes)
                            mesh.Dispose();
                    }
                }
            }

            PointCloudLoader.RenderFrustum = RenderFrustum;
            PointCloudLoader.ViewportHeight = ViewportHeight;
            PointCloudLoader.Update(Fov, CamPos);
        }

        private void OnItemEvictedFromCache(object guid, object meshes, EvictionReason reason, object state)
        {
            lock (_lockDisposeQueue)
            {
                _disposeQueue.Add((IEnumerable<GpuMesh>)meshes);
            }
        }

        private IEnumerable<GpuMesh> OnCreateMesh(object sender, EventArgs e)
        {
            var args = (GpuMeshFromPointsEventArgs)e;
            var ptCnt = args.NumberOfPoints;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)ptCnt / maxVertCount);
            List<GpuMesh> meshes = new(noOfMeshes);

            int meshCnt = 0;

            for (int i = 0; i < ptCnt; i += maxVertCount)
            {
                int numberOfPointsInMesh;
                if (noOfMeshes == 1)
                    numberOfPointsInMesh = ptCnt;
                else if (noOfMeshes == meshCnt + 1)
                    numberOfPointsInMesh = (ptCnt - maxVertCount * meshCnt);
                else
                    numberOfPointsInMesh = maxVertCount;

                GpuMesh mesh;
                switch (Type)
                {
                    default:
                    case PointType.Undefined:
                        {
                            throw new InvalidOperationException("Invalid Point Type!");
                        }

                    case PointType.Pos64:
                        {
                            Pos64[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64((Pos64Accessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Col32IShort:
                        {
                            Pos64Col32IShort[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Col32IShort>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Col32IShort[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Col32IShort((Pos64Col32IShortAccessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64IShort:
                        {
                            Pos64IShort[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64IShort>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64IShort[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64IShort((Pos64IShortAccessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Col32:
                        {
                            Pos64Col32[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Col32>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Col32[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Col32((Pos64Col32Accessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Label8:
                        {
                            Pos64Label8[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Label8>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Label8[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Label8((Pos64Label8Accessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Nor32Col32IShort:
                        {
                            Pos64Nor32Col32IShort[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Nor32Col32IShort>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Nor32Col32IShort[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort((Pos64Nor32Col32IShortAccessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Nor32IShort:
                        {
                            Pos64Nor32IShort[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Nor32IShort>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Nor32IShort[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32IShort((Pos64Nor32IShortAccessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                    case PointType.Pos64Nor32Col32:
                        {
                            Pos64Nor32Col32[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Pos64Nor32Col32>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Pos64Nor32Col32[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32((Pos64Nor32Col32Accessor)_pointAccessor, points, false, float3.Zero, typedArgs.RenderContext.CreateGpuMesh);
                            break;
                        }
                }

                meshes.Add(mesh);
                meshCnt++;
            }

            return meshes;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _lockDisposeQueue = null;
                    foreach (var meshes in _disposeQueue)
                    {
                        foreach (var mesh in meshes)
                        {
                            mesh.Dispose();
                        }
                    }
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~PointCloud()
        {
            Dispose(disposing: false);
        }
    }
}
