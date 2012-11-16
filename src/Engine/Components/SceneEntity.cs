using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    public class SceneEntity
    {
        public TraversalState _traversalState; // TODO: Make this private again >>currently just a Hack
        private FuseeObject _sceneManager;
        private List<SceneEntity> _childGameEntities;
        private List<Component> _childComponents; 
        public string tag;
        public Transformation transform;
        public Renderer renderer;
        private SceneEntity _parent;


        public SceneEntity()
        {
            _childComponents = new List<Component>();
            _childGameEntities = new List<SceneEntity>();
            transform = new Transformation();
            _traversalState = new TraversalState(transform.Matrix,this);
            _childComponents.Add(transform);
            tag = "default";
        }


        public SceneEntity(SceneEntity parent)
        {
            _childComponents = new List<Component>();
            _childGameEntities = new List<SceneEntity>();
            transform = new Transformation();
            _traversalState = new TraversalState(transform.Matrix, this);
            _childComponents.Add(transform);
            tag = "default";
            _parent = parent;
        }




        public void Traverse(TraversalState traversal)
        {
            _traversalState.Matrix *= traversal.Matrix;
 
            foreach (var childComponent in _childComponents)
            {
                childComponent.Traverse(_traversalState);
            }

            foreach (var childSceneEntity in _childGameEntities)
            {
                childSceneEntity.Traverse(_traversalState);
            }

        }

        public void AddComponent(Component component)
        {
            if (component is Action)
            {
                

            }
            _childComponents.Add(component);

           
            //Console.WriteLine("The name of the added Component is " + type);

        }

        public void AddChild(SceneEntity child)
        {
            _childGameEntities.Add(child);
            
        }
        
        public void Log(string text)
        {
           Console.WriteLine(text+transform.WorldMatrix); 
        }

        public SceneEntity parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
         
    }
}


/*
  public class SceneEntity
    {
        public SceneEntity()
        {
        }

        public SceneEntity(SceneEntity parent)
        {
            _parent = parent;
        }

        private SceneEntity _parent;
 * 
	    public SceneEntity Parent
	    {
		    get { return _parent;}
		    set { _parent = value;}
	    }
	
        private List<SceneEntity> _childSceneEntities;
        private List<Component> _childComponents;

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
    }
 */