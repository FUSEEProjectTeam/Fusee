using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Initializes all objects of type <see cref="ActionCode"/> in the scene by calling their Start methods.
    /// </summary>
    class SceneVisitorStartAction : SceneVisitor
    {
        #region Overrides
        /// <summary>
        /// Visits the specified <see cref="SceneEntity"/> to gather data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="cEntity">The SceneEntity instance.</param>
        public override void Visit(SceneEntity cEntity)
        {
            cEntity.TraverseForRendering(this);
        }

        /// <summary>
        /// Visits the specified <see cref="ActionCode"/> to call it's Start method for initialization.
        /// </summary>
        /// <param name="actionCode">The ActionCode instance.</param>
        public override void Visit(ActionCode actionCode)
        {
            actionCode.Start();
        }
        #endregion
    }
}
