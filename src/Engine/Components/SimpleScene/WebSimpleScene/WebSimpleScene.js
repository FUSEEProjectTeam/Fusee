
var $customMsCore = JSIL.GetAssembly("mscorlib");
var $fuseeSimpleScene = JSIL.GetAssembly("Fusee.Engine.SimpleScene");

JSIL.ImplementExternals("Fusee.Engine.SimpleScene.VisitorCallerFactory", function ($)
{
    // Hand coded replacements saving some levels of indirection. 
    // One call to GetMethodImplementation, one nesting of function calls and wrapping 
    // the params into an array.
    $.Method({ Static: true, Public: true }, "MakeNodeVistor",
      new JSIL.MethodSignature($fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitorHelpers/VisitNodeMethod"), [$customMsCore.TypeRef("System.Reflection.MethodInfo")], []),
      function VisitorCallerFactory_MakeNodeVisitor(info) {
          var $closure0 = {};
          $closure0.method = JSIL.$GetMethodImplementation(info);
          return function (visitor, node) {
              return this.method.call(visitor, node);
          }.bind($closure0);
      }
    );


    $.Method({ Static: true, Public: true }, "MakeComponentVisitor",
      new JSIL.MethodSignature($fuseeSimpleScene.TypeRef("Fusee.Engine.SimpleScene.SceneVisitorHelpers/VisitComponentMethod"), [$customMsCore.TypeRef("System.Reflection.MethodInfo")], []),
      function VisitorCallerFactory_MakeComponentVisitor (info) {
          var $closure0 = {};
          $closure0.method = JSIL.$GetMethodImplementation(info);
          return function (visitor, component) {
              return this.method.call(visitor, component);
          }.bind($closure0);
      }
    );

});
