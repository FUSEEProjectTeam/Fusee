using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class SceneEntity
    {
        public string name;
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
            name = "defaultSceneEntity";
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



        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            
            foreach (var childComponent in _childComponents)
            {
                childComponent.Accept(sceneVisitorRendering);
            }

            foreach (var childSceneEntity in _childSceneEntities)
            {
                childSceneEntity.Accept(sceneVisitorRendering);
            }
        }


        public void Accept(SceneVisitor sv)
        {
            sv.Visit(this);    
        }

        public void AddComponent(Component component)
        {
            if (component is Renderer) // TODO sorting addition process
            {
                if(renderer==null)
                {
                    _childComponents.Add(component);
                    renderer = (Renderer)component;  
                }else
                {
                    Debug.WriteLine("A SceneEntity can only have one RendererComponent at a time.");
                    //throw new Exception("A SceneEntity can only have one RendererComponent at a time.");
                }
                
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
        // TODO: Add Find Functions, Add GetComponent/s in Children
        public SceneEntity[] GetChildren()
        {
            return _childSceneEntities.ToArray();
        }

        public T GetComponent<T>()
        {
            foreach (var childComponent in _childComponents)
            {
                if(childComponent.GetType() == typeof(T))
                {
                    return (T)(object)childComponent;
                }
            }
            return (T)(object)null;
        }

        
        public T[] GetComponents<T>()
        {
            List<T> componentItems = new List<T>();
            foreach (var childComponent in _childComponents)
            {
                if (childComponent.GetType() == typeof(T))
                {
                    componentItems.Add((T)(object)childComponent);
                }
            }
            return componentItems.ToArray();
        }

        
        public  static SceneEntity FindSceneEntity(string sceneEntityName)
        {
            return SceneManager.Manager.FindSceneEntity(sceneEntityName);
        }

        public SceneEntity parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public static void DestroySceneEntity(SceneEntity objectToBeDestroyed)
        {
               
        }

    }
}

