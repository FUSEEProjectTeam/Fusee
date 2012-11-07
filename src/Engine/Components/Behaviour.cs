using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneManagement
{
    public class Behaviour : Component
    {
        public GameEntity gameEntity;
        public Transformation transform;
        public Renderer renderer;
        public Behaviour(TraversalState _traversalState)
        {
            gameEntity = _traversalState.Owner;
            transform = gameEntity.transform;
            renderer = gameEntity.renderer;
            Start();
        }

        public Behaviour()
        {
            // TODO: Complete member initialization
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
    }
}
