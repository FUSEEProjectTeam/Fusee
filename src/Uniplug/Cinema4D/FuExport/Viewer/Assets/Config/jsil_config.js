var jsilConfig = {
	libraryRoot: "./Assets/Scripts/",
	manifestRoot: "./Assets/Scripts/",
	scriptRoot: "./Assets/Scripts/",
			
	localStorage: true,
	readOnlyStorage: true,
	contentRoot: "./",
	xna: "3",

	showProgressBar: true,
	premultipliedAlpha: true,

	manifests: [
		"Examples.SceneViewer.exe",
		"Examples.SceneViewer.contentproj",
	],

	updateProgressBar: function (prefix, suffix, bytesLoaded, bytesTotal) {
  		var loadingProgress = document.getElementById("loadingProgress");
  		var progressBar = document.getElementById("progressBar");
  		var progressText = document.getElementById("progressText");
  
  		var w = 0;

  		if (loadingProgress) {
    			w = (bytesLoaded * (loadingProgress.clientWidth+101)) / (bytesTotal);
    			if (w < 0)
      				w = 0;
    			else if (w > (loadingProgress.clientWidth+101))
      				w = (loadingProgress.clientWidth+101);
  		}

  		if (progressBar)
    			progressBar.style.width = w.toString() + "px";

  		if (progressText) {
    			var progressString;

   			if (suffix === null) {
     				progressString = prefix;
    			} else {
      				progressString = prefix + Math.floor(bytesLoaded) + suffix + " / " + Math.floor(bytesTotal) + suffix;
    			}

    			if (jsilConfig.formatProgressText)
      				progressString = jsilConfig.formatProgressText(prefix, suffix, bytesLoaded, bytesTotal, progressString);

    			progressText.textContent = progressString;
    			progressText.style.left = ((loadingProgress.clientWidth - progressText.clientWidth) / 2).toString() + "px";
    			progressText.style.top = ((loadingProgress.clientHeight - progressText.clientHeight) / 2).toString() + "px";
  		}
	}
};