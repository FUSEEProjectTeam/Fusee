using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Base class from which all Components are derived.
    /// </summary>
   public class Component
   {
       #region Private Members
       private SceneEntity _sceneEntity;
       #endregion

       #region Constructors
       public Component()
        {
            
        }

       public Component(SceneEntity sceneEntity)
       {
          _sceneEntity = sceneEntity;
       }

       #endregion; 

       #region Public Members
       public SceneEntity SceneEntity
       {
           get { return _sceneEntity; }
           set { _sceneEntity = value; }
       }

       virtual public void Traverse(ITraversalState _traversalState)
       {

       }
       #endregion
   }
}
