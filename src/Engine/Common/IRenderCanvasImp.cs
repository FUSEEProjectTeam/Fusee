using System;

namespace Fusee.Engine
{
    public interface IRenderCanvasImp
    {
        /// <summary>
        /// Implementation Tasks: Gets or sets the width(pixel units) of the Canvas.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        int Width { get; set; }

        /// <summary>
        /// Implementation Tasks: Gets or sets the height(pixel units) of the Canvas.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        int Height { get; set; }

        /// <summary>
        /// Implementation Tasks: Gets or sets the caption of the Application(Window Title).
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        string Caption { get; set; }

        /// <summary>
        /// Implementation Tasks: Gets the delta time. The delta time is the time it took the last frame to render in milliseconds.
        /// </summary>
        /// <value>
        /// The delta time.
        /// </value>
        double DeltaTime { get; }

        /// <summary>
        /// Implementation Tasks: Gets or sets a value indicating whether vertical synchronization is enabled.
        /// This option is used to reduce "Glitches" during rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertical sync]; otherwise, <c>false</c>.
        /// </value>
        bool VerticalSync { get; set; }

        /// <summary>
        /// Implementation Tasks: Gets or sets a value indicating whether [enable blending].
        /// Blending is required to allow rendering of transparent objects.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable blending]; otherwise, <c>false</c>.
        /// </value>
        bool EnableBlending { get; set; }

        /// <summary>
        /// Implementation Tasks: Gets or sets a value indicating whether this <see cref="IRenderCanvasImp"/> is in fullscreen mode.
        /// This option can not be applied to all plattforms. 
        /// </summary>
        /// <value>
        ///   <c>true</c> if fullscreen; otherwise, <c>false</c>.
        /// </value>
        bool Fullscreen { get; set; }

        /// <summary>
        /// Implementation Tasks: Presents the final rendered image. The FrameBuffer needs to be cleared afterwards.
        /// The delta time needs to be recalculated when this function is called.
        /// </summary>
        void Present();

        /// <summary>
        /// Runs this instance. This function should not be called more than once as its only for initilization purposes.
        /// </summary>
        void Run();

        /// <summary>
        /// Set the cursor (the mouse pointer image) to one of the pre-defined types
        /// </summary>
        /// <param name="cursorType">The type of the cursor to set.</param>
        void SetCursor(CursorType cursorType);

        /// <summary>
        /// Opens the given URL in the user's standard web browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open</param>
        void OpenLink(string link);

        /// <summary>
        /// Sets the data for a video wall.
        /// </summary>
        /// <param name="monitorsHor"></param>
        /// <param name="monitorsVert"></param>
        void VideoWall(int monitorsHor, int monitorsVert, bool activate = true, bool borderHidden = true);

        /// <summary>
        /// Sets the size of the output window for desktop development.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="borderHidden">Show the window border or not.</param>
        void SetWindowSize(int width, int height, bool borderHidden = false, int posx = 0, int posy = 0);


        /// <summary>
        /// Occurs when [init] is called.
        /// Implementation Tasks: Handle the Initialization process
        /// </summary>
        event EventHandler<InitEventArgs> Init;
        /// <summary>
        /// Occurs when [UnLoad] is called.
        /// </summary>
        event EventHandler<InitEventArgs> UnLoad;
        /// <summary>
        /// Occurs when [Render] is called.
        /// </summary>
        event EventHandler<RenderEventArgs> Render;
        /// <summary>
        /// Occurs when [Resize] is called.
        /// </summary>
        event EventHandler<ResizeEventArgs> Resize;
    }
}
