using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class SceneEntity
    {

        public string tag;
        public Transformation transform;
        public Renderer renderer;
        private SceneEntity _parent;
        private List<SceneEntity> _childSceneEntities;
        private List<Component> _childComponents;


        public SceneEntity()
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            transform = new Transformation(this);

            _childComponents.Add(transform);
            tag = "default";
        }


        public SceneEntity(SceneEntity parent)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            transform = new Transformation(this);
            _childComponents.Add(transform);
            tag = "default";
            _parent = parent;
        }




        public void Traverse(ITraversalState traversal)
        {

            traversal.Push();
            foreach (var childComponent in _childComponents)
            {
                childComponent.Traverse(traversal);
            }

            foreach (var childSceneEntity in _childSceneEntities)
            {
                childSceneEntity.Traverse(traversal);
            }
            traversal.Pop();

        }

        public void AddComponent(Component component)
        {
            if (component is Renderer) // TODO sorting addition process
            {
                _childComponents.Add(component);
                renderer =(Renderer)component;
            }
            else
            {
                _childComponents.Add(component);  
            }
            //Console.WriteLine("The name of the added Component is " + type);
        }

        public void AddChild(SceneEntity child)
        {
            _childSceneEntities.Add(child);

        }


        public SceneEntity parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

    }
}

