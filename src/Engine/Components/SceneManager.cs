using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;


namespace Fusee.SceneManagement
{
    public class SceneManager
    {
        
        private ITraversalState _traversal;
        public List<RenderJob> RenderJobs = new List<RenderJob>(); 
        public List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        

        public SceneManager()
        {
            _traversal =  new TraversalStateRender(this);
        }


        public void  Traverse(RenderCanvas renderCanvas, RenderContext RC, float4x4 camera)
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            foreach (var sceneMember in SceneMembers)
            {
                sceneMember.Traverse(_traversal);
            }

            // Order: Matrix, Mesh, Renderer
            // TODO: change renderjob submission to method.
            for (int i = 0; i < RenderJobs.Count; i+=3 )
            {
                //Console.WriteLine(RenderJobs[i].ToString()+" ist auf index"+i);
                //Console.WriteLine(RenderJobs[i+1].ToString() + " ist auf index" + (i+1));
                //Console.WriteLine(RenderJobs[i+2].ToString() + " ist auf index" + (i+2));
                RC.ModelView = RenderJobs[i].GetMatrix() * camera;
                RC.Render(RenderJobs[i+1].GetMesh());
            }
            renderCanvas.Present();

            //Console.WriteLine("Rendering at "+DeltaTime+"ms and "+(1/DeltaTime)+" FPS"); // Use this to checkout Framerate
            RenderJobs.Clear();
        }

        public void AddRenderJob(RenderJob job)
        {
            RenderJobs.Add(job);
        }

    }
}
