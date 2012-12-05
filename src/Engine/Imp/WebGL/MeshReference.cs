using JSIL.Proxy;


namespace Fusee.Engine.Proxies
{
    [JSProxy(
        typeof(Fusee.Engine.MeshImp),
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared
    )]
    public abstract class MeshReference
    {
        
    }
}

/*
namespace Fusee.EngineImp
{
    public class MeshImp
    {
        public void InvalidateVertices()
        {
            int i = 42;
        }
        public bool VerticesSet { get { return false; } }

        public void InvalidateNormals()
        {
            int i = 42;
        }
        public bool NormalsSet { get { return false; } }

        public void InvalidateColors()
        {
            int i = 42;
        }
        public bool ColorsSet { get { return false; } }

        public void InvalidateTriangles()
        {
            int i = 42;
        }
        public bool TrianglesSet { get { return false; } }

    }
}
*/