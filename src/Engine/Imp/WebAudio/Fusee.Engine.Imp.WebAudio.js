/* This file contains the hand generated part of the FUSEE implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   This file creates the connection to the underlying WebAudio part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.6.0 build 16283. Until then it was changed and maintained manually.
*/

var $WebAudioImp = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebAudio");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.AudioStream", true, [], function ($) {
  var $thisType = $.publicInterface;

  $.Property({Static:false, Public:false}, "MainOutputStream", $.Object);
  $.Property({Static:false, Public:false}, "StreamFileName", $.String);
 
  $.Method({Static:false, Public:true }, ".ctor", 
    new JSIL.MethodSignature(null, [$.String]), 
    function AudioStream__ctor (fileName) {
      this.AudioStream$StreamFileName$value = fileName.replace(/\.[^/.]+$/, "");
	    //this.AudioStream$MainOutputStream$value = createjs.Sound.registerSound(fileName);
    }
  );

  $.Method({Static:false, Public:true }, "Play", 
    new JSIL.MethodSignature(null, []), 
    function AudioStream_Play () {
		createjs.Sound.play(JSIL.Host.getAssetVal(this.AudioStream$StreamFileName$value));
    }
  );

  $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IAudioStream"))
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebAudioImp", true, [], function ($) {
  var $thisType = $.publicInterface;
  var soundInstance;
  
  $.Method({Static:false, Public:true }, ".ctor", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp__ctor () {
      this.AllStreams = JSIL.Array.New($fuseeCommon.Fusee.Engine.IAudioStream, 128);
    }
  );

  $.Method({Static:false, Public:true }, "LoadFile", 
    new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IAudioStream"), [$.String]),
    function WebAudioImp_LoadFile (fileName) {
      var tmp = new $WebAudioImp.Fusee.Engine.AudioStream(fileName);
      this.AllStreams[this.LoadedStreams] = tmp;
      ++this.LoadedStreams;
      return tmp;
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
		//var instance = allAssets["./Assets/tetris"];
		//soundInstance = createjs.Sound.play(instance);
    }
  );
  
  $.Field({ Static: false, Public: false }, "loadedAudio", $.String, null);
  $.Field({Static:false, Public:false, ReadOnly:true }, "AllStreams", $jsilcore.TypeRef("System.Array", [$fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")]));
    
  $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IAudioImp"))
});