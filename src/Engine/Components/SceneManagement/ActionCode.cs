using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.SceneManagement
{
    public class ActionCode : Component
    {
       
        public SceneEntity SceneEntity = new SceneEntity();
        public Transformation transform = new Transformation();
        public Renderer renderer = new Renderer();

        public void Init(SceneEntity entity)
        {
            SceneEntity = entity;
            transform = SceneEntity.transform;
            renderer = SceneEntity.renderer;
        }
        virtual public void Start()
        {
            
        }

        virtual public void Update()
        {
            
        }

        public override void Traverse(ITraversalState _traversalState)
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