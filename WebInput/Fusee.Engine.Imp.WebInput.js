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

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.WebInputImp", true, [], function($interfaceBuilder) {

    $ = $interfaceBuilder;


    $.Method({ Static: false, Public: true }, "GetXAxis",
        new JSIL.MethodSignature($.Double, [], []),
        function GetXAxis() {
            // not implemented
        }
    );




    $.ImplementInterfaces(
        $fuseeCommon.TypeRef("Fusee.Engine.IAudioStream")
    );

    return function (newThisType) { $thisType = newThisType; };
});