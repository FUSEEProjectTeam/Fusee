JSIL
====

This folder contains the FUSEE-Adoption of the [JSIL IL to JavaScript compiler](https://github.org/kg/JSIL "JSIL") written by K. Gadd.



Compiler Subfolder
------------------
Contains the output of the JSIL project's
   bin folder with a self-written defaults.jsilconfig

###defaults.jsilconfig
```JSON
	{
	  "OutputDirectory": "%currentdirectory%",
	  "ProxyWarnings": false,
	  "Assemblies": {
	    "Stubbed": [
	      "^mscorlib,",
	      "^System,",
	      "^System\\.(.+),",
	      "^Microsoft\\.(.+),",
	      "FSharp.Core,"
	    ],
	    "Ignored": [
	      "Microsoft\\.VisualC,",
	      "Accessibility,",
	      "SMDiagnostics,",
	      "System\\.EnterpriseServices,",
	      "System\\.Security,",
	      "System\\.Runtime\\.Serialization\\.Formatters\\.Soap,",
	      "System\\.Runtime\\.DurableInstancing,",
	      "System\\.Data\\.SqlXml,",
	      "JSIL\\.Meta,",
	 	  "^mscorlib, Version=2"
	   ]
	  },
	  "ProfileSettings": {
	    "OverwriteExistingContent": true,
	    "ContentOutputDirectory": "%outputdirectory%/Content"
	  }
	}
```

Scripts Subfolder
-----------------
Contains the contents of the JSIL project's Libraries folder. The following folders/files are removed:

  - StubbedBCL\
  - TranslatedBCL\
  - XNA\
  - webgl-2d.js


In addition the following changes to individual files need to be done:

###JSIL.js (254)
-> Add loading opentype.js
```JavaScript
  // fusee custom
  environment.loadScript(libraryRoot + "opentype.js");  
```

###JSIL.Browser.Audio.js (366)  
-> replace method finishLoadingSound:
```JavaScript
  function finishLoadingSound (filename, createInstance) {
    $jsilbrowserstate.allAssetNames.push(filename);
    allAssets[getAssetName(filename)] = createInstance(false);
    //var asset = new CallbackSoundAsset(getAssetName(filename, true), createInstance);
    //allAssets[getAssetName(filename)] = asset;
  };
```

###JSIL.Core (5095)
```JavaScript
  castFunction = function Cast(expression) {
    if (isFunction(expression) || JSIL.IsTypedArray(expression))
      return expression;
    else if (expression === null)
      return null;
    else
      throwCastError(expression);
  };
```

###JSIL.Core (9399)
```JavaScript
	JSIL.Array.Clone = function(array) {
	  if (JSIL.IsTypedArray(array)) {
	    var ctor = Object.getPrototypeOf(array).constructor;
	    return new ctor(array);
	  }
	  var type = JSIL.GetType(array);
	  if (type.__IsArray__) {
	    if (!JSIL.IsArray(array)) {
	      var bounds = [];
	      for (var i = 0; i < array.LowerBounds.length; i++) {
	        bounds.push(array.LowerBounds[i]);
	        bounds.push(array.DimensionLength[i]);
	      }
	      return JSIL.MultidimensionalArray.New(array.__ElementType__, bounds, array.Items);
	    } else {
	      return JSIL.Array.New(array.__ElementType__, array);
	    }
	  } else if (JSIL.IsArray(array)) {
	    return Array.prototype.slice.call(array);
	  } else {
	    JSIL.RuntimeError("Invalid array");
	  }
	};
```


###JSIL.Browser.Loaders.js (397 _AND_ 507)
-> replace finisher function:
```JavaScript
    var finisher = function () {
      $jsilbrowserstate.allAssetNames.push(filename);
      allAssets[getAssetName(filename)] = {"image": e};
    };
```

###IgnoredBCL/JSIL.IO.js (107)
-> Added getter for MemoryStream.CanWrite property:
```JavaScript
  $.Method({ Static: false, Public: true }, "get_CanWrite",
    (new JSIL.MethodSignature($.Boolean, [], [])),
    function get_CanWrite() {
        return true;
    }
  );
```
