namespace Fusee.Xirkit
{
    /// <summary>
    /// <para>
    /// This module implements functionality for dynamically setting up circuits of
    /// arbitrary objects with selected fields and properties connected to each other. 
    /// </para>
    /// <para>
    /// This allows for building object graphs with a defined data flow from source
    /// properties to other objects' destination properties known from many graphically
    /// oriented use cases such as the "Node Editor" in Blender, binding dependency properties
    /// in WPF, etc. In contrast to other libraries, 
    /// Xirkit can take any C# type of object as a node within a circuit and any field or property 
    /// of such objects may serve as incoming or outgoing connection pins. 
    /// </para>
    /// <para>
    /// The animation part of FUSEEs <see cref="T:Fusee.Engine.SceneRenderer"/> is implemented
    /// on top of Xirkit to dynamically wire animation tracks to actual scene graph objects' properties.
    /// </para>
    /// The most important types in this module are 
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="Circuit"/></term>
    ///     <description>
    ///         A container that holds a list of all interconnected objects. Contains methods to build
    ///         up circuits and wires between nodes.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="Node"/></term>
    ///     <description>
    ///       Elements within a circuit. Arbitrary objects can be encapsulated within a node to allow
    ///       them to participate in a circuit.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="Pin"/></term>
    ///     <description>
    ///       An arbitrary object wrapped into a <see cref="Node"/> can expose members through Pins.
    ///       Pins are automatically created using the <see cref="Node.Attach"/> method.
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}