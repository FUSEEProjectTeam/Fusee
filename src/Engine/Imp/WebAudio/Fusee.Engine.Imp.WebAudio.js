/* This file contains the hand generated part of the FUSEE implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   This file creates the connection to the underlying WebAudio part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.6.0 build 16283. Until then it was changed and maintained manually.
*/

var $asmThis = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebAudio");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebAudioImp", true, [], function ($) {
  var $thisType = $.publicInterface;
  
  $.Method({Static:false, Public:true }, ".ctor", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp__ctor () {
			// not implemented
    }
  );

  $.Method({Static:false, Public:true }, "LoadFile", 
    new JSIL.MethodSignature(null, [$.String]),
    function WebAudioImp_LoadFile (fileName) {
      //loadedAudio = this.get_Content().Load$b1($T00())(fileName);
    }
  );

  $.Method({Static:false, Public:true }, "OpenDevice", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_OpenDevice () {
		// not implemented
    }
  );

  $.Method({Static:false, Public:true }, "Pause", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_Pause () {
		// not implemented
    }
  );

  $.Method({Static:false, Public:true }, "Play", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_Play () {
      // $XNAasm02.CallStatic($T01(), "Play", null, loadedAudio);
    }
  );
  
  $.Field({ Static: false, Public: false }, "loadedAudio", $.String, null);
  
  $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IAudioImp"))
});