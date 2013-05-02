using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using System;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// The SceneManager class handles the rendering of the scene
    /// </summary>
    public sealed class SceneManager
    {
        /// <summary>
        /// The _manager Singleton
        /// </summary>
        private static readonly SceneManager _manager = new SceneManager();
        /// <summary>
        /// The _render context
        /// </summary>
        private RenderContext _renderContext;
        //private TraversalStateRender _traversalRender;

        /// <summary>
        /// The render jobs
        /// </summary>
        private List<RenderJob>[] RenderJobs = new List<RenderJob>[10];
        /// <summary>
        /// The scene members
        /// </summary>
        private List<SceneEntity> SceneMembers = new List<SceneEntity>();
        /// <summary>
        /// The _scene visitor rendering
        /// </summary>
        private SceneVisitorRendering _sceneVisitorRendering;

        /// <summary>
        /// Prevents a default instance of the <see cref="SceneManager"/> class from being created.
        /// </summary>
        private SceneManager()
        {
            _sceneVisitorRendering = new SceneVisitorRendering(this);
            //_traversalRender =  new TraversalStateRender(this);
            for (int i = 0; i < RenderJobs.Length; i++ )
            {
                RenderJobs[i] = new List<RenderJob>();
            }
            
        }

        /// <summary>
        /// Gets the Singleton SceneManager instance.
        /// </summary>
        /// <value>
        /// The manager instance.
        /// </value>
        public static SceneManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Gets or sets(only once) a RenderContext instance.
        /// </summary>
        /// <value>
        /// The RenderContext.
        /// </value>
        public static RenderContext RC
        {
            set{
                if (_manager._renderContext == null)
                {
                    _manager._renderContext = value;
                }
            }
            get { return _manager._renderContext; }
        }


        /// <summary>
        /// Adds the scene entity to the scene.
        /// </summary>
        /// <param name="sceneEntity">The scene entity.</param>
        public void AddSceneEntity(SceneEntity sceneEntity)
        {
            SceneMembers.Add(sceneEntity);
        }

        /// <summary>
        /// Adds the camera to the rendering queue [0].
        /// </summary>
        /// <param name="cameramatrix">The cameramatrix.</param>
        public void AddCamera(RenderCamera cameramatrix)
        {
            RenderJobs[1].Add(cameramatrix);
        }

        /// <summary>
        /// Traverses the scene's entities with their corresponding components.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        public void Traverse(RenderCanvas renderCanvas)
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            foreach (var sceneMember in SceneMembers)
            {
                sceneMember.Accept(_sceneVisitorRendering);
            }

            // Order: Matrix, Mesh, Renderer
            for (int i = 1; i < RenderJobs.Length; i++ )
            {

                
                
                for (int k = 0; k < RenderJobs[i].Count;k++)
                {

                    
                    RenderJobs[i][k].SubmitWork(RC);

                }
                    
                    
            }
            renderCanvas.Present();

            //Console.WriteLine("Rendering at "+DeltaTime+"ms and "+(1/DeltaTime)+" FPS"); // Use this to checkout Framerate
            foreach (var renderjob in RenderJobs)
            {
                renderjob.Clear();
            }
        }

        public void UpdateLights()
        {
            for (int j = 0; j < RenderJobs[0].Count; j++)
            {
                RenderJobs[0][j].SubmitWork(RC);
            }
        }

        /// <summary>
        /// Adds a render job to the render queue[2].
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddRenderJob(RenderJob job)
        {
            
            RenderJobs[2].Add(job);
        }

        /// <summary>
        /// Adds a light job to the light queue [1].
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddLightJob(RenderJob job)
        {
            RenderJobs[0].Add(job);
        }


        // Ursprüngliche SceneEntity suche
        /*public SceneEntity FindSceneEntity(string sceneEntityName)
        {
            foreach (var sceneMember in SceneMembers)
            {
                if(sceneMember.name == sceneEntityName)
                {
                    return sceneMember;
                }
                foreach (var child in sceneMember.GetChildren())
                {
                    if (child.name == sceneEntityName)
                    {
                        return child;
                    }
                }
            }
            return null;
        }*/

        // neue suche
        /// <summary>
        /// Finds the scene entity inside the current scene.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SceneEntity FindSceneEntity(string name)
        {
            SceneVisitorSearch sceneVisitorSearch = new SceneVisitorSearch();
            SceneEntity result = sceneVisitorSearch.FindSceneEntity(SceneMembers, name);
            return result;
        }

        public void StartActionCode()
        {
            SceneVisitorStartAction startactions = new SceneVisitorStartAction();
            foreach (var sceneMember in SceneMembers)
            {
                sceneMember.Accept(startactions);
            }
        }


    }
}
