using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using System;

namespace Fusee.SceneManagement
{
    public class SceneManager
    {
        
        private ITraversalState _traversal;
        public List<RenderJob>[] RenderJobs = new List<RenderJob>[10]; 
        public List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        

        public SceneManager()
        {
            _traversal =  new TraversalStateRender(this);
            for (int i = 0; i < RenderJobs.Length; i++ )
            {
                RenderJobs[i] = new List<RenderJob>();
            }
            
        }


        public void Traverse(RenderCanvas renderCanvas, RenderContext RC, float4x4 camera)
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            foreach (var sceneMember in SceneMembers)
            {
                sceneMember.Traverse(_traversal);
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

    }
}
