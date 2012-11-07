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
        private List<GameEntity> _gameEntities;
        public FuseeObject()
        {
            
            _gameEntities = new List<GameEntity>();
        }
        public FuseeObject(RenderQueue render)
        {
            _RenderQueue = render;
            _gameEntities = new List<GameEntity>();
        }
        virtual public void Traverse()
        {
            foreach (var gameEntity in _gameEntities)
            {
                gameEntity.Traverse(_traversalState);
            }
        }

       virtual public void Instantiate(GameEntity entity)
       {
           _gameEntities.Add(entity);
       }
    }


}
