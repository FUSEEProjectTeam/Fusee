using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    public class ActionCode : Component
    {
     //TODO: Test without new calls  
        public SceneEntity SceneEntity = new SceneEntity();
        public Transformation transform = new Transformation();
        public Renderer renderer = new Renderer();

        protected Input Input;
        protected float DeltaTime;

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
            // Input = _traversalState.Input;
            // DeltaTime = _traversalState.DeltaTime;
            Update();
            Input = null;
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