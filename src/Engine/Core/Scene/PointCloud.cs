using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V2;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Point cloud file types that Fusee can handle.
    /// </summary>
    public enum PointCloudFileType
    {
        Las,
        Potree2
    }

    /// <summary>
    /// File type independent properties for point cloud rendering.
    /// </summary>
    public interface IPointCloudImp
    {
        public List<GpuMesh> MeshesToRender { get; set; }

        public PointCloudFileType FileType { get; }
    }

    /// <summary>
    /// File type independent point cloud implementation.
    /// </summary>
    public class PointCloud : SceneComponent
    {
        /// <summary>
        /// File type independent properties for point cloud rendering.
        /// </summary>
        public IPointCloudImp PointCloudImp { get; private set; }

        /// <summary>
        /// Instantiates the <see cref="PointCloudImp"/>, depending on the file type.
        /// </summary>
        /// <param name="fileFileFolderPath">The path to the point cloud file.</param>
        /// <param name="fileType">The type of the point cloud file.</param>
        /// <param name="pointType">The point type.</param>
        public PointCloud(string fileFileFolderPath, PointCloudFileType fileType, PointType pointType)
        {
            switch (fileType)
            {
                case PointCloudFileType.Las:
                    throw new NotImplementedException();
                case PointCloudFileType.Potree2:
                    {
                        switch (pointType)
                        {
                            default:
                            case PointType.Undefined:
                                throw new ArgumentException("Invalid Point Type!");
                            case PointType.Pos64:
                                PointCloudImp = new Potree2Cloud<Pos64>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Col32IShort:
                                PointCloudImp = new Potree2Cloud<Pos64Col32IShort>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64IShort:
                                PointCloudImp = new Potree2Cloud<Pos64IShort>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Col32:
                                PointCloudImp = new Potree2Cloud<Pos64Col32>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Label8:
                                PointCloudImp = new Potree2Cloud<Pos64Label8>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Nor32Col32IShort:
                                PointCloudImp = new Potree2Cloud<Pos64Nor32Col32IShort>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Nor32IShort:
                                PointCloudImp = new Potree2Cloud<Pos64Nor32IShort>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Pos64Nor32Col32:
                                PointCloudImp = new Potree2Cloud<Pos64Nor32Col32>(fileFileFolderPath, pointType);
                                break;
                            case PointType.Position_double__Color_float__Label_byte:
                                PointCloudImp = new Potree2Cloud<Position_double__Color_float__Label_byte>(fileFileFolderPath, pointType);
                                break;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get; private set; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float Size { get; private set; }
    }

    public abstract class Potree2BaseCloud : IPointCloudImp
    {
        public List<GpuMesh> MeshesToRender { get; set; }

        public PointCloudFileType FileType{ get; } = PointCloudFileType.Potree2;

        internal IPointCloudLoader PointCloudLoader;

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => PointCloudLoader.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of a octant. If an octant is smaller it won't be rendered.
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
        /// Maximal number of points that are visible in one frame - trade-off between performance and quality.
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

        /// <summary>
        /// The <see cref="PointType"/> of this PointCloud.
        /// </summary>
        public PointType Type { get; protected set; }

        protected IPointAccessor PointAccessor;

        /// <summary>
        /// Instantiates the <see cref="PointAccessor"/>.
        /// </summary>
        /// <param name="pointType">The <see cref="PointType"/>.</param>
        public Potree2BaseCloud(PointType pointType)
        {
            switch (pointType)
            {
                default:
                case PointType.Undefined:
                    throw new ArgumentException();
                case PointType.Pos64:
                    PointAccessor = new Pos64Accessor();
                    break;
                case PointType.Pos64Col32IShort:
                    PointAccessor = new Pos64Col32IShortAccessor();
                    break;
                case PointType.Pos64IShort:
                    PointAccessor = new Pos64IShortAccessor();
                    break;
                case PointType.Pos64Col32:
                    PointAccessor = new Pos64Col32Accessor();
                    break;
                case PointType.Pos64Label8:
                    PointAccessor = new Pos64Label8Accessor();
                    break;
                case PointType.Pos64Nor32Col32IShort:
                    PointAccessor = new Pos64Nor32Col32IShortAccessor();
                    break;
                case PointType.Pos64Nor32IShort:
                    PointAccessor = new Pos64Nor32IShortAccessor();
                    break;
                case PointType.Pos64Nor32Col32:
                    PointAccessor = new Pos64Nor32Col32Accessor();
                    break;
                case PointType.Position_double__Color_float__Label_byte:
                    PointAccessor = new Position_double__Color_float__Label_byte___Accessor();
                    break;
            }
        }

        public abstract void Update(CreateGpuMesh createGpuMesh, float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos);
    }

    /// <summary>
    /// Point type specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2Cloud<TPoint> : Potree2BaseCloud, IDisposable where TPoint : new()
    {
        /// <summary>
        /// Caches already created meshes.
        /// </summary>
        private MemoryCache<IEnumerable<GpuMesh>> _meshCache { get; set; }
        private readonly List<IEnumerable<GpuMesh>> _disposeQueue;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        /// <param name="fileFolderPath"></param>
        /// <param name="pointType"></param>
        public Potree2Cloud(string fileFolderPath, PointType pointType) : base(pointType)
        {
            var noOfOctants = (8 ^ 8) / 4;// Directory.GetFiles($"{fileFolderPath}\\Octants").Length;
                        
            PointCloudLoader = new PointCloudLoader<TPoint>(fileFolderPath, noOfOctants)
            {
                Octree = ReadPotree2Data.GetOctree<TPoint>(PointAccessor, fileFolderPath),
                FileFolderPath = fileFolderPath,
                PtAccessor = PointAccessor,

            };

            _meshCache = new();
            _meshCache.AddItem += OnCreateMesh;
            _meshCache.HandleEvictedItem = OnItemEvictedFromCache;
            Type = pointType;
            _disposeQueue = new List<IEnumerable<GpuMesh>>(noOfOctants);
            MeshesToRender = new List<GpuMesh>();
        }
       
        private int _maxNumberOfDisposals = 3;

        private static object _lockDisposeQueue = new object();

        public override void Update(CreateGpuMesh createGpuMesh, float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos)
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

            PointCloudLoader.RenderFrustum = renderFrustum;
            PointCloudLoader.ViewportHeight = viewportHeight;
            PointCloudLoader.Fov = fov;
            PointCloudLoader.CamPos = camPos;

            PointCloudLoader.Update();

            MeshesToRender.Clear();
            foreach (var guid in PointCloudLoader.VisibleNodes)
            {
                if (guid != null)
                {
                    if (!((PointCloudLoader<TPoint>)PointCloudLoader).PointCache.TryGetValue(guid, out var points)) return;
                    _meshCache.AddOrUpdate(guid, new GpuMeshFromPointsEventArgs<TPoint>(points, createGpuMesh, points.Length));

                    _meshCache.TryGetValue(guid, out var meshes);
                    foreach (var mesh in meshes)
                    {
                        // Add to Render List
                        MeshesToRender.Add(mesh);
                    }

                }
            }
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64((Pos64Accessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Col32IShort((Pos64Col32IShortAccessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64IShort((Pos64IShortAccessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Col32((Pos64Col32Accessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Label8((Pos64Label8Accessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort((Pos64Nor32Col32IShortAccessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32IShort((Pos64Nor32IShortAccessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
                            mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32((Pos64Nor32Col32Accessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
                            break;
                        }
                    case PointType.Position_double__Color_float__Label_byte:
                        {
                            Position_double__Color_float__Label_byte[] points;
                            var typedArgs = (GpuMeshFromPointsEventArgs<Position_double__Color_float__Label_byte>)e;
                            if (ptCnt > maxVertCount)
                            {
                                points = new Position_double__Color_float__Label_byte[numberOfPointsInMesh];
                                Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                            }
                            else
                            {
                                points = typedArgs.Points;
                            }
                            mesh = MeshFromPointCloudPoints.GetMesh___Position_double__Color_float__Label_byte((Position_double__Color_float__Label_byte___Accessor)PointAccessor, points, false, float3.Zero, typedArgs.CreateGpuMesh);
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
        ~Potree2Cloud()
        {
            Dispose(disposing: false);
        }
    }
}
