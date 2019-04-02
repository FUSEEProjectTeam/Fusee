JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Base");
JSIL.DeclareNamespace("Fusee.Base.Core");

JSIL.ImplementExternals("Fusee.Base.Core.Diagnostics", function ($) {
    $.Method({ Static: true, Public: true }, "Log",
        new JSIL.MethodSignature(null, [$.Object]),
        function Log(o) {
            if (typeof window.console !== 'undefined') {
                console.log(o);
            }
        }
    );

    var getTimestamp;
    if (window.performance.now) {
        console.log("Using high performance timer");
        getTimestamp = function() { return window.performance.now(); };
    } else {
        if (window.performance.webkitNow) {
            console.log("Using webkit high performance timer");
            getTimestamp = function() { return window.performance.webkitNow(); };
        } else {
            console.log("Using low performance timer");
            getTimestamp = function() { return new Date().getTime(); };
        }
    }

    $.Method({ Static: true, Public: true }, "get_Timer",
        new JSIL.MethodSignature($.Double, []),
        function get_Timer() {
            return getTimestamp();
        }
    );
});

JSIL.ImplementExternals("Fusee.Base.Core.AsyncHttpAsset", function ($) {

	$.Method({ Static: false, Public: false }, "StartGet",
		new JSIL.MethodSignature(null, null),
		function StartGet() {

			var oReq = new XMLHttpRequest();

			oReq.onload = () => this.JsDoneCallback(oReq.response);
			oReq.onerror = () => this.JsFailCallback();

			oReq.open("GET", this.Id);
			oReq.responseType = "arraybuffer";
			oReq.send();
		}
	);

	$.Method({ Static: true, Public: false }, "WrapString",
		new JSIL.MethodSignature($.String, [$.Object]),
		function WrapString(data) {
			return new TextDecoder("utf-8").decode(data);
		}
	);

	return function (newThisType) { $thisType = newThisType; };
});
