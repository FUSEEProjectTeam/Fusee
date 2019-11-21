using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This struct saves a light and all its parameters, as found by a Viserator.
    /// </summary>
    public struct LightResult
    {
        /// <summary>
        /// The light component as present (1 to n times) in the scene graph.
        /// </summary>
        public LightComponent Light { get; private set; }

        /// <summary>
        /// It should be possible for one instance of type LightComponent to be used multiple times in the scene graph.
        /// Therefore the LightComponent itself has no position information - it gets set while traversing the scene graph.
        /// </summary>
        public float3 WorldSpacePos { get; set; }

        /// <summary>
        /// The rotation matrix. Determines the direction of the light, also set while traversing the scene graph.
        /// </summary>
        public float4x4 Rotation { get; set; }

        /// <summary>
        /// The session unique identifier of tis LightResult.
        /// </summary>
        public Suid Id;

        /// <summary>
        /// Creates a new instance of type LightResult.
        /// </summary>
        /// <param name="light">The LightComponent.</param>
        public LightResult(LightComponent light)
        {
            Light = light;
            WorldSpacePos = float3.Zero;
            Rotation = float4x4.Identity;
            Id = Suid.GenerateSuid();
        }       

        /// <summary>
        /// Override for the Equals method.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var lc = (LightResult)obj;
            return this.Id.Equals(lc.Id);
        }

        /// <summary>
        /// Override of the == operator.
        /// </summary>
        /// <param name="thisLc">The first LightResult that will be compared with a second one.</param>
        /// <param name="otherLc">The second LightResult that will be compared with the first one.</param>
        /// <returns></returns>
        public static bool operator ==(LightResult thisLc, LightResult otherLc)
        {
            return otherLc.Id.Equals(thisLc.Id);
        }

        /// <summary>
        /// Override of the != operator.
        /// </summary>
        /// <param name="thisLc">The first LightResult that will be compared with a second one.</param>
        /// <param name="otherLc">The second LightResult that will be compared with the first one.</param>
        /// <returns></returns>
        public static bool operator !=(LightResult thisLc, LightResult otherLc)
        {
            return !otherLc.Id.Equals(thisLc.Id);
        }

        /// <summary>
        /// Override of the GetHashCode method.
        /// Returns the session unique identifier as hash code.
        /// </summary>  
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }

    internal class LightViseratorState : VisitorState
    {
        private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// Gets and sets the top of the Model matrix stack. The Model matrix transforms model coordinates into world coordinates.
        /// </summary>
        /// <value>
        /// The Model matrix.
        /// </value>
        public float4x4 Model
        {
            set { _model.Tos = value; }
            get { return _model.Tos; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightViseratorState"/> class.
        /// </summary>
        public LightViseratorState()
        {
            RegisterState(_model);
        }
    }

    internal class LightViserator : Viserator<Tuple<SceneNodeContainer, LightResult>, LightViseratorState>
    {         
        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
        }

        [VisitMethod]
        public void OnTransform(TransformComponent xform)
        {
            State.Model *= xform.Matrix();
        }

        [VisitMethod]
        public void OnLight(LightComponent lightComponent)
        {
            var lightResult = new LightResult(lightComponent)
            {                
                Rotation = State.Model.RotationComponent(),
                WorldSpacePos = new float3(State.Model.M14, State.Model.M24, State.Model.M34)
            };

            YieldItem(new Tuple<SceneNodeContainer, LightResult>(CurrentNode, lightResult));
        }
    }
   
}