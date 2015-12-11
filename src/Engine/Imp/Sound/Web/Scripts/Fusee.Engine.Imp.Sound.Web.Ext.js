/* This file contains the hand generated part of the FUSEE implementation.
	 Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
	 This file creates the connection to the underlying WebAudio part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.6.0 build 16283. From then on it was changed and maintained manually.

    This implementation relys on JSIL.Browser.Audio.js
*/

var $WebAudioImp = JSIL.GetAssembly("Fusee.Engine.Imp.Sound.Web");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");
JSIL.DeclareNamespace("Fusee.Engine.Imp");
JSIL.DeclareNamespace("Fusee.Engine.Imp.Sound");
JSIL.DeclareNamespace("Fusee.Engine.Imp.Sound.Web");


JSIL.ImplementExternals("Fusee.Engine.Imp.Sound.Web.AudioStream", function ($)
{
    $.Field({ Static: false, Public: false }, "MainOutputStream", $.Object);
    $.Field({ Static: false, Public: false }, "StreamFileName", $.String);
    $.Field({ Static: false, Public: false }, "_loop", $.Boolean);
    $.Field({ Static: false, Public: false }, "_bufferSourceProbablyCreated", $.Boolean);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [$.String]),
        function _ctor(fileName) {
            // var playbackFile = fileName.replace(/\.[^/.]+$/, "");
            // playbackFile = JSIL.Host.getAssetVal(playbackFile);
            var instance = JSIL.Host.getAsset(fileName);

            this.StreamFileName = fileName;
            this.MainOutputStream = instance;
            this._loop = false;
            this._bufferSourceProbablyCreated = false;
        }
    );

    $.Method({ Static: false, Public: true }, "Play",
        new JSIL.MethodSignature(null, []),
        function Play() {
            if (!this.MainOutputStream)
                return;

            if (this.MainOutputStream.isPaused)
            {
                this.MainOutputStream.resume();
                // HACK around JSIL.Browser.Audio.js crashing if loop is set and no bufferSource created yet.
                // after resume we can be sure that the buffer is created and can safely set the loop property.
                this.MainOutputStream.set_loop(this._loop);
            }
            else
            {
                this.MainOutputStream.play();
                // HACK around JSIL.Browser.Audio.js crashing if loop is set and no bufferSource created yet.
                // after resume we can be sure that the buffer is created and can safely set the loop property.
                this.MainOutputStream.set_loop(this._loop);
            }
            this._bufferSourceProbablyCreated = true;
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

            if (this.MainOutputStream.isPlaying)
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

            this.MainOutputStream.set_volume(maxVal / 100);
        }
    );

    $.Method({ Static: false, Public: true }, "get_Volume",
        new JSIL.MethodSignature($.Single, []),
        function get_Volume() {
            if (!this.MainOutputStream)
                return 0;

            return this.MainOutputStream.get_volume() * 100;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Loop",
        new JSIL.MethodSignature(null, [$.Boolean]),
        function set_Loop(value) {
            // HACK around JSIL.Browser.Audio.js crashing if loop is set and no bufferSource created yet.
            // We just store the loop property here and set it right after play() or resume().
            this._loop = value;

            if (!this.MainOutputStream) 
                return;

            // unfortunately isPlaying yields false for looping sounds when repeating (only true for the first time)
            // if (!this.MainOutputStream.get_isPlaying())
            if (!this._bufferSourceProbablyCreated)
                return;

            this.MainOutputStream.set_loop(value);
        }
    );

    $.Method({ Static: false, Public: true }, "get_Loop",
        new JSIL.MethodSignature($.Boolean, []),
        function get_Loop() {
            return this._loop;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Panning",
        new JSIL.MethodSignature(null, [$.Single]),
        function set_Panning(value) {
            if (!this.MainOutputStream)
                return;

            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, -100);

            this.MainOutputStream.set_pan(maxVal / 100);
        }
    );

    $.Method({ Static: false, Public: true }, "get_Panning",
        new JSIL.MethodSignature($.Single, []),
        function get_Panning() {
            if (!this.MainOutputStream)
                return;

            return this.MainOutputStream.get_pan() * 100;
        }
    );

    return function(newThisType) { $thisType = newThisType; };
});

JSIL.ImplementExternals("Fusee.Engine.Imp.Sound.Web.AudioImp", function ($)
{
    $.Field({ Static: false, Public: false }, "AllStreams", $jsilcore.TypeRef("System.Array", [$fuseeCommon.TypeRef("Fusee.Engine.Common.IAudioStream")]));
    $.Field({ Static: false, Public: false }, "LoadedStreams", $.Int32);
    $.Field({ Static: false, Public: false }, "_globalVolume", $.Single);
    $.Field({ Static: false, Public: false }, "_globalPanning", $.Single);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, []),
        function WebAudioImp__ctor() {
            this.AllStreams = JSIL.Array.New($fuseeCommon.Fusee.Engine.Common.IAudioStream, 128);
            this._globalVolume = 100;
            this._globalPanning = 0;
            this.LoadedStreams = 0;
        }
    );

    $.Method({ Static: false, Public: true }, "LoadFile",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.Common.IAudioStream"), [$.String, $.Boolean]),
        function WebAudioImp_LoadFile(fileName, streaming) {
            var tmp = new $WebAudioImp.Fusee.Engine.Imp.Sound.Web.AudioStream(fileName);
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
            for (var x = 0; x < this.LoadedStreams; x++)
                this.AllStreams[x].Stop();
        }
    );

    $.Method({ Static: false, Public: true }, "GetVolume",
        new JSIL.MethodSignature($.Single, []),
        function WebAudioImp_GetVolume() {
            return this._globalVolume;
        }
    );

    $.Method({ Static: false, Public: true }, "SetVolume",
        new JSIL.MethodSignature(null, [$.Single]),
        function WebAudioImp_SetVolume(value) {
            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, 0);
            this._globalVolume = maxVal;

            for (var x = 0; x < this.LoadedStreams; x++)
                this.AllStreams[x].set_Volume(maxVal);
        }
    );

    $.Method({ Static: false, Public: true }, "GetPanning",
        new JSIL.MethodSignature($.Single, []),
        function WebAudioImp_GetPanning() {
            return this._globalPanning;
        }
    );

    $.Method({ Static: false, Public: true }, "SetPanning",
        new JSIL.MethodSignature(null, [$.Single]),
        function WebAudioImp_SetPanning(value) {
            var maxVal = System.Math.Min(100, value);
            maxVal = System.Math.Max(maxVal, -100);
            this._globalPanning = maxVal;

            for (var x = 0; x < this.LoadedStreams; x++)
                this.AllStreams[x].set_Panning(maxVal);
        }
    );

    return function(newThisType) { $thisType = newThisType; };
});