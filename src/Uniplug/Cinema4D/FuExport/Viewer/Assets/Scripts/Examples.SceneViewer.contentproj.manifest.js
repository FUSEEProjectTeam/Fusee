/* The size of these files were reduced. The following function fixes all references. */
var $customMSCore = JSIL.GetAssembly("mscorlib");
var $customSys = JSIL.GetAssembly("System");
var $customSysConf = JSIL.GetAssembly("System.Configuration");
var $customSysCore = JSIL.GetAssembly("System.Core");
var $customSysNum = JSIL.GetAssembly("System.Numerics");
var $customSysXml = JSIL.GetAssembly("System.Xml");
var $customSysSec = JSIL.GetAssembly("System.Security");

if (typeof (contentManifest) !== "object") { contentManifest = {}; };
contentManifest["Examples.SceneViewer.contentproj"] = [
    ["Script",	"Fusee.Engine.Imp.WebAudio.js",	{  "sizeBytes": 8151 }],
    ["Script",	"Fusee.Engine.Imp.WebNet.js",	{  "sizeBytes": 6618 }],
    ["Script",	"Fusee.Engine.Imp.WebGL.js",	{  "sizeBytes": 103435 }],
    ["Script",	"Fusee.Engine.Imp.WebInput.js",	{  "sizeBytes": 6868 }],
    ["Script",	"XirkitScript.js",	{  "sizeBytes": 43845 }],
    ["Image",	"Assets/FuseeLogo150.png",	{  "sizeBytes": 19685 }],
    ["Font",	"Assets/Lato-Black.ttf",	{  "sizeBytes": 114588 }],
    ["File",	"Assets/Model.fus",	{  "sizeBytes": 2419 }],
    ];