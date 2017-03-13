var $fuseeBaseCommon = JSIL.GetAssembly("Fusee.Base.Common");
var $fuseeMathCore = JSIL.GetAssembly("Fusee.Math.Core");
var $fuseeBaseImp = JSIL.GetAssembly("Fusee.Base.Imp.Web");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Base");
JSIL.DeclareNamespace("Fusee.Base.Imp");
JSIL.DeclareNamespace("Fusee.Base.Imp.Web");


JSIL.ImplementExternals("Fusee.Base.Imp.Web.WebAssetProvider", function ($) {
    //  public static ImageData LoadImage(object assetOb)

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

    // public static ImageData LoadImage(object assetOb)
    $.Method({ Static: true, Public: true }, "WrapImage",
        new JSIL.MethodSignature($fuseeBaseCommon.TypeRef("Fusee.Base.Common.ImageData"), [$.Object]),
        function WrapImage(assetOb) {
            var image = assetOb.image;

            // Create and initialize return object (FUSEE ImageData)
            var imageData = new $fuseeBaseCommon.Fusee.Base.Common.ImageData();
            imageData.Width = image.width;
            imageData.Height = image.height;
            imageData.PixelFormat = $fuseeBaseCommon.Fusee.Base.Common.ImagePixelFormat.RGBA;
            imageData.Stride = image.width * 4; //TODO: Adjust pixel-size

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
            imageData.PixelData = myData.data;

            return imageData;
        }
    );

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
    function($) {

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

                if (glyph.points != null) {
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

                            var helper = $fuseeBaseImp.Fusee.Base.Imp.Web.SplitToCurveSegmentHelper;
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
      new JSIL.MethodSignature($fuseeBaseCommon.TypeRef("Fusee.Base.Common.ImageData"), [
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

              var retImage = new $fuseeBaseCommon.Fusee.Base.Common.ImageData();
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
                  retImage.Width = bmpWidth;
                  retImage.Height = bmpRows;
                  retImage.PixelFormat = $fuseeBaseCommon.Fusee.Base.Common.ImagePixelFormat.Intensity;
                  retImage.Stride = bmpWidth;
                  retImage.PixelData = alpha;

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


    /* Automatic properties anyway (not [JSExternal])
    // public bool UseKerning { get; set; }
    $.Method({ Static: false, Public: true }, "get_UseKerning",
        new JSIL.MethodSignature($.Boolean, []),
        function get_UseKerning() {
            return this.FontImp$UseKerning$value;
        }
    );
    $.Method({ Static: false, Public: true }, "set_UseKerning",
        new JSIL.MethodSignature(null, [$.Boolean]),
        function set_UseKerning(val) {
            this.FontImp$UseKerning$value = val;
        }
    );

    //public uint PixelHeight { get; set; }
    $.Method({ Static: false, Public: true }, "get_PixelHeight",
        new JSIL.MethodSignature($.UInt32, []),
        function get_PixelHeight() {
            
        }
    );
    $.Method({ Static: false, Public: true }, "set_PixelHeight",
        new JSIL.MethodSignature(null, [$.UInt32]),
        function set_PixelHeight(val) {

        }
    );
    */

    return function (newThisType) { $thisType = newThisType; };
});
