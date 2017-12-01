namespace Fusee.Serialization
{
    /// <summary>
    /// <para>
    /// The classes in this module are the building blocks for storing
    /// FUSEE's scene graph file contents.
    /// </para>
    /// <para>
    /// All classes and their properties are decorated with Protobuf.net
    /// attributes to mark them as serializable in the Google Protobuf 
    /// format (v2). The Protobuf.net compiler generates serialization 
    /// code directly from these classes. This automatically generated 
    /// serialization code can be found in Fusee.SerializationSerializer.dll.
    /// </para>
    /// <para>
    /// Serialized FUSEE scenegraphs are stored in *.fus files. The three
    /// main building block types for scenegraphs are 
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="SceneContainer"/></term>
    ///     <description>
    ///       Root object of each .fus file. Contains a file header
    ///       and a list of root nodes.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="SceneNodeContainer"/></term>
    ///     <description>
    ///       Instances of this type (called nodes) make up the hierarchy of objects.
    ///       Each node contains a list of child nodes and a list of components.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="SceneComponentContainer"/></term>
    ///     <description>
    ///       Instances of this type (called components) store the scene's payload. 
    ///       Various component types (inherited from <see cref="SceneComponentContainer"/>)
    ///       exist to keep the different components a scene is made of.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}
