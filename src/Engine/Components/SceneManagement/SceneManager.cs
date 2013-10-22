using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using System;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// The SceneManager class handles the rendering of the scene. 
    /// </summary>
    public sealed class SceneManager
    {
        #region Fields
        private static readonly SceneManager _manager = new SceneManager();
        private RenderContext _renderContext;
        private List<RenderJob>[] RenderJobs = new List<RenderJob>[10];
        private List<SceneEntity> SceneMembers = new List<SceneEntity>();
        private SceneVisitorRendering _sceneVisitorRendering;

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
            set
            {
                if (_manager._renderContext == null)
                {
                    _manager._renderContext = value;
                }
            }
            get { return _manager._renderContext; }
        }

        #endregion

        #region Constructors
        private SceneManager()
        {
            _sceneVisitorRendering = new SceneVisitorRendering(this);
            for (int i = 0; i < RenderJobs.Length; i++ )
            {
                RenderJobs[i] = new List<RenderJob>();
            }
        }
        #endregion

        #region Members
        /// <summary>
        /// Adds a <see cref="SceneEntity"/> to the scene.
        /// </summary>
        /// <param name="sceneEntity">The scene entity.</param>
        public void AddSceneEntity(SceneEntity sceneEntity)
        {
            SceneMembers.Add(sceneEntity);
        }

        /// <summary>
        /// Adds the camera to the rendering queue [1].
        /// </summary>
        /// <param name="cameramatrix">The RenderCamera object that holds the Projection and Transformation matrix.</param>
        public void AddCamera(RenderCamera cameramatrix)
        {
            RenderJobs[1].Add(cameramatrix);
        }

        /// <summary>
        /// Traverses the scene's entities with their corresponding components in order to construct an 3D world. This function is called every frame.
        /// </summary>
        /// <param name="renderCanvas">The render canvas that presents the final render image.</param>
        public void Traverse(RenderCanvas renderCanvas)
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            int length = SceneMembers.Count;
            for (int i=0;i<length; i++)
            {
                SceneMembers[i].Accept(_sceneVisitorRendering);
                //sceneMember.Accept();
            }

            // Order: Matrix, Mesh, Renderer
            length = RenderJobs.Length;
            for (int i = 1; i < length; i++ )
            {


                int innerlength = RenderJobs[i].Count;
                for (int k = 0; k < innerlength;k++)
                {

                    
                    RenderJobs[i][k].SubmitWork(RC);

                }
                    
                    
            }
            renderCanvas.Present();
            length = RenderJobs.Length;
            //Debug.WriteLine("Drawn "+RenderJobs[2].Count/3+" Objects at "+Time.Instance.FramePerSecondSmooth+" FPS!");
            for (int j = 0; j < length;j++ )
            {
                RenderJobs[j].Clear();
            }
        }

        /// <summary>
        /// Updates the lights information before the scene is rendered.
        /// </summary>
        public void UpdateLights()
        {
            int length = RenderJobs[0].Count;
            for (int j = 0; j < length; j++)
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
        /// Adds a light job to the light queue [0].
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddLightJob(RenderJob job)
        {
            RenderJobs[0].Add(job);
        }

        /// <summary>
        /// Finds the scene entity inside the current scene.
        /// </summary>
        /// <param name="name">The name of the SceneEntity that needs to be found.</param>
        /// <returns>On successful search the first SceneEntity that has the name is returned.</returns>
        public SceneEntity FindSceneEntity(string name)
        {
            SceneVisitorSearch sceneVisitorSearch = new SceneVisitorSearch();
            SceneEntity result = sceneVisitorSearch.FindSceneEntity(SceneMembers, name);
            return result;
        }

        /// <summary>
        /// Initializes all <see cref="ActionCode"/> objects in the scene and calls their Start method once.
        /// </summary>
        public void StartActionCode()
        {
            SceneVisitorStartAction startactions = new SceneVisitorStartAction();
            foreach (var sceneMember in SceneMembers)
            {
                sceneMember.Accept(startactions);
            }
        }
        #endregion

    }
}
