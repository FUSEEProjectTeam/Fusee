using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// All supported lightning calculation methods LegacyShaderCodeBuilder.cs supports.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public enum LightingCalculationMethod
    {
        /// <summary> 
        /// Simple Blinn Phong Shading without fresnel and distribution function
        /// </summary>
        SIMPLE,

        /// <summary>
        /// Physical based shading
        /// </summary>
        ADVANCED,

        /// <summary>
        /// Physical based shading with environment cube map algorithm
        /// </summary>
        ADVANCEDENVMAP
    }

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

        public Suid Id;

        public LightResult(LightComponent light)
        {
            Light = light;
            WorldSpacePos = float3.Zero;
            Rotation = float4x4.Identity;
            Id = Suid.GenerateSuid();
        }       

        public override bool Equals(object obj)
        {
            var lc = (LightResult)obj;
            return this.Id.Equals(lc.Id);
        }

        public static bool operator ==(LightResult thisLc, LightResult otherLc)
        {
            return otherLc.Id.Equals(thisLc.Id);
        }


        public static bool operator !=(LightResult thisLc, LightResult otherLc)
        {
            return !otherLc.Id.Equals(thisLc.Id);
        }

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