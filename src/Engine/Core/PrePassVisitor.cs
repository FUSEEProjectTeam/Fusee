using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    internal class PrePassVisitor : Visitor<SceneNode, SceneComponent>
    {
        public List<Tuple<SceneNode, LightResult>> LightPrepassResuls;
        public List<Tuple<SceneNode, CameraResult>> CameraPrepassResults;

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        private readonly RendererState _state;
        private int _currentLight;

        public PrePassVisitor()
        {
            _state = new RendererState();
            IgnoreInactiveComponents = true;
            LightPrepassResuls = new List<Tuple<SceneNode, LightResult>>();
            CameraPrepassResults = new List<Tuple<SceneNode, CameraResult>>();
        }

        public void PrePassTraverse(SceneContainer sc)
        {
            _currentLight = 0;
            CameraPrepassResults.Clear();
            Traverse(sc.Children);
        }

        /// <summary>
        /// Sets the initial values in the <see cref="RendererState"/>.
        /// </summary>
        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _state.CanvasXForm = float4x4.Identity;
        }

        /// <summary>
        /// Pushes into the RenderState.
        /// </summary>
        protected override void PushState()
        {
            _state.Push();
        }

        /// <summary>
        /// Pops from the RenderState and sets the Model and View matrices in the RenderContext.
        /// </summary>
        protected override void PopState()
        {
            _state.Pop();
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(Transform transform)
        {
            _state.Model *= transform.Matrix;
        }

        [VisitMethod]
        public void OnLight(Light lightComponent)
        {
            if (LightPrepassResuls.Count - 1 < _currentLight)
            {
                var lightResult = new LightResult(lightComponent)
                {
                    Rotation = _state.Model.RotationComponent(),
                    WorldSpacePos = new float3(_state.Model.M14, _state.Model.M24, _state.Model.M34)
                };

                LightPrepassResuls.Add(new Tuple<SceneNode, LightResult>(CurrentNode, lightResult));
            }
            else
            {
                var currentRes = LightPrepassResuls[_currentLight];
                currentRes.Item2.Rotation = _state.Model.RotationComponent();
                currentRes.Item2.WorldSpacePos = new float3(_state.Model.M14, _state.Model.M24, _state.Model.M34);
            }
            _currentLight++;
        }

        [VisitMethod]
        public void OnCamera(Camera camComp)
        {
            var view = _state.Model;
            var scale = float4x4.GetScale(view);

            if (scale.x != 1)
            {
                view.M11 /= scale.x;
                view.M21 /= scale.x;
                view.M31 /= scale.x;
            }

            if (scale.y != 1)
            {
                view.M12 /= scale.y;
                view.M22 /= scale.y;
                view.M32 /= scale.y;
            }

            if (scale.z != 1)
            {
                view.M13 /= scale.z;
                view.M23 /= scale.z;
                view.M33 /= scale.z;
            }

            var cameraResult = new CameraResult(camComp, view.Invert());
            CameraPrepassResults.Add(new Tuple<SceneNode, CameraResult>(CurrentNode, cameraResult));
        }
    }
}