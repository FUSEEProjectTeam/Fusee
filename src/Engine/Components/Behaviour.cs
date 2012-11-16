using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneManagement
{
    public class Behaviour : Component
    {
        private readonly int _id = 1;
        public GameEntity gameEntity;
        public Transformation transform;
        public Renderer renderer;

        public Behaviour()
        {
            // TODO: Complete member initialization
        }
        
        public Behaviour(TraversalState _traversalState)
        {
            gameEntity = _traversalState.Owner;
            transform = gameEntity.transform;
            renderer = gameEntity.renderer;
            Start();
        }





        virtual public void Start()
        {
            
        }

        virtual public void Update()
        {
            
        }

        public override void Traverse(TraversalState _traversalState)
        {
            Update();
        }
        public override int GETID()
        {
            return _id;
        }
    }
}
