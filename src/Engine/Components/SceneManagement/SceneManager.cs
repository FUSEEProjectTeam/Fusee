using System.Collections.Generic;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    ///     The SceneManager class handles the rendering of the scene.
    /// </summary>
    public class SceneManager
    {
        #region Fields

        private RenderContext _renderContext;

        private readonly List<RenderJob>[] _renderJobs = new List<RenderJob>[10];
        private readonly List<SceneEntity> _sceneMembers = new List<SceneEntity>();
        private SceneVisitorRendering _sceneVisitorRendering;

        internal static RenderContext RContext { get; private set; }
        internal static SceneManager Manager { get; private set; }

        #endregion

        #region Constructors

        public SceneManager()
        {
            InitManager();
        }

        public SceneManager(RenderContext rc)
        {
            _renderContext = rc;

            InitManager();
        }

        private void InitManager()
        {
            _sceneVisitorRendering = new SceneVisitorRendering(this);

            for (int i = 0; i < _renderJobs.Length; i++)
            {
                _renderJobs[i] = new List<RenderJob>();
            }
        }

        #endregion

        #region Members

        /// <summary>
        ///     Attaches this SceneManager to a specific <see cref="RenderContext" />.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext" /> to which the SceneManager should be attached to.</param>
        public void AttachToContext(RenderContext rc)
        {
            _renderContext = rc;
        }

        /// <summary>
        ///     Detaches this SceneManager from a specific <see cref="RenderContext" />.
        /// </summary>
        public void DetachFromContext()
        {
            _renderContext = null;
        }

        /// <summary>
        ///     Adds a <see cref="SceneEntity" /> to the scene.
        /// </summary>
        /// <param name="sceneEntity">The scene entity.</param>
        public void AddSceneEntity(SceneEntity sceneEntity)
        {
            _sceneMembers.Add(sceneEntity);
        }

        /// <summary>
        ///     Adds the camera to the rendering queue [1].
        /// </summary>
        /// <param name="cameramatrix">The RenderCamera object that holds the Projection and Transformation matrix.</param>
        public void AddCamera(RenderCamera cameramatrix)
        {
            _renderJobs[1].Add(cameramatrix);
        }

        /// <summary>
        ///     Traverses the scene's entities with their corresponding components in order to construct an 3D world. This function
        ///     is called every frame.
        /// </summary>
        /// <param name="renderCanvas">The render canvas that presents the final render image.</param>
        public void Traverse(RenderCanvas renderCanvas)
        {
            // statics
            RContext = _renderContext;
            Manager = this;

            // do jobs
            _renderContext.Clear(ClearFlags.Color | ClearFlags.Depth);

            int length = _sceneMembers.Count;

            for (int i = 0; i < length; i++)
            {
                _sceneMembers[i].Accept(_sceneVisitorRendering);
            }

            // Order: Matrix, Mesh, Renderer
            length = _renderJobs.Length;
            for (int i = 1; i < length; i++)
            {
                int innerlength = _renderJobs[i].Count;
                for (int k = 0; k < innerlength; k++)
                {
                    _renderJobs[i][k].SubmitWork(_renderContext);
                }
            }

            renderCanvas.Present();
            length = _renderJobs.Length;

            for (int j = 0; j < length; j++)
            {
                _renderJobs[j].Clear();
            }
        }

        /// <summary>
        ///     Updates the lights information before the scene is rendered.
        /// </summary>
        public void UpdateLights()
        {
            int length = _renderJobs[0].Count;
            for (int j = 0; j < length; j++)
            {
                _renderJobs[0][j].SubmitWork(_renderContext);
            }
        }

        /// <summary>
        ///     Adds a render job to the render queue[2].
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddRenderJob(RenderJob job)
        {
            _renderJobs[2].Add(job);
        }

        /// <summary>
        ///     Adds a light job to the light queue [0].
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddLightJob(RenderJob job)
        {
            _renderJobs[0].Add(job);
        }

        /// <summary>
        ///     Finds the scene entity inside the current scene.
        /// </summary>
        /// <param name="name">The name of the SceneEntity that needs to be found.</param>
        /// <returns>On successful search the first SceneEntity that has the name is returned.</returns>
        public SceneEntity FindSceneEntity(string name)
        {
            var sceneVisitorSearch = new SceneVisitorSearch();
            SceneEntity result = sceneVisitorSearch.FindSceneEntity(_sceneMembers, name);
            return result;
        }

        /// <summary>
        ///     Initializes all <see cref="ActionCode" /> objects in the scene and calls their Start method once.
        /// </summary>
        public void StartActionCode()
        {
            Manager = this;
            RContext = _renderContext;

            var startactions = new SceneVisitorStartAction();

            foreach (var sceneMember in _sceneMembers)
            {
                sceneMember.Accept(startactions);
            }
        }

        #endregion
    }
}