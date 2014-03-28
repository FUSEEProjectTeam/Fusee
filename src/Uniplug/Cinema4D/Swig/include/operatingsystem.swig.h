///////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
///////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
///////////////////////////////////////////////////////////

// interface functions

#ifndef __OPERATINGSYSTEM_H
#define __OPERATINGSYSTEM_H

#include "include\ge_math.swig.h"

#include "include\ge_prepass.swig.h"
#include "include\c4d_basetime.swig.h"
#include "include\vector4.swig.h"
#include "include\matrix4.swig.h"
#include "include\ddoc.swig.h"

#define API_VERSION	12000

class C4DAtom;
class C4DAtomGoal;
class AtomArray;
class NodeData;
class GeListNode;
class GeListHead;
class GeMarker;
class AliasTrans;
class Registry;
class CDialog;
class CUserArea;
class _GeListView;
class _SimpleListView;
class BaseTag;
class MPBaseThread;
class Semaphore;
class VariableTag;
class BaseTime;
class BaseChannel;
class BaseContainer;
class BaseDocument;
class BaseSelect;
class HyperFile;
class MemoryFileStruct;
class BaseList2D;
class BaseObject;
class SplineObject;
class GeSignal;
class BaseShader;
class PointObject;
class PolygonObject;
class LineObject;
class MultipassObject;
class BaseDraw;
class BaseDrawHelp;
class BaseView;
class BaseLink;
class String;
class BaseBitmap;
class MemoryPool;
class BaseMaterial;
class Material;
class BaseMaterial;
class BaseVideoPost;
class RenderData;
class LocalFileTime;
class Render;
class TextureTag;
class MovieSaver;
class IpConnection;
class BrowseFiles;
class BrowseVolumes;
class Parser;
class BaseFile;
class AESFile;
class SelectionTag;
class BaseTag;
class LassoSelection;
class UVWTag;
class ObjectSafety;
class BaseSceneHook;
class ParticleTag;
class StickTextureTag;
class Particle;
class LocalResource;
class HierarchyHelp;
class FolderSequence;
class SoundSequence;
class BaseSound;
class Stratified2DRandom;
class BaseThread;
class EnumerateEdges;
class PaintTexture;
class PaintLayer;
class PaintLayerBmp;
class SDKBrowserURL;
class PaintLayerMask;
class PaintLayerFolder;
class PaintBitmap;
class PaintMaterial;
class LayerSet;
class EditorWindow;
class VPBuffer;
class GeData;
class Description;
class DescID;
class DynamicDescription;
class Description;
class DescID;
class BaseContainer;
class Filename;
class BasePlugin;
class PriorityList;
class LensGlowStruct;
class CBaseFrame;
class PolyTriangulate;
class ViewportSelect;
class Pgon;
class NgonBase;
class MultipassBitmap;
class Coffee;
class VALUE;
class GeData;
class CKey;
class CCurve;
class CTrack;
class GeClipMap;
class CAnimInfo;
class GlString;
class Gradient;
class GlProgramFactory;
class GlFrameBuffer;
class GlVertexBuffer;
class LayerObject;
class C4DObjectList;
class LayerSetHelper;
class BaseBitmapLink;
class BitmapLoaderPlugin;
class ColorProfile;
class ColorProfileConvert;
class SplineLengthData;
class UnitScaleData;
class ParserCache;
class GlVertexBufferAttributeInfo;
class GlVertexBufferVectorInfo;

struct RayShaderStackElement;
struct AssetData;
struct LayerData;
struct CustomDataType;
struct ParticleDetails;
struct PolyInfo;
struct IlluminanceSurfacePointData;
struct SData;
struct VolumeData;
struct Ray;
struct Tangent;
struct UVWStruct;
struct Segment;
struct TexList;
struct CPolygon;
struct RayHitID;
struct InitRenderStruct;
struct RayObject;
struct RayLight;
struct PolyVector;
struct TexData;
struct VideoPostStruct;
struct SurfaceData;
struct RayPolyWeight;
struct MouseDownInfo;
struct DrawInfo;
struct RayLightCache;
struct IconData;
struct C4D_GraphView;
struct NODEPLUGIN;
struct VPFragment;
struct BranchInfo;
struct AnimValue;
struct ObjectColorProperties;
struct PixelFragment;
struct ChannelData;
struct CUSTOMDATATYPEPLUGIN;
struct RESOURCEDATATYPEPLUGIN;
struct TempUVHandle;
struct TextureSize;
struct ViewportPixel;
struct NgonNeighbor;
struct SurfaceIntersection;
struct ARRAY;
struct OBJECT;
struct CSTRING;
struct CLASS;
struct C4DPL_CommandLineArgs;
struct BakeProgressInfo;
struct GlLight;
struct GlGetIdentity;
struct OITInfo;
struct SoundInfo;
struct BitmapRenderRegion;
struct GeRWSpinlock;
struct GeSpinlock;
struct PickSessionDataStruct;
struct AnaglyphCameraInfo;
struct GlVertexSubBufferData;
struct GlVertexBufferDrawSubbuffer;
struct GlFBAdditionalTextureInfo;
struct GlTextureInfo;
struct BaseSelectData;
struct C4D_BitmapFilter;

// function prototypes
//
//typedef Int32 CDialogMessage(CDialog *cd, CUserArea *cu, BaseContainer *msg);
//typedef void ListViewCallBack(Int32 &res_type,void *&result,void *userdata,void *secret,Int32 cmd,Int32 line,Int32 col,void *data1);
//typedef void IlluminationModel(VolumeData *sd, RayLightCache *rlc, void *dat);
//typedef Bool COFFEE_ERRORHANDLER(void *priv_data, const BaseContainer &bc);
//typedef void* (*GlProgramFactoryAllocPrivate)();
//typedef void (*GlProgramFactoryFreePrivate)(void* pData);
//typedef void* (*GlProgramFactoryAllocDescription)();
//typedef void (*GlProgramFactoryFreeDescription)(void* pData);
//typedef void (*GlProgramFactoryCreateTextureFunctionCallback)(const Float* prIn, Float* prOut, void* pData);
//typedef void (*GlProgramFactoryMessageCallback)(Int32 lMessage, const void* pObj, LULONG ulIndex, GlProgramFactory* pFactory);
//typedef Int32 (*GlProgramFactoryErrorHandler)(GlProgramType type, const char* pszError);
//typedef void (*GlVertexBufferDrawElementCallback)(Int32 lElement, void* pData);
//typedef void (*BrowserPopupCallback)(void *userdata, Int32 cmd, SDKBrowserURL &url);
//
//typedef void ThreadMain(void *data);
//typedef Bool ThreadTest(void *data);
//typedef const Char *ThreadName(void *data);
//typedef void ProgressHook(Float p, RENDERPROGRESSTYPE progress_type, void *private_data);
//typedef void BakeProgressHook(BakeProgressInfo* info);
//
//typedef void *HierarchyAlloc(void *main);
//typedef void HierarchyFree(void *main, void *data);
//typedef void HierarchyCopyTo(void *main, void *src, void *dst);
//typedef Bool HierarchyDo(void *main, void *data, BaseObject *op, const Matrix &mg, Bool controlobject);
//
//typedef void PopupEditTextCallback(Int32 mode, const String &text, void *userdata);
//
//typedef void (*LASTCURSORINFOFUNC)();
//
//typedef Bool (*SaveCallbackImageFunc)(RDATA_SAVECALLBACK_CMD cmd, void* userdata, BaseDocument* doc, Int32 framenum, BaseBitmap* bmp, const Filename &fn);
//
//typedef Bool BackgroundHandler(void *data, BACKGROUNDHANDLERCOMMAND command, BACKGROUNDHANDLERFLAGS parm);
//
//typedef void (*C4D_CrashHandler)(Char *crashinfo);
//typedef void (*C4D_CreateOpenGLContext)(void* context, void* root, ULONG flags);
//typedef void (*C4D_DeleteOpenGLContext)(void* context, ULONG flags);
//
//typedef GeData CoffeeEditorCallback(BaseList2D *obj, const BaseContainer &msg);
//typedef void (*V_CODE)(Coffee*, VALUE*&, Int32);
//
//typedef void* UVWHandle;
//typedef void* NormalHandle;
//
// structure definitions
struct UVWStruct
{
	UVWStruct(_DONTCONSTRUCT dc) : a(DC), b(DC), c(DC), d(DC) { }

	UVWStruct(void) {}
	UVWStruct(const Vector &t_a, const Vector &t_b, const Vector &t_c, const Vector &t_d) { a=t_a; b=t_b; c=t_c; d=t_d; }
	UVWStruct(const Vector &t_a, const Vector &t_b, const Vector &t_c) { a=t_a; b=t_b; c=t_c; }

	Vector a,b,c,d;
};
//
//struct NormalStruct
//{
//	Vector a,b,c,d;
//
//	NormalStruct() {}
//	NormalStruct(_DONTCONSTRUCT dc) : a(DC), b(DC), c(DC), d(DC) {}
//	NormalStruct(const Vector &t_a, const Vector &t_b, const Vector &t_c, const Vector &t_d) { a=t_a; b=t_b; c=t_c; d=t_d; }
//};
//
//struct IconData
//{
//	IconData() { bmp=NULL; x=y=w=h=0; flags=ICONDATAFLAGS_0; }
//	IconData(BaseBitmap *t_bmp, Int32 t_x, Int32 t_y, Int32 t_w, Int32 t_h) { bmp=t_bmp; x=t_x; y=t_y; w=t_w; h=t_h; }
//
//	BaseBitmap			*bmp;
//	Int32						x,y,w,h;
//	ICONDATAFLAGS		flags;
//};

struct ModelingCommandData
{
	ModelingCommandData()
		{
			BaseDocument* doc=NULL;	BaseObject* op=NULL;BaseContainer* bc=NULL;
		MODELINGCOMMANDMODE mode=MODELINGCOMMANDMODE_ALL;
		MODELINGCOMMANDFLAGS flags=MODELINGCOMMANDFLAGS_0;
		AtomArray* result=NULL;
		AtomArray* arr=NULL;
		
		 }
	//~ModelingCommandData();

	BaseDocument*					doc;
	BaseObject*						op;
	BaseContainer*				bc;
	MODELINGCOMMANDMODE		mode;
	MODELINGCOMMANDFLAGS	flags;

	AtomArray*      arr;
	AtomArray*      result;
	
};
//
//struct MultiPassChannel
//{
//	Int32   id;
//	String *name;
//	MULTIPASSCHANNEL flags;
//};
//
//enum VIEWPORTSELECTFLAGS
//{
//	VIEWPORTSELECTFLAGS_0									= 0,
//	VIEWPORTSELECTFLAGS_USE_HN						= 1,
//	VIEWPORTSELECTFLAGS_USE_DEFORMERS			= 2,
//	VIEWPORTSELECTFLAGS_REGION_SELECT			= 4,
//	VIEWPORTSELECTFLAGS_IGNORE_HIDDEN_SEL	= 8,
//	VIEWPORTSELECTFLAGS_USE_DISPLAY_MODE	= 16,
//} ENUM_END_FLAGS(VIEWPORTSELECTFLAGS);
//
//
//// function tables
//
//struct C4D_Shader
//{
//	Float						(*SNoise							)(Vector p);
//	Float						(*SNoiseT							)(Vector p, Float t);
//	Float						(*Noise								)(Vector p);
//	Float						(*NoiseT							)(Vector p, Float t);
//	Float						(*PNoise							)(Vector p, Vector d);
//	Float						(*PNoiseT							)(Vector p, Float t, Vector d, Float dt);
//	Float						(*Turbulence					)(Vector p, Float oct, Bool abs);
//	Float						(*TurbulenceT					)(Vector p, Float t, Float oct, Bool abs);
//	Float						(*WavyTurbulence			)(Vector p, Float t, Float oct, Float start);
//	void						(*InitFbm							)(Float *table, Int32 max_octaves, Float lacunarity, Float h);
//	Float						(*Fbm									)(Float *table, Vector p, Float oct);
//	Float						(*FbmT								)(Float *table, Vector p, Float t, Float oct);
//	Float						(*RidgedMultifractal	)(Float *table, Vector p, Float oct, Float offset, Float gain);
//	Float						(*CalcSpline					)(Float x, Float *knot, Int32 nknots);
//	Vector					(*CalcSplineV					)(Float x, Vector *knot, Int32 nknots);
//
//	void						(*Illuminance1				)(VolumeData *sd, Vector *diffuse, Vector *specular, Float exponent);
//
//	Int32						(*GetCurrentCPU				)(VolumeData *sd);
//	Int32						(*GetCPUCount					)(VolumeData *sd);
//	void*						(*GetRayData					)(VolumeData *sd, Int32 i);
//
//	RayObject*			(*GetObj							)(VolumeData *sd, Int32 i);
//	Int32						(*GetObjCount					)(VolumeData *sd);
//
//	RayLight*				(*GetLight						)(VolumeData *sd, Int32 i);
//	Int32						(*GetLightCount				)(VolumeData *sd);
//
//	RayObject*			(*ID_to_Obj						)(VolumeData *sd, const RayHitID &id);
//
//	void						(*GetUVW							)(VolumeData *sd, RayObject *op, Int32 uvwind, Int32 i, PolyVector *uvw);
//	Vector					(*GetPointUVW					)(VolumeData *sd, TexData *tdp, const RayHitID &lhit, const LVector &p);
//	void						(*GetNormals					)(VolumeData *sd, RayObject *op, Int32 polygon, PolyVector *norm);
//	TexData*				(*GetTexData					)(VolumeData *sd, RayObject *op, Int32 index);
//	LVector					(*GetNormal						)(VolumeData *sd, RayObject *op, Int32 polygon, Bool second);
//	LVector					(*GetSmoothedNormal		)(VolumeData *sd, const RayHitID &hit, const LVector &p);
//	Bool						(*GetRS								)(VolumeData *sd, const RayHitID &hit, const LVector &p, Float *r, Float *s);
//
//	void						(*GetRay							)(VolumeData *sd, Float x, Float y, Ray *ray);
//	LVector					(*ScreenToCamera			)(VolumeData *sd, const LVector &p);
//	LVector					(*CameraToScreen			)(VolumeData *sd, const LVector &p);
//	Bool						(*ProjectPoint				)(VolumeData *sd, TexData *tdp, const RayHitID &lhit, const LVector &p, const LVector &n, Vector *uv);
//
//	VolumeData*			(*AllocVolumeData			)(void);
//	void						(*FreeVolumeData			)(VolumeData *sd);
//
//	void						(*StatusSetText				)(VolumeData *sd, const String *str);
//	void						(*StatusSetBar				)(VolumeData *sd, Float percentage);
//
//	TexData*				(*AllocTexData				)(void);
//	void						(*FreeTexData					)(TexData *td);
//	void						(*InitTexData					)(TexData *td);
//
//	Vector					(*CalcVisibleLight		)(VolumeData *sd, Ray *ray, Float maxdist, Vector *trans);
//	void						(*GetXY								)(VolumeData *sd, Int32 *x, Int32 *y, Int32 *scale);
//
//	Int32						(*Obj_to_Num					)(VolumeData *sd, RayObject *obj);
//	Int32						(*Light_to_Num				)(VolumeData *sd, RayLight *light);
//
//	void						(*CopyVolumeData			)(VolumeData *src, VolumeData *dst);
//
//	Bool						(*VPAllocateBuffer		)(Render *render, Int32 id, Int32 subid, Int32 bitdepth, Bool visible);
//	VPBuffer*				(*VPGetBuffer					)(Render *render, Int32 id, Int32 subid);
//	Int32						(*VPGetInfo						)(VPBuffer *buf, VPGETINFO type);
//	Bool						(*VPGetLine						)(VPBuffer *buf, Int32 x, Int32 y, Int32 cnt, void *data, Int32 bitdepth, Bool dithering);
//	Bool						(*VPSetLine						)(VPBuffer *buf, Int32 x, Int32 y, Int32 cnt, void *data, Int32 bitdepth, Bool dithering);
//
//	void						(*OutOfMemory					)(VolumeData *sd);
//	Float						(*GetLightFalloff			)(Int32 falloff, Float inner, Float outer, Float dist);
//
//	Bool						(*TestBreak						)(VolumeData *sd);
//
//	void						(*VPGetRenderData			)(Render *render, BaseContainer *bc);
//	void						(*VPSetRenderData			)(Render *render, const BaseContainer *bc);
//
//	void						(*Illuminance					)(VolumeData *sd, Vector *diffuse, Vector *specular, IlluminationModel *model, void *data);
//
//	BaseVideoPost*  (*FindVideoPost				)(VolumeData *sd, Int32 i, Bool index);
//	VPFragment**		(*VPGetFragments			)(VolumeData *sd, Int32 x, Int32 y, Int32 cnt, VPGETFRAGMENTS flags);
//	Bool						(*AddLensGlowEx				)(VolumeData *sd, LensGlowStruct *lgs, Vector *lgs_pos, Int32 lgs_cnt, Float intensity);
//
//	RayLight*				(*AllocRayLight				)(BaseDocument *doc, BaseObject *op);
//	void						(*FreeRayLight				)(RayLight *lgt);
//
//	Int32						(*TranslateObjIndex		)(VolumeData *sd, Int32 index, Bool old_to_new);
//	Bool						(*TranslatePolygon		)(VolumeData *sd, RayObject *op, Int32 local_index, Vector *previous_points);
//	Bool						(*SampleLensFX				)(VolumeData *sd, VPBuffer *rgba, VPBuffer *fx, BaseThread *bt);
//	Int32						(*VPAllocateBufferFX	)(Render *render, Int32 id, const String &name, Int32 bitdepth, Bool visible);
//
//	VolumeData*			(*VPGetInitialVolumeData)(Render *render, Int32 cpu);
//
//	Bool						(*CalcFgBg						)(VolumeData *sd, Bool foreground, Int32 x, Int32 y, Int32 subx, Int32 suby, Vector *color, Float *alpha);
//	Int32						(*GetRayPolyState			)(VolumeData *sd, RayObject *op, Int32 i);
//
//	void						(*GetWeights					)(VolumeData *sd, const RayHitID &hit, const LVector &p, RayPolyWeight *wgt);
//	void						(*InitVolumeData			)(VolumeData *src, VolumeData *dst);
//
//	Vector					(*TraceColor					)(VolumeData *sd, Ray *ray, Float maxdist, const RayHitID &lhit, SurfaceIntersection *si);
//	Bool  					(*TraceGeometry				)(VolumeData *sd, Ray *ray, Float maxdist, const RayHitID &lhit, SurfaceIntersection *si);
//	void  					(*GetSurfaceData		  )(VolumeData *sd, SurfaceData *cd, Int32 calc_illum, Bool calc_shadow, Bool calc_refl, Bool calc_trans, Ray *ray, const SurfaceIntersection &si);
//
//	void						(*SkipRenderProcess   )(VolumeData *sd);
//
//	void*						(*VPGetPrivateData    )(Render *render);
//	Render*					(*GetRenderInstance   )(VolumeData *sd);
//
//	Vector					(*CentralDisplacePointUV)(VolumeData *sd, RayObject *op, Int32 local_id, Int32 uu, Int32 vv);
//	Vector					(*CalcDisplacementNormals)(VolumeData *sd, Float par_u, Float par_v, Int32 u0, Int32 v0, Int32 u1, Int32 v1, Int32 u2, Int32 v2, const Vector &a, const Vector &b, const Vector &c, RayObject *op, Int32 polynum);
//
//	Stratified2DRandom* (*SNAlloc										)(void);
//	void								(*SNFree										)(Stratified2DRandom *rnd);
//	Bool						(Stratified2DRandom::*SNInit		)(Int32 initial_value, Bool reset);
//	void						(Stratified2DRandom::*SNGetNext	)(Float *xx, Float *yy);
//	BAKE_TEX_ERR    (*BakeTexture										)(BaseDocument* doc, const BaseContainer &data, BaseBitmap *bmp, BaseThread* th, BakeProgressHook* hook, BakeProgressInfo* info);
//	BaseDocument*   (*InitBakeTexture   						)(BaseDocument* doc, TextureTag* textag, UVWTag* texuvw, UVWTag* destuvw, const BaseContainer &bc, BAKE_TEX_ERR* err, BaseThread* th);
//	BaseDocument*   (*InitBakeTextureA   						)(BaseDocument* doc, TextureTag** textags, UVWTag** texuvws, UVWTag** destuvws, Int32 cnt, const BaseContainer &bc, BAKE_TEX_ERR* err, BaseThread* th);
//
//	void						(*GetDUDV   										)(VolumeData *vd, TexData *tex, const LVector &p, const LVector &phongn, const LVector &orign, const RayHitID &hitid, Bool forceuvw, Vector *ddu, Vector *ddv);
//	void						(*CalcArea											)(VolumeData *sd, RayLight *light, Bool nodiffuse, Bool nospecular, Float specular_exponent, const LVector &ray_vector, const LVector &p, const LVector &bumpn, const LVector &orign, RAYBIT raybits, Vector *diffuse, Vector *specular);
//	Bool						(*Illuminate										)(VolumeData *sd, RayLight *light, Vector *col, LVector *lv, const LVector &p, const LVector &bumpn, const LVector &phongn, const LVector &orign, const LVector &ray_vector, ILLUMINATEFLAGS calc_shadow, const RayHitID &hitid, RAYBIT raybits, Bool cosine_cutoff, Vector *xshadow);
//	void						(*IlluminanceSurfacePoint				)(VolumeData *sd, IlluminanceSurfacePointData *f, Vector *diffuse, Vector *specular);
//	Vector					(*IlluminanceAnyPoint						)(VolumeData *sd, const LVector &p, ILLUMINATEFLAGS flags, RAYBIT raybits);
//	Vector					(*CalcShadow										)(VolumeData *sd, RayLight *light, const LVector &p, const LVector &bumpn, const LVector &phongn, const LVector &orign, const LVector &rayv, Bool transparency, const RayHitID &hitid, RAYBIT raybits);
//
//	Bool						(*SetRenderProperty							)(Render *render, Int32 id, const GeData &dat);
//	void						(*SetXY													)(VolumeData *sd, Float x, Float y);
//	void						(*InitSurfacePointProperties		)(VolumeData *sd, TexData *td);
//
//	Float						(*SNoiseP												)(Vector p, Float t, Int32 t_repeat);
//	Float						(*TurbulenceP										)(Vector p, Float t, Float oct, Bool abs, Int32 t_repeat);
//	Float						(*FbmP													)(Float *table, Vector p, Float t, Float oct, Int32 t_repeat);
//	Float						(*RidgedMultifractalP						)(Float *table, Vector p, Float t, Float oct, Float offset, Float gain, Int32 t_repeat);
//
//	Bool						(*AttachVolumeDataFake					)(VolumeData *sd, BaseObject *camera, const BaseContainer &renderdata, void *priv);
//	RayObject*			(*AllocRayObject								)(Int32 tex_cnt, void *priv);
//	void						(*FreeRayObject									)(RayObject *op);
//
//	Bool  					(*TraceGeometryEnhanced				  )(VolumeData *sd, Ray *ray, Float maxdist, const RayHitID &lhit, Int32 raydepth, RAYBIT raybits, LVector *oldray, SurfaceIntersection *si);
//	Vector					(*TraceColorDirect							)(VolumeData *sd, Ray *ray, Float maxdist, Int32 raydepth, RAYBIT raybits, const RayHitID &lhit, LVector *oldray, SurfaceIntersection *si);
//
//	Vector					(*CalcHardShadow								)(VolumeData *sd, const LVector &p1, const LVector &p2, CALCHARDSHADOW flags, const RayHitID &lhit, Int32 recursion_id, void *recursion_data);
//	void						(*StatusSetSpinMode							)(VolumeData *sd, Bool on);
//
//	Vector					(*TransformColor								)(const Vector &v, COLORSPACETRANSFORMATION colortransformation);
//
//	void            (*VPIccConvert									)(Render *rnd, void *data, Int32 xcnt, Int32 components, Bool inverse);
//
//	Bool						(*SaveShaderStack								)(VolumeData *sd, RayShaderStackElement *&stack, Int32 &stack_cnt);
//	Bool						(*RestoreShaderStack						)(VolumeData *sd, RayShaderStackElement *stack, Int32 stack_cnt);
//
//	RayObject*			(*GetSky												)(VolumeData *sd, Int32 i);
//	Int32						(*GetSkyCount										)(VolumeData *sd);
//
//	Bool						(*AddLensGlow										)(VolumeData *sd, LensGlowStruct *lgs, Vector *lgs_pos, Int32 lgs_cnt, Float intensity, Bool linear_workflow);
//};
//
//struct C4D_HyperFile
//{
//	Bool						(HyperFile::*Open								)(Int32 ident, const Filename &name, FILEOPEN mode, FILEDIALOG error_dialog);
//	Bool						(HyperFile::*Close							)();
//
//	Bool						(HyperFile::*WriteChar					)(Char  v);
//	Bool						(HyperFile::*WriteUChar					)(UChar v);
//	Bool						(HyperFile::*WriteWord					)(SWORD  v);
//	Bool						(HyperFile::*WriteUWord					)(UWORD v);
//	Bool						(HyperFile::*WriteLong					)(Int32  v);
//	Bool						(HyperFile::*WriteULong					)(ULONG v);
//	Bool						(HyperFile::*WriteReal					)(Float  v);
//	Bool						(HyperFile::*WriteSReal					)(Float32 v);
//	Bool						(HyperFile::*WriteLReal					)(LReal v);
//	Bool						(HyperFile::*WriteBool					)(Bool  v);
//	Bool						(HyperFile::*WriteTime					)(const BaseTime &v);
//	Bool						(HyperFile::*WriteVector				)(const Vector &v);
//	Bool						(HyperFile::*WriteSVector				)(const SVector &v);
//	Bool						(HyperFile::*WriteLVector				)(const LVector &v);
//	Bool						(HyperFile::*WriteMatrix				)(const Matrix &v);
//	Bool						(HyperFile::*WriteSMatrix				)(const SMatrix &v);
//	Bool						(HyperFile::*WriteLMatrix				)(const LMatrix &v);
//	Bool						(HyperFile::*WriteString				)(const String &v);
//	Bool						(HyperFile::*WriteFilename			)(const Filename &v);
//	Bool						(HyperFile::*WriteContainer			)(const BaseContainer &v);
//	Bool						(HyperFile::*WriteMemory				)(const void *data, VLONG count);
//	Bool						(HyperFile::*WriteLLong         )(LLONG v);
//
//	Bool						(HyperFile::*ReadChar						)(Char         *v);
//	Bool						(HyperFile::*ReadUChar					)(UChar        *v);
//	Bool						(HyperFile::*ReadWord						)(SWORD        *v);
//	Bool						(HyperFile::*ReadUWord					)(UWORD        *v);
//	Bool						(HyperFile::*ReadLong					  )(Int32         *v);
//	Bool						(HyperFile::*ReadULong					)(ULONG        *v);
//	Bool						(HyperFile::*ReadReal					  )(Float         *v);
//	Bool						(HyperFile::*ReadSReal					)(Float32        *v);
//	Bool						(HyperFile::*ReadLReal					)(LReal        *v);
//	Bool						(HyperFile::*ReadBool					  )(Bool         *v);
//	Bool						(HyperFile::*ReadTime						)(BaseTime     *v);
//	Bool						(HyperFile::*ReadVector					)(Vector       *v);
//	Bool						(HyperFile::*ReadSVector				)(SVector      *v);
//	Bool						(HyperFile::*ReadLVector				)(LVector      *v);
//	Bool						(HyperFile::*ReadMatrix					)(Matrix       *v);
//	Bool						(HyperFile::*ReadSMatrix				)(SMatrix      *v);
//	Bool						(HyperFile::*ReadLMatrix				)(LMatrix      *v);
//	Bool						(HyperFile::*ReadString					)(String       *v);
//	Bool						(HyperFile::*ReadFilename				)(Filename     *v);
//	Bool						(HyperFile::*ReadImage					)(BaseBitmap   *v);
//	Bool						(HyperFile::*ReadContainer			)(BaseContainer *v, Bool flush);
//	Bool						(HyperFile::*ReadMemory					)(void **data, VLONG *size);
//	Bool						(HyperFile::*ReadLLong					)(LLONG *v);
//
//	FILEERROR				(HyperFile::*GetError						)() const;
//	void						(HyperFile::*SetError						)(FILEERROR err);
//	Bool						(HyperFile::*ReadValueHeader		)(HYPERFILEVALUE *h);
//	Bool						(HyperFile::*SkipValue					)(HYPERFILEVALUE h);
//	Bool						(HyperFile::*WriteChunkStart		)(Int32 id, Int32 level);
//	Bool						(HyperFile::*WriteChunkEnd			)();
//	Bool						(HyperFile::*ReadChunkStart			)(Int32 *id, Int32 *level);
//	Bool						(HyperFile::*ReadChunkEnd				)();
//	Bool						(HyperFile::*SkipToEndChunk			)();
//	BaseDocument*		(HyperFile::*GetDocument				)() const;
//
//	Int32						(HyperFile::*GetFileVersion			)() const;
//
//	Bool						(HyperFile::*WriteImage					)(BaseBitmap *v, Int32 format, const BaseContainer *data, SAVEBIT savebits);
//	Bool						(HyperFile::*WriteArray					)(const void *data, HYPERFILEARRAY type, Int32 structure_increment, Int32 count);
//	Bool						(HyperFile::*ReadArray					)(void *data, HYPERFILEARRAY type, Int32 structure_increment, Int32 count);
//
//	HyperFile*			(*AllocHF             )();
//	void						(*FreeHF              )(HyperFile *&hf);
//
//	FILEERROR				(*ReadFile						)(BaseDocument *doc, GeListNode *node, const Filename &filename, Int32 ident, String *warning_string);
//	FILEERROR				(*WriteFile						)(BaseDocument *doc, GeListNode *node, const Filename &filename, Int32 ident);
//
//	Bool						(*WriteGeData					)(HyperFile *hf, const GeData &v);
//	Bool						(*ReadGeData					)(HyperFile *hf, GeData &v);
//};
//
//struct C4D_BaseContainer
//{
//	BaseContainer		*Default;
//
//	BaseContainer*	(*Alloc								)(Int32 id);
//	void						(*Free								)(BaseContainer *killme);
//	Bool						(*Compare							)(const BaseContainer &bc1, const BaseContainer &bc2);
//	void*						(*BrowseContainer			)(const BaseContainer *bc, Int32 *id, GeData **data, void *handle);
//
//	void						(BaseContainer::*SDKInit						)(Int32 id, const BaseContainer *src);
//	BaseContainer*	(BaseContainer::*GetClone						)(COPYFLAGS flags, AliasTrans *trans) const;
//	void						(BaseContainer::*FlushAll						)();
//	Int32						(BaseContainer::*GetId							)() const;
//	void						(BaseContainer::*SetId							)(Int32 newid);
//
//	void						(BaseContainer::*SetVoid						)(Int32 id, void *v);
//	void						(BaseContainer::*SetReal						)(Int32 id, Float v);
//	void						(BaseContainer::*SetBool						)(Int32 id, Bool v);
//	void						(BaseContainer::*SetLong						)(Int32 id, Int32 v);
//	void						(BaseContainer::*SetString					)(Int32 id, const String &v);
//	void						(BaseContainer::*SetFilename				)(Int32 id, const Filename &v);
//	void						(BaseContainer::*SetTime						)(Int32 id, const BaseTime &v);
//	void						(BaseContainer::*SetContainer				)(Int32 id, const BaseContainer &v);
//	void						(BaseContainer::*SetVector					)(Int32 id, const Vector &v);
//	void						(BaseContainer::*SetMatrix					)(Int32 id, const Matrix &v);
//
//	BaseContainer		(BaseContainer::*GetContainer				)(Int32 id) const;
//	BaseContainer*	(BaseContainer::*GetContainerInstance)(Int32 id) const;
//	Bool						(BaseContainer::*RemoveData					)(Int32 id);
//	Int32						(BaseContainer::*FindIndex					)(Int32 id, GeData **data) const;
//	Int32						(BaseContainer::*GetIndexId					)(Int32 index) const;
//	Bool						(BaseContainer::*RemoveIndex				)(Int32 index);
//	GeData*					(BaseContainer::*GetIndexData				)(Int32 index) const;
//
//	BaseList2D*			(BaseContainer::*GetLink						)(Int32 id, const BaseDocument *doc, Int32 instanceof) const;
//	void						(BaseContainer::*SetLink						)(Int32 id, C4DAtomGoal *link);
//
//	const GeData&		(BaseContainer::*GetData						)(Int32 id) const;
//	Bool						(BaseContainer::*GetParameter				)(const DescID &id, GeData &t_data) const;
//	Bool						(BaseContainer::*SetParameter				)(const DescID &id, const GeData &t_data);
//
//	GeData*					(BaseContainer::*InsData  					)(Int32 id, const GeData &n);
//	GeData*					(BaseContainer::*SetData  					)(Int32 id, const GeData &n);
//	void						(BaseContainer::*SetLLong						)(Int32 id, LLONG v);
//	GeData*					(BaseContainer::*InsDataAfter				)(Int32 id, const GeData &n, const GeData *last);
//	Bool						(BaseContainer::*CopyTo							)(BaseContainer *dest,COPYFLAGS flags,AliasTrans *trans) const;
//	void						(BaseContainer::*Sort								)();
//
//	Float						(BaseContainer::*GetReal						)(Int32 id, Float preset) const;
//	Bool						(BaseContainer::*GetBool						)(Int32 id, Bool preset) const;
//	Int32						(BaseContainer::*GetLong						)(Int32 id, Int32 preset) const;
//	String					(BaseContainer::*GetString					)(Int32 id, const String& preset) const;
//	Filename				(BaseContainer::*GetFilename				)(Int32 id, const Filename& preset) const;
//	BaseTime				(BaseContainer::*GetTime						)(Int32 id, const BaseTime& preset) const;
//	Vector					(BaseContainer::*GetVector					)(Int32 id, const Vector& preset) const;
//	Matrix					(BaseContainer::*GetMatrix					)(Int32 id, const Matrix& preset) const;
//	LLONG						(BaseContainer::*GetLLong						)(Int32 id, LLONG preset) const;
//	void*						(BaseContainer::*GetVoid						)(Int32 id, void* preset) const;
//	const GeData*		(BaseContainer::*GetDataPointer			)(Int32 id) const;
//	void						(BaseContainer::*Merge							)(const BaseContainer &src);
//};
//
//struct C4D_GeData
//{
//	void            (*Free                )(GeData *data);
//	Bool            (*IsEqual             )(const GeData *data,const GeData *data2);
//	Int32						(*GetType							)(const GeData *data);
//	void            (*CopyData						)(GeData *dest,const GeData *src,AliasTrans *aliastrans);
//
//	Bool						(*SetNil              )(GeData *data);
//	Bool						(*SetLong             )(GeData *data,Int32 n);
//	Bool						(*SetReal             )(GeData *data,Float n);
//	Bool						(*SetVector           )(GeData *data,const Vector &n);
//	Bool						(*SetMatrix           )(GeData *data,const Matrix &n);
//	Bool						(*SetString           )(GeData *data,const String *n);
//	Bool            (*SetFilename         )(GeData *data,const Filename *n);
//	Bool            (*SetBaseTime         )(GeData *data,const BaseTime &n);
//	Bool            (*SetBaseContainer    )(GeData *data,const BaseContainer *n);
//	Bool            (*SetLink             )(GeData *data,const BaseLink &n);
//
//	Int32						(*GetLong							)(const GeData *data);
//	Float						(*GetReal							)(const GeData *data);
//	const Vector&		(*GetVector						)(const GeData *data);
//	const Matrix&		(*GetMatrix						)(const GeData *data);
//	const String&		(*GetString						)(const GeData *data);
//	const Filename&	(*GetFilename					)(const GeData *data);
//	const BaseTime&	(*GetTime							)(const GeData *data);
//	BaseContainer*	(*GetContainer				)(const GeData *data);
//	BaseLink*				(*GetLink             )(const GeData *data);
//
//	Bool            (*SetCustomData				)(GeData *data,Int32 type,const CustomDataType &n);
//	CustomDataType* (*GetCustomData				)(const GeData *data,Int32 type);
//	Bool            (*InitCustomData			)(GeData *data,Int32 type);
//
//	Bool						(*SetLLong            )(GeData *data,LLONG n);
//	LLONG						(*GetLLong						)(const GeData *data);
//	void 						(*SetVoid							)(GeData *data, void *v);
//};
//
//struct C4D_String
//{
//	String					*Default; // safety value
//
//	String*					(*Alloc								)(void);
//	void						(*Init								)(String *str);
//	void						(*Free								)(String *str);
//	void						(*InitCString					)(String *str, const Char *txt, Int32 count, STRINGENCODING type);
//	void						(*InitArray						)(String *str, Int32 count, UWORD fillchr);
//	void						(*Flush								)(String *str);
//	void						(*CopyTo							)(const String *src, String *dst);
//	Bool						(*Add									)(String *dst,const String *src);
//
//	String					(*RealToString				)(Float v, Int32 vk, Int32 nk, Bool e, UWORD xchar);
//	String					(*LongToString				)(Int32 l);
//	String					(*LLongToString				)(LLONG l);
//	String					(*LLongToStringExEx		)(LLONG l);
//
//	UWORD						(*GetChr							)(const String *str,Int32 pos);
//	void						(*SetChr							)(String *str,Int32 pos,UWORD chr);
//
//	Int32						(String::*GetLength			)() const;
//	Bool						(String::*FindFirst			)(const String &str, Int32 *pos, Int32 start) const;
//	Bool						(String::*FindLast			)(const String &str, Int32 *pos, Int32 start) const;
//	void						(String::*Delete				)(Int32 pos,Int32 count);
//	void						(String::*Insert				)(Int32 pos, const String &str, Int32 start, Int32 end);
//	String					(String::*SubStr				)(Int32 start, Int32 count) const;
//	Float						(String::*ToReal				)(Int32 *error, Int32 unit, Int32 angular_type, Int32 base) const;
//	Int32						(String::*ToLong				)(Int32 *error) const;
//	String					(String::*ToUpper				)() const;
//	String					(String::*ToLower				)() const;
//	Int32						(String::*GetCStringLen	)(STRINGENCODING type) const;
//	Int32						(String::*GetCString		)(Char *cstr, Int32 max, STRINGENCODING type) const;
//	void						(String::*GetUcBlock		)(UWORD *Ubc, Int32 Max) const;
//	void						(String::*GetUcBlockNull)(UWORD *Ubc, Int32 Max) const;
//	void						(String::*SetUcBlock		)(const UWORD *Ubc, Int32 Count);
//	Int32						(String::*Compare				)(const String &dst) const;
//	Int32						(String::*LexCompare		)(const String &dst) const;
//	Int32						(String::*RelCompare		)(const String &dst) const;
//	Int32						(String::*ComparePart		)(const String &Str, Int32 cnt, Int32 pos) const;
//	Int32						(String::*LexComparePart)(const String &Str, Int32 cnt, Int32 pos) const;
//	Bool						(String::*FindFirstCh		)(UWORD ch, Int32 *Pos, Int32 Start) const;
//	Bool						(String::*FindLastCh		)(UWORD ch, Int32 *Pos, Int32 Start) const;
//	Bool						(String::*FindFirstUpper)(const String &find, Int32 *pos, Int32 start) const;
//	Bool						(String::*FindLastUpper	)(const String &find, Int32 *pos, Int32 start) const;
//};
//
//struct C4D_Bitmap
//{
//	BaseBitmap*			(*Alloc								)(void);
//	void						(*Free								)(BaseBitmap *bm);
//	BaseBitmap*			(*GetClone						)(const BaseBitmap *src);
//	Int32						(*GetBw								)(const BaseBitmap *bm);
//	Int32						(*GetBh								)(const BaseBitmap *bm);
//	Int32						(*GetBt								)(const BaseBitmap *bm);
//	Int32						(*GetBpz							)(const BaseBitmap *bm);
//	IMAGERESULT			(*Init2								)(BaseBitmap *bm, const Filename *name, Int32 frame, Bool *ismovie);
//	void					  (*FlushAll						)(BaseBitmap *bm);
//	IMAGERESULT	    (BaseBitmap::*Save		)(const Filename &name, Int32 format, const BaseContainer *data, SAVEBIT savebits) const;
//	void						(*SetCMAP							)(BaseBitmap *bm, Int32 i, Int32 r, Int32 g, Int32 b);
//	void						(*ScaleIt							)(const BaseBitmap *src, BaseBitmap *dst, Int32 intens, Bool sample, Bool nprop);
//	void						(*SetPen							)(BaseBitmap *bm, Int32 r, Int32 g, Int32 b);
//	void						(*Clear								)(BaseBitmap *bm, Int32 x1, Int32 y1, Int32 x2, Int32 y2, Int32 r, Int32 g, Int32 b);
//	void						(*Line								)(BaseBitmap *bm, Int32 x1, Int32 y1, Int32 x2, Int32 y2);
//	void						(*GetPixel						)(const BaseBitmap *bm, Int32 x, Int32 y, UWORD *r, UWORD *g, UWORD *b);
//	BaseBitmap *		(*AddChannel					)(BaseBitmap *bm, Bool internal, Bool straight);
//	void						(*RemoveChannel				)(BaseBitmap *bm, BaseBitmap *channel);
//	void						(*GetAlphaPixel				)(const BaseBitmap *bm, BaseBitmap *channel, Int32 x, Int32 y, UWORD *val);
//	BaseBitmap *		(*GetInternalChannel	)(BaseBitmap *bm);
//	Int32						(*GetChannelCount			)(const BaseBitmap *bm);
//	BaseBitmap *		(*GetChannelNum				)(BaseBitmap *bm, Int32 num);
//	BaseBitmap*			(*GetClonePart				)(const BaseBitmap *src, Int32 x, Int32 y, Int32 w, Int32 h);
//	Bool						(*CopyTo							)(const BaseBitmap *src, BaseBitmap *dst);
//	void						(*ScaleBicubic				)(const BaseBitmap *src, BaseBitmap *dest, Int32 src_xmin, Int32 src_ymin, Int32 src_xmax, Int32 src_ymax, Int32 dst_xmin, Int32 dst_ymin, Int32 dst_xmax, Int32 dst_ymax);
//	BaseBitmap*			(*GetAlphaBitmap			)(const BaseBitmap *bm, BaseBitmap *channel);
//
//	Bool						(*IsMultipassBitmap		)(const BaseBitmap *bm);
//
//	MultipassBitmap*(*MPB_AllocWrapperPB	)(Int32 bx, Int32 by, COLORMODE mode);
//	MultipassBitmap*(*MPB_AllocWrapper    )(BaseBitmap *bmp);
//	PaintBitmap*		(*MPB_GetPaintBitmap	)(MultipassBitmap *mp);
//	Int32						(*MPB_GetLayerCount		)(const MultipassBitmap *mp);
//	Int32						(*MPB_GetAlphaLayerCount)(const MultipassBitmap *mp);
//	Int32						(*MPB_GetHiddenLayerCount)(const MultipassBitmap *mp);
//	MultipassBitmap*(*MPB_GetLayerNum			)(MultipassBitmap *mp,Int32 num);
//	MultipassBitmap*(*MPB_GetAlphaLayerNum)(MultipassBitmap *mp,Int32 num);
//	MultipassBitmap*(*MPB_GetHiddenLayerNum)(MultipassBitmap *mp,Int32 num);
//	MultipassBitmap*(*MPB_AddLayer				)(MultipassBitmap *mp,MultipassBitmap *insertafter,COLORMODE colormode,Bool hidden);
//	MultipassBitmap*(*MPB_AddFolder				)(MultipassBitmap *mp,MultipassBitmap *insertafter,Bool hidden);
//	MultipassBitmap*(*MPB_AddAlpha				)(MultipassBitmap *mp,MultipassBitmap *insertafter,COLORMODE colormode);
//	Bool						(*MPB_DeleteLayer			)(MultipassBitmap *mp,MultipassBitmap *layer);
//	MultipassBitmap*(*MPB_FindUserID			)(MultipassBitmap *mp,Int32 id,Int32 subid);
//	void						(*MPB_ClearImageData	)(MultipassBitmap *mp);
//	void						(*MPB_SetMasterAlpha	)(MultipassBitmap *mp, BaseBitmap *master);
//	GeData					(*MPB_GetParameter		)(const MultipassBitmap *mp, MPBTYPE id);
//	Bool						(*MPB_SetParameter		)(MultipassBitmap *mp, MPBTYPE id,const GeData &par);
//
//	ULONG						(*GetDirty						)(const BaseBitmap *bm);
//
//	void						(*GetPixelCnt					)(const BaseBitmap *bm, Int32 x, Int32 y, Int32 cnt, UChar *buffer, Int32 inc, COLORMODE dstmode, PIXELCNT flags, ColorProfileConvert *conversion);
//	GeData					(*GetBaseBitmapData		)(const BaseBitmap *bm, Int32 id, const GeData &t_default);
//	Bool						(*SetBaseBitmapData		)(BaseBitmap *bm, Int32 id, const GeData &data);
//
//	void						(*SetDirty						)(BaseBitmap *bm);
//	Bool						(*CopyPartTo					)(const BaseBitmap *src, BaseBitmap *dst, Int32 x, Int32 y, Int32 w, Int32 h);
//
//	BaseBitmapLink *(*BBL_Alloc						)(void);
//	void						(*BBL_Free						)(BaseBitmapLink *link);
//	BaseBitmap		 *(*BBL_Get							)(BaseBitmapLink *link);
//	void						(*BBL_Set							)(BaseBitmapLink *link, BaseBitmap *bmp);
//
//	VLONG           (*GetMemoryInfo  			)(const BaseBitmap *bmp);
//	Int32						(*MPB_GetEnabledLayerCount)(const MultipassBitmap *mp);
//	Bool						(*MPB_GetLayers				)(MultipassBitmap *mp, MPB_GETLAYERS flags, BaseBitmap **&list, Int32 &count);
//	UChar*					(*GetDrawPortBits			)(BaseBitmap *bm);
//	Bool						(*GetUpdateRegions		)(const BaseBitmap *mp, BaseContainer &regions);
//	IMAGERESULT			(*Init1								)(BaseBitmap *bm, Int32 x, Int32 y, Int32 depth, INITBITMAPFLAGS flags);
//	Bool						(*AddUpdateRegion			)(BaseBitmap *bm, Int32 id, Int32 type, Int32 xmin, Int32 xmax, Int32 ymin, Int32 ymax);
//	Bool						(*RemoveUpdateRegion	)(BaseBitmap *bm, Int32 id);
//	BaseBitmap*			(*GetUpdateRegionBitmap)(const BaseBitmap *bm);
//	IMAGERESULT			(*Init3								)(BaseBitmap *&bm, const Filename &name, Int32 frame, Bool *ismovie, BitmapLoaderPlugin **loaderplugin);
//	MultipassBitmap*(*MPB_GetSelectedLayer)(MultipassBitmap *mp);
//	Bool						(*SetPixelCnt					)(BaseBitmap *bm, Int32 x, Int32 y, Int32 cnt, UChar *buffer, Int32 inc, COLORMODE srcmode, PIXELCNT flags);
//	Bool						(*SetPixel						)(BaseBitmap *bm, Int32 x, Int32 y, Int32 r, Int32 g, Int32 b);
//	Bool						(*SetAlphaPixel				)(BaseBitmap *bm, BaseBitmap *channel, Int32 x, Int32 y, Int32 val);
//	void						(*MPB_FreeHiddenLayers)(MultipassBitmap *mp);
//	Bool						(*SetColorProfile			)(BaseBitmap *bm, const ColorProfile *profile);
//	const ColorProfile* (*GetColorProfile	)(const BaseBitmap *bm);
//
//	ColorProfile*		(*ProfileAlloc				)(void);
//	void						(*ProfileFree					)(ColorProfile *profile);
//	Bool						(*ProfileCopy					)(const ColorProfile *src, ColorProfile *dst);
//	Bool						(*ProfileEqual				)(const ColorProfile *src, const ColorProfile *dst);
//	const ColorProfile *(*ProfileSRGB			)();
//	const ColorProfile *(*ProfileLinearRGB)();
//	Bool						(*ProfileWindow				)(ColorProfile *profile, CDialog *dlg);
//	Bool						(*ProfileFromFile			)(ColorProfile *profile, const Filename &fn);
//	Bool						(*ProfileFromMemory		)(ColorProfile *profile, const void *mem, LLONG memsize);
//	Bool						(*ProfileToMemory			)(const ColorProfile *profile, void *&mem, LLONG &memsize);
//	Bool						(*ProfileToFile				)(const ColorProfile *profile, const Filename &fn);
//	String					(*ProfileInfo					)(const ColorProfile *profile, Int32 info);
//	Bool						(*ProfileIsMonitorMode)(const ColorProfile *profile);
//	Bool						(*ProfileHasProfile		)(const ColorProfile *profile);
//	Bool						(*ProfileSetMonitorMode)(ColorProfile *profile, Bool on);
//
//	ColorProfileConvert* (*ProfileConvAlloc)(void);
//	void						(*ProfileConvFree			)(ColorProfileConvert *profile);
//	Bool						(*ProfileConvTransform)(ColorProfileConvert *profile, COLORMODE srccolormode, const ColorProfile *srcprofile, COLORMODE dstcolormode, const ColorProfile *dstprofile, Bool bgr);
//	void						(*ProfileConvConvert	)(const ColorProfileConvert *profile, const PIX *src, PIX *dst, Int32 cnt, Int32 SkipInputComponents, Int32 SkipOutputComponents);
//	Bool						(*ProfileCheckColorMode)(const ColorProfile *profile, COLORMODE colormode);
//	const ColorProfile *(*ProfileSGray		)();
//	const ColorProfile *(*ProfileLinearGray)();
//	Bool						(MultipassBitmap::*SetTempColorProfile)(const ColorProfile *profile, Bool dithering);
//};
//
//struct C4D_MovieSaver
//{
//	MovieSaver*			(*Alloc								)(void);
//	void						(*Free								)(MovieSaver *ms);
//	IMAGERESULT			(*Write								)(MovieSaver *ms, BaseBitmap *bm);
//	void						(*Close								)(MovieSaver *ms);
//	Bool						(*Choose							)(MovieSaver *ms, Int32 id, BaseContainer *bc);
//	IMAGERESULT			(*Open								)(MovieSaver *ms, const Filename *name, BaseBitmap *bm, Int32 fps, Int32 id, const BaseContainer *data, SAVEBIT savebits,BaseSound *sound);
//};
//
//struct C4D_BaseChannel
//{
//	BaseChannel*		(*Alloc								)(void);
//	void						(*Free								)(BaseChannel *bc);
//	Bool						(*Compare							)(BaseChannel *bc1,BaseChannel *bc2);
//	INITRENDERRESULT(*InitTexture					)(BaseChannel *bc, const InitRenderStruct &irs);
//	void						(*FreeTexture					)(BaseChannel *bc);
//	Vector					(*BcSample						)(BaseChannel *bc, VolumeData *vd, Vector *p, Vector *delta, Vector *n, Float t, Int32 tflag, Float off, Float scale);
//	BaseBitmap*			(*BCGetBitmap					)(BaseChannel *bc);
//	void						(*GetData							)(BaseChannel *bc, BaseContainer *ct);
//	void						(*SetData							)(BaseChannel *bc, const BaseContainer *ct);
//	Bool						(*ReadData						)(HyperFile *hf, BaseChannel *bc);
//	Bool						(*WriteData						)(HyperFile *hf, BaseChannel *bc);
//
//	Int32						(*GetPluginID					)(BaseChannel *bc);
//	BaseShader*		(*GetPluginShader			)(BaseChannel *bc);
//
//	Bool						(*Attach							)(BaseChannel *bc, GeListNode *element);
//
//	Bool						(*HandleShaderPopup		 )(const BaseContainer &bc, const DescID &descid, Int32 value, VLONG param);
//	Bool						(*HandleShaderPopupI	 )(BaseList2D *parent, BaseShader *&current, Int32 value, VLONG param);
//	Bool						(*BuildShaderPopupMenu )(BaseContainer *menu, const BaseContainer &bc, const DescID &descid, VLONG param);
//	Bool						(*BuildShaderPopupMenuI)(BaseContainer *menu, BaseList2D *parent, BaseShader *current, VLONG param);
//
//	void						(*HandleShaderMessage )(GeListNode *node, BaseShader *ps, Int32 type, void *data);
//	Bool            (*ReadDataConvert     )(GeListNode *node, Int32 id, HyperFile *hf);
//
//	INITRENDERRESULT(BaseShader::*InitRender		)(const InitRenderStruct &irs);
//	void						(BaseShader::*FreeRender		)(void);
//	Vector					(BaseShader::*Sample				)(ChannelData *vd);
//	Vector					(BaseShader::*SampleBump		)(ChannelData *vd, SAMPLEBUMP bumpflags);
//	BaseBitmap*			(BaseShader::*GetBitmap			)(void);
//	SHADERINFO			(BaseShader::*GetRenderInfo	)(void);
//
//	BaseShader*			(*PsAlloc										)(Int32 type);
//	Bool						(BaseShader::*PsCompare			)(BaseShader* dst);
//
//	String					(*GetChannelName						)(Int32 channelid);
//	Int32						(BaseShader::*GlMessage			)(Int32 type, void *data);
//	Bool						(BaseShader::*IsColorManagementOff)(BaseDocument *doc);
//};
//
//struct C4D_Filename
//{
//	Filename				*Default; // safety value
//
//	Filename*				(*Alloc								)(void);
//	Filename*				(*GetClone						)(const Filename *fn);
//	void						(*Free								)(Filename *fn);
//	Bool						(Filename::*FileSelect)(FILESELECTTYPE type, FILESELECT flags, const String &title, const String &force_suffix);
//	Bool						(*Content							)(const Filename *fn);
//	const String		(*GetString						)(const Filename *fn);
//	void						(*SetString						)(Filename *fn, const String *str);
//	const Filename	(*GetDirectory				)(const Filename *fn);
//	const Filename	(*GetFile							)(const Filename *fn);
//	void						(*ClearSuffix					)(Filename *fn);
//	void						(*SetSuffix						)(Filename *fn, const String *str);
//	Bool						(*CheckSuffix					)(const Filename *fn, const String *str);
//	void						(*SetDirectory				)(Filename *fn, const Filename *str);
//	void						(*SetFile							)(Filename *fn, const Filename *str);
//	Bool						(*Compare							)(const Filename *fn1, const Filename *fn2);
//	void						(*Add									)(Filename *dst, const Filename *src);
//
//	void						(*Init								)(Filename *fn);
//	void						(*Flush								)(Filename *fn);
//	void						(*CopyTo							)(const Filename *src, Filename *dst);
//	void						(*SetMemoryReadMode		)(Filename *fn, void *adr, VLONG size);
//	void						(*SetMemoryWriteMode	)(Filename *fn, MemoryFileStruct *mfs);
//
//	MemoryFileStruct* (*MemoryFileStructAlloc)();
//	void						(*MemoryFileStructFree)(MemoryFileStruct *&mfs);
//	void						(*MemoryFileStructGetData)(MemoryFileStruct *mfs, void *&data, VLONG &size, Bool release);
//	void						(*SetCString					)(Filename *fn, const Char *str);
//	void						(*ClearSuffixComplete	)(Filename *fn);
//
//	void						(Filename::*SetIpConnection)(IpConnection *ipc);
//	IpConnection*		(Filename::*GetIpConnection)() const;
//};
//
//struct C4D_BrowseFiles
//{
//	BrowseFiles*		(*Alloc								)(const Filename *dir, Int32 flags);
//	void						(*Free								)(BrowseFiles *bf);
//	void						(*Init								)(BrowseFiles *bf, const Filename *dir, Int32 flags);
//
//	Bool						(BrowseFiles::*GetNext				)(void);
//	Filename				(BrowseFiles::*GetFilename		)(void);
//	Bool						(BrowseFiles::*IsDir					)(void);
//	LLONG						(BrowseFiles::*GetSize				)(void);
//	void						(BrowseFiles::*GetFileTime		)(Int32 mode, LocalFileTime *out);
//	Bool						(BrowseFiles::*IsHidden				)(void);
//	Bool						(BrowseFiles::*IsReadOnly			)(void);
//	Bool						(BrowseFiles::*IsBundle				)(void);
//
//	BrowseVolumes*	(*BvAlloc							)(void);
//	void						(*BvFree							)(BrowseVolumes *bv);
//	void						(BrowseVolumes::*BvInit				)(void);
//	Bool						(BrowseVolumes::*BvGetNext		)(void);
//	Filename				(BrowseVolumes::*BvGetFilename)(void);
//	String					(BrowseVolumes::*BvGetVolumeName)( Int32 *out_flags );
//};
//
//struct C4D_File
//{
//	BaseFile*				(*Alloc								)(void);
//	void						(*Free								)(BaseFile *fl);
//	AESFile*				(*AESAlloc						)(void);
//	Bool						(*AESCheckEncryption	)(const Filename& encrypt, const Filename& decrypt, const char* key, Int32 keylen, Int32 blocksize);
//
//	Bool						(BaseFile::*Open							)(const Filename &name, FILEOPEN mode, FILEDIALOG error_dialog, BYTEORDER order, Int32 type, Int32 creator);
//	Bool						(BaseFile::*Close							)();
//	void						(BaseFile::*SetOrder					)(BYTEORDER order);
//	VLONG						(BaseFile::*ReadBytes					)(void *data, VLONG len, Bool just_try_it);
//	Bool						(BaseFile::*WriteBytes				)(const void *data, VLONG len);
//	Bool						(BaseFile::*Seek							)(LLONG pos, FILESEEK mode);
//	LLONG						(BaseFile::*GetPosition				)();
//	LLONG						(BaseFile::*GetLength					)();
//	FILEERROR				(BaseFile::*GetError					)() const;
//	void						(BaseFile::*SetError					)(FILEERROR error);
//	Bool						(BaseFile::*WriteChar					)(Char  v);
//	Bool						(BaseFile::*WriteUChar				)(UChar v);
//	Bool						(BaseFile::*WriteWord					)(SWORD  v);
//	Bool						(BaseFile::*WriteUWord				)(UWORD v);
//	Bool						(BaseFile::*WriteLong					)(Int32  v);
//	Bool						(BaseFile::*WriteULong				)(ULONG v);
//	Bool						(BaseFile::*WriteSReal				)(Float32 v);
//	Bool						(BaseFile::*WriteLReal				)(LReal v);
//	Bool						(BaseFile::*ReadChar					)(Char  *v);
//	Bool						(BaseFile::*ReadUChar					)(UChar *v);
//	Bool						(BaseFile::*ReadWord					)(SWORD  *v);
//	Bool						(BaseFile::*ReadUWord					)(UWORD *v);
//	Bool						(BaseFile::*ReadLong					)(Int32  *v);
//	Bool						(BaseFile::*ReadULong					)(ULONG *v);
//	Bool						(BaseFile::*ReadSReal					)(Float32 *v);
//	Bool						(BaseFile::*ReadLReal					)(LReal *v);
//	Bool						(BaseFile::*ReadLLong					)(LLONG *v);
//	Bool						(BaseFile::*WriteLLong				)(LLONG v);
//
//	Bool						(AESFile::*AESOpen						)(const Filename &name, const char* key, Int32 keylen, Int32 blocksize, ULONG aes_flags, FILEOPEN mode, FILEDIALOG error_dialog, BYTEORDER order, Int32 type, Int32 creator);
//};
//
//struct C4D_Dialog
//{
//	CDialog*				(*Alloc									)(CDialogMessage *dlgfunc,void *userdata);
//	void						(*Free									)(CDialog *cd);
//	void*						(*GetUserData						)(CDialog *cd);
//	Bool						(*Close									)(CDialog *cd);
//	Bool						(*Enable								)(CDialog *cd, Int32 id, Bool enabled,void *gadptr);
//	void						(*SetTimer							)(CDialog *cd, Int32 timer);
//	Bool						(*SetLong								)(CDialog *cd, Int32 id, Int32 value,Int32 min,Int32 max,Int32 step,void *gadptr);
//	Bool						(*SetReal								)(CDialog *cd, Int32 id, Float value,Float min,Float max,Float step,Int32 format,void *gadptr);
//	Bool						(*SetVector							)(CDialog *cd, Int32 id, const Vector &value,void *gadptr);
//	Bool						(*SetString							)(CDialog *cd, Int32 id, const String *text,void *gadptr);
//	Bool						(*SetColorField					)(CDialog *cd, Int32 id, const Vector &color, Float brightness,Float maxbrightness,Int32 flags,void *gadptr);
//	Bool						(*GetLong								)(CDialog *cd, Int32 id, Int32 &value,void *gadptr);
//	Bool						(*GetReal								)(CDialog *cd, Int32 id, Float &value,void *gadptr);
//	Bool						(*GetVector							)(CDialog *cd, Int32 id, Vector &value,void *gadptr);
//	Bool						(*GetString							)(CDialog *cd, Int32 id, String *&text,void *gadptr);
//	Bool						(*GetColorField					)(CDialog *cd, Int32 id, Vector &color, Float &brightness,void *gadptr);
//	Bool						(*LoadDialogResource		)(CDialog *cd, Int32 id, LocalResource *lr, Int32 flags);
//	Bool						(*TabGroupBegin					)(CDialog *cd, Int32 id, Int32 flags,Int32 tabtype);
//	Bool						(*GroupBegin						)(CDialog *cd, Int32 id, Int32 flags,Int32 cols,Int32 rows,const String *title,Int32 groupflags);
//	Bool						(*GroupSpace						)(CDialog *cd, Int32 spacex,Int32 spacey);
//	Bool						(*GroupBorder						)(CDialog *cd, ULONG borderstyle);
//	Bool						(*GroupBorderSize				)(CDialog *cd, Int32 left, Int32 top,Int32 right,Int32 bottom);
//	Bool						(*GroupEnd							)(CDialog *cd);
//	Bool						(*SetPopup							)(CDialog *cd, Int32 id, const BaseContainer *bc,void *gadptr);
//	Bool						(*Screen2Local					)(CDialog *cd, Int32 *x, Int32 *y);
//	Bool						(*SetVisibleArea				)(CDialog *cd, Int32 scrollgroupid, Int32 x1, Int32 y1,Int32 x2,Int32 y2);
//	Bool						(*GetItemDim						)(CDialog *cd, Int32 id, Int32 *x, Int32 *y, Int32 *w, Int32 *h,void *gadptr);
//	Bool						(*SendRedrawThread			)(CDialog *cd, Int32 id);
//	Bool            (*GetVisibleArea				)(CDialog *cd, Int32 id, Int32 *x1,Int32 *y1,Int32 *x2,Int32 *y2);
//	Bool						(*RestoreLayout					)(CDialog *cd, void *secret);
//	Bool						(*SetMessageResult			)(CDialog *cd, const BaseContainer *result);
//
//	Bool            (*SetDragDestination		)(CDialog *cd, Int32 cursor);
//	Bool						(*AttachSubDialog				)(CDialog *parentcd,Int32 id,CDialog *cd);
//	Int32						(*GetID									)(CDialog *cu);
//	void*						(*FindCustomGui					)(CDialog *cd,Int32 id);
//	Bool						(*AddGadget							)(CDialog *cd, Int32 type, Int32 id, const String *name,Int32 par1,Int32 par2, Int32 par3, Int32 par4,const BaseContainer *customdata,void **resptr);
//	Bool						(*ReleaseLink						)(CDialog *cd);
//	Bool						(*SendParentMessage			)(CDialog *cd,const BaseContainer *msg);
//
//	Bool						(*Open									)(CDialog *cd, DLG_TYPE dlgtype, CDialog *parent, Int32 xpos, Int32 ypos,Int32 defaultw,Int32 defaulth);
//	CUserArea*			(*AttachUserArea				)(CDialog *cd, Int32 id,void *userdata,Int32 userareaflags,void *gadptr);
//	Bool            (*GetDragObject					)(CDialog *cd, const BaseContainer *msg,Int32 *type,void **object);
//
//	LassoSelection*	(*LSAlloc								)(void);
//	void						(*LSFree								)(LassoSelection *ls);
//	Int32						(*LSGetMode							)(LassoSelection *ls);
//	Bool						(*LSTest								)(LassoSelection *ls, Int32 x, Int32 y);
//	Bool						(*LSCheckSingleClick		)(LassoSelection *ls);
//	Bool						(*LSStart								)(LassoSelection *ls, CBaseFrame *cd, Int32 mode, Int32 start_x, Int32 start_y, Int32 start_button,Int32 sx1, Int32 sy1, Int32 sx2, Int32 sy2);
//	Bool						(*LSTestPolygon					)(LassoSelection *ls, const Vector &pa, const Vector &pb, const Vector &pc, const Vector &pd);
//
//	CBaseFrame*			(*CBF_FindBaseFrame			)(CDialog *cd,Int32 id);
//	Bool						(*CBF_SetDragDestination)(CBaseFrame *cbf,Int32 cursor);
//	void*						(*CBF_GetWindowHandle		)(CBaseFrame *cbf);
//	GeData					(*SendUserAreaMessage		)(CDialog *cd, Int32 id, BaseContainer *msg,void *gadptr);
//	Bool						(*LSGetRectangle				)(LassoSelection *ls,Float &x1, Float &y1, Float &x2, Float &y2);
//	Bool						(*CBF_GetColorRGB				)(CBaseFrame *cbf,Int32 colorid, Int32 &r, Int32 &g, Int32 &b);
//	Bool						(*RemoveLastCursorInfo	)(LASTCURSORINFOFUNC func);
//	String					(*Shortcut2String				)(Int32 shortqual,Int32 shortkey);
//
//	Bool						(*GetIconCoordInfo			)(Int32 &id, const Char* ident);
//	Bool						(*GetInterfaceIcon			)(Int32 type, Int32 id_x, Int32 id_y, Int32 id_w, Int32 id_h, IconData &d);
//	Bool						(*IsEnabled							)(CDialog *cd, Int32 id, void *gadptr);
//};
//
//struct C4D_UserArea
//{
//	void						(*Free                )(CUserArea* cu);
//	void*						(*GetUserData         )(CUserArea *cu);
//	Int32						(*GetWidth            )(CUserArea *cu);
//	Int32						(*GetHeight           )(CUserArea *cu);
//	Int32						(*GetID               )(CUserArea *cu);
//	void						(*SetMinSize          )(CUserArea *cu, Int32 w,Int32 h);
//	void						(*DrawLine            )(CUserArea *cu, Int32 x1,Int32 y1,Int32 x2,Int32 y2);
//	void						(*DrawRectangle       )(CUserArea *cu, Int32 x1,Int32 y1,Int32 x2,Int32 y2);
//	void						(*DrawSetPenV         )(CUserArea *cu, const Vector &color);
//	void						(*DrawSetPenI         )(CUserArea *cu, Int32 id);
//	void						(*SetTimer            )(CUserArea *cu, Int32 timer);
//	Bool						(*GetInputState       )(CBaseFrame *cu, Int32 askdevice,Int32 askchannel,BaseContainer *res);
//	Bool						(*GetInputEvent       )(CBaseFrame *cu, Int32 askdevice,BaseContainer *res);
//	void						(*KillEvents          )(CBaseFrame *cu);
//	void						(*DrawSetFont         )(CUserArea *cu, Int32 fontid);
//	Int32						(*DrawGetTextWidth    )(CUserArea *cu, const String *text);
//	Int32						(*DrawGetFontHeight   )(CUserArea *cu);
//	void						(*DrawSetTextColII    )(CUserArea *cu, Int32 fg,Int32 bg);
//	void						(*DrawSetTextColVI    )(CUserArea *cu, const Vector &fg,Int32 bg);
//	void						(*DrawSetTextColIV    )(CUserArea *cu, Int32 fg,const Vector &bg);
//	void						(*DrawSetTextColVV    )(CUserArea *cu, const Vector &fg,const Vector &bg);
//	void						(*DrawBitmap          )(CUserArea *cu, BaseBitmap *bmp,Int32 wx,Int32 wy,Int32 ww,Int32 wh,Int32 x,Int32 y,Int32 w,Int32 h,Int32 mode);
//	void						(*SetClippingRegion   )(CUserArea *cu, Int32 x,Int32 y,Int32 w,Int32 h);
//	void						(*ScrollArea          )(CUserArea *cu, Int32 xdiff,Int32 ydiff,Int32 x,Int32 y,Int32 w,Int32 h);
//	void						(*ClearClippingRegion )(CUserArea *cu);
//	Bool						(*OffScreenOn         )(CUserArea *cu);
//
//	Bool						(*Global2Local        )(CBaseFrame *cu, Int32 *x,Int32 *y);
//	Bool						(*SendParentMessage   )(CUserArea *cu, const BaseContainer *msg);
//
//	Bool						(*Screen2Local        )(CBaseFrame *cu, Int32 *x, Int32 *y);
//	Bool            (*SetDragDestination  )(CUserArea *cu, Int32 cursor);
//	Bool            (*HandleMouseDrag     )(CUserArea *cu, const BaseContainer *msg,Int32 type,void *data,Int32 dragflags);
//	Bool						(*IsEnabled           )(CUserArea *cu);
//
//	void						(*GetBorderSize       )(CUserArea *cu,Int32 type,Int32 *l,Int32 *t,Int32 *r,Int32 *b);
//	void            (*DrawBorder          )(CUserArea *cu,Int32 type,Int32 x1,Int32 y1,Int32 x2,Int32 y2);
//
//	_GeListView*		(*GeListView_Alloc            )(void);
//	void						(*GeListView_Free             )(_GeListView *lv);
//	Bool            (*GeListView_Attach           )(_GeListView *lv,CDialog *cd,Int32 id,ListViewCallBack *callback,void *userdata);
//	void            (*GeListView_LvSuperCall      )(_GeListView *lv,Int32 &res_type,void *&result,void *secret,Int32 cmd,Int32 line,Int32 col);
//	void            (*GeListView_Redraw           )(_GeListView *lv);
//	void            (*GeListView_DataChanged      )(_GeListView *lv);
//	Bool            (*GeListView_ExtractMouseInfo )(_GeListView *lv,void *secret,MouseDownInfo &info,Int32 size);
//	Bool            (*GeListView_ExtractDrawInfo  )(_GeListView *lv,void *secret,DrawInfo &info,Int32 size);
//	Bool            (*GeListView_SendParentMessage)(_GeListView *lv,const BaseContainer *msg);
//	Int32						(*GeListView_GetId            )(_GeListView *lv);
//	void						(*GeListView_ShowCell					)(_GeListView *lv,Int32 line,Int32 col);
//
//	_SimpleListView* (*SimpleListView_Alloc       )(void);
//	void            (*SimpleListView_Free         )(_SimpleListView *lv);
//	Bool            (*SimpleListView_SetLayout    )(_SimpleListView *lv,Int32 columns,const BaseContainer &data);
//	Bool            (*SimpleListView_SetItem      )(_SimpleListView *lv,Int32 id,const BaseContainer &data);
//	Bool            (*SimpleListView_GetItem      )(_SimpleListView *lv,Int32 id,BaseContainer *data);
//	Int32            (*SimpleListView_GetItemCount )(_SimpleListView *lv);
//	Bool            (*SimpleListView_GetItemLine  )(_SimpleListView *lv,Int32 num,Int32 *id,BaseContainer *data);
//	Bool            (*SimpleListView_RemoveItem   )(_SimpleListView *lv,Int32 id);
//	Int32            (*SimpleListView_GetSelection )(_SimpleListView *lv,BaseSelect *selection);
//	Bool            (*SimpleListView_SetSelection )(_SimpleListView *lv,BaseSelect *selection);
//	void            (*SimpleListView_ShowCell			)(_SimpleListView *lv,Int32 line, Int32 col);
//
//	Int32            (*SimpleListView_GetProperty  )(_SimpleListView *lv,Int32 id);
//	Bool            (*SimpleListView_SetProperty  )(_SimpleListView *lv,Int32 id,Int32 val);
//
//	Bool						(*IsHotkeyDown                )(CUserArea *cu, Int32 id);
//	Bool						(*HasFocus										)(CUserArea *cu);
//
//	void						(*MouseDragStart							)(CUserArea *cu,Int32 Button,Float mx,Float my,MOUSEDRAGFLAGS flag);
//	MOUSEDRAGRESULT (*MouseDrag										)(CUserArea *cu,Float *mx,Float *my,BaseContainer *channels);
//	MOUSEDRAGRESULT (*MouseDragEnd								)(CUserArea *cu);
//	Int32						(*DrawGetTextWidth_ListNodeName)(CUserArea *cu,BaseList2D *node, Int32 fontid);
//	Bool						(*OffScreenOnRect							)(CUserArea *cu, Int32 x, Int32 y, Int32 w, Int32 h);
//	void						(*DrawText										)(CUserArea *cu, const String &txt,Int32 x,Int32 y,Int32 flags);
//};
//
//struct C4D_Parser
//{
//	Parser*					(*Alloc								)(void);
//	void						(*Free								)(Parser *pr);
//	Bool						(*Eval								)(Parser *pr, const String *str, Int32 *error,Float *res,Int32 unit,Int32 angletype,Int32 basis);
//	Bool						(*EvalLong						)(Parser *pr, const String *str, Int32 *error,Int32 *res,Int32 unit,Int32 basis);
//	Bool						(*AddVar							)(Parser *pr, const String *str, Float *value, Bool case_sensitive);
//	Bool						(*AddVarLong					)(Parser *pr, const String *str, Int32 *value, Bool case_sensitive);
//	Bool						(*RemoveVar						)(Parser *pr, const String *s, Bool case_sensitive);
//	Bool						(*RemoveAllVars				)(Parser *pr);
//	void						(*GetParserData				)(Parser *pr, ParserCache *p);
//	Bool						(*Init								)(Parser *pr, const String *s, Int32 *error, Int32 unit, Int32 angle_unit, Int32 base);
//	Bool						(*ReEval							)(Parser *pr, Float *result, Int32 *error);
//	Bool						(*Calculate						)(Parser *pr, const ParserCache *pdat, Float *result, Int32 *error);
//	Bool						(*ReEvalLong					)(Parser *pr, Int32 *result, Int32 *error);
//	Bool						(*CalculateLong				)(Parser *pr, const ParserCache *pdat, Int32 *result, Int32 *error);
//	Bool						(*Reset								)(Parser *pr, ParserCache *p);
//	ParserCache*		(*AllocPCache					)(void);
//	void						(*FreePCache					)(ParserCache *p);
//	Bool						(*CopyPCache					)(ParserCache *p, ParserCache *d);
//};
//
//struct C4D_Resource
//{
//	LocalResource*	(*Alloc								)(const Filename *path);
//	void						(*Free                )(LocalResource *lr);
//	LocalResource*  (*GetCinemaResource   )(void);
//	const String&		(*LoadString          )(LocalResource *lr,Int32 id);
//
//	BaseContainer*	(*GetMenuResource			)(const String &menuname);
//	void						(*UpdateMenus					)(void);
//	Bool						(*ReloadResource			)(LocalResource *lr,const Filename *path);
//};
//
//struct C4D_Atom
//{
//	C4DAtom*				(C4DAtom::*GetClone							)(COPYFLAGS flags, AliasTrans *trn);
//	Bool						(C4DAtom::*CopyTo								)(C4DAtom *dst, COPYFLAGS flags, AliasTrans *trn);
//	Int32						(C4DAtom::*GetType							)(void) const;
//	Bool						(C4DAtom::*IsInstanceOf					)(Int32 id) const;
//	Bool						(C4DAtom::*Message							)(Int32 type, void *data);
//	Bool						(C4DAtom::*MultiMessage					)(MULTIMSG_ROUTE flags, Int32 type, void *data);
//	Bool            (C4DAtom::*GetDescription				)(Description &res,DESCFLAGS_DESC flags);
//	Bool            (C4DAtom::*GetParameter					)(const DescID &id,GeData &t_data,DESCFLAGS_GET flags);
//	Bool            (C4DAtom::*SetParameter					)(const DescID &id,const GeData &t_data,DESCFLAGS_SET flags);
//	DynamicDescription*		(C4DAtom::*GetDynamicDescription)();
//
//	GeListNode*			(GeListNode::*GetNext					)(void) const;
//	GeListNode*			(GeListNode::*GetPred					)(void) const;
//	GeListNode*			(GeListNode::*GetUp						)(void) const;
//	GeListNode*			(GeListNode::*GetDown					)(void) const;
//	GeListNode*			(GeListNode::*GetDownLast			)(void) const;
//	void						(GeListNode::*InsertBefore		)(GeListNode *bl);
//	void						(GeListNode::*InsertAfter			)(GeListNode *bl);
//	void						(GeListNode::*InsertUnder			)(GeListNode *bl);
//	void						(GeListNode::*InsertUnderLast	)(GeListNode *bl);
//	void						(GeListNode::*Remove					)(void);
//	GeListHead*			(GeListNode::*GetListHead			)(void);
//	BaseDocument*		(GeListNode::*GetDocument			)(void) const;
//
//	GeListNode*			(GeListHead::*GetFirst				)(void) const;
//	GeListNode*			(GeListHead::*GetLast					)(void) const;
//	void						(GeListHead::*FlushAll				)(void);
//	void						(GeListHead::*InsertFirst			)(GeListNode *bn);
//	void						(GeListHead::*InsertLast			)(GeListNode *bn);
//	void						(GeListHead::*SetParent				)(GeListNode *parent);
//	GeListNode*			(GeListHead::*GetParent				)(void) const;
//
//	BaseList2D*			(BaseList2D::*GetMain					)(void) const;
//	const String&   (BaseList2D::*GetName					)(void) const;
//	void						(BaseList2D::*SetName					)(const String &str);
//	Bool						(BaseList2D::*GetAnimatedParameter)(const DescID &id,GeData &t_data1,GeData &t_data2,Float &mix,DESCFLAGS_GET flags);
//	Bool						(BaseList2D::*Scale						)(Float scale);
//
//	// AtomArray
//	AtomArray*			(*AtomArrayAlloc							)();
//	void						(*AtomArrayFree								)(AtomArray *&obj);
//	Int32						(AtomArray::*GetCount					)() const;
//	C4DAtom *				(AtomArray::*GetIndex					)(Int32 idx) const;
//	Bool						(AtomArray::*Append						)(C4DAtom *obj);
//	void						(AtomArray::*Flush						)();
//	Bool						(AtomArray::*AACopyTo					)(AtomArray *dest) const;
//
//	Int32						(AtomArray::*AAGetUserID			)() const;
//	void						(AtomArray::*AASetUserID			)(Int32 t_userid);
//
//	void*						(AtomArray::*AAGetUserData		)() const;
//	void						(AtomArray::*AASetUserData		)(void *t_userdata);
//
//	C4DAtom*				(AtomArray::*AAGetPreferred		)() const;
//	void						(AtomArray::*AASetPreferred		)(C4DAtom *t_preferred);
//
//	BaseShader*			(BaseList2D::*GetFirstShader  )() const;
//	void						(BaseList2D::*InsertShader    )(BaseShader *shader, BaseShader *pred);
//
//	Bool						(BaseList2D::*SetAnimatedParameter)(CTrack *track, const DescID &id,const GeData &t_data1,const GeData &t_data2,Float mix,DESCFLAGS_SET flags);
//	void            (AtomArray::*AAFilterObject   )(Int32 type, Int32 instance, Bool generators);
//	Bool						(AtomArray::*AACopyToFilter	  )(AtomArray *dest, Int32 type, Int32 instance, Bool generators) const;
//	Bool						(AtomArray::*AAAppendArr  	  )(AtomArray *src);
//	void						(AtomArray::*AAFilterObjectChildren)();
//	Bool						(AtomArray::*AARemove					)(C4DAtom *obj);
//
//	Bool            (C4DAtom::*GetEnabling						)(const DescID &id,const GeData &t_data,DESCFLAGS_ENABLE flags,const BaseContainer *itemdesc);
//	Int32						(AtomArray::*AAGetCountTI			)(Int32 type, Int32 instance) const;
//
//	Bool						(GeListNode::*IsDocumentRelated)(void) const;
//	Int32						(AtomArray::*AAFind						)(C4DAtom *obj);
//	Bool            (GeListNode::*GetNBit					)(NBIT bit) const;
//	Bool            (GeListNode::*ChangeNBit      )(NBIT bit, NBITCONTROL bitmode);
//	Int32						(GeListNode::*GetBranchInfo   )(BranchInfo *info, Int32 max, GETBRANCHINFO flags);
//	Int32						(C4DAtom::*GetClassification	)(void) const;
//	Bool						(BaseList2D::*TransferMarker	)(BaseList2D *dst) const;
//	const GeMarker&	(BaseList2D::*GetMarker				)(void) const;
//	void						(BaseList2D::*SetMarker				)(const GeMarker &m);
//	GeMarker*				(*GeMarkerAlloc								)();
//	void						(*GeMarkerFree								)(GeMarker *&obj);
//	Bool						(GeMarker::*IsEqual						)(const GeMarker &m) const;
//	Bool						(GeMarker::*Content						)() const;
//	Int32						(C4DAtom::*GetRealType				)(void) const;
//	Int32						(GeMarker::*Compare						)(const GeMarker &m) const;
//	void						(GeMarker::*GeMarkerSet				)(const GeMarker &m);
//	Bool						(GeMarker::*GeMarkerRead			)(HyperFile *hf);
//	Bool						(GeMarker::*GeMarkerWrite			)(HyperFile *hf) const;
//	Bool						(BaseList2D::*TransferGoal		)(BaseList2D *dst, Bool undolink);
//	void						(GeMarker::*GeMarkerGetMemory )(void *&data, Int32 &size) const;
//	Bool						(BaseList2D::*AddUniqueID     )(Int32 appid, const Char *const mem, VLONG bytes);
//	Bool						(BaseList2D::*FindUniqueID    )(Int32 appid, const Char *&mem, VLONG &bytes) const;
//	Int32						(BaseList2D::*GetUniqueIDCount)() const;
//	Bool						(BaseList2D::*GetUniqueIDIndex)(Int32 idx, Int32 &id, const Char *&mem, VLONG &bytes) const;
//	Bool						(C4DAtom::*TranslateDescID	)(const DescID &id, DescID &res_id, C4DAtom *&res_at);
//};
//
//struct C4D_Coffee
//{
//	Coffee*					(*GeCoffeeAlloc								)();
//	void						(*GeCoffeeFree								)(Coffee* &cof);
//	Bool						(*GeCoffeeCompileString				)(Coffee* cof, const String& src);
//	Bool						(*GeCoffeeCompileFile					)(Coffee* cof, const Filename& file);
//	VALUE*					(*GeCoffeeAddGlobalSymbol			)(Coffee* cof, const String& name);
//	VALUE*					(*GeCoffeeFindGlobalSymbol		)(Coffee* cof, const String& name);
//	Bool						(*GeCoffeeExecute							)(Coffee* cof, VALUE* func, GeData* retval, GeData* arg1, GeData* arg2, GeData* arg3);
//	Bool						(*GeCoffeeGeData2Value				)(Coffee* cof, const GeData& src, VALUE* dest);
//	Bool						(*GeCoffeeValue2GeData				)(Coffee* cof, VALUE *src, GeData* dest);
//	Bool						(*GeCoffeeGetLastError				)(Coffee* cof, String *err_string, Int32 *err_line, Int32 *err_pos);
//	Bool						(*CoffeeEditor_Open						)(BaseList2D *obj,CoffeeEditorCallback *callback);
//
//	VALUE*					(*CoValGetObjMember						)(VALUE *val, Int32 nr);
//	VALUE*					(*CoValGetArrayMember					)(VALUE *val, Int32 i);
//	void						(*CoValSetArray								)(VALUE *val, ARRAY *a);
//	OBJECT*					(*CoValGetObject							)(VALUE *val, Int32 *err);
//	void						(*CoValSetObject							)(VALUE *val, OBJECT *o);
//	void						(*CoValSetString							)(VALUE *val, CSTRING *s);
//	String					(*CoValGetString							)(VALUE *val);
//	Bool						(*CoValIsInstanceOf						)(VALUE *val, VALUE *cl, Int32 *err);
//	Int32						(*CoValGetSize								)(VALUE *val);
//	UChar*					(*CoValGetBytes								)(VALUE *val);
//
//	VALUE*					(*CoGetGlobalClass						)(Coffee* cof, const String &name);
//	CLASS*					(*CoAddGlobalClass						)(Coffee* cof, const String &name, const String &parent);
//	Bool						(*CoAddGlobalSymbol						)(Coffee* cof, const String &name, const VALUE *v, Int32 type);
//	Bool						(*CoAddGlobalFunction					)(Coffee* cof, const String &name, V_CODE fcn);
//	Bool						(*CoAddClassMember						)(Coffee* cof, const String &name, CLASS *c, Int32 type);
//	Bool						(*CoAddClassMethod						)(Coffee* cof, const String &name, CLASS *c, Int32 type, V_CODE fcn, Int32 argc);
//	CSTRING*				(*CoAllocString								)(Coffee* cof, const String &s);
//	OBJECT*					(*CoNewObject									)(Coffee* cof, const String &cl_name);
//	ARRAY*					(*CoNewArray									)(Coffee* cof, Int32 size);
//	void						(*CoWrongcnt									)(Coffee* cof, Int32 n, Int32 cnt);
//	void						(*CoErrCheckType							)(Coffee* cof, VALUE *v, Int32 type, Int32 *err);
//	void						(*CoErrCheckObjectType				)(Coffee* cof, VALUE *v, const String &cl_name, Int32 *err);
//	void						(*CoErrCheckArgCount					)(Coffee* cof, Int32 argc, Int32 cnt, Int32 *err);
//	Int32						(*CoGetType										)(Coffee* cof);
//	const Filename&	(*CoGetRootFile								)(Coffee* cof);
//	OBJECT*					(*CoAllocDynamic							)(Coffee* cof, BaseList2D *bl, Bool coffeeallocation);
//	CLASS*					(*CoAddInheritance						)(Coffee* cof, Int32 id, const String &name, const String &from, Bool use_constructor);
//	Coffee*					(*CoGetMaster									)(void);
//	void						(*CoSetError									)(Coffee* cof, Int32 type, const String &s1, const String &s2);
//	void						(*CoInstallErrorHook					)(Coffee* cof, COFFEE_ERRORHANDLER *priv_hndl, void *priv_data);
//	void						(*CoSetRootFile								)(Coffee* cof, const Filename &fn);
//};
//
//struct C4D_BaseList
//{
//	Int32						(*GetDiskType					)(const C4DAtom *at);
//	void						(*GetMarker						)(BaseList2D *bl, ULONG *l1, ULONG *l2);
//	void						(*SetAllBits					)(BaseList2D *bl, Int32 mask);
//	Int32						(*GetAllBits					)(BaseList2D *bl);
//	void						(*Free								)(C4DAtom *at);
//	Bool						(*Read								)(C4DAtom *at, HyperFile *hf, Int32 id, Int32 level);
//	Bool						(*Write								)(C4DAtom *at, HyperFile *hf);
//	Bool						(*ReadObject					)(C4DAtom *bn, HyperFile *hf, Bool readheader);
//	Bool						(*WriteObject					)(C4DAtom *bn, HyperFile *hf);
//	void						(*GetData							)(BaseList2D *bl, BaseContainer *ct);
//	void						(*SetData							)(BaseList2D *bl, const BaseContainer *ct, Bool add);
//	BaseContainer*	(*GetDataInstance			)(BaseList2D *bl);
//
//	GeListHead*			(*AllocListHead				)(void);
//	GeListNode*			(*AllocListNode				)(Int32 bits, Int32 *id_array, Int32 id_cnt);
//
//	NodeData*				(*GetNodeData					)(GeListNode *bn, Int32 index);
//	Int32						(*GetNodeID						)(GeListNode *bn, Int32 index);
//	NODEPLUGIN*			(*RetrieveTable				)(GeListNode *node, Int32 index);
//	NODEPLUGIN*			(*RetrieveTableX			)(NodeData *node, Int32 index);
//
//	GeListNode*			(*GetCustomData				)(GeListNode *bn);
//	void						(*SetCustomData				)(GeListNode *bn, GeListNode *custom);
//	String					(*GetBubbleHelp				)(BaseList2D *bl);
//
//	void						(BaseList2D::*ClearKeyframeSelection  )();
//	Bool						(BaseList2D::*FindKeyframeSelection   )(const DescID &id);
//	Bool						(BaseList2D::*SetKeyframeSelection    )(const DescID &id, Bool selection);
//	Bool						(BaseList2D::*KeyframeSelectionContent)();
//
//	// layer
//	LayerObject*     (BaseList2D::*GetLayerObject         )(BaseDocument *doc);
//	Bool             (BaseList2D::*SetLayerObject         )(LayerObject *layer);
//	const LayerData* (BaseList2D::*GetLayerData           )(BaseDocument *doc, Bool rawdata) const;
//	Bool             (BaseList2D::*SetLayerData           )(BaseDocument *doc, const LayerData &data);
//
//	// animation system
//	GeListHead*      (BaseList2D::*GetCTrackRoot )();
//	CTrack*	         (BaseList2D::*GetFirstCTrack)(void);
//	CTrack*	         (BaseList2D::*FindCTrack    )(const DescID &id);
//
//	const String &   (BaseList2D::*GetTypeName   )(void);
//
//	void             (BaseList2D::*InsertTrackSorted)(CTrack *track);
//	BaseList2D*			 (*Alloc								        )(Int32 type);
//
//	// nla system
//	GeListHead*      (BaseList2D::*GetNLARoot         )();
//	BaseList2D*      (BaseList2D::*AnimationLayerRemap)(BaseObject **layer);
//
//	Bool (BaseList2D::*AddEventNotification)(BaseList2D *bl, NOTIFY_EVENT eventid, NOTIFY_EVENT_FLAG flags, const BaseContainer *data);
//	Bool (BaseList2D::*RemoveEventNotification)(BaseDocument *doc, BaseList2D *bl, NOTIFY_EVENT eventid);
//	Bool (BaseList2D::*FindEventNotification)(BaseDocument *doc, BaseList2D *bl, NOTIFY_EVENT eventid);
//};
//
//struct C4D_Tag
//{
//	BaseTag*				(*Alloc								)(Int32 type, Int32 count);
//	Int32						(*GetDataCount				)(VariableTag *bt);
//	Int32						(*GetDataSize					)(VariableTag *bt);
//	void*						(*GetDataAddressW			)(VariableTag *bt);
//	BaseSelect*			(*GetBaseSelect				)(SelectionTag *tag);
//	Bool						(*Record							)(StickTextureTag *stt, BaseObject *op, Bool always);
//
//	// UVWs
//	void						(*UvGet								)(UVWTag *tag, Int32 i, UVWStruct *s);
//	void						(*UvSet								)(UVWTag *tag, Int32 i, UVWStruct *s);
//	void						(*UvCpy								)(UVWTag *tag, Int32 dst, UVWTag *srctag, Int32 src);
//
//	BaseTag*				(*GetOrigin						)(BaseTag *tag);
//	const void*			(*GetDataAddressR			)(VariableTag *bt);
//	void						(*UvGet2							)(const void *handle, Int32 i, UVWStruct *s);
//	void						(*UvSet2							)(void *handle, Int32 i, const UVWStruct &s);
//	void						(*UvCpy2							)(const void *srchandle, Int32 src, void *dsthandle, Int32 dst);
//
//	void						(*NrmGet							)(const void *handle, Int32 i, NormalStruct *s);
//	void						(*NrmSet							)(void *handle, Int32 i, const NormalStruct &s);
//	void						(*NrmCpy							)(const void *srchandle, Int32 src, void *dsthandle, Int32 dst);
//};
//
//struct C4D_Object
//{
//	BaseObject*			(*Alloc								)(Int32 type);
//	SplineObject*		(*AllocSplineObject		)(Int32 pcnt, SPLINETYPE type);
//	Float						(*GetVisibility				)(BaseObject *op, Float parent);
//
//	Vector					(BaseObject::*GetAbsPos)() const;
//	void						(BaseObject::*SetAbsPos						)(const Vector &v);
//	Vector					(BaseObject::*GetAbsScale					)() const;
//	void						(BaseObject::*SetAbsScale					)(const Vector &v);
//	Vector					(BaseObject::*GetAbsRot						)() const;
//	void						(BaseObject::*SetAbsRot						)(const Vector &v);
//	const Matrix&		(BaseObject::*GetMl								)() const;
//	void						(BaseObject::*SetMl								)(const Matrix &m);
//	Matrix					(BaseObject::*GetMg								)() const;
//	void						(BaseObject::*SetMg								)(const Matrix &m);
//	Matrix					(BaseObject::*GetMln							)() const;
//	Matrix					(BaseObject::*GetMgn							)() const;
//	Matrix					(BaseObject::*GetUpMg							)() const;
//
//	Vector					(BaseObject::*GetFrozenPos				)() const;
//	void						(BaseObject::*SetFrozenPos				)(const Vector &v);
//	Vector					(BaseObject::*GetFrozenScale			)() const;
//	void						(BaseObject::*SetFrozenScale			)(const Vector &v);
//	Vector					(BaseObject::*GetFrozenRot				)() const;
//	void						(BaseObject::*SetFrozenRot				)(const Vector &v);
//	Matrix					(BaseObject::*GetFrozenMln					)() const;
//	Matrix					(BaseObject::*GetRelMln						)() const;
//
//	Vector					(BaseObject::*GetRelPos						)() const;
//	void						(BaseObject::*SetRelPos						)(const Vector &v);
//	Vector					(BaseObject::*GetRelScale					)() const;
//	void						(BaseObject::*SetRelScale					)(const Vector &v);
//	Vector					(BaseObject::*GetRelRot						)() const;
//	void						(BaseObject::*SetRelRot						)(const Vector &v);
//	Matrix					(BaseObject::*GetRelMl						)() const;
//	void						(BaseObject::*SetRelMl						)(const Matrix &m);
//
//	Vector					(*GetMp								)(BaseObject *op);
//	Vector					(*GetRad							)(BaseObject *op);
//	Int32						(*GetMode							)(BaseObject *op, OBJECTSTATE mode);
//	void						(*SetMode							)(BaseObject *op, OBJECTSTATE mode, Int32 val);
//	BaseTag*				(*GetFirstTag					)(BaseObject *op);
//	BaseTag*				(*GetTag							)(BaseObject *op, Int32 type, Int32 nr);
//	void*						(*GetTagData					)(BaseObject *op, Int32 type, Int32 nr);
//	Int32						(*GetTagDataCount			)(const BaseObject *op, Int32 type);
//	void						(*InsertTag						)(BaseObject *op, BaseTag *tp, BaseTag *pred);
//	void						(*KillTag							)(BaseObject *op, Int32 type, Int32 nr);
//	Int32						(*GetInfo							)(GeListNode *op);
//	Bool						(*Edit								)(BaseList2D *op);
//	BaseObject*			(*GetCache						)(BaseObject *op, HierarchyHelp *hh);
//	BaseObject*			(*GetDeformCache			)(BaseObject *op);
//	LineObject*			(*GetIsoparm					)(BaseObject *op);
//	Bool						(*IsDirty							)(BaseObject *op, DIRTYFLAGS flags);
//	void						(*SetDirty						)(C4DAtom *op, DIRTYFLAGS flags);
//	Bool						(*CheckCache					)(BaseObject *op, HierarchyHelp *hh);
//	void						(*SetIsoparm					)(BaseObject *op, LineObject *l);
//	BaseObject*			(*GenPrimitive				)(BaseDocument *doc, Int32 type, const BaseContainer *bc, Float lod, Bool isoparm, BaseThread *bt);
//	BaseObject*			(*GenSplinePrimitive	)(BaseDocument *doc, Int32 type, const BaseContainer *bc, Float lod, BaseThread *bt);
//	void						(*NewDependenceList		)(BaseObject *op);
//	Bool						(*CmpDependenceList		)(BaseObject *op);
//	void						(*TouchDependenceList	)(BaseObject *op);
//	void						(*AddDependence				)(BaseObject *op,HierarchyHelp *hh, BaseObject *pp);
//	Bool						(*AddTexture					)(BaseList2D *op, const Filename *fn, AssetData *gd);
//	BaseObject*			(*GetVirtualLineObject)(BaseObject *op, HierarchyHelp *hh, const Matrix &mloc, Bool keep_spline, Bool recurse, Matrix *mres, Bool *dirty);
//	void						(*Touch								)(BaseObject *op);
//
//	// point object
//	BaseSelect*			(*PoGetPointS					)(PointObject *op);
//	BaseSelect*			(*PoGetPointH					)(PointObject *op);
//	Bool						(*PoResizeObject			)(PointObject *op, Int32 pcnt);
//	Float32*					(*PoCalcVertexMap			)(PointObject *op, BaseObject *modifier);
//
//	// line object
//	Bool						(*LoResizeObject			)(LineObject *op, Int32 pcnt, Int32 scnt);
//
//	// polygon object
//	BaseSelect*			(*PyGetPolygonS				)(PolygonObject *op);
//	BaseSelect*			(*PyGetPolygonH				)(PolygonObject *op);
//	Bool						(*PyResizeObject			)(PolygonObject *op, Int32 pcnt, Int32 vcnt);
//
//	// spline object
//	Vector					(*SpGetSplinePoint		)(SplineObject *op, Float t, Int32 segment, const Vector *padr);
//	Vector					(*SpGetSplineTangent	)(SplineObject *op, Float t, Int32 segment, const Vector *padr);
//
//	SplineLengthData* (*SpLDAlloc					)(void);
//	void						(*SpLDFree						)(SplineLengthData *&sp);
//
//	Bool						(*SpInitLength				)(SplineLengthData *dat, SplineObject *op, Int32 segment, const Vector *padr);
//	Float						(*SpUniformToNatural	)(SplineLengthData *dat, Float t);
//	Float						(*SpGetLength					)(SplineLengthData *dat);
//	Float						(*SpGetSegmentLength	)(SplineLengthData *dat, Int32 a, Int32 b);
//	LineObject*			(*SpGetLineObject			)(SplineObject *op, BaseDocument *doc, Float lod, BaseThread *thread);
//	SplineObject*		(*SpGetRealSpline			)(BaseObject *op);
//	Bool						(*SpResizeObject			)(SplineObject *op, Int32 pcnt, Int32 scnt);
//
//	// particle object
//	Int32						(*PrGetCount					)(BaseObject *op);
//	Float						(*PrGetLifetime				)(BaseObject *op);
//	Particle*				(*PrGetParticleW			)(BaseObject *op, ParticleTag *pt, Int32 i);
//	Bool						(*PrIsMatrixAvailable	)(BaseObject *op);
//	ParticleDetails*(*PrGetParticleDetails)(BaseDocument *doc, BaseObject *op);
//
//	// camera object
//	Int32						(*CoGetProjection			)(BaseObject *op);
//	Float						(*CoGetFocus					)(BaseObject *op);
//	Float						(*CoGetZoom						)(BaseObject *op);
//	Vector					(*CoGetOffset					)(BaseObject *op);
//	Float						(*CoGetAperture				)(BaseObject *op);
//	Bool						(*CoSetProjection			)(BaseObject *op, Int32 projection);
//	Bool						(*CoSetFocus					)(BaseObject *op, Float v);
//	Bool						(*CoSetZoom						)(BaseObject *op, Float v);
//	Bool						(*CoSetOffset					)(BaseObject *op, const Vector &offset);
//	Bool						(*CoSetAperture				)(BaseObject *op, Float v);
//
//	// object safety
//	ObjectSafety*		(*OsAlloc							)(BaseObject *op);
//	void						(*OsFree							)(ObjectSafety *os, Bool restore);
//
//	// triangulation
//	Bool						(*Triangulate					)(const Vector *padr, Int32 pcnt, CPolygon **vadr, Int32 *vcnt);
//	PolygonObject*	(*TriangulateLine			)(LineObject *op, Float regular, BaseThread *bt);
//	SplineObject*		(*FitCurve						)(Vector *padr, Int32 pcnt, Float error, BaseThread *bt);
//
//	// uv stuff
//	UVWTag*					(*GenerateUVW					)(BaseObject *op, const Matrix &opmg, TextureTag *tp, const Matrix &texopmg, BaseView *view);
//
//	ULONG						(*GetDirty						)(const C4DAtom *op, DIRTYFLAGS flags);
//
//	Bool						(*TriangulateStandard )(const Vector *padr, Int32 pcnt, const Int32 *list, Int32 lcnt, CPolygon *&vadr, Int32 &vcnt, BaseThread *thread);
//	Bool						(*TriangulateRegular	)(const Vector *pinp, Int32 pinp_cnt, const Int32 *list, Int32 lcnt, Vector *&padr, Int32 &pcnt, CPolygon *&vadr, Int32 &vcnt, Float regular_width, BaseThread *thread);
//
//	Bool						(*SpSetDefaultCoeff		)(SplineObject *op);
//	BaseObject*			(*GenerateText				)(BaseContainer *cp, BaseThread *bt, Bool separate);
//
//	BaseSelect*			(*PyGetEdgeS					)(PolygonObject *op);
//	BaseSelect*			(*PyGetEdgeH					)(PolygonObject *op);
//	void						(*GetColorProperties	)(BaseObject *op, ObjectColorProperties *co);
//	void						(*SetColorProperties	)(BaseObject *op, ObjectColorProperties *co);
//
//	Bool						(*CopyTagsTo					)(BaseObject *op, BaseObject *dest, Int32 visible, Int32 variable, Int32 hierarchical, AliasTrans *trans);
//	BaseObject*			(*GetHierarchyClone		)(BaseObject *op,HierarchyHelp *hh, BaseObject *pp, HIERARCHYCLONEFLAGS flags, Bool *dirty, AliasTrans *trans);
//
//	BaseObject*			(*GetCacheParent			)(BaseObject *op);
//	Bool						(*CheckDisplayFilter	)(BaseObject *op, Int32 flags);
//
//	BaseSelect*			(*PyGetPhongBreak			)(PolygonObject *op);
//
//	Int32						(*GetUniqueIP					)(BaseObject *op);
//	void						(*SetUniqueIP					)(BaseObject *op, Int32 ip);
//
//	void						(*SetOrigin						)(BaseObject *op, BaseObject *origin);
//	BaseObject*			(*GetOrigin						)(BaseObject *op, Bool safe);
//	BaseObject*			(*InternalCalcBooleOld)(BaseObject *curr,Int32 booletype, HierarchyHelp *hh);
//
//	SVector*				(*CreatePhongNormals	)(PolygonObject *op);
//
//	// triangulation
//	PolyTriangulate* (*PolyTriangAlloc    )();
//	void            (*PolyTriangFree      )(PolyTriangulate *&pTriang);
//	Bool            (*PolyTriangTriang    )(PolyTriangulate* pTriang, const Vector* pvPoints, const Int32 lPointCount, const Int32* plSegments, const Int32 lSegCnt,
//		CPolygon*& pPolys, Int32& lPolyCount, Int32 lFlags, const Int32* plMap, BaseThread* pThread,
//		const Int32 lConstraints, const Int32* plConstrainedEdges);
//	Bool            (*PolyTriangTriangRelease)(PolyTriangulate* pTriang, const Vector* pvPoints, const Int32 lPointCount, const Int32* plSegments, const Int32 lSegCnt,
//		CPolygon*& pPolys, Int32& lPolyCount, Int32 lFlags, const Int32* plMap, BaseThread* pThread,
//		const Int32 lConstraints, const Int32* plConstrainedEdges);
//
//	Bool						(*PyGetPolygonTranslationMap)(PolygonObject *op, Int32 &ngoncnt, Int32 *&polymap);
//	Bool						(*PyGetNGonTranslationMap)(PolygonObject *op, const Int32 ngoncnt, const Int32 *polymap, Int32 **&ngons);
//	Pgon*           (*PyGetNgon)(PolygonObject *op);
//	Int32            (*PyGetNgonCount)(PolygonObject *op);
//	Bool						(*PyResizeObjectNgon	)(PolygonObject *op, Int32 pcnt, Int32 vcnt, Int32 ncnt);
//	void						(*PyGetSelectedNgons  )(PolygonObject *op, BaseSelect* sel);
//	void						(*PyGetHiddenNgons    )(PolygonObject *op, BaseSelect* sel);
//	NgonBase*				(*PyGetNgonBase)(PolygonObject *op);
//	Bool						(*PyValidateEdgeSelection)(PolygonObject *op, BaseSelect* sel);
//	Bool						(*PyGetEdgeSelection)(PolygonObject *op, BaseSelect* sel, EDGESELECTIONTYPE type);
//
//	BaseObject*			(*GetTopOrigin						)(BaseObject *op, Bool parent, Bool safe);
//	BaseObject*			(*GetEditObject						)(BaseObject *op, BaseObject **psds, DISPLAYEDITSTATE state);
//	Int32						(*GetHighlightHandle			)(BaseObject *op, BaseDraw *bd);
//
//	const Matrix &  (*GetModelingAxis)(BaseObject *op, BaseDocument *doc);
//	void            (*SetModelingAxis)(BaseObject *op, const Matrix &m);
//	Bool            (*PolyTriangHasIdentical)(PolyTriangulate* pTriang);
//	Bool            (*CalculateVisiblePoints)(BaseDraw *bd, PolygonObject *op, Vector *padr, UChar *pset, Bool select_visibonly);
//	Int32            (*PolyTriangGetType)(PolyTriangulate* pTriang);
//
//	void            (*PyGetNgonEdgesCompact)(PolygonObject *op, UChar *&edges);
//	ULONG						(*PyVBOInitUpdate)(PolygonObject *op, BaseDraw* bd);
//	Bool						(*PyVBOStartUpdate)(PolygonObject *op, BaseDraw* bd);
//	void						(*PyVBOUpdateVector)(PolygonObject *op, Int32 i, const SVector &v, ULONG flags);
//	void						(*PyVBOEndUpdate)(PolygonObject *op, BaseDraw* bd);
//	void						(*PyVBOFreeUpdate)(PolygonObject *op);
//
//	Int32						(*IntersectionTest			)(PolygonObject *op, BaseDraw *bd, Float x, Float y, const Matrix &mg, Float *z, MODELINGCOMMANDMODE mode, UChar *pPointSelect, Int32 lSelectCount);
//
//	Bool						(*PyValidateEdgeSelectionA)(PolygonObject *op);
//
//#ifdef __SINGLEPRECISION
//	Bool            (*PolyTriangTriangA   )(PolyTriangulate* pTriang, const LVector* pvPoints, const Int32 lPointCount, const Int32* plSegments, const Int32 lSegCnt,
//		CPolygon*& pPolys, Int32& lPolyCount, Int32 lFlags, const Int32* plMap, BaseThread* pThread,
//		const Int32 lConstraints, const Int32* plConstrainedEdges);
//	Bool            (*PolyTriangTriangReleaseA)(PolyTriangulate* pTriang, const LVector* pvPoints, const Int32 lPointCount, const Int32* plSegments, const Int32 lSegCnt,
//		CPolygon*& pPolys, Int32& lPolyCount, Int32 lFlags, const Int32* plMap, BaseThread* pThread,
//		const Int32 lConstraints, const Int32* plConstrainedEdges);
//#endif
//
//	void            (*PolyTriangSetPolygonMatrix)(PolyTriangulate* pTriang, LMatrix* m);
//	ULONG           (*PolyTriangGetState)(PolyTriangulate* pTriang);
//	const Particle*	(*PrGetParticleR			)(BaseObject *op, ParticleTag *pt, Int32 i);
//
//	void						(*GetIcon							)(BaseList2D *op, IconData *dat);
//
//	ULONG						(C4DAtom::*GetHDirty	)(HDIRTYFLAGS mask) const;
//	void						(C4DAtom::*SetHDirty	)(HDIRTYFLAGS mask);
//
//	const void*			(*GetTagDataR					)(const BaseObject *op, Int32 type, Int32 nr);
//
//	void						(*RemoveFromCache			)(BaseObject *op);
//	Bool						(*SearchHierarchy)(BaseObject *op, BaseObject *find);
//	Bool						(*PyResizeObjectNgonFlags	)(PolygonObject *op, Int32 pcnt, Int32 vcnt, Int32 ncnt, Int32 vc_flags);
//	void						(*CopyMatrixTo						)(BaseObject *src, BaseObject *dst);
//	Int32						(*CoAnaglyphGetCameraCount)(const BaseObject* op, BaseDocument* doc, BaseDraw* bd, RenderData* rd, Int32 flags);
//	Bool						(*CoAnaglyphGetCameraInfo)(const BaseObject* op, BaseDocument* doc, BaseDraw* bd, RenderData* rd, Int32 n, AnaglyphCameraInfo &info, Int32 flags);
//};
//
//struct C4D_Document
//{
//	// render data
//	RenderData*			(*RdAlloc							)(void);
//
//	// document
//	BaseDocument*		(*Alloc								)(void);
//	void						(*FlushAll						)(BaseDocument *doc);
//	BaseContainer		(*GetData							)(BaseDocument *doc, DOCUMENTSETTINGS type);
//	void						(*SetData							)(BaseDocument *doc, DOCUMENTSETTINGS type, const BaseContainer *bc);
//	BaseObject*			(*GetFirstObject			)(BaseDocument *doc);
//	BaseMaterial*		(*GetFirstMaterial		)(BaseDocument *doc);
//	RenderData*			(*GetFirstRenderData	)(BaseDocument *doc);
//	void						(*InsertRenderData		)(BaseDocument *doc, RenderData *rd, RenderData *parent, RenderData *pred);
//	void						(*InsertMaterial			)(BaseDocument *doc, BaseMaterial *mat, BaseMaterial *pred, Bool checknames);
//	void						(*InsertObject				)(BaseDocument *doc, BaseObject *op, BaseObject *parent, BaseObject *pred, Bool checknames);
//	RenderData*			(*GetActiveRenderData	)(BaseDocument *doc);
//	BaseObject*			(*GetActiveObject			)(BaseDocument *doc);
//	BaseMaterial*		(*GetActiveMaterial		)(BaseDocument *doc);
//	BaseTag*				(*GetActiveTag				)(BaseDocument *doc, BaseObject *active);
//	void						(*SetActiveRenderData	)(BaseDocument *doc, RenderData *rd);
//	BaseObject*			(*GetHighest					)(BaseDocument *doc, Int32 type, Bool editor);
//	BaseMaterial*		(*SearchMaterial			)(BaseDocument *doc, const String *str, Bool inc);
//	BaseObject*			(*SearchObject				)(BaseDocument *doc, const String *str, Bool inc);
//	Bool						(*StartUndo						)(BaseDocument *doc);
//	Bool						(*EndUndo							)(BaseDocument *doc);
//	Bool						(*AddUndo							)(BaseDocument *doc, UNDOTYPE type, void *data);
//	Bool						(*DoRedo							)(BaseDocument *doc);
//	void						(*AnimateObject				)(BaseDocument *doc, BaseList2D *op, const BaseTime &time, ANIMATEFLAGS flags);
//	BaseDraw*				(*GetActiveBaseDraw		)(BaseDocument *doc);
//	BaseDraw*				(*GetRenderBaseDraw		)(BaseDocument *doc);
//	BaseDraw*				(*GetBaseDraw					)(BaseDocument *doc, Int32 num);
//	Int32						(*GetSplinePlane			)(BaseDocument *doc);
//
//	// hierarchy help
//	Float						(*HhGetLOD						)(HierarchyHelp *hh);
//	BUILDFLAGS			(*HhGetBuildFlags			)(HierarchyHelp *hh);
//	BaseThread*			(*HhGetThread					)(HierarchyHelp *hh);
//	BaseDocument*		(*HhGetDocument				)(HierarchyHelp *hh);
//	const Matrix&		(*HhGetMg							)(HierarchyHelp *hh);
//
//	// hierarchy
//	Bool						(*RunHierarchy				)(void *main, BaseDocument *doc, Bool spheres, Float lod, Bool uselod, BUILDFLAGS flags, void *startdata, BaseThread *bt, HierarchyAlloc *ha, HierarchyFree *hf, HierarchyCopyTo *hc, HierarchyDo *hd);
//
//	BaseSceneHook*  (*FindSceneHook				)(const BaseDocument *doc,Int32 id);
//
//	void						(BaseDocument::*SetActiveObject		)(BaseObject *op,Int32 mode);
//	void						(BaseDocument::*GetActiveObjects	)(AtomArray &selection,Bool children) const;
//	void						(BaseDocument::*GetActiveTags			)(AtomArray &selection) const;
//
//	void						(*PrAdd								)(PriorityList *list, GeListNode *node, Int32 priority, EXECUTIONFLAGS flags);
//	BaseObject*			(*GetHelperAxis				)(BaseDocument *doc);
//	BaseVideoPost*  (*RdGetFirstVideoPost	)(RenderData *rd);
//	void						(*RdInsertVideoPost		)(RenderData *rd, BaseVideoPost *pvp, BaseVideoPost *pred);
//	void						(BaseDocument::*GetActiveMaterials)(AtomArray &selection) const;
//
//	void						(*SetRewind						)(BaseDocument *doc, Int32 flags);
//
//	void						(BaseDocument::*SetActiveTag			)(BaseTag *op,Int32 mode);
//	void						(BaseDocument::*SetActiveMaterial	)(BaseMaterial *mat,Int32 mode);
//
//	BaseVideoPost*(*VpAlloc								)(Int32 type);
//
//	BaseList2D*     (*GetUndoPtr            )(BaseDocument *doc);
//	void            (*AutoKey               )(BaseDocument *doc, BaseList2D *undo, BaseList2D *op, Bool recursive, Bool pos, Bool scale, Bool rot, Bool param, Bool pla);
//	Bool						(*DoUndo								)(BaseDocument *doc, Bool multiple);
//	Bool						(*IsCacheBuilt				  )(BaseDocument *doc, Bool force);
//	void						(BaseDocument::*GetActivePolygonObjects)(AtomArray &selection,Bool children) const;
//	BaseTime				(*GetUsedTime						)(BaseDocument *doc, BaseList2D *check_op, Bool min);
//	void						(*ForceCreateBaseDraw		)(BaseDocument *doc);
//
//	BaseContainer*  (*GetDataInstance				)(BaseDocument *doc, Int32 type);
//	void						(*RunAnimation					)(BaseDocument *doc, Bool forward, Bool stop);
//	void						(*SetDocumentTime				)(BaseDocument *doc, const BaseTime &time);
//
//	BaseDocument*		(*IsolateObjects				)(BaseDocument *doc,const AtomArray &t_objects);
//
//	void						(BaseDocument::*GetSelection)(AtomArray &selection) const;
//	void						(BaseDocument::*SetSelection      )(BaseList2D *bl,Int32 mode);
//
//	// layers
//	LayerObject*		(*DlAlloc							  )(void);
//	GeListHead*     (*GetLayerObjectRoot    )(BaseDocument *doc);
//	Bool            (*HandleSelectedTextureFilename)(BaseDocument *doc, BaseChannel *bc, const Filename &fn, Filename *resfilename, Bool undo, GEMB_R *already_answered);
//	Bool            (*ReceiveMaterials             )(BaseDocument *doc, BaseObject *op, AtomArray *mat, Bool clearfirst);
//	Bool            (*ReceiveNewTexture            )(BaseDocument *doc, BaseObject *op, const Filename &filename, Bool sdown, GEMB_R *already_answered);
//
//	void            (*SetKeyDefault         )(BaseDocument *doc, CCurve *seq, Int32 kidx);
//
//	void            (*Record                )(BaseDocument *doc);
//	BaseContainer		(*GetAllTextures				)(BaseDocument *doc, const AtomArray *ar);
//	Bool						(*RecordKey	            )(BaseDocument *doc, BaseList2D *op, const BaseTime &time, const DescID &id, BaseList2D *undo, Bool eval_attribmanager, Bool autokeying, Bool allow_linking);
//
//	Bool						(*CloseAllDocuments			)();
//	BaseObject*			(*GetRealActiveObject		)(BaseDocument *doc, const AtomArray *help, Bool *multi);
//	Bool						(*AddUndoBD							)(BaseDocument *doc, BaseDraw* bd);
//
//	Bool						(*CollectSounds         )(BaseDocument *doc, BaseSound *snd, const BaseTime &from, const BaseTime &to);
//	void            (*RecordZero            )(BaseDocument *doc);
//
//	MultipassObject*(*RdGetFirstMultipass   )(RenderData *rd);
//	void						(*RdInsertMultipass     )(RenderData *rd, MultipassObject *pvp, MultipassObject *pred);
//	MultiPassChannel *multipasschannels;
//	void						(*InsertRenderDataLast  )(BaseDocument *doc, RenderData *rd);
//
//	void            (*RecordNoEvent         )(BaseDocument *doc);
//	Int32						(*GetDrawTime						)(BaseDocument *doc);
//	Bool						(*StopExternalRenderer	)();
//	Int32						(*GetBaseDrawCount			)(BaseDocument *doc);
//	Bool						(BaseDocument::*ExecutePasses)(BaseThread *bt, Bool animation, Bool expressions, Bool caches, BUILDFLAGS flags);
//
//	const PickSessionDataStruct *(BaseDocument::*GetPickSession)() const;
//	void						(BaseDocument::*StartPickSession)(PickSessionDataStruct *psd);
//	void						(BaseDocument::*StopPickSession)(Bool cancel);
//};
//
//struct C4D_Thread
//{
//	BaseThread*			(*Alloc								)(ThreadMain *tm, ThreadTest *tt, void *data, ThreadName *tn);
//	void						(*Free								)(BaseThread *bt);
//	Bool						(*Start								)(BaseThread *bt, THREADMODE mode, THREADPRIORITY priority, void *reserved );
//	void						(*End									)(BaseThread *bt, Bool wait);
//	void						(*Wait								)(BaseThread *bt, Bool checkevents);
//	Bool						(*TestBreak						)(BaseThread *bt);
//	Bool						(*TestBaseBreak				)(BaseThread *bt);
//	Bool						(*IsRunning						)(BaseThread *bt);
//	THREADTYPE			(*Identify						)(BaseThread *bt);
//	void						(*ThreadLock					)(void);
//	void						(*ThreadUnlock				)(void);
//	Int32						(*GetCPUCount					)(void);
//	ULONG						(*GetCurrentThreadId	)(void);
//	BaseThread*			(*GetCurrentThread		)(void);
//
//	MPBaseThread*		(*MPAlloc							)(BaseThread *parent, Int32 count, ThreadMain *tm, ThreadTest *tt, void **data, ThreadName *tn);
//	void						(*MPFree							)(MPBaseThread *mp);
//	BaseThread*			(*MPGetThread					)(MPBaseThread *mp, Int32 i);
//	void*						(*MPWaitForNextFree		)(MPBaseThread *mp);
//	void						(*MPWait							)(MPBaseThread *mp);
//	void						(*MPEnd								)(MPBaseThread *mp);
//
//	Semaphore*			(*SMAlloc							)(void);
//	void						(*SMFree							)(Semaphore *&sm);
//	Bool						(*SMLock							)(Semaphore *sm);
//	Bool						(*SMLockAndWait				)(Semaphore *sm, BaseThread *bt);
//	void						(*SMUnlock						)(Semaphore *sm);
//
//	BaseThread			*NoThread;
//
//	void						(GeSpinlock::*Lock						)();
//	void						(GeSpinlock::*Unlock					)();
//	Bool						(GeSpinlock::*AttemptLock			)();
//
//	void						(GeRWSpinlock::*ReadLock				)();
//	void						(GeRWSpinlock::*ReadUnlock			)();
//	void						(GeRWSpinlock::*WriteLock				)();
//	void						(GeRWSpinlock::*WriteUnlock			)();
//	Bool						(GeRWSpinlock::*AttemptWriteLock)();
//
//	GeSignal*				(*SIGAlloc						)(void);
//	void						(*SIGFree							)(GeSignal *&sm);
//
//	Bool						(GeSignal::*SIGInit		)(SIGNALMODE mode);
//	void						(GeSignal::*SIGSet		)(void);
//	void						(GeSignal::*SIGClear	)(void);
//	Bool						(GeSignal::*SIGWait		)(Int32 timeout);
//};
//
//struct C4D_Material
//{
//	BaseMaterial*		(*Alloc								)(Int32 type);
//	void						(*Update							)(BaseMaterial *mat, Int32 preview, Bool rttm);
//	BaseChannel*		(*GetChannel					)(BaseMaterial *bm, Int32 id);
//	Bool						(*GetChannelState			)(Material *mat, Int32 channel);
//	void						(*SetChannelState			)(Material *mat, Int32 channel, Bool state);
//	Bool						(*Compare							)(BaseMaterial *m1, BaseMaterial *m2);
//	BaseBitmap*			(*GetPreview					)(BaseMaterial *bm, Int32 flags);
//
//	void						(BaseMaterial::*Displace				)(VolumeData *sd);
//	void						(BaseMaterial::*ChangeNormal		)(VolumeData *sd);
//	void						(BaseMaterial::*CalcSurface			)(VolumeData *sd);
//	void						(BaseMaterial::*CalcTransparency)(VolumeData *sd);
//	void						(BaseMaterial::*CalcAlpha				)(VolumeData *sd);
//	void						(BaseMaterial::*CalcVolumetric  )(VolumeData *sd);
//	void						(BaseMaterial::*InitCalculation )(VolumeData *sd, INITCALCULATION type);
//	VOLUMEINFO			(BaseMaterial::*GetRenderInfo	  )(void);
//
//	Vector					(*GetAverageColor			)(BaseMaterial *mat, Int32 channel);
//	Int32						(BaseMaterial::*GlMessage	)(Int32 type, void *data);
//};
//
//struct C4D_Texture
//{
//	Vector					(*GetPos							)(TextureTag *tag);
//	Vector					(*GetScale						)(TextureTag *tag);
//	Vector					(*GetRot							)(TextureTag *tag);
//	Matrix					(*GetMl								)(TextureTag *tag);
//	void						(*SetPos							)(TextureTag *tag, const Vector &v);
//	void						(*SetScale						)(TextureTag *tag, const Vector &v);
//	void						(*SetRot							)(TextureTag *tag, const Vector &v);
//	void						(*SetMl								)(TextureTag *tag, const Matrix &m);
//
//	void						(*SetMaterial					)(TextureTag *tag, BaseMaterial *mat);
//	BaseMaterial*		(*GetMaterial					)(TextureTag *tag, Bool ignoredoc);
//};
//
//struct C4D_BaseSelect
//{
//	BaseSelect*			(*Alloc								)(void);
//	void						(*Free								)(BaseSelect *bs);
//	Int32						(*GetCount						)(const BaseSelect *bs);
//	Int32						(*GetSegments					)(const BaseSelect *bs);
//	Bool						(*Select							)(BaseSelect *bs, Int32 num);
//	Bool						(*SelectAll						)(BaseSelect *bs, Int32 min, Int32 max);
//	Bool						(*Deselect						)(BaseSelect *bs, Int32 num);
//	Bool						(*DeselectAll					)(BaseSelect *bs);
//	Bool						(*Toggle							)(BaseSelect *bs, Int32 num);
//	Bool						(*ToggleAll						)(BaseSelect *bs, Int32 min, Int32 max);
//	Bool						(*GetRange						)(const BaseSelect *bs, Int32 seg, Int32 *a, Int32 *b);
//	Bool						(*IsSelected					)(const BaseSelect *bs, Int32 num);
//	Bool						(*CopyTo							)(const BaseSelect *bs, BaseSelect *dest);
//	BaseSelect*			(*GetClone						)(const BaseSelect *bs);
//	Bool						(*FromArray						)(BaseSelect *bs, UChar *selection, Int32 count);
//	UChar*					(*ToArray							)(const BaseSelect *bs, Int32 count);
//	Bool						(*Merge 							)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*DeselectBS					)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*Cross								)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*FindSegment					)(const BaseSelect *bs, Int32 num, Int32 *segment);
//	BaseSelectData*	(*GetData							)(BaseSelect *bs);
//	Bool						(*CopyFrom						)(BaseSelect *bs, BaseSelectData *ndata, Int32 ncnt);
//	Int32						(*GetDirty						)(const BaseSelect *bs);
//};
//
//struct C4D_CAnimation
//{
//	BaseTime        (CKey::*GetTime               )(void) const;
//	Bool            (CKey::*CopyDataTo            )(CCurve *destseq, CKey *dest, AliasTrans *trans) const;
//	void            (CKey::*FlushData1            )(void);
//	BaseTime				(CKey::*GetTimeLeft						)(void) const;
//	BaseTime				(CKey::*GetTimeRight					)(void) const;
//	Float						(CKey::*GetValue							)(void) const;
//	Float						(CKey::*GetValueLeft					)(void) const;
//	Float						(CKey::*GetValueRight					)(void) const;
//	CINTERPOLATION  (CKey::*GetInterpolation      )(void) const;
//	void						(CKey::*SetTime								)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetTimeLeft						)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetTimeRight					)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetValue							)(CCurve *seq, Float v);
//	void						(CKey::*SetValueLeft					)(CCurve *seq, Float v);
//	void						(CKey::*SetValueRight					)(CCurve *seq, Float v);
//	void						(CKey::*SetGeData							)(CCurve *seq, const GeData &d);
//	void						(CKey::*SetInterpolation      )(CCurve *seq, CINTERPOLATION inter);
//	CTrack*					(CKey::*GetTrackCKey					)(void);
//	CCurve*	    		(CKey::*GetSequenceCKey				)(void);
//	const GeData&   (CKey::*GetGeData             )(void) const;
//	CKey*						(*CKey_Alloc									)(void);
//	void						(*CKey_Free										)(CKey *&ckey);
//
//	Int32            (CCurve::*GetKeyCount         )(void) const;
//	CKey*           (CCurve::*GetKey1             )(Int32 index);
//	const CKey*     (CCurve::*GetKey2             )(Int32 index) const;
//	CKey*           (CCurve::*FindKey1            )(const BaseTime &time, Int32 *nidx, FINDANIM match);
//	const CKey*     (CCurve::*FindKey2            )(const BaseTime &time, Int32 *nidx, FINDANIM match) const;
//	CKey*           (CCurve::*AddKey              )(const BaseTime &time, Int32 *nidx);
//	Bool						(CCurve::*InsertKey				    )(const CKey *key);
//	Bool            (CCurve::*DelKey              )(Int32 index);
//	Int32						(CCurve::*MoveKey					    )(const BaseTime &time, Int32 idx, CCurve *dseq);
//	void            (CCurve::*FlushKeys           )(void);
//	void						(CCurve::*SortKeysByTime		  )(void);
//	void            (CCurve::*CalcSoftTangents    )(Int32 kidx, Float *vl, Float *vr, BaseTime *tl, BaseTime *tr) const;
//	LReal           (CCurve::*CalcHermite         )(LReal time, LReal t1, LReal t2, LReal val1, LReal val2, LReal tan1_val, LReal tan2_val, LReal tan1_t, LReal tan2_t, Bool deriv) const;
//	void	          (CCurve::*GetTangents         )(Int32 kidx, LReal *vl, LReal *vr, LReal *tl, LReal *tr) const;
//
//	Float            (CCurve::*GetValue1           )(const BaseTime &time,Int32 fps) const;
//	CTrack*					(CCurve::*GetTrackCSeq			  )(void);
//
//
//	CTrack*			  	(*CTrackAlloc								  )(BaseList2D *bl, const DescID &id);
//	Bool            (CTrack::*Animate             )(const CAnimInfo *info, Bool *chg, void *data);
//	const DescID&   (CTrack::*GetDescriptionID    )(void) const;
//	Bool            (CTrack::*SetDescriptionID    )(BaseList2D *object, const DescID &id);
//
//	Bool            (CTrack::*Draw                )(GeClipMap *map, const BaseTime &clip_left, const BaseTime &clip_right) const;
//	CLOOP						(CTrack::*GetBefore           )(void) const;
//	CLOOP						(CTrack::*GetAfter            )(void) const;
//	void						(CTrack::*SetBefore           )(CLOOP type);
//	void						(CTrack::*SetAfter            )(CLOOP type);
//
//	CTrack*         (CTrack::*GetTimeTrack        )(BaseDocument *doc);
//	CCurve*         (CTrack::*GetCurve            )(CCURVE type,Bool bCreate);
//	void            (CTrack::*SetTimeTrack        )(CTrack *track);
//	void            (CTrack::*FlushData           )(void);
//	Float            (CTrack::*GetValue3           )(BaseDocument *doc, const BaseTime &time, Int32 fps);
//	Bool            (CTrack::*Remap								)(LReal time, LReal *ret_time, Int32 *ret_cycle) const;
//	Int32						(CTrack::*GuiMessage					)(const BaseContainer &msg,BaseContainer &result);
//	Int32						(CTrack::*GetHeight						)();
//	Bool						(CTrack::*FillKey							)(BaseDocument *doc, BaseList2D *bl, CKey *key);
//	Bool						(CTrack::*TrackInformation    )(BaseDocument *doc, CKey *key, String *str, Bool set);
//
//	const BaseContainer *(CTrack::*GetParameterDescription)(BaseContainer &temp) const;
//
//	Int32						(CTrack::*GetTrackCategory    )(void) const;
//	Bool            (CTrack::*AnimateTrack        )(BaseDocument *doc, BaseList2D *op, const BaseTime &tt, const Int32 flags, Bool *chg, void *data);
//	Int32						(CTrack::*GetUnit							)(Float *step);
//
//	Int32						(CTrack::*GetTLHeight					)(Int32 id);
//	void						(CTrack::*SetTLHeight					)(Int32 id,Int32 size);
//};
//
//
//struct C4D_BaseSound
//{
//	BaseSound*			(*Alloc								)(void);
//	void						(*Free								)(BaseSound *bs);
//	BaseSound*			(*GetClone						)(BaseSound *bs);
//	Bool						(*CopyTo							)(BaseSound *bs, BaseSound *dest);
//	Bool						(*Init								)(BaseSound *bs, Int32 samples, Int32 fmode, Bool stereo, Bool b16);
//	void						(*FlushAll						)(BaseSound *bs);
//	Bool						(*Load								)(BaseSound *bs, const Filename *fn);
//	Bool						(*Save								)(BaseSound *bs, const Filename *fn);
//	void						(*GetSample						)(BaseSound *bs, Int32 i, SData *data);
//	void						(*SetSample						)(BaseSound *bs, Int32 i, SData *data);
//	BaseBitmap*			(*GetBitmap						)(BaseSound *bs, Int32 width, Int32 height, const BaseTime &start, const BaseTime &stop);
//	Char*						(*GetRAW							)(BaseSound *bs);
//	Bool            (*WriteIt             )(BaseSound *bs, HyperFile *hf);
//	Bool            (*ReadIt              )(BaseSound *bs, HyperFile *hf, Int32 level);
//	BaseBitmap*			(*GetBitmap2					)(BaseSound *bs, Int32 width, Int32 height, const BaseTime &start, const BaseTime &stop, const Vector &draw_col, const Vector &back_col);
//	GeListHead*     (*GetMarkerRoot       )(BaseSound *bs);
//	BaseSound*			(*GetClonePart				)(BaseSound *bs,	const BaseTime &start,	const BaseTime &stop,	Bool reverse);
//	Char*						(*GetSoundInfo				)(BaseSound *bs, Bool *stereo, Bool *b16, Int32 *frequency, Int32 *samples, BaseTime *length);
//};
//
//struct C4D_BaseDraw
//{
//	// basedraw
//	Bool						(*HasCameraLink				)(BaseDraw *bd);
//	BaseObject*			(*GetEditorCamera			)(BaseDraw *bd);
//	Vector					(*CheckColor					)(BaseDraw *bd, const Vector &col);
//	void						(*SetTransparency			)(BaseDraw *bd, Int32 trans);
//	Int32						(*GetTransparency			)(BaseDraw *bd);
//	Bool						(*PointInRange				)(BaseDraw *bd, const Vector &p, Int32 x, Int32 y);
//	void						(*SetPen							)(BaseDraw *bd, const Vector &col, Int32 flags);
//	Float						(*SimpleShade					)(BaseDraw *bd, const Vector &p, const Vector &n);
//	void						(*DrawPoint2D					)(BaseDraw *bd, const Vector &p);
//	void						(*DrawLine2D					)(BaseDraw *bd, const Vector &p1, const Vector &p2); // draw line with 2D clipping
//	void						(*DrawHandle2D				)(BaseDraw *bd, const Vector &p, DRAWHANDLE type);
//	void						(*DrawCircle2D				)(BaseDraw *bd, Int32 mx, Int32 my, Float rad);
//	DRAWRESULT			(*DrawPObject					)(BaseDraw *bd, BaseDrawHelp *bh, BaseObject *op, DRAWOBJECT drawpolyflags, DRAWPASS drawpass, BaseObject* parent, const Vector &col);
//
//	// basedraw help
//	BaseDocument*		(*BbGetDocument				)(BaseDrawHelp *bb);
//	BaseTag*				(*BbGetActiveTag			)(BaseDrawHelp *bb);
//	const Matrix&		(*BbGetMg							)(BaseDrawHelp *bb);
//	void						(*BbGetDisplay				)(BaseDrawHelp *bb, BaseContainer *bc);
//	void						(*BbSetDisplay				)(BaseDrawHelp *bb, BaseContainer *bc);
//	void						(*BbSetMg							)(BaseDrawHelp *bb, const Matrix &mg);
//	DRAWFLAGS				(*BbGetViewSchedulerFlags)(const BaseDrawHelp *bb);
//	BaseDrawHelp   *(*BbAlloc   					)(BaseDraw *bd, BaseDocument *doc);
//	void            (*BbFree    					)(BaseDrawHelp *&p);
//
//	BaseObject*			(*GetSceneCamera			)(BaseDraw *bd, BaseDocument *doc);
//	Vector					(*GetObjectColor			)(BaseDraw *bd, BaseObject *op, BaseDrawHelp *bh, Bool lines);
//	void            (*LineZOffset         )(BaseDraw *bd, Int32 offset);
//	void            (*SetMatrix_Projection)(BaseDraw *bd);
//	void            (*SetMatrix_Screen    )(BaseDraw *bd);
//	void            (*SetMatrix_Camera    )(BaseDraw *bd);
//	void            (*SetMatrix_Matrix    )(BaseDraw *bd, BaseObject *op,const Matrix &mg);
//	void            (*DrawLine            )(BaseDraw *bd, const Vector &p1, const Vector &p2,Int32 flags);
//	void            (*LineStripBegin      )(BaseDraw *bd);
//	void            (*LineStrip           )(BaseDraw *bd, const Vector &vp,const Vector &vc,Int32 flags);
//	void            (*LineStripEnd        )(BaseDraw *bd);
//	void            (*DrawHandle          )(BaseDraw *bd, const Vector &vp,DRAWHANDLE type,Int32 flags);
//	void						(*DrawTexture					)(BaseDraw *bd, BaseBitmap *bmp, Vector *padr4, Vector *cadr, Vector *vnadr, Vector *uvadr, Int32 pntcnt, DRAW_ALPHA alphamode, DRAW_TEXTUREFLAGS flags);
//	void						(*SetLightList				)(BaseDraw *bd, Int32 mode);
//
//	void						(*InitUndo						)(BaseDraw *bd, BaseDocument *doc);
//	void						(*DoUndo							)(BaseDraw *bd, BaseDocument *doc);
//	void						(*SetDrawParam				)(BaseDraw *bd, Int32 id,const GeData &data);
//	GeData					(*GetDrawParam				)(BaseDraw *bd, Int32 id);
//
//	void						(*DrawPoly						)(BaseDraw *bd, Vector *vp,Vector *vf,Vector *vn,Int32 anz,Int32 flags);
//	DISPLAYFILTER		(*GetDisplayFilter		)(BaseDraw *bd);
//	DISPLAYEDITSTATE(*GetEditState			)(BaseDraw *bd);
//	Bool						(*IsViewOpen					)(BaseDraw *bd, BaseDocument *doc);
//
//	void						(*DrawCircle					)(BaseDraw *bd, const Matrix &m);
//	void						(*DrawBox 						)(BaseDraw *bd, const Matrix &m, Float size, const Vector &col, Bool wire);
//	void						(*DrawPolygon					)(BaseDraw *bd, Vector *p, Vector *f, Bool quad);
//	void						(*DrawSphere					)(BaseDraw *bd, const Vector &off, const Vector &size, const Vector &col, Int32 flags);
//	Bool						(*TestBreak						)(BaseDraw *bd);
//	void						(*DrawArrayEnd				)(BaseDraw *bd);
//	OITInfo&				(*GetOITInfo					)(BaseDraw *bd);
//	Int32						(*GetGlLightCount			)(const BaseDraw *bd);
//	const GlLight*	(*GetGlLight					)(const BaseDraw *bd, Int32 lIndex);
//
//	Bool						(*GetFullscreenPolygonVectors)(BaseDraw *bd, Int32 &lAttributeCount, const GlVertexBufferAttributeInfo* const *&ppAttibuteInfo, Int32 &lVectorInfoCount, const GlVertexBufferVectorInfo* const* &ppVectorInfo);
//	Bool						(*DrawFullscreenPolygon)(BaseDraw *bd, Int32 lVectorInfoCount, const GlVertexBufferVectorInfo* const* ppVectorInfo);
//	Bool						(*AddToPostPass				)(BaseDraw *bd, BaseObject *op,BaseDrawHelp *bh, Int32 flags);
//	Bool						(*GetDrawStatistics		)(const BaseDraw* bd, BaseContainer &bc);
//
//	void            (*SetMatrix_MatrixO   )(BaseDraw *bd, BaseObject *op,const Matrix &mg, Int32 zoffset);
//	void            (*SetMatrix_ScreenO   )(BaseDraw *bd, Int32 zoffset);
//	void            (*SetMatrix_CameraO   )(BaseDraw *bd, Int32 zoffset);
//
//	EditorWindow*		(*GetEditorWindow)(BaseDraw *bd);
//
//	void						(*DrawPointArray			)(BaseDraw *bd, Int32 cnt, const SVector *vp, const Float32 *vc, Int32 colcnt, const SVector *vn);
//	void						(*DrawTexture1				)(BaseDraw *bd, C4DGLuint bmp, Vector *padr4, Vector *cadr, Vector *vnadr, Vector *uvadr, Int32 pntcnt, DRAW_ALPHA alphamode);
//	void						(*InitClipbox					)(BaseDraw *bd, Int32 left, Int32 top, Int32 right, Int32 bottom, Int32 flags);
//	void						(*InitView						)(BaseDraw *bd, BaseContainer *camera, const Matrix &op_m, Float sv, Float pix_x, Float pix_y, Bool fitview);
//	void						(*InitializeView			)(BaseDraw *bd, BaseDocument *doc, BaseObject *cam, Bool editorsv);
//	void						(*SetTexture					)(BaseDraw *bd, BaseBitmap *bm, Bool tile, DRAW_ALPHA alphamode, DRAW_TEXTUREFLAGS flags);
//	void						(*SetSceneCamera			)(BaseDraw *bd, BaseObject *op, Bool animate);
//	void            (*SetMatrix_ScreenOM  )(BaseDraw *bd, Int32 zoffset, const Matrix4* m);
//
//	Bool						(*InitDrawXORLine			)(BaseDraw *bd);
//	void						(*FreeDrawXORLine			)(BaseDraw *bd);
//	void						(*DrawXORPolyLine			)(BaseDraw *bd, const Float32* p, Int32 cnt);
//	void						(*BeginDrawXORPolyLine)(BaseDraw *bd);
//	void						(*EndDrawXORPolyLine	)(BaseDraw *bd, Bool blit);
//	BaseDraw*				(*AllocBaseDraw				)();
//	void						(*FreeBaseDraw				)(BaseDraw*& bv);
//
//	Bool						(*DrawScene						)(BaseDraw* bd, Int32 flags);
//	DISPLAYMODE			(*GetReductionMode		)(const BaseDraw* bd);
//	void						(*GlDebugString				)(BaseDraw* bd, const char* str);
//	void						(*SetPointSize				)(BaseDraw* bd, Float pointsize);
//};
//
//struct C4D_BaseView
//{
//	void						(*GetFrame						)(BaseView *bv, Int32 *cl, Int32 *ct, Int32 *cr, Int32 *cb);
//	void						(*GetSafeFrame				)(BaseView *bv, Int32 *from, Int32 *to, Int32 *horizontal);
//	void						(*GetParameter				)(const BaseView *bv, Vector *offset, Vector *scale, Vector *scale_z);
//	Matrix					(*GetMg								)(BaseView *bv);
//	Matrix					(*GetMi								)(BaseView *bv);
//	Int32						(*GetProjection				)(BaseView *bv);
//	Bool						(*TestPoint						)(BaseView *bv, Float x, Float y);
//	Bool						(*TestPointZ					)(BaseView *bv, const Vector &p);
//	Bool						(*TestClipping3D			)(BaseView *bv, const Vector &mp, const Vector &rad, const Matrix &mg, Bool *clip2d, Bool *clipz);
//	Bool						(*ClipLine2D					)(BaseView *bv, Vector *p1, Vector *p2);
//	Bool						(*ClipLineZ						)(BaseView *bv, Vector *p1, Vector *p2);
//	Vector					(*WS									)(BaseView *bv, const Vector &p);
//	Vector					(*SW									)(BaseView *bv, const Vector &p);
//	Vector					(*SW_R								)(BaseView *bv, Float x, Float y, const Vector &wp);
//	Vector					(*WC									)(BaseView *bv, const Vector &p);
//	Vector					(*CW									)(BaseView *bv, const Vector &p);
//	Vector					(*SC									)(BaseView *bv, const Vector &p);
//	Vector					(*CS									)(BaseView *bv, const Vector &p, Bool z_inverse);
//	Vector					(*WC_V								)(BaseView *bv, const Vector &v);
//	Vector					(*CW_V								)(BaseView *bv, const Vector &v);
//	Bool						(*BackfaceCulling			)(BaseView *bv, const Vector &n, const Vector &p);
//	Bool						(*ZSensitive					)(BaseView *bv);
//
//	ViewportSelect* (*VSAlloc   					)();
//	void            (*VSFree    					)(ViewportSelect *&p);
//	Bool						(*VSInitObj 					)(ViewportSelect *vs, Int32 w, Int32 h, BaseDraw* bd, BaseObject* op, Int32 mode, Bool onlyvisible, VIEWPORTSELECTFLAGS flags);
//	Bool						(*VSInitAr  					)(ViewportSelect *vs, Int32 w, Int32 h, BaseDraw* bd, AtomArray* ar, Int32 mode, Bool onlyvisible, VIEWPORTSELECTFLAGS flags);
//	ViewportPixel*	(*VSGetPixelInfoPoint )(ViewportSelect *vs, Int32 x, Int32 y);
//	ViewportPixel*	(*VSGetPixelInfoPolygon)(ViewportSelect *vs, Int32 x, Int32 y);
//	ViewportPixel*	(*VSGetPixelInfoEdge  )(ViewportSelect *vs, Int32 x, Int32 y);
//	void            (*VSShowHotspot				)(ViewportSelect *p, EditorWindow *bw, Int32 x, Int32 y);
//	void            (*VSSetBrushRadius		)(ViewportSelect *p, Int32 r);
//	void          	(*VSClearPixelInfo    )(ViewportSelect *vs, Int32 x, Int32 y, UChar mask);
//	Bool						(*VSGetCameraCoordinates)(ViewportSelect *vs, Float x, Float y, Float z, Vector &v);
//	Float						(*ZSensitiveNearClipping)(BaseView *bv);
//	Bool						(*VSDrawPolygon				)(ViewportSelect *vs, const Vector* p, Int32 ptcnt, Int32 i, BaseObject* op, Bool onlyvisible);
//	Bool						(*VSDrawHandle				)(ViewportSelect *vs, const Vector& p, Int32 i, BaseObject* op, Bool onlyvisible);
//	Int32						(*GetFrameScreen			)(BaseDraw *bv, Int32 *cl, Int32 *ct, Int32 *cr, Int32 *cb);
//	const Matrix4&	(*GetViewMatrix				)(BaseDraw *bv, Int32 n);
//	ViewportPixel*	(*VSGetNearestPoint   )(ViewportSelect *vs, BaseObject* op, Int32 &x, Int32 &y, Int32 maxrad, Bool onlyselected, Int32* ignorelist, Int32 ignorecnt);
//	ViewportPixel*	(*VSGetNearestPolygon )(ViewportSelect *vs, BaseObject* op, Int32 &x, Int32 &y, Int32 maxrad, Bool onlyselected, Int32* ignorelist, Int32 ignorecnt);
//	ViewportPixel*	(*VSGetNearestEdge    )(ViewportSelect *vs, BaseObject* op, Int32 &x, Int32 &y, Int32 maxrad, Bool onlyselected, Int32* ignorelist, Int32 ignorecnt);
//	void            (*VSShowHotspotS			)(EditorWindow *bw, Int32 x, Int32 y, Int32 rad, Bool bRemove);
//	Bool						(*VSPickObject				)(BaseDraw* bd, BaseDocument* doc, Int32 x, Int32 y, Int32 rad, Bool allowOpenGL, LassoSelection* ls, C4DObjectList* list, Matrix4* m);
//	AnaglyphCameraInfo*	(*GetAnaglyphInfo	)(const BaseView* bd);
//	void						(*OverrideCamera			)(BaseDraw* bd, AnaglyphCameraInfo* si);
//};
//
//struct C4D_Pool
//{
//	MemoryPool*			(*Alloc								)(VLONG block_size);
//	void						(*Free								)(MemoryPool *pool);
//	void*						(*AllocElement				)(MemoryPool *pool, VLONG size, Bool clear);
//	void						(*FreeElement					)(MemoryPool *pool, void *mem, VLONG size);
//	void*						(*AllocElementS				)(MemoryPool *pool, VLONG size, Bool clear);
//	void						(*FreeElementS				)(MemoryPool *pool, void *mem);
//};
//
//typedef Int32 (*GeExecuteProgramExCallback)(Int32 cmd, void *userdata, const Filename &logfile);
//
//struct C4D_General
//{
//	void						(*Free								)(void *data);
//	void						(*Print								)(const String &str);
//
//	Bool						(*FExist							)(const Filename *name, Bool isdir);
//	Bool						(*SearchFile					)(const Filename *directory, const Filename *name, Filename *found);
//	Bool						(*FKill								)(const Filename *name, Int32 flags);
//	Bool						(*FCopyFile						)(const Filename *source, const Filename *dest, Int32 flags);
//	Bool						(*FRename							)(const Filename *source, const Filename *dest);
//	Bool						(*FCreateDir					)(const Filename *name);
//	Bool						(*ExecuteFile					)(const Filename *path);
//	const Filename	(*GetStartupPath			)(void);
//	Bool						(*ExecuteProgram      )(const Filename *program, const Filename *file);
//
//	void						(*ShowMouse						)(Int32 v);
//	void						(*GetSysTime					)(Int32 *year, Int32 *month, Int32 *day, Int32 *hour, Int32 *minute, Int32 *second);
//	Int32						(*GetTimer						)(void);
//	void						(*GetLineEnd					)(String *str);
//	Int32						(*GetDefaultFPS				)(void);
//	GEMB_R					(*OutString						)(const String *str, GEMB flags);
//	OPERATINGSYSTEM (*GetCurrentOS			)(void);
//	BYTEORDER			(*GetByteOrder				)(void);
//	void						(*GetGray							)(Int32 *r, Int32 *g, Int32 *b);
//	Bool						(*ChooseColor					)(Vector *col, Int32 flags);
//	void						(*SetMousePointer			)(Int32 l);
//	Bool						(*ShowBitmap1					)(const Filename *fn);
//	Bool						(*ShowBitmap2					)(BaseBitmap *bm);
//	void						(*StopAllThreads			)(void);
//	void						(*StatusClear					)(void);
//	void						(*StatusSetSpin				)(void);
//	void						(*StatusSetBar				)(Int32 p);
//	void						(*StatusSetText				)(const String *str);
//	void						(*SpecialEventAdd			)(Int32 type, VULONG p1, VULONG p2);
//	Bool						(*DrawViews						)(DRAWFLAGS flags, BaseDraw *bd); 
//	void						(*GetGlobalTexturePath)(Int32 i, Filename *fn);
//	void						(*SetGlobalTexturePath)(Int32 i, const Filename *fn);
//	void						(*FlushUnusedTextures	)(void);
//	void						(*GetWorldContainer		)(BaseContainer *bc);
//	void						(*ErrorStringDialog   )(CHECKVALUERANGE type, Float x, Float y, CHECKVALUEFORMAT is);
//
//	void						(*InsertBaseDocument	)(BaseDocument *doc);
//	void						(*SetActiveDocument		)(BaseDocument *doc);
//	BaseDocument*		(*GetActiveDocument		)(void);
//	BaseDocument*		(*GetFirstDocument		)(void);
//	void						(*KillDocument				)(BaseDocument *doc);
//	Bool						(*LoadFile						)(const Filename *name);
//	Bool						(*SaveDocument				)(BaseDocument *doc, const Filename &name, SAVEDOCUMENTFLAGS saveflags, Int32 format);
//	RENDERRESULT 		(*RenderDocument			)(BaseDocument *doc, ProgressHook *pr, void *private_data, BaseBitmap *bmp, const BaseContainer *rdata, RENDERFLAGS renderflags, BaseThread *th);
//	Vector					(*GetColor						)(Int32 i);
//	Bool						(*RegisterPlugin			)(Int32 api_version, PLUGINTYPE type, Int32 id, const String *str,void *data,Int32 datasize);
//	void						(*GetSerialInfo				)(SERIALINFO type, String *s1, String *s2, String *s3, String *s4, String *s5, String *s6);
//	VERSIONTYPE			(*GetVersionType			)(void);
//	Bool						(*ReadPluginInfo			)(Int32 pluginid, Char *buffer, Int32 size);
//	Bool						(*WritePluginInfo			)(Int32 pluginid, Char *buffer, Int32 size);
//
//	void						(*EwDrawXORLine				)(EditorWindow *win, Int32 x1,Int32 y1,Int32 x2,Int32 y2);
//	void						(*EwMouseDragStart		)(EditorWindow *win, Int32 button,Float mx,Float my,MOUSEDRAGFLAGS flag);
//	MOUSEDRAGRESULT (*EwMouseDrag					)(EditorWindow *win, Float *mx,Float *my,BaseContainer *channels);
//	MOUSEDRAGRESULT (*EwMouseDragEnd			)(EditorWindow *win);
//	Bool						(*EwBfGetInputState		)(EditorWindow *win, Int32 askdevice,Int32 askchannel,BaseContainer *res);
//	Bool						(*EwBfGetInputEvent		)(EditorWindow *win, Int32 askdevice,BaseContainer *res);
//
//	Bool						(*RegistryAdd         )(Int32 sub_id, REGISTRYTYPE main_id, void *data);
//	Bool						(*RegistryRemove      )(Int32 sub_id, REGISTRYTYPE main_id);
//	Registry*				(*RegistryFind				)(Int32 sub_id, REGISTRYTYPE main_id);
//	Registry*				(*RegistryGetLast			)(REGISTRYTYPE main_id);
//	Registry*				(*RegistryGetFirst		)(REGISTRYTYPE main_id);
//	Bool						(*RegistryGetAutoID		)(Int32 *id);
//	Bool						(*RegistryGetData			)(Registry *reg, REGISTRYTYPE *main_id, Int32 *sub_id, void **data);
//
//	void*						(*Alloc								)(VLONG size,Int32 line,const Char *file);
//	BaseContainer*	(*GetWorldPluginData  )(Int32 id);
//	Bool						(*SetWorldPluginData  )(Int32 id, const BaseContainer *bc, Bool add);
//	Bool            (*SyncMessage         )(Int32 message, Int32 core_id, VLONG par1, VLONG par2);
//	void						(*SetWorldContainer		)(const BaseContainer *bc);
//	Bool						(*PluginMessage				)(Int32 id, void *data);
//
//	BasePlugin*			(*FindPlugin										)(Int32 id, PLUGINTYPE type);
//	BasePlugin*			(*GetFirstPlugin								)(void);
//	void*						(BasePlugin::*GetPluginStructure)();
//	Filename				(BasePlugin::*GetFilename				)(void);
//	Int32						(BasePlugin::*GetID							)(void) const;
//	Int32						(BasePlugin::*GetInfo						)(void) const;
//
//	Bool						(*ChooseFont					)(BaseContainer *col);
//
//	void						(*GeDebugBreak				)(Int32 line, const Char *file);
//	void						(*GeDebugOut					)(const Char* s,...);
//	Bool						(*RenameDialog				)(String *str);
//	Bool						(*OpenHTML						)(const String &webaddress);
//	Bool						(*SendModelingCommand )(Int32 command, ModelingCommandData &data);
//
//	void						(*EventAdd						)(EVENT flags);
//	void						(*FindInManager				)(BaseList2D *bl);
//
//	CUSTOMDATATYPEPLUGIN*		(*FindCustomDataTypePlugin		)(Int32 type);
//	RESOURCEDATATYPEPLUGIN*	(*FindResourceDataTypePlugin	)(Int32 type);
//
//	void						(*GeSleep							)(Int32 milliseconds);
//	GeData					(*SendCoreMessage			)(Int32 coreid, const BaseContainer &msg, Int32 eventid);
//	Bool						(*CheckIsRunning			)(CHECKISRUNNING type);
//	BaseContainer*	(*GetWorldContainerInstance)(void);
//
//	Bool						(*GenerateTexturePath )(const Filename &docpath, const Filename &srcname, const Filename &suggestedpath, Filename *dstname);
//	Bool						(*IsInSearchPath			)(const Filename &texfilename, const Filename &docpath);
//
//	BaseContainer*	(*GetToolPluginData   )(BaseDocument *doc, Int32 id);
//	Bool						(*IsMainThread				)(void);
//
//
//	Filename				(*GetDefaultFilename  )(Int32 id);
//
//	Bool						(*AddBackgroundHandler					)(BackgroundHandler *handler, void *tdata, Int32 typeclass, Int32 priority);
//	Bool						(*RemoveBackgroundHandler				)(void *tdata, Int32 typeclass);
//	void						(*StopBackgroundThreads					)(Int32 typeclass, BACKGROUNDHANDLERFLAGS flags);
//	Bool						(*CheckBackgroundThreadsRunning	)(Int32 typeclass, Bool all);
//	void						(*ProcessBackgroundThreads			)(Int32 typeclass);
//
//	void						(*FlushTexture									)(const Filename *docpath, const String *name, const Filename &suggestedfolder);
//
//	Bool						(*GetMovieInfo									)(const Filename *name, Int32 *frames, Float *fps);
//	String          (*GetObjectName                 )(Int32 type);
//	String          (*GetTagName                    )(Int32 type);
//	Int32            (*GetObjectType                 )(const String &name);
//	Int32            (*GetTagType                    )(const String &name);
//
//	void            (*CopyToClipboard               )(const String &str);
//
//	void*						(*AllocNC												)(VLONG size,Int32 line,const Char *file);
//	BaseContainer*	(*GetToolData										)(BaseDocument *doc,Int32 pluginid);
//	Bool						(*GeGetMemoryStat								)(BaseContainer &stat);
//	Bool						(*PopupEditText									)(Int32 screenx,Int32 screeny, Int32 width, Int32 height,const String &changeme,Int32 flags,PopupEditTextCallback *func, void *userdata);
//
//	Bool						(*EWScreen2Local								)(EditorWindow *win, Int32 *x, Int32 *y);
//	Bool						(*EWLocal2Screen								)(EditorWindow *win, Int32 *x, Int32 *y);
//
//	void						(*StartEditorRender							)(Bool active_only, Bool raybrush, Int32 x1, Int32 y1, Int32 x2, Int32 y2, BaseThread *bt, BaseDraw *bd, Bool newthread);
//
//	GeData					(*StringToNumber								)(const String &text, Int32 format, Int32 fps, const LENGTHUNIT *unit);
//
//	Bool						(*IsActiveToolEnabled						)();
//	SYSTEMINFO			(*GetSystemInfo									)(void);
//	Bool						(*PrivateSystemFunction01				)(void *par1);
//	Bool						(*GetLanguage										)(Int32 index, String *extension, String *name, Bool *default_language);
//
//	GeListHead*			(*GetScriptHead									)(Int32 type);
//	Int32						(*GetDynamicScriptID						)(BaseList2D *bl);
//	Float						(*GetToolScale									)(BaseDraw* bd, AtomArray* arr, Bool all, Int32 mode);
//	Bool						(*GetCommandLineArgs						)(C4DPL_CommandLineArgs &args);
//	Bool						(*FilterPluginList							)(AtomArray &arr, PLUGINTYPE type, Bool sortbyname);
//
//	Bool						(*LoadDocument									)(BaseDocument *doc, const Filename &name, SCENEFILTER loadflags, BaseThread *bt);
//	void						(*FrameScene										)(BaseDocument *doc);
//	IDENTIFYFILE		(*IdentifyFile									)(const Filename &name, UChar *probe, Int32 probesize, IDENTIFYFILE recognition, BasePlugin **bp);
//	const Filename	(*GetC4DPath										)(Int32 whichpath);
//
//	Bool						(*FMove													)(const Filename &source, const Filename &dest);
//
//	Bool						(*HandleViewCommand							)(Int32 command_id, BaseDocument *doc, BaseDraw *bd, Int32 *value);
//
//	Bool						(*AddUndoHandler								)(BaseDocument *doc, void *dat, UNDOTYPE type);
//
//	String					(*GeGetDegreeChar               )();
//	String					(*GeGetPercentChar              )();
//	Bool						(*HandleCommand									)(Int32 command_id, Int32 *value);
//
//	void						(*lSwap  												)(void *adr, VLONG cnt);
//	void						(*wSwap  												)(void *adr, VLONG cnt);
//	void						(*lIntel  											)(void *adr, VLONG cnt);
//	void						(*wIntel  											)(void *adr, VLONG cnt);
//	void						(*lMotor  											)(void *adr, VLONG cnt);
//	void						(*wMotor  											)(void *adr, VLONG cnt);
//	void						(*llSwap												)(void *adr, VLONG cnt);
//	void						(*llIntel												)(void *adr, VLONG cnt);
//	void						(*llMotor												)(void *adr, VLONG cnt);
//
//	void*						(*GeCipher256Open								)(const UChar *key, Int32 klength, Bool stream);
//	void						(*GeCipher256Close							)(void* h);
//
//	IpConnection*		(*IpWaitForIncoming							)(IpConnection* listener, BaseThread* connection, Int32 *ferr);
//	void						(*IpCloseConnection							)(IpConnection* ipc);
//	void						(*IpKillConnection							)(IpConnection *ipc);
//	VLONG						(*IpBytesInInputBuffer					)(IpConnection* ipc);
//	VLONG						(*IpReadBytes										)(IpConnection* ipc, void* buf, VLONG size);
//	VLONG						(*IpSendBytes										)(IpConnection* ipc, void* buf, VLONG size);
//	void						(*IpGetHostAddr									)(IpConnection* ipc, Char *buf, Int32 bufsize);
//	void						(*IpGetRemoteAddr								)(IpConnection* ipc, Char *buf, Int32 bufsize);
//
//	Bool						(*RecordCommand									)(Int32 command_id, Int32 flags, const String &str);
//
//	Bool						(*SendMailAvailable							)();
//	Bool						(*SendMail											)(const String &t_subject, const String *t_to, const String *t_cc, const String *t_bcc, Filename *t_attachments,const String &t_body, Int32 flags);
//	Bool						(*GetSystemEnvironmentVariable  )(const String &varname, String &result);
//	Bool						(*CallHelpBrowser								)(const String &optype, const String &main, const String &group, const String &property);
//	String					(*FormatNumber                  )(const GeData &val, Int32 format, Int32 fps, Bool bUnit, LENGTHUNIT *unit);
//
//	void			      (*BuildGlobalTagPluginContainer	 )(BaseContainer *plugincontainer,Int32 *id);
//	Int32			      (*ResolveGlobalTagPluginContainer)(Int32 *id);
//	Vector					(*GetGuiWorldColor							)(Int32 cid);
//
//	Int32						(*GetShortcutCount							)();
//	BaseContainer		(*GetShortcut										)(Int32 index);
//	Bool						(*AddShortcut										)(const BaseContainer &bc);
//	Bool						(*RemoveShortcut								)(Int32 index);
//	Bool						(*LoadShortcutSet								)(const Filename &fn, Bool add);
//	Bool						(*SaveShortcutSet								)(const Filename &fn);
//	Int32						(*FindShortcutsFromID						)(Int32 pluginid, Int32 *indexarray, Int32 maxarrayelements);
//	Int32						(*FindShortcuts									)(const BaseContainer &scut, Int32 *pluginidarray, Int32 maxarrayelements);
//	void						(*SetViewColor									)(Int32 colid, const Vector &col);
//
//	void						(*RemovePlugin									)(BasePlugin *plug);
//
//	Bool						(*GetAllocSize									)( void *p, VLONG *out_size );
//	void            (*InsertCreateObject            )(BaseDocument *doc, BaseObject *op, BaseObject *activeobj);
//
//	void						(*GeCipher256Encrypt						)(void *h, UChar *mem, Int32 size);
//	void						(*GeCipher256Decrypt						)(void *h, UChar *mem, Int32 size);
//
//	IpConnection*		(*IpOpenListener								)(ULONG ipAddr, Int32 port, BaseThread* thread, Int32 timeout, Bool dontwait, Int32* ferr);
//	IpConnection*		(*IpOpenOutgoing								)(Char* hostname, BaseThread* thread, Int32 initial_timeout, Int32 timeout, Bool dontwait, Int32* ferr);
//	String					(*DateToString									)(const LocalFileTime &t, Bool date_only);
//	Bool						(*ShowInFinder									)(const Filename &fn, Bool open);
//
//	Bool						(*WriteLayout										)(const Filename &fn);
//	Bool						(*WritePreferences							)(const Filename &fn);
//	Bool						(*SaveProjectCopy								)(BaseDocument *t_doc, const Filename &directory, Bool allow_gui);
//
//	Int32						(*ShowPopupMenu									)(CDialog *parent,Int32 screenx,Int32 screeny,const BaseContainer *bc,Int32 flags, Int32 *res_mainid);
//
//	Bool						(*AskForAdministratorPrivileges	)(const String &msg, const String &caption, Bool bAllowSuperUser, void **token);
//	void						(*EndAdministratorPrivileges		)();
//	void						(*RestartApplication						)(const UWORD* param, Int32 exitcode, const UWORD** path);
//	const Filename	(*GetStartupApplication					)(void);
//
//	Bool						(*EWGlobal2Local								)(EditorWindow *win, Int32 *x, Int32 *y);
//	Bool						(*EWLocal2Global								)(EditorWindow *win, Int32 *x, Int32 *y);
//
//	Bool						(*RequestFileFromServer					)(const Filename &fn, Filename &res);
//	Bool						(*ReadPluginReg									)(Int32 pluginid, Char *buffer, Int32 size);
//	Bool						(*WritePluginReg								)(Int32 pluginid, Char *buffer, Int32 size);
//	Bool						(*GeFGetDiskFreeSpace						)(const Filename &vol, LULONG &freecaller, LULONG &total, LULONG &freespace);
//	ULONG						(*GeFGetAttributes							)(const Filename *name);
//	Bool						(*GeFSetAttributes							)(const Filename *name, ULONG flags, ULONG mask);
//
//	void						(*BrowserLibraryPopup						)(Int32 mx, Int32 my, Int32 defw, Int32 defh, Int32 pluginwindowid, Int32 presettypeid, void *userdata, BrowserPopupCallback callback);
//	Bool						(*GeExecuteProgramEx						)(const Filename &program, const String *args, Int32 argcnt, GeExecuteProgramExCallback callback, void *userdata);
//	LReal						(*GeGetMilliSeconds							)(void);
//
//	void*						(*ReallocNC											)(void *old_data, VLONG new_size, Int32 line, const Char *file);
//	Bool						(*GeGetAllocatorStatistics			)(BaseContainer &stat, void *in_allocator);
//	VULONG					(*GeMemGetFreePhysicalMemoryEstimate)(void);
//	void            (*CopyToClipboardB              )(BaseBitmap *map, Int32 ownerid);
//	Bool            (*GetStringFromClipboard        )(String *txt);
//	Bool            (*GetBitmapFromClipboard        )(BaseBitmap *map);
//	CLIPBOARDTYPE   (*GetClipboardType              )(void);
//
//	void						(*EndGlobalRenderThread					)(Bool external_only);
//	Int32						(*GeDebugSetFloatingPointChecks	)(Int32 on);
//	Int32						(*GetC4DClipboardOwner					)(void);
//	void						(*GeCheckMem										)(void *memptr);
//
//	Bool						(*GetFileTime										)(const Filename &name, Int32 mode, LocalFileTime *out);
//	Bool						(*SetFileTime										)(const Filename &name, Int32 mode, const LocalFileTime *in);
//	void						(*GetCurrentTime								)(LocalFileTime *out);
//
//	void						(*GeUpdateUI										)();
//	Float						(*CalculateTranslationScale			)(const UnitScaleData *src_unit, const UnitScaleData *dst_unit);
//	Int32						(*CheckPythonColor							)(const String &txt);
//
//	void						(*PrintNoCR											)(const String &str);
//};
//
//struct C4D_Link
//{
//	BaseLink*				(*Alloc								)(void);
//	void						(*Free								)(BaseLink *link);
//	BaseList2D*			(*GetLink							)(const BaseLink *link, const BaseDocument *doc, Int32 instanceof);
//	void						(*SetLink							)(BaseLink *link, C4DAtomGoal *list);
//	Bool						(*Read								)(BaseLink *link, HyperFile *hf);
//	Bool						(*Write								)(const BaseLink *link, HyperFile *hf);
//	BaseLink*				(*GetClone						)(const BaseLink *link, COPYFLAGS flags, AliasTrans *trn);
//	Bool						(*CopyTo							)(const BaseLink *src, BaseLink *dst, COPYFLAGS flags, AliasTrans *trn);
//	AliasTrans*			(*TrnAlloc						)(void);
//	Bool						(*TrnInit							)(AliasTrans *trn, const BaseDocument *doc);
//	void						(*TrnFree							)(AliasTrans *trn);
//	void						(*TrnTranslate				)(AliasTrans *trn, Bool connect_oldgoals);
//	BaseList2D*			(*ForceGetLink				)(const BaseLink *link);
//	Bool						(*IsCacheLink					)(const BaseLink *link);
//	C4DAtomGoal*		(*GetLinkAtom					)(const BaseLink *link, const BaseDocument *doc, Int32 instanceof);
//	C4DAtomGoal*		(*ForceGetLinkAtom		)(const BaseLink *link);
//};
//
//struct C4D_Neighbor
//{
//	EnumerateEdges*	(*Alloc								)(Int32 pcnt, const CPolygon *vadr, Int32 vcnt, BaseSelect *bs);
//	void						(*Free								)(EnumerateEdges *nb);
//	void						(*GetEdgePolys				)(EnumerateEdges *nb, Int32 a, Int32 b,Int32 *first,Int32 *second);
//	void						(*GetPointPolys				)(EnumerateEdges *nb, Int32 pnt, Int32 **dadr, Int32 *dcnt);
//	Int32						(*GetEdgeCount				)(EnumerateEdges *nb);
//	PolyInfo*				(*GetPolyInfo					)(EnumerateEdges *nb, Int32 poly);
//	Bool    				(*GetNGons  					)(EnumerateEdges *nb, PolygonObject* op, Int32 &ngoncnt, NgonNeighbor *&ngons);
//	void						(*ResetAddress				)(EnumerateEdges *nb, const CPolygon *a_polyadr);
//};
//
//struct C4D_Painter
//{
//	void*						(*SendPainterCommand  )(Int32 command, BaseDocument *doc, PaintTexture *tex, const BaseContainer *bc);
//	Bool						(*CallUVCommand				)(const Vector *padr, Int32 PointCount, const CPolygon *polys, Int32 lPolyCount, UVWStruct *uvw, BaseSelect *polyselection,
//																						BaseSelect* pointselection, BaseObject*op, Int32 mode, Int32 cmdid, const BaseContainer &settings);
//
//	TempUVHandle*		(*GetActiveUVSet			)(BaseDocument* doc, Int32 flags);
//	void						(*FreeActiveUVSet			)(TempUVHandle *handle);
//
//	Int32						(*UVSetGetMode				)(TempUVHandle *handle);
//	const Vector*		(*UVSetGetPoint				)(TempUVHandle *handle);
//	Int32						(*UVSetGetPointCount	)(TempUVHandle *handle);
//	const CPolygon*	(*UVSetGetPoly				)(TempUVHandle *handle);
//	Int32						(*UVSetGetPolyCount		)(TempUVHandle *handle);
//	UVWStruct*			(*UVSetGetUVW					)(TempUVHandle *handle);
//	BaseSelect*			(*UVSetGetPolySel			)(TempUVHandle *handle);
//	BaseSelect*			(*UVSetGetPointSel		)(TempUVHandle *handle);
//	BaseObject*			(*UVSetGetBaseObject	)(TempUVHandle *handle);
//
//	Bool						(*UVSetSetUVW					)(TempUVHandle *handle, UVWStruct *uv);
//
//	Bool						(*Private1						)(Int32 lCommand, AtomArray* pArray, BaseSelect **polyselection,BaseContainer& setttings, BaseThread* th);
//
//	PaintTexture*		(*CreateNewTexture		)(const Filename &path, const BaseContainer &settings);
//	Bool						(*GetTextureDefaults	)(Int32 channel,BaseContainer &settings);
//
//	Bool						(*UVSetIsEditable			)(TempUVHandle *handle);
//
//	Int32						(*IdentifyImage				)(const Filename &texpath);
//	Bool						(*BPSetupWizardWithParameters)(BaseDocument *doc, const BaseContainer &settings, AtomArray &objects, AtomArray &material);
//
//	Bool						(*CalculateTextureSize)(BaseDocument *doc, AtomArray &materials, TextureSize *&sizes);
//
//	Int32						(*PB_GetBw)(PaintBitmap *bmp);
//	Int32						(*PB_GetBh)(PaintBitmap *bmp);
//	PaintLayer*			(*PB_GetLayerDownFirst)(PaintBitmap *tex);
//	PaintLayer*			(*PB_GetLayerDownLast)(PaintBitmap *tex);
//	PaintLayerBmp*	(*PT_AddLayerBmp)(PaintTexture *tex,PaintLayer *insertafter,PaintLayer *layerset,COLORMODE mode,Bool useundo, Bool activate);
//	GeListHead*			(*PT_GetPaintTextureHead)();
//	Bool						(*PLB_ImportFromBaseBitmap)(PaintLayerBmp *layer,BaseBitmap *bmp, Bool usealpha);
//	Bool						(*PLB_ImportFromBaseBitmapAlpha)(PaintLayerBmp *layer,BaseBitmap *bmp,BaseBitmap *channel);
//	Bool						(*PLB_GetPixelCnt)(PaintLayerBmp *layer,Int32 x,Int32 y,Int32 num,PIX *dst,COLORMODE dstmode,PIXELCNT flags);
//	PaintTexture*		(*GetPaintTextureOfBaseChannel)(BaseDocument *doc,BaseChannel *bc);
//
//	LayerSet*				(*LSL_Alloc)();
//	void						(*LSL_Free)(LayerSet *layerset);
//	Bool						(*LSL_Content)(const LayerSet *l);
//	Bool						(*LSL_IsLayerEnabled)(const LayerSet *l,const String &name);
//	Bool						(*LSL_FindLayerSet)(const LayerSet *l,const String &name);
//	String					(*LSL_GetName)(const LayerSet *l);
//	LAYERSETMODE		(*LSL_GetMode)(const LayerSet *l);
//	void						(*LSL_SetMode)(LayerSet *l,LAYERSETMODE t_mode);
//	void						(*LSL_RemoveLayer)(LayerSet *l,const String &layer);
//	void						(*LSL_AddLayer)(LayerSet *l,const String &layer);
//	void						(*LSL_FlushLayers)(LayerSet *l);
//	Bool						(*LSL_operator_cmp)(const LayerSet *l,const LayerSet &layerset);
//	void						(*LSL_CopyTo)(const LayerSet *l,LayerSet &dst);
//	Bool						(*GetAllStrings_AddTexture)(const void *msgdata, const BaseContainer &d);
//
//	PaintTexture*		(*PB_GetPaintTexture)(PaintBitmap *bmp);
//	PaintBitmap*		(*PB_GetParent)(PaintBitmap *bmp);
//	PaintLayer*			(*PB_GetAlphaFirst)(PaintBitmap *bmp);
//	PaintLayer*			(*PB_GetAlphaLast)(PaintBitmap *bmp);
//	PaintLayerBmp*	(*PB_AddAlphaChannel)(PaintBitmap *bmp,Int32 bitdepth,PaintLayer *prev,Bool undo, Bool activate);
//	Bool						(*PB_AskApplyAlphaMask)(PaintBitmap *bmp);
//	void						(*PB_ApplyAlphaMask)(PaintBitmap *bmp,Int32 x,Int32 y,Int32 num,PIX *bits,Int32 mode,Bool inverted, Int32 flags);
//	PaintLayerMask*	(*PB_FindSelectionMask)(PaintBitmap *bmp,PaintBitmap **toplevel,Int32 *bitdepth);
//	Int32						(*PB_GetColorMode)(PaintBitmap *bmp);
//	ULONG						(*PB_GetDirty)(PaintBitmap *bmp,DIRTYFLAGS flags);
//	void						(*PB_UpdateRefresh)(PaintBitmap *bmp,Int32 xmin,Int32 ymin,Int32 xmax,Int32 ymax,ULONG flags);
//	void						(*PB_UpdateRefreshAll)(PaintBitmap *bmp,ULONG flags,Bool reallyall);
//	Bool						(*PB_ReCalc)(PaintBitmap *bmpthis,BaseThread *thread,Int32 x1,Int32 y1,Int32 x2,Int32 y2,BaseBitmap *bmp,Int32 flags,ULONG showbit);
//	Bool						(*PB_ConvertBits)(Int32 num,const PIX *src,Int32 srcinc,COLORMODE srcmode,PIX *dst,Int32 dstinc,COLORMODE dstmode,Int32 dithery,Int32 ditherx);
//	Bool						(*PLB_SetPixelCnt)(PaintLayerBmp *layer,Int32 x,Int32 y,Int32 num,const PIX *src,Int32 incsrc,COLORMODE srcmode,PIXELCNT flags);
//	void						(*PLB_GetBoundingBox)(PaintLayerBmp *layer,Int32 &x1,Int32 &y1,Int32 &x2,Int32 &y2, Bool hasselectionpixels);
//	PaintLayerFolder*(*PT_AddLayerFolder)(PaintTexture *tex,PaintLayer *insertafter,PaintLayer *insertunder,Bool useundo, Bool activate);
//	void						(*PT_SetActiveLayer)(PaintTexture *tex,PaintLayer *layer,Bool activatetexture,Bool show);
//	PaintLayer*			(*PT_GetActive)(PaintTexture *tex);
//	void						(*PT_GetLinkLayers)(PaintTexture *tex,AtomArray &layers, Bool addfolders);
//	Bool						(*PT_SetSelected_Texture)(PaintBitmap *bmp, PaintMaterial *preferred);
//	PaintTexture*		(*PT_GetSelectedTexture)();
//	PaintTexture*		(*PT_GetSelectedTexturePP)(PaintMaterial **ppmat);
//	void						(*PM_EnableMaterial)(PaintMaterial *,BaseDocument *doc,Bool on,Bool suppressevent,Bool domaterialundo);
//	PaintMaterial*	(*PM_GetActivePaintMaterial)(BaseDocument *doc,BaseMaterial **mat);
//	PaintMaterial*	(*PM_GetPaintMaterialFromTexture)(PaintTexture *tex,Bool onlyeditable);
//	PaintMaterial*	(*PM_GetPaintMaterial)(BaseDocument *dok,BaseMaterial *material,Bool create);
//	Bool						(*PM_UnloadPaintMaterial)(BaseDocument *doc,BaseMaterial *material,Bool forcesave);
//	Bool						(*PT_SetColorMode)(PaintTexture *tex,COLORMODE newcolormode,Bool doundo);
//
//	void						(*LSL_SetPreviewMode)(LayerSet *l, Int32 mode);
//	Int32						(*LSL_GetPreviewMode)(const LayerSet *l);
//	LayerSetHelper *(*LSH_Alloc)();
//	void						(*LSH_Free)(LayerSetHelper *lsh);
//	Bool						(*LSH_Init)(LayerSetHelper *lsh, const Filename &fn, const LayerSet *selection);
//	Bool						(*LSH_EditLayerSet)(LayerSetHelper *lsh,const String &dialogtitle, LayerSet *layerset, LayerSet *layerset_a);
//
//	Bool						(*CLL_CalculateResolution)(BaseDocument *doc, const Filename &filename, Int32 *xres, Int32 *yres);
//	Bool						(*CLL_CalculateFilename)(BaseDocument *doc, Filename &fn, LayerSet *lsl);
//
//	Bool						(*PL_GetShowBit)(PaintLayer *bmp,Bool hierarchy, ULONG bit);
//	Bool						(*PL_SetShowBit)(PaintLayer *bmp,Bool onoff, ULONG bit);
//	PaintTexture*		(*PT_CreateNewTextureDialog)(String &result,Filename &resultdirectory,Int32 channelid,BaseMaterial *bmat);
//	void						(*PN_ActivateChannel)(Int32 channel, Bool multi, Bool enable);
//	const Filename  (*PT_GetFilename)(PaintTexture *tex);
//
//	Bool						(*LSH_MergeLayerSet)(LayerSetHelper *lsh, const LayerSet &selection, BaseBitmap *bmp, Bool preview);
//};
//
//struct C4D_GLSL
//{
//	void						(GlString::*SDKInit1)();
//	void						(GlString::*SDKInit2)(const char* pchString);
//	void						(GlString::*SDKInit3)(const GlString& str);
//	void						(GlString::*SDKInit4)(Int32 n);
//	void						(GlString::*SDKInit5)(Float r, const char* pszFormat);
//	void						(GlString::*SDKFree)();
//
//	const GlString&	(GlString::*SDKAssign1)(const GlString &str);
//	const GlString&	(GlString::*SDKAssign2)(const char* pszString);
//
//	GlString				(*GlStringAdd1)(const GlString &str1, const GlString &str2);
//	GlString				(*GlStringAdd2)(const GlString &str1, const char* str2);
//
//	const GlString&	(GlString::*GlStringAdd3)(const GlString &str);
//	const GlString&	(GlString::*GlStringAdd4)(const char* str);
//
//	const char*			(GlString::*GetCString)() const;
//	VLONG						(GlString::*GetLength)() const;
//	Int32						(GlString::*Compare)(const GlString &s) const;
//	Int32						(GlString::*Replace)(const GlString& strSearch, const GlString &strReplace, VLONG lStart, Bool bReplaceAll, Bool bOnlyWord);
//
//	// GlProgramFactory
//	GlProgramFactory* (*GetFactory)(const BaseDraw* pBaseDraw, const C4DAtom* pObj, LULONG ulIndex, GlProgramFactoryMessageCallback fnCallback, void* pIdentity, VLONG lIdentityLength,
//		const GlLight* const* ppLights, Int32 lLightCount, LULONG ulFlags,
//		const GlVertexBufferAttributeInfo* const* ppBufferAttributeInfo, Int32 lBufferAttributeInfoCount,
//		const GlVertexBufferVectorInfo* const* ppBufferVectorInfo, Int32 lBufferVectorInfoCount,
//		GlFBAdditionalTextureInfo* pAdditionalTextures);
//	void						(*RemoveReference)(const C4DAtom* pObj, LULONG lIndex);
//	void						(*RemoveTextureReferenceA)(const C4DAtom* pObj, Int32 lIndex);
//	void						(*RemoveTextureReferenceB)(const Filename &fn);
//	void*						(*IncreaseBufferSize)(GlGetIdentity* pIdentity, VLONG lNeededSize, Int32 lLine, const char* pszFile);
//	ULONG						(GlProgramFactory::*Init)(Int32 lSubdivideCount);
//
//	Bool						(GlProgramFactory::*BindToView)(const BaseDraw* pDraw);
//	Bool						(GlProgramFactory::*CompilePrograms)();
//	Bool						(GlProgramFactory::*BindPrograms)();
//	Bool						(GlProgramFactory::*UnbindPrograms)();
//	Bool						(GlProgramFactory::*DestroyPrograms)(Bool bChangeContext);
//	void						(GlProgramFactory::*LockFactory)();
//	void						(GlProgramFactory::*UnlockFactory)();
//	void*						(GlProgramFactory::*GetPrivateData)(const void* pObj, Int32 lDataIndex, GlProgramFactoryAllocPrivate fnAlloc, GlProgramFactoryFreePrivate fnFree);
//	void*						(GlProgramFactory::*GetDescriptionData)(Int32 lObjIndex, Int32 lDataIndex, GlProgramFactoryAllocDescription fnAlloc, GlProgramFactoryFreeDescription fnFree);
//	Bool						(GlProgramFactory::*IsProgram)(GlProgramType t);
//
//	void						(GlProgramFactory::*InitSetParameters)();
//	void						(GlProgramFactory::*SetScreenCoordinates)(BaseDraw* pBaseDraw);
//	void						(GlProgramFactory::*AddErrorHandler)(GlProgramFactoryErrorHandler fn);
//	Bool						(GlProgramFactory::*SetParameterMatrixTransform)(GlProgramParameter param);
//	Bool						(GlProgramFactory::*SetParameterMatrix1)(GlProgramParameter param, const SMatrix4 &m);
//	Bool						(GlProgramFactory::*SetParameterMatrix2)(GlProgramParameter paramOffset, GlProgramParameter paramAxes, GlProgramParameter paramNormalTrans, const SMatrix &m);
//	Bool						(GlProgramFactory::*SetParameterMatrix3x3)(GlProgramParameter param, const Float32* r);
//	Bool						(GlProgramFactory::*SetParameterMatrix4x4)(GlProgramParameter param, const Float32* r);
//	Bool						(GlProgramFactory::*SetParameterVector1)(GlProgramParameter param, const SVector &v);
//	Bool						(GlProgramFactory::*SetParameterVector2)(GlProgramParameter param, const SVector4 &v);
//	Bool						(GlProgramFactory::*SetParameterVector3)(GlProgramParameter param, const SVector &v, Float32 w);
//	Bool						(GlProgramFactory::*SetParameterColor1)(GlProgramParameter param, const Vector &v, Float rBrightness);
//	Bool						(GlProgramFactory::*SetParameterColor2)(GlProgramParameter param, const Vector &v, Float rBrightness, Float32 rAlpha);
//	Bool						(GlProgramFactory::*SetParameterColorReverse1)(GlProgramParameter param, const Vector &v, Float rBrightness);
//	Bool						(GlProgramFactory::*SetParameterColorReverse2)(GlProgramParameter param, const Vector &v, Float rBrightness, Float32 rAlpha);
//	Bool						(GlProgramFactory::*SetParameterReal)(GlProgramParameter param, Float32 r);
//	Bool						(GlProgramFactory::*SetParameterReal2a)(GlProgramParameter param, Float32 a, Float32 b);
//	Bool						(GlProgramFactory::*SetParameterReal2b)(GlProgramParameter param, const Float32* v);
//	Bool						(GlProgramFactory::*SetParameterReal3a)(GlProgramParameter param, Float32 a, Float32 b, Float32 c);
//	Bool						(GlProgramFactory::*SetParameterReal3b)(GlProgramParameter param, const Float32* v);
//	Bool						(GlProgramFactory::*SetParameterReal4a)(GlProgramParameter param, Float32 a, Float32 b, Float32 c, Float32 d);
//	Bool						(GlProgramFactory::*SetParameterReal4b)(GlProgramParameter param, const Float32* v);
//	Bool						(GlProgramFactory::*SetParameterTexture)(GlProgramParameter param, Int32 lDimension, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTextureCube)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTexture2D1)(GlProgramParameter param, const BaseBitmap* pBmp, Int32 lFlags, DRAW_ALPHA alphamode, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTexture2D2)(GlProgramParameter param, const Filename &fn, Int32 lFrame, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, DRAW_ALPHA alphamode, Int32 lMaxSize, LayerSet* pLayerSet, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTexture2D3)(GlProgramParameter param, const BaseBitmap* pBmp, ULONG ulDirty, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, DRAW_ALPHA alphamode, Int32 lMaxSize, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureGradient)(GlProgramParameter param, Gradient* pGradient, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureFunction)(GlProgramParameter param, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, Int32 lDataType, GlProgramFactoryCreateTextureFunctionCallback fn, void* pData, VLONG lDataLen, Int32 lInParams, Int32 lOutParams, Int32 lCycle, Bool bInterpolate, Int32 lSizeX, Int32 lSizeY, Int32 lSizeZ, Bool bParallel, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureRaw)(GlProgramParameter param, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, Int32 lDataType, const void* pData, VLONG lDataLen, ULONG ulDirty, Int32 lDimension, Int32 lComponents, Int32 lCycle, Bool bInterpolate, Int32 lSizeX, Int32 lSizeY, Int32 lSizeZ, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureCubeMap)(GlProgramParameter param, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, Int32 lDataType, GlProgramFactoryCreateTextureFunctionCallback fn, void* pData, VLONG lDataLen,	Int32 lOutParams, Int32 lCycle, Bool bInterpolate, Int32 lSize, Bool bParallel, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterInt)(GlProgramParameter param, int r);
//	Bool						(GlProgramFactory::*SetParameterInt2a)(GlProgramParameter param, int a, int b);
//	Bool						(GlProgramFactory::*SetParameterInt2b)(GlProgramParameter param, const int* v);
//	Bool						(GlProgramFactory::*SetParameterInt3a)(GlProgramParameter param, int a, int b, int c);
//	Bool						(GlProgramFactory::*SetParameterInt3b)(GlProgramParameter param, const int* v);
//	Bool						(GlProgramFactory::*SetParameterInt4a)(GlProgramParameter param, int a, int b, int c, int d);
//	Bool						(GlProgramFactory::*SetParameterInt4b)(GlProgramParameter param, const int* v);
//	Bool						(GlProgramFactory::*SetParameterUint)(GlProgramParameter param, unsigned int r);
//	Bool						(GlProgramFactory::*SetParameterUint2a)(GlProgramParameter param, unsigned int a, unsigned int b);
//	Bool						(GlProgramFactory::*SetParameterUint2b)(GlProgramParameter param, const unsigned int* v);
//	Bool						(GlProgramFactory::*SetParameterUint3a)(GlProgramParameter param, unsigned int a, unsigned int b, unsigned int c);
//	Bool						(GlProgramFactory::*SetParameterUint3b)(GlProgramParameter param, const unsigned int* v);
//	Bool						(GlProgramFactory::*SetParameterUint4a)(GlProgramParameter param, unsigned int a, unsigned int b, unsigned int c, unsigned int d);
//	Bool						(GlProgramFactory::*SetParameterUint4b)(GlProgramParameter param, const unsigned int* v);
//	Bool						(GlProgramFactory::*SetParameterRealArray)(GlProgramParameter param, Int32 lElements, const Float32 *r);
//	Bool						(GlProgramFactory::*SetParameterTextureNormalizeCube)(GlProgramParameter param);
//	Bool						(GlProgramFactory::*SetParameterTexture2DDepth)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTextureCubeDepth)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetLightParameters)(const GlLight* const* pLights, Int32 lLightCount, const SMatrix4& mObject);
//
//	Bool						(GlProgramFactory::*GetCgFXDescription)(BaseContainer* pbcDescription);
//	Bool						(GlProgramFactory::*CompileCgFX)(const Filename& fn, ULONG ulFlags);
//	Bool						(GlProgramFactory::*BindCgFXPrograms)(Int32 lTechnique, Int32 lPass);
//
//	void						(GlProgramFactory::*AddParameters)(ULONG ulParameters, ULONG ulFormat);
//	LULONG					(GlProgramFactory::*GetParameters)() const;
//	ULONG						(GlProgramFactory::*GetParameterFormats)() const;
//	GlString				(GlProgramFactory::*AddUniformParameter1)(GlProgramType t, GlUniformParamType type, const char* pszName);
//	GlString				(GlProgramFactory::*AddUniformParameter2)(GlProgramType t, GlUniformParamType type, Int32 lCount, const char* pszName);
//	Bool						(GlProgramFactory::*HeaderFinished)();
//	Bool						(GlProgramFactory::*AddLightProjection)();
//	void						(GlProgramFactory::*AddLine)(GlProgramType t, const GlString &strLine);
//	void						(GlProgramFactory::*StartLightLoop)();
//	Bool						(GlProgramFactory::*EndLightLoop)();
//	GlString				(GlProgramFactory::*GetUniqueID)();
//	Bool						(GlProgramFactory::*InitLightParameters)();
//	Int32						(GlProgramFactory::*GetMaxLights)();
//	const UChar*		(GlProgramFactory::*GetIdentity)() const;
//	GlProgramParameter		(GlProgramFactory::*GetParameterHandle)(GlProgramType t, const char* pszName) const;
//	GlString				(GlProgramFactory::*AddColorBlendFunction)(GlProgramType t, Int32 lBlendMode);
//	GlString				(GlProgramFactory::*AddRGBToHSVFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddHSVToRGBFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddRGBToHSLFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddHSLToRGBFunction)(GlProgramType t);
//	Bool						(GlProgramFactory::*AddFunction)(GlProgramType t, const GlString &strFunction);
//	const GlString&	(GlProgramFactory::*AddNoiseFunction)(GlProgramType t, Int32 lNoise, Int32 lFlags);
//	Bool						(GlProgramFactory::*AddEncryptedBlock)(GlProgramType t, const char* pchData, VLONG lDataLength, const UChar* pchKey, Int32 lKeyLength);
//	Bool						(GlProgramFactory::*EncryptBlock)(const GlString &strLine, const UChar* pchKey, Int32 lKeyLength, char *&pchData, VLONG &lDataLength, Bool bCStyle);
//	void						(GlProgramFactory::*GetVectorInfo)(Int32 &lVectorCount, const GlVertexBufferVectorInfo* const* &ppVectorInfo) const;
//
//	Bool						(*CacheTextureFn)(BaseDraw* pBaseDraw, const Filename &fn, Int32 lFrame, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, DRAW_ALPHA alphamode, Int32 lMaxSize, GlProgramType progType, LayerSet* pLayerSet, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(*CacheTextureBmp)(BaseDraw* pBaseDraw, const BaseBitmap* pBmp, ULONG ulDirty, C4DAtom* pObj, Int32 lIndex, Int32 lFlags, DRAW_ALPHA alphamode, Int32 lMaxSize, GlProgramType progType, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	ULONG						(*GetLanguageFeatures)(Int32 lCompiler, Int32 lFlags);
//	Bool						(*HasNoiseSupport)(GlProgramType t, Int32 lNoise, BaseDraw* pBaseDraw, Int32 lCompiler);
//	Int32						(GlProgramFactory::*GetCompiler)();
//
//	// GlFrameBuffer
//	GlFrameBuffer*	(*GetFrameBuffer)(BaseDraw* pBaseDraw, VULONG lID1, Int32 lID2, UINT nWidth, UINT nHeight, Int32 lColorTextureCount, Int32 lDepthTextureCount, Int32 lColorCubeCount, Int32 lDepthCubeCount, ULONG ulFlags, Int32 lAAMode, GlFBAdditionalTextureInfo* pAdditionalTextures);
//	void						(*RemoveObjectF)(BaseDraw* pBaseDraw, VULONG lID1, Int32 lID2);
//	GlFrameBuffer*	(*FindFrameBuffer)(BaseDraw* pBaseDraw, VULONG lID1, Int32 lID2);
//
//	void						(GlFrameBuffer::*PrepareForRendering)(Int32 lTexture);
//	void						(GlFrameBuffer::*SetInterpolation)(Int32 lInterpolate, Int32 lTexture);
//	Bool						(GlFrameBuffer::*Activate)(BaseDraw* pBaseDraw);
//	void						(GlFrameBuffer::*Deactivate)(BaseDraw* pBaseDraw);
//	Bool						(GlFrameBuffer::*SetRenderTarget)(Int32 lTexture, Int32 lFlags);
//	void						(GlFrameBuffer::*GetRatios)(Int32 lFlags, Float& rWidth, Float& rHeight);
//	void						(GlFrameBuffer::*GetSize)(Int32 lFlags, UINT &nWidth, UINT &nHeight, Bool bFramesize);
//	C4DGLuint				(GlFrameBuffer::*GetTexture)(Int32 lTexture, Int32 lFlags);
//	Int32						(GlFrameBuffer::*SaveTextureToDisk)(const Filename &fn, Int32 lTexture, Int32 lFlags);
//	Bool						(GlFrameBuffer::*CopyToBitmap)(BaseBitmap* pBmp, Int32 lTexture, Int32 lFlags);
//	void						(GlFrameBuffer::*Clear1)();
//	void						(GlFrameBuffer::*Clear2)(const SVector &vColor, Float32 rAlpha);
//	Bool						(GlFrameBuffer::*DrawBuffer)(Int32 lTexture, Int32 lFlags, Int32 lColorConversion, C4DGLint nConversionTexture, Float32 rAlpha, const SVector &vColor, Float32 xs1, Float32 ys1, Float32 xs2, Float32 ys2, Float32 xd1, Float32 yd1, Float32 xd2, Float32 yd2);
//	Bool						(GlFrameBuffer::*IsNPOTBuffer)();
//	Bool						(GlFrameBuffer::*CopyToBuffer)(void* pBuffer, VLONG lBufferSize, Int32 lTexture, Int32 lFlags);
//	Bool						(GlFrameBuffer::*GetTextureData)(BaseDraw* pBaseDraw, Int32 x1, Int32 y1, Int32 x2, Int32 y2, void* pData, Int32 lTexture, Int32 lFlags);
//	GlFBAdditionalTextureInfo* (GlFrameBuffer::*GetAdditionalTexture)(Int32 lType, void* pBuffer);
//
//	// GlVertexBuffer
//	GlVertexBuffer* (*GetVertexBuffer)(const BaseDraw* pBaseDraw, const C4DAtom* pObj, Int32 lIndex, void* pIdentity, VLONG lIdentityLen, ULONG ulFlags);
//	void						(*RemoveObjectV)(C4DAtom* pObj, Int32 lIndex);
//
//	Bool 						(GlVertexBuffer::*IsDirty)();
//	void 						(GlVertexBuffer::*SetDirty)(Bool bDirty);
//	Bool 						(*DrawSubBuffer)(const BaseDraw* pBaseDraw, const GlVertexSubBufferData* pVertexSubBuffer, const GlVertexSubBufferData* pElementSubBuffer,
//																	Int32 lDrawinfoCount, const GlVertexBufferDrawSubbuffer* pDrawInfo,
//																	Int32 lVectorCount, const GlVertexBufferVectorInfo* const* ppVectorInfo,
//																	GlVertexBufferDrawElementCallback fnCallback, void* pCallbackData);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocSubBuffer)(GlVertexBufferSubBufferType type, VLONG lSize, const void* pData);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocIndexSubBuffer1)(Int32 lCount, ULONG* pulIndex, Int32 lMaxIndex);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocIndexSubBuffer2)(Int32 lCount, const UWORD* puwIndex);
//	Bool 						(GlVertexBuffer::*FlushAllSubBuffers)();
//	Bool 						(GlVertexBuffer::*FreeBuffers)(GlVertexBufferSubBufferType type);
//	Bool 						(GlVertexBuffer::*FreeBuffer)(GlVertexSubBufferData* pBuffer);
//	Bool						(GlVertexBuffer::*MarkAllForDelete)();
//	Bool						(GlVertexBuffer::*MarkBuffersForDelete)(GlVertexBufferSubBufferType type);
//	Bool						(GlVertexBuffer::*MarkBufferForDelete)(GlVertexSubBufferData* pBuffer);
//	Bool						(GlVertexBuffer::*DeleteMarkedBuffers)();
//	Bool						(GlVertexBuffer::*SetSubBufferData)(GlVertexSubBufferData* pSubBuffer, VLONG lSize, const void* pData);
//	Int32						(GlVertexBuffer::*GetSubbufferCount)() const;
//	void*						(GlVertexBuffer::*MapBuffer)(GlVertexSubBufferData* pSubBuffer, GlVertexBufferAccessFlags flags);
//	Bool						(GlVertexBuffer::*UnmapBuffer)(GlVertexSubBufferData* pSubBuffer);
//};
//
//struct OperatingSystem
//{
//	Int32 version;
//
//	C4D_General				*Ge;
//	C4D_Shader				*Sh;
//	C4D_HyperFile			*Hf;
//	C4D_BaseContainer *Bc;
//	C4D_String				*St;
//	C4D_Bitmap				*Bm;
//	C4D_MovieSaver		*Ms;
//	C4D_BaseChannel		*Ba;
//	C4D_Filename			*Fn;
//	C4D_File					*Fl;
//	C4D_BrowseFiles		*Bf;
//	C4D_Dialog				*Cd;
//	C4D_UserArea			*Cu;
//	C4D_Parser				*Pr;
//	C4D_Resource			*Lr;
//	C4D_BaseList			*Bl;
//	C4D_Tag						*Tg;
//	C4D_Object				*Bo;
//	C4D_Document			*Bd;
//	C4D_Thread				*Bt;
//	C4D_Material			*Mt;
//	C4D_Texture				*Tx;
//	C4D_BaseSelect		*Bs;
//	C4D_BaseSound			*Bu;
//	C4D_BaseDraw			*Br;
//	C4D_BaseView			*Bv;
//	C4D_Neighbor			*Nb;
//	C4D_Pool					*Pl;
//	C4D_BitmapFilter	*Fi;
//	C4D_Painter				*Pa;
//	C4D_Link					*Ln;
//	C4D_GraphView			*Gv;
//	C4D_GeData				*Gd;
//	C4D_Atom					*At;
//	C4D_Coffee				*Co;
//	C4D_CAnimation		*CA;
//	C4D_CrashHandler	CrashHandler;
//	C4D_CreateOpenGLContext	CreateOpenGLContext;
//	C4D_DeleteOpenGLContext DeleteOpenGLContext;
//	C4D_GLSL					*GL;
//};

//#ifndef __API_INTERN__
//	#define C4DOS (*t_C4DOS)
//	Int32 InitOS(void *p);
//#endif
//
//extern OperatingSystem C4DOS;

#endif
