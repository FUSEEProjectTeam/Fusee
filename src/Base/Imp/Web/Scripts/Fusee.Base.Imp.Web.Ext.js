var $fuseeBaseCommon = JSIL.GetAssembly("Fusee.Base.Common");
var $WebBaseCore = JSIL.GetAssembly("Fusee.Base.Core");
var $fuseeMathCore = JSIL.GetAssembly("Fusee.Math.Core");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Base");
JSIL.DeclareNamespace("Fusee.Base.Imp");
JSIL.DeclareNamespace("Fusee.Base.Imp.Web");

JSIL.ImplementExternals("Fusee.Base.Imp.Web.WebAssetProvider", function ($) {
	// public static ImageData LoadImage(object assetOb)

	// private bool CheckExists(string id)
	$.Method({ Static: false, Public: false }, "CheckExists",
		new JSIL.MethodSignature($.Boolean, [$.String]),
		function CheckExists(id) {
			var idName = "assets/" + id.toLowerCase();
			var idNoExt = idName.substring(0, idName.lastIndexOf("."));
			return allFiles.hasOwnProperty(idName) || allAssets.hasOwnProperty(idNoExt);
		}
	);

	// private object GetRawAsset(string id)
	$.Method({ Static: false, Public: false }, "GetRawAsset",
		new JSIL.MethodSignature($.Object, [$.String]),
		function GetRawAsset(id) {
			var idName = "assets/" + id.toLowerCase();
			var idNoExt = idName.substring(0, idName.lastIndexOf("."));
			if (allFiles.hasOwnProperty(idName))
				return allFiles[idName];

			if (allAssets.hasOwnProperty(idNoExt))
				return allAssets[idNoExt];

			return null;
		}
	);

	return function (newThisType) { $thisType = newThisType; };
});

JSIL.ImplementExternals("Fusee.Base.Imp.Web.FileDecoder", function ($) {
	// public static ImageData LoadImage(object assetOb)
	$.Method({ Static: true, Public: true }, "WrapImageJsil",
		new JSIL.MethodSignature($WebBaseCore.TypeRef("Fusee.Base.Core.ImageData"), [$.Object]),
		function WrapImageJsil(assetOb) {
			return DoWrapImage(assetOb.image);
		}
	);

	$.Method({ Static: true, Public: true }, "WrapImageAsync",
		new JSIL.MethodSignature(null, [$.Object, $.Object]),
		function WrapImageAsync(arrayBuffer, callback) {
			var bytes = new Uint8Array(arrayBuffer);

			var image = new Image();
			image.onload = function () {
				callback(DoWrapImage(image));
			};
			image.src = 'data:image/png;base64,' + encode(bytes);
		}
	);

	function encode(input) {
		var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
		var output = "";
		var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
		var i = 0;

		while (i < input.length) {
			chr1 = input[i++];
			chr2 = i < input.length ? input[i++] : Number.NaN; // Not sure if the index 
			chr3 = i < input.length ? input[i++] : Number.NaN; // checks are needed here

			enc1 = chr1 >> 2;
			enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
			enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
			enc4 = chr3 & 63;

			if (isNaN(chr2)) {
				enc3 = enc4 = 64;
			} else if (isNaN(chr3)) {
				enc4 = 64;
			}
			output += keyStr.charAt(enc1) + keyStr.charAt(enc2) +
				keyStr.charAt(enc3) + keyStr.charAt(enc4);
		}
		return output;
	}

	function DoWrapImage(image) {
		// Akquire and copy pixel data
		var canvas = document.createElement("canvas");
		canvas.width = image.width;
		canvas.height = image.height;
		var context = canvas.getContext("2d");
		context.translate(canvas.width / 2, canvas.height / 2);
		context.scale(1, -1);
		context.translate(-canvas.width / 2, -canvas.height / 2);
		context.drawImage(image, 0, 0);
		var myData = context.getImageData(0, 0, image.width, image.height);
		// Create and initialize return object (FUSEE ImageData)
		var imageData = new $WebBaseCore.Fusee.Base.Core.ImageData(myData.data, image.width, image.height, new $fuseeBaseCommon.Fusee.Base.Common.ImagePixelFormat($fuseeBaseCommon.Fusee.Base.Common.ColorFormat.RGBA));
		return imageData;
	}

	$.Method({ Static: true, Public: true }, "WrapString",
		new JSIL.MethodSignature($.String, [$.Object]),
		function WrapString(assetOb) {

			if (!('TextDecoder' in window)) {
				// assume 1:1 text file contents
				return String.fromCharCode.apply(null, new Uint8Array(assetOb.buffer));
			} else {
				// The decode() method takes a DataView as a parameter, which is a wrapper on top of the ArrayBuffer.
				var dataView = new DataView(assetOb.buffer);
				// The TextDecoder interface is documented at http://encoding.spec.whatwg.org/#interface-textdecoder
				// Possible TExtDecoder constructor parameters.
				//  'utf-8',
				//  'utf-16le',
				//  'macintosh'
				var decoder = new TextDecoder('utf-8');
				var string = decoder.decode(dataView.buffer);

				return string;
			}
		}
	);

	return function (newThisType) { $thisType = newThisType; };
});

JSIL.ImplementExternals("Fusee.Base.Imp.Web.FontImp",
	function ($) {

		var _face;

		//public FontImp(object storage)
		$.Method({ Static: false, Public: true },
			".ctor",
			new JSIL.MethodSignature(null, [$.Object]),
			function _ctor(storage) {
				this._face = opentype.parse(storage.buffer);
			}
		);

		//public GlyphInfo GetGlyphInfo(uint c)
		$.Method({ Static: false, Public: true },
			"GetGlyphInfo",
			new JSIL.MethodSignature($fuseeBaseCommon.TypeRef("Fusee.Base.Common.GlyphInfo"), [$.UInt32]),
			function GetGlyphInfo(c) {
				var glyph = this._face.charToGlyph(String.fromCharCode(c));
				var fontSize = this.PixelHeight;
				var fontScale = 1 / this._face.unitsPerEm * fontSize;

				var glyphInfo = new $fuseeBaseCommon.Fusee.Base.Common.GlyphInfo();
				// TODO: maybe normalize values with 1 / this._face.unitsPerEm
				glyphInfo.CharCode = c;
				glyphInfo.AdvanceX = glyph.advanceWidth * fontScale;
				glyphInfo.AdvanceY = 0;
				glyphInfo.Width = (glyph.xMax - glyph.xMin) * fontScale;
				glyphInfo.Height = (glyph.yMax - glyph.yMin) * fontScale;
				return glyphInfo;
			}
		);

		//public Curve GetGlyphCurve(uint c)
		$.Method({ Static: false, Public: true },
			"GetGlyphCurve",
			new JSIL.MethodSignature($fuseeMathCore.TypeRef("Fusee.Math.Core.Curve"), [$.UInt32]),
			function GetGlyphCurve(c) {

				var glyph = this._face.charToGlyph(String.fromCharCode(c));
				var curve = new $fuseeMathCore.Fusee.Math.Core.Curve();

				curve.CurveParts = new ($jsilcore.System.Collections.Generic.List$b1
					.Of($fuseeMathCore.Fusee.Math.Core.CurvePart))();

				var contour = [];

				if (glyph.points !== null) {
					for (var i = 0; i < glyph.points.length; i++) {
						if (!glyph.points[i].lastPointOfContour) {
							contour.push(glyph.points[i]);
						} else {
							contour.push(glyph.points[i]);

							var cp = new $fuseeMathCore.Fusee.Math.Core.CurvePart();

							cp.Closed = true;

							cp.StartPoint = new $fuseeMathCore.Fusee.Math.Core.float3();
							cp.StartPoint.x = contour[0].x;
							cp.StartPoint.y = contour[0].y;
							cp.StartPoint.z = 0;

							cp.CurveSegments = new ($jsilcore.System.Collections.Generic.List$b1
								.Of($fuseeMathCore.Fusee.Math.Core.CurveSegment))();

							var partVertices = new ($jsilcore.System.Collections.Generic.List$b1
								.Of($fuseeMathCore.Fusee.Math.Core.float3))();
							var partTags = new ($jsilcore.System.Collections.Generic.List$b1
								.Of($jsilcore.System.Byte))();

							for (var j = 0; j < contour.length; j++) {
								var x = contour[j].x;
								var y = contour[j].y;
								var z = 0;
								partVertices.Add(new $fuseeMathCore.Fusee.Math.Core.float3(x, y, z));

								var tag = contour[j].onCurve ? 1 : 0;
								partTags.Add(tag);
							}

							contour = [];
							curve.CurveParts.Add(cp);

							var helper = $fuseeBaseCommon.Fusee.Base.Common.SplitToCurveSegmentHelper;
							var segments = helper.SplitPartIntoSegments(cp, partTags, partVertices);
							helper.CombineCurveSegmentsAndAddThemToCurvePart(segments, cp);

							partVertices.Clear();
							partTags.Clear();
						}
					}
				}
				return curve;
			}
		);

		//public float GetUnscaledAdvance(uint c)
		$.Method({ Static: false, Public: true }, "GetUnscaledAdvance",
			new JSIL.MethodSignature($.Single, [$.UInt32]),
			function GetUnscaledAdvance(c) {

				var glyph = this._face.charToGlyph(String.fromCharCode(c));
				var advanceWidth = glyph.advanceWidth;
				return advanceWidth;
			}
		);

		//public ImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
		$.Method({ Static: false, Public: true }, "RenderGlyph",
			new JSIL.MethodSignature($fuseeBaseCommon.TypeRef("Fusee.Base.Common.IImageData"), [
				$.UInt32, $jsilcore.TypeRef("JSIL.Reference", [$.Int32]),
				$jsilcore.TypeRef("JSIL.Reference", [$.Int32])
			]),
			function RenderGlyph(c, /* ref */ bitmapLeft, /* ref */ bitmapTop) {
				var glyph = this._face.charToGlyph(String.fromCharCode(c));

				var fontSize = this.PixelHeight;
				var fontScale = 1 / this._face.unitsPerEm * fontSize;
				var xMin = ~ ~(glyph.xMin * fontScale);
				var yMin = ~ ~(glyph.yMin * fontScale);
				var xMax = ~ ~(glyph.xMax * fontScale + 1);
				var yMax = ~ ~(glyph.yMax * fontScale + 1);

				var bmpWidth = xMax - xMin;
				var bmpRows = yMax - yMin;

				var retImage; // = new $WebBaseCore.Fusee.Base.Core.ImageData();
				var bmpLeft = +0;
				var bmpTop = +0;
				if (bmpWidth > 0 && bmpRows > 0) {
					var canvas = document.createElement("canvas");
					canvas.width = bmpWidth;
					canvas.height = bmpRows;
					var ctx = canvas.getContext("2d");

					glyph.draw(ctx, -xMin, yMax, fontSize);

					var bitmap = ctx.getImageData(0, 0, canvas.width, canvas.height);
					var alpha = new Uint8Array(canvas.width * canvas.height);

					var alphaChan = 0;
					for (var pix = 3; pix < canvas.width * canvas.height * 4; pix += 4) {
						alpha[alphaChan++] = bitmap.data[pix];
					}

					retImage = new $WebBaseCore.Fusee.Base.Core.ImageData(alpha, bmpWidth, bmpRows, new $fuseeBaseCommon.Fusee.Base.Common.ImagePixelFormat($fuseeBaseCommon.Fusee.Base.Common.ColorFormat.Intensity));
					bmpLeft = glyph.xMin * fontScale;
					bmpTop = glyph.yMax * fontScale;
				}
				bitmapLeft.set(bmpLeft);
				bitmapTop.set(bmpTop);
				return retImage;
			}
		);

		//public float GetKerning(uint leftC, uint rightC)
		$.Method({ Static: false, Public: true }, "GetKerning",
			new JSIL.MethodSignature($.Single, [$.UInt32, $.UInt32]),
			function GetKerning(leftC, rightC) {
				if (!this.UseKerning)
					return 0;

				var fontScale = 1 / this._face.unitsPerEm * this.PixelHeight;
				var leftGlyph = this._face.charToGlyph(String.fromCharCode(leftC));
				var rightGlyph = this._face.charToGlyph(String.fromCharCode(rightC));

				var ret = this._face.getKerningValue(leftGlyph, rightGlyph) * fontScale;
				return ret;
			}
		);

		//public float GetUnscaledKerning(uint leftC, uint rightC)
		$.Method({ Static: false, Public: true }, "GetUnscaledKerning",
			new JSIL.MethodSignature($.Single, [$.UInt32, $.UInt32]),
			function GetUnscaledKerning(leftC, rightC) {

				var leftGlyph = this._face.charToGlyph(String.fromCharCode(leftC));
				var rightGlyph = this._face.charToGlyph(String.fromCharCode(rightC));

				var kerning = this._face.getKerningValue(leftGlyph, rightGlyph);
				return kerning;
			}
		);

		return function (newThisType) { $thisType = newThisType; };
	});
