JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Base");
JSIL.DeclareNamespace("Fusee.Base.Common");


JSIL.ImplementExternals("Fusee.Base.Common.AsyncHttpAsset`1", function ($) {

	$.Method({ Static: false, Public: false }, "StartGet",
		new JSIL.MethodSignature(null, null),
		function StartGet() {

			var oReq = new XMLHttpRequest();

			oReq.onload = () => this.JsDoneCallback(oReq.response);
			oReq.onerror = () => this.JsFailCallback();

			oReq.open("GET", this.id);
			oReq.responseType = "arraybuffer";
			oReq.send();
		}
	);

	return function (newThisType) { $thisType = newThisType; };
});