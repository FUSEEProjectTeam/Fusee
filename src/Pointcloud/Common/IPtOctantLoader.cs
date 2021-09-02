
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    public interface IPtOctantLoader
    {
        public double3 InitCamPos { get; set; }

        public bool IsShuttingDown { get; set; }

        public RenderContext RC { get; }

        public void Init(RenderContext rc);

        public void UpdateScene(PointSizeMode sizeMode, ShaderEffect depthFx, ShaderEffect colorFx);

        public void ShowOctants(SceneContainer scene);

        public SceneNode RootNode { get; set; }

        public bool WasSceneUpdated { get; }

        public int PointThreshold { get; set; }

        public float MinProjSizeModifier { get; set; }

        public string FileFolderPath { get; set; }

        public Texture VisibleOctreeHierarchyTex { get; set; }

        public void DeleteOctants(SceneContainer scene);
    }
}