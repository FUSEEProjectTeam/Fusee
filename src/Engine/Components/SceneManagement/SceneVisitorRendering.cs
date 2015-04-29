using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    ///     A derivate of SceneVisitor, used to Visit Renderer, Transformation and Mesh components and submit to renderqueue of
    ///     <see cref="SceneManager" />.
    /// </summary>
    public class SceneVisitorRendering : SceneVisitor
    {
        #region Fields

        /// <summary>
        ///     The _has transform
        /// </summary>
        private readonly Stack<bool> _hasTransform;

        /// <summary>
        ///     The _has renderer
        /// </summary>
        private readonly Stack<bool> _hasRenderer;

        /// <summary>
        ///     The _has mesh
        /// </summary>
        private readonly Stack<bool> _hasMesh;

        /// <summary>
        ///     The _queue
        /// </summary>
        private readonly SceneManager _queue;

        /// <summary>
        ///     The _MTX model view stack
        /// </summary>
        private readonly Stack<float4x4> _mtxModelViewStack;

        /// <summary>
        ///     The _mesh stack
        /// </summary>
        private readonly Stack<Mesh> _meshStack;

        /// <summary>
        ///     The _ renderer stack
        /// </summary>
        private readonly Stack<Renderer> _rendererStack;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SceneVisitorRendering" /> class.
        /// </summary>
        /// <param name="queue">The SceneManager reference.</param>
        public SceneVisitorRendering(SceneManager queue)
        {
            _queue = queue;

            _mtxModelViewStack = new Stack<float4x4>();
            _meshStack = new Stack<Mesh>();
            _rendererStack = new Stack<Renderer>();
            _hasTransform = new Stack<bool>();
            _hasRenderer = new Stack<bool>();
            _hasMesh = new Stack<bool>();
        }

        #endregion

        #region Members

        // TODO: Store Mesh as local variable instead of stacks as it is not used in further traversals.
        /// <summary>
        ///     Stores the mesh in internal stack.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        private void StoreMesh(Mesh mesh)
        {
            _hasMesh.Pop();
            _meshStack.Push(mesh);
            _hasMesh.Push(true);

            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _rendererStack.Peek());
            }
        }

        /// <summary>
        ///     Stores the renderer in internal stack.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        private void StoreRenderer(Renderer renderer)
        {
            _hasRenderer.Pop();

            _rendererStack.Push(renderer);
            _hasRenderer.Push(true);

            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _rendererStack.Peek());
            }
        }

        /// <summary>
        ///     Adds the transform object to the internal stack.
        /// </summary>
        /// <param name="transform">The transform.</param>
        private void AddTransform(Transformation transform)
        {
            if (_mtxModelViewStack.Count > 0)
            {
                if (transform.GlobalMatrixDirty)
                {
                    transform.Matrix = transform.GlobalMatrix*float4x4.Invert(_mtxModelViewStack.Peek());
                    transform.SetGlobalMat(_mtxModelViewStack.Peek()*transform.Matrix);
                    _mtxModelViewStack.Push(_mtxModelViewStack.Pop()*transform.Matrix);
                    //Debug.WriteLine("Matrix: " + transform.GlobalMatrix + " Point: " + transform.GlobalPosition);

                    _hasTransform.Pop();
                    _hasTransform.Push(true);


                    if (HasRenderingTriple())
                    {
                        AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _rendererStack.Peek());
                    }
                    return;
                }

                transform.SetGlobalMat(_mtxModelViewStack.Peek()*transform.Matrix);
                _mtxModelViewStack.Push(_mtxModelViewStack.Pop()*transform.Matrix);
                //Debug.WriteLine("Matrix: " + transform.GlobalMatrix + " Point: " + transform.GlobalPosition);

                _hasTransform.Pop();
                _hasTransform.Push(true);


                if (HasRenderingTriple())
                {
                    AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _rendererStack.Peek());
                }
            }
            else
            {
                _mtxModelViewStack.Push(transform.GlobalMatrix);
                //Debug.WriteLine("Matrix: " + transform.GlobalMatrix + " Point: " + transform.GlobalPosition);

                if (_hasTransform.Count > 0)
                {
                    _hasTransform.Pop();
                }

                _hasTransform.Push(true);


                if (HasRenderingTriple())
                {
                    AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _rendererStack.Peek());
                }
            }
        }

        /// <summary>
        ///     Determines whether [has rendering triple].
        /// </summary>
        /// <returns>
        ///     <c>true</c> if [has rendering triple]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasRenderingTriple()
        {
            return (_hasMesh.Peek() && _hasRenderer.Peek() && _hasTransform.Peek());
        }

        private void Push()
        {
            if (_mtxModelViewStack.Count > 0)
                _mtxModelViewStack.Push(_mtxModelViewStack.Peek());
            _hasMesh.Push(false);
            _hasRenderer.Push(false);
            _hasTransform.Push(false);
        }

        private void Pop()
        {
            if (_mtxModelViewStack.Count > 0)
            {
                _mtxModelViewStack.Pop();
            }

            _hasTransform.Pop();
            _hasMesh.Pop();
            _hasRenderer.Pop();
            if (_meshStack.Count > 0)
            {
                _meshStack.Pop();
            }

            if (_rendererStack.Count > 0)
            {
                _rendererStack.Pop();
            }
        }

        /// <summary>
        ///     Adds a <see cref="DirectionalLight" /> to the rendering queue.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="diffuse">The diffuse color.</param>
        /// <param name="ambient">The ambient color.</param>
        /// <param name="specular">The specular color.</param>
        /// <param name="type">The type.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightDirectional(float3 direction, float4 diffuse, float4 ambient, float4 specular,
            Light.LightType type, int channel)
        {
            var light = new RenderDirectionalLight(direction, diffuse, ambient, specular, type, channel);
            _queue.AddLightJob(light);
        }

        /// <summary>
        ///     Adds a <see cref="PointLight" /> to the rendering queue.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="type">The lighttype.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightPoint(float3 position, float4 diffuse, float4 ambient, float4 specular, Light.LightType type,
            int channel)
        {
            var light = new RenderPointLight(position, diffuse, ambient, specular, type, channel);
            _queue.AddLightJob(light);
        }

        /// <summary>
        ///     Adds a <see cref="SpotLight" /> to the rendering queue.
        /// </summary>
        /// <param name="position">The position of the light in the scene.</param>
        /// <param name="direction">The direction of the light along its z-axis.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="angle">The angle of the spot light.</param>
        /// <param name="type">The lighttype.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightSpot(float3 position, float3 direction, float4 diffuse, float4 ambient, float4 specular,
            float angle, Light.LightType type, int channel)
        {
            var light = new RenderSpotLight(position, direction, diffuse, ambient, specular, angle, type, channel);
            _queue.AddLightJob(light);
        }

        /// <summary>
        ///     Adds the render job that consists of Matrix, Renderer and Mesh to the rendering queue.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="renderer">The renderer.</param>
        public void AddRenderJob(float4x4 matrix, Mesh mesh, Renderer renderer)
        {
            var renderMatrix = new RenderMatrix(matrix);
            _queue.AddRenderJob(renderMatrix);

            var renderRenderer = new RenderRenderer(renderer);
            _queue.AddRenderJob(renderRenderer);

            var renderMesh = new RenderMesh(mesh);
            _queue.AddRenderJob(renderMesh);
        }

        #endregion

        #region Overrides

        // Polymorphic Visitcomponent Methods
        /// <summary>
        ///     Visits the specified <see cref="SceneEntity" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="cEntity">The cEntity.</param>
        public override void Visit(SceneEntity cEntity)
        {
            Push();
            cEntity.TraverseForRendering(this);
            Pop();
        }

        /*
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
        }*/

        /// <summary>
        ///     Visits the specified <see cref="ActionCode" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="actionCode">The action code.</param>
        public override void Visit(ActionCode actionCode)
        {
            actionCode.TraverseForRendering(this);
        }

        /// <summary>
        ///     Visits the specified <see cref="Renderer" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public override void Visit(Renderer renderer)
        {
            if (renderer.mesh != null && renderer.material != null)
            {
                StoreMesh(renderer.mesh);
                StoreRenderer(renderer);
            }
        }

        /// <summary>
        ///     Visits the specified <see cref="Transformation" />.
        /// </summary>
        /// <param name="transform">The transformation instance.</param>
        public override void Visit(Transformation transform)
        {
            AddTransform(transform);
        }

        /// <summary>
        ///     Visits the specified <see cref="DirectionalLight" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="directionalLight">The directional light.</param>
        public override void Visit(DirectionalLight directionalLight)
        {
            directionalLight.TraverseForRendering(this);
        }

        /// <summary>
        ///     Visits the specified <see cref="PointLight" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="pointLight">The point light.</param>
        public override void Visit(PointLight pointLight)
        {
            pointLight.TraverseForRendering(this);
        }

        /// <summary>
        ///     Visits the specified <see cref="SpotLight" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="spotLight">The spot light.</param>
        public override void Visit(SpotLight spotLight)
        {
            spotLight.TraverseForRendering(this);
        }

        /// <summary>
        ///     Visits the specified <see cref="Camera" /> to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public override void Visit(Camera camera)
        {
            if (_mtxModelViewStack.Count > 0)
            {
                camera.ViewMatrix = _mtxModelViewStack.Peek();
                _queue.AddCamera(camera.SubmitWork());
            }
        }

        #endregion
    }
}