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
       /// <summary>
       /// Initializes a new instance of the <see cref="Component"/> class. Whitout setting a SceneEntity.
       /// </summary>
       public Component()
        {
            
        }

       /// <summary>
       /// Initializes a new instance of the <see cref="Component" /> class with a SceneEntity.
       /// </summary>
       /// <param name="sceneEntity">The owner scene entity.</param>
       public Component(SceneEntity sceneEntity)
       {
          _sceneEntity = sceneEntity;
       }

       #endregion; 

       #region Public Members
       /// <summary>
       /// Gets or sets the scene entity of this Component.
       /// </summary>
       public SceneEntity SceneEntity
       {
           get { return _sceneEntity; }
           set { _sceneEntity = value; }
       }
       /// <summary>
       /// Passes the Component to the SceneVisitor which decides what to do with that Component.
       /// </summary>
       /// <param name="sv">The sv.</param>
       virtual public void Accept(SceneVisitor sv)
       {
           //sv.Visit(this);
       }
       #endregion
   }
}
