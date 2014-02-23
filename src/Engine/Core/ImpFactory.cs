using System;
using System.IO;
using System.Reflection;
using JSIL.Meta;

namespace Fusee.Engine
{

    #region Documentation Header

    /// <summary>
    ///     The Fusee Framework.
    /// </summary>
    internal static class NamespaceDoc
    {
        // documentation
    }

    #endregion

    /// <summary>
    ///     The implementation factory. Creates all the implementation specific objects and returns
    ///     their implementation agnostic interfaces.
    /// </summary>
    public static class ImpFactory
    {
        #region Types

        [JSIgnore] private static Type _renderingImplementor;
        [JSIgnore] private static Type _audioImplementor;
        [JSIgnore] private static Type _physicsImplementor;
        [JSIgnore] private static Type _networkImplementor;
        [JSIgnore] private static Type _inputDriverImplementor;

        [JSIgnore]
        private static Type LoadImplementorAssemblyType(string file, string type)
        {
            var impAsmName = AssemblyName.GetAssemblyName(file);

            if (impAsmName.Name.Equals(Path.GetFileNameWithoutExtension(file)))
            {
                Assembly impAsm = Assembly.Load(impAsmName);

                if (impAsm == null)
                    throw new TypeLoadException("Couldn't load implementor assembly (" + file + ").");

                return impAsm.GetType(type);
            }

            throw new FileLoadException("Implementor assembly signature was wrong (" + file + ").");
        }

        [JSIgnore]
        private static Type RenderingImplementor
        {
            get
            {
                return _renderingImplementor ??
                       (_renderingImplementor =
                           LoadImplementorAssemblyType("Fusee.Engine.Imp.OpenTK.dll",
                               "Fusee.Engine.RenderingImplementor"));
            }
        }

        [JSIgnore]
        private static Type PhysicsImplementor
        {
            get
            {
                return _physicsImplementor ??
                       (_physicsImplementor =
                           LoadImplementorAssemblyType("Fusee.Engine.Imp.Bullet.dll",
                               "Fusee.Engine.PhysicsImplementor"));
            }
        }

        [JSIgnore]
        private static Type AudioImplementor
        {
            get
            {
                return _audioImplementor ??
                       (_audioImplementor =
                           LoadImplementorAssemblyType("Fusee.Engine.Imp.SFMLAudio.dll",
                               "Fusee.Engine.AudioImplementor"));
            }
        }

        [JSIgnore]
        private static Type NetworkImplementor
        {
            get
            {
                return _networkImplementor ??
                       (_networkImplementor =
                           LoadImplementorAssemblyType("Fusee.Engine.Imp.Lidgren.dll",
                               "Fusee.Engine.NetworkImplementor"));
            }
        }

        [JSIgnore]
        private static Type InputDriverImplementor
        {
            get
            {
                return _inputDriverImplementor ??
                       (_inputDriverImplementor =
                           LoadImplementorAssemblyType("Fusee.Engine.Imp.SlimDX.dll",
                               "Fusee.Engine.InputDriverImplementor"));
            }
        }

        #endregion

        #region Members

        [JSIgnore]
        private static MethodInfo CreateIImp(Type imp, string method)
        {
            MethodInfo mi = imp.GetMethod(method);

            if (mi == null)
                throw new MissingMethodException("Implementor type (" + imp + ") doesn't contain method " + method);

            return mi;
        }

        /// <summary>
        ///     Creates an instance of <see cref="IRenderCanvasImp" /> by reflection of RenderingImplementor.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IRenderCanvasImp" />.
        /// </returns>
        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp()
        {
            return (IRenderCanvasImp) CreateIImp(RenderingImplementor, "CreateRenderCanvasImp").Invoke(null, null);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IDynamicWorldImp" /> by reflection of PhysicsImplementor.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IDynamicWorldImp" />.
        /// </returns>
        [JSExternal]
        public static IDynamicWorldImp CreateIDynamicWorldImp()
        {
            return (IDynamicWorldImp) CreateIImp(PhysicsImplementor, "CreateDynamicWorldImp").Invoke(null, null);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IRenderContextImp" /> by reflection of RenderingImplementor.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <returns>
        ///     An instance of <see cref="IRenderContextImp" />.
        /// </returns>
        [JSExternal]
        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            return
                (IRenderContextImp)
                    CreateIImp(RenderingImplementor, "CreateRenderContextImp").Invoke(null, new object[] {renderCanvas});
        }

        /// <summary>
        ///     Creates an instance of <see cref="IInputImp" /> by reflection of RenderingImplementor.
        /// </summary>
        /// <param name="renderCanvas">The render canvas.</param>
        /// <returns>
        ///     An instance of <see cref="IInputImp" />.
        /// </returns>
        [JSExternal]
        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            return
                (IInputImp) CreateIImp(RenderingImplementor, "CreateInputImp").Invoke(null, new object[] {renderCanvas});
        }

        /// <summary>
        ///     Creates an instance of <see cref="IAudioImp" /> by reflection of AudioImplementor.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IAudioImp" />.
        /// </returns>
        [JSExternal]
        public static IAudioImp CreateIAudioImp()
        {
            return (IAudioImp) CreateIImp(AudioImplementor, "CreateAudioImp").Invoke(null, null);
        }

        /// <summary>
        ///     Creates an instance of <see cref="INetworkImp" /> by reflection of NetworkImplementor.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="INetworkImp" />.
        /// </returns>
        [JSExternal]
        public static INetworkImp CreateINetworkImp()
        {
            return (INetworkImp) CreateIImp(NetworkImplementor, "CreateNetworkImp").Invoke(null, null);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IInputDriverImp" /> by reflection of InputDriverImplementor.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IInputDriverImp" />.
        /// </returns>
        [JSExternal]
        public static IInputDriverImp CreateIInputDriverImp()
        {
            return (IInputDriverImp) CreateIImp(InputDriverImplementor, "CreateInputDriverImp").Invoke(null, null);
        }

        #endregion
    }
}