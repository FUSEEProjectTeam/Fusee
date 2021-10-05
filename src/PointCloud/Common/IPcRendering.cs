using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Delegate that allows us to implement a setup method in the app's Main.cs
    /// </summary>
    public delegate void AppSetupDelegate();

    /// <summary>
    /// Implement this interface into wpf apps that use point cloud ooc rendering.
    /// </summary>
    public interface IPcRendering
    {
        /// <summary>
        /// Set to true if closing the app was requested from wpf.
        /// </summary>
        bool ClosingRequested { get; set; }

        /// <summary>
        /// Needed when using UI / WPF. Set to false on DeInit().
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Needed when using UI / WPF. Determines if a new file can be loaded. E.g. when 
        /// </summary>
        bool ReadyToLoadNewFile { get; }

        /// <summary>
        /// Set to true if the outlines of visible octants shall be rendered.
        /// </summary>
        bool DoShowOctants { get; set; }

        /// <summary>
        /// Set to false before changing something in your scene and to false after.
        /// </summary>
        bool IsSceneLoaded { get; }

        /// <summary>
        /// Allows different logic if you use WPF (or another UI).
        /// </summary>
        bool UseWPF { get; set; }

        /// <summary>
        /// Allows to check if the app has finished its Init() call.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// The initial camera position. E.g. for resetting the camera.
        /// </summary>
        float3 InitCameraPos { get; }

        /// <summary>
        /// <see cref="RenderCanvas.CanvasImplementor"/>
        /// </summary>
        IRenderCanvasImp CanvasImplementor { get; set; }

        /// <summary>
        /// <see cref="RenderCanvas.ContextImplementor"/>
        /// </summary>
        IRenderContextImp ContextImplementor { get; set; }

        IPtOctantLoader OocLoader { get; }

        IPtOctreeFileReader OocFileReader { get; }

        /// <summary>
        /// Wrapper to get the Root Node from the app's File Loader./>
        /// </summary>
        SceneNode GetOocLoaderRootNode();

        /// <summary>
        /// Wrapper to get the WasSceneUpdated bool from the app's File Loader./>
        /// </summary>
        bool GetOocLoaderWasSceneUpdated();

        /// <summary>
        /// Wrapper to get the Point Threshold from the app's File Loader./>
        /// </summary>
        int GetOocLoaderPointThreshold();

        /// <summary>
        /// Wrapper to set the Point Threshold in the app's File Loader./>
        /// </summary>
        void SetOocLoaderPointThreshold(int value);

        /// <summary>
        /// Wrapper to set the minimal screen projected size in the app's File Loader./>
        /// </summary>
        void SetOocLoaderMinProjSizeMod(float value);

        /// <summary>
        /// Wrapper to get the minimal screen projected size from the app's File Loader./>
        /// </summary>
        float GetOocLoaderMinProjSizeMod();

        /// <summary>
        /// Method to delete the octants from the scene.
        /// </summary>
        void DeleteOctants();

        /// <summary>
        /// Method to delete the point cloud from the scene.
        /// </summary>
        void DeletePointCloud();

        /// <summary>
        /// Method to reset the camera.
        /// </summary>
        void ResetCamera();

        /// <summary>
        /// Method to load the point cloud from an ooc file.
        /// </summary>
        void LoadPointCloudFromFile();

        /// <summary>
        /// <see cref="RenderCanvas.CloseGameWindow"/>
        /// </summary>
        void CloseGameWindow();

        /// <summary>
        /// <see cref="RenderCanvas.Run"/>
        /// </summary>
        void Run();

        /// <summary>
        /// Initializes the canvas for the rendering loop.
        /// </summary>
        void InitCanvas();

        /// <summary>
        /// Wrapper to get the app's RenderContext.
        /// </summary>
        /// <returns></returns>
        RenderContext GetRc();
    }
}