using Fusee.Xene;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Nodes are the building blocks for scene graphs within .fus files.
    /// </summary>
    [ProtoContract]
    public class FusNode : INode
    {
        #region Payload
        /// <summary>
        /// The name of the node. Often used to identify individual parts of a scene.
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// Indices into the <see cref="FusScene"/>'s list of components that make up this node.
        /// </summary>
        [ProtoMember(2)]
        public List<int> Components;

        /// <summary>
        /// This node's children. Possibly empty.
        /// </summary>
        [ProtoMember(3)]
        public List<FusNode> Children;
        #endregion

        #region Scene assembly helpers
        public FusScene Scene;

        /// <summary>
        /// Returns all children of this node
        /// </summary>
        public IEnumerable<INode> EnumChildren => Children;

        /// <summary>
        /// Returns all components of this node
        /// </summary>
        public IEnumerable<IComponent> EnumComponents => Components?.Select(idx => Scene?.ComponentList[idx]);

        /// <summary>
        /// Adds a component to this node's list of components. Internally the component is 
        /// stored in this node's <see cref="Scene"/> instance and referenced in the node.
        /// </summary>
        /// <param name="component">The component to store in this node's component list.</param>
        public void AddComponent(FusComponent component)
        {
            if (Scene == null)
                throw new InvalidOperationException($"Cannot add component {component} to node {this} (not yet attached to a scene)");
            Components.Add(Scene.GetComponentIndex(component));
        }

        /// <summary>
        /// Adds a node as a a child node to 
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(FusNode node)
        {
            if (node.Scene == null)
            {
                node.Scene = Scene;
                if (node.Children != null && node.Children.Count != 0)
                    throw new InvalidOperationException("Adding FusNode objects with children is not yet implemented");
            }
            else if (node.Scene != Scene)
            {
                throw new InvalidOperationException("Adding FusNode objects from other FusScenes is not yet implemented");
            }

            if (node.Components == null)
                node.Components = new List<int>();

            if (Children == null)
                Children = new List<FusNode>();

            Children.Add(node);
        }
        #endregion
    }
}