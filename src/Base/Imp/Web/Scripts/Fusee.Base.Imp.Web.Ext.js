var $fuseeBaseCommon = JSIL.GetAssembly("Fusee.Base.Common");

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

    return function (newThisType) { $thisType = newThisType; };
});




JSIL.ImplementExternals("Fusee.Base.Imp.Web.FontImp", function ($) {

    var _face;

    //public FontImp(object storage)
    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [$.Object]),
        function _ctor(storage) {
            this._face = opentype.parse(storage.buffer);
        }
    );

    //public GlyphInfo GetGlyphInfo(uint c)
    $.Method({ Static: false, Public: true }, "GetGlyphInfo",
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
              if (bmpWidth > 0 && bmpRows > 0) 
              {
                  var canvas = document.createElement("canvas");
                  canvas.width = bmpWidth;
                  canvas.height = bmpRows;
                  var ctx = canvas.getContext("2d");

                  glyph.draw(ctx, -xMin, yMax, fontSize);

                  var bitmap = ctx.getImageData(0, 0, canvas.width, canvas.height);
                  var alpha = new Uint8Array(canvas.width * canvas.height);

                  var alphaChan = 0;
                  for (var pix = 3; pix < canvas.width * canvas.height * 4; pix += 4)
                  {
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
