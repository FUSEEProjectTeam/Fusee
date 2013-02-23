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
  $.Property({Static:false, Public:false}, "Loop", $.Boolean);
 
  $.Method({Static:false, Public:true }, ".ctor", 
    new JSIL.MethodSignature(null, [$.String]), 
    function AudioStream__ctor (fileName) {
      this.AudioStream$StreamFileName$value = fileName.replace(/\.[^/.]+$/, "");

      var instance = createjs.Sound.createInstance(fileName);
      instance.onComplete = function(instance) {
                              if (this.AudioStream$Loop$value)
                                instance.play();
                            }

      this.AudioStream$MainOutputStream$value = instance;
    }
  );

  $.Method({Static:false, Public:true }, "Play", 
    new JSIL.MethodSignature(null, []), 
    function AudioStream_Play () {
      this.AudioStream$MainOutputStream$value.play();
    }
  );

  $.Method({Static:false, Public:true }, "Pause", 
    new JSIL.MethodSignature(null, []), 
    function AudioStream_Pause () {
      this.AudioStream$MainOutputStream$value.pause();
    }
  );

  $.Method({Static:false, Public:true }, "Stop", 
    new JSIL.MethodSignature(null, []), 
    function AudioStream_Stop () {
      this.AudioStream$MainOutputStream$value.stop();
    }
  );

  $.Method({Static:false, Public:true }, "set_Volume", 
    new JSIL.MethodSignature(null, [$.Double]), 
    function AudioStream_set_Volume (value) {
      // not implemented
    }
  );

  $.Method({Static:false, Public:true }, "get_Volume", 
    new JSIL.MethodSignature($.Double, []), 
    function AudioStream_get_Volume () {
      // not implemented
    }
  );

  $.Method({Static:false, Public:true }, "set_Loop", 
    new JSIL.MethodSignature(null, [$.Boolean]), 
    function AudioStream_set_Loop (value) {
      this.AudioStream$Loop$value = value;
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

  $.Method({Static:false, Public:true }, "CloseDevice", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_CloseDevice () {
      // not implemented
    }
  );

  $.Method({Static:false, Public:true }, "Play", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_Play () {
      for (var x = 0; x < this.LoadedStreams; x++)
        this.AllStreams[this.LoadedStreams].Play();
    }
  );
  
  $.Method({Static:false, Public:true }, "Pause", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_Pause () {
      for (var x = 0; x < this.LoadedStreams; x++)
        this.AllStreams[this.LoadedStreams].Pause();
    }
  );

  $.Method({Static:false, Public:true }, "Stop", 
    new JSIL.MethodSignature(null, []),
    function WebAudioImp_Stop () {
      for (var x = 0; x < this.LoadedStreams; x++)
        this.AllStreams[this.LoadedStreams].Stop();
    }
  );

  $.Method({Static:false, Public:true }, "GetVolume", 
    new JSIL.MethodSignature($.Double, []),
    function WebAudioImp_GetVolume () {
      // not implemented
    }
  );

  $.Method({Static:false, Public:true }, "SetVolume", 
    new JSIL.MethodSignature(null, [$.Double]),
    function WebAudioImp_SetVolume () {
      // not implemented
    }
  );

  $.Field({ Static: false, Public: false }, "loadedAudio", $.String, null);
  $.Field({Static:false, Public:false, ReadOnly:true }, "AllStreams", $jsilcore.TypeRef("System.Array", [$fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")]));
    
  $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IAudioImp"))
});