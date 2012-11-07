using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    public class GameEntity : FuseeObject
    {
        public TraversalState _traversalState; // TODO: Make this private again >>currently just a Hack
        private FuseeObject _sceneManager;
        private List<GameEntity> _childGameEntities;
        private List<Component> _childComponents; 
        public string tag;
        public Transformation transform;
        public Renderer renderer;
        
        public GameEntity(FuseeObject sceneManager)
        {
            _sceneManager = sceneManager;
            _RenderQueue = sceneManager._RenderQueue;
            _childComponents = new List<Component>();
            _childGameEntities = new List<GameEntity>();
            transform = new Transformation();
            _traversalState = new TraversalState(transform.Matrix,this);
            _childComponents.Add(transform);
            tag = "default";
        }

        public void Traverse(TraversalState traversal)
        {
            _traversalState.Matrix *= traversal.Matrix;
 
            foreach (var childComponent in _childComponents)
            {
                childComponent.Traverse(_traversalState);
            }
            foreach (var childGameEntity in _childGameEntities)
            {
                childGameEntity.Traverse(_traversalState);
            }

            if(transform!=null && renderer!=null)
            {
                //Console.WriteLine("Drawing Object at "+transform.LocalPosition.ToString()+" LocalPosition "+"And at "+transform.WorldMatrix.ToString()+" World");
                _RenderQueue.AddDrawCall(new DrawCall(transform,renderer,renderer.mesh));
            }
            
        }

        public void AddComponent(Component component)
        {
            int id = component.GETID();
            //Console.WriteLine("The name of the added Component is " + type);
            switch (id)
            {
                case 1:
                    _childComponents.Add(component);
                    break;
                case 2:
                    _childComponents.Add(component as Transformation);
                    break;
                case 3:
                    _childComponents.Add(component as Renderer);
                    if (renderer == null)
                    {
                        renderer = component as Renderer;
                    }
                    break;
                default:
                    Console.WriteLine("The name of the added Component is "+component.GetType().Name);
                    break;
            }
        }

        public void AddChild(GameEntity child)
        {
            _childGameEntities.Add(child);
            
        }
        
         
    }
}
