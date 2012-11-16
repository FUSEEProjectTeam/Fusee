using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace SceneManagementScratch
{


    public class Component
    {
        public Component()
        {
        }

        public Component(SceneEntity sceneEntity)
        {
            _sceneEntity = sceneEntity;
        }

        private SceneEntity _sceneEntity;
	    public SceneEntity SceneEntity
	    {
		    get { return _sceneEntity;}
		    set { _sceneEntity = value;}
	    }
	
        virtual public void Traverse(ITraversalState traversal)
        {
        }
    }

    public class MeshComponent : Component // TODO: think about name
    {
        private Mesh _mesh;
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        override public void Traverse(ITraversalState traversal)
        {
        }        
    }

    public class Behavior : Component // AE
    {
        virtual public void Update()
        {
            
        }

        override public void Traverse(ITraversalState traversal)
        {
            Update();
        }           
    }

    public class Transform : Component
    {
        private float4x4  _mtx;

        override public void Traverse(ITraversalState traversal)
        {
            traversal.AddTransform(_mtx);
        }           

    }


    public interface ITraversalState
    {
        void StoreMesh(Mesh mesh);
        void AddTransform(float4x4 mtx);
        void Push();
        void Pop();
    }
    
    public class TraversalStateRender : ITraversalState
    {
        private Stack<bool> _hasTransform;
        private Stack<bool> _hasRenderer;
        private Stack<bool> _hasMesh;
 
        private Stack<float4x4> _mtxModelViewStack; 

        public TraversalStateRender()
        {
            _mtxModelViewStack = new Stack<float4x4>();
        }

        public void StoreMesh(Mesh mesh)
        {
            _hasMesh.Pop();
            _hasMesh.Push(true);
            if (HasRenderingTriple())
            {
                // Add DrawCall(matrix, mesh, renderer);
            }
            
        }
        public void StoreRenderer(Renderer Renderer)
        {
            _hasRenderer.Pop();
            _hasRenderer.Push(true);
            if (HasRenderingTriple())
            {
                // Add DrawCall(matrix, Renderer, renderer);
            }
            
        }
        public void AddTransform(float4x4 mtx)
        {
            _mtxModelViewStack.Push(mtx * _mtxModelViewStack.Pop());
            _hasTransform.Push(true);
            if (HasRenderingTriple())
            {
                // Add MtxRQE
                // Add RendererRQE
                // Add MeshRQE
                // Add DrawCall(matrix, mesh, renderer);
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
    }


    // Was GameEntity
    public class SceneEntity
    {
        public SceneEntity()
        {
        }

        public SceneEntity(SceneEntity parent)
        {
            _parent = parent;
        }

        private SceneEntity _parent;
	    public SceneEntity Parent
	    {
		    get { return _parent;}
		    set { _parent = value;}
	    }
	
        private List<SceneEntity> _childSceneEntities;
        private List<Component> _childComponents;

        public void Traverse(ITraversalState traversal)
        {
            traversal.Push();
            foreach (var childComponent in _childComponents)
            {
                childComponent.Traverse(traversal);
            }

            foreach (var childGameEntity in _childSceneEntities)
            {
                childGameEntity.Traverse(traversal);
            }
            traversal.Pop();
        }
    }
}
