/* This file contains dummy sourcecode for the FUSEE network implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   Unfortunately, it is not possible to connect two or browsers right now.

	Just for the records: The first version of this file was generated using 
	JSIL v0.7.6 build 16283. From then on it was changed and maintained manually.
*/

var $fuseeInput = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebInput");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebInputDriverImp", true, [], function($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Field({ Static: false, Public: true }, "Devices", $jsilcore.TypeRef("System.Collections.Generic.List`1", [$fuseeCommon.TypeRef("Fusee.Engine.IInputDeviceImp")]));

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, []),
        function _ctor() {

            var callbackClosure = this;
            var tmp = new ($jsilcore.System.Collections.Generic.List$b1.Of($fuseeCommon.Fusee.Engine.IInputDeviceImp))();
            this.Devices = tmp;

            this.Devices.Add(new $fuseeInput.Fusee.Engine.InputDeviceImp());
            this.Devices.Add(new $fuseeInput.Fusee.Engine.InputDeviceImp());
            this.Devices.Add(new $fuseeInput.Fusee.Engine.InputDeviceImp());
            this.Devices.Add(new $fuseeInput.Fusee.Engine.InputDeviceImp());

            window.addEventListener("gamepadconnected", function(event) {
                callbackClosure.GpadConnected.call(callbackClosure, event);
            }, false);

            window.addEventListener("gamepadbuttondown", function(event) {
                callbackClosure.GpadButtonDown.call(callbackClosure, event);
            }, false);

            window.addEventListener("gamepadbuttonup", function(event) {
                callbackClosure.GpadButtonUp.call(callbackClosure, event);
            }, false);

            window.addEventListener("gamepadaxismove", function(event) {
                callbackClosure.GpadAxisMove.call(callbackClosure, event);
            }, false);
        }
    );

    $.Method({ Static: false, Public: true }, "DeviceImps",
        new JSIL.MethodSignature($jsilcore.TypeRef("System.Collections.Generic.List`1", [$fuseeCommon.TypeRef("Fusee.Engine.IInputDeviceImp")]), []),
        function DeviceImps() {
            return this.Devices;
        }
    );

    $.Method({ Static: false, Public: true }, "GpadConnected",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.Object")]),
        function GpadConnected(e) {
            //not implemented
        });

    $.Method({ Static: false, Public: true }, "GpadButtonDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.Object")]),
        function GpadButtonDown(e) {
            this.Devices.get_Item(e.gamepad.index)._buttons[e.button] = true;
        });

    $.Method({ Static: false, Public: true }, "GpadButtonUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.Object")]),
        function GpadButtonUp(e) {
            this.Devices.get_Item(e.gamepad.index)._buttons[e.button] = false;
        });

    $.Method({ Static: false, Public: true }, "GpadAxisMove",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.Object")]),
        function GpadAxisMove(e) {
            this.Devices.get_Item(e.gamepad.index)._axis[e.axis] = e.value;
        });

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IInputDriverImp")
    );

    return function(newThisType) { $thisType = newThisType; };
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.InputDeviceImp", true, [], function($interfaceBuilder) {

    $ = $interfaceBuilder;

    $.Field({ Static: false, Public: true }, "_buttons", $jsilcore.TypeRef("System.Collections.Generic.List`1", $.Boolean), null);
    $.Field({ Static: false, Public: true }, "_axis", $jsilcore.TypeRef("System.Collections.Generic.List`1", $.Double), null);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, []),
        function _ctor() {
            this._buttons = new $jsilcore.TypeRef("System.Collections.Generic.List`1", [$.Boolean]);
            this._axis = new $jsilcore.TypeRef("System.Collections.Generic.List`1", [$.Double]);
            for (var i = 0; i < 10; i++) {
                this._buttons[i] = false;
            }
            for (var i = 0; i < 3; i++) {
                this._axis[i] = 0;
            }
        }
    );

    $.Method({ Static: false, Public: true }, "GetXAxis",
        new JSIL.MethodSignature($.Single, []),
        function GetXAxis() {
            return this._axis[0];
        }
    );

    $.Method({ Static: false, Public: true }, "GetYAxis",
        new JSIL.MethodSignature($.Single, []),
        function GetYAxis() {
            return -this._axis[1];
        }
    );

    $.Method({ Static: false, Public: true }, "GetZAxis",
        new JSIL.MethodSignature($.Single, []),
        function GetZAxis() {
            return this._axis[2];
        }
    );

    $.Method({ Static: false, Public: true }, "GetName",
        new JSIL.MethodSignature($.String, []),
        function GetName() {
            return "Web Gamepad";
        }
    );

    $.Method({ Static: false, Public: true }, "GetPressedButton",
        new JSIL.MethodSignature($.Single, []),
        function GetPressedButton() {
            //not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "IsButtonDown",
        new JSIL.MethodSignature($.Boolean, [$.Single]),
        function IsButtonDown(ind) {
            return this._buttons[ind];
        }
    );

    $.Method({ Static: false, Public: true }, "IsButtonPressed",
        new JSIL.MethodSignature($.Boolean, [$.Single]),
        function IsButtonPressed(ind) {
            var tmp = this._buttons[ind];
            this._buttons[ind] = false;
            return tmp;
        }
    );

    $.Method({ Static: false, Public: true }, "GetButtonCount",
        new JSIL.MethodSignature($.Single, []),
        function GetButtonCount() {
            //not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "GetCategory",
        new JSIL.MethodSignature($.String, []),
        function GetCategory() {
            return "Web Gamepad";
        }
    );

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IInputDeviceImp")
    );

    return function(newThisType) { $thisType = newThisType; };
});