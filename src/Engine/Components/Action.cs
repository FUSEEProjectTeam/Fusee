using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneManagement
{
    public class Action : Component
    {
       
        public SceneEntity SceneEntity = new SceneEntity();
        public Transformation transform = new Transformation();
        public Renderer renderer = new Renderer();


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

/*
      virtual public void Update()
        {
            
        }

        override public void Traverse(ITraversalState traversal)
        {
            Update();
        }   

*/