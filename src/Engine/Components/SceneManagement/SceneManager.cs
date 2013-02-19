using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using System;

namespace Fusee.SceneManagement
{
    public sealed class SceneManager
    {
        private static readonly SceneManager _manager = new SceneManager();
        private RenderContext _renderContext;
        private TraversalStateRender _traversalRender;
        private List<RenderJob>[] RenderJobs = new List<RenderJob>[10]; 
        private List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        

        private SceneManager()
        {
            _traversalRender =  new TraversalStateRender(this);
            for (int i = 0; i < RenderJobs.Length; i++ )
            {
                RenderJobs[i] = new List<RenderJob>();
            }
            
        }

        public static SceneManager Manager
        {
            get { return _manager; }
        }

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


        public void AddSceneEntity(SceneEntity sceneEntity)
        {
            SceneMembers.Add(sceneEntity);
        }

        public void Traverse(RenderCanvas renderCanvas)
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            foreach (var sceneMember in SceneMembers)
            {
                
                _traversalRender.SetDeltaTime(Time.Instance.DeltaTime);
                sceneMember.Traverse(_traversalRender);
            }

            // Order: Matrix, Mesh, Renderer
            for (int i = 0; i < RenderJobs.Length; i++ )
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

        public void AddRenderJob(RenderJob job)
        {
            
            RenderJobs[1].Add(job);
        }

        public void AddLightJob(RenderJob job)
        {
            RenderJobs[0].Add(job);
        }

        public void SetInput(Input input)
        {
            _traversalRender.Input = input;
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
        public SceneEntity FindSceneEntity(string name)
        {
            TraversalStateSearch searcher = new TraversalStateSearch();
            SceneEntity result = searcher.FindSceneEntity(SceneMembers, name);
            return result;
        }



    }
}
