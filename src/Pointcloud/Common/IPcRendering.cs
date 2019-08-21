using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using System;

namespace Fusee.Pointcloud.Common
{ 

    /// <summary>
    /// Implement this interface into apps that use the ooc file converter.
    /// </summary>
    public interface IPcRendering
    {
        bool IsAlive { get; set; }

        bool ReadyToLoadNewFile { get; set; }

        bool DoShowOctants { get; set; }

        bool IsSceneLoaded { get; set; }

        bool UseWPF { get; set; }

        bool IsInitialized { get; set; }        

        float3 InitCameraPos { get; }

        IRenderCanvasImp CanvasImplementor { get; set; }

        IRenderContextImp ContextImplementor { get; set; }

        SceneNodeContainer GetOocLoaderRootNode();

        bool GetOocLoaderWasSceneUpdated();

        int GetOocLoaderPointThreshold();

        void SetOocLoaderPointThreshold(int value);

        void DeleteOctants();

        void DeletePointCloud();

        void ResetCamera();

        void LoadPointCloudFromFile();

        void CloseGameWindow();

        void Run();        

        RenderContext GetRc();        
    }
}