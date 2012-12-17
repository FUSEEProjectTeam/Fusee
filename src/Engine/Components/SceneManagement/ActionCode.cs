using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    public class ActionCode : Component
    {
     //TODO: Test without new calls  
        public SceneEntity SceneEntity;
        public Transformation transform;
        public Renderer renderer;

        protected Input Input;
        protected double DeltaTime;

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
            _traversalState.GetInput(out Input);
            _traversalState.GetDeltaTime(out DeltaTime);
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