using JSIL.Proxy;

namespace Fusee.Engine.Proxies
{
    [JSProxy(
        typeof(Fusee.Engine.ShaderParam),
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared
    )]
    public abstract class ShaderParamHandle
    {

    }
}

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.EngineImp
{
    public class ShaderParamHandleImp
    {
    }
}

*/