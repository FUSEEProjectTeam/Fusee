window.addEventListener("DOMContentLoaded", init);

function init() {
    LaFuInsertStartScreen("feedback-body");
}
//// Startanimation helpers

var LaFuDivContainer = null;
var LaFuProgressState = 0;
var LaFuAnimStartTime = 0.0;
var LaFuAnimDuration = 500.0;
var LaFuTextMessages = ["LOADING", "INITIALIZING", "MADE WITH"]
var LaFuProgressSteps = [0.5, 0.66, 10];
var LaFuNextThreshold = 0;
// Preload the rocket animation
var rocketAnimImg = new Image();
rocketAnimImg.src = "Assets/FuseeAnim.gif";

function LaFuInitAnim() {
    LaFuAnimStartTime = window.performance.now();
}

function LaFuStartRocket() {
    if (document.getElementById("LaFuProgressText"))      
        document.getElementById("LaFuProgressText").style.opacity = 0;

    animTime = window.performance.now() - LaFuAnimStartTime;
    restTime = LaFuAnimDuration - animTime % LaFuAnimDuration;
    window.setTimeout(LaFuDoChangeImage, restTime);
}

function LaFuDoChangeImage() {
    LaFuNextText(2400);
    window.setTimeout(LaFuShowFuseeText, 2400);
    document.getElementById("LaFusee").src = "Assets/FuseeAnim.gif";
    window.setTimeout(LaFuRemoveAllLoading, 3500);
}

function LaFuRemoveAllLoading() {
    var feedback = document.getElementById("feedback-body");
    feedback.parentElement.removeChild(feedback);
}


function LaFuShowFuseeText() {
    document.getElementById("LaFuseeTxt").style.opacity = 1.0; 
}

function LaFuNextText(timeout) {
    LaFuProgressState++;
    if (LaFuProgressState >= LaFuTextMessages.length)
        LaFuProgressState = LaFuTextMessages.length - 1;
    document.getElementById("LaFuProgressText").style.opacity = 0;
    window.setTimeout(LaFuDoChangeText, timeout);
}

function LaFuDoChangeText() {
    document.getElementById("LaFuProgressText").innerText = LaFuTextMessages[LaFuProgressState];
    document.getElementById("LaFuProgressText").style.opacity = 1;
}

function LaFuSetProgress(progress) {
    if (progress > LaFuProgressSteps[LaFuNextThreshold]) {
        LaFuAdvanceProgress();
        LaFuNextThreshold++;
        if (LaFuNextThreshold >= LaFuProgressSteps.length)
            LaFuNextThreshold = LaFuProgressSteps.length - 1;
    }
}

function LaFuAdvanceProgress() {
    switch (LaFuProgressState) {
        case 0:
            LaFuNextText(500);
            break;
        case 1:
            LaFuStartRocket();
            break;
    }
}

function LaFuInsertStartScreen(someElementId) {
    LaFuDivContainer = document.getElementById(someElementId);
    LaFuDivContainer.innerHTML += "\
	        <img id=\"LaFusee\" \
		        style=\"position:absolute; top:50%; left:50%; margin-left:-57px; margin-top:-96px\" \
		        src=\"Assets/FuseeSpinning.gif\" onload=\"LaFuInitAnim()\"/> \
	        <img id=\"LaFuseeTxt\" \
		        style=\"opacity: 0.0; transition: opacity 1s; position:absolute; top:50%; left:50%; margin-left:-89px; margin-top:93px\" \
		        src=\"Assets/FuseeText.png\"/> \
	        <div style=\"position:absolute; top:50%; left:50%; width:300px; margin-left:-150px; margin-top:-143px\" > \
		        <p id=\"LaFuProgressText\" style=\"text-align:center; font:30px 'Open Sans', sans-serif; font-weight:bold; color:#708090; transition: opacity 0.5s\">LOADING</p></div> ";
}


function updateProgressBar (prefix, suffix, bytesLoaded, bytesTotal) {
        var loadingProgress = document.getElementById("loadingProgress");
        var progressBar = document.getElementById("progressBar");
        var progressText = document.getElementById("progressText");

        if (LaFuDivContainer == null)
            LaFuInsertStartScreen("loadingProgress");

        var progress = 0.0;
        if (prefix.startsWith("Downloading")) {
            progress = 0.5 * bytesLoaded / bytesTotal;
        }
        else if (prefix.startsWith("Loading")) {
            progress = 0.5 + 0.5 * bytesLoaded / bytesTotal;
        }
        else if (prefix.startsWith("Starting")) {
            progress = 1;
        }
        if (progress > 1)
            progress = 1;
        if (progress < 0)
            progress = 0;

        LaFuSetProgress(progress);

        if (loadingProgress && progressBar) {
            if (progress == 1)
                progressBar.style.transition = "width 0s";
            progressBar.style.width = (progress * loadingProgress.clientWidth).toString() + "px";
        }

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
            progressText.style.top = ((loadingProgress.clientHeight - progressText.clientHeight) * 0.75).toString() + "px";
        }
    }

