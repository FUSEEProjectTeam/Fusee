using JSIL.Proxy;

namespace Fusee.Engine.Proxies
{
    [JSProxy(
        typeof(Fusee.Engine.ShaderProgramImp),
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared
    )]
    public abstract class ShaderProgramImp
    {
        
    }
}

/*

namespace Fusee.EngineImp
{
    public struct ShaderProgramImp
    {
    }
}
*/