/* This file contains the hand generated part of the FUSEE implementation.
	 Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
	 This file creates the connection to the underlying WebAudio part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.6.0 build 16283. From then on it was changed and maintained manually.
*/

var $WebAudioImp = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebAudio");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.AudioStream", true, [], function($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Field({ Static: false, Public: false }, "MainOutputStream", $.Object);
    $.Field({ Static: false, Public: false }, "StreamFileName", $.String);

    $.Property({ Static: false, Public: false }, "Loop", $.Boolean);
    $.Property({ Static: false, Public: false }, "Panning", $.Single);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [$.String]),
        function _ctor(fileName) {
            var playbackFile = fileName.replace(/\.[^/.]+$/, "");
            playbackFile = JSIL.Host.getAssetVal(playbackFile);

            try {
                var instance = createjs.Sound.createInstance(playbackFile);
            } catch(e) {
                var instance = null;
            }

            this.StreamFileName = playbackFile;
            this.MainOutputStream = instance;

            if (instance)
                this.MainOutputStream.addEventListener("complete", this.playingComplete.bind(this));
        }
    );

    $.Method({ Static: false, Public: false }, "playingComplete",
        new JSIL.MethodSignature(null, [$.Object]),
        function playingComplete(event) {
            if (event.type == "complete")
                if (this.AudioStream$Loop$value) {
                    var self = this;
                    window.setTimeout(function() { self.MainOutputStream.play(); }, 1);
                }
        }
    );

    $.Method({ Static: false, Public: true }, "Play",
        new JSIL.MethodSignature(null, []),
        function Play() {
            if (!this.MainOutputStream)
                return;

            if (this.MainOutputStream.paused && this.MainOutputStream.playState == "playSucceeded")
                this.MainOutputStream.resume();
            else
                this.MainOutputStream.play();
        }
    );

    $.Method({ Static: false, Public: true }, "Play",
        new JSIL.MethodSignature(null, [$.Boolean]),
        function Play(loop) {
            this.Loop = loop;
            this.Play();
        }
    );

    $.Method({ Static: false, Public: true }, "Pause",
        new JSIL.MethodSignature(null, []),
        function Pause() {
            if (!this.MainOutputStream)
                return;

            this.MainOutputStream.pause();
        }
    );

    $.Method({ Static: false, Public: true }, "Stop",
        new JSIL.MethodSignature(null, []),
        function Stop() {
            if (!this.MainOutputStream)
                return;

            this.MainOutputStream.stop();
        }
    );

    $.Method({ Static: false, Public: true }, "set_Volume",
        new JSIL.MethodSignature(null, [$.Single]),
        function set_Volume(value) {
            if (!this.MainOutputStream)
                return;

            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, 0);

            this.MainOutputStream.setVolume(maxVal / 100);
        }
    );

    $.Method({ Static: false, Public: true }, "get_Volume",
        new JSIL.MethodSignature($.Single, []),
        function get_Volume() {
            if (!this.MainOutputStream)
                return;

            return this.MainOutputStream.getVolume() * 100;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Loop",
        new JSIL.MethodSignature(null, [$.Boolean]),
        function set_Loop(value) {
            this.AudioStream$Loop$value = value;
        }
    );

    $.Method({ Static: false, Public: true }, "get_Loop",
        new JSIL.MethodSignature($.Boolean, []),
        function get_Loop() {
            return this.AudioStream$Loop$value;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Panning",
        new JSIL.MethodSignature(null, [$.Single]),
        function set_Panning(value) {
            if (!this.MainOutputStream)
                return;

            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, -100);

            this.MainOutputStream.setPan(maxVal / 100);
        }
    );

    $.Method({ Static: false, Public: true }, "get_Panning",
        new JSIL.MethodSignature($.Boolean, []),
        function get_Panning() {
            if (!this.MainOutputStream)
                return;

            return this.MainOutputStream.getPan() * 100;
        }
    );

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")
    );

    return function(newThisType) { $thisType = newThisType; };
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebAudioImp", true, [], function($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Field({ Static: false, Public: false }, "AllStreams", $jsilcore.TypeRef("System.Array", [$fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")]));
    $.Field({ Static: false, Public: false }, "LoadedStreams", $.Int32);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, []),
        function WebAudioImp__ctor() {
            this.AllStreams = JSIL.Array.New($fuseeCommon.Fusee.Engine.IAudioStream, 128);
        }
    );

    $.Method({ Static: false, Public: true }, "LoadFile",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IAudioStream"), [$.String]),
        function WebAudioImp_LoadFile(fileName) {
            var tmp = new $WebAudioImp.Fusee.Engine.AudioStream(fileName);
            this.AllStreams[this.LoadedStreams] = tmp;
            ++this.LoadedStreams;
            return tmp;
        }
    );

    $.Method({ Static: false, Public: true }, "OpenDevice",
        new JSIL.MethodSignature(null, []),
        function WebAudioImp_OpenDevice() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "CloseDevice",
        new JSIL.MethodSignature(null, []),
        function WebAudioImp_CloseDevice() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "Stop",
        new JSIL.MethodSignature(null, []),
        function WebAudioImp_Stop() {
            createjs.Sound.stop();
        }
    );

    $.Method({ Static: false, Public: true }, "GetVolume",
        new JSIL.MethodSignature($.Single, []),
        function WebAudioImp_GetVolume() {
            return createjs.Sound.getVolume() * 100;
        }
    );

    $.Method({ Static: false, Public: true }, "SetVolume",
        new JSIL.MethodSignature(null, [$.Single]),
        function WebAudioImp_SetVolume(value) {
            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, 0);

            createjs.Sound.setVolume(maxVal / 100);
        }
    );

    $.Method({ Static: false, Public: true }, "SetPanning",
        new JSIL.MethodSignature(null, [$.Single]),
        function WebAudioImp_SetPanning(value) {
            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, -100);

            for (var x = 0; x < this.LoadedStreams; x++)
                this.AllStreams[x].Panning = maxVal;
        }
    );

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IAudioImp")
    );

    return function(newThisType) { $thisType = newThisType; };
});