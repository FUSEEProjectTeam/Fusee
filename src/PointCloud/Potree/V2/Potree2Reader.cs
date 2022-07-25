using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Core.Accessors;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2.Data;
using System;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : IPointReader
    {
        /// <summary>
        /// A PointAccessor allows access to the point information (position, color, ect.) without casting it to a specific <see cref="PointType"/>.
        /// </summary>
        public IPointAccessor PointAccessor { get; private set; }

        private PotreeReaderMetadata _potreeReaderMetadata;

        internal PotreeReaderMetadata PotreeReaderMetadata
        {
            get
            {
                if (_potreeReaderMetadata == null)
                {
                    _potreeReaderMetadata = Potree2HierarchyReader.LoadHierarchy(_fileFolderPath);
                }
                return _potreeReaderMetadata;
            }
            set
            {
                _potreeReaderMetadata = value;
            }
        }

        private string _fileFolderPath;

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType GetPointType()
        {
            return PointType.PosD3ColF3LblB;
        }

        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="renderMode">Determines which <see cref="RenderMode"/> is used to display the returned point cloud."/></param>
        /// <param name="fileFolderPath">Path to the file.</param>
        public IPointCloud GetPointCloudComponent(string fileFolderPath, RenderMode renderMode = RenderMode.PointSize)
        {
            _fileFolderPath = fileFolderPath;

            var ptType = GetPointType();

            switch (ptType)
            {
                default:
                case PointType.PosD3:
                case PointType.PosD3ColF3InUs:
                case PointType.PosD3InUs:
                case PointType.PosD3ColF3:
                case PointType.PosD3LblB:
                case PointType.PosD3NorF3ColF3InUs:
                case PointType.PosD3NorF3InUs:
                case PointType.PosD3NorF3ColF3:
                case PointType.PosD3ColF3InUsLblB:
                    throw new ArgumentOutOfRangeException($"Invalid point type {ptType}");
                case PointType.PosD3ColF3LblB:
                    PointAccessor = new PosD3ColF3LblBAccessor();

                    switch (renderMode)
                    {
                        default:
                        case RenderMode.PointSize:
                            {
                                var dataHandler = new PointCloudDataHandler<GpuMesh, PosD3ColF3LblB>((PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateMeshPosD3ColF3LblB, LoadNodeData<PosD3ColF3LblB>);
                                var imp = new Potree2Cloud(dataHandler, GetOctree());
                                return new PointCloudComponent(imp, renderMode);
                            }

                        case RenderMode.Instanced:
                            {
                                var dataHandlerInstanced = new PointCloudDataHandler<InstanceData, PosD3ColF3LblB>((PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateInstanceDataPosD3ColF3LblB, LoadNodeData<PosD3ColF3LblB>, true);
                                var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree());
                                return new PointCloudComponent(imp, renderMode);
                            }

                    }
            }
        }

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        /// <returns></returns>
        public IPointCloudOctree GetOctree()
        {
            int pointSize = 0;

            if (PotreeReaderMetadata.Metadata != null)
            {
                foreach (var metaAttributeItem in PotreeReaderMetadata.Metadata.Attributes)
                {
                    pointSize += metaAttributeItem.Size;
                }

                PotreeReaderMetadata.Metadata.PointSize = pointSize;
            }

            var center = PotreeReaderMetadata.Hierarchy.Root.Aabb.Center;
            var size = PotreeReaderMetadata.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = PotreeReaderMetadata.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            Potree2Helpers.MapChildNodesRecursive(octree.Root, PotreeReaderMetadata.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Returns the points for one octant as generic array.
        /// </summary>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="id">The unique id of the octant.</param>
        /// <returns></returns>
        public TPoint[] LoadNodeData<TPoint>(OctantId id) where TPoint : new()
        {
            TPoint[] points = null;

            var node = Potree2Helpers.FindNode(ref PotreeReaderMetadata.Hierarchy, id);

            if (node != null)
            {
                points = Potree2NodeReader.LoadNodeData<TPoint>(node, PotreeReaderMetadata.Metadata, PointAccessor);
            }
 
            return points;
        }
    }
}