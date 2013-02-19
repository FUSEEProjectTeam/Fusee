using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class SceneVisitorRendering : SceneVisitor
    {
        private Stack<bool> _hasTransform;
        private Stack<bool> _hasRenderer;
        private Stack<bool> _hasMesh;
        private SceneManager _queue;
        private Stack<float4x4> _mtxModelViewStack;
        private Stack<Mesh> _meshStack;
        private Stack<Renderer> _RendererStack;
        private double _deltaTime;
        private Input _input;



        public void SetDeltaTime(double delta)
        {
            _deltaTime = delta;
        }




        public Input Input
        {
            set { _input = value; }
            get
            {
                if (_input != null)
                {
                    return _input;
                }else
                {
                    return null;
                }
            }
        }

        public void GetDeltaTime(out double deltaTime)
        {
            deltaTime = _deltaTime;
        }


        public SceneVisitorRendering(SceneManager queue)
        {
            _queue = queue;
            
            _mtxModelViewStack = new Stack<float4x4>();
            _meshStack = new Stack<Mesh>();
            _RendererStack = new Stack<Renderer>();
            _hasTransform = new Stack<bool>();
            _hasRenderer = new Stack<bool>();
            _hasMesh = new Stack<bool>();
            _mtxModelViewStack.Push(float4x4.Identity);
            PrepareDoubleDispatch();
            /*
            
            _meshStack.Push(null);
            _RendererStack.Push(null);
            */
        }
        // TODO: Store Mesh as local variable instead of stacks as it is not used in further traversals.
        private void StoreMesh(Mesh mesh)
        {
            
            _hasMesh.Pop();
            _meshStack.Push(mesh);
            _hasMesh.Push(true);
            
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }

        private void StoreRenderer(Renderer Renderer)
        {

                _hasRenderer.Pop();

            _RendererStack.Push(Renderer);
            _hasRenderer.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }



        private void AddTransform(float4x4 mtx)
        {
            _hasTransform.Pop();
                _mtxModelViewStack.Push(mtx * _mtxModelViewStack.Pop());

            
            _hasTransform.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }
        }

        private bool HasRenderingTriple()
        {
            return (_hasMesh.Peek() && _hasRenderer.Peek() && _hasTransform.Peek());
        }



        private void Push()
        {

                _mtxModelViewStack.Push(_mtxModelViewStack.Peek());

            
            _hasMesh.Push(false);
            _hasRenderer.Push(false);
            _hasTransform.Push(false);
        }

        private void Pop()
        {
            _mtxModelViewStack.Pop();
            _hasTransform.Pop();
            if(_meshStack.Count > 0)
            {
                _meshStack.Pop();
                _hasMesh.Pop();  
            }
            
            if(_RendererStack.Count > 0)
            {
                _RendererStack.Pop();
                _hasRenderer.Pop();
            }
            
            
        }

        public void AddLightDirectional(float3 direction, float4 color, Light.LightType type, int channel) 
        {
            RenderDirectionalLight light = new RenderDirectionalLight(direction, color, type, channel );
            _queue.AddLightJob(light);
        }

        public void AddLightPoint(float3 position, float4 color, Light.LightType type, int channel) 
        {
            RenderPointLight light = new RenderPointLight(position, color, type, channel );
            _queue.AddLightJob(light);
        }

        public void AddLightSpot(float3 position, float3 direction, float4 color, Light.LightType type, int channel) 
        {
            RenderSpotLight light = new RenderSpotLight(position, direction, color, type, channel );
            _queue.AddLightJob(light);
        }

        public void AddRenderJob(float4x4 matrix, Mesh mesh, Renderer renderer)
        {
            //Console.WriteLine("_meshstack"+_meshStack.Count+"_viewstack"+_mtxModelViewStack.Count+"_renderstack"+_RendererStack.Count);
            //Console.WriteLine("_hasTransform+"+_hasTransform.Count+"_hasMesh+"+_hasMesh.Count+"_hasRenderer"+_hasRenderer.Count);
            RenderMatrix renderMatrix = new RenderMatrix(matrix);
            _queue.AddRenderJob(renderMatrix);
            RenderMesh renderMesh = new RenderMesh(mesh);
            _queue.AddRenderJob(renderMesh);
            RenderRenderer renderRenderer = new RenderRenderer(renderer);
            _queue.AddRenderJob(renderRenderer);
        }


        // Polymorphic Visitcomponent Methods
        public override void Visit(SceneEntity cEntity)
        {
            Push();
            cEntity.TraverseForRendering(this);
            Pop();
        }
        private void VisitComponent(ActionCode actionCode)
        {
            actionCode.TraverseForRendering(this);
        }
        private void VisitComponent(Renderer renderer)
        {
            StoreMesh(renderer.mesh);
            StoreRenderer(renderer);
        }
        private void VisitComponent(Transformation transform)
        {
            AddTransform(transform.Matrix);
        }
        private void VisitComponent(DirectionalLight directionalLight)
        {
            directionalLight.TraverseForRendering(this);
        }
        private void VisitComponent(PointLight pointLight)
        {
            pointLight.TraverseForRendering(this);
        }
        private void VisitComponent(SpotLight spotLight)
        {
            spotLight.TraverseForRendering(this);
        }
        private void VisitComponent(Camera camera)
        {
            
            if (_mtxModelViewStack.Peek() != null)
            {
                camera.ViewMatrix = _mtxModelViewStack.Peek();
                _queue.AddCamera(camera.SubmitWork());
            }
        }
        
    }
}
