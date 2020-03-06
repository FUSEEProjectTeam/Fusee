using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// .fus File contents making up a scene.
    /// </summary>
    [ProtoContract]
    public class FusScene : FusContents
    {
        #region Payload
        /// <summary>
        /// Overall list of components used in this scene. This list contains the .fus file's physical payload.
        /// Indices into this list are used as references at other places in the .fus file.
        /// </summary>
        [ProtoMember(1)]
        public List<FusComponent> ComponentList;

        /// <summary>
        /// List of root nodes of the scene graph making up this scene.
        /// </summary>
        [ProtoMember(2)]
        public List<FusNode> Children;

        /// <summary>
        /// Adds a node as a a child node to 
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(FusNode node)
        {
            if (node.Scene == null)
            {
                node.Scene = this;
                if (node.Children != null && node.Children.Count != 0)
                    throw new InvalidOperationException("Adding FusNode objects with children is not yet implemented");
            }
            else if (node.Scene != this)
            {
                throw new InvalidOperationException("Adding FusNode objects from other FusScenes is not yet implemented");
            }

            if (node.Components == null)
                node.Components = new List<int>();

            Children.Add(node);
        }

        #endregion Payload


        internal int GetComponentIndex(FusComponent component)
        {
            if (ComponentList == null)
                ComponentList = new List<FusComponent>();

            int inx = ComponentList.FindIndex(comp => comp == component);
            if (inx < 0)
                inx = ComponentList.Count;

            ComponentList.Add(component);
            return inx;
        }
    }
}