using JSIL.Proxy;

namespace Fusee.Engine.Proxies
{
    [JSProxy(
        typeof(Fusee.Engine.RenderCanvasImp),
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared
    )]
    public abstract class RenderCanvasImp
    {
        
    }
}

/*


namespace Fusee.EngineImp
{
    public class RenderCanvasImp
    {
        public int Width { get { return 42; }}
        public int Height { get { return 42; } }

        public double DeltaTime
        {
            get
            {
                return 42.42; 
            }
        }

        public RenderCanvasImp()
        {
        }

        public void Present()
        {
        }

        public void Run()
        {
        }


        public virtual void Init()
        {
        }

        public virtual void RenderAFrame()
        {
        }

        public virtual void Resize()
        {
        }
    }

}
*/