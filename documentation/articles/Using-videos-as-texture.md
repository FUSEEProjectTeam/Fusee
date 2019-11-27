  > ❌ **Outdated**

If you think of using videos as a texture, FUSEE provides two ways to do so.
If you are new to using textures in FUSEE please read the relating wiki-entry first.

First of all, you have to load your video. Just create a `IVideoStream`-variable in your application.
In the `Init()`-function assign a value to this variable by calling `VideoManager.Instance.LoadVideoFromFile("your_video.webm", true)`.<br>
The first parameter is the path to your video (if you want to have a web-build, make sure to have a HTML5-compatible format; check https://developer.mozilla.org/en-US/docs/Web/HTML/Supported_media_formats for further information), the second parameter indicates if the video should be looped.<br>
There is also a third paramter `useAudio` which allows to mute a video. By default, this parameter is set to true. **Note:**  at the moment sound playback only works in the web-build of FUSEE.
In the `RenderAFrame`-routine you have two options how to use the video as a texture.<br>
If you just want to play your video as it is, call `RC.UpdateTextureFromVideoStream(_videoStream, _iTex)`. <br>
This function grabs the current frame from the video every time `RenderAFrame` is called. <br>
You can see that you need an `ITex`-variable just like for static textures. <br>
If you want to manipulate the frames of the video and have direct access to the pixel data, you can do it like this: <br>
```C#
var imgData = _videoStream.GetCurrentFrame();
    if (imgData.PixelData != null)
    {
       if (_iTex == null)
          _iTex = RC.CreateTexture(imgData);
       RC.UpdateTextureRegion(_iTex, imgData, 0, 0);
    }
```
**Note:** having direct access to the pixel data works fine in the C#-build but only very slow in the web-buil. So if you plan to have a web-build, use the first method.<br>
After grabbing the frame, just call `RC.SetShaderParamTexture(_textureParam, _iTex);` to use your texture on a model. Make sure to check if `_iTex` is `null` before calling this function.<br><br>
FUSEE can also use the input of a web-cam as a texture. Just call `VideoManager.InstanceLoadVideoFromCamera(0,false)`. The first parameter is the index of the camera, the other paramter are the same as when loading a video from your disk. To avoid accoustic feedback, the sound is muted by default when loading a video from a live input.

