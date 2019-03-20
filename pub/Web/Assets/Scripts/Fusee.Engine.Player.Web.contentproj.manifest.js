/* The size of these files were reduced. The following function fixes all references. */
var $customMSCore = JSIL.GetAssembly("mscorlib");
var $customSys = JSIL.GetAssembly("System");
var $customSysConf = JSIL.GetAssembly("System.Configuration");
var $customSysCore = JSIL.GetAssembly("System.Core");
var $customSysNum = JSIL.GetAssembly("System.Numerics");
var $customSysXml = JSIL.GetAssembly("System.Xml");
var $customSysSec = JSIL.GetAssembly("System.Security");

if (typeof (contentManifest) !== "object") { contentManifest = {}; };
contentManifest["Fusee.Engine.Player.Web.contentproj"] = [
    ["Script",	"Fusee.Base.Core.Ext.js",	{  "sizeBytes": 1273 }],
    ["Script",	"Fusee.Base.Imp.Web.Ext.js",	{  "sizeBytes": 13491 }],
    ["Script",	"opentype.js",	{  "sizeBytes": 166330 }],
    ["Script",	"Fusee.Xene.Ext.js",	{  "sizeBytes": 1441 }],
    ["Script",	"Fusee.Xirkit.Ext.js",	{  "sizeBytes": 44215 }],
    ["Script",	"Fusee.Engine.Imp.Graphics.Web.Ext.js",	{  "sizeBytes": 128092 }],
    ["Script",	"SystemExternals.js",	{  "sizeBytes": 11976 }],
    ["Image",	"Assets/FuseeAnim.gif",	{  "sizeBytes": 221729 }],
    ["Image",	"Assets/FuseeSpinning.gif",	{  "sizeBytes": 19491 }],
    ["Image",	"Assets/FuseeText.png",	{  "sizeBytes": 4009 }],
    ["File",	"Assets/Lato-Black.ttf",	{  "sizeBytes": 114588 }],
    ["File",	"Assets/Model.fus",	{  "sizeBytes": 984588 }],
    ["File",	"Assets/nineSlice.vert",	{  "sizeBytes": 7709 }],
    ["File",	"Assets/nineSliceTile.frag",	{  "sizeBytes": 2961 }],
    ["File",	"Assets/texture.frag",	{  "sizeBytes": 414 }],
    ["File",	"Assets/texture.vert",	{  "sizeBytes": 392 }],    ["File",    "Assets/roboter_arm.fus", {  "sizeBytes": 3588774 }],
    ["Image",    "Assets/xy-plane.png", {  "sizeBytes": 17479 }],
    ["Image",    "Assets/xz-plane.png", {  "sizeBytes": 10293 }],

    ];