using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
   public class FuseeObject
   {
        public RenderQueue _RenderQueue;
        private TraversalState _traversalState = new TraversalState(float4x4.Identity);
        private List<SceneEntity> _gameEntities;
        public FuseeObject()
        {
            
            _gameEntities = new List<SceneEntity>();
        }
        public FuseeObject(RenderQueue render)
        {
            _RenderQueue = render;
            _gameEntities = new List<SceneEntity>();
        }
        virtual public void Traverse()
        {
            foreach (var SceneEntity in _gameEntities)
            {
                SceneEntity.Traverse(_traversalState);
            }
        }

       virtual public void Instantiate(SceneEntity entity)
       {
           _gameEntities.Add(entity);
       }
    }


}
