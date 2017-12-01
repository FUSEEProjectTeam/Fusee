namespace Fusee.Engine.Core
{
    /// <summary>
    /// <para>
    /// Classes in this module implement FUSEE's 3D rendering capability. 
    /// Many classes contain methods that
    /// enable user code to render 3D content, create 3D output windows, create and maniulate scene contents,
    /// organize scene contents in scene graphs, etc. 
    /// </para>
    /// <para>
    /// Among the most important classses are
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="RenderCanvas"/></term>
    ///     <description>The base class for FUSEE Apps. Creates and manages the rendering window and provides
    ///     a render context to draw onto.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="RenderContext"/></term>
    ///     <description>Abstracts 3D Drawing functionality from the underlying hardware-API (e.g. OpenGL).</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="SceneRenderer"/></term>
    ///     <description>Visits a scene graph's items and renders each item's contribution to a render context.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Some of the functionality relies on platform specific implementations found in underlying platform-dependent modules.
    /// Common classes used by both, this module and underlying platform-dependent implementation modules can be found int the
    /// <see cref="Fusee.Engine.Common"/> namespace.
    /// </para>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}
