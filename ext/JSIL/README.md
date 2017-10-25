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

###JSIL.Core (5133)
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
-> add getter for MemoryStream.CanWrite property:
```JavaScript
  $.Method({ Static: false, Public: true }, "get_CanWrite",
    (new JSIL.MethodSignature($.Boolean, [], [])),
    function get_CanWrite() {
        return true;
    }
  );
```


###Scripts/JSIL.Bootstrap.Linq.js (436)
--> add the following functions (OrderBy, SequenceEqual, Take):
```JavaScript
$.Method({ Static: true, Public: true }, "OrderBy",
  new JSIL.MethodSignature($jsilcore.TypeRef("System.Linq.IOrderedEnumerable`1", ["!!0"]),
  [
    $jsilcore.TypeRef("System.Collections.Generic.IEnumerable`1", ["!!0"]),
    $jsilcore.TypeRef("System.Func`2", ["!!0", "!!1"])
  ],
  ["TSource", "TKey"]),

	function(TSource, TKey, source, keySelector){

    var srcEumerator = JSIL.GetEnumerator(source, TSource);
    var moveNext = $jsilcore.System.Collections.IEnumerator.MoveNext;
    var getCurrent = $jsilcore.System.Collections.IEnumerator.get_Current;

    var res =  JSIL.EnumerableToArray(source, TSource);
    var i = -1;
    var swapped = true;

    //Bubble sort
    while(swapped){

      swapped = false;

      while(moveNext.Call(srcEumerator)){

        i++;
        var current = getCurrent.Call(srcEumerator);
        var predicatVal = keySelector(current);

        if(i+1 <= res.length-1){
        if(predicatVal > keySelector(res[i+1])){
            var temp = res[i];
            res[i] = res[i+1];
            res[i+1] = temp;
            swapped = true;
        }
      }
    }
    if(swapped){
      //Reset Enumerator
      srcEumerator = JSIL.GetEnumerator(res, TSource)
  		i = -1;
    }
  }
  	return res;
  }
);

$.Method({ Static: true, Public: true }, "SequenceEqual",
	new JSIL.MethodSignature($.Boolean,
  [
    $jsilcore.TypeRef("System.Collections.Generic.IEnumerable`1",
    ["!!0"]),
    $jsilcore.TypeRef("System.Collections.Generic.IEnumerable`1",
    ["!!0"])
  ],
	["TSource"]),
	
  function (TSource, sourceOne, sourceTwo) {

    var enumeratorOne = JSIL.GetEnumerator(sourceOne, TSource);
    var enumeratorTwo = JSIL.GetEnumerator(sourceTwo, TSource);
    var moveNext = $jsilcore.System.Collections.IEnumerator.MoveNext;
    var getCurrent = $jsilcore.System.Collections.IEnumerator.get_Current;

    var countOne = 0;
    var countTwo = 0;

      try{
        while(moveNext.Call(enumeratorOne)){
          countOne ++;
        }
        	enumeratorOne = JSIL.GetEnumerator(sourceOne, TSource);

        while(moveNext.Call(enumeratorTwo)){
          countTwo ++;
        }
        enumeratorTwo = JSIL.GetEnumerator(sourceTwo, TSource);

        if (countOne != countTwo) return false;

        while (moveNext.Call(enumeratorOne) && moveNext.Call(enumeratorTwo))
        {
          if (getCurrent.Call(enumeratorOne) !== getCurrent.Call(enumeratorTwo))
            return false;
        }
        	return true;
      }
      finally{
        JSIL.Dispose(enumeratorOne);
        JSIL.Dispose(enumeratorTwo);
      }
    }
  );

  $.Method({ Static: true, Public: true }, "Take",
    new JSIL.MethodSignature(
      $jsilcore.TypeRef("System.Collections.Generic.IEnumerable`1",["!!0"]),
      [$jsilcore.TypeRef("System.Collections.Generic.IEnumerable`1", ["!!0"]), "System.Int32"],
      ["TSource"]
    ),
    function (TSource, source, count) {

      var enumerator = JSIL.GetEnumerator(source, TSource);
      var moveNext = $jsilcore.System.Collections.IEnumerator.MoveNext;
      var getCurrent = $jsilcore.System.Collections.IEnumerator.get_Current;
      var res = [];
      var i = 0;

      try {
        while (moveNext.Call(enumerator)) {
          if (i < count){
            res.push(getCurrent.Call(enumerator));
            i++;
          }
          else
            break;
        }
			} 
			finally {
        JSIL.Dispose(enumerator);
			}
      return res;
    }
	);
``` 
