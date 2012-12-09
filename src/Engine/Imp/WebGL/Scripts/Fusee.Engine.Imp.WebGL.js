/* This file contains the hand generated part of the FUSEE implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   This file creates the connection to the underlying WebGL part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.5.0 build 25310. Until then it was changed and maintained manually.
*/

var $asmThis = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebGL, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.TheEmptyDummyClass", true, [], function ($) {
   $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IShaderProgramImp"));
     $.Field({ Static: false, Public: false }, "test", $.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.ShaderProgramImp", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IShaderProgramImp"));

    $.Field({ Static: false, Public: true }, "Program",$.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.ShaderParam", true, [], function ($) {

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
    }
  );

    $.Field({ Static: false, Public: true }, "handle",$.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.RenderCanvasImp", true, [], function ($) {
    //var gl;
    //var theCanvas;
    $.Field({ Static: false, Public: false }, "gl", $.Object, null);
    $.Field({ Static: false, Public: true }, "theCanvas", $.Object, null);
    $.Field({ Static: false, Public: false }, "deltaTime", $.Double, null);
    $.Field({ Static: false, Public: false }, "lastFrame", $.Double, null);
    $.Field({ Static: false, Public: false }, "currWidth", $.Int32, null);
    $.Field({ Static: false, Public: false }, "currHeight", $.Int32, null);

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
        this.theCanvas = document.getElementById("canvas");
        this.gl = this.theCanvas.getContext("experimental-webgl");
        this.currWidth = 0;
        this.currHeight = 0;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_DeltaTime",
    new JSIL.MethodSignature($.Double, []),
    function get_DeltaTime() {
        return this.deltaTime / 1000.0;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_Height",
    new JSIL.MethodSignature($.Int32, []),
    function get_Height() {
        return this.gl.drawingBufferHeight;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_Width",
    new JSIL.MethodSignature($.Int32, []),
    function get_Width() {
        return this.gl.drawingBufferWidth;
    }
  );

    $.Field({ Static: false, Public: false }, "IRenderCanvasImp_Init", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Init",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_add_Init(value) {
            var eventHandler = this.IRenderCanvasImp_Init;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Init"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Init",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_remove_Init(value) {
            var eventHandler = this.IRenderCanvasImp_Init;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Init"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );


    $.Method({ Static: false, Public: true }, "DoInit",
    new JSIL.MethodSignature(null, []),
    function DoInit() {
        if (this.IRenderCanvasImp_Init !== null) {
            this.IRenderCanvasImp_Init(this, (new $fuseeCommon.Fusee.Engine.InitEventArgs()).__Initialize__({
        }));
    }
}
  );


$.Field({ Static: false, Public: false }, "IRenderCanvasImp_Render", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
    return null;
});

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Render",
            new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.RenderEventArgs")])]),
            function IRenderCanvasImp_add_Render(value) {
                var eventHandler = this.IRenderCanvasImp_Render;
                do {
                    var eventHandler2 = eventHandler;
                    var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                    eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.RenderEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Render"), value2, eventHandler2);
                } while (eventHandler !== eventHandler2);
            }
        );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Render",
            new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.RenderEventArgs")])]),
            function IRenderCanvasImp_remove_Render(value) {
                var eventHandler = this.IRenderCanvasImp_Render;
                do {
                    var eventHandler2 = eventHandler;
                    var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                    eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.RenderEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Render"), value2, eventHandler2);
                } while (eventHandler !== eventHandler2);
            }
        );



$.Method({ Static: false, Public: true }, "DoRender",
    new JSIL.MethodSignature(null, []),
    function DoRender() {
        if (this.IRenderCanvasImp_Render !== null) {
            this.IRenderCanvasImp_Render(this, (new $fuseeCommon.Fusee.Engine.RenderEventArgs()).__Initialize__({
        }));
    }
}
  );

$.Field({ Static: false, Public: false }, "IRenderCanvasImp_Resize", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
    return null;
});

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Resize",
                new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.ResizeEventArgs")])]),
                function IRenderCanvasImp_add_Resize(value) {
                    var eventHandler = this.IRenderCanvasImp_Resize;
                    do {
                        var eventHandler2 = eventHandler;
                        var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                        eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.ResizeEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Resize"), value2, eventHandler2);
                    } while (eventHandler !== eventHandler2);
                }
            );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Resize",
                new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.ResizeEventArgs")])]),
                function IRenderCanvasImp_remove_Resize(value) {
                    var eventHandler = this.IRenderCanvasImp_Resize;
                    do {
                        var eventHandler2 = eventHandler;
                        var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                        eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.ResizeEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Resize"), value2, eventHandler2);
                    } while (eventHandler !== eventHandler2);
                }
            );



$.Method({ Static: false, Public: true }, "DoResize",
        new JSIL.MethodSignature(null, []),
        function DoResize() {
            if (this.IRenderCanvasImp_Resize !== null) {
                this.IRenderCanvasImp_Resize(this, (new $fuseeCommon.Fusee.Engine.ResizeEventArgs()).__Initialize__({
            }));
        }
    }
      );


$.Method({ Static: false, Public: true }, "IRenderCanvasImp_Present",
    new JSIL.MethodSignature(null, []),
    function IRenderCanvasImp_Present() {
    }
  );


$.Method({ Static: false, Public: true }, "frameTicker",
    new JSIL.MethodSignature(null, []),
    function frameTicker() {
        if (this.currWidth != this.gl.drawingBufferWidth || this.currHeight != this.gl.drawingBufferHeight) {
            this.currWidth = this.gl.drawingBufferWidth;
            this.currHeight = this.gl.drawingBufferHeight;
            this.DoResize();
        }

        var now = +new Date;
        this.DoRender();
        this.deltaTime = now - this.lastFrame;
        this.lastFrame = now;
        window.requestAnimFrame(this.frameTicker.bind(this), this.theCanvas);
    }
  );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_Run",
    new JSIL.MethodSignature(null, []),
    function IRenderCanvasImp_Run() {
        this.lastFrame = +new Date;
        this.deltaTime = 0.0;
        // canvas initialization is now in the constructor:
        // this.theCanvas = document.getElementById("canvas");
        // this.gl = this.theCanvas.getContext("experimental-webgl");
        this.DoInit();
        this.currWidth = 0;
        this.currHeight = 0;
        this.frameTicker();
    }
  );

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_DeltaTime");

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_Height");

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_Width");

});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.RenderContextImp", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IRenderContextImp"));

    $.Field({ Static: false, Public: false }, "gl",$.Object, null);

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.RenderCanvasImp")]),
    function _ctor(renderCanvas) {
        this.gl = document.getElementById("canvas").getContext("experimental-webgl");
        this.gl.enable(this.gl.DEPTH_TEST);
        this.gl.clearColor(0.0, 0.0, 0.2, 1.0);

    }
  );

    // <IRenderContextImp Properties implementation>
    // Note: unlike the method interface-implementations, the Property interface-implementations
    // are NOT to be prefixed with the interface name.
    $.Property({ Static: false, Public: true }, "ModelView");

    $.Method({ Static: false, Public: true }, "get_ModelView",
    new JSIL.MethodSignature($asm00.TypeRef("Fusee.Math.float4x4"), []),
    function get_ModelView() {
        return $asm00.Fusee.Math.float4x4.Identity;
    }
  );

    $.Method({ Static: false, Public: true }, "set_ModelView",
    new JSIL.MethodSignature(null, [$asm00.TypeRef("Fusee.Math.float4x4")]),
    function set_ModelView(value) {
    }
  );

    $.Property({ Static: false, Public: true }, "Projection");

    $.Method({ Static: false, Public: true }, "get_Projection",
    new JSIL.MethodSignature($asm00.TypeRef("Fusee.Math.float4x4"), []),
    function get_Projection() {
        return $asm00.Fusee.Math.float4x4.Identity;
    }
  );

    $.Method({ Static: false, Public: true }, "set_Projection",
    new JSIL.MethodSignature(null, [$asm00.TypeRef("Fusee.Math.float4x4")]),
    function set_Projection(value) {
    }
  );


    $.Property({ Static: false, Public: true }, "ClearColor");

    $.Method({ Static: false, Public: true }, "get_ClearColor",
    new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Math.float4"), []),
    function get_ClearColor() {
        var ret = this.gl.getParameter(this.gl.COLOR_CLEAR_VALUE);
        return new $fuseeCommon.Fusee.Math.float4(ret[0], ret[1], ret[2], ret[3]);
    }
  );

    $.Method({ Static: false, Public: true }, "set_ClearColor",
    new JSIL.MethodSignature(null, [$fuseeCommon.TypeRef("Fusee.Math.float4")]),
    function set_ClearColor(value) {
        this.gl.clearColor(value.x, value.y, value.z, value.w);
    }
  );

    $.Property({ Static: false, Public: true }, "ClearDepth");

    $.Method({ Static: false, Public: true }, "get_ClearDepth",
    new JSIL.MethodSignature($.Single, []),
    function get_ClearDepth() {
        return this.gl.getParameter(this.gl.DEPTH_CLEAR_VALUE); ;
    }
  );

    $.Method({ Static: false, Public: true }, "set_ClearDepth",
    new JSIL.MethodSignature(null, [$.Single]),
    function set_ClearDepth(value) {
        this.gl.clearDepth(value);
    }
  );
    // </IRenderContextImp Properties implementation>



    $.Method({ Static: false, Public: true }, "IRenderContextImp_Clear",
    new JSIL.MethodSignature(null, [$asm02.TypeRef("RenderEngine.ClearFlags")]),
    function IRenderContextImp_Clear(flags) {
        this.gl.clear(flags.value);
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_CreateShader",
    new JSIL.MethodSignature($asmThis.TypeRef("Fusee.Engine.ShaderProgramImp"), [$.String, $.String]),
    function IRenderContextImp_CreateShader(vs, ps) {
        var vertexObject = this.gl.createShader(this.gl.VERTEX_SHADER);
        var fragmentObject = this.gl.createShader(this.gl.FRAGMENT_SHADER);

        // Compile vertex shader
        this.gl.shaderSource(vertexObject, vs);
        this.gl.compileShader(vertexObject);
        var info = this.gl.getShaderInfoLog(vertexObject);
        var statusCode = this.gl.getShaderParameter(vertexObject, this.gl.COMPILE_STATUS);

        if (statusCode != true)
            throw new Error(info);

        // Compile fragment shader
        this.gl.shaderSource(fragmentObject, ps);
        this.gl.compileShader(fragmentObject);
        info = this.gl.getShaderInfoLog(fragmentObject);
        statusCode = this.gl.getShaderParameter(fragmentObject, this.gl.COMPILE_STATUS);

        if (statusCode != true)
            throw new Error(info);

        var program = this.gl.createProgram();
        this.gl.attachShader(program, fragmentObject);
        this.gl.attachShader(program, vertexObject);

        // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
        this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation, $fuseeCommon.Fusee.Engine.Helper.VertexAttribName);
        this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation, $fuseeCommon.Fusee.Engine.Helper.ColorAttribName);
        this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation, $fuseeCommon.Fusee.Engine.Helper.NormalAttribName);

        // Must happen AFTER the bindAttribLocation calls
        this.gl.linkProgram(program);

        var ret = new $asmThis.Fusee.Engine.ShaderProgramImp();
        ret.Program = program;
        return ret;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_GetShaderParam",
    new JSIL.MethodSignature($asmThis.TypeRef("Fusee.Engine.IShaderParam"), [$asmThis.TypeRef("Fusee.Engine.IShaderProgramImp"), $.String]),
    function IRenderContextImp_GetShaderParam(program, paramName) {
        var h = this.gl.getUniformLocation(program.Program, paramName);
        if (h == null)
            return null;
        var ret = new $asmThis.Fusee.Engine.ShaderParam();
        ret.handle = h;
        return ret;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_Render",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IMeshImp")]),
    function IRenderContextImp_Render(mr) {

        if (mr.VertexBufferObject != null) {
            this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.VertexBufferObject);
            this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation, 3, this.gl.FLOAT, false, 0, 0);
        }
        if (mr.ColorBufferObject != null) {
            this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation);
            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.ColorBufferObject);
            this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation, 4, this.gl.FLOAT, false, 0, 0);
        }
        if (mr.NormalBufferObject != null) {
            this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation);
            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.NormalBufferObject);
            this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation, 3, this.gl.FLOAT, false, 0, 0);
        }
        if (mr.ElementBufferObject != null) {
            this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, mr.ElementBufferObject);
            this.gl.drawElements(this.gl.TRIANGLES, mr.NElements, this.gl.UNSIGNED_SHORT, 0);
            //this.gl.DrawArrays(this.gl.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
        }
        if (mr.VertexBufferObject != null) {
            this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
        }
        if (mr.ColorBufferObject != null) {
            this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation);
        }
        if (mr.NormalBufferObject != null) {
            this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation);
        }
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetShader",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderProgramImp")]),
    function IRenderContextImp_SetShader(program) {
        this.gl.useProgram(program.Program);
    }
  );

    $.Method({ Static: false, Public: true }, "SetShaderParam1f",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderParam"), $.Single]),
    function SetShaderParam1f(param, val) {
        this.gl.uniform1f(param.handle, val);
    }
  );

    $.Method({ Static: false, Public: true }, "SetShaderParam2f",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderParam"), $asm00.TypeRef("Fusee.Math.float2")]),
    function SetShaderParam2f(param, val) {
        this.gl.uniform2f(param.handle, val);
    }
  );

    $.Method({ Static: false, Public: true }, "SetShaderParam3f",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderParam"), $asm00.TypeRef("Fusee.Math.float3")]),
    function SetShaderParam3f(param, val) {
        this.gl.uniform3f(param.handle, val);
    }
  );

    $.Method({ Static: false, Public: true }, "SetShaderParam4f",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderParam"), $asm00.TypeRef("Fusee.Math.float4")]),
    function SetShaderParam4f(param, val) {
        var flatVector = new Float32Array(val.ToArray());
        this.gl.uniform4fv(param.handle, flatVector);
    }
  );

    $.Method({ Static: false, Public: true }, "SetShaderParamMtx4f",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IShaderParam"), $asm00.TypeRef("Fusee.Math.float4x4")]),
    function SetShaderParamMtx4f(param, val) {
        var flatMatrix = new Float32Array(val.ToArray());
        this.gl.uniformMatrix4fv(param.handle, false, flatMatrix);
    }
  );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_Viewport",
    new JSIL.MethodSignature(null, [
        $.Int32, $.Int32,
        $.Int32, $.Int32
      ]),
    function IRenderContextImp_Viewport(x, y, width, height) {
        this.gl.viewport(x, y, width, height);
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetColors",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$.UInt32])]),
    function IRenderContextImp_SetColors(mr, colors) {
        if (colors == null || colors.length == 0) {
            throw new Exception("colors must not be null or empty");
        }

        var vboBytes;
        var colsBytes = colors.length * 4 * 4;
        if (mr.ColorBufferObject == null)
            mr.ColorBufferObject = this.gl.createBuffer();

        var nInts = colors.length;
        var flatBuffer = new Float32Array(colors.length * 4);
        for (var i = 0; i < colors.length; i++) {
            flatBuffer[4 * i] = (colors[i] & 0xFF) / 255.0;
            flatBuffer[4 * i + 1] = ((colors[i] >> 8) & 0xFF) / 255.0;
            flatBuffer[4 * i + 2] = ((colors[i] >> 16) & 0xFF) / 255.0;
            flatBuffer[4 * i + 3] = ((colors[i] >> 24) & 0xFF) / 255.0;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.ColorBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != colsBytes)
            throw new Exception("Problem uploading color buffer to VBO (colors). Tried to upload " + colsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetNormals",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$asm00.TypeRef("Fusee.Math.float3")])]),
    function IRenderContextImp_SetNormals(mr, normals) {
        if (normals == null || normals.length == 0) {
            throw new Exception("Normals must not be null or empty");
        }

        var vboBytes;
        var normsBytes = normals.length * 3 * 4;
        if (mr.NormalBufferObject == null)
            mr.NormalBufferObject = this.gl.createBuffer();

        var nFloats = normals.length * 3;
        var flatBuffer = new Float32Array(nFloats);
        for (var i = 0; i < normals.length; i++) {
            flatBuffer[3 * i + 0] = normals[i].x;
            flatBuffer[3 * i + 1] = normals[i].y;
            flatBuffer[3 * i + 2] = normals[i].z;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.NormalBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != normsBytes)
            throw new Exception("Problem uploading normal buffer to VBO (normals). Tried to upload " + normsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetTriangles",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$.Int16])]),
    function IRenderContextImp_SetTriangles(mr, triangleIndices) {
        if (triangleIndices == null || triangleIndices.length == 0) {
            throw new Exception("triangleIndices must not be null or empty");
        }
        mr.NElements = triangleIndices.length;
        var vboBytes;
        var trisBytes = triangleIndices.length * 2;

        if (mr.ElementBufferObject == null)
            mr.ElementBufferObject = this.gl.createBuffer();

        var nInts = triangleIndices.length;
        var flatBuffer = new Int16Array(nInts);
        for (var i = 0; i < triangleIndices.length; i++) {
            flatBuffer[i] = triangleIndices[i];
        }

        // Upload the   index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
        this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, mr.ElementBufferObject);
        this.gl.bufferData(this.gl.ELEMENT_ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ELEMENT_ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != trisBytes)
            throw new Exception("Problem uploading vertex buffer to VBO (offsets). Tried to upload " + normsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetVertices",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$asm00.TypeRef("Fusee.Math.float3")])]),
    function IRenderContextImp_SetVertices(mr, vertices) {
        if (vertices == null || vertices.length == 0) {
            throw new Exception("vertices must not be null or empty");
        }

        var vboBytes;
        var vertsBytes = vertices.length * 3 * 4;
        if (mr.VertexBufferObject == null)
            mr.VertexBufferObject = this.gl.createBuffer();

        var nFloats = vertices.length * 3;
        var flatBuffer = new Float32Array(nFloats);
        for (var i = 0; i < vertices.length; i++) {
            flatBuffer[3 * i + 0] = vertices[i].x;
            flatBuffer[3 * i + 1] = vertices[i].y;
            flatBuffer[3 * i + 2] = vertices[i].z;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.VertexBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != vertsBytes)
            throw new Exception("Problem uploading normal buffer to VBO (vertices). Tried to upload " + vertsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_CreateMeshImp",
    new JSIL.MethodSignature($asmThis.TypeRef("Fusee.Engine.IMeshImp"), []),
    function IRenderContextImp_CreateMeshImp() {
        return new $asmThis.Fusee.Engine.MeshImp();
    }
  );


});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.MeshImp", true, [], function ($) {
   $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IMeshImp"));

    $.Field({ Static: false, Public: true }, "VertexBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "NormalBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "ColorBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "ElementBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "NElements", $.Int32, null);

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_ColorsSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_ColorsSet() {
        return this.ColorBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_NormalsSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_NormalsSet() {
        return this.NormalBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_TrianglesSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_TrianglesSet() {
        return this.ElementBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_VerticesSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_VerticesSet() {
        return this.VertexBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateColors",
    new JSIL.MethodSignature(null, []),
    function InvalidateColors() {
        this.ColorBufferObject = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateNormals",
    new JSIL.MethodSignature(null, []),
    function InvalidateNormals() {
        this.NormalBufferObject = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateTriangles",
    new JSIL.MethodSignature(null, []),
    function InvalidateTriangles() {
        this.ElementBufferObject = null;
        this.NElements = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateVertices",
    new JSIL.MethodSignature(null, []),
    function InvalidateVertices() {
        this.VertexBufferObject = null;
    }
  );

    $.Property({ Static: false, Public: true }, "ColorsSet");

    $.Property({ Static: false, Public: true }, "NormalsSet");

    $.Property({ Static: false, Public: true }, "TrianglesSet");

    $.Property({ Static: false, Public: true }, "VerticesSet");

});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.InputImp", true, [], function ($) {

    /*
    $.Field({Static:false, Public:true}, "_currentMMouse", $fuseeCommon.TypeRef("Fusee.Engine.Point"), null);
    $.Field({Static:false, Public:true}, "NElements",  $.Int32, null);
    protected GameWindow _gameWindow;
    */
    $.Field({ Static: false, Public: true }, "_currentMouse", $fuseeCommon.TypeRef("Fusee.Engine.Point"), null);
    $.Field({ Static: false, Public: true }, "_mouseDown", $.Boolean, null);
    $.Field({ Static: false, Public: true }, "_currentMouseWheel", $.Int32, null);

    $.Method({ Static: false, Public: true }, "OnCanvasMouseDown",
        new JSIL.MethodSignature(null, []),
        function OnCanvasMouseDown(event) {
            var mb = $fuseeCommon.Fusee.Engine.MouseButtons.Unknown;
            switch (event.button) {
                case 0:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Left;
                    break;
                case 1:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Middle;
                    break;
                case 2:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Right;
                    break;
            }

            if (this.IInputImp_MouseButtonDown !== null) {
				var pt = new $fuseeCommon.Fusee.Engine.Point().__Initialize__({x: event.clientX, y: event.clientY});
                this.IInputImp_MouseButtonDown(this, (new $fuseeCommon.Fusee.Engine.MouseEventArgs()).__Initialize__({
                    Button: mb,
                    Position: pt
                }));
            }
        }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasMouseUp",
        new JSIL.MethodSignature(null, []),
        function OnCanvasMouseUp(event) {
            var mb = $fuseeCommon.Fusee.Engine.MouseButtons.Unknown;
            switch (event.button) {
                case 0:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Left;
                    break;
                case 1:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Middle;
                    break;
                case 2:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Right;
                    break;
            }

            if (this.IInputImp_MouseButtonUp !== null) {
				var pt = new $fuseeCommon.Fusee.Engine.Point().__Initialize__({x: event.clientX, y: event.clientY});
                this.IInputImp_MouseButtonUp(this, (new $fuseeCommon.Fusee.Engine.MouseEventArgs()).__Initialize__({
                    Button: mb,
                    Position: pt
                }));
            }
        }
    );


    $.Method({ Static: false, Public: true }, "OnCanvasKeyDown",
        new JSIL.MethodSignature(null, []),
        function OnCanvasKeyDown(event) {
            if (this.IInputImp_KeyDown !== null) {
                this.IInputImp_KeyDown(this, (new $fuseeCommon.Fusee.Engine.KeyEventArgs()).__Initialize__({
                    Shift: event.shiftKey,
                    Alt: event.altKey,
                    Control: event.ctrlKey,
                    KeyCode: event.keyCode
                }));
            }
        }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasKeyUp",
        new JSIL.MethodSignature(null, []),
        function OnCanvasKeyUp(event) {
            if (this.IInputImp_KeyUp !== null) {
                this.IInputImp_KeyUp(this, (new $fuseeCommon.Fusee.Engine.KeyEventArgs()).__Initialize__({
                    Shift: event.shiftKey,
                    Alt: event.altKey,
                    Control: event.ctrlKey,
                    KeyCode: event.keyCode
                }));
            }
        }
    );


    $.Method({ Static: false, Public: true }, "OnCanvasMouseMove",
      new JSIL.MethodSignature(null, []),
      function OnCanvasMouseMove(event) {
          this._currentMouse.x = event.clientX;
          this._currentMouse.y = event.clientY;
      }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasMouseWheel",
      new JSIL.MethodSignature(null, []),
      function OnCanvasMouseWheel(event) {
          this._currentMouseWheel += event.wheelDelta;
      }
    );

    // IInputImp implementation
    $.Method({ Static: false, Public: true }, "IInputImp_GetMouseWheelPos",
        new JSIL.MethodSignature($.Int32, []),
            function IInputImp_GetMouseWheelPos(event) {
                return this._currentMouseWheel;
            }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_GetMousePos",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.Point"), []),
            function IInputImp_GetMousePos(event) {
                return this._currentMouse.MemberwiseClone();
            }
    );

    // KeyDown event
    $.Field({ Static: false, Public: false }, "IInputImp_KeyDown", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_KeyDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_add_KeyDown(value) {
            var eventHandler = this.IInputImp_KeyDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_KeyDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_remove_KeyDown(value) {
            var eventHandler = this.IInputImp_KeyDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    // KeyUp event
    $.Field({ Static: false, Public: false }, "IInputImp_KeyUp", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_KeyUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_add_KeyUp(value) {
            var eventHandler = this.IInputImp_KeyUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_KeyUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_remove_KeyUp(value) {
            var eventHandler = this.IInputImp_KeyUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );


    // MouseButtonDown event
    $.Field({ Static: false, Public: false }, "IInputImp_MouseButtonDown", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_MouseButtonDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_add_MouseButtonDown(value) {
            var eventHandler = this.IInputImp_MouseButtonDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_MouseButtonDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_remove_MouseButtonDown(value) {
            var eventHandler = this.IInputImp_MouseButtonDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    // MouseButtonUp event
    $.Field({ Static: false, Public: false }, "IInputImp_MouseButtonUp", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_MouseButtonUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_add_MouseButtonUp(value) {
            var eventHandler = this.IInputImp_MouseButtonUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_MouseButtonUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_remove_MouseButtonUp(value) {
            var eventHandler = this.IInputImp_MouseButtonUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );




    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, [$asmThis.TypeRef("Fusee.Engine.RenderCanvasImp")]),
    function _ctor(renderCanvas) {

        if (renderCanvas == null)
            throw new Exception("renderCanvas must not be null");

        this._currentMouse = new $fuseeCommon.Fusee.Engine.Point();
        this._currentMouse.x = 0;
        this._currentMouse.y = 0;
        var callbackClosure = this;
        renderCanvas.theCanvas.onmousedown = function (event) {
            callbackClosure.OnCanvasMouseDown.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmouseup = function (event) {
            callbackClosure.OnCanvasMouseUp.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmousemove = function (event) {
            callbackClosure.OnCanvasMouseMove.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmousewheel = function (event) {
            callbackClosure.OnCanvasMouseWheel.call(callbackClosure, event);
        };
        document.onkeydown = function (event) {
            callbackClosure.OnCanvasKeyDown.call(callbackClosure, event);
        };
        document.onkeyup = function (event) {
            callbackClosure.OnCanvasKeyUp.call(callbackClosure, event);
        };
    });
});


JSIL.ImplementExternals("Fusee.Engine.ImpFactory", function ($) {


    $.Method({ Static: true, Public: true }, "CreateIInputImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IInputImp"), [$fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp")]),
            function ImpFactory_CreateIInputImp(renderCanvasImp) {
                return new $asmThis.Fusee.Engine.InputImp(renderCanvasImp);
            }
    );

    $.Method({ Static: true, Public: true }, "CreateIRenderCanvasImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp"), []),
            function ImpFactory_CreateIRenderCanvasImp(renderCanvasImp) {
				// return new $asmThis.Fusee.Engine.TheEmptyDummyClass
                return new $asmThis.Fusee.Engine.RenderCanvasImp();
            }
    );

    $.Method({ Static: true, Public: true }, "CreateIRenderContextImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IRenderContextImp"), [$fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp")]),
            function ImpFactory_CreateIRenderContextImp(renderCanvasImp) {
                return new $asmThis.Fusee.Engine.RenderContextImp(renderCanvasImp);
            }
    );

});


JSIL.ImplementExternals("Fusee.Engine.MeshReader", function ($) {
    $.Method({ Static: true, Public: true }, "Double_Parse",
        new JSIL.MethodSignature($.Double, [$.String]),
            function Double_Parse(str) {
                return Number(str);
            }
    );
});


var FUSEE_GLOBAL_TimerFunc = null;
var FUSEE_GLOBAL_first = 0.0;

JSIL.ImplementExternals("Fusee.Engine.Diagnostics", function ($) {

    $.Method({ Static: true, Public: true }, "get_Timer",
    new JSIL.MethodSignature($.Double, []),
        function get_Timer() {
            if (FUSEE_GLOBAL_TimerFunc === null) {
                if ("now" in window.performance && window.performance.now !== null) {
                    FUSEE_GLOBAL_TimerFunc = window.performance.now;
                }
                else if ("webkitNow" in window.performance && window.performance.webkitNow !== null) {
                    FUSEE_GLOBAL_TimerFunc = window.performance.webkitNow;
                } else {
                    FUSEE_GLOBAL_first = +new Date;
                    FUSEE_GLOBAL_TimerFunc = function () {
                        var now = +new Date;
                        return (now - FUSEE_GLOBAL_first) / 1000.0;
                    };
                }
            }
            return FUSEE_GLOBAL_TimerFunc.call(window.performance);
        }
    );

    $.Method({ Static: true, Public: true }, "Log",
    new JSIL.MethodSignature(null, [$.Object]),
        function Log(o) {
            return console.log(o.toString());
        }
    );


});



/**
* Provides requestAnimationFrame in a cross browser way.
*/
window.requestAnimFrame = (function () {
    return window.requestAnimationFrame ||
         window.webkitRequestAnimationFrame ||
         window.mozRequestAnimationFrame ||
         window.oRequestAnimationFrame ||
         window.msRequestAnimationFrame ||
         function (/* function FrameRequestCallback */callback, /* DOMElement Element */element) {
             return window.setTimeout(callback, 1000 / 60);
         };
})();

/**
* Provides cancelAnimationFrame in a cross browser way.
*/
window.cancelAnimFrame = (function () {
    return window.cancelAnimationFrame ||
         window.webkitCancelAnimationFrame ||
         window.mozCancelAnimationFrame ||
         window.oCancelAnimationFrame ||
         window.msCancelAnimationFrame ||
         window.clearTimeout;
})();