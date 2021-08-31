
window.init = (instance) => {
    window.theInstance = instance;
    window.requestAnimationFrame(gameLoop);

    const cvs = document.getElementById("canvas");
    window.addEventListener("keydown", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnKeyDown', evt.keyCode); });
    window.addEventListener("keyup", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnKeyUp', evt.keyCode); });

    window.addEventListener("resize", (evt) => {

        const w = document.documentElement.clientWidth;
        const h = document.documentElement.clientHeight;

        cvs.width = w;
        cvs.height = h;

        DotNet.invokeMethod('Fusee.Base.Imp.WebAsm', 'OnResize', w, h);

    });

    cvs.addEventListener("mousedown", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnMouseDown', evt.button));
    cvs.addEventListener("mouseup", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnMouseUp', evt.button));
    cvs.addEventListener("mousemove", (evt) => DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnMouseMove', evt.offsetX, evt.offsetY));
    cvs.addEventListener("wheel", (evt) => {
        evt.preventDefault();
        DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnMouseWheel', evt.deltaY);
    });

    cvs.addEventListener("touchstart", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnTouchStart', evt); });
    cvs.addEventListener("touchmove", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnTouchMove', evt); });
    cvs.addEventListener("touchcancel", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnTouchCancel', evt); });
    cvs.addEventListener("touchend", (evt) => { evt.preventDefault(); DotNet.invokeMethod('Fusee.Engine.Imp.Graphics.WebAsm', 'OnTouchEnd', evt); });


    // Loading and installation of event listener complete, hide loading overlay
    // Loading complete, hide loading overlay
    document.getElementById("LoadingOverlay").hidden = true;
};

function getClearColor() {
    const gl2 = document.getElementById("canvas").getContext('webgl2');
    const clearColor = gl2.getParameter(gl2.COLOR_CLEAR_VALUE);
    var r = clearColor[0];
    var g = clearColor[1];
    var b = clearColor[2];
    var a = clearColor[3];
    return [r, g, b, a];
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

function customTexImage2DFloat(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');
    const extension = gl2.getExtension('EXT_color_buffer_half_float');
    console.log(extension);

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
    const data = new Float32Array(Module.HEAPU8.buffer, dataPtr, length);
    gl2.texImage2D(target, level, internalformat, width, height, border, format, type, data);
}

function customTexImage2DInt(params, source) {
    const gl2 = document.getElementsByTagName("canvas")[0].getContext('webgl2');
    const extension = gl2.getExtension('EXT_color_buffer_half_float');
    console.log(extension);

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
    const data = new Uint32Array(Module.HEAPU8.buffer, dataPtr, length);
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
        const ints = new Uint16Array(Module.HEAPU8.buffer, dataPtr, length);
        gl2.bufferData(target, ints, usage);
    } else {
        console.error("Error: Buffer type not found:", target);
    }
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
