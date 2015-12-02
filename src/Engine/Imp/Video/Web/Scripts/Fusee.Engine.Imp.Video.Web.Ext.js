var $fuseeInput = JSIL.DeclareAssembly("Fusee.Engine.Imp.AForge");

var $fuseeCore = JSIL.GetAssembly("Fusee.Engine.Core");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");



JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.VideoManagerImp", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;


    $.Method({ Static: false, Public: true }, "CreateVideoStreamImp",
     new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IVideoStreamImp"), $.String),
     function CreateVideoStreamImp(filename) {
         return new $fuseeCommon.Fusee.Engine.IVideoStreamImp(filename);
     }
 );

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IVideoManagerImp")
    );

    return function (newThisType) { $thisType = newThisType; };
});


JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.VideoStreamImp", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Field({ Static: false, Public: false }, "_nextFrame", $fuseeCommon.TypeRef("Fusee.Engine.ImageData"), null);

    $.Method({ Static: false, Public: true }, ".ctor",
       new JSIL.MethodSignature(null, $.String),
       function _ctor(source) {
           var video = document.createElement('video');
           video.id = "videoTexture";
           videoElement.addEventListener("canplaythrough", function (event) {
               callbackClosure.StartVideo.call(callbackClosure, event);
           }, true);
       }
   );

    $.Method({ Static: false, Public: true }, "StartVideo",
        new JSIL.MethodSignature([], []),
        function StartVideo() {
            var videoTexture = document.getElementById("videoTexture");
            videoTexture.play();
            videoElement.addEventListener("timeupdate", function (event) {
                callbackClosure.NextFrame.call(callbackClosure, event);
            }, true);
        }
    );

    $.Method({ Static: false, Public: true }, "NextFrame",
       new JSIL.MethodSignature([], []),
       function NextFrame() {
           var videoTexture = document.getElementById("videoTexture");
           var canvas = document.createElement("canvas");
           canvas.width = videoTexture.videoWidth;
           canvas.height = videoTexture.videoHeight;
           var context = canvas.getContext('2d');
           context.drawImage(videoTexture, 0, 0);

           var myData = context.getImageData(0, 0, videoTexture.videoWidth, videoTexture.videoHeight);
           this._nextFrame.Width = videoTexture.width;
           this._nextFrame.Height = videoTexture.height;
           this._nextFrame.PixelFormat = ImagePixelFormat.RGB;
           this._nextFrame.Stride = image.width * 3; //TODO: Adjust pixel-size
           this._nextFrame.PixelData = myData.data;
       }
   );

    $.Method({ Static: false, Public: true }, "GetCurrentFrame",
     new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.ImageData"), []),
     function DeviceImps() {
         return this._nextFrame;
     }
 );




    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IVideoStreampImp")
    );

    return function (newThisType) { $thisType = newThisType; };
});