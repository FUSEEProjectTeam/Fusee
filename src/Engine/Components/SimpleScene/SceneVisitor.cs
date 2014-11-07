using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fusee.Serialization;
using JSIL.Meta;

namespace Fusee.Engine.SimpleScene
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VisitMethodAttribute : Attribute
    {
    }


    internal class VisitorCallerFactory
    {
        [JSExternal]
        public static SceneVisitor.VisitComponentMethod MakeComponentVisitor(SceneVisitor visitor, MethodInfo info)
        {
            return delegate(SceneComponentContainer component) { info.Invoke(visitor, new object[] { component }); };
        }

        [JSExternal]
        public static SceneVisitor.VisitNodeMethod MakeNodeVistor(SceneVisitor visitor, MethodInfo info)
        {
            return delegate(SceneNodeContainer node) { info.Invoke(visitor, new object[] { node }); };
        }

    }


    /// <summary>
    /// Serves as a base class for (kindalike) visitor pattern implementations traversing scenes. Visitors derived from this class may implement
    /// their own Visit methods for all kinds of scene graph elements. Visitor methods can be defined for scene nodes (although many implementations
    /// will most likely NOT have a very big inheritance tree for nodes) as well as for scene components.
    /// A Visitor method can be any instance method (not static) taking one parameter either derived from <see cref="SceneNodeContainer"/> or derived from
    /// <see cref="SceneComponentContainer"/>. To mark such a method as a Visitor method it needs to be decorated with the <see cref="VisitMethodAttribute"/> 
    /// attribute. Visitor methods can have arbitrary names and don't need to be virtual. 
    /// </summary>
    public class SceneVisitor
    {
        internal delegate void VisitNodeMethod(SceneNodeContainer node);
        internal delegate void VisitComponentMethod(SceneComponentContainer component);

        internal class VisitorSet
        {
            public Dictionary<Type, VisitNodeMethod> Nodes = new Dictionary<Type, VisitNodeMethod>();
            public Dictionary<Type, VisitComponentMethod> Components = new Dictionary<Type, VisitComponentMethod>();
        }

        // The list of visitor methods defined in a concrete child class of SceneVisitor
        private VisitorSet _visitors;

        // The static list of all known sets of visitor methods
        // This is kept to avoid building the visitor map again and again different instances of the same visitor.
        private static Dictionary<Type, VisitorSet> _visitorMap;

        /// <summary>
        /// Start traversing an entire scene. Performs a recursive depth-first traversal 
        /// over the child list starting with the first child in the list.
        /// </summary>
        /// <param name="scene">The scene containing the child list to traverse over.</param>
        public void Traverse(SceneContainer scene)
        {
            if (scene == null)
                return;

            InitTraversal();

            if (scene.Children == null)
                return;

            if (_visitors.Components.Count > 0)
            {
                foreach (var node in scene.Children)
                {
                    DoTraverse(node);
                }
            }
            else
            {
                foreach (var node in scene.Children)
                {
                    DoTraverseNoComponents(node);
                }
            }
        }

        /// <summary>
        /// Start traversing a scene graph starting with the given root node. Performs a recursive depth-first 
        /// traversal from the speciefiedd root.
        /// </summary>
        /// <param name="rootNode">The root node where to start the traversal.</param>
        public void Traverse(SceneNodeContainer rootNode)
        {
            if (rootNode == null)
                return;

            InitTraversal();

            if (_visitors.Components.Count > 0)
                DoTraverse(rootNode);
            else
                DoTraverseNoComponents(rootNode);
        }

        /// <summary>
        /// Start traversing a list of nodes. Performs a recursive depth-first traversal 
        /// over the list starting with the first node in the list.
        /// </summary>
        /// <param name="scene">The list of nodes to traverse over.</param>
        public void Traverse(IEnumerable<SceneNodeContainer> children)
        {
            if (children == null)
                return;

            InitTraversal();

            if (_visitors.Components.Count > 0)
            {
                foreach (var node in children)
                {
                    DoTraverse(node);
                }
            }
            else
            {
                foreach (var node in children)
                {
                    DoTraverseNoComponents(node);
                }
            }
        }

        private void InitTraversal()
        {
            if (_visitors != null) 
                return;

            if (_visitorMap == null)
                _visitorMap = new Dictionary<Type, VisitorSet>();

            Type myType = GetType();
            if (_visitorMap.TryGetValue(myType, out _visitors)) 
                return;

            _visitors = new VisitorSet();
            foreach (MethodInfo methodInfo in myType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsVisitor(methodInfo))
                    continue;

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != 1) 
                    continue;

                Type paramType = parameters[0].ParameterType;
                if (typeof(SceneComponentContainer).IsAssignableFrom(paramType))
                {
                    _visitors.Components.Add(paramType,
                        VisitorCallerFactory.MakeComponentVisitor(this, methodInfo));
                }
                else if (typeof(SceneNodeContainer).IsAssignableFrom(paramType))
                {
                    _visitors.Nodes.Add(paramType,
                        VisitorCallerFactory.MakeNodeVistor(this, methodInfo));
                }
            }
            _visitorMap.Add(myType, _visitors);
        }

        private static bool IsVisitor(MethodInfo methodInfo)
        {
            // First try - only available in .Net 4.5
            // Attribute a = methodInfo.GetCustomAttribute(VisitMethodAttribute));
            // return (a != null);

            // Second try - runs on .Net since 2.0. JSIL doesn't implement "Attribute.GetCustomAttribute", however
            // Attribute a = Attribute.GetCustomAttribute(methodInfo, typeof (VisitMethodAttribute));
            // return (a != null);

            // Third try:
            object[] a = methodInfo.GetCustomAttributes(typeof(VisitMethodAttribute), false);
            return (a != null && a.Length > 0);
        }

 
        private void DoTraverseNoComponents(SceneNodeContainer node)
        {
            DoVisitNode(node);

            // DO NOT visit components

            if (node.Children == null)
                return;

            foreach (var child in node.Children)
            {
                DoTraverseNoComponents(child);
            }
        }

        private void DoTraverse(SceneNodeContainer node)
        {
            DoVisitNode(node);

            DoVisitComponents(node);

            if (node.Children == null)
                return;

            foreach (var child in node.Children)
            {
                DoTraverse(child);
            }
        }

        private void DoVisitComponents(SceneNodeContainer node)
        {
            // Are there any components at all?
            if (node.Components == null)
                return;
            // Visit each component
            foreach (var component in node.Components)
            {
                VisitComponentMethod visitComponent;
                if (_visitors.Components.TryGetValue(component.GetType(), out visitComponent))
                {
                    visitComponent(component);
                }
            }
        }

        private void DoVisitNode(SceneNodeContainer node)
        {
            VisitNodeMethod visitNode;
            if (_visitors.Nodes.TryGetValue(node.GetType(), out visitNode))
            {
                visitNode(node);
            }
        }

    }
}
