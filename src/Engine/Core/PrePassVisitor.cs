﻿using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// This struct saves a light and all its parameters, as found by a Visitor.
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


    public struct CameraResult
    {
        public CameraComponent Camera { get; private set; }

        public float4x4 View { get; private set; }

        public CameraResult(CameraComponent cam, float4x4 view)
        {
            Camera = cam;
            View = view;
        }
    }

    internal class PrepassVisitorState : VisitorState
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
        /// Initializes a new instance of the <see cref="PrepassVisitorState"/> class.
        /// </summary>
        public PrepassVisitorState()
        {
            RegisterState(_model);
        }
    }

    internal class PrePassVisitor : SceneVisitor
    {
        private TransformComponent _currentTransform;
        public List<Tuple<SceneNodeContainer, LightResult>> LightPrepassResuls;
        public List<Tuple<SceneNodeContainer, CameraResult>> CameraPrepassResults;

        /// <summary>
        /// Holds the status of the model matrices and other information we need while traversing up and down the scene graph.
        /// </summary>
        private RendererState _state;

        private CanvasTransformComponent _ctc;
        private MinMaxRect _parentRect;       
        protected RenderContext _rc;
        private bool isCtcInitialized = false;

        public PrePassVisitor()
        {
            
            _state = new RendererState();
            LightPrepassResuls = new List<Tuple<SceneNodeContainer, LightResult>>();
            CameraPrepassResults = new List<Tuple<SceneNodeContainer, CameraResult>>();
        }

        public void PrePassTraverse(SceneContainer sc, RenderContext rc)
        {
            _rc = rc;
            Traverse(sc.Children);
        }

        protected override void InitState()
        {
            base.InitState();
            _state.Model = float4x4.Identity;
        }

        /// <summary>
        /// Sets the state of the model matrices and UiRects.
        /// </summary>
        /// <param name="ctc">The CanvasTransformComponent.</param>
        [VisitMethod]
        public void RenderCanvasTransform(CanvasTransformComponent ctc)
        {
            _ctc = ctc;

            if (ctc.CanvasRenderMode == CanvasRenderMode.WORLD)
            {
                var newRect = new MinMaxRect
                {
                    Min = ctc.Size.Min,
                    Max = ctc.Size.Max
                };

                _state.CanvasXForm *= float4x4.CreateTranslation(newRect.Center.x, newRect.Center.y, 0);
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;
            }

            if (ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                float aspect;
                float fov;
                float zNear;
                float width;
                float height;
                float3 canvasPos;

                var cam = CurrentNode.GetComponent<CameraComponent>();
                if (cam != null)
                {
                    width = (int)(_rc.ViewportWidth * (cam.Viewport.z / 100));
                    height = (int)(_rc.ViewportHeight * (cam.Viewport.w / 100));
                    zNear = cam.ClippingPlanes.x;
                    canvasPos = new float3(_rc.InvView.M14, _rc.InvView.M24, _rc.InvView.M34 + zNear);
                }
                else
                {
                    var projection = _rc.Projection;
                    zNear = System.Math.Abs(projection.M34 / (projection.M33 + 1));
                    fov = 2f * (float)System.Math.Atan(1f / projection.M22);
                    aspect = projection.M22 / projection.M11;
                    height = (float)(2f * System.Math.Tan(fov / 2f) * zNear);
                    width = height * aspect;

                    canvasPos = new float3(_rc.InvView.M14, _rc.InvView.M24, _rc.InvView.M34 + zNear);
                }

                ctc.ScreenSpaceSize = new MinMaxRect
                {
                    Min = new float2(canvasPos.x - width / 2, canvasPos.y - height / 2),
                    Max = new float2(canvasPos.x + width / 2, canvasPos.y + height / 2)
                };

                var newRect = new MinMaxRect
                {
                    Min = ctc.ScreenSpaceSize.Min,
                    Max = ctc.ScreenSpaceSize.Max
                };

                if (!isCtcInitialized)
                {
                    ctc.Scale = new float2(ctc.Size.Size.x / ctc.ScreenSpaceSize.Size.x,
                        ctc.Size.Size.y / ctc.ScreenSpaceSize.Size.y);

                    _ctc = ctc;
                    isCtcInitialized = true;

                }
                _state.CanvasXForm *= _rc.InvView * float4x4.CreateTranslation(0, 0, zNear + (zNear * 0.01f));
                _state.Model *= _state.CanvasXForm;

                _parentRect = newRect;
                _state.UiRect = newRect;
            }
        }

        /// <summary>
        /// If a RectTransformComponent is visited the model matrix and MinMaxRect get updated in the <see cref="RendererState"/>.
        /// </summary>
        /// <param name="rtc">The XFormComponent.</param>
        [VisitMethod]
        public void RenderRectTransform(RectTransformComponent rtc)
        {
            MinMaxRect newRect;
            if (_ctc.CanvasRenderMode == CanvasRenderMode.SCREEN)
            {
                newRect = new MinMaxRect
                {
                    Min = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Min + (rtc.Offsets.Min / _ctc.Scale.x),
                    Max = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Max + (rtc.Offsets.Max / _ctc.Scale.y)
                };
            }
            else
            {
                // The Heart of the UiRect calculation: Set anchor points relative to parent
                // rectangle and add absolute offsets
                newRect = new MinMaxRect
                {
                    Min = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Min + rtc.Offsets.Min,
                    Max = _state.UiRect.Min + _state.UiRect.Size * rtc.Anchors.Max + rtc.Offsets.Max
                };
            }

            var translationDelta = newRect.Center - _state.UiRect.Center;
            var translationX = translationDelta.x / _state.UiRect.Size.x;
            var translationY = translationDelta.y / _state.UiRect.Size.y;

            _parentRect = _state.UiRect;
            _state.UiRect = newRect;

            _state.Model *= float4x4.CreateTranslation(translationX, translationY, 0);
        }

        /// <summary>
        /// If a XFormComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormComponent.</param>
        [VisitMethod]
        public void RenderXForm(XFormComponent xfc)
        {
            float4x4 scale;

            if (_state.UiRect.Size != _parentRect.Size)
            {
                var scaleX = _state.UiRect.Size.x / _parentRect.Size.x;
                var scaleY = _state.UiRect.Size.y / _parentRect.Size.y;
                scale = float4x4.CreateScale(scaleX, scaleY, 1);
            }
            else if (_state.UiRect.Size == _parentRect.Size && xfc.Name.Contains("Canvas"))
                scale = float4x4.CreateScale(_state.UiRect.Size.x, _state.UiRect.Size.y, 1);
            else
                scale = float4x4.CreateScale(1, 1, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a XFormTextComponent is visited the model matrix gets updated in the <see cref="RendererState"/> and set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="xfc">The XFormTextComponent.</param>
        [VisitMethod]
        public void RenderXFormText(XFormTextComponent xfc)
        {
            var scaleX = 1 / _state.UiRect.Size.x * xfc.TextScaleFactor;
            var scaleY = 1 / _state.UiRect.Size.y * xfc.TextScaleFactor;
            var scale = float4x4.CreateScale(scaleX, scaleY, 1);

            _state.Model *= scale;
            _rc.Model = _state.Model;
        }

        /// <summary>
        /// If a TransformComponent is visited the model matrix of the <see cref="RenderContext"/> and <see cref="RendererState"/> is updated.
        /// It additionally updates the view matrix of the RenderContext.
        /// </summary> 
        /// <param name="transform">The TransformComponent.</param>
        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            _currentTransform = transform;
            _state.Model *= transform.Matrix();
            _rc.Model = _state.Model;
        }



        [VisitMethod]
        public void OnLight(LightComponent lightComponent)
        {
            var lightResult = new LightResult(lightComponent)
            {                
                Rotation = _state.Model.RotationComponent(),
                WorldSpacePos = new float3(_state.Model.M14, _state.Model.M24, _state.Model.M34)
            };

            LightPrepassResuls.Add(new Tuple<SceneNodeContainer, LightResult>(CurrentNode, lightResult));            
        }

        [VisitMethod]
        public void OnCamera(CameraComponent camComp)
        {
            float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f;
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 2, -10, 0, 2, 0, 0, 1, 0);
            var test = mtxCam * mtxRot;


            var view1 = float4x4.Invert(float4x4.CreateTranslation(_currentTransform.Translation) * float4x4.CreateRotationX(_currentTransform.Rotation.x) * float4x4.CreateRotationY(_currentTransform.Rotation.y));

            var view = _state.Model;

            var cameraResult = new CameraResult(camComp, view1);
            
            CameraPrepassResults.Add(new Tuple<SceneNodeContainer, CameraResult>(CurrentNode, cameraResult));
        }
    }
   
}