using System;
using System.Reflection;
using JSIL.Meta;

namespace Fusee.Engine
{
    #region Documentation Header
    /// <summary>
    /// The Fusee Framework.
    /// </summary>
    internal class NamespaceDoc
    {
    }
    #endregion

    /// <summary>
    /// The implementation factory. Creates all the implementation specific objects and returns
    /// their implementation agnostic interfaces. 
    /// TODO: replace this with something more Dependency Injection Container like
    /// </summary>
    public static class ImpFactory
    {
        #region Types
        [JSIgnore] 
        private static Type _renderingImplementor;

        [JSIgnore]
        private static Type _audioImplementor;

        [JSIgnore]
        private static Type _physicsImplementor;
        
        [JSIgnore]
        private static Type _networkImplementor;

        [JSIgnore]
        private static Type _inputDriverImplementor;

        [JSIgnore]
        private static Type RenderingImplementor
        {
            get
            {
                if (_renderingImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to OpenTK

                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.OpenTK.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.OpenTK.dll).");

                    _renderingImplementor = impAsm.GetType("Fusee.Engine.RenderingImplementor");
                }
                return _renderingImplementor;
            }
        }

        // Physics
        [JSIgnore]
        private static Type PhysicsImplementor
        {
            get
            {
                if (_physicsImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to OpenTK

                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.Bullet.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.Bullet.dll).");

                    _physicsImplementor = impAsm.GetType("Fusee.Engine.PhysicsImplementor");
                }
                return _physicsImplementor;
            }
        }

        [JSIgnore]
        private static Type AudioImplementor
        {
            get
            {
                if (_audioImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to SFMLAudio
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.SFMLAudio.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.SFMLAudio.dll).");

                    _audioImplementor = impAsm.GetType("Fusee.Engine.AudioImplementor");
                }
                return _audioImplementor;
            }
        }

        [JSIgnore]
        private static Type NetworkImplementor
        {
            get
            {
                if (_networkImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to Lidgren
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.Lidgren.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.Lidgren.dll).");

                    _networkImplementor = impAsm.GetType("Fusee.Engine.NetworkImplementor");
                }
                return _networkImplementor;
            }
        }

        [JSIgnore]
        private static Type InputDriverImplementor
        {
            get
            {
                if (_inputDriverImplementor == null)
                {

                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.SlimDX.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.SlimDX.dll).");

                    _inputDriverImplementor = impAsm.GetType("Fusee.Engine.InputDriverImplementor");
                }
                return _inputDriverImplementor;
            }
        }

        #endregion

        #region Members
        /// <summary>
        /// Creates an instance of <see cref="IRenderCanvasImp"/> by reflection of RenderingImplementor.
        /// </summary>
        /// <returns>An instance of <see cref="IRenderCanvasImp"/>.</returns>
        /// <exception cref="System.Exception">Implementor type ( + RenderingImplementor.ToString() + ) doesn't contain method CreateRenderCanvasImp</exception>
        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp()
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateRenderCanvasImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateRenderCanvasImp");

            return (IRenderCanvasImp)mi.Invoke(null, null);
        }

        //PhysicsImp
        [JSExternal]
        public static IDynamicWorldImp CreateIDynamicWorldImp()
        {
            MethodInfo mi = PhysicsImplementor.GetMethod("CreateDynamicWorldImp");
            if (mi == null)
                throw new Exception("Implementor type (" + PhysicsImplementor.ToString() + ") doesn't contain method CreateDynamicWorldImp");

            return (IDynamicWorldImp)mi.Invoke(null, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRenderContextImp"/> by reflection of RenderingImplementor.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Implementor type ( + RenderingImplementor.ToString() + ) doesn't contain method CreateRenderContextImp</exception>
        [JSExternal]
        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateRenderContextImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateRenderContextImp");

            return (IRenderContextImp)mi.Invoke(null, new object[] { renderCanvas });
        }

        /// <summary>
        /// Creates an instance of <see cref="IInputImp"/> by reflection of RenderingImplementor.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Implementor type ( + RenderingImplementor.ToString() + ) doesn't contain method CreateInputImp</exception>
        [JSExternal]
        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateInputImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateInputImp");

            return (IInputImp)mi.Invoke(null, new object[] { renderCanvas });
        }

        /// <summary>
        /// Creates an instance of <see cref="IAudioImp"/> by reflection of AudioImplementor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Implementor type ( + AudioImplementor.ToString() + ) doesn't contain method CreateAudioImp</exception>
        [JSExternal]
        public static IAudioImp CreateIAudioImp()
        {
            MethodInfo mi = AudioImplementor.GetMethod("CreateAudioImp");

            if (mi == null)
                throw new Exception("Implementor type (" + AudioImplementor.ToString() + ") doesn't contain method CreateAudioImp");

            return (IAudioImp)mi.Invoke(null, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="INetworkImp"/> by reflection of NetworkImplementor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Implementor type ( + RenderingImplementor.ToString() + ) doesn't contain method CreateNetworkImp</exception>
        [JSExternal]
        public static INetworkImp CreateINetworkImp()
        {
            MethodInfo mi = NetworkImplementor.GetMethod("CreateNetworkImp");

            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateNetworkImp");

            return (INetworkImp)mi.Invoke(null, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="INetworkImp"/> by reflection of NetworkImplementor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Implementor type ( + RenderingImplementor.ToString() + ) doesn't contain method CreateNetworkImp</exception>
        [JSExternal]
        public static IInputDriverImp CreateIInputDriverImp()
        {
            MethodInfo mi = InputDriverImplementor.GetMethod("CreateInputDriverImp");

            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateInputDriverImp");

            return (IInputDriverImp)mi.Invoke(null, null);
        }

        #endregion
    }
}
