using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// 
    /// </summary>
    public class SceneEntity
    {
        /// <summary>
        /// The name of the SceneEntity.
        /// </summary>
        public string name;
        /// <summary>
        /// The tag of the SceneEntity.
        /// </summary>
        public string tag;
        /// <summary>
        /// The transform Component of the SceneEntity.
        /// </summary>
        public Transformation transform;
        /// <summary>
        /// The renderer Component of the SceneEntity.
        /// </summary>
        public Renderer renderer;
        /// <summary>
        /// The _parent SceneEntity of the current SceneEntity.
        /// </summary>
        private SceneEntity _parent;
        /// <summary>
        /// The _child scene entities of the current SceneEntity.
        /// </summary>
        private List<SceneEntity> _childSceneEntities;
        /// <summary>
        /// The components of the current SceneEntity. 
        /// </summary>
        private List<Component> _childComponents;


        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        public SceneEntity()
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            transform = new Transformation(this);

            _childComponents.Add(transform);
            name = "defaultSceneEntity";
            tag = "default";
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="parent">The parent SceneEntity.</param>
        public SceneEntity(SceneEntity parent)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            transform = new Transformation(this);
            _childComponents.Add(transform);
            tag = "default";
            _parent = parent;
        }



        /// <summary>
        /// Traverses through the current SceneEntities components and through its children and their components.
        /// </summary>
        /// <param name="sceneVisitorRendering">The scene visitor rendering.</param>
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


        /// <summary>
        /// Accepts the specified SceneVisitor.
        /// </summary>
        /// <param name="sv">The sv.</param>
        public void Accept(SceneVisitor sv)
        {
            sv.Visit(this);    
        }

        /// <summary>
        /// Adds a component to the SceneEntity.
        /// </summary>
        /// <param name="component">The component.</param>
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

        /// <summary>
        /// Adds a child SceneEntity to the current SceneEntity.
        /// </summary>
        /// <param name="child">The child.</param>
        public void AddChild(SceneEntity child)
        {
            _childSceneEntities.Add(child);
        }
        // TODO: Add Find Functions, Add GetComponent/s in Children
        /// <summary>
        /// Gets the child SceneEntities of the current SceneEntity.
        /// </summary>
        /// <returns></returns>
        public SceneEntity[] GetChildren()
        {
            return _childSceneEntities.ToArray();
        }

        /// <summary>
        /// Gets the specified component from the SceneEntity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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


        /// <summary>
        /// Gets all specified components of the current SceneEntity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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


        /// <summary>
        /// Finds a SceneEntity in the scene.
        /// </summary>
        /// <param name="sceneEntityName">Name of the scene entity.</param>
        /// <returns></returns>
        public  static SceneEntity FindSceneEntity(string sceneEntityName)
        {
            return SceneManager.Manager.FindSceneEntity(sceneEntityName);
        }

        /// <summary>
        /// Gets or sets the parent SceneEntity.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public SceneEntity parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Removes the scene entity and all of its children from the scene.
        /// </summary>
        /// <param name="objectToBeDestroyed">The object to be destroyed.</param>
        public static void DestroySceneEntity(SceneEntity objectToBeDestroyed)
        {
               
        }

    }
}

