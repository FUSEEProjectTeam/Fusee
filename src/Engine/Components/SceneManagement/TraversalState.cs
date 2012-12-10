using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;
using Fusee.Math;


namespace Fusee.SceneManagement
{
    public class TraversalStateRender : ITraversalState
    {
        private Stack<bool> _hasTransform;
        private Stack<bool> _hasRenderer;
        private Stack<bool> _hasMesh;
        private SceneManager _queue;
        private Stack<float4x4> _mtxModelViewStack;
        private Stack<Mesh> _meshStack;
        private Stack<Renderer> _RendererStack;

        public TraversalStateRender(SceneManager queue)
        {
            _queue = queue;
            _mtxModelViewStack = new Stack<float4x4>();
            _meshStack = new Stack<Mesh>();
            _RendererStack = new Stack<Renderer>();
            _hasTransform = new Stack<bool>();
            _hasRenderer = new Stack<bool>();
            _hasMesh = new Stack<bool>();
            _mtxModelViewStack.Push(float4x4.Identity);
            /*
            
            _meshStack.Push(null);
            _RendererStack.Push(null);
            */
        }
        // TODO: Store Mesh as local variable instead of stacks as it is not used in further traversals.
        public void StoreMesh(Mesh mesh)
        {
            _hasMesh.Pop();
            _meshStack.Push(mesh);
            _hasMesh.Push(true);
            
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }

        public void StoreRenderer(Renderer Renderer)
        {

                _hasRenderer.Pop();

            _RendererStack.Push(Renderer);
            _hasRenderer.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }

        public void AddTransform(float4x4 mtx)
        {
            _hasTransform.Pop();
                _mtxModelViewStack.Push(mtx * _mtxModelViewStack.Pop());

            
            _hasTransform.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }
        }

        public bool HasRenderingTriple()
        {
            return (_hasMesh.Peek() && _hasRenderer.Peek() && _hasTransform.Peek());
        }



        public void Push()
        {

                _mtxModelViewStack.Push(_mtxModelViewStack.Peek());

            
            _hasMesh.Push(false);
            _hasRenderer.Push(false);
            _hasTransform.Push(false);
        }

        public void Pop()
        {
            _mtxModelViewStack.Pop();
            _meshStack.Pop();
            _RendererStack.Pop();
            _hasMesh.Pop();
            _hasRenderer.Pop();
            _hasTransform.Pop();
        }

        public void addLightDirectional(float3 direction, float4 color) 
        {
            RenderDirectionalLight light = new RenderDirectionalLight(direction, color);
            _queue.AddLightJob(light);
        }

        public void addLightPoint(float3 position, float4 color) 
        {
            RenderPointLight light = new RenderPointLight(position, color);
            _queue.AddLightJob(light);
        }

        public void addLightSpot(float3 position, float3 direction, float4 color) 
        {
            RenderSpotLight light = new RenderSpotLight(position, direction, color);
            _queue.AddLightJob(light);
        }

        private void AddRenderJob(float4x4 matrix, Mesh mesh, Renderer renderer)
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


    }
}