namespace Fusee.Base.Core
{
    /// <summary>
    /// <para>
    /// Types in this module implent FUSEE's basic functionality 
    /// such as operating system functionality abstraction with varying implementations
    /// on different platforms and general functionality not related to 3D rendering
    /// </para>
    /// <para>
    /// Among the most important classses are
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="AssetStorage"/></term>
    ///     <description>Can be used in applications to load assets of various types.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="Diagnostics"/></term>
    ///     <description>Contains diagnostic aids to be used during debugging and logging.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="Font"/></term>
    ///     <description>Platform independent access to font data.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Some of the functionality relies on platform specific implementations found in underlying platform-dependent modules.
    /// Common classes used by both, this module and underlying platform-dependent implementation modules can be found int the
    /// <see cref="Fusee.Base.Common"/> namespace.
    /// </para>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}
