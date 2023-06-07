using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Visitor is inside <see cref="SceneRendererForward"/> before rendering the scene.
    /// Collects <see cref="Light"/>s and <see cref="Camera"/>s for rendering, picking, etc.
    /// </summary>
    public class PrePassVisitor : Visitor<SceneNode, SceneComponent>
    {
        /// <summary>
        /// Collection of <see cref="LightResult"/>s found while traversing the scene.
        /// </summary>
        public List<LightResult> LightPrepassResults { get; private set; }

        /// <summary>
        /// Collection of <see cref="CameraResult"/>s found while traversing the scene.
        /// </summary>
        public List<CameraResult> CameraPrepassResults { get; private set; }

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        private readonly RendererState _state;
        private int _currentLight;

        /// <summary>
        /// <see cref="Visitor{TNode, TComponent}"/> which traverses the scene and yields <see cref="CameraPrepassResults"/> and <see cref="LightPrepassResults"/>.
        /// </summary>
        public PrePassVisitor()
        {
            _state = new RendererState();
            IgnoreInactiveComponents = true;
            LightPrepassResults = new List<LightResult>();
            CameraPrepassResults = new List<CameraResult>();
        }

        /// <summary>
        /// Call this method to initialize the traversal process.
        /// </summary>
        /// <param name="sc">The <see cref="SceneContainer"/> to traverse.</param>
        public void PrePassTraverse(SceneContainer sc)
        {
            _currentLight = 0;
            CameraPrepassResults.Clear();
            LightPrepassResults.Clear();
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

        /// <summary>
        /// Called when a <see cref="Light"/> is found during traversal, add current position to <see cref="LightPrepassResults"/>.
        /// This is necessary, as a <see cref="Light"/> can be positioned arbitrarily inside a <see cref="SceneContainer"/>.
        /// However, the resulting lighting affects all <see cref="Mesh"/>s, therefore we need to collect everything before rendering.
        /// </summary>
        /// <param name="lightComponent"></param>
        [VisitMethod]
        public void OnLight(Light lightComponent)
        {
            if (LightPrepassResults.Count - 1 < _currentLight)
            {
                var lightResult = new LightResult(lightComponent)
                {
                    Rotation = _state.Model.RotationComponent(),
                    WorldSpacePos = new float3(_state.Model.M14, _state.Model.M24, _state.Model.M34)
                };

                LightPrepassResults.Add(lightResult);
            }
            else
            {
                var currentRes = LightPrepassResults[_currentLight];
                currentRes.Rotation = _state.Model.RotationComponent();
                currentRes.WorldSpacePos = new float3(_state.Model.M14, _state.Model.M24, _state.Model.M34);
            }
            _currentLight++;
        }

        /// <summary>
        /// Called when a camera is found inside the <see cref="SceneContainer"/>
        /// This is done inside this pre-pass visitor, as the camera can be anywhere inside the container, however we want to render the whole container
        /// independent of the camera position inside the scene graph.
        /// </summary>
        /// <param name="camComp"></param>
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

            CameraPrepassResults.Add(new CameraResult(camComp, view.Invert()));
        }
    }
}