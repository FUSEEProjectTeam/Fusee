
var $customMsCore = JSIL.GetAssembly("mscorlib");
var $fuseeSimpleScene = JSIL.GetAssembly("Fusee.Engine.SimpleScene");

JSIL.ImplementExternals("Fusee.Engine.SimpleScene.VisitorCallerFactory_NONO", function ($)
{
    // Hand coded replacements saving some levels of indirection. 
    // One call to GetMethodImplementation, one nesting of function calls and wrapping 
    // the params into an array.
    $.Method({ Static: true, Public: true }, "MakeNodeVistor",
      new JSIL.MethodSignature($fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitorHelpers/VisitNodeMethod"), [$fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitor"), $customMsCore.TypeRef("System.Reflection.MethodInfo")], []),
      function VisitorCallerFactory_MakeNodeVisitor(visitor, info) {
          var $closure0 = {};
          $closure0.method = JSIL.$GetMethodImplementation(info);
          $closure0.visitor = visitor;
          return function (node) {
              return this.method.call(this.visitor, node);
          }.bind($closure0);
      }
    );


    $.Method({ Static: true, Public: true }, "MakeComponentVisitor",
      new JSIL.MethodSignature($fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitorHelpers/VisitComponentMethod"), [$fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitor"), $customMsCore.TypeRef("System.Reflection.MethodInfo")], []),
      function VisitorCallerFactory_MakeComponentVisitor (visitor, info) {
          var $closure0 = {};
          $closure0.method = JSIL.$GetMethodImplementation(info);
          $closure0.visitor = visitor;
          return function (component) {
              return this.method.call(this.visitor, component);
          }.bind($closure0);
      }
    );

});
