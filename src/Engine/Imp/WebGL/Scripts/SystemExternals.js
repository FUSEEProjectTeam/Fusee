/*
JSIL.ImplementExternals("System.Double", function($) {
    $.ExternalMethod({ Static: true, Public: true }, "Parse",
        new JSIL.MethodSignature($.Type, [$.String, $asm04.TypeRef("System.IFormatProvider")]),
        function Parse(str, fmtProv) {
            return Number(str);
        }
    );
});


JSIL.ImplementExternals("System.Globalization.CultureInfo", function($) {
    $.Method({ Static: true, Public: true }, "get_InvariantCulture",
        new JSIL.MethodSignature($.Type, []),
        function get_InvariantCulture() {
            return null;
        }
    );
});
*/



JSIL.ImplementExternals("System.IO.StreamReader", function ($) {
    $.Field({ Static: false, Public: false }, "_contents", $.Array, null);
    $.Field({ Static: false, Public: false }, "_currentLine", $.Int32, null);

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [$.String]),
            function _ctor(fileName) {
                var httpRequest = new XMLHttpRequest();
                httpRequest.open("GET", fileName, false);
                httpRequest.send(null);
                this._contents = httpRequest.responseText.split("\n");
                this._currentLine = 0;
            }
    );

    $.Method({ Static: false, Public: true }, "ReadLine",
        new JSIL.MethodSignature($.String, []),
            function ReadLine() {
                if (this._currentLine < this._contents.length) {
                    return this._contents[this._currentLine++];
                } else {
                    return null;
                }
            }
  );

});


function GetElemName(element) {
    var en = parseInt(element.value);
    var elemName;
    if (isNaN(en)) {
        elemName = "E_" + element.toString();
    }
    else {
        elemName = "E_" + en.toString();
    }
    return elemName;    
}

JSIL.ImplementExternals("System.Collections.Generic.HashSet`1", function ($) {

    $.Field({ Static: false, Public: false }, "_hashTable", $.Object, null);


    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function HashSet_ctor() {
        this._hashTable = [];
    }
  );

    $.Method({ Static: false, Public: true }, "Add",
    new JSIL.MethodSignature($.Boolean, [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]),
        function HashSet_Add(element) {
            this._hashTable[GetElemName(element)] = true;
        }
  );

    $.Method({ Static: false, Public: true }, "Remove",
    new JSIL.MethodSignature($.Boolean, [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]),
        function HashSet_Remove(element) {
            this._hashTable[GetElemName(element)] = false;
        }
  );

    // This entire class, but especially this method is one big entire piece of shit.
    // The combination of how C# compiles literal [Flags] enum values in combination with JSIL handling those literals
    // as strings led to this code entropy. TODO: work it over.
    $.Method({ Static: false, Public: true }, "Contains",
    new JSIL.MethodSignature($.Boolean, [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]),
        function HashSet_Contains(element) {
            var elemName = GetElemName(element);
            var hasProp = this._hashTable.hasOwnProperty(elemName);
            if (hasProp) {
                var isPresent = this._hashTable[elemName];
                // var isPresentByName = this._hashTable[element.toString()];
                return isPresent;
            }
            return false;
            // return this._hashTable.hasOwnProperty(element.toString()) && this._hashTable[element];
        }
  );

});
   
    /*
    JSIL.MakeClass($asm04.TypeRef("System.Object"), "System.Collections.Generic.HashSet`1", true, ["T"], function ($) {

    $.ExternalMethod({Static:false, Public:true }, ".ctor", 
    $sig.get(32855, null, [], [])
    );



    $.ExternalMethod({ Static: false, Public: true }, ".ctor",
    $sig.get(32857, null, [$asm04.TypeRef("System.Collections.Generic.IEqualityComparer`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, ".ctor",
    $sig.get(32859, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, ".ctor",
    $sig.get(32862, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), $asm04.TypeRef("System.Collections.Generic.IEqualityComparer`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "Clear",
    $sig.get(32879, null, [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "CopyTo",
    $sig.get(32885, null, [$jsilcore.TypeRef("System.Array", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), $.Int32], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "CopyTo",
    $sig.get(32887, null, [$jsilcore.TypeRef("System.Array", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "CopyTo",
    $sig.get(32890, null, [
        $jsilcore.TypeRef("System.Array", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), $.Int32,
        $.Int32
      ], [])
  );

    $.ExternalMethod({ Static: true, Public: true }, "CreateSetComparer",
    $sig.get(32893, $asm04.TypeRef("System.Collections.Generic.IEqualityComparer`1", [$.Type]), [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "ExceptWith",
    $sig.get(32895, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "get_Comparer",
    $sig.get(32898, $asm04.TypeRef("System.Collections.Generic.IEqualityComparer`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "get_Count",
    $sig.get(32903, $.Int32, [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "GetEnumerator",
    $sig.get(32904, $asm07.TypeRef("System.Collections.Generic.HashSet`1/Enumerator", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "GetObjectData",
    $sig.get(32907, null, [$asm04.TypeRef("System.Runtime.Serialization.SerializationInfo"), $asm04.TypeRef("System.Runtime.Serialization.StreamingContext")], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "IntersectWith",
    $sig.get(32920, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "IsProperSubsetOf",
    $sig.get(32923, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "IsProperSupersetOf",
    $sig.get(32924, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "IsSubsetOf",
    $sig.get(32925, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "IsSubsetOfHashSetWithSameEC",
    $sig.get(32926, $.Boolean, [$.Type], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "IsSupersetOf",
    $sig.get(32927, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "OnDeserialization",
    $sig.get(32928, null, [$.Object], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "Overlaps",
    $sig.get(32929, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "RemoveWhere",
    $sig.get(32931, $.Int32, [$asm04.TypeRef("System.Predicate`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "SetEquals",
    $sig.get(32932, $.Boolean, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "SymmetricExceptWith",
    $sig.get(32933, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "SymmetricExceptWithEnumerable",
    $sig.get(32934, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "SymmetricExceptWithUniqueHashSet",
    $sig.get(32935, null, [$.Type], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "ICollection`1.Add",
    $sig.get(32936, null, [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "ICollection`1.get_IsReadOnly",
    $sig.get(32937, $.Boolean, [], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "IEnumerable`1.GetEnumerator",
    $sig.get(32938, $asm04.TypeRef("System.Collections.Generic.IEnumerator`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), [], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "IEnumerable.GetEnumerator",
    $sig.get(32939, $asm04.TypeRef("System.Collections.IEnumerator"), [], [])
  );

    $.ExternalMethod({ Static: false, Public: false }, "ToArray",
    $sig.get(32940, $jsilcore.TypeRef("System.Array", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "TrimExcess",
    $sig.get(32941, null, [], [])
  );

    $.ExternalMethod({ Static: false, Public: true }, "UnionWith",
    $sig.get(32943, null, [$asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")])], [])
  );


    $.ImplementInterfaces(
    $asm04.TypeRef("System.Runtime.Serialization.ISerializable"), $asm04.TypeRef("System.Runtime.Serialization.IDeserializationCallback"), 
    $asm05.TypeRef("System.Collections.Generic.ISet`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), $asm04.TypeRef("System.Collections.Generic.ICollection`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), 
    $asm04.TypeRef("System.Collections.Generic.IEnumerable`1", [new JSIL.GenericParameter("T", "System.Collections.Generic.HashSet`1")]), $asm04.TypeRef("System.Collections.IEnumerable"), 
    $asm04.TypeRef("System.Collections.IEnumerable")
    )


    */


