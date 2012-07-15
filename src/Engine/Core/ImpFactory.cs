using System;
using System.Reflection;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    /// The implementation factory. Creates all the implementation specific objects and returns
    /// their implementation agnostic interfaces. 
    /// TODO: replace this with something more Dependency Injection Container like
    /// </summary>
    public static class ImpFactory
    {
        [JSIgnore] 
        private static Type _implementor;
        
        [JSIgnore]
        private static Type Implementor
        {
            get
            {
                if (_implementor == null)
                {
                    // TODO: Remove this hardcoded hack to OpenTK
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.OpenTK.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.OpenTK.dll).");
                    _implementor = impAsm.GetType("Fusee.Engine.Implementor");
                }
                return _implementor;
            }
        }

        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp()
        {
            MethodInfo mi = Implementor.GetMethod("CreateRenderCanvasImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateRenderCanvasImp");

            return (IRenderCanvasImp) mi.Invoke(null, null);
        }

        [JSExternal]
        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = Implementor.GetMethod("CreateRenderContextImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateRenderContextImp");

            return (IRenderContextImp)mi.Invoke(null, new object[] { renderCanvas });
        }

        [JSExternal]
        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = Implementor.GetMethod("CreateInputImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateInputImp");

            return (IInputImp)mi.Invoke(null, new object[] { renderCanvas });
        }



    }
}
