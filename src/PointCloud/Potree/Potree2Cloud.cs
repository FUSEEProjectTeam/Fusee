using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Potree
{
    /// <summary>
    /// Point type specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2Cloud : IPointCloudImp, IDisposable
    {
        /// <summary>
        /// The complete list of meshes that can be rendered.
        /// </summary>
        public List<GpuMesh> MeshesToRender { get; set; }

        /// <summary>
        /// Nows which octants are visible and when to invoke a point loading event.
        /// </summary>
        public VisibilityTester VisibilityTester { get; }

        /// <summary>
        /// Nows which octants are visible and when to invoke a point loading event.
        /// </summary>
        public PointCloudDataHandlerBase DataHandler { get; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => VisibilityTester.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of a octant. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => VisibilityTester.MinProjSizeModifier;
            set
            {
                _doUpdate = true;
                VisibilityTester.MinProjSizeModifier = value;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - trade-off between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get => VisibilityTester.PointThreshold;
            set
            {
                _doUpdate = true;
                VisibilityTester.PointThreshold = value;
            }
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public float UpdateRate
        {
            get => VisibilityTester.UpdateRate;
            set
            {
                VisibilityTester.UpdateRate = value;
            }
        }

        public float3 Center => (float3)VisibilityTester.Octree.Root.Center;

        public float3 Size => new((float)VisibilityTester.Octree.Root.Size);

        private bool _disposed;
        private readonly GetMeshes _getMeshes;

        private bool _doUpdate = true;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        public Potree2Cloud(PointCloudDataHandlerBase dataHandler, IPointCloudOctree octree)
        {
            MeshesToRender = new List<GpuMesh>();
            DataHandler = dataHandler;
            VisibilityTester = new VisibilityTester(octree, dataHandler.TriggerPointLoading);
            _getMeshes = dataHandler.GetMeshes;
        }


        public void Update(float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos)
        {
            DataHandler.ProcessDisposeQueue();

            if (!_doUpdate &&
                renderFrustum == VisibilityTester.RenderFrustum &&
                viewportHeight == VisibilityTester.ViewportHeight &&
                fov == VisibilityTester.Fov &&
                camPos == VisibilityTester.CamPos) return;

            MeshesToRender.Clear();

            VisibilityTester.RenderFrustum = renderFrustum;
            VisibilityTester.ViewportHeight = viewportHeight;
            VisibilityTester.Fov = fov;
            VisibilityTester.CamPos = camPos;

            VisibilityTester.Update();

            foreach (var guid in VisibilityTester.VisibleNodes)
            {
                if (guid == null) continue;

                var meshes = _getMeshes(guid);

                if (meshes == null) continue; //points for this octant aren't loaded yet.

                MeshesToRender.AddRange(meshes);
            }
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
                    foreach (var mesh in MeshesToRender)
                    {
                        mesh.Dispose();
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
