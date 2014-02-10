/* This file contains dummy sourcecode for the FUSEE network implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   Unfortunately, it is not possible to connect two or browsers right now.

	Just for the records: The first version of this file was generated using 
	JSIL v0.7.6 build 16283. From then on it was changed and maintained manually.
*/

var $fuseeNetwork = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebNet");
var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebNetImp", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, []),
        function _ctor() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "CloseConnection",
        new JSIL.MethodSignature(null, [], []),
        function CloseConnection$00() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "CloseConnection",
        new JSIL.MethodSignature(null, [$asm01.TypeRef("Fusee.Engine.SysType")], []),
        function CloseConnection$01(sysType) {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "CloseConnections",
        new JSIL.MethodSignature(null, [], []),
        function CloseConnections() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "CloseDevices",
        new JSIL.MethodSignature(null, [], []),
        function CloseDevices() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "EndPeer",
        new JSIL.MethodSignature(null, [$asm01.TypeRef("Fusee.Engine.SysType")], []),
        function EndPeer(sysType) {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "EndPeers",
        new JSIL.MethodSignature(null, [], []),
        function EndPeers() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "get_Config",
        new JSIL.MethodSignature($asm01.TypeRef("Fusee.Engine.NetConfigValues"), [], []),
        function get_Config() {
            return this;
        }
    );

    $.Method({ Static: false, Public: true }, "get_IncomingMsg",
        new JSIL.MethodSignature($asm05.TypeRef("System.Collections.Generic.List`1", [$asm01.TypeRef("Fusee.Engine.INetworkMsg")]), [], []),
        function get_IncomingMsg() {
            return null;
        }
    );

    $.Method({ Static: false, Public: true }, "get_Status",
        new JSIL.MethodSignature($asm01.TypeRef("Fusee.Engine.NetStatusValues"), [], []),
        function get_Status() {
            return null;
        }
    );

    $.Method({ Static: false, Public: true }, "OnUpdateFrame",
        new JSIL.MethodSignature(null, [], []),
        function OnUpdateFrame() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "OpenConnection",
        new JSIL.MethodSignature($.Boolean, [$asm01.TypeRef("Fusee.Engine.SysType"), $asm06.TypeRef("System.Net.IPEndPoint")], []),
        function OpenConnection$02(type, ip) {
            return false;
        }
    );

    $.Method({ Static: false, Public: true }, "OpenConnection",
        new JSIL.MethodSignature($.Boolean, [
            $asm01.TypeRef("Fusee.Engine.SysType"), $.String,
            $.Int32
        ], []),
        function OpenConnection$03(type, host, port) {
            return false;
        }
    );

    $.Method({ Static: false, Public: false }, "ReadMessage",
        new JSIL.MethodSignature(null, [], []),
        function ReadMessage() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "SendDiscoveryMessage",
        new JSIL.MethodSignature(null, [$.Int32], []),
        function SendDiscoveryMessage(port) {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: false }, "SendDiscoveryResponse",
        new JSIL.MethodSignature(null, [], []),
        function SendDiscoveryResponse() {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "SendMessage",
        new JSIL.MethodSignature($.Boolean, [$jsilcore.TypeRef("System.Array", [$.Byte])], []),
        function SendMessage$04(msg) {
            return false;
        }
    );

    $.Method({ Static: false, Public: true }, "SendMessage",
        new JSIL.MethodSignature($.Boolean, [$.String], []),
        function SendMessage$05(msg) {
            return false;
        }
    );

    $.Method({ Static: false, Public: true }, "SendMessage",
        new JSIL.MethodSignature($.Boolean, [$.Object, $.Boolean], []),
        function SendMessage$06(obj, compress) {
            return false;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Config",
        new JSIL.MethodSignature(null, [$asm01.TypeRef("Fusee.Engine.NetConfigValues")], []),
        function set_Config(value) {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: false }, "set_IncomingMsg",
        new JSIL.MethodSignature(null, [$asm05.TypeRef("System.Collections.Generic.List`1", [$asm01.TypeRef("Fusee.Engine.INetworkMsg")])], []),
        function set_IncomingMsg(value) {
            // not implemented
        }
    );


    $.Method({ Static: false, Public: true }, "set_Status",
        new JSIL.MethodSignature(null, [$asm01.TypeRef("Fusee.Engine.NetStatusValues")], []),
        function set_Status(value) {
            // not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "StartPeer",
        new JSIL.MethodSignature(null, [$.Int32], []),
        function StartPeer(port) {
            // not implemented
        }
    );

    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.INetworkImp")
    );

    return function (newThisType) { $thisType = newThisType; };
});

JSIL.ImplementExternals("Fusee.Engine.Network", function($) {
    $.Method({ Static: false, Public: false }, "FirstMessage",
        new JSIL.MethodSignature($asm01.TypeRef("Fusee.Engine.INetworkMsg"), []),
        function FirstMessage() {
            return null;
        }
    );
});




/* This file contains dummy sourcecode for the FUSEE network implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   Unfortunately, it is not possible to connect two or browsers right now.

	Just for the records: The first version of this file was generated using 
	JSIL v0.7.6 build 16283. From then on it was changed and maintained manually.
*/

//var $fuseeInput = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebInput");
//var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");

//JSIL.DeclareNamespace("Fusee");
//JSIL.DeclareNamespace("Fusee.Engine");

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.InputDriverImp", true, [], function ($interfaceBuilder) {

    $ = $interfaceBuilder;
    $.Field({ Static: false, Public: true }, "Devices", $jsilcore.TypeRef("System.Collections.Generics.List", [$fuseeCommon.TypeRef("Fusee.Engine.DeviceInstance")]), null);

    $.Method({ Static: false, Public: true }, "DeviceImps",
        new JSIL.MethodSignature($asm04.TypeRef("System.Collections.Generic.List`1", [$asm02.TypeRef("Fusee.Engine.IInputDeviceImp")]), []), 
        function DeviceImps() {
        //    createDevices();
        //    var retList = $asm04.TypeRef("System.Collections.Generic.List`1", [$asm02.TypeRef("Fusee.Engine.IInputDeviceImp")]).Construct();

        //    for (var a$0 = this.Devices._items, i$0 = 0, l$0 = this.Devices._size; i$0 < l$0; ($temp00 = i$0, 
        //        i$0 = ((i$0 + 1) | 0), $temp00)) {
        //        var instance = a$0[i$0];
        //        retList.Add(new (Fusee.Engine.InputDeviceImp())(instance));
        //    }
        //    return retList;
        }
    );


    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IInputDriverImp")
    );

    return function (newThisType) { $thisType = newThisType; };
});



JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.InputDeviceImp", true, [], function ($interfaceBuilder) {

    $ = $interfaceBuilder;
    
    $.Method({ Static: false, Public: true }, "GetXAxis",
        new JSIL.MethodSignature($.Single, []),
        function GetXAxis() {
            var value = 0;
            return value;
        }
    );
    
    $.Method({ Static: false, Public: true }, "GetYAxis",
    new JSIL.MethodSignature($.Single, []),
        function GetYAxis() {
            //not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "GetZAxis",
    new JSIL.MethodSignature($.Single, []),
        function GetZAxis() {
            //not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "GetName",
    new JSIL.MethodSignature($.String, []),
        function GetName() {
            //not implemented
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
        function IsButtonDown(button) {
            //not implemented
        }
    );

    $.Method({ Static: false, Public: true }, "IsButtonPressed",
    new JSIL.MethodSignature($.Boolean, [$.Single]),
        function IsButtonPressed(button) {
            //not implemented
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
            //not implemented
        }
    );



    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IInputDeviceImp")
    );

    return function (newThisType) { $thisType = newThisType; };
});




//JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebInputImp", true, [], function($interfaceBuilder) {

//    $ = $interfaceBuilder;


//    $.Method({ Static: false, Public: true }, "GetXAxis",
//        new JSIL.MethodSignature($.Double, [], []),
//        function GetXAxis() {
//            // not implemented
//        }
//    );




//    $.ImplementInterfaces(
//        $fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")
//    );

//    return function (newThisType) { $thisType = newThisType; };
//});

//JSIL.ImplementExternals("Fusee.Engine.Input", function ($) {
//    $.Method({ Static: false, Public: true }, "InitializeDevices",
//        new JSIL.MethodSignature($.Double, []),
//        function InitializeDevices() {
            
//        }
//    );
//});