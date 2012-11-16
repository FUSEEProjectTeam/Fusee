using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
   public class Component
    {
       
       private SceneEntity _sceneEntity;

       #region Constructors
       public Component()
        {
            
        }

       public Component(SceneEntity sceneEntity)
       {
          _sceneEntity = sceneEntity;
       }

       #endregion; 

       public SceneEntity SceneEntity
       {
           get { return _sceneEntity; }
           set { _sceneEntity = value; }
       }

       virtual public void Traverse(ITraversalState _traversalState)
       {
           
       }

    }
}
