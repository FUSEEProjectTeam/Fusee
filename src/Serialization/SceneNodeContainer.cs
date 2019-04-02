using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    public class OnAddChild : EventArgs
    {
        public SceneNodeContainer Snc { get; }
        public OnAddChild(SceneNodeContainer snc)
        {
            Snc = snc;
        }
        
    }

    /// <summary>
    /// The building block to create hierarchies.
    /// </summary>
    [ProtoContract]
    [DebuggerDisplay("{Name,nq}, {(Components==null)? \"No\" : Components.Count.ToString(),nq} components, {(Children==null)? \"No\" : Children.Count.ToString(),nq} children")]
    public class SceneNodeContainer
    {
        /// <summary>
        /// The name.
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// The components this node is made of.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public List<SceneComponentContainer> Components;

        /// <summary>
        /// Possible children. 
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public ChildList Children {
            get => _children;
            set
            {
                _children = value;
                foreach (var child in _children)
                {
                    child.Parent = this;
                }
            }
        }

        private ChildList _children;

        /// <summary>
        /// This SceneNodeContainer's snc. 
        /// </summary>
        public SceneNodeContainer Parent;

        /// <summary>
        /// Creates a new instance of te SceneNodeContainer class. 
        /// </summary>
        public SceneNodeContainer()
        {
            Components = new List<SceneComponentContainer>();
            Children = new ChildList();
            Children.OnAdd += (sender, e) => e.Snc.Parent = this;
        }

        public float4x4 GetGlobalTransformation()
        {
            var res = float4x4.Identity;
            if (Parent == null)
                return this.GetComponent<TransformComponent>().Matrix();

            AccumulateGlobalTransform(ref res, this);
            return res;
        }

        private static void AccumulateGlobalTransform(ref float4x4 res, SceneNodeContainer snc)
        {
            while (true)
            {
                if (snc.Parent == null)
                {
                    return;
                }

                res *= snc.GetComponent<TransformComponent>().Matrix();
                snc = snc.Parent;
            }
        }


        public float4x4 GetParentProjection()
        {
            var res = float4x4.Identity;
            if (Parent == null)
                return this.GetComponent<ProjectionComponent>().Matrix();

            var parent = Parent;
            while (true)
            {
                if (parent.Parent == null || res != float4x4.Identity)
                {
                    return res;
                }

                res = parent.GetComponent<ProjectionComponent>().Matrix();
                parent = parent.Parent;
            }
        }
    }
}
