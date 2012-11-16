using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;


namespace SceneManagement
{
    public class TraversalStateRender : ITraversalState
    {
        private Stack<bool> _hasTransform;
        private Stack<bool> _hasRenderer;
        private Stack<bool> _hasMesh;
        private RenderQueue _queue;
        private Stack<float4x4> _mtxModelViewStack;
        private Stack<Mesh> _meshStack;
        private Stack<Renderer> _RendererStack;

        public TraversalStateRender(RenderQueue queue)
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
            _hasMesh.Pop();
            _hasRenderer.Pop();
            _hasTransform.Pop();
        }

        private void AddRenderJob(float4x4 matrix, Mesh mesh, Renderer renderer)
        {
            RenderMatrix renderMatrix = new RenderMatrix(matrix);
            _queue.AddRenderJob(renderMatrix);
            RenderMesh renderMesh = new RenderMesh(mesh);
            _queue.AddRenderJob(renderMesh);
            RenderRenderer renderRenderer = new RenderRenderer(renderer);
            _queue.AddRenderJob(renderRenderer);
        }
    }
}