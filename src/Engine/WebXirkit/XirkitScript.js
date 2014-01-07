/*
This file contains the js implementation of Xirkit of methods that cant be translatet from the JSIL Cross Compiler.
This file is written manually. 
*/


var $WebXirkitImp = JSIL.DeclareAssembly("Fusee.Engine.WebXirkit");
var $fuseeXirkit = JSIL.GetAssembly("Fusee.Xirkit");
var $customMsCore = JSIL.GetAssembly("mscorlib");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Xirkit");

//Strange things i try to make it run///////////////////////////////////////

var $fOutPin$b1 = function () {
    return ($fOutPin$b1 = JSIL.Memoize($WebXirkitImp.Fusee.Xirkit.OutPin$b1))();
};

var $fType = function () {
    return ($fType = JSIL.Memoize($customMsCore.System.Type))();
};
var $T01 = function () {
    return ($T01 = JSIL.Memoize($customMsCore.System.Collections.Generic.Dictionary$b2.Of($customMsCore.System.Type, $customMsCore.System.Delegate)))();
};
var $T02 = function () {
    return ($T02 = JSIL.Memoize($customMsCore.System.Collections.Generic.Dictionary$b2.Of($customMsCore.System.Type, $customMsCore.System.Collections.Generic.Dictionary$b2.Of($customMsCore.System.Type, $customMsCore.System.Delegate))))();
};
var $fDelegate = function () {
    return ($fDelegate = JSIL.Memoize($customMsCore.System.Delegate))();
};
var $fBoolean = function () {
    return ($fBoolean = JSIL.Memoize($customMsCore.System.Boolean))();
};
var $Node = function () {
    return ($Node = JSIL.Memoize($asm06.Fusee.Xirkit.Node))();
};
var $fString = function () {
    return ($fString = JSIL.Memoize($customMsCore.System.String))();
};
var $fObjekt = function () {
    return ($fObjekt = JSIL.Memoize($customMsCore.System.Object))();
};
var $fException = function () {
    return ($fException = JSIL.Memoize($customMsCore.System.Exception))();
};
var $fVoid = function () {
    return ($fVoid = JSIL.Memoize($customMsCore.System.Void))();
};
var $fMemberInfo = function () {
    return ($fMemberInfo = JSIL.Memoize($customMsCore.System.Reflection.MemberInfo))();
};

////////////////////////////////////////////////////////////////////////////


//Interfaces
JSIL.MakeInterface(
  "Fusee.Xirkit.IJsMemberAccessor", true, [], function ($) {
      $.Method({}, "Set", new JSIL.MethodSignature(null, [$.Object, $.Object]));
      $.Method({}, "Get", new JSIL.MethodSignature($.Object, [$.Object], []));
  }, []);


JSIL.MakeInterface(
  "Fusee.Xirkit.JSIInPin", true, [], function ($) {
      $.Method({}, "get_Member", new JSIL.MethodSignature($.String, [], []));
      $.Method({}, "add_ReceivedValue", new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []));
      $.Method({}, "remove_ReceivedValue", new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []));
      $.Method({}, "GetPinType", new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []));
      $.Property({}, "Member");
  }, []);

// FieldAccesssor Implementation


JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Xirkit.JsFieldAccesssor", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [/*$customMsCore*/$customMsCore.TypeRef("System.Reflection.FieldInfo")], []),
        function ctor(fieldInfo) {
            this._fieldInfo = fieldInfo;
        }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Get",
        new JSIL.MethodSignature($.Object, [$.Object], []),
        function Get(o) {
            return o[ this._fieldInfo.Name];
        }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Set",
        new JSIL.MethodSignature(null, [$.Object, ], []),
        function Set(o, val) {
            o[this._fieldInfo.Name] = val;
        }
    );

    $.Field({ Static: false, Public: false, ReadOnly: true }, "_fieldInfo", $customMSCore.TypeRef("System.Reflection.FieldInfo"));
    $.ImplementInterfaces(
          /* 0 */ $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor")
    );

    return function (newThisType) { $thisType = newThisType; };
});

// implementation of public PinFactory Methods

JSIL.ImplementExternals("Fusee.Xirkit.PinFactory", function ($) {

    $.Method({ Static: true, Public: true }, "CreateOutPin",
       new JSIL.MethodSignature($fuseeXirkit.TypeRef("Fusee.Xirkit.IOutPin"), [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String], []),
         function PinFactory_CreateOutPin(n, member) {
             // typeAndAccessor contains two entries:
             //  typeAndAccessor.elementType;
             //  typeAndAccessor.elementAccessor;
             var typeAndAccessor = PinFactory_GetMemberTypeAndAccessor(n, member, null);
             var outPin = new $WebXirkitImp.Fusee.Xirkit.JsOutPin(n, member, typeAndAccessor.elementType, typeAndAccessor.elementAccessor);
             return outPin;
         }
     );

    // Pin Factory Create InPin
    

    $.Method({ Static: true, Public: true }, "CreateInPin",
          new JSIL.MethodSignature($asm06.TypeRef("Fusee.Xirkit.IInPin"), [
              $fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String,
              $customMsCore.TypeRef("System.Type")
          ], []),
              function PinFactory_CreateInPin(n, member, targetType) {
                  // typeAndAccessor contains two entries:
                  //  typeAndAccessor.elementType;
                  //  typeAndAccessor.elementAccessor;
                  var typeAndAccessor = PinFactory_GetMemberTypeAndAccessor(n, member, null);

                  if (typeAndAccessor.elementType != targetType) // TODO: && !CanConvert(targetType, memberType))
                      throw new Exception("No suitable converter to create converting InPin from " + targetType.Name + " to " + typeAndAccessor.elementType.Name);

                  var inPin = new $WebXirkitImp.Fusee.Xirkit.JsInPin(n, member, typeAndAccessor.elementAccessor);

                  return inPin;
              }
    );

    function PinFactory_GetMemberTypeAndAccessor(n, member, pinType) {
        var t = JSIL.GetType(n.get_O());
        var elementAccessor = null;
        var result = null;
        if ((member.indexOf(".") != -1)) {
            $fOutPin$b1
            /*
                var memberName = (JSIL.SplitString(member, JSIL.Array.New($fChar(), ["."])));
                var miList = JSIL.Array.New($fMemberInfo(), memberName.length);
                var currentType = JSIL.GetType(n.get_O());
                for (var i = 0; i < memberName.length; i = ((i + 1) | 0)) {
                    var miFound = currentType.GetMember(memberName[i]);
                    if (!((miFound.length !== 0) && (($fFieldInfo().$As(miFound[0]) !== null) ||
                    $fPropertyInfo().$Is(miFound[0])))) {
                        throw $S02().Construct(("Neither a field nor a property named " + memberName[i] + " exists along the member chain " + member));
                    }
                    if (miFound.length > 1) {
                        throw $S02().Construct(("More than one member named " + memberName[i] + " exists along the member chain " + member));
                    }
                    miList[i] = miFound[0];
                    currentType = (($fFieldInfo().$As(miList[i]) !== null) ? $fFieldInfo().$Cast(miList[i]).get_FieldType() : $fPropertyInfo().$Cast(miList[i]).get_PropertyType());
                }
                var memberType = currentType;
                if ($S04().CallStatic($fType(), "op_Equality", null, pinType, null)) {
                    pinType = memberType;
                }
                elementAccessor.set($thisType.InstantiateChainedMemberAccessor(miList, pinType, memberType));
                var result = memberType;
                */
        } else {
            // Simple member access (no chain)
            var propertyInfo = t.GetProperty(member);
            if (propertyInfo === null) {
                // It's a field
                var fieldInfo = t.GetField(member);
                if (fieldInfo === null) {
                    throw new Exception("Neither a field nor a property named " + member + " exists");
                }
                memberType = fieldInfo.get_FieldType();

                if (pinType === null || memberType === pinType) {
                    // No conversion
                    elementAccessor = PinFactory_InstantiateFieldAccessor(fieldInfo, memberType);
                } else {
                    // Conversion
                    elementAccessor = PinFactory_InstantiateConvertingFieldAccessor(fieldInfo, pinType, memberType);
                }
            } else {
                // It's a property
                /*
                    if (!propertyInfo.get_CanRead()) {
                        throw $S02().Construct(("A property named " + member + " exists but we cannot read from it"));
                    }
                    memberType = propertyInfo.get_PropertyType();
                    if (!(!$S04().CallStatic($fType(), "op_Equality", null, pinType, null) && !$S04().CallStatic($fType(), "op_Equality", null, memberType, pinType))) {
                        elementAccessor.set($thisType.InstantiatePropertyAccessor(propertyInfo, memberType));
                    } else {
                        elementAccessor.set($thisType.InstantiateConvertingPropertyAccessor(propertyInfo, pinType, memberType));
                    }
                    */
            }
            result = {
                "elementType": memberType,
                "elementAccessor": elementAccessor
            };
        }
        return result;
    }

    function PinFactory_InstantiateFieldAccessor(fieldInfo, memberType) {
        var ret = new Fusee.Xirkit.JsFieldAccesssor(fieldInfo);
        return ret;
    }

    function PinFactory_InstantiateConvertingFieldAccessor(fieldInfo, pinType, memberType) {
    }

    return function (newThisType) { $thisType = newThisType; };
});

//$T02 = $JSPin
var $JSPin = function () {
    return ($JSPin = JSIL.Memoize($fuseeXirkit.Fusee.Xirkit.Pin))();
};


// Implemantation of OutPin

JSIL.MakeClass($fuseeXirkit.TypeRef("Fusee.Xirkit.Pin"), "Fusee.Xirkit.JsOutPin", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;

    $.Method({ Static: false, Public: true }, ".ctor",
           new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String, $customMsCore.TypeRef("System.Type"), $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor")], []),

           function ctor(n, member, pinType, memberAccessor)
           {
                var $InPinList = new JSIL.ConstructorSignature($customMsCore.TypeRef("System.Collections.Generic.List`1", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsOutPin")]), []);
                $JSPin().prototype._ctor.call(this, n, member);
                this._links = $InPinList.Construct();
                this._pinType = pinType;
                this._memberAccessor = memberAccessor;
           }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Attach",
      new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin")], []),
       function Attach(other) {
           this._links.Add(other); // cast other to IInPin?
       }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Detach",
      new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin")], []),
       function Detach(other) {
           this._links.Remove(other); // cast other to IInPin?
       }
    );

    $.Method({ Static: false, Public: true }, "get_MemberAccessor",
      new JSIL.MethodSignature($WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"), [], []),
      function get_MemberAccessor() {
          return this._memberAccessor;
      }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "GetPinType",
      new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []),
       function GetPinType() {
           return this._pinType;
       }
    );

    $.Method({ Static: false, Public: true }, "GetValue",
      new JSIL.MethodSignature($fuseeXirkit.TypeRef("Fusee.Xirkit.JsOutPin"), [], []),
      function GetValue() {
          return this._memberAccessor.Get(this.get_N().get_O());
          //var $im00 = this._memberAccessor.Get;
          //return $im00.Call(this._memberAccessor, null, this.get_N().get_O());
      }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Propagate",
      new JSIL.MethodSignature(null, [], []),
      function Propagate() {
          var $temp00;

          for (var a$0 = this._links._items, i$0 = 0, l$0 = this._links._size; i$0 < l$0; ($temp00 = i$0,
              i$0 = ((i$0 + 1) | 0),
              $temp00)) {
              var inPin = a$0[i$0];
              inPin.SetValue(this.GetValue());
              // $fuseeXirkit.Fusee.Xirkit.InPin$b1.Of($thisType.T.get(this)).prototype.SetValue.call(inPin, JSIL.CloneParameter($thisType.T.get(this), $thisType.Of($thisType.T.get(this)).prototype.GetValue.call(this)));
          }
      }
    );

    $.Method({ Static: false, Public: true }, "set_MemberAccessor",
      new JSIL.MethodSignature(null, [$WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$fuseeXirkit.TypeRef("Fusee.Xirkit.JsOutPin")])], []),
      function set_MemberAccessor(value) {
          this._memberAccessor = value;
      }
    );

    // $.Field({ Static: false, Public: false }, "_links", $customMsCore.TypeRef("System.Collections.Generic.List`1", [$fuseeXirkit.TypeRef("Fusee.Xirkit.JsInPin")]));
    $.Field({ Static: false, Public: false }, "_memberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    $.Property({ Static: false, Public: true }, "MemberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    $.Field({ Static: false, Public: false }, "_pinType", $customMsCore.TypeRef("System.Type"));

    $.ImplementInterfaces(
      /* 0 */ $fuseeXirkit.TypeRef("Fusee.Xirkit.IOutPin")
    );

    return function (newThisType) { $thisType = newThisType; };
});






//InPin implementation

JSIL.MakeClass($fuseeXirkit.TypeRef("Fusee.Xirkit.Pin"), "Fusee.Xirkit.JsInPin", true, [], function ($interfaceBuilder) {
    $ = $interfaceBuilder;

    var $T04 = function () {
        return ($T04 = JSIL.Memoize($customMsCore.System.Delegate))();
    };
    var $T05 = function () {
        return ($T05 = JSIL.Memoize($customMsCore.System.Threading.Interlocked))();
    };
    var $T03 = function () {
        return ($T03 = JSIL.Memoize($fuseeXirkit.Fusee.Xirkit.ReceivedValueHandler))();
    };

  //  not used ?!?!?  
  //  var $T01 = function () {
  //      return ($T01 = JSIL.Memoize($asm07.System.String))();
  //  };


    $.Method({ Static: false, Public: true }, ".ctor",
               new JSIL.MethodSignature(null,
                   [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"),
                       $.String,
                       $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor")],
                    []),

               function ctor(n, member, memberAccessor) {
               $JSPin().prototype._ctor.call(this, n, member);
               this._memberAccessor = memberAccessor;
          }
    );

    // wont run !!!!!!
    $.Method({ Static: false, Public: true, Virtual: true }, "add_ReceivedValue",
        new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []),
            function add_ReceivedValue(value) {
                var receivedValueHandler = this.ReceivedValue;

                do {
                    var receivedValueHandler2 = receivedValueHandler;
                    var value2 = $T04().Combine(receivedValueHandler2, value);
                    receivedValueHandler = $T05().CompareExchange$b1($T03())(/* ref */new JSIL.MemberReference(this, "ReceivedValue"), value2, receivedValueHandler2);
                } while (receivedValueHandler !== receivedValueHandler2);
            }
    );

    $.Method({ Static: false, Public: true }, "get_MemberAccessor",
        new JSIL.MethodSignature($WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")]), [], []),
            function get_MemberAccessor() {
            return this._memberAccessor;
            }
    );



    $.Method({ Static: false, Public: true, Virtual: true }, "GetPinType",
        new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []), 
            function GetPinType () {
                return $thisType.T.get(this);
            }
    );


    $.Method({ Static: false, Public: true, Virtual: true }, "remove_ReceivedValue",
        new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []),
        function remove_ReceivedValue(value) {
            var receivedValueHandler = this.ReceivedValue;

            do {
                    var receivedValueHandler2 = receivedValueHandler;
                    var value2 = $T04().Remove(receivedValueHandler2, value);
                    receivedValueHandler = $T05().CompareExchange$b1($T03())(/* ref */ new JSIL.MemberReference(this, "ReceivedValue"), value2, receivedValueHandler2);
                } while (receivedValueHandler !== receivedValueHandler2);
            }
    );


    $.Method({ Static: false, Public: true }, "set_MemberAccessor",
        new JSIL.MethodSignature(null, [$WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")])], []),
        
          function set_MemberAccessor(value) {
              this._memberAccessor = value;
          }
    );



    $.Method({ Static: false, Public: true }, "SetValue",
        new JSIL.MethodSignature(null, [ $WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")], []), 
    
        function SetValue (value) {
            this._memberAccessor.Set(this.get_N().get_O(), value);
            //var $im00 = $asm06.Fusee.Xirkit.IMemberAccessor$b1.Of($thisType.T.get(this)).Set;
            //$im00.Call(this._memberAccessor, null, this.get_N().get_O(), value);
            if (this.ReceivedValue !== null) {
                this.ReceivedValue(this, null);
            }
        }
    );

    $.Field({
        Static: false,
        Public: false
    }, "_memberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    

    $.Field({
        Static: false,
        Public: false
    }, "ReceivedValue", $fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler"));
    

    $.Property({
        Static: false,
        Public: true
    }, "MemberAccessor", $fuseeXirkit.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")]));
    


    $.ImplementInterfaces(
    /* 0 */
    $fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin"));

    return function (newThisType) {
        $thisType = newThisType;
    };
});


