var Module = { 
	onRuntimeInitialized: function () {
		MONO.mono_load_runtime_and_bcl (
			"managed",
			"managed",
			1,
			[ "Fusee.Base.Common.dll", "Fusee.Base.Core.dll", "Fusee.Engine.Common.dll", "Fusee.Engine.Core.dll", "Fusee.Examples.RocketOnly.Core.dll", "Fusee.Examples.RocketOnly.WebAsm.dll", "Fusee.Jometri.dll", "Fusee.Math.Core.dll", "Fusee.Serialization.dll", "Fusee.SerializationSerializer.dll", "Fusee.Xene.dll", "Fusee.Xirkit.dll", "glTFLoader.dll", "Humanizer.dll", "JSIL.Meta.dll", "Mono.Security.dll", "mscorlib.dll", "Newtonsoft.Json.dll", "protobuf-net.dll", "SkiaSharp.Wasm.dll", "System.Core.dll", "System.Data.dll", "System.dll", "System.Drawing.Common.dll", "System.Net.Http.dll", "System.Numerics.dll", "System.Runtime.Serialization.dll", "System.ServiceModel.Internals.dll", "System.Web.Services.dll", "System.Xml.dll", "System.Xml.Linq.dll", "WaveEngine.Common.dll", "WebAssembly.Bindings.dll", "WebAssembly.Net.Http.dll", "WebAssembly.Net.WebSockets.dll", "WebGLDotNET.dll", "Fusee.Base.Common.pdb", "Fusee.Base.Core.pdb", "Fusee.Engine.Common.pdb", "Fusee.Engine.Core.pdb", "Fusee.Examples.RocketOnly.Core.pdb", "Fusee.Examples.RocketOnly.WebAsm.pdb", "Fusee.Jometri.pdb", "Fusee.Math.Core.pdb", "Fusee.Serialization.pdb", "Fusee.Xene.pdb", "Fusee.Xirkit.pdb", "Mono.Security.pdb", "mscorlib.pdb", "System.Core.pdb", "System.Data.pdb", "System.Drawing.Common.pdb", "System.Net.Http.pdb", "System.Numerics.pdb", "System.pdb", "System.Runtime.Serialization.pdb", "System.ServiceModel.Internals.pdb", "System.Web.Services.pdb", "System.Xml.Linq.pdb", "System.Xml.pdb", "WebAssembly.Bindings.pdb", "WebAssembly.Net.Http.pdb", "WebAssembly.Net.WebSockets.pdb", "WebGLDotNET.pdb" ],
			function () {
				App.init ();
			}
		);
	},
};


