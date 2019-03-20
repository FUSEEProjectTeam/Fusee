/* Generated by JSIL v0.8.2 build 31050. See http://jsil.org/ for more information. */ 
'use strict';
var $asm25 = JSIL.DeclareAssembly("RobotArm_Inverse_Kinematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

JSIL.DeclareNamespace("FuseeApp");
/* class FuseeApp.Test */ 

(function Test$Members () {
  var $, $thisType;
  var $T00 = function () {
    return ($T00 = JSIL.Memoize($asm04.Fusee.Engine.Core.RenderCanvas)) ();
  };
  var $T01 = function () {
    return ($T01 = JSIL.Memoize($asm03.Fusee.Engine.Common.IInputDeviceImp)) ();
  };
  var $T02 = function () {
    return ($T02 = JSIL.Memoize($asm0F.System.NotImplementedException)) ();
  };
  var $T03 = function () {
    return ($T03 = JSIL.Memoize($asm0F.System.Predicate$b1.Of($asm0B.Fusee.Serialization.SceneNodeContainer))) ();
  };
  var $T04 = function () {
    return ($T04 = JSIL.Memoize($asm04.Fusee.Engine.Core.RenderContext)) ();
  };
  var $T05 = function () {
    return ($T05 = JSIL.Memoize($asm0A.Fusee.Math.Core.float4)) ();
  };
  var $T06 = function () {
    return ($T06 = JSIL.Memoize($asm0B.Fusee.Serialization.SceneContainer)) ();
  };
  var $T07 = function () {
    return ($T07 = JSIL.Memoize($asm01.Fusee.Base.Core.AssetStorage)) ();
  };
  var $T08 = function () {
    return ($T08 = JSIL.Memoize($asm0F.System.Collections.Generic.IEnumerable$b1.Of($asm0B.Fusee.Serialization.SceneNodeContainer))) ();
  };
  var $T09 = function () {
    return ($T09 = JSIL.Memoize($asm25.FuseeApp.Test_$l$gc)) ();
  };
  var $T0A = function () {
    return ($T0A = JSIL.Memoize($asm0D.Fusee.Xene.SceneFinderExtensions)) ();
  };
  var $T0B = function () {
    return ($T0B = JSIL.Memoize($asm0B.Fusee.Serialization.TransformComponent)) ();
  };
  var $T0C = function () {
    return ($T0C = JSIL.Memoize($asm0B.Fusee.Serialization.SceneNodeContainer)) ();
  };
  var $T0D = function () {
    return ($T0D = JSIL.Memoize($asm15.System.Linq.Enumerable)) ();
  };
  var $T0E = function () {
    return ($T0E = JSIL.Memoize($asm0D.Fusee.Xene.ContainerExtensions)) ();
  };
  var $T0F = function () {
    return ($T0F = JSIL.Memoize($asm0A.Fusee.Math.Core.float3)) ();
  };
  var $T10 = function () {
    return ($T10 = JSIL.Memoize($asm04.Fusee.Engine.Core.SceneRenderer)) ();
  };
  var $T11 = function () {
    return ($T11 = JSIL.Memoize($asm03.Fusee.Engine.Common.ClearFlags)) ();
  };
  var $T12 = function () {
    return ($T12 = JSIL.Memoize($asm0F.System.Boolean)) ();
  };
  var $T13 = function () {
    return ($T13 = JSIL.Memoize($asm04.Fusee.Engine.Core.MouseDevice)) ();
  };
  var $T14 = function () {
    return ($T14 = JSIL.Memoize($asm04.Fusee.Engine.Core.Input)) ();
  };
  var $T15 = function () {
    return ($T15 = JSIL.Memoize($asm04.Fusee.Engine.Core.Time)) ();
  };
  var $T16 = function () {
    return ($T16 = JSIL.Memoize($asm0F.System.Single)) ();
  };
  var $T17 = function () {
    return ($T17 = JSIL.Memoize($asm0F.System.Math)) ();
  };
  var $T18 = function () {
    return ($T18 = JSIL.Memoize($asm0A.Fusee.Math.Core.float4x4)) ();
  };
  var $T19 = function () {
    return ($T19 = JSIL.Memoize($asm04.Fusee.Engine.Core.KeyboardDevice)) ();
  };
  var $T1A = function () {
    return ($T1A = JSIL.Memoize($asm0F.System.Double)) ();
  };
  var $T1B = function () {
    return ($T1B = JSIL.Memoize($asm0A.Fusee.Math.Core.M)) ();
  };
  var $T1C = function () {
    return ($T1C = JSIL.Memoize($asm01.Fusee.Base.Core.Diagnostics)) ();
  };
  var $T1D = function () {
    return ($T1D = JSIL.Memoize($asm0F.System.String)) ();
  };
  var $T1E = function () {
    return ($T1E = JSIL.Memoize($asm04.Fusee.Engine.Core.InputDevice)) ();
  };
  var $S00 = function () {
    return ($S00 = JSIL.Memoize(new JSIL.ConstructorSignature($asm0F.System.NotImplementedException, null))) ();
  };
  var $S01 = function () {
    return ($S01 = JSIL.Memoize(new JSIL.ConstructorSignature($asm0A.Fusee.Math.Core.float4, [
        $asm0F.System.Single, $asm0F.System.Single, 
        $asm0F.System.Single, $asm0F.System.Single
      ]))) ();
  };
  var $S02 = function () {
    return ($S02 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S03 = function () {
    return ($S03 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Collections.Generic.IEnumerable$b1.Of($asm0B.Fusee.Serialization.SceneNodeContainer), [$asm0F.System.Collections.Generic.IEnumerable$b1.Of($asm0B.Fusee.Serialization.SceneNodeContainer), $asm0F.System.Predicate$b1.Of($asm0B.Fusee.Serialization.SceneNodeContainer)]))) ();
  };
  var $S04 = function () {
    return ($S04 = JSIL.Memoize(new JSIL.MethodSignature("!!0", [$asm0F.TypeRef("System.Collections.Generic.IEnumerable`1", ["!!0"])], ["TSource"]))) ();
  };
  var $S05 = function () {
    return ($S05 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S06 = function () {
    return ($S06 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S07 = function () {
    return ($S07 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S08 = function () {
    return ($S08 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S09 = function () {
    return ($S09 = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S0A = function () {
    return ($S0A = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S0B = function () {
    return ($S0B = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S0C = function () {
    return ($S0C = JSIL.Memoize(new JSIL.MethodSignature($asm0F.System.Boolean, [$asm0B.Fusee.Serialization.SceneNodeContainer]))) ();
  };
  var $S0D = function () {
    return ($S0D = JSIL.Memoize(new JSIL.ConstructorSignature($asm0A.Fusee.Math.Core.float3, [
        $asm0F.System.Single, $asm0F.System.Single, 
        $asm0F.System.Single
      ]))) ();
  };
  var $S0E = function () {
    return ($S0E = JSIL.Memoize(new JSIL.ConstructorSignature($asm04.Fusee.Engine.Core.SceneRenderer, [$asm0B.Fusee.Serialization.SceneContainer]))) ();
  };
  var $S0F = function () {
    return ($S0F = JSIL.Memoize(new JSIL.MethodSignature($asm0A.Fusee.Math.Core.float4x4, [$asm0A.Fusee.Math.Core.float4x4, $asm0A.Fusee.Math.Core.float4x4]))) ();
  };


  function Test__ctor () {
    $T00().prototype._ctor.call(this);
  }; 

  function Test_Creator (device) {
    throw $S00().Construct();
  }; 

  function Test_Init () {
    var arg_61_1 = null, arg_AF_1 = null, arg_FD_1 = null, arg_14B_1 = null, arg_199_1 = null, arg_1E7_1 = null, arg_235_1 = null, arg_283_1 = null, arg_2D1_1 = null;
    (this.RenderCanvas$RC$value.ClearColor = $S01().Construct(1, 1, 1, 1));
    this._scene = $T07().Get$b1($T06())("roboter_arm.fus");
    var arg_61_0 = this._scene.Children;
    if ((arg_61_1 = $T09().$l$g9__22_0) === null) {
      arg_61_1 = $T09().$l$g9__22_0 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_0", $S02(), false, false));
    }
    var expr_66 = $S03().CallStatic($T0A(), "FindNodes", null, arg_61_0, arg_61_1);
    if (expr_66 === null) {
      var arg_7F_1 = null;
    } else {
      var expr_72 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_66);
      arg_7F_1 = (
        (expr_72 !== null)
           ? $T0E().GetTransform(expr_72, 0)
           : null)
      ;
    }
    this._lowerAxleTransform = arg_7F_1;
    var arg_AF_0 = this._scene.Children;
    if ((arg_AF_1 = $T09().$l$g9__22_1) === null) {
      arg_AF_1 = $T09().$l$g9__22_1 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_1", $S05(), false, false));
    }
    var expr_B4 = $S03().CallStatic($T0A(), "FindNodes", null, arg_AF_0, arg_AF_1);
    if (expr_B4 === null) {
      var arg_CD_1 = null;
    } else {
      var expr_C0 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_B4);
      arg_CD_1 = (
        (expr_C0 !== null)
           ? $T0E().GetTransform(expr_C0, 0)
           : null)
      ;
    }
    this._middleAxleTransform = arg_CD_1;
    var arg_FD_0 = this._scene.Children;
    if ((arg_FD_1 = $T09().$l$g9__22_2) === null) {
      arg_FD_1 = $T09().$l$g9__22_2 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_2", $S06(), false, false));
    }
    var expr_102 = $S03().CallStatic($T0A(), "FindNodes", null, arg_FD_0, arg_FD_1);
    if (expr_102 === null) {
      var arg_11B_1 = null;
    } else {
      var expr_10E = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_102);
      arg_11B_1 = (
        (expr_10E !== null)
           ? $T0E().GetTransform(expr_10E, 0)
           : null)
      ;
    }
    this._upperAxleTransform = arg_11B_1;
    var arg_14B_0 = this._scene.Children;
    if ((arg_14B_1 = $T09().$l$g9__22_3) === null) {
      arg_14B_1 = $T09().$l$g9__22_3 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_3", $S07(), false, false));
    }
    var expr_150 = $S03().CallStatic($T0A(), "FindNodes", null, arg_14B_0, arg_14B_1);
    if (expr_150 === null) {
      var arg_169_1 = null;
    } else {
      var expr_15C = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_150);
      arg_169_1 = (
        (expr_15C !== null)
           ? $T0E().GetTransform(expr_15C, 0)
           : null)
      ;
    }
    this._footTransform = arg_169_1;
    var arg_199_0 = this._scene.Children;
    if ((arg_199_1 = $T09().$l$g9__22_4) === null) {
      arg_199_1 = $T09().$l$g9__22_4 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_4", $S08(), false, false));
    }
    var expr_19E = $S03().CallStatic($T0A(), "FindNodes", null, arg_199_0, arg_199_1);
    if (expr_19E === null) {
      var arg_1B7_1 = null;
    } else {
      var expr_1AA = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_19E);
      arg_1B7_1 = (
        (expr_1AA !== null)
           ? $T0E().GetTransform(expr_1AA, 0)
           : null)
      ;
    }
    this._rightPincerTransform = arg_1B7_1;
    var arg_1E7_0 = this._scene.Children;
    if ((arg_1E7_1 = $T09().$l$g9__22_5) === null) {
      arg_1E7_1 = $T09().$l$g9__22_5 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_5", $S09(), false, false));
    }
    var expr_1EC = $S03().CallStatic($T0A(), "FindNodes", null, arg_1E7_0, arg_1E7_1);
    if (expr_1EC === null) {
      var arg_205_1 = null;
    } else {
      var expr_1F8 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_1EC);
      arg_205_1 = (
        (expr_1F8 !== null)
           ? $T0E().GetTransform(expr_1F8, 0)
           : null)
      ;
    }
    this._leftPincerTransform = arg_205_1;
    var arg_235_0 = this._scene.Children;
    if ((arg_235_1 = $T09().$l$g9__22_6) === null) {
      arg_235_1 = $T09().$l$g9__22_6 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_6", $S0A(), false, false));
    }
    var expr_23A = $S03().CallStatic($T0A(), "FindNodes", null, arg_235_0, arg_235_1);
    if (expr_23A === null) {
      var arg_253_1 = null;
    } else {
      var expr_246 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_23A);
      arg_253_1 = (
        (expr_246 !== null)
           ? $T0E().GetTransform(expr_246, 0)
           : null)
      ;
    }
    this._rightPincerTransformUp = arg_253_1;
    var arg_283_0 = this._scene.Children;
    if ((arg_283_1 = $T09().$l$g9__22_7) === null) {
      arg_283_1 = $T09().$l$g9__22_7 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_7", $S0B(), false, false));
    }
    var expr_288 = $S03().CallStatic($T0A(), "FindNodes", null, arg_283_0, arg_283_1);
    if (expr_288 === null) {
      var arg_2A1_1 = null;
    } else {
      var expr_294 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_288);
      arg_2A1_1 = (
        (expr_294 !== null)
           ? $T0E().GetTransform(expr_294, 0)
           : null)
      ;
    }
    this._leftPincerTransformUp = arg_2A1_1;
    var arg_2D1_0 = this._scene.Children;
    if ((arg_2D1_1 = $T09().$l$g9__22_8) === null) {
      arg_2D1_1 = $T09().$l$g9__22_8 = $T03().New($T09().$l$g9, null, new JSIL.MethodPointerInfo($asm25.FuseeApp.Test_$l$gc, "$lInit$gb__22_8", $S0C(), false, false));
    }
    var expr_2D6 = $S03().CallStatic($T0A(), "FindNodes", null, arg_2D1_0, arg_2D1_1);
    if (expr_2D6 === null) {
      var arg_2EF_1 = null;
    } else {
      var expr_2E2 = $S04().CallStatic($T0D(), "FirstOrDefault$b1", [$asm0B.Fusee.Serialization.SceneNodeContainer], expr_2D6);
      arg_2EF_1 = (
        (expr_2E2 !== null)
           ? $T0E().GetTransform(expr_2E2, 0)
           : null)
      ;
    }
    this._pointer = arg_2EF_1;
    this._virtualPos = $S0D().Construct(0, 5, 0);
    this._open = false;
    this._sceneRenderer = $S0E().Construct(this._scene);
  }; 

  function Test_RenderAFrame () {
    (this.RenderCanvas$RC$value).Clear($T11().$Flags("Color", "Depth"));
    var middleButton = $T14().get_Mouse().get_MiddleButton();
    if (middleButton) {
      $thisType._angleVelHorz = ((-7 * +$T14().get_Mouse().get_XVel()) * +$T15().get_DeltaTime()) * 0.0005;
      $thisType._angleVelVert = ((-7 * +$T14().get_Mouse().get_YVel()) * +$T15().get_DeltaTime()) * 0.0005;
    } else {
      var flag = +$T14().get_Mouse().get_WheelVel() !== 0;
      if (flag) {
        $thisType._distanceVel = +$thisType._distanceVel + (((7 * +$T14().get_Mouse().get_WheelVel()) * +$T15().get_DeltaTime()) * 0.0005);
      } else {
        var num = Math.fround($T17().Exp(-0.8 * +$T15().get_DeltaTime()));
        $thisType._angleVelHorz = +$thisType._angleVelHorz * num;
        $thisType._angleVelVert = +$thisType._angleVelVert * num;
        $thisType._distanceVel = +$thisType._distanceVel * num;
      }
    }
    $thisType._angleHorz = +$thisType._angleHorz + +$thisType._angleVelHorz;
    $thisType._angleVert = +$thisType._angleVert + +$thisType._angleVelVert;
    $thisType._distance = +$thisType._distance + +$thisType._distanceVel;
    var right = $S0F().CallStatic($T18(), "op_Multiply", null, 
      $T18().CreateRotationX($thisType._angleVert).MemberwiseClone(), 
      $T18().CreateRotationY($thisType._angleHorz).MemberwiseClone()
    ).MemberwiseClone();
    var left = $T18().LookAt(
      0, 
      2, 
      -20 + +$thisType._distance, 
      0, 
      1, 
      0, 
      0, 
      1, 
      0
    ).MemberwiseClone();
    (this.RenderCanvas$RC$value.View = $S0F().CallStatic($T18(), "op_Multiply", null, left.MemberwiseClone(), right.MemberwiseClone()).MemberwiseClone());
    this._virtualPos = $T0F().op_Addition(this._virtualPos.MemberwiseClone(), $S0D().Construct((+$T14().get_Keyboard().get_LeftRightAxis() * +$T15().get_DeltaTime()), (+$T14().get_Keyboard().get_WSAxis() * +$T15().get_DeltaTime()), (+$T14().get_Keyboard().get_UpDownAxis() * +$T15().get_DeltaTime()))).MemberwiseClone();
    this._pointer.Translation = this._virtualPos.MemberwiseClone();
    var num4 = Math.fround(Math.acos(+(((Math.pow(Math.sqrt(((Math.pow(Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2)))), 2)) + (Math.pow((this._virtualPos.y - 1), 2)))), 2)) / (4 * (Math.sqrt(((Math.pow(Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2)))), 2)) + (Math.pow((this._virtualPos.y - 1), 2))))))))));
    var num5 = Math.fround(Math.acos(+((((Math.pow(Math.sqrt(((Math.pow(Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2)))), 2)) + (Math.pow((this._virtualPos.y - 1), 2)))), 2)) - 8) / -8))));
    var flag2 = num5 < +$T1B().DegreesToRadians(71);
    if (flag2) {
      num5 = +$T1B().DegreesToRadians(71);
    }
    var num6 = Math.fround(Math.atan2(+this._virtualPos.y - 1, Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2))))));
    var num7 = -Math.fround(Math.atan2(this._virtualPos.z, this._virtualPos.x));
    if ((Math.sqrt(((Math.pow(Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2)))), 2)) + (Math.pow((this._virtualPos.y - 1), 2))))) >= 4) {
      var num8 = -(+$T1B().DegreesToRadians(90) - num6);
      var flag4 = num8 < +$T1B().DegreesToRadians(-90);
      if (flag4) {
        num8 = +$T1B().DegreesToRadians(-90);
      } else {
        var flag5 = num8 > +$T1B().DegreesToRadians(90);
        if (flag5) {
          num8 = +$T1B().DegreesToRadians(90);
        }
      }
      var z = 0;
      var num9 = +$T1B().DegreesToRadians(-90) - num8;
    } else {
      num8 = -((+$T1B().DegreesToRadians(90) - num4) - num6);
      var flag6 = num8 < +$T1B().DegreesToRadians(-90);
      if (flag6) {
        num8 = +$T1B().DegreesToRadians(-90);
      } else {
        var flag7 = num8 > +$T1B().DegreesToRadians(90);
        if (flag7) {
          num8 = +$T1B().DegreesToRadians(90);
        }
      }
      z = -(+$T1B().DegreesToRadians(180) - num5);
      num9 = (+$T1B().DegreesToRadians(90) - num8) - num5;
    }
    this._lowerAxleTransform.Rotation = $S0D().Construct(0, 0, num8);
    this._middleAxleTransform.Rotation = $S0D().Construct(0, 0, z);
    this._upperAxleTransform.Rotation = $S0D().Construct(0, 0, num9);
    this._footTransform.Rotation = $S0D().Construct(0, num7, 0);
    $T1C().Log(JSIL.ConcatString("Coordinates: ", this._virtualPos));
    $T1C().Log(JSIL.ConcatString("Distance: ", $T1A().$Box(Math.sqrt(((Math.pow(Math.sqrt(((Math.pow(this._virtualPos.x, 2)) + (Math.pow(this._virtualPos.z, 2)))), 2)) + (Math.pow((this._virtualPos.y - 1), 2)))))));
    $T1C().Log(JSIL.ConcatString("Alpha: ", $T16().$Box($T1B().RadiansToDegrees(num4))));
    $T1C().Log(JSIL.ConcatString("Beta: ", $T16().$Box($T1B().RadiansToDegrees(num5))));
    $T1C().Log(JSIL.ConcatString("Gamma: ", $T16().$Box($T1B().RadiansToDegrees(num6))));
    $T1C().Log(JSIL.ConcatString("Epsilon: ", $T16().$Box($T1B().RadiansToDegrees(num7))));
    $T1C().Log(JSIL.ConcatString("Delta: ", $T16().$Box($T1B().RadiansToDegrees(num9))));
    var button = $T14().get_Keyboard().GetButton(79);
    if (button) {
      this._move = true;
    }
    if (this._move && this._open) {
      var flag9 = +this._rightPincerTransform.Rotation.x < +$T1B().DegreesToRadians(0);
      if (flag9) {
        this._leftPincerTransform.Rotation = $T0F().op_Subtraction(this._leftPincerTransform.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
        this._rightPincerTransform.Rotation = $T0F().op_Addition(this._rightPincerTransform.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
        this._leftPincerTransformUp.Rotation = $T0F().op_Subtraction(this._leftPincerTransformUp.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
        this._rightPincerTransformUp.Rotation = $T0F().op_Addition(this._rightPincerTransformUp.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
      } else {
        var flag10 = +this._rightPincerTransform.Rotation.x >= +$T1B().DegreesToRadians(0);
        if (flag10) {
          this._move = false;
          this._open = false;
        }
      }
    } else {
      if (this._move && !this._open) {
        var flag12 = +this._rightPincerTransform.Rotation.x > +$T1B().DegreesToRadians(-45);
        if (flag12) {
          this._leftPincerTransform.Rotation = $T0F().op_Addition(this._leftPincerTransform.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
          this._rightPincerTransform.Rotation = $T0F().op_Subtraction(this._rightPincerTransform.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
          this._leftPincerTransformUp.Rotation = $T0F().op_Addition(this._leftPincerTransformUp.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
          this._rightPincerTransformUp.Rotation = $T0F().op_Subtraction(this._rightPincerTransformUp.Rotation.MemberwiseClone(), $S0D().Construct((1 * +$T15().get_DeltaTime()), 0, 0)).MemberwiseClone();
        } else {
          var flag13 = +this._rightPincerTransform.Rotation.x <= +$T1B().DegreesToRadians(-45);
          if (flag13) {
            this._move = false;
            this._open = true;
          }
        }
      }
    }
    (this._sceneRenderer).Render(this.RenderCanvas$RC$value);
    this.Present();
  }; 

  function Test_Resize () {
    (this.RenderCanvas$RC$value).Viewport(
      0, 
      0, 
      this.get_Width(), 
      this.get_Height()
    );
    var aspect = +((+(this.get_Width()) / +(this.get_Height())));
    var projection = $T18().CreatePerspectiveFieldOfView(0.7853982, aspect, 0.01, 200);
    (this.RenderCanvas$RC$value.Projection = projection.MemberwiseClone());
  }; 

  JSIL.MakeType({
      BaseType: $asm04.TypeRef("Fusee.Engine.Core.RenderCanvas"), 
      Name: "FuseeApp.Test", 
      IsPublic: true, 
      IsReferenceType: true, 
      MaximumConstructorArguments: 0, 
    }, function ($ib) {
    $ = $ib;

    $.Method({Static:false, Public:true }, ".ctor", 
      JSIL.MethodSignature.Void, 
      Test__ctor
    );

    $.Method({Static:false, Public:false}, "Creator", 
      new JSIL.MethodSignature($asm04.TypeRef("Fusee.Engine.Core.InputDevice"), [$asm03.TypeRef("Fusee.Engine.Common.IInputDeviceImp")]), 
      Test_Creator
    );

    $.Method({Static:false, Public:true , Virtual:true }, "Init", 
      JSIL.MethodSignature.Void, 
      Test_Init
    );

    $.Method({Static:false, Public:true , Virtual:true }, "RenderAFrame", 
      JSIL.MethodSignature.Void, 
      Test_RenderAFrame
    );

    $.Method({Static:false, Public:true , Virtual:true }, "Resize", 
      JSIL.MethodSignature.Void, 
      Test_Resize
    );

    $.Field({Static:true , Public:false}, "_angleHorz", $.Single, 0.7853982);

    $.Field({Static:true , Public:false}, "_angleVert", $.Single);

    $.Field({Static:true , Public:false}, "_distance", $.Single);

    $.Field({Static:true , Public:false}, "_angleVelHorz", $.Single);

    $.Field({Static:true , Public:false}, "_angleVelVert", $.Single);

    $.Field({Static:true , Public:false}, "_distanceVel", $.Single);

    $.Constant({Static:true , Public:false}, "RotationSpeed", $.Single, 7);

    $.Constant({Static:true , Public:false}, "Damping", $.Single, 0.8);

    $.Field({Static:false, Public:false}, "_scene", $asm0B.TypeRef("Fusee.Serialization.SceneContainer"));

    $.Field({Static:false, Public:false}, "_sceneRenderer", $asm04.TypeRef("Fusee.Engine.Core.SceneRenderer"));

    $.Field({Static:false, Public:false}, "_lowerAxleTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_middleAxleTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_upperAxleTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_footTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_pointer", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_rightPincerTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_leftPincerTransform", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_rightPincerTransformUp", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_leftPincerTransformUp", $asm0B.TypeRef("Fusee.Serialization.TransformComponent"));

    $.Field({Static:false, Public:false}, "_virtualPos", $asm0A.TypeRef("Fusee.Math.Core.float3"));

    $.Field({Static:false, Public:false}, "_open", $.Boolean);

    $.Field({Static:false, Public:false}, "_move", $.Boolean);


    function Test__cctor () {
      $thisType._angleHorz = 0.7853982;
    }; 

    $.Method({Static:true , Public:false}, ".cctor", 
      JSIL.MethodSignature.Void, 
      Test__cctor
    );


    return function (newThisType) { $thisType = newThisType; }; 
  })
    .Attribute($asm03.TypeRef("Fusee.Engine.Common.FuseeApplicationAttribute"));

})();

/* class FuseeApp.Test+<>c */ 

(function $l$gc$Members () {
  var $, $thisType;
  var $T00 = function () {
    return ($T00 = JSIL.Memoize($asm0B.Fusee.Serialization.SceneNodeContainer)) ();
  };
  var $T01 = function () {
    return ($T01 = JSIL.Memoize($asm0F.System.String)) ();
  };


  function $l$gc__ctor () {
  }; 

  function $l$gc_$lInit$gb__22_0 (node) {
    return node.Name == "LowerAxle";
  }; 

  function $l$gc_$lInit$gb__22_1 (node) {
    return node.Name == "MiddleAxle";
  }; 

  function $l$gc_$lInit$gb__22_2 (node) {
    return node.Name == "UpperAxle";
  }; 

  function $l$gc_$lInit$gb__22_3 (node) {
    return node.Name == "Foot";
  }; 

  function $l$gc_$lInit$gb__22_4 (node) {
    return node.Name == "RightLowerAxle";
  }; 

  function $l$gc_$lInit$gb__22_5 (node) {
    return node.Name == "LeftLowerAxle";
  }; 

  function $l$gc_$lInit$gb__22_6 (node) {
    return node.Name == "RightHigherAxle";
  }; 

  function $l$gc_$lInit$gb__22_7 (node) {
    return node.Name == "LeftHigherAxle";
  }; 

  function $l$gc_$lInit$gb__22_8 (node) {
    return node.Name == "Pointer";
  }; 

  JSIL.MakeType({
      BaseType: $asm0F.TypeRef("System.Object"), 
      Name: "FuseeApp.Test+<>c", 
      IsPublic: false, 
      IsReferenceType: true, 
      MaximumConstructorArguments: 0, 
    }, function ($ib) {
    $ = $ib;

    $.Method({Static:false, Public:true }, ".ctor", 
      JSIL.MethodSignature.Void, 
      $l$gc__ctor
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_0", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_0
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_1", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_1
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_2", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_2
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_3", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_3
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_4", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_4
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_5", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_5
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_6", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_6
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_7", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_7
    );

    $.Method({Static:false, Public:false}, "$lInit$gb__22_8", 
      new JSIL.MethodSignature($.Boolean, [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]), 
      $l$gc_$lInit$gb__22_8
    );

    $.Field({Static:true , Public:true , ReadOnly:true }, "$l$g9", $.Type);

    $.Field({Static:true , Public:true }, "$l$g9__22_0", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_1", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_2", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_3", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_4", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_5", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_6", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_7", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));

    $.Field({Static:true , Public:true }, "$l$g9__22_8", $asm0F.TypeRef("System.Predicate`1", [$asm0B.TypeRef("Fusee.Serialization.SceneNodeContainer")]));


    function $l$gc__cctor () {
      $thisType.$l$g9 = new $thisType();
    }; 

    $.Method({Static:true , Public:false}, ".cctor", 
      JSIL.MethodSignature.Void, 
      $l$gc__cctor
    );


    return function (newThisType) { $thisType = newThisType; }; 
  })
    .Attribute($asm0F.TypeRef("System.Runtime.CompilerServices.CompilerGeneratedAttribute"));

})();

