using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// The SceneEntity class can contain components that do different tasks during runtime in order to create a scene. Every SceneEntity automatically has a transform component which is used for the 3D orientation. 
    /// It is required to give a SceneEntity a name in order to use the search algorithms of <see cref="SceneManager"/> during runtime.
    /// </summary>
    public class SceneEntity
    {
        #region Fields
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
        #endregion

        #region Constructors
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
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="action">The action.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="material">The material.</param>
        /// <param name="_renderer">The _renderer.</param>
        public SceneEntity(string _name, ActionCode action, SceneEntity parent, Material material, Renderer _renderer)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            name = _name;
            transform = new Transformation(this);
            _childComponents.Add(transform);
            _childComponents.Add(action);
            action.Init(this);
            AddComponent(_renderer);
            renderer.material = material;
            tag = "default";
            _parent = parent;
            _parent.AddChild(this);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="action">The action.</param>
        /// <param name="material">The material.</param>
        /// <param name="_renderer">The _renderer.</param>
        public SceneEntity(string _name, ActionCode action, Material material, Renderer _renderer)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            name = _name;
            transform = new Transformation(this);
            _childComponents.Add(transform);
            _childComponents.Add(action);
            action.Init(this);
            AddComponent(_renderer);
            renderer.material = material;
            tag = "default";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="material">The material.</param>
        /// <param name="_renderer">The _renderer.</param>
        public SceneEntity(string _name, Material material, Renderer _renderer)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            name = _name;
            transform = new Transformation(this);
            _childComponents.Add(transform);
            AddComponent(_renderer);
            renderer.material = material;
            tag = "default";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="action">The action.</param>
        /// <param name="parent">The parent.</param>
        public SceneEntity(string _name, ActionCode action, SceneEntity parent)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            name = _name;
            transform = new Transformation(this);
            _childComponents.Add(transform);
            _childComponents.Add(action);
            action.Init(this);
            tag = "default";
            _parent = parent;
            _parent.AddChild(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEntity"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="action">The action.</param>
        public SceneEntity(string _name, ActionCode action)
        {
            _childComponents = new List<Component>();
            _childSceneEntities = new List<SceneEntity>();
            name = _name;
            transform = new Transformation(this);
            _childComponents.Add(transform);
            _childComponents.Add(action);
            action.Init(this);
            tag = "default";
        }
        #endregion

        #region Members
        /// <summary>
        /// Traverses through the current SceneEntities components and through its children and their components.
        /// </summary>
        /// <param name="sceneVisitorRendering">The scene visitor rendering that grabs the informations from the SceneEntity that are required to render it.</param>
        public void TraverseForRendering(SceneVisitor sceneVisitorRendering)
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
        /// Accepts the specified SceneVisitor in order to pass informations to it.
        /// </summary>
        /// <param name="sv">The SceneVisitor.</param>
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
                    throw new Exception("A SceneEntity can only have one RendererComponent at a time.");
                }

            }
            else if (component is Light)
            {
                component.SceneEntity = this;
                _childComponents.Add(component);
            }
            else
            {
                _childComponents.Add(component);
            }
        }

        /// <summary>
        /// Adds a child SceneEntity to the current SceneEntity. 
        /// This method creates a relation between SceneEntities that is required for the transformation order.
        /// </summary>
        /// <param name="child">The child of type SceneEntity.</param>
        public void AddChild(SceneEntity child)
        {
            child.parent = this;
            _childSceneEntities.Add(child);
        }
        // TODO: Add Find Functions, Add GetComponent/s in Children
        /// <summary>
        /// Gets all child SceneEntities of the current SceneEntity. 
        /// If no children are attached to this SceneEntity null is returned.
        /// </summary>
        /// <returns>All child component of this <see cref="SceneEntity"/>.</returns>
        public SceneEntity[] GetChildren()
        {
            return _childSceneEntities.ToArray();
        }

        /// <summary>
        /// Gets the specified type of Component from the SceneEntity.
        /// </summary>
        /// <typeparam name="T">The specified type. This has to be a derivate of the <see cref="Component"/> class.</typeparam>
        /// <returns>The first component of the specified type if one is found, otherwise null.</returns>
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
        /// Gets all components of specified type from this current SceneEntity.
        /// </summary>
        /// <typeparam name="T">This is the type of the Component. The type in this case can be any derivate of the Component class, e.g. <see cref="Renderer"/></typeparam>
        /// <returns>On successful search a list of Components of specified type is returned.</returns>
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
        /// Finds a SceneEntity in the scene. This method is an access point to FindSceneEntity method inside <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="sceneEntityName">Name of the scene entity.</param>
        /// <returns>On successful search the first SceneEntity with the selected name is returned.</returns>
        public  static SceneEntity FindSceneEntity(string sceneEntityName)
        {
            return SceneManager.Manager.FindSceneEntity(sceneEntityName);
        }

        /// <summary>
        /// Gets or sets the parent SceneEntity. Null is returned if this SceneEntity doesn't have a parent.
        /// </summary>
        /// <value>
        /// The parent. 
        /// </value>
        public SceneEntity parent
        {
            get { return _parent; }
            set {
                    transform.Parent = value;
                    _parent = value; 
                }
        }
        #endregion

    }
}

