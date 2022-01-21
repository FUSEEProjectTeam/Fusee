
window.init = (instance) => {
    window.theInstance = instance;
    window.requestAnimationFrame(gameLoop);

    const cvs = document.getElementById("canvas");

    const gl = cvs.getContext('webgl2');

    if (!gl || gl.getExtension('EXT_color_buffer_float') === null) {
        console.log('Not a WebGL2 context or EXT_color_buffer_float not supported, this test will fail');
    };

    window.addEventListener("keydown", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnKeyDown', evt.keyCode); });
    window.addEventListener("keyup", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnKeyUp', evt.keyCode); });

    window.addEventListener("resize", (evt) => {

        const w = document.documentElement.clientWidth;
        const h = document.documentElement.clientHeight;

        cvs.width = w;
        cvs.height = h;

        DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnResize', w, h);

    });

    cvs.addEventListener("mousedown", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnMouseDown', evt.button));
    cvs.addEventListener("mouseup", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnMouseUp', evt.button));
    cvs.addEventListener("mousemove", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnMouseMove', evt.offsetX, evt.offsetY));
    cvs.addEventListener("wheel", (evt) => {
        evt.preventDefault();
        DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnMouseWheel', evt.deltaY);
    });

    // fake mouse behaviour
    cvs.addEventListener("touchstart", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnTouchStart', evt.touches[0].identifier); });
    cvs.addEventListener("touchmove", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnTouchMove', evt.touches[0].clientX, evt.touches[0].clientY); });
    // cancel event with mouse behaviour breaks application
    //cvs.addEventListener("touchcancel", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnTouchCancel', evt.changedTouches[0].identifier); });
    cvs.addEventListener("touchend", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.Blazor', 'OnTouchEnd', evt.changedTouches[0].identifier); });
};

function LoadingFinished() {

    document.getElementById("LoadingOverlay").hidden = true;
    document.getElementById("LoadingFinishedOverlay").style.visibility = 'visible';

    // wait 3 seconds
    setTimeout(function () {
        var fadeTarget = document.getElementById("LoadingFinishedOverlay");
        var fadeEffect = setInterval(function () {
            if (!fadeTarget.style.opacity) {
                fadeTarget.style.opacity = 1;
            }
            if (fadeTarget.style.opacity > 0) {
                fadeTarget.style.opacity -= 0.1;
            } else {
                clearInterval(fadeEffect);
                fadeTarget.hidden = true;
            }
        }, 80);
    }, 3000);
}

function getClearColor() {
    const gl2 = document.getElementById("canvas").getContext('webgl2');
    const clearColor = gl2.getParameter(gl2.COLOR_CLEAR_VALUE);
    var r = clearColor[0];
    var g = clearColor[1];
    var b = clearColor[2];
    var a = clearColor[3];
    return [r, g, b, a];
}

function customTexImage3DUInt(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const depth = parameter[5];
    const border = parameter[6];
    const format = parameter[7];
    const type = parameter[8];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 4);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Uint32Array(Module.HEAPU32.buffer, dataPtr, length);
    gl2.texImage3D(target, level, internalformat, width, height, depth, border, format, type, data);
}

function customTexImage2D(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const border = parameter[5];
    const format = parameter[6];
    const type = parameter[7];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 2);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Uint8Array(Module.HEAPU8.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, type, data);
}

function customTexImage2DHalfFlot(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const border = parameter[5];
    const format = parameter[6];
    const type = parameter[7];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 2);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Uint16Array(Module.HEAPU16.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, gl2.HALF_FLOAT, data);
}


function customTexImage2DFloat(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');
    const extension = gl2.getExtension('EXT_color_buffer_half_float');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const border = parameter[5];
    const format = parameter[6];
    const type = parameter[7];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 4);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Float32Array(Module.HEAPF32.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, type, data);
}

function customTexImage2DUInt(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');
    const extension = gl2.getExtension('EXT_color_buffer_half_float');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const border = parameter[5];
    const format = parameter[6];
    const type = parameter[7];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 4);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Uint32Array(Module.HEAPU32.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, type, data);
}


function customTexImage2DDepth16(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');
    const extension = gl2.getExtension('EXT_color_buffer_half_float');

    // extract setting params from array
    const paramPtr = Blazor.platform.getArrayEntryPtr(params, 0, 4);
    const paramLength = Blazor.platform.getArrayLength(params);
    var parameter = new Int32Array(Module.HEAPU8.buffer, paramPtr, paramLength);

    const target = parameter[0];
    const level = parameter[1];
    const internalformat = parameter[2];
    const width = parameter[3];
    const height = parameter[4];
    const border = parameter[5];
    const format = parameter[6];
    const type = parameter[7];

    const dataPtr = Blazor.platform.getArrayEntryPtr(source, 0, 2);
    const length = Blazor.platform.getArrayLength(source);
    const data = new Uint16Array(Module.HEAPU16.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, type, data);
}

function customBufferData(target, data, usage) {

    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');

    if (target == gl2.ARRAY_BUFFER) {
        const dataPtr = Blazor.platform.getArrayEntryPtr(data, 0, 8);
        const length = Blazor.platform.getArrayLength(data);
        const floats = new Float32Array(Module.HEAPU8.buffer, dataPtr, length);
        gl2.bufferData(target, floats, usage);
    }
    else if (target == gl2.ELEMENT_ARRAY_BUFFER) {
        const dataPtr = Blazor.platform.getArrayEntryPtr(data, 0, 2);
        const length = Blazor.platform.getArrayLength(data);
        const ints = new Uint16Array(Module.HEAPU16.buffer, dataPtr, length);
        gl2.bufferData(target, ints, usage);
    } else {
        console.error("Error: Buffer type not found:", target);
    }
}

function generateCtx(contextAttributes) {
    const canvas = document.getElementsByTagName("canvas")[0];
    const gl = canvas.getContext('webgl2', contextAttributes);
    var buf = gl.getExtension('EXT_color_buffer_float');
    console.log("Extension enabled:", buf);
    return gl;
}


function gameLoop(timeStamp) {
    window.requestAnimationFrame(gameLoop);
    theInstance.invokeMethod('Loop', timeStamp);
}

function customAddEventListener(reference, propertyIdentifier, val) {
    reference.addEventListener(propertyIdentifier, val);
}

function setObjectProperty(reference, propertyIdentifier, val) {
    reference[propertyIdentifier] = val;
}

function setAttribute(reference, propertyIdentifier, val) {
    reference.setAttribute(propertyIdentifier, val);
}

function getObjectProperty(reference, property) {
    return reference[property];
}

function getObject(objectToRetrive) {
    if (objectToRetrive === 'window')
        return window;
    if (objectToRetrive === 'document')
        return window.document;
    return document.getElementsByTagName(objectToRetrive)[0];
}
