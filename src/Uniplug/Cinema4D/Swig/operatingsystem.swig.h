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
//typedef LONG CDialogMessage(CDialog *cd, CUserArea *cu, BaseContainer *msg);
//typedef void ListViewCallBack(LONG &res_type,void *&result,void *userdata,void *secret,LONG cmd,LONG line,LONG col,void *data1);
//typedef void IlluminationModel(VolumeData *sd, RayLightCache *rlc, void *dat);
//typedef Bool COFFEE_ERRORHANDLER(void *priv_data, const BaseContainer &bc);
//typedef void* (*GlProgramFactoryAllocPrivate)();
//typedef void (*GlProgramFactoryFreePrivate)(void* pData);
//typedef void* (*GlProgramFactoryAllocDescription)();
//typedef void (*GlProgramFactoryFreeDescription)(void* pData);
//typedef void (*GlProgramFactoryCreateTextureFunctionCallback)(const Real* prIn, Real* prOut, void* pData);
//typedef void (*GlProgramFactoryMessageCallback)(LONG lMessage, const void* pObj, LULONG ulIndex, GlProgramFactory* pFactory);
//typedef LONG (*GlProgramFactoryErrorHandler)(GlProgramType type, const char* pszError);
//typedef void (*GlVertexBufferDrawElementCallback)(LONG lElement, void* pData);
//typedef void (*BrowserPopupCallback)(void *userdata, LONG cmd, SDKBrowserURL &url);
//
//typedef void ThreadMain(void *data);
//typedef Bool ThreadTest(void *data);
//typedef const CHAR *ThreadName(void *data);
//typedef void ProgressHook(Real p, RENDERPROGRESSTYPE progress_type, void *private_data);
//typedef void BakeProgressHook(BakeProgressInfo* info);
//
//typedef void *HierarchyAlloc(void *main);
//typedef void HierarchyFree(void *main, void *data);
//typedef void HierarchyCopyTo(void *main, void *src, void *dst);
//typedef Bool HierarchyDo(void *main, void *data, BaseObject *op, const Matrix &mg, Bool controlobject);
//
//typedef void PopupEditTextCallback(LONG mode, const String &text, void *userdata);
//
//typedef void (*LASTCURSORINFOFUNC)();
//
//typedef Bool (*SaveCallbackImageFunc)(RDATA_SAVECALLBACK_CMD cmd, void* userdata, BaseDocument* doc, LONG framenum, BaseBitmap* bmp, const Filename &fn);
//
//typedef Bool BackgroundHandler(void *data, BACKGROUNDHANDLERCOMMAND command, BACKGROUNDHANDLERFLAGS parm);
//
//typedef void (*C4D_CrashHandler)(CHAR *crashinfo);
//typedef void (*C4D_CreateOpenGLContext)(void* context, void* root, ULONG flags);
//typedef void (*C4D_DeleteOpenGLContext)(void* context, ULONG flags);
//
//typedef GeData CoffeeEditorCallback(BaseList2D *obj, const BaseContainer &msg);
//typedef void (*V_CODE)(Coffee*, VALUE*&, LONG);
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
//	IconData(BaseBitmap *t_bmp, LONG t_x, LONG t_y, LONG t_w, LONG t_h) { bmp=t_bmp; x=t_x; y=t_y; w=t_w; h=t_h; }
//
//	BaseBitmap			*bmp;
//	LONG						x,y,w,h;
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
//	LONG   id;
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
//	Real						(*SNoise							)(Vector p);
//	Real						(*SNoiseT							)(Vector p, Real t);
//	Real						(*Noise								)(Vector p);
//	Real						(*NoiseT							)(Vector p, Real t);
//	Real						(*PNoise							)(Vector p, Vector d);
//	Real						(*PNoiseT							)(Vector p, Real t, Vector d, Real dt);
//	Real						(*Turbulence					)(Vector p, Real oct, Bool abs);
//	Real						(*TurbulenceT					)(Vector p, Real t, Real oct, Bool abs);
//	Real						(*WavyTurbulence			)(Vector p, Real t, Real oct, Real start);
//	void						(*InitFbm							)(Real *table, LONG max_octaves, Real lacunarity, Real h);
//	Real						(*Fbm									)(Real *table, Vector p, Real oct);
//	Real						(*FbmT								)(Real *table, Vector p, Real t, Real oct);
//	Real						(*RidgedMultifractal	)(Real *table, Vector p, Real oct, Real offset, Real gain);
//	Real						(*CalcSpline					)(Real x, Real *knot, LONG nknots);
//	Vector					(*CalcSplineV					)(Real x, Vector *knot, LONG nknots);
//
//	void						(*Illuminance1				)(VolumeData *sd, Vector *diffuse, Vector *specular, Real exponent);
//
//	LONG						(*GetCurrentCPU				)(VolumeData *sd);
//	LONG						(*GetCPUCount					)(VolumeData *sd);
//	void*						(*GetRayData					)(VolumeData *sd, LONG i);
//
//	RayObject*			(*GetObj							)(VolumeData *sd, LONG i);
//	LONG						(*GetObjCount					)(VolumeData *sd);
//
//	RayLight*				(*GetLight						)(VolumeData *sd, LONG i);
//	LONG						(*GetLightCount				)(VolumeData *sd);
//
//	RayObject*			(*ID_to_Obj						)(VolumeData *sd, const RayHitID &id);
//
//	void						(*GetUVW							)(VolumeData *sd, RayObject *op, LONG uvwind, LONG i, PolyVector *uvw);
//	Vector					(*GetPointUVW					)(VolumeData *sd, TexData *tdp, const RayHitID &lhit, const LVector &p);
//	void						(*GetNormals					)(VolumeData *sd, RayObject *op, LONG polygon, PolyVector *norm);
//	TexData*				(*GetTexData					)(VolumeData *sd, RayObject *op, LONG index);
//	LVector					(*GetNormal						)(VolumeData *sd, RayObject *op, LONG polygon, Bool second);
//	LVector					(*GetSmoothedNormal		)(VolumeData *sd, const RayHitID &hit, const LVector &p);
//	Bool						(*GetRS								)(VolumeData *sd, const RayHitID &hit, const LVector &p, Real *r, Real *s);
//
//	void						(*GetRay							)(VolumeData *sd, Real x, Real y, Ray *ray);
//	LVector					(*ScreenToCamera			)(VolumeData *sd, const LVector &p);
//	LVector					(*CameraToScreen			)(VolumeData *sd, const LVector &p);
//	Bool						(*ProjectPoint				)(VolumeData *sd, TexData *tdp, const RayHitID &lhit, const LVector &p, const LVector &n, Vector *uv);
//
//	VolumeData*			(*AllocVolumeData			)(void);
//	void						(*FreeVolumeData			)(VolumeData *sd);
//
//	void						(*StatusSetText				)(VolumeData *sd, const String *str);
//	void						(*StatusSetBar				)(VolumeData *sd, Real percentage);
//
//	TexData*				(*AllocTexData				)(void);
//	void						(*FreeTexData					)(TexData *td);
//	void						(*InitTexData					)(TexData *td);
//
//	Vector					(*CalcVisibleLight		)(VolumeData *sd, Ray *ray, Real maxdist, Vector *trans);
//	void						(*GetXY								)(VolumeData *sd, LONG *x, LONG *y, LONG *scale);
//
//	LONG						(*Obj_to_Num					)(VolumeData *sd, RayObject *obj);
//	LONG						(*Light_to_Num				)(VolumeData *sd, RayLight *light);
//
//	void						(*CopyVolumeData			)(VolumeData *src, VolumeData *dst);
//
//	Bool						(*VPAllocateBuffer		)(Render *render, LONG id, LONG subid, LONG bitdepth, Bool visible);
//	VPBuffer*				(*VPGetBuffer					)(Render *render, LONG id, LONG subid);
//	LONG						(*VPGetInfo						)(VPBuffer *buf, VPGETINFO type);
//	Bool						(*VPGetLine						)(VPBuffer *buf, LONG x, LONG y, LONG cnt, void *data, LONG bitdepth, Bool dithering);
//	Bool						(*VPSetLine						)(VPBuffer *buf, LONG x, LONG y, LONG cnt, void *data, LONG bitdepth, Bool dithering);
//
//	void						(*OutOfMemory					)(VolumeData *sd);
//	Real						(*GetLightFalloff			)(LONG falloff, Real inner, Real outer, Real dist);
//
//	Bool						(*TestBreak						)(VolumeData *sd);
//
//	void						(*VPGetRenderData			)(Render *render, BaseContainer *bc);
//	void						(*VPSetRenderData			)(Render *render, const BaseContainer *bc);
//
//	void						(*Illuminance					)(VolumeData *sd, Vector *diffuse, Vector *specular, IlluminationModel *model, void *data);
//
//	BaseVideoPost*  (*FindVideoPost				)(VolumeData *sd, LONG i, Bool index);
//	VPFragment**		(*VPGetFragments			)(VolumeData *sd, LONG x, LONG y, LONG cnt, VPGETFRAGMENTS flags);
//	Bool						(*AddLensGlowEx				)(VolumeData *sd, LensGlowStruct *lgs, Vector *lgs_pos, LONG lgs_cnt, Real intensity);
//
//	RayLight*				(*AllocRayLight				)(BaseDocument *doc, BaseObject *op);
//	void						(*FreeRayLight				)(RayLight *lgt);
//
//	LONG						(*TranslateObjIndex		)(VolumeData *sd, LONG index, Bool old_to_new);
//	Bool						(*TranslatePolygon		)(VolumeData *sd, RayObject *op, LONG local_index, Vector *previous_points);
//	Bool						(*SampleLensFX				)(VolumeData *sd, VPBuffer *rgba, VPBuffer *fx, BaseThread *bt);
//	LONG						(*VPAllocateBufferFX	)(Render *render, LONG id, const String &name, LONG bitdepth, Bool visible);
//
//	VolumeData*			(*VPGetInitialVolumeData)(Render *render, LONG cpu);
//
//	Bool						(*CalcFgBg						)(VolumeData *sd, Bool foreground, LONG x, LONG y, LONG subx, LONG suby, Vector *color, Real *alpha);
//	LONG						(*GetRayPolyState			)(VolumeData *sd, RayObject *op, LONG i);
//
//	void						(*GetWeights					)(VolumeData *sd, const RayHitID &hit, const LVector &p, RayPolyWeight *wgt);
//	void						(*InitVolumeData			)(VolumeData *src, VolumeData *dst);
//
//	Vector					(*TraceColor					)(VolumeData *sd, Ray *ray, Real maxdist, const RayHitID &lhit, SurfaceIntersection *si);
//	Bool  					(*TraceGeometry				)(VolumeData *sd, Ray *ray, Real maxdist, const RayHitID &lhit, SurfaceIntersection *si);
//	void  					(*GetSurfaceData		  )(VolumeData *sd, SurfaceData *cd, LONG calc_illum, Bool calc_shadow, Bool calc_refl, Bool calc_trans, Ray *ray, const SurfaceIntersection &si);
//
//	void						(*SkipRenderProcess   )(VolumeData *sd);
//
//	void*						(*VPGetPrivateData    )(Render *render);
//	Render*					(*GetRenderInstance   )(VolumeData *sd);
//
//	Vector					(*CentralDisplacePointUV)(VolumeData *sd, RayObject *op, LONG local_id, LONG uu, LONG vv);
//	Vector					(*CalcDisplacementNormals)(VolumeData *sd, Real par_u, Real par_v, LONG u0, LONG v0, LONG u1, LONG v1, LONG u2, LONG v2, const Vector &a, const Vector &b, const Vector &c, RayObject *op, LONG polynum);
//
//	Stratified2DRandom* (*SNAlloc										)(void);
//	void								(*SNFree										)(Stratified2DRandom *rnd);
//	Bool						(Stratified2DRandom::*SNInit		)(LONG initial_value, Bool reset);
//	void						(Stratified2DRandom::*SNGetNext	)(Real *xx, Real *yy);
//	BAKE_TEX_ERR    (*BakeTexture										)(BaseDocument* doc, const BaseContainer &data, BaseBitmap *bmp, BaseThread* th, BakeProgressHook* hook, BakeProgressInfo* info);
//	BaseDocument*   (*InitBakeTexture   						)(BaseDocument* doc, TextureTag* textag, UVWTag* texuvw, UVWTag* destuvw, const BaseContainer &bc, BAKE_TEX_ERR* err, BaseThread* th);
//	BaseDocument*   (*InitBakeTextureA   						)(BaseDocument* doc, TextureTag** textags, UVWTag** texuvws, UVWTag** destuvws, LONG cnt, const BaseContainer &bc, BAKE_TEX_ERR* err, BaseThread* th);
//
//	void						(*GetDUDV   										)(VolumeData *vd, TexData *tex, const LVector &p, const LVector &phongn, const LVector &orign, const RayHitID &hitid, Bool forceuvw, Vector *ddu, Vector *ddv);
//	void						(*CalcArea											)(VolumeData *sd, RayLight *light, Bool nodiffuse, Bool nospecular, Real specular_exponent, const LVector &ray_vector, const LVector &p, const LVector &bumpn, const LVector &orign, RAYBIT raybits, Vector *diffuse, Vector *specular);
//	Bool						(*Illuminate										)(VolumeData *sd, RayLight *light, Vector *col, LVector *lv, const LVector &p, const LVector &bumpn, const LVector &phongn, const LVector &orign, const LVector &ray_vector, ILLUMINATEFLAGS calc_shadow, const RayHitID &hitid, RAYBIT raybits, Bool cosine_cutoff, Vector *xshadow);
//	void						(*IlluminanceSurfacePoint				)(VolumeData *sd, IlluminanceSurfacePointData *f, Vector *diffuse, Vector *specular);
//	Vector					(*IlluminanceAnyPoint						)(VolumeData *sd, const LVector &p, ILLUMINATEFLAGS flags, RAYBIT raybits);
//	Vector					(*CalcShadow										)(VolumeData *sd, RayLight *light, const LVector &p, const LVector &bumpn, const LVector &phongn, const LVector &orign, const LVector &rayv, Bool transparency, const RayHitID &hitid, RAYBIT raybits);
//
//	Bool						(*SetRenderProperty							)(Render *render, LONG id, const GeData &dat);
//	void						(*SetXY													)(VolumeData *sd, Real x, Real y);
//	void						(*InitSurfacePointProperties		)(VolumeData *sd, TexData *td);
//
//	Real						(*SNoiseP												)(Vector p, Real t, LONG t_repeat);
//	Real						(*TurbulenceP										)(Vector p, Real t, Real oct, Bool abs, LONG t_repeat);
//	Real						(*FbmP													)(Real *table, Vector p, Real t, Real oct, LONG t_repeat);
//	Real						(*RidgedMultifractalP						)(Real *table, Vector p, Real t, Real oct, Real offset, Real gain, LONG t_repeat);
//
//	Bool						(*AttachVolumeDataFake					)(VolumeData *sd, BaseObject *camera, const BaseContainer &renderdata, void *priv);
//	RayObject*			(*AllocRayObject								)(LONG tex_cnt, void *priv);
//	void						(*FreeRayObject									)(RayObject *op);
//
//	Bool  					(*TraceGeometryEnhanced				  )(VolumeData *sd, Ray *ray, Real maxdist, const RayHitID &lhit, LONG raydepth, RAYBIT raybits, LVector *oldray, SurfaceIntersection *si);
//	Vector					(*TraceColorDirect							)(VolumeData *sd, Ray *ray, Real maxdist, LONG raydepth, RAYBIT raybits, const RayHitID &lhit, LVector *oldray, SurfaceIntersection *si);
//
//	Vector					(*CalcHardShadow								)(VolumeData *sd, const LVector &p1, const LVector &p2, CALCHARDSHADOW flags, const RayHitID &lhit, LONG recursion_id, void *recursion_data);
//	void						(*StatusSetSpinMode							)(VolumeData *sd, Bool on);
//
//	Vector					(*TransformColor								)(const Vector &v, COLORSPACETRANSFORMATION colortransformation);
//
//	void            (*VPIccConvert									)(Render *rnd, void *data, LONG xcnt, LONG components, Bool inverse);
//
//	Bool						(*SaveShaderStack								)(VolumeData *sd, RayShaderStackElement *&stack, LONG &stack_cnt);
//	Bool						(*RestoreShaderStack						)(VolumeData *sd, RayShaderStackElement *stack, LONG stack_cnt);
//
//	RayObject*			(*GetSky												)(VolumeData *sd, LONG i);
//	LONG						(*GetSkyCount										)(VolumeData *sd);
//
//	Bool						(*AddLensGlow										)(VolumeData *sd, LensGlowStruct *lgs, Vector *lgs_pos, LONG lgs_cnt, Real intensity, Bool linear_workflow);
//};
//
//struct C4D_HyperFile
//{
//	Bool						(HyperFile::*Open								)(LONG ident, const Filename &name, FILEOPEN mode, FILEDIALOG error_dialog);
//	Bool						(HyperFile::*Close							)();
//
//	Bool						(HyperFile::*WriteChar					)(CHAR  v);
//	Bool						(HyperFile::*WriteUChar					)(UCHAR v);
//	Bool						(HyperFile::*WriteWord					)(SWORD  v);
//	Bool						(HyperFile::*WriteUWord					)(UWORD v);
//	Bool						(HyperFile::*WriteLong					)(LONG  v);
//	Bool						(HyperFile::*WriteULong					)(ULONG v);
//	Bool						(HyperFile::*WriteReal					)(Real  v);
//	Bool						(HyperFile::*WriteSReal					)(SReal v);
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
//	Bool						(HyperFile::*ReadChar						)(CHAR         *v);
//	Bool						(HyperFile::*ReadUChar					)(UCHAR        *v);
//	Bool						(HyperFile::*ReadWord						)(SWORD        *v);
//	Bool						(HyperFile::*ReadUWord					)(UWORD        *v);
//	Bool						(HyperFile::*ReadLong					  )(LONG         *v);
//	Bool						(HyperFile::*ReadULong					)(ULONG        *v);
//	Bool						(HyperFile::*ReadReal					  )(Real         *v);
//	Bool						(HyperFile::*ReadSReal					)(SReal        *v);
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
//	Bool						(HyperFile::*WriteChunkStart		)(LONG id, LONG level);
//	Bool						(HyperFile::*WriteChunkEnd			)();
//	Bool						(HyperFile::*ReadChunkStart			)(LONG *id, LONG *level);
//	Bool						(HyperFile::*ReadChunkEnd				)();
//	Bool						(HyperFile::*SkipToEndChunk			)();
//	BaseDocument*		(HyperFile::*GetDocument				)() const;
//
//	LONG						(HyperFile::*GetFileVersion			)() const;
//
//	Bool						(HyperFile::*WriteImage					)(BaseBitmap *v, LONG format, const BaseContainer *data, SAVEBIT savebits);
//	Bool						(HyperFile::*WriteArray					)(const void *data, HYPERFILEARRAY type, LONG structure_increment, LONG count);
//	Bool						(HyperFile::*ReadArray					)(void *data, HYPERFILEARRAY type, LONG structure_increment, LONG count);
//
//	HyperFile*			(*AllocHF             )();
//	void						(*FreeHF              )(HyperFile *&hf);
//
//	FILEERROR				(*ReadFile						)(BaseDocument *doc, GeListNode *node, const Filename &filename, LONG ident, String *warning_string);
//	FILEERROR				(*WriteFile						)(BaseDocument *doc, GeListNode *node, const Filename &filename, LONG ident);
//
//	Bool						(*WriteGeData					)(HyperFile *hf, const GeData &v);
//	Bool						(*ReadGeData					)(HyperFile *hf, GeData &v);
//};
//
//struct C4D_BaseContainer
//{
//	BaseContainer		*Default;
//
//	BaseContainer*	(*Alloc								)(LONG id);
//	void						(*Free								)(BaseContainer *killme);
//	Bool						(*Compare							)(const BaseContainer &bc1, const BaseContainer &bc2);
//	void*						(*BrowseContainer			)(const BaseContainer *bc, LONG *id, GeData **data, void *handle);
//
//	void						(BaseContainer::*SDKInit						)(LONG id, const BaseContainer *src);
//	BaseContainer*	(BaseContainer::*GetClone						)(COPYFLAGS flags, AliasTrans *trans) const;
//	void						(BaseContainer::*FlushAll						)();
//	LONG						(BaseContainer::*GetId							)() const;
//	void						(BaseContainer::*SetId							)(LONG newid);
//
//	void						(BaseContainer::*SetVoid						)(LONG id, void *v);
//	void						(BaseContainer::*SetReal						)(LONG id, Real v);
//	void						(BaseContainer::*SetBool						)(LONG id, Bool v);
//	void						(BaseContainer::*SetLong						)(LONG id, LONG v);
//	void						(BaseContainer::*SetString					)(LONG id, const String &v);
//	void						(BaseContainer::*SetFilename				)(LONG id, const Filename &v);
//	void						(BaseContainer::*SetTime						)(LONG id, const BaseTime &v);
//	void						(BaseContainer::*SetContainer				)(LONG id, const BaseContainer &v);
//	void						(BaseContainer::*SetVector					)(LONG id, const Vector &v);
//	void						(BaseContainer::*SetMatrix					)(LONG id, const Matrix &v);
//
//	BaseContainer		(BaseContainer::*GetContainer				)(LONG id) const;
//	BaseContainer*	(BaseContainer::*GetContainerInstance)(LONG id) const;
//	Bool						(BaseContainer::*RemoveData					)(LONG id);
//	LONG						(BaseContainer::*FindIndex					)(LONG id, GeData **data) const;
//	LONG						(BaseContainer::*GetIndexId					)(LONG index) const;
//	Bool						(BaseContainer::*RemoveIndex				)(LONG index);
//	GeData*					(BaseContainer::*GetIndexData				)(LONG index) const;
//
//	BaseList2D*			(BaseContainer::*GetLink						)(LONG id, const BaseDocument *doc, LONG instanceof) const;
//	void						(BaseContainer::*SetLink						)(LONG id, C4DAtomGoal *link);
//
//	const GeData&		(BaseContainer::*GetData						)(LONG id) const;
//	Bool						(BaseContainer::*GetParameter				)(const DescID &id, GeData &t_data) const;
//	Bool						(BaseContainer::*SetParameter				)(const DescID &id, const GeData &t_data);
//
//	GeData*					(BaseContainer::*InsData  					)(LONG id, const GeData &n);
//	GeData*					(BaseContainer::*SetData  					)(LONG id, const GeData &n);
//	void						(BaseContainer::*SetLLong						)(LONG id, LLONG v);
//	GeData*					(BaseContainer::*InsDataAfter				)(LONG id, const GeData &n, const GeData *last);
//	Bool						(BaseContainer::*CopyTo							)(BaseContainer *dest,COPYFLAGS flags,AliasTrans *trans) const;
//	void						(BaseContainer::*Sort								)();
//
//	Real						(BaseContainer::*GetReal						)(LONG id, Real preset) const;
//	Bool						(BaseContainer::*GetBool						)(LONG id, Bool preset) const;
//	LONG						(BaseContainer::*GetLong						)(LONG id, LONG preset) const;
//	String					(BaseContainer::*GetString					)(LONG id, const String& preset) const;
//	Filename				(BaseContainer::*GetFilename				)(LONG id, const Filename& preset) const;
//	BaseTime				(BaseContainer::*GetTime						)(LONG id, const BaseTime& preset) const;
//	Vector					(BaseContainer::*GetVector					)(LONG id, const Vector& preset) const;
//	Matrix					(BaseContainer::*GetMatrix					)(LONG id, const Matrix& preset) const;
//	LLONG						(BaseContainer::*GetLLong						)(LONG id, LLONG preset) const;
//	void*						(BaseContainer::*GetVoid						)(LONG id, void* preset) const;
//	const GeData*		(BaseContainer::*GetDataPointer			)(LONG id) const;
//	void						(BaseContainer::*Merge							)(const BaseContainer &src);
//};
//
//struct C4D_GeData
//{
//	void            (*Free                )(GeData *data);
//	Bool            (*IsEqual             )(const GeData *data,const GeData *data2);
//	LONG						(*GetType							)(const GeData *data);
//	void            (*CopyData						)(GeData *dest,const GeData *src,AliasTrans *aliastrans);
//
//	Bool						(*SetNil              )(GeData *data);
//	Bool						(*SetLong             )(GeData *data,LONG n);
//	Bool						(*SetReal             )(GeData *data,Real n);
//	Bool						(*SetVector           )(GeData *data,const Vector &n);
//	Bool						(*SetMatrix           )(GeData *data,const Matrix &n);
//	Bool						(*SetString           )(GeData *data,const String *n);
//	Bool            (*SetFilename         )(GeData *data,const Filename *n);
//	Bool            (*SetBaseTime         )(GeData *data,const BaseTime &n);
//	Bool            (*SetBaseContainer    )(GeData *data,const BaseContainer *n);
//	Bool            (*SetLink             )(GeData *data,const BaseLink &n);
//
//	LONG						(*GetLong							)(const GeData *data);
//	Real						(*GetReal							)(const GeData *data);
//	const Vector&		(*GetVector						)(const GeData *data);
//	const Matrix&		(*GetMatrix						)(const GeData *data);
//	const String&		(*GetString						)(const GeData *data);
//	const Filename&	(*GetFilename					)(const GeData *data);
//	const BaseTime&	(*GetTime							)(const GeData *data);
//	BaseContainer*	(*GetContainer				)(const GeData *data);
//	BaseLink*				(*GetLink             )(const GeData *data);
//
//	Bool            (*SetCustomData				)(GeData *data,LONG type,const CustomDataType &n);
//	CustomDataType* (*GetCustomData				)(const GeData *data,LONG type);
//	Bool            (*InitCustomData			)(GeData *data,LONG type);
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
//	void						(*InitCString					)(String *str, const CHAR *txt, LONG count, STRINGENCODING type);
//	void						(*InitArray						)(String *str, LONG count, UWORD fillchr);
//	void						(*Flush								)(String *str);
//	void						(*CopyTo							)(const String *src, String *dst);
//	Bool						(*Add									)(String *dst,const String *src);
//
//	String					(*RealToString				)(Real v, LONG vk, LONG nk, Bool e, UWORD xchar);
//	String					(*LongToString				)(LONG l);
//	String					(*LLongToString				)(LLONG l);
//	String					(*LLongToStringExEx		)(LLONG l);
//
//	UWORD						(*GetChr							)(const String *str,LONG pos);
//	void						(*SetChr							)(String *str,LONG pos,UWORD chr);
//
//	LONG						(String::*GetLength			)() const;
//	Bool						(String::*FindFirst			)(const String &str, LONG *pos, LONG start) const;
//	Bool						(String::*FindLast			)(const String &str, LONG *pos, LONG start) const;
//	void						(String::*Delete				)(LONG pos,LONG count);
//	void						(String::*Insert				)(LONG pos, const String &str, LONG start, LONG end);
//	String					(String::*SubStr				)(LONG start, LONG count) const;
//	Real						(String::*ToReal				)(LONG *error, LONG unit, LONG angular_type, LONG base) const;
//	LONG						(String::*ToLong				)(LONG *error) const;
//	String					(String::*ToUpper				)() const;
//	String					(String::*ToLower				)() const;
//	LONG						(String::*GetCStringLen	)(STRINGENCODING type) const;
//	LONG						(String::*GetCString		)(CHAR *cstr, LONG max, STRINGENCODING type) const;
//	void						(String::*GetUcBlock		)(UWORD *Ubc, LONG Max) const;
//	void						(String::*GetUcBlockNull)(UWORD *Ubc, LONG Max) const;
//	void						(String::*SetUcBlock		)(const UWORD *Ubc, LONG Count);
//	LONG						(String::*Compare				)(const String &dst) const;
//	LONG						(String::*LexCompare		)(const String &dst) const;
//	LONG						(String::*RelCompare		)(const String &dst) const;
//	LONG						(String::*ComparePart		)(const String &Str, LONG cnt, LONG pos) const;
//	LONG						(String::*LexComparePart)(const String &Str, LONG cnt, LONG pos) const;
//	Bool						(String::*FindFirstCh		)(UWORD ch, LONG *Pos, LONG Start) const;
//	Bool						(String::*FindLastCh		)(UWORD ch, LONG *Pos, LONG Start) const;
//	Bool						(String::*FindFirstUpper)(const String &find, LONG *pos, LONG start) const;
//	Bool						(String::*FindLastUpper	)(const String &find, LONG *pos, LONG start) const;
//};
//
//struct C4D_Bitmap
//{
//	BaseBitmap*			(*Alloc								)(void);
//	void						(*Free								)(BaseBitmap *bm);
//	BaseBitmap*			(*GetClone						)(const BaseBitmap *src);
//	LONG						(*GetBw								)(const BaseBitmap *bm);
//	LONG						(*GetBh								)(const BaseBitmap *bm);
//	LONG						(*GetBt								)(const BaseBitmap *bm);
//	LONG						(*GetBpz							)(const BaseBitmap *bm);
//	IMAGERESULT			(*Init2								)(BaseBitmap *bm, const Filename *name, LONG frame, Bool *ismovie);
//	void					  (*FlushAll						)(BaseBitmap *bm);
//	IMAGERESULT	    (BaseBitmap::*Save		)(const Filename &name, LONG format, const BaseContainer *data, SAVEBIT savebits) const;
//	void						(*SetCMAP							)(BaseBitmap *bm, LONG i, LONG r, LONG g, LONG b);
//	void						(*ScaleIt							)(const BaseBitmap *src, BaseBitmap *dst, LONG intens, Bool sample, Bool nprop);
//	void						(*SetPen							)(BaseBitmap *bm, LONG r, LONG g, LONG b);
//	void						(*Clear								)(BaseBitmap *bm, LONG x1, LONG y1, LONG x2, LONG y2, LONG r, LONG g, LONG b);
//	void						(*Line								)(BaseBitmap *bm, LONG x1, LONG y1, LONG x2, LONG y2);
//	void						(*GetPixel						)(const BaseBitmap *bm, LONG x, LONG y, UWORD *r, UWORD *g, UWORD *b);
//	BaseBitmap *		(*AddChannel					)(BaseBitmap *bm, Bool internal, Bool straight);
//	void						(*RemoveChannel				)(BaseBitmap *bm, BaseBitmap *channel);
//	void						(*GetAlphaPixel				)(const BaseBitmap *bm, BaseBitmap *channel, LONG x, LONG y, UWORD *val);
//	BaseBitmap *		(*GetInternalChannel	)(BaseBitmap *bm);
//	LONG						(*GetChannelCount			)(const BaseBitmap *bm);
//	BaseBitmap *		(*GetChannelNum				)(BaseBitmap *bm, LONG num);
//	BaseBitmap*			(*GetClonePart				)(const BaseBitmap *src, LONG x, LONG y, LONG w, LONG h);
//	Bool						(*CopyTo							)(const BaseBitmap *src, BaseBitmap *dst);
//	void						(*ScaleBicubic				)(const BaseBitmap *src, BaseBitmap *dest, LONG src_xmin, LONG src_ymin, LONG src_xmax, LONG src_ymax, LONG dst_xmin, LONG dst_ymin, LONG dst_xmax, LONG dst_ymax);
//	BaseBitmap*			(*GetAlphaBitmap			)(const BaseBitmap *bm, BaseBitmap *channel);
//
//	Bool						(*IsMultipassBitmap		)(const BaseBitmap *bm);
//
//	MultipassBitmap*(*MPB_AllocWrapperPB	)(LONG bx, LONG by, COLORMODE mode);
//	MultipassBitmap*(*MPB_AllocWrapper    )(BaseBitmap *bmp);
//	PaintBitmap*		(*MPB_GetPaintBitmap	)(MultipassBitmap *mp);
//	LONG						(*MPB_GetLayerCount		)(const MultipassBitmap *mp);
//	LONG						(*MPB_GetAlphaLayerCount)(const MultipassBitmap *mp);
//	LONG						(*MPB_GetHiddenLayerCount)(const MultipassBitmap *mp);
//	MultipassBitmap*(*MPB_GetLayerNum			)(MultipassBitmap *mp,LONG num);
//	MultipassBitmap*(*MPB_GetAlphaLayerNum)(MultipassBitmap *mp,LONG num);
//	MultipassBitmap*(*MPB_GetHiddenLayerNum)(MultipassBitmap *mp,LONG num);
//	MultipassBitmap*(*MPB_AddLayer				)(MultipassBitmap *mp,MultipassBitmap *insertafter,COLORMODE colormode,Bool hidden);
//	MultipassBitmap*(*MPB_AddFolder				)(MultipassBitmap *mp,MultipassBitmap *insertafter,Bool hidden);
//	MultipassBitmap*(*MPB_AddAlpha				)(MultipassBitmap *mp,MultipassBitmap *insertafter,COLORMODE colormode);
//	Bool						(*MPB_DeleteLayer			)(MultipassBitmap *mp,MultipassBitmap *layer);
//	MultipassBitmap*(*MPB_FindUserID			)(MultipassBitmap *mp,LONG id,LONG subid);
//	void						(*MPB_ClearImageData	)(MultipassBitmap *mp);
//	void						(*MPB_SetMasterAlpha	)(MultipassBitmap *mp, BaseBitmap *master);
//	GeData					(*MPB_GetParameter		)(const MultipassBitmap *mp, MPBTYPE id);
//	Bool						(*MPB_SetParameter		)(MultipassBitmap *mp, MPBTYPE id,const GeData &par);
//
//	ULONG						(*GetDirty						)(const BaseBitmap *bm);
//
//	void						(*GetPixelCnt					)(const BaseBitmap *bm, LONG x, LONG y, LONG cnt, UCHAR *buffer, LONG inc, COLORMODE dstmode, PIXELCNT flags, ColorProfileConvert *conversion);
//	GeData					(*GetBaseBitmapData		)(const BaseBitmap *bm, LONG id, const GeData &t_default);
//	Bool						(*SetBaseBitmapData		)(BaseBitmap *bm, LONG id, const GeData &data);
//
//	void						(*SetDirty						)(BaseBitmap *bm);
//	Bool						(*CopyPartTo					)(const BaseBitmap *src, BaseBitmap *dst, LONG x, LONG y, LONG w, LONG h);
//
//	BaseBitmapLink *(*BBL_Alloc						)(void);
//	void						(*BBL_Free						)(BaseBitmapLink *link);
//	BaseBitmap		 *(*BBL_Get							)(BaseBitmapLink *link);
//	void						(*BBL_Set							)(BaseBitmapLink *link, BaseBitmap *bmp);
//
//	VLONG           (*GetMemoryInfo  			)(const BaseBitmap *bmp);
//	LONG						(*MPB_GetEnabledLayerCount)(const MultipassBitmap *mp);
//	Bool						(*MPB_GetLayers				)(MultipassBitmap *mp, MPB_GETLAYERS flags, BaseBitmap **&list, LONG &count);
//	UCHAR*					(*GetDrawPortBits			)(BaseBitmap *bm);
//	Bool						(*GetUpdateRegions		)(const BaseBitmap *mp, BaseContainer &regions);
//	IMAGERESULT			(*Init1								)(BaseBitmap *bm, LONG x, LONG y, LONG depth, INITBITMAPFLAGS flags);
//	Bool						(*AddUpdateRegion			)(BaseBitmap *bm, LONG id, LONG type, LONG xmin, LONG xmax, LONG ymin, LONG ymax);
//	Bool						(*RemoveUpdateRegion	)(BaseBitmap *bm, LONG id);
//	BaseBitmap*			(*GetUpdateRegionBitmap)(const BaseBitmap *bm);
//	IMAGERESULT			(*Init3								)(BaseBitmap *&bm, const Filename &name, LONG frame, Bool *ismovie, BitmapLoaderPlugin **loaderplugin);
//	MultipassBitmap*(*MPB_GetSelectedLayer)(MultipassBitmap *mp);
//	Bool						(*SetPixelCnt					)(BaseBitmap *bm, LONG x, LONG y, LONG cnt, UCHAR *buffer, LONG inc, COLORMODE srcmode, PIXELCNT flags);
//	Bool						(*SetPixel						)(BaseBitmap *bm, LONG x, LONG y, LONG r, LONG g, LONG b);
//	Bool						(*SetAlphaPixel				)(BaseBitmap *bm, BaseBitmap *channel, LONG x, LONG y, LONG val);
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
//	String					(*ProfileInfo					)(const ColorProfile *profile, LONG info);
//	Bool						(*ProfileIsMonitorMode)(const ColorProfile *profile);
//	Bool						(*ProfileHasProfile		)(const ColorProfile *profile);
//	Bool						(*ProfileSetMonitorMode)(ColorProfile *profile, Bool on);
//
//	ColorProfileConvert* (*ProfileConvAlloc)(void);
//	void						(*ProfileConvFree			)(ColorProfileConvert *profile);
//	Bool						(*ProfileConvTransform)(ColorProfileConvert *profile, COLORMODE srccolormode, const ColorProfile *srcprofile, COLORMODE dstcolormode, const ColorProfile *dstprofile, Bool bgr);
//	void						(*ProfileConvConvert	)(const ColorProfileConvert *profile, const PIX *src, PIX *dst, LONG cnt, LONG SkipInputComponents, LONG SkipOutputComponents);
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
//	Bool						(*Choose							)(MovieSaver *ms, LONG id, BaseContainer *bc);
//	IMAGERESULT			(*Open								)(MovieSaver *ms, const Filename *name, BaseBitmap *bm, LONG fps, LONG id, const BaseContainer *data, SAVEBIT savebits,BaseSound *sound);
//};
//
//struct C4D_BaseChannel
//{
//	BaseChannel*		(*Alloc								)(void);
//	void						(*Free								)(BaseChannel *bc);
//	Bool						(*Compare							)(BaseChannel *bc1,BaseChannel *bc2);
//	INITRENDERRESULT(*InitTexture					)(BaseChannel *bc, const InitRenderStruct &irs);
//	void						(*FreeTexture					)(BaseChannel *bc);
//	Vector					(*BcSample						)(BaseChannel *bc, VolumeData *vd, Vector *p, Vector *delta, Vector *n, Real t, LONG tflag, Real off, Real scale);
//	BaseBitmap*			(*BCGetBitmap					)(BaseChannel *bc);
//	void						(*GetData							)(BaseChannel *bc, BaseContainer *ct);
//	void						(*SetData							)(BaseChannel *bc, const BaseContainer *ct);
//	Bool						(*ReadData						)(HyperFile *hf, BaseChannel *bc);
//	Bool						(*WriteData						)(HyperFile *hf, BaseChannel *bc);
//
//	LONG						(*GetPluginID					)(BaseChannel *bc);
//	BaseShader*		(*GetPluginShader			)(BaseChannel *bc);
//
//	Bool						(*Attach							)(BaseChannel *bc, GeListNode *element);
//
//	Bool						(*HandleShaderPopup		 )(const BaseContainer &bc, const DescID &descid, LONG value, VLONG param);
//	Bool						(*HandleShaderPopupI	 )(BaseList2D *parent, BaseShader *&current, LONG value, VLONG param);
//	Bool						(*BuildShaderPopupMenu )(BaseContainer *menu, const BaseContainer &bc, const DescID &descid, VLONG param);
//	Bool						(*BuildShaderPopupMenuI)(BaseContainer *menu, BaseList2D *parent, BaseShader *current, VLONG param);
//
//	void						(*HandleShaderMessage )(GeListNode *node, BaseShader *ps, LONG type, void *data);
//	Bool            (*ReadDataConvert     )(GeListNode *node, LONG id, HyperFile *hf);
//
//	INITRENDERRESULT(BaseShader::*InitRender		)(const InitRenderStruct &irs);
//	void						(BaseShader::*FreeRender		)(void);
//	Vector					(BaseShader::*Sample				)(ChannelData *vd);
//	Vector					(BaseShader::*SampleBump		)(ChannelData *vd, SAMPLEBUMP bumpflags);
//	BaseBitmap*			(BaseShader::*GetBitmap			)(void);
//	SHADERINFO			(BaseShader::*GetRenderInfo	)(void);
//
//	BaseShader*			(*PsAlloc										)(LONG type);
//	Bool						(BaseShader::*PsCompare			)(BaseShader* dst);
//
//	String					(*GetChannelName						)(LONG channelid);
//	LONG						(BaseShader::*GlMessage			)(LONG type, void *data);
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
//	void						(*SetCString					)(Filename *fn, const CHAR *str);
//	void						(*ClearSuffixComplete	)(Filename *fn);
//
//	void						(Filename::*SetIpConnection)(IpConnection *ipc);
//	IpConnection*		(Filename::*GetIpConnection)() const;
//};
//
//struct C4D_BrowseFiles
//{
//	BrowseFiles*		(*Alloc								)(const Filename *dir, LONG flags);
//	void						(*Free								)(BrowseFiles *bf);
//	void						(*Init								)(BrowseFiles *bf, const Filename *dir, LONG flags);
//
//	Bool						(BrowseFiles::*GetNext				)(void);
//	Filename				(BrowseFiles::*GetFilename		)(void);
//	Bool						(BrowseFiles::*IsDir					)(void);
//	LLONG						(BrowseFiles::*GetSize				)(void);
//	void						(BrowseFiles::*GetFileTime		)(LONG mode, LocalFileTime *out);
//	Bool						(BrowseFiles::*IsHidden				)(void);
//	Bool						(BrowseFiles::*IsReadOnly			)(void);
//	Bool						(BrowseFiles::*IsBundle				)(void);
//
//	BrowseVolumes*	(*BvAlloc							)(void);
//	void						(*BvFree							)(BrowseVolumes *bv);
//	void						(BrowseVolumes::*BvInit				)(void);
//	Bool						(BrowseVolumes::*BvGetNext		)(void);
//	Filename				(BrowseVolumes::*BvGetFilename)(void);
//	String					(BrowseVolumes::*BvGetVolumeName)( LONG *out_flags );
//};
//
//struct C4D_File
//{
//	BaseFile*				(*Alloc								)(void);
//	void						(*Free								)(BaseFile *fl);
//	AESFile*				(*AESAlloc						)(void);
//	Bool						(*AESCheckEncryption	)(const Filename& encrypt, const Filename& decrypt, const char* key, LONG keylen, LONG blocksize);
//
//	Bool						(BaseFile::*Open							)(const Filename &name, FILEOPEN mode, FILEDIALOG error_dialog, BYTEORDER order, LONG type, LONG creator);
//	Bool						(BaseFile::*Close							)();
//	void						(BaseFile::*SetOrder					)(BYTEORDER order);
//	VLONG						(BaseFile::*ReadBytes					)(void *data, VLONG len, Bool just_try_it);
//	Bool						(BaseFile::*WriteBytes				)(const void *data, VLONG len);
//	Bool						(BaseFile::*Seek							)(LLONG pos, FILESEEK mode);
//	LLONG						(BaseFile::*GetPosition				)();
//	LLONG						(BaseFile::*GetLength					)();
//	FILEERROR				(BaseFile::*GetError					)() const;
//	void						(BaseFile::*SetError					)(FILEERROR error);
//	Bool						(BaseFile::*WriteChar					)(CHAR  v);
//	Bool						(BaseFile::*WriteUChar				)(UCHAR v);
//	Bool						(BaseFile::*WriteWord					)(SWORD  v);
//	Bool						(BaseFile::*WriteUWord				)(UWORD v);
//	Bool						(BaseFile::*WriteLong					)(LONG  v);
//	Bool						(BaseFile::*WriteULong				)(ULONG v);
//	Bool						(BaseFile::*WriteSReal				)(SReal v);
//	Bool						(BaseFile::*WriteLReal				)(LReal v);
//	Bool						(BaseFile::*ReadChar					)(CHAR  *v);
//	Bool						(BaseFile::*ReadUChar					)(UCHAR *v);
//	Bool						(BaseFile::*ReadWord					)(SWORD  *v);
//	Bool						(BaseFile::*ReadUWord					)(UWORD *v);
//	Bool						(BaseFile::*ReadLong					)(LONG  *v);
//	Bool						(BaseFile::*ReadULong					)(ULONG *v);
//	Bool						(BaseFile::*ReadSReal					)(SReal *v);
//	Bool						(BaseFile::*ReadLReal					)(LReal *v);
//	Bool						(BaseFile::*ReadLLong					)(LLONG *v);
//	Bool						(BaseFile::*WriteLLong				)(LLONG v);
//
//	Bool						(AESFile::*AESOpen						)(const Filename &name, const char* key, LONG keylen, LONG blocksize, ULONG aes_flags, FILEOPEN mode, FILEDIALOG error_dialog, BYTEORDER order, LONG type, LONG creator);
//};
//
//struct C4D_Dialog
//{
//	CDialog*				(*Alloc									)(CDialogMessage *dlgfunc,void *userdata);
//	void						(*Free									)(CDialog *cd);
//	void*						(*GetUserData						)(CDialog *cd);
//	Bool						(*Close									)(CDialog *cd);
//	Bool						(*Enable								)(CDialog *cd, LONG id, Bool enabled,void *gadptr);
//	void						(*SetTimer							)(CDialog *cd, LONG timer);
//	Bool						(*SetLong								)(CDialog *cd, LONG id, LONG value,LONG min,LONG max,LONG step,void *gadptr);
//	Bool						(*SetReal								)(CDialog *cd, LONG id, Real value,Real min,Real max,Real step,LONG format,void *gadptr);
//	Bool						(*SetVector							)(CDialog *cd, LONG id, const Vector &value,void *gadptr);
//	Bool						(*SetString							)(CDialog *cd, LONG id, const String *text,void *gadptr);
//	Bool						(*SetColorField					)(CDialog *cd, LONG id, const Vector &color, Real brightness,Real maxbrightness,LONG flags,void *gadptr);
//	Bool						(*GetLong								)(CDialog *cd, LONG id, LONG &value,void *gadptr);
//	Bool						(*GetReal								)(CDialog *cd, LONG id, Real &value,void *gadptr);
//	Bool						(*GetVector							)(CDialog *cd, LONG id, Vector &value,void *gadptr);
//	Bool						(*GetString							)(CDialog *cd, LONG id, String *&text,void *gadptr);
//	Bool						(*GetColorField					)(CDialog *cd, LONG id, Vector &color, Real &brightness,void *gadptr);
//	Bool						(*LoadDialogResource		)(CDialog *cd, LONG id, LocalResource *lr, LONG flags);
//	Bool						(*TabGroupBegin					)(CDialog *cd, LONG id, LONG flags,LONG tabtype);
//	Bool						(*GroupBegin						)(CDialog *cd, LONG id, LONG flags,LONG cols,LONG rows,const String *title,LONG groupflags);
//	Bool						(*GroupSpace						)(CDialog *cd, LONG spacex,LONG spacey);
//	Bool						(*GroupBorder						)(CDialog *cd, ULONG borderstyle);
//	Bool						(*GroupBorderSize				)(CDialog *cd, LONG left, LONG top,LONG right,LONG bottom);
//	Bool						(*GroupEnd							)(CDialog *cd);
//	Bool						(*SetPopup							)(CDialog *cd, LONG id, const BaseContainer *bc,void *gadptr);
//	Bool						(*Screen2Local					)(CDialog *cd, LONG *x, LONG *y);
//	Bool						(*SetVisibleArea				)(CDialog *cd, LONG scrollgroupid, LONG x1, LONG y1,LONG x2,LONG y2);
//	Bool						(*GetItemDim						)(CDialog *cd, LONG id, LONG *x, LONG *y, LONG *w, LONG *h,void *gadptr);
//	Bool						(*SendRedrawThread			)(CDialog *cd, LONG id);
//	Bool            (*GetVisibleArea				)(CDialog *cd, LONG id, LONG *x1,LONG *y1,LONG *x2,LONG *y2);
//	Bool						(*RestoreLayout					)(CDialog *cd, void *secret);
//	Bool						(*SetMessageResult			)(CDialog *cd, const BaseContainer *result);
//
//	Bool            (*SetDragDestination		)(CDialog *cd, LONG cursor);
//	Bool						(*AttachSubDialog				)(CDialog *parentcd,LONG id,CDialog *cd);
//	LONG						(*GetID									)(CDialog *cu);
//	void*						(*FindCustomGui					)(CDialog *cd,LONG id);
//	Bool						(*AddGadget							)(CDialog *cd, LONG type, LONG id, const String *name,LONG par1,LONG par2, LONG par3, LONG par4,const BaseContainer *customdata,void **resptr);
//	Bool						(*ReleaseLink						)(CDialog *cd);
//	Bool						(*SendParentMessage			)(CDialog *cd,const BaseContainer *msg);
//
//	Bool						(*Open									)(CDialog *cd, DLG_TYPE dlgtype, CDialog *parent, LONG xpos, LONG ypos,LONG defaultw,LONG defaulth);
//	CUserArea*			(*AttachUserArea				)(CDialog *cd, LONG id,void *userdata,LONG userareaflags,void *gadptr);
//	Bool            (*GetDragObject					)(CDialog *cd, const BaseContainer *msg,LONG *type,void **object);
//
//	LassoSelection*	(*LSAlloc								)(void);
//	void						(*LSFree								)(LassoSelection *ls);
//	LONG						(*LSGetMode							)(LassoSelection *ls);
//	Bool						(*LSTest								)(LassoSelection *ls, LONG x, LONG y);
//	Bool						(*LSCheckSingleClick		)(LassoSelection *ls);
//	Bool						(*LSStart								)(LassoSelection *ls, CBaseFrame *cd, LONG mode, LONG start_x, LONG start_y, LONG start_button,LONG sx1, LONG sy1, LONG sx2, LONG sy2);
//	Bool						(*LSTestPolygon					)(LassoSelection *ls, const Vector &pa, const Vector &pb, const Vector &pc, const Vector &pd);
//
//	CBaseFrame*			(*CBF_FindBaseFrame			)(CDialog *cd,LONG id);
//	Bool						(*CBF_SetDragDestination)(CBaseFrame *cbf,LONG cursor);
//	void*						(*CBF_GetWindowHandle		)(CBaseFrame *cbf);
//	GeData					(*SendUserAreaMessage		)(CDialog *cd, LONG id, BaseContainer *msg,void *gadptr);
//	Bool						(*LSGetRectangle				)(LassoSelection *ls,Real &x1, Real &y1, Real &x2, Real &y2);
//	Bool						(*CBF_GetColorRGB				)(CBaseFrame *cbf,LONG colorid, LONG &r, LONG &g, LONG &b);
//	Bool						(*RemoveLastCursorInfo	)(LASTCURSORINFOFUNC func);
//	String					(*Shortcut2String				)(LONG shortqual,LONG shortkey);
//
//	Bool						(*GetIconCoordInfo			)(LONG &id, const CHAR* ident);
//	Bool						(*GetInterfaceIcon			)(LONG type, LONG id_x, LONG id_y, LONG id_w, LONG id_h, IconData &d);
//	Bool						(*IsEnabled							)(CDialog *cd, LONG id, void *gadptr);
//};
//
//struct C4D_UserArea
//{
//	void						(*Free                )(CUserArea* cu);
//	void*						(*GetUserData         )(CUserArea *cu);
//	LONG						(*GetWidth            )(CUserArea *cu);
//	LONG						(*GetHeight           )(CUserArea *cu);
//	LONG						(*GetID               )(CUserArea *cu);
//	void						(*SetMinSize          )(CUserArea *cu, LONG w,LONG h);
//	void						(*DrawLine            )(CUserArea *cu, LONG x1,LONG y1,LONG x2,LONG y2);
//	void						(*DrawRectangle       )(CUserArea *cu, LONG x1,LONG y1,LONG x2,LONG y2);
//	void						(*DrawSetPenV         )(CUserArea *cu, const Vector &color);
//	void						(*DrawSetPenI         )(CUserArea *cu, LONG id);
//	void						(*SetTimer            )(CUserArea *cu, LONG timer);
//	Bool						(*GetInputState       )(CBaseFrame *cu, LONG askdevice,LONG askchannel,BaseContainer *res);
//	Bool						(*GetInputEvent       )(CBaseFrame *cu, LONG askdevice,BaseContainer *res);
//	void						(*KillEvents          )(CBaseFrame *cu);
//	void						(*DrawSetFont         )(CUserArea *cu, LONG fontid);
//	LONG						(*DrawGetTextWidth    )(CUserArea *cu, const String *text);
//	LONG						(*DrawGetFontHeight   )(CUserArea *cu);
//	void						(*DrawSetTextColII    )(CUserArea *cu, LONG fg,LONG bg);
//	void						(*DrawSetTextColVI    )(CUserArea *cu, const Vector &fg,LONG bg);
//	void						(*DrawSetTextColIV    )(CUserArea *cu, LONG fg,const Vector &bg);
//	void						(*DrawSetTextColVV    )(CUserArea *cu, const Vector &fg,const Vector &bg);
//	void						(*DrawBitmap          )(CUserArea *cu, BaseBitmap *bmp,LONG wx,LONG wy,LONG ww,LONG wh,LONG x,LONG y,LONG w,LONG h,LONG mode);
//	void						(*SetClippingRegion   )(CUserArea *cu, LONG x,LONG y,LONG w,LONG h);
//	void						(*ScrollArea          )(CUserArea *cu, LONG xdiff,LONG ydiff,LONG x,LONG y,LONG w,LONG h);
//	void						(*ClearClippingRegion )(CUserArea *cu);
//	Bool						(*OffScreenOn         )(CUserArea *cu);
//
//	Bool						(*Global2Local        )(CBaseFrame *cu, LONG *x,LONG *y);
//	Bool						(*SendParentMessage   )(CUserArea *cu, const BaseContainer *msg);
//
//	Bool						(*Screen2Local        )(CBaseFrame *cu, LONG *x, LONG *y);
//	Bool            (*SetDragDestination  )(CUserArea *cu, LONG cursor);
//	Bool            (*HandleMouseDrag     )(CUserArea *cu, const BaseContainer *msg,LONG type,void *data,LONG dragflags);
//	Bool						(*IsEnabled           )(CUserArea *cu);
//
//	void						(*GetBorderSize       )(CUserArea *cu,LONG type,LONG *l,LONG *t,LONG *r,LONG *b);
//	void            (*DrawBorder          )(CUserArea *cu,LONG type,LONG x1,LONG y1,LONG x2,LONG y2);
//
//	_GeListView*		(*GeListView_Alloc            )(void);
//	void						(*GeListView_Free             )(_GeListView *lv);
//	Bool            (*GeListView_Attach           )(_GeListView *lv,CDialog *cd,LONG id,ListViewCallBack *callback,void *userdata);
//	void            (*GeListView_LvSuperCall      )(_GeListView *lv,LONG &res_type,void *&result,void *secret,LONG cmd,LONG line,LONG col);
//	void            (*GeListView_Redraw           )(_GeListView *lv);
//	void            (*GeListView_DataChanged      )(_GeListView *lv);
//	Bool            (*GeListView_ExtractMouseInfo )(_GeListView *lv,void *secret,MouseDownInfo &info,LONG size);
//	Bool            (*GeListView_ExtractDrawInfo  )(_GeListView *lv,void *secret,DrawInfo &info,LONG size);
//	Bool            (*GeListView_SendParentMessage)(_GeListView *lv,const BaseContainer *msg);
//	LONG						(*GeListView_GetId            )(_GeListView *lv);
//	void						(*GeListView_ShowCell					)(_GeListView *lv,LONG line,LONG col);
//
//	_SimpleListView* (*SimpleListView_Alloc       )(void);
//	void            (*SimpleListView_Free         )(_SimpleListView *lv);
//	Bool            (*SimpleListView_SetLayout    )(_SimpleListView *lv,LONG columns,const BaseContainer &data);
//	Bool            (*SimpleListView_SetItem      )(_SimpleListView *lv,LONG id,const BaseContainer &data);
//	Bool            (*SimpleListView_GetItem      )(_SimpleListView *lv,LONG id,BaseContainer *data);
//	LONG            (*SimpleListView_GetItemCount )(_SimpleListView *lv);
//	Bool            (*SimpleListView_GetItemLine  )(_SimpleListView *lv,LONG num,LONG *id,BaseContainer *data);
//	Bool            (*SimpleListView_RemoveItem   )(_SimpleListView *lv,LONG id);
//	LONG            (*SimpleListView_GetSelection )(_SimpleListView *lv,BaseSelect *selection);
//	Bool            (*SimpleListView_SetSelection )(_SimpleListView *lv,BaseSelect *selection);
//	void            (*SimpleListView_ShowCell			)(_SimpleListView *lv,LONG line, LONG col);
//
//	LONG            (*SimpleListView_GetProperty  )(_SimpleListView *lv,LONG id);
//	Bool            (*SimpleListView_SetProperty  )(_SimpleListView *lv,LONG id,LONG val);
//
//	Bool						(*IsHotkeyDown                )(CUserArea *cu, LONG id);
//	Bool						(*HasFocus										)(CUserArea *cu);
//
//	void						(*MouseDragStart							)(CUserArea *cu,LONG Button,Real mx,Real my,MOUSEDRAGFLAGS flag);
//	MOUSEDRAGRESULT (*MouseDrag										)(CUserArea *cu,Real *mx,Real *my,BaseContainer *channels);
//	MOUSEDRAGRESULT (*MouseDragEnd								)(CUserArea *cu);
//	LONG						(*DrawGetTextWidth_ListNodeName)(CUserArea *cu,BaseList2D *node, LONG fontid);
//	Bool						(*OffScreenOnRect							)(CUserArea *cu, LONG x, LONG y, LONG w, LONG h);
//	void						(*DrawText										)(CUserArea *cu, const String &txt,LONG x,LONG y,LONG flags);
//};
//
//struct C4D_Parser
//{
//	Parser*					(*Alloc								)(void);
//	void						(*Free								)(Parser *pr);
//	Bool						(*Eval								)(Parser *pr, const String *str, LONG *error,Real *res,LONG unit,LONG angletype,LONG basis);
//	Bool						(*EvalLong						)(Parser *pr, const String *str, LONG *error,LONG *res,LONG unit,LONG basis);
//	Bool						(*AddVar							)(Parser *pr, const String *str, Real *value, Bool case_sensitive);
//	Bool						(*AddVarLong					)(Parser *pr, const String *str, LONG *value, Bool case_sensitive);
//	Bool						(*RemoveVar						)(Parser *pr, const String *s, Bool case_sensitive);
//	Bool						(*RemoveAllVars				)(Parser *pr);
//	void						(*GetParserData				)(Parser *pr, ParserCache *p);
//	Bool						(*Init								)(Parser *pr, const String *s, LONG *error, LONG unit, LONG angle_unit, LONG base);
//	Bool						(*ReEval							)(Parser *pr, Real *result, LONG *error);
//	Bool						(*Calculate						)(Parser *pr, const ParserCache *pdat, Real *result, LONG *error);
//	Bool						(*ReEvalLong					)(Parser *pr, LONG *result, LONG *error);
//	Bool						(*CalculateLong				)(Parser *pr, const ParserCache *pdat, LONG *result, LONG *error);
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
//	const String&		(*LoadString          )(LocalResource *lr,LONG id);
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
//	LONG						(C4DAtom::*GetType							)(void) const;
//	Bool						(C4DAtom::*IsInstanceOf					)(LONG id) const;
//	Bool						(C4DAtom::*Message							)(LONG type, void *data);
//	Bool						(C4DAtom::*MultiMessage					)(MULTIMSG_ROUTE flags, LONG type, void *data);
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
//	Bool						(BaseList2D::*GetAnimatedParameter)(const DescID &id,GeData &t_data1,GeData &t_data2,Real &mix,DESCFLAGS_GET flags);
//	Bool						(BaseList2D::*Scale						)(Real scale);
//
//	// AtomArray
//	AtomArray*			(*AtomArrayAlloc							)();
//	void						(*AtomArrayFree								)(AtomArray *&obj);
//	LONG						(AtomArray::*GetCount					)() const;
//	C4DAtom *				(AtomArray::*GetIndex					)(LONG idx) const;
//	Bool						(AtomArray::*Append						)(C4DAtom *obj);
//	void						(AtomArray::*Flush						)();
//	Bool						(AtomArray::*AACopyTo					)(AtomArray *dest) const;
//
//	LONG						(AtomArray::*AAGetUserID			)() const;
//	void						(AtomArray::*AASetUserID			)(LONG t_userid);
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
//	Bool						(BaseList2D::*SetAnimatedParameter)(CTrack *track, const DescID &id,const GeData &t_data1,const GeData &t_data2,Real mix,DESCFLAGS_SET flags);
//	void            (AtomArray::*AAFilterObject   )(LONG type, LONG instance, Bool generators);
//	Bool						(AtomArray::*AACopyToFilter	  )(AtomArray *dest, LONG type, LONG instance, Bool generators) const;
//	Bool						(AtomArray::*AAAppendArr  	  )(AtomArray *src);
//	void						(AtomArray::*AAFilterObjectChildren)();
//	Bool						(AtomArray::*AARemove					)(C4DAtom *obj);
//
//	Bool            (C4DAtom::*GetEnabling						)(const DescID &id,const GeData &t_data,DESCFLAGS_ENABLE flags,const BaseContainer *itemdesc);
//	LONG						(AtomArray::*AAGetCountTI			)(LONG type, LONG instance) const;
//
//	Bool						(GeListNode::*IsDocumentRelated)(void) const;
//	LONG						(AtomArray::*AAFind						)(C4DAtom *obj);
//	Bool            (GeListNode::*GetNBit					)(NBIT bit) const;
//	Bool            (GeListNode::*ChangeNBit      )(NBIT bit, NBITCONTROL bitmode);
//	LONG						(GeListNode::*GetBranchInfo   )(BranchInfo *info, LONG max, GETBRANCHINFO flags);
//	LONG						(C4DAtom::*GetClassification	)(void) const;
//	Bool						(BaseList2D::*TransferMarker	)(BaseList2D *dst) const;
//	const GeMarker&	(BaseList2D::*GetMarker				)(void) const;
//	void						(BaseList2D::*SetMarker				)(const GeMarker &m);
//	GeMarker*				(*GeMarkerAlloc								)();
//	void						(*GeMarkerFree								)(GeMarker *&obj);
//	Bool						(GeMarker::*IsEqual						)(const GeMarker &m) const;
//	Bool						(GeMarker::*Content						)() const;
//	LONG						(C4DAtom::*GetRealType				)(void) const;
//	LONG						(GeMarker::*Compare						)(const GeMarker &m) const;
//	void						(GeMarker::*GeMarkerSet				)(const GeMarker &m);
//	Bool						(GeMarker::*GeMarkerRead			)(HyperFile *hf);
//	Bool						(GeMarker::*GeMarkerWrite			)(HyperFile *hf) const;
//	Bool						(BaseList2D::*TransferGoal		)(BaseList2D *dst, Bool undolink);
//	void						(GeMarker::*GeMarkerGetMemory )(void *&data, LONG &size) const;
//	Bool						(BaseList2D::*AddUniqueID     )(LONG appid, const CHAR *const mem, VLONG bytes);
//	Bool						(BaseList2D::*FindUniqueID    )(LONG appid, const CHAR *&mem, VLONG &bytes) const;
//	LONG						(BaseList2D::*GetUniqueIDCount)() const;
//	Bool						(BaseList2D::*GetUniqueIDIndex)(LONG idx, LONG &id, const CHAR *&mem, VLONG &bytes) const;
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
//	Bool						(*GeCoffeeGetLastError				)(Coffee* cof, String *err_string, LONG *err_line, LONG *err_pos);
//	Bool						(*CoffeeEditor_Open						)(BaseList2D *obj,CoffeeEditorCallback *callback);
//
//	VALUE*					(*CoValGetObjMember						)(VALUE *val, LONG nr);
//	VALUE*					(*CoValGetArrayMember					)(VALUE *val, LONG i);
//	void						(*CoValSetArray								)(VALUE *val, ARRAY *a);
//	OBJECT*					(*CoValGetObject							)(VALUE *val, LONG *err);
//	void						(*CoValSetObject							)(VALUE *val, OBJECT *o);
//	void						(*CoValSetString							)(VALUE *val, CSTRING *s);
//	String					(*CoValGetString							)(VALUE *val);
//	Bool						(*CoValIsInstanceOf						)(VALUE *val, VALUE *cl, LONG *err);
//	LONG						(*CoValGetSize								)(VALUE *val);
//	UCHAR*					(*CoValGetBytes								)(VALUE *val);
//
//	VALUE*					(*CoGetGlobalClass						)(Coffee* cof, const String &name);
//	CLASS*					(*CoAddGlobalClass						)(Coffee* cof, const String &name, const String &parent);
//	Bool						(*CoAddGlobalSymbol						)(Coffee* cof, const String &name, const VALUE *v, LONG type);
//	Bool						(*CoAddGlobalFunction					)(Coffee* cof, const String &name, V_CODE fcn);
//	Bool						(*CoAddClassMember						)(Coffee* cof, const String &name, CLASS *c, LONG type);
//	Bool						(*CoAddClassMethod						)(Coffee* cof, const String &name, CLASS *c, LONG type, V_CODE fcn, LONG argc);
//	CSTRING*				(*CoAllocString								)(Coffee* cof, const String &s);
//	OBJECT*					(*CoNewObject									)(Coffee* cof, const String &cl_name);
//	ARRAY*					(*CoNewArray									)(Coffee* cof, LONG size);
//	void						(*CoWrongcnt									)(Coffee* cof, LONG n, LONG cnt);
//	void						(*CoErrCheckType							)(Coffee* cof, VALUE *v, LONG type, LONG *err);
//	void						(*CoErrCheckObjectType				)(Coffee* cof, VALUE *v, const String &cl_name, LONG *err);
//	void						(*CoErrCheckArgCount					)(Coffee* cof, LONG argc, LONG cnt, LONG *err);
//	LONG						(*CoGetType										)(Coffee* cof);
//	const Filename&	(*CoGetRootFile								)(Coffee* cof);
//	OBJECT*					(*CoAllocDynamic							)(Coffee* cof, BaseList2D *bl, Bool coffeeallocation);
//	CLASS*					(*CoAddInheritance						)(Coffee* cof, LONG id, const String &name, const String &from, Bool use_constructor);
//	Coffee*					(*CoGetMaster									)(void);
//	void						(*CoSetError									)(Coffee* cof, LONG type, const String &s1, const String &s2);
//	void						(*CoInstallErrorHook					)(Coffee* cof, COFFEE_ERRORHANDLER *priv_hndl, void *priv_data);
//	void						(*CoSetRootFile								)(Coffee* cof, const Filename &fn);
//};
//
//struct C4D_BaseList
//{
//	LONG						(*GetDiskType					)(const C4DAtom *at);
//	void						(*GetMarker						)(BaseList2D *bl, ULONG *l1, ULONG *l2);
//	void						(*SetAllBits					)(BaseList2D *bl, LONG mask);
//	LONG						(*GetAllBits					)(BaseList2D *bl);
//	void						(*Free								)(C4DAtom *at);
//	Bool						(*Read								)(C4DAtom *at, HyperFile *hf, LONG id, LONG level);
//	Bool						(*Write								)(C4DAtom *at, HyperFile *hf);
//	Bool						(*ReadObject					)(C4DAtom *bn, HyperFile *hf, Bool readheader);
//	Bool						(*WriteObject					)(C4DAtom *bn, HyperFile *hf);
//	void						(*GetData							)(BaseList2D *bl, BaseContainer *ct);
//	void						(*SetData							)(BaseList2D *bl, const BaseContainer *ct, Bool add);
//	BaseContainer*	(*GetDataInstance			)(BaseList2D *bl);
//
//	GeListHead*			(*AllocListHead				)(void);
//	GeListNode*			(*AllocListNode				)(LONG bits, LONG *id_array, LONG id_cnt);
//
//	NodeData*				(*GetNodeData					)(GeListNode *bn, LONG index);
//	LONG						(*GetNodeID						)(GeListNode *bn, LONG index);
//	NODEPLUGIN*			(*RetrieveTable				)(GeListNode *node, LONG index);
//	NODEPLUGIN*			(*RetrieveTableX			)(NodeData *node, LONG index);
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
//	BaseList2D*			 (*Alloc								        )(LONG type);
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
//	BaseTag*				(*Alloc								)(LONG type, LONG count);
//	LONG						(*GetDataCount				)(VariableTag *bt);
//	LONG						(*GetDataSize					)(VariableTag *bt);
//	void*						(*GetDataAddressW			)(VariableTag *bt);
//	BaseSelect*			(*GetBaseSelect				)(SelectionTag *tag);
//	Bool						(*Record							)(StickTextureTag *stt, BaseObject *op, Bool always);
//
//	// UVWs
//	void						(*UvGet								)(UVWTag *tag, LONG i, UVWStruct *s);
//	void						(*UvSet								)(UVWTag *tag, LONG i, UVWStruct *s);
//	void						(*UvCpy								)(UVWTag *tag, LONG dst, UVWTag *srctag, LONG src);
//
//	BaseTag*				(*GetOrigin						)(BaseTag *tag);
//	const void*			(*GetDataAddressR			)(VariableTag *bt);
//	void						(*UvGet2							)(const void *handle, LONG i, UVWStruct *s);
//	void						(*UvSet2							)(void *handle, LONG i, const UVWStruct &s);
//	void						(*UvCpy2							)(const void *srchandle, LONG src, void *dsthandle, LONG dst);
//
//	void						(*NrmGet							)(const void *handle, LONG i, NormalStruct *s);
//	void						(*NrmSet							)(void *handle, LONG i, const NormalStruct &s);
//	void						(*NrmCpy							)(const void *srchandle, LONG src, void *dsthandle, LONG dst);
//};
//
//struct C4D_Object
//{
//	BaseObject*			(*Alloc								)(LONG type);
//	SplineObject*		(*AllocSplineObject		)(LONG pcnt, SPLINETYPE type);
//	Real						(*GetVisibility				)(BaseObject *op, Real parent);
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
//	LONG						(*GetMode							)(BaseObject *op, OBJECTSTATE mode);
//	void						(*SetMode							)(BaseObject *op, OBJECTSTATE mode, LONG val);
//	BaseTag*				(*GetFirstTag					)(BaseObject *op);
//	BaseTag*				(*GetTag							)(BaseObject *op, LONG type, LONG nr);
//	void*						(*GetTagData					)(BaseObject *op, LONG type, LONG nr);
//	LONG						(*GetTagDataCount			)(const BaseObject *op, LONG type);
//	void						(*InsertTag						)(BaseObject *op, BaseTag *tp, BaseTag *pred);
//	void						(*KillTag							)(BaseObject *op, LONG type, LONG nr);
//	LONG						(*GetInfo							)(GeListNode *op);
//	Bool						(*Edit								)(BaseList2D *op);
//	BaseObject*			(*GetCache						)(BaseObject *op, HierarchyHelp *hh);
//	BaseObject*			(*GetDeformCache			)(BaseObject *op);
//	LineObject*			(*GetIsoparm					)(BaseObject *op);
//	Bool						(*IsDirty							)(BaseObject *op, DIRTYFLAGS flags);
//	void						(*SetDirty						)(C4DAtom *op, DIRTYFLAGS flags);
//	Bool						(*CheckCache					)(BaseObject *op, HierarchyHelp *hh);
//	void						(*SetIsoparm					)(BaseObject *op, LineObject *l);
//	BaseObject*			(*GenPrimitive				)(BaseDocument *doc, LONG type, const BaseContainer *bc, Real lod, Bool isoparm, BaseThread *bt);
//	BaseObject*			(*GenSplinePrimitive	)(BaseDocument *doc, LONG type, const BaseContainer *bc, Real lod, BaseThread *bt);
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
//	Bool						(*PoResizeObject			)(PointObject *op, LONG pcnt);
//	SReal*					(*PoCalcVertexMap			)(PointObject *op, BaseObject *modifier);
//
//	// line object
//	Bool						(*LoResizeObject			)(LineObject *op, LONG pcnt, LONG scnt);
//
//	// polygon object
//	BaseSelect*			(*PyGetPolygonS				)(PolygonObject *op);
//	BaseSelect*			(*PyGetPolygonH				)(PolygonObject *op);
//	Bool						(*PyResizeObject			)(PolygonObject *op, LONG pcnt, LONG vcnt);
//
//	// spline object
//	Vector					(*SpGetSplinePoint		)(SplineObject *op, Real t, LONG segment, const Vector *padr);
//	Vector					(*SpGetSplineTangent	)(SplineObject *op, Real t, LONG segment, const Vector *padr);
//
//	SplineLengthData* (*SpLDAlloc					)(void);
//	void						(*SpLDFree						)(SplineLengthData *&sp);
//
//	Bool						(*SpInitLength				)(SplineLengthData *dat, SplineObject *op, LONG segment, const Vector *padr);
//	Real						(*SpUniformToNatural	)(SplineLengthData *dat, Real t);
//	Real						(*SpGetLength					)(SplineLengthData *dat);
//	Real						(*SpGetSegmentLength	)(SplineLengthData *dat, LONG a, LONG b);
//	LineObject*			(*SpGetLineObject			)(SplineObject *op, BaseDocument *doc, Real lod, BaseThread *thread);
//	SplineObject*		(*SpGetRealSpline			)(BaseObject *op);
//	Bool						(*SpResizeObject			)(SplineObject *op, LONG pcnt, LONG scnt);
//
//	// particle object
//	LONG						(*PrGetCount					)(BaseObject *op);
//	Real						(*PrGetLifetime				)(BaseObject *op);
//	Particle*				(*PrGetParticleW			)(BaseObject *op, ParticleTag *pt, LONG i);
//	Bool						(*PrIsMatrixAvailable	)(BaseObject *op);
//	ParticleDetails*(*PrGetParticleDetails)(BaseDocument *doc, BaseObject *op);
//
//	// camera object
//	LONG						(*CoGetProjection			)(BaseObject *op);
//	Real						(*CoGetFocus					)(BaseObject *op);
//	Real						(*CoGetZoom						)(BaseObject *op);
//	Vector					(*CoGetOffset					)(BaseObject *op);
//	Real						(*CoGetAperture				)(BaseObject *op);
//	Bool						(*CoSetProjection			)(BaseObject *op, LONG projection);
//	Bool						(*CoSetFocus					)(BaseObject *op, Real v);
//	Bool						(*CoSetZoom						)(BaseObject *op, Real v);
//	Bool						(*CoSetOffset					)(BaseObject *op, const Vector &offset);
//	Bool						(*CoSetAperture				)(BaseObject *op, Real v);
//
//	// object safety
//	ObjectSafety*		(*OsAlloc							)(BaseObject *op);
//	void						(*OsFree							)(ObjectSafety *os, Bool restore);
//
//	// triangulation
//	Bool						(*Triangulate					)(const Vector *padr, LONG pcnt, CPolygon **vadr, LONG *vcnt);
//	PolygonObject*	(*TriangulateLine			)(LineObject *op, Real regular, BaseThread *bt);
//	SplineObject*		(*FitCurve						)(Vector *padr, LONG pcnt, Real error, BaseThread *bt);
//
//	// uv stuff
//	UVWTag*					(*GenerateUVW					)(BaseObject *op, const Matrix &opmg, TextureTag *tp, const Matrix &texopmg, BaseView *view);
//
//	ULONG						(*GetDirty						)(const C4DAtom *op, DIRTYFLAGS flags);
//
//	Bool						(*TriangulateStandard )(const Vector *padr, LONG pcnt, const LONG *list, LONG lcnt, CPolygon *&vadr, LONG &vcnt, BaseThread *thread);
//	Bool						(*TriangulateRegular	)(const Vector *pinp, LONG pinp_cnt, const LONG *list, LONG lcnt, Vector *&padr, LONG &pcnt, CPolygon *&vadr, LONG &vcnt, Real regular_width, BaseThread *thread);
//
//	Bool						(*SpSetDefaultCoeff		)(SplineObject *op);
//	BaseObject*			(*GenerateText				)(BaseContainer *cp, BaseThread *bt, Bool separate);
//
//	BaseSelect*			(*PyGetEdgeS					)(PolygonObject *op);
//	BaseSelect*			(*PyGetEdgeH					)(PolygonObject *op);
//	void						(*GetColorProperties	)(BaseObject *op, ObjectColorProperties *co);
//	void						(*SetColorProperties	)(BaseObject *op, ObjectColorProperties *co);
//
//	Bool						(*CopyTagsTo					)(BaseObject *op, BaseObject *dest, LONG visible, LONG variable, LONG hierarchical, AliasTrans *trans);
//	BaseObject*			(*GetHierarchyClone		)(BaseObject *op,HierarchyHelp *hh, BaseObject *pp, HIERARCHYCLONEFLAGS flags, Bool *dirty, AliasTrans *trans);
//
//	BaseObject*			(*GetCacheParent			)(BaseObject *op);
//	Bool						(*CheckDisplayFilter	)(BaseObject *op, LONG flags);
//
//	BaseSelect*			(*PyGetPhongBreak			)(PolygonObject *op);
//
//	LONG						(*GetUniqueIP					)(BaseObject *op);
//	void						(*SetUniqueIP					)(BaseObject *op, LONG ip);
//
//	void						(*SetOrigin						)(BaseObject *op, BaseObject *origin);
//	BaseObject*			(*GetOrigin						)(BaseObject *op, Bool safe);
//	BaseObject*			(*InternalCalcBooleOld)(BaseObject *curr,LONG booletype, HierarchyHelp *hh);
//
//	SVector*				(*CreatePhongNormals	)(PolygonObject *op);
//
//	// triangulation
//	PolyTriangulate* (*PolyTriangAlloc    )();
//	void            (*PolyTriangFree      )(PolyTriangulate *&pTriang);
//	Bool            (*PolyTriangTriang    )(PolyTriangulate* pTriang, const Vector* pvPoints, const LONG lPointCount, const LONG* plSegments, const LONG lSegCnt,
//		CPolygon*& pPolys, LONG& lPolyCount, LONG lFlags, const LONG* plMap, BaseThread* pThread,
//		const LONG lConstraints, const LONG* plConstrainedEdges);
//	Bool            (*PolyTriangTriangRelease)(PolyTriangulate* pTriang, const Vector* pvPoints, const LONG lPointCount, const LONG* plSegments, const LONG lSegCnt,
//		CPolygon*& pPolys, LONG& lPolyCount, LONG lFlags, const LONG* plMap, BaseThread* pThread,
//		const LONG lConstraints, const LONG* plConstrainedEdges);
//
//	Bool						(*PyGetPolygonTranslationMap)(PolygonObject *op, LONG &ngoncnt, LONG *&polymap);
//	Bool						(*PyGetNGonTranslationMap)(PolygonObject *op, const LONG ngoncnt, const LONG *polymap, LONG **&ngons);
//	Pgon*           (*PyGetNgon)(PolygonObject *op);
//	LONG            (*PyGetNgonCount)(PolygonObject *op);
//	Bool						(*PyResizeObjectNgon	)(PolygonObject *op, LONG pcnt, LONG vcnt, LONG ncnt);
//	void						(*PyGetSelectedNgons  )(PolygonObject *op, BaseSelect* sel);
//	void						(*PyGetHiddenNgons    )(PolygonObject *op, BaseSelect* sel);
//	NgonBase*				(*PyGetNgonBase)(PolygonObject *op);
//	Bool						(*PyValidateEdgeSelection)(PolygonObject *op, BaseSelect* sel);
//	Bool						(*PyGetEdgeSelection)(PolygonObject *op, BaseSelect* sel, EDGESELECTIONTYPE type);
//
//	BaseObject*			(*GetTopOrigin						)(BaseObject *op, Bool parent, Bool safe);
//	BaseObject*			(*GetEditObject						)(BaseObject *op, BaseObject **psds, DISPLAYEDITSTATE state);
//	LONG						(*GetHighlightHandle			)(BaseObject *op, BaseDraw *bd);
//
//	const Matrix &  (*GetModelingAxis)(BaseObject *op, BaseDocument *doc);
//	void            (*SetModelingAxis)(BaseObject *op, const Matrix &m);
//	Bool            (*PolyTriangHasIdentical)(PolyTriangulate* pTriang);
//	Bool            (*CalculateVisiblePoints)(BaseDraw *bd, PolygonObject *op, Vector *padr, UCHAR *pset, Bool select_visibonly);
//	LONG            (*PolyTriangGetType)(PolyTriangulate* pTriang);
//
//	void            (*PyGetNgonEdgesCompact)(PolygonObject *op, UCHAR *&edges);
//	ULONG						(*PyVBOInitUpdate)(PolygonObject *op, BaseDraw* bd);
//	Bool						(*PyVBOStartUpdate)(PolygonObject *op, BaseDraw* bd);
//	void						(*PyVBOUpdateVector)(PolygonObject *op, LONG i, const SVector &v, ULONG flags);
//	void						(*PyVBOEndUpdate)(PolygonObject *op, BaseDraw* bd);
//	void						(*PyVBOFreeUpdate)(PolygonObject *op);
//
//	LONG						(*IntersectionTest			)(PolygonObject *op, BaseDraw *bd, Real x, Real y, const Matrix &mg, Real *z, MODELINGCOMMANDMODE mode, UCHAR *pPointSelect, LONG lSelectCount);
//
//	Bool						(*PyValidateEdgeSelectionA)(PolygonObject *op);
//
//#ifdef __SINGLEPRECISION
//	Bool            (*PolyTriangTriangA   )(PolyTriangulate* pTriang, const LVector* pvPoints, const LONG lPointCount, const LONG* plSegments, const LONG lSegCnt,
//		CPolygon*& pPolys, LONG& lPolyCount, LONG lFlags, const LONG* plMap, BaseThread* pThread,
//		const LONG lConstraints, const LONG* plConstrainedEdges);
//	Bool            (*PolyTriangTriangReleaseA)(PolyTriangulate* pTriang, const LVector* pvPoints, const LONG lPointCount, const LONG* plSegments, const LONG lSegCnt,
//		CPolygon*& pPolys, LONG& lPolyCount, LONG lFlags, const LONG* plMap, BaseThread* pThread,
//		const LONG lConstraints, const LONG* plConstrainedEdges);
//#endif
//
//	void            (*PolyTriangSetPolygonMatrix)(PolyTriangulate* pTriang, LMatrix* m);
//	ULONG           (*PolyTriangGetState)(PolyTriangulate* pTriang);
//	const Particle*	(*PrGetParticleR			)(BaseObject *op, ParticleTag *pt, LONG i);
//
//	void						(*GetIcon							)(BaseList2D *op, IconData *dat);
//
//	ULONG						(C4DAtom::*GetHDirty	)(HDIRTYFLAGS mask) const;
//	void						(C4DAtom::*SetHDirty	)(HDIRTYFLAGS mask);
//
//	const void*			(*GetTagDataR					)(const BaseObject *op, LONG type, LONG nr);
//
//	void						(*RemoveFromCache			)(BaseObject *op);
//	Bool						(*SearchHierarchy)(BaseObject *op, BaseObject *find);
//	Bool						(*PyResizeObjectNgonFlags	)(PolygonObject *op, LONG pcnt, LONG vcnt, LONG ncnt, LONG vc_flags);
//	void						(*CopyMatrixTo						)(BaseObject *src, BaseObject *dst);
//	LONG						(*CoAnaglyphGetCameraCount)(const BaseObject* op, BaseDocument* doc, BaseDraw* bd, RenderData* rd, LONG flags);
//	Bool						(*CoAnaglyphGetCameraInfo)(const BaseObject* op, BaseDocument* doc, BaseDraw* bd, RenderData* rd, LONG n, AnaglyphCameraInfo &info, LONG flags);
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
//	BaseObject*			(*GetHighest					)(BaseDocument *doc, LONG type, Bool editor);
//	BaseMaterial*		(*SearchMaterial			)(BaseDocument *doc, const String *str, Bool inc);
//	BaseObject*			(*SearchObject				)(BaseDocument *doc, const String *str, Bool inc);
//	Bool						(*StartUndo						)(BaseDocument *doc);
//	Bool						(*EndUndo							)(BaseDocument *doc);
//	Bool						(*AddUndo							)(BaseDocument *doc, UNDOTYPE type, void *data);
//	Bool						(*DoRedo							)(BaseDocument *doc);
//	void						(*AnimateObject				)(BaseDocument *doc, BaseList2D *op, const BaseTime &time, ANIMATEFLAGS flags);
//	BaseDraw*				(*GetActiveBaseDraw		)(BaseDocument *doc);
//	BaseDraw*				(*GetRenderBaseDraw		)(BaseDocument *doc);
//	BaseDraw*				(*GetBaseDraw					)(BaseDocument *doc, LONG num);
//	LONG						(*GetSplinePlane			)(BaseDocument *doc);
//
//	// hierarchy help
//	Real						(*HhGetLOD						)(HierarchyHelp *hh);
//	BUILDFLAGS			(*HhGetBuildFlags			)(HierarchyHelp *hh);
//	BaseThread*			(*HhGetThread					)(HierarchyHelp *hh);
//	BaseDocument*		(*HhGetDocument				)(HierarchyHelp *hh);
//	const Matrix&		(*HhGetMg							)(HierarchyHelp *hh);
//
//	// hierarchy
//	Bool						(*RunHierarchy				)(void *main, BaseDocument *doc, Bool spheres, Real lod, Bool uselod, BUILDFLAGS flags, void *startdata, BaseThread *bt, HierarchyAlloc *ha, HierarchyFree *hf, HierarchyCopyTo *hc, HierarchyDo *hd);
//
//	BaseSceneHook*  (*FindSceneHook				)(const BaseDocument *doc,LONG id);
//
//	void						(BaseDocument::*SetActiveObject		)(BaseObject *op,LONG mode);
//	void						(BaseDocument::*GetActiveObjects	)(AtomArray &selection,Bool children) const;
//	void						(BaseDocument::*GetActiveTags			)(AtomArray &selection) const;
//
//	void						(*PrAdd								)(PriorityList *list, GeListNode *node, LONG priority, EXECUTIONFLAGS flags);
//	BaseObject*			(*GetHelperAxis				)(BaseDocument *doc);
//	BaseVideoPost*  (*RdGetFirstVideoPost	)(RenderData *rd);
//	void						(*RdInsertVideoPost		)(RenderData *rd, BaseVideoPost *pvp, BaseVideoPost *pred);
//	void						(BaseDocument::*GetActiveMaterials)(AtomArray &selection) const;
//
//	void						(*SetRewind						)(BaseDocument *doc, LONG flags);
//
//	void						(BaseDocument::*SetActiveTag			)(BaseTag *op,LONG mode);
//	void						(BaseDocument::*SetActiveMaterial	)(BaseMaterial *mat,LONG mode);
//
//	BaseVideoPost*(*VpAlloc								)(LONG type);
//
//	BaseList2D*     (*GetUndoPtr            )(BaseDocument *doc);
//	void            (*AutoKey               )(BaseDocument *doc, BaseList2D *undo, BaseList2D *op, Bool recursive, Bool pos, Bool scale, Bool rot, Bool param, Bool pla);
//	Bool						(*DoUndo								)(BaseDocument *doc, Bool multiple);
//	Bool						(*IsCacheBuilt				  )(BaseDocument *doc, Bool force);
//	void						(BaseDocument::*GetActivePolygonObjects)(AtomArray &selection,Bool children) const;
//	BaseTime				(*GetUsedTime						)(BaseDocument *doc, BaseList2D *check_op, Bool min);
//	void						(*ForceCreateBaseDraw		)(BaseDocument *doc);
//
//	BaseContainer*  (*GetDataInstance				)(BaseDocument *doc, LONG type);
//	void						(*RunAnimation					)(BaseDocument *doc, Bool forward, Bool stop);
//	void						(*SetDocumentTime				)(BaseDocument *doc, const BaseTime &time);
//
//	BaseDocument*		(*IsolateObjects				)(BaseDocument *doc,const AtomArray &t_objects);
//
//	void						(BaseDocument::*GetSelection)(AtomArray &selection) const;
//	void						(BaseDocument::*SetSelection      )(BaseList2D *bl,LONG mode);
//
//	// layers
//	LayerObject*		(*DlAlloc							  )(void);
//	GeListHead*     (*GetLayerObjectRoot    )(BaseDocument *doc);
//	Bool            (*HandleSelectedTextureFilename)(BaseDocument *doc, BaseChannel *bc, const Filename &fn, Filename *resfilename, Bool undo, GEMB_R *already_answered);
//	Bool            (*ReceiveMaterials             )(BaseDocument *doc, BaseObject *op, AtomArray *mat, Bool clearfirst);
//	Bool            (*ReceiveNewTexture            )(BaseDocument *doc, BaseObject *op, const Filename &filename, Bool sdown, GEMB_R *already_answered);
//
//	void            (*SetKeyDefault         )(BaseDocument *doc, CCurve *seq, LONG kidx);
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
//	LONG						(*GetDrawTime						)(BaseDocument *doc);
//	Bool						(*StopExternalRenderer	)();
//	LONG						(*GetBaseDrawCount			)(BaseDocument *doc);
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
//	LONG						(*GetCPUCount					)(void);
//	ULONG						(*GetCurrentThreadId	)(void);
//	BaseThread*			(*GetCurrentThread		)(void);
//
//	MPBaseThread*		(*MPAlloc							)(BaseThread *parent, LONG count, ThreadMain *tm, ThreadTest *tt, void **data, ThreadName *tn);
//	void						(*MPFree							)(MPBaseThread *mp);
//	BaseThread*			(*MPGetThread					)(MPBaseThread *mp, LONG i);
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
//	Bool						(GeSignal::*SIGWait		)(LONG timeout);
//};
//
//struct C4D_Material
//{
//	BaseMaterial*		(*Alloc								)(LONG type);
//	void						(*Update							)(BaseMaterial *mat, LONG preview, Bool rttm);
//	BaseChannel*		(*GetChannel					)(BaseMaterial *bm, LONG id);
//	Bool						(*GetChannelState			)(Material *mat, LONG channel);
//	void						(*SetChannelState			)(Material *mat, LONG channel, Bool state);
//	Bool						(*Compare							)(BaseMaterial *m1, BaseMaterial *m2);
//	BaseBitmap*			(*GetPreview					)(BaseMaterial *bm, LONG flags);
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
//	Vector					(*GetAverageColor			)(BaseMaterial *mat, LONG channel);
//	LONG						(BaseMaterial::*GlMessage	)(LONG type, void *data);
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
//	LONG						(*GetCount						)(const BaseSelect *bs);
//	LONG						(*GetSegments					)(const BaseSelect *bs);
//	Bool						(*Select							)(BaseSelect *bs, LONG num);
//	Bool						(*SelectAll						)(BaseSelect *bs, LONG min, LONG max);
//	Bool						(*Deselect						)(BaseSelect *bs, LONG num);
//	Bool						(*DeselectAll					)(BaseSelect *bs);
//	Bool						(*Toggle							)(BaseSelect *bs, LONG num);
//	Bool						(*ToggleAll						)(BaseSelect *bs, LONG min, LONG max);
//	Bool						(*GetRange						)(const BaseSelect *bs, LONG seg, LONG *a, LONG *b);
//	Bool						(*IsSelected					)(const BaseSelect *bs, LONG num);
//	Bool						(*CopyTo							)(const BaseSelect *bs, BaseSelect *dest);
//	BaseSelect*			(*GetClone						)(const BaseSelect *bs);
//	Bool						(*FromArray						)(BaseSelect *bs, UCHAR *selection, LONG count);
//	UCHAR*					(*ToArray							)(const BaseSelect *bs, LONG count);
//	Bool						(*Merge 							)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*DeselectBS					)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*Cross								)(BaseSelect *bs, const BaseSelect *src);
//	Bool						(*FindSegment					)(const BaseSelect *bs, LONG num, LONG *segment);
//	BaseSelectData*	(*GetData							)(BaseSelect *bs);
//	Bool						(*CopyFrom						)(BaseSelect *bs, BaseSelectData *ndata, LONG ncnt);
//	LONG						(*GetDirty						)(const BaseSelect *bs);
//};
//
//struct C4D_CAnimation
//{
//	BaseTime        (CKey::*GetTime               )(void) const;
//	Bool            (CKey::*CopyDataTo            )(CCurve *destseq, CKey *dest, AliasTrans *trans) const;
//	void            (CKey::*FlushData1            )(void);
//	BaseTime				(CKey::*GetTimeLeft						)(void) const;
//	BaseTime				(CKey::*GetTimeRight					)(void) const;
//	Real						(CKey::*GetValue							)(void) const;
//	Real						(CKey::*GetValueLeft					)(void) const;
//	Real						(CKey::*GetValueRight					)(void) const;
//	CINTERPOLATION  (CKey::*GetInterpolation      )(void) const;
//	void						(CKey::*SetTime								)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetTimeLeft						)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetTimeRight					)(CCurve *seq, const BaseTime &t);
//	void						(CKey::*SetValue							)(CCurve *seq, Real v);
//	void						(CKey::*SetValueLeft					)(CCurve *seq, Real v);
//	void						(CKey::*SetValueRight					)(CCurve *seq, Real v);
//	void						(CKey::*SetGeData							)(CCurve *seq, const GeData &d);
//	void						(CKey::*SetInterpolation      )(CCurve *seq, CINTERPOLATION inter);
//	CTrack*					(CKey::*GetTrackCKey					)(void);
//	CCurve*	    		(CKey::*GetSequenceCKey				)(void);
//	const GeData&   (CKey::*GetGeData             )(void) const;
//	CKey*						(*CKey_Alloc									)(void);
//	void						(*CKey_Free										)(CKey *&ckey);
//
//	LONG            (CCurve::*GetKeyCount         )(void) const;
//	CKey*           (CCurve::*GetKey1             )(LONG index);
//	const CKey*     (CCurve::*GetKey2             )(LONG index) const;
//	CKey*           (CCurve::*FindKey1            )(const BaseTime &time, LONG *nidx, FINDANIM match);
//	const CKey*     (CCurve::*FindKey2            )(const BaseTime &time, LONG *nidx, FINDANIM match) const;
//	CKey*           (CCurve::*AddKey              )(const BaseTime &time, LONG *nidx);
//	Bool						(CCurve::*InsertKey				    )(const CKey *key);
//	Bool            (CCurve::*DelKey              )(LONG index);
//	LONG						(CCurve::*MoveKey					    )(const BaseTime &time, LONG idx, CCurve *dseq);
//	void            (CCurve::*FlushKeys           )(void);
//	void						(CCurve::*SortKeysByTime		  )(void);
//	void            (CCurve::*CalcSoftTangents    )(LONG kidx, Real *vl, Real *vr, BaseTime *tl, BaseTime *tr) const;
//	LReal           (CCurve::*CalcHermite         )(LReal time, LReal t1, LReal t2, LReal val1, LReal val2, LReal tan1_val, LReal tan2_val, LReal tan1_t, LReal tan2_t, Bool deriv) const;
//	void	          (CCurve::*GetTangents         )(LONG kidx, LReal *vl, LReal *vr, LReal *tl, LReal *tr) const;
//
//	Real            (CCurve::*GetValue1           )(const BaseTime &time,LONG fps) const;
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
//	Real            (CTrack::*GetValue3           )(BaseDocument *doc, const BaseTime &time, LONG fps);
//	Bool            (CTrack::*Remap								)(LReal time, LReal *ret_time, LONG *ret_cycle) const;
//	LONG						(CTrack::*GuiMessage					)(const BaseContainer &msg,BaseContainer &result);
//	LONG						(CTrack::*GetHeight						)();
//	Bool						(CTrack::*FillKey							)(BaseDocument *doc, BaseList2D *bl, CKey *key);
//	Bool						(CTrack::*TrackInformation    )(BaseDocument *doc, CKey *key, String *str, Bool set);
//
//	const BaseContainer *(CTrack::*GetParameterDescription)(BaseContainer &temp) const;
//
//	LONG						(CTrack::*GetTrackCategory    )(void) const;
//	Bool            (CTrack::*AnimateTrack        )(BaseDocument *doc, BaseList2D *op, const BaseTime &tt, const LONG flags, Bool *chg, void *data);
//	LONG						(CTrack::*GetUnit							)(Real *step);
//
//	LONG						(CTrack::*GetTLHeight					)(LONG id);
//	void						(CTrack::*SetTLHeight					)(LONG id,LONG size);
//};
//
//
//struct C4D_BaseSound
//{
//	BaseSound*			(*Alloc								)(void);
//	void						(*Free								)(BaseSound *bs);
//	BaseSound*			(*GetClone						)(BaseSound *bs);
//	Bool						(*CopyTo							)(BaseSound *bs, BaseSound *dest);
//	Bool						(*Init								)(BaseSound *bs, LONG samples, LONG fmode, Bool stereo, Bool b16);
//	void						(*FlushAll						)(BaseSound *bs);
//	Bool						(*Load								)(BaseSound *bs, const Filename *fn);
//	Bool						(*Save								)(BaseSound *bs, const Filename *fn);
//	void						(*GetSample						)(BaseSound *bs, LONG i, SData *data);
//	void						(*SetSample						)(BaseSound *bs, LONG i, SData *data);
//	BaseBitmap*			(*GetBitmap						)(BaseSound *bs, LONG width, LONG height, const BaseTime &start, const BaseTime &stop);
//	CHAR*						(*GetRAW							)(BaseSound *bs);
//	Bool            (*WriteIt             )(BaseSound *bs, HyperFile *hf);
//	Bool            (*ReadIt              )(BaseSound *bs, HyperFile *hf, LONG level);
//	BaseBitmap*			(*GetBitmap2					)(BaseSound *bs, LONG width, LONG height, const BaseTime &start, const BaseTime &stop, const Vector &draw_col, const Vector &back_col);
//	GeListHead*     (*GetMarkerRoot       )(BaseSound *bs);
//	BaseSound*			(*GetClonePart				)(BaseSound *bs,	const BaseTime &start,	const BaseTime &stop,	Bool reverse);
//	CHAR*						(*GetSoundInfo				)(BaseSound *bs, Bool *stereo, Bool *b16, LONG *frequency, LONG *samples, BaseTime *length);
//};
//
//struct C4D_BaseDraw
//{
//	// basedraw
//	Bool						(*HasCameraLink				)(BaseDraw *bd);
//	BaseObject*			(*GetEditorCamera			)(BaseDraw *bd);
//	Vector					(*CheckColor					)(BaseDraw *bd, const Vector &col);
//	void						(*SetTransparency			)(BaseDraw *bd, LONG trans);
//	LONG						(*GetTransparency			)(BaseDraw *bd);
//	Bool						(*PointInRange				)(BaseDraw *bd, const Vector &p, LONG x, LONG y);
//	void						(*SetPen							)(BaseDraw *bd, const Vector &col, LONG flags);
//	Real						(*SimpleShade					)(BaseDraw *bd, const Vector &p, const Vector &n);
//	void						(*DrawPoint2D					)(BaseDraw *bd, const Vector &p);
//	void						(*DrawLine2D					)(BaseDraw *bd, const Vector &p1, const Vector &p2); // draw line with 2D clipping
//	void						(*DrawHandle2D				)(BaseDraw *bd, const Vector &p, DRAWHANDLE type);
//	void						(*DrawCircle2D				)(BaseDraw *bd, LONG mx, LONG my, Real rad);
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
//	void            (*LineZOffset         )(BaseDraw *bd, LONG offset);
//	void            (*SetMatrix_Projection)(BaseDraw *bd);
//	void            (*SetMatrix_Screen    )(BaseDraw *bd);
//	void            (*SetMatrix_Camera    )(BaseDraw *bd);
//	void            (*SetMatrix_Matrix    )(BaseDraw *bd, BaseObject *op,const Matrix &mg);
//	void            (*DrawLine            )(BaseDraw *bd, const Vector &p1, const Vector &p2,LONG flags);
//	void            (*LineStripBegin      )(BaseDraw *bd);
//	void            (*LineStrip           )(BaseDraw *bd, const Vector &vp,const Vector &vc,LONG flags);
//	void            (*LineStripEnd        )(BaseDraw *bd);
//	void            (*DrawHandle          )(BaseDraw *bd, const Vector &vp,DRAWHANDLE type,LONG flags);
//	void						(*DrawTexture					)(BaseDraw *bd, BaseBitmap *bmp, Vector *padr4, Vector *cadr, Vector *vnadr, Vector *uvadr, LONG pntcnt, DRAW_ALPHA alphamode, DRAW_TEXTUREFLAGS flags);
//	void						(*SetLightList				)(BaseDraw *bd, LONG mode);
//
//	void						(*InitUndo						)(BaseDraw *bd, BaseDocument *doc);
//	void						(*DoUndo							)(BaseDraw *bd, BaseDocument *doc);
//	void						(*SetDrawParam				)(BaseDraw *bd, LONG id,const GeData &data);
//	GeData					(*GetDrawParam				)(BaseDraw *bd, LONG id);
//
//	void						(*DrawPoly						)(BaseDraw *bd, Vector *vp,Vector *vf,Vector *vn,LONG anz,LONG flags);
//	DISPLAYFILTER		(*GetDisplayFilter		)(BaseDraw *bd);
//	DISPLAYEDITSTATE(*GetEditState			)(BaseDraw *bd);
//	Bool						(*IsViewOpen					)(BaseDraw *bd, BaseDocument *doc);
//
//	void						(*DrawCircle					)(BaseDraw *bd, const Matrix &m);
//	void						(*DrawBox 						)(BaseDraw *bd, const Matrix &m, Real size, const Vector &col, Bool wire);
//	void						(*DrawPolygon					)(BaseDraw *bd, Vector *p, Vector *f, Bool quad);
//	void						(*DrawSphere					)(BaseDraw *bd, const Vector &off, const Vector &size, const Vector &col, LONG flags);
//	Bool						(*TestBreak						)(BaseDraw *bd);
//	void						(*DrawArrayEnd				)(BaseDraw *bd);
//	OITInfo&				(*GetOITInfo					)(BaseDraw *bd);
//	LONG						(*GetGlLightCount			)(const BaseDraw *bd);
//	const GlLight*	(*GetGlLight					)(const BaseDraw *bd, LONG lIndex);
//
//	Bool						(*GetFullscreenPolygonVectors)(BaseDraw *bd, LONG &lAttributeCount, const GlVertexBufferAttributeInfo* const *&ppAttibuteInfo, LONG &lVectorInfoCount, const GlVertexBufferVectorInfo* const* &ppVectorInfo);
//	Bool						(*DrawFullscreenPolygon)(BaseDraw *bd, LONG lVectorInfoCount, const GlVertexBufferVectorInfo* const* ppVectorInfo);
//	Bool						(*AddToPostPass				)(BaseDraw *bd, BaseObject *op,BaseDrawHelp *bh, LONG flags);
//	Bool						(*GetDrawStatistics		)(const BaseDraw* bd, BaseContainer &bc);
//
//	void            (*SetMatrix_MatrixO   )(BaseDraw *bd, BaseObject *op,const Matrix &mg, LONG zoffset);
//	void            (*SetMatrix_ScreenO   )(BaseDraw *bd, LONG zoffset);
//	void            (*SetMatrix_CameraO   )(BaseDraw *bd, LONG zoffset);
//
//	EditorWindow*		(*GetEditorWindow)(BaseDraw *bd);
//
//	void						(*DrawPointArray			)(BaseDraw *bd, LONG cnt, const SVector *vp, const SReal *vc, LONG colcnt, const SVector *vn);
//	void						(*DrawTexture1				)(BaseDraw *bd, C4DGLuint bmp, Vector *padr4, Vector *cadr, Vector *vnadr, Vector *uvadr, LONG pntcnt, DRAW_ALPHA alphamode);
//	void						(*InitClipbox					)(BaseDraw *bd, LONG left, LONG top, LONG right, LONG bottom, LONG flags);
//	void						(*InitView						)(BaseDraw *bd, BaseContainer *camera, const Matrix &op_m, Real sv, Real pix_x, Real pix_y, Bool fitview);
//	void						(*InitializeView			)(BaseDraw *bd, BaseDocument *doc, BaseObject *cam, Bool editorsv);
//	void						(*SetTexture					)(BaseDraw *bd, BaseBitmap *bm, Bool tile, DRAW_ALPHA alphamode, DRAW_TEXTUREFLAGS flags);
//	void						(*SetSceneCamera			)(BaseDraw *bd, BaseObject *op, Bool animate);
//	void            (*SetMatrix_ScreenOM  )(BaseDraw *bd, LONG zoffset, const Matrix4* m);
//
//	Bool						(*InitDrawXORLine			)(BaseDraw *bd);
//	void						(*FreeDrawXORLine			)(BaseDraw *bd);
//	void						(*DrawXORPolyLine			)(BaseDraw *bd, const SReal* p, LONG cnt);
//	void						(*BeginDrawXORPolyLine)(BaseDraw *bd);
//	void						(*EndDrawXORPolyLine	)(BaseDraw *bd, Bool blit);
//	BaseDraw*				(*AllocBaseDraw				)();
//	void						(*FreeBaseDraw				)(BaseDraw*& bv);
//
//	Bool						(*DrawScene						)(BaseDraw* bd, LONG flags);
//	DISPLAYMODE			(*GetReductionMode		)(const BaseDraw* bd);
//	void						(*GlDebugString				)(BaseDraw* bd, const char* str);
//	void						(*SetPointSize				)(BaseDraw* bd, Real pointsize);
//};
//
//struct C4D_BaseView
//{
//	void						(*GetFrame						)(BaseView *bv, LONG *cl, LONG *ct, LONG *cr, LONG *cb);
//	void						(*GetSafeFrame				)(BaseView *bv, LONG *from, LONG *to, LONG *horizontal);
//	void						(*GetParameter				)(const BaseView *bv, Vector *offset, Vector *scale, Vector *scale_z);
//	Matrix					(*GetMg								)(BaseView *bv);
//	Matrix					(*GetMi								)(BaseView *bv);
//	LONG						(*GetProjection				)(BaseView *bv);
//	Bool						(*TestPoint						)(BaseView *bv, Real x, Real y);
//	Bool						(*TestPointZ					)(BaseView *bv, const Vector &p);
//	Bool						(*TestClipping3D			)(BaseView *bv, const Vector &mp, const Vector &rad, const Matrix &mg, Bool *clip2d, Bool *clipz);
//	Bool						(*ClipLine2D					)(BaseView *bv, Vector *p1, Vector *p2);
//	Bool						(*ClipLineZ						)(BaseView *bv, Vector *p1, Vector *p2);
//	Vector					(*WS									)(BaseView *bv, const Vector &p);
//	Vector					(*SW									)(BaseView *bv, const Vector &p);
//	Vector					(*SW_R								)(BaseView *bv, Real x, Real y, const Vector &wp);
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
//	Bool						(*VSInitObj 					)(ViewportSelect *vs, LONG w, LONG h, BaseDraw* bd, BaseObject* op, LONG mode, Bool onlyvisible, VIEWPORTSELECTFLAGS flags);
//	Bool						(*VSInitAr  					)(ViewportSelect *vs, LONG w, LONG h, BaseDraw* bd, AtomArray* ar, LONG mode, Bool onlyvisible, VIEWPORTSELECTFLAGS flags);
//	ViewportPixel*	(*VSGetPixelInfoPoint )(ViewportSelect *vs, LONG x, LONG y);
//	ViewportPixel*	(*VSGetPixelInfoPolygon)(ViewportSelect *vs, LONG x, LONG y);
//	ViewportPixel*	(*VSGetPixelInfoEdge  )(ViewportSelect *vs, LONG x, LONG y);
//	void            (*VSShowHotspot				)(ViewportSelect *p, EditorWindow *bw, LONG x, LONG y);
//	void            (*VSSetBrushRadius		)(ViewportSelect *p, LONG r);
//	void          	(*VSClearPixelInfo    )(ViewportSelect *vs, LONG x, LONG y, UCHAR mask);
//	Bool						(*VSGetCameraCoordinates)(ViewportSelect *vs, Real x, Real y, Real z, Vector &v);
//	Real						(*ZSensitiveNearClipping)(BaseView *bv);
//	Bool						(*VSDrawPolygon				)(ViewportSelect *vs, const Vector* p, LONG ptcnt, LONG i, BaseObject* op, Bool onlyvisible);
//	Bool						(*VSDrawHandle				)(ViewportSelect *vs, const Vector& p, LONG i, BaseObject* op, Bool onlyvisible);
//	LONG						(*GetFrameScreen			)(BaseDraw *bv, LONG *cl, LONG *ct, LONG *cr, LONG *cb);
//	const Matrix4&	(*GetViewMatrix				)(BaseDraw *bv, LONG n);
//	ViewportPixel*	(*VSGetNearestPoint   )(ViewportSelect *vs, BaseObject* op, LONG &x, LONG &y, LONG maxrad, Bool onlyselected, LONG* ignorelist, LONG ignorecnt);
//	ViewportPixel*	(*VSGetNearestPolygon )(ViewportSelect *vs, BaseObject* op, LONG &x, LONG &y, LONG maxrad, Bool onlyselected, LONG* ignorelist, LONG ignorecnt);
//	ViewportPixel*	(*VSGetNearestEdge    )(ViewportSelect *vs, BaseObject* op, LONG &x, LONG &y, LONG maxrad, Bool onlyselected, LONG* ignorelist, LONG ignorecnt);
//	void            (*VSShowHotspotS			)(EditorWindow *bw, LONG x, LONG y, LONG rad, Bool bRemove);
//	Bool						(*VSPickObject				)(BaseDraw* bd, BaseDocument* doc, LONG x, LONG y, LONG rad, Bool allowOpenGL, LassoSelection* ls, C4DObjectList* list, Matrix4* m);
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
//typedef LONG (*GeExecuteProgramExCallback)(LONG cmd, void *userdata, const Filename &logfile);
//
//struct C4D_General
//{
//	void						(*Free								)(void *data);
//	void						(*Print								)(const String &str);
//
//	Bool						(*FExist							)(const Filename *name, Bool isdir);
//	Bool						(*SearchFile					)(const Filename *directory, const Filename *name, Filename *found);
//	Bool						(*FKill								)(const Filename *name, LONG flags);
//	Bool						(*FCopyFile						)(const Filename *source, const Filename *dest, LONG flags);
//	Bool						(*FRename							)(const Filename *source, const Filename *dest);
//	Bool						(*FCreateDir					)(const Filename *name);
//	Bool						(*ExecuteFile					)(const Filename *path);
//	const Filename	(*GetStartupPath			)(void);
//	Bool						(*ExecuteProgram      )(const Filename *program, const Filename *file);
//
//	void						(*ShowMouse						)(LONG v);
//	void						(*GetSysTime					)(LONG *year, LONG *month, LONG *day, LONG *hour, LONG *minute, LONG *second);
//	LONG						(*GetTimer						)(void);
//	void						(*GetLineEnd					)(String *str);
//	LONG						(*GetDefaultFPS				)(void);
//	GEMB_R					(*OutString						)(const String *str, GEMB flags);
//	OPERATINGSYSTEM (*GetCurrentOS			)(void);
//	BYTEORDER			(*GetByteOrder				)(void);
//	void						(*GetGray							)(LONG *r, LONG *g, LONG *b);
//	Bool						(*ChooseColor					)(Vector *col, LONG flags);
//	void						(*SetMousePointer			)(LONG l);
//	Bool						(*ShowBitmap1					)(const Filename *fn);
//	Bool						(*ShowBitmap2					)(BaseBitmap *bm);
//	void						(*StopAllThreads			)(void);
//	void						(*StatusClear					)(void);
//	void						(*StatusSetSpin				)(void);
//	void						(*StatusSetBar				)(LONG p);
//	void						(*StatusSetText				)(const String *str);
//	void						(*SpecialEventAdd			)(LONG type, VULONG p1, VULONG p2);
//	Bool						(*DrawViews						)(DRAWFLAGS flags, BaseDraw *bd); 
//	void						(*GetGlobalTexturePath)(LONG i, Filename *fn);
//	void						(*SetGlobalTexturePath)(LONG i, const Filename *fn);
//	void						(*FlushUnusedTextures	)(void);
//	void						(*GetWorldContainer		)(BaseContainer *bc);
//	void						(*ErrorStringDialog   )(CHECKVALUERANGE type, Real x, Real y, CHECKVALUEFORMAT is);
//
//	void						(*InsertBaseDocument	)(BaseDocument *doc);
//	void						(*SetActiveDocument		)(BaseDocument *doc);
//	BaseDocument*		(*GetActiveDocument		)(void);
//	BaseDocument*		(*GetFirstDocument		)(void);
//	void						(*KillDocument				)(BaseDocument *doc);
//	Bool						(*LoadFile						)(const Filename *name);
//	Bool						(*SaveDocument				)(BaseDocument *doc, const Filename &name, SAVEDOCUMENTFLAGS saveflags, LONG format);
//	RENDERRESULT 		(*RenderDocument			)(BaseDocument *doc, ProgressHook *pr, void *private_data, BaseBitmap *bmp, const BaseContainer *rdata, RENDERFLAGS renderflags, BaseThread *th);
//	Vector					(*GetColor						)(LONG i);
//	Bool						(*RegisterPlugin			)(LONG api_version, PLUGINTYPE type, LONG id, const String *str,void *data,LONG datasize);
//	void						(*GetSerialInfo				)(SERIALINFO type, String *s1, String *s2, String *s3, String *s4, String *s5, String *s6);
//	VERSIONTYPE			(*GetVersionType			)(void);
//	Bool						(*ReadPluginInfo			)(LONG pluginid, CHAR *buffer, LONG size);
//	Bool						(*WritePluginInfo			)(LONG pluginid, CHAR *buffer, LONG size);
//
//	void						(*EwDrawXORLine				)(EditorWindow *win, LONG x1,LONG y1,LONG x2,LONG y2);
//	void						(*EwMouseDragStart		)(EditorWindow *win, LONG button,Real mx,Real my,MOUSEDRAGFLAGS flag);
//	MOUSEDRAGRESULT (*EwMouseDrag					)(EditorWindow *win, Real *mx,Real *my,BaseContainer *channels);
//	MOUSEDRAGRESULT (*EwMouseDragEnd			)(EditorWindow *win);
//	Bool						(*EwBfGetInputState		)(EditorWindow *win, LONG askdevice,LONG askchannel,BaseContainer *res);
//	Bool						(*EwBfGetInputEvent		)(EditorWindow *win, LONG askdevice,BaseContainer *res);
//
//	Bool						(*RegistryAdd         )(LONG sub_id, REGISTRYTYPE main_id, void *data);
//	Bool						(*RegistryRemove      )(LONG sub_id, REGISTRYTYPE main_id);
//	Registry*				(*RegistryFind				)(LONG sub_id, REGISTRYTYPE main_id);
//	Registry*				(*RegistryGetLast			)(REGISTRYTYPE main_id);
//	Registry*				(*RegistryGetFirst		)(REGISTRYTYPE main_id);
//	Bool						(*RegistryGetAutoID		)(LONG *id);
//	Bool						(*RegistryGetData			)(Registry *reg, REGISTRYTYPE *main_id, LONG *sub_id, void **data);
//
//	void*						(*Alloc								)(VLONG size,LONG line,const CHAR *file);
//	BaseContainer*	(*GetWorldPluginData  )(LONG id);
//	Bool						(*SetWorldPluginData  )(LONG id, const BaseContainer *bc, Bool add);
//	Bool            (*SyncMessage         )(LONG message, LONG core_id, VLONG par1, VLONG par2);
//	void						(*SetWorldContainer		)(const BaseContainer *bc);
//	Bool						(*PluginMessage				)(LONG id, void *data);
//
//	BasePlugin*			(*FindPlugin										)(LONG id, PLUGINTYPE type);
//	BasePlugin*			(*GetFirstPlugin								)(void);
//	void*						(BasePlugin::*GetPluginStructure)();
//	Filename				(BasePlugin::*GetFilename				)(void);
//	LONG						(BasePlugin::*GetID							)(void) const;
//	LONG						(BasePlugin::*GetInfo						)(void) const;
//
//	Bool						(*ChooseFont					)(BaseContainer *col);
//
//	void						(*GeDebugBreak				)(LONG line, const CHAR *file);
//	void						(*GeDebugOut					)(const CHAR* s,...);
//	Bool						(*RenameDialog				)(String *str);
//	Bool						(*OpenHTML						)(const String &webaddress);
//	Bool						(*SendModelingCommand )(LONG command, ModelingCommandData &data);
//
//	void						(*EventAdd						)(EVENT flags);
//	void						(*FindInManager				)(BaseList2D *bl);
//
//	CUSTOMDATATYPEPLUGIN*		(*FindCustomDataTypePlugin		)(LONG type);
//	RESOURCEDATATYPEPLUGIN*	(*FindResourceDataTypePlugin	)(LONG type);
//
//	void						(*GeSleep							)(LONG milliseconds);
//	GeData					(*SendCoreMessage			)(LONG coreid, const BaseContainer &msg, LONG eventid);
//	Bool						(*CheckIsRunning			)(CHECKISRUNNING type);
//	BaseContainer*	(*GetWorldContainerInstance)(void);
//
//	Bool						(*GenerateTexturePath )(const Filename &docpath, const Filename &srcname, const Filename &suggestedpath, Filename *dstname);
//	Bool						(*IsInSearchPath			)(const Filename &texfilename, const Filename &docpath);
//
//	BaseContainer*	(*GetToolPluginData   )(BaseDocument *doc, LONG id);
//	Bool						(*IsMainThread				)(void);
//
//
//	Filename				(*GetDefaultFilename  )(LONG id);
//
//	Bool						(*AddBackgroundHandler					)(BackgroundHandler *handler, void *tdata, LONG typeclass, LONG priority);
//	Bool						(*RemoveBackgroundHandler				)(void *tdata, LONG typeclass);
//	void						(*StopBackgroundThreads					)(LONG typeclass, BACKGROUNDHANDLERFLAGS flags);
//	Bool						(*CheckBackgroundThreadsRunning	)(LONG typeclass, Bool all);
//	void						(*ProcessBackgroundThreads			)(LONG typeclass);
//
//	void						(*FlushTexture									)(const Filename *docpath, const String *name, const Filename &suggestedfolder);
//
//	Bool						(*GetMovieInfo									)(const Filename *name, LONG *frames, Real *fps);
//	String          (*GetObjectName                 )(LONG type);
//	String          (*GetTagName                    )(LONG type);
//	LONG            (*GetObjectType                 )(const String &name);
//	LONG            (*GetTagType                    )(const String &name);
//
//	void            (*CopyToClipboard               )(const String &str);
//
//	void*						(*AllocNC												)(VLONG size,LONG line,const CHAR *file);
//	BaseContainer*	(*GetToolData										)(BaseDocument *doc,LONG pluginid);
//	Bool						(*GeGetMemoryStat								)(BaseContainer &stat);
//	Bool						(*PopupEditText									)(LONG screenx,LONG screeny, LONG width, LONG height,const String &changeme,LONG flags,PopupEditTextCallback *func, void *userdata);
//
//	Bool						(*EWScreen2Local								)(EditorWindow *win, LONG *x, LONG *y);
//	Bool						(*EWLocal2Screen								)(EditorWindow *win, LONG *x, LONG *y);
//
//	void						(*StartEditorRender							)(Bool active_only, Bool raybrush, LONG x1, LONG y1, LONG x2, LONG y2, BaseThread *bt, BaseDraw *bd, Bool newthread);
//
//	GeData					(*StringToNumber								)(const String &text, LONG format, LONG fps, const LENGTHUNIT *unit);
//
//	Bool						(*IsActiveToolEnabled						)();
//	SYSTEMINFO			(*GetSystemInfo									)(void);
//	Bool						(*PrivateSystemFunction01				)(void *par1);
//	Bool						(*GetLanguage										)(LONG index, String *extension, String *name, Bool *default_language);
//
//	GeListHead*			(*GetScriptHead									)(LONG type);
//	LONG						(*GetDynamicScriptID						)(BaseList2D *bl);
//	Real						(*GetToolScale									)(BaseDraw* bd, AtomArray* arr, Bool all, LONG mode);
//	Bool						(*GetCommandLineArgs						)(C4DPL_CommandLineArgs &args);
//	Bool						(*FilterPluginList							)(AtomArray &arr, PLUGINTYPE type, Bool sortbyname);
//
//	Bool						(*LoadDocument									)(BaseDocument *doc, const Filename &name, SCENEFILTER loadflags, BaseThread *bt);
//	void						(*FrameScene										)(BaseDocument *doc);
//	IDENTIFYFILE		(*IdentifyFile									)(const Filename &name, UCHAR *probe, LONG probesize, IDENTIFYFILE recognition, BasePlugin **bp);
//	const Filename	(*GetC4DPath										)(LONG whichpath);
//
//	Bool						(*FMove													)(const Filename &source, const Filename &dest);
//
//	Bool						(*HandleViewCommand							)(LONG command_id, BaseDocument *doc, BaseDraw *bd, LONG *value);
//
//	Bool						(*AddUndoHandler								)(BaseDocument *doc, void *dat, UNDOTYPE type);
//
//	String					(*GeGetDegreeChar               )();
//	String					(*GeGetPercentChar              )();
//	Bool						(*HandleCommand									)(LONG command_id, LONG *value);
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
//	void*						(*GeCipher256Open								)(const UCHAR *key, LONG klength, Bool stream);
//	void						(*GeCipher256Close							)(void* h);
//
//	IpConnection*		(*IpWaitForIncoming							)(IpConnection* listener, BaseThread* connection, LONG *ferr);
//	void						(*IpCloseConnection							)(IpConnection* ipc);
//	void						(*IpKillConnection							)(IpConnection *ipc);
//	VLONG						(*IpBytesInInputBuffer					)(IpConnection* ipc);
//	VLONG						(*IpReadBytes										)(IpConnection* ipc, void* buf, VLONG size);
//	VLONG						(*IpSendBytes										)(IpConnection* ipc, void* buf, VLONG size);
//	void						(*IpGetHostAddr									)(IpConnection* ipc, CHAR *buf, LONG bufsize);
//	void						(*IpGetRemoteAddr								)(IpConnection* ipc, CHAR *buf, LONG bufsize);
//
//	Bool						(*RecordCommand									)(LONG command_id, LONG flags, const String &str);
//
//	Bool						(*SendMailAvailable							)();
//	Bool						(*SendMail											)(const String &t_subject, const String *t_to, const String *t_cc, const String *t_bcc, Filename *t_attachments,const String &t_body, LONG flags);
//	Bool						(*GetSystemEnvironmentVariable  )(const String &varname, String &result);
//	Bool						(*CallHelpBrowser								)(const String &optype, const String &main, const String &group, const String &property);
//	String					(*FormatNumber                  )(const GeData &val, LONG format, LONG fps, Bool bUnit, LENGTHUNIT *unit);
//
//	void			      (*BuildGlobalTagPluginContainer	 )(BaseContainer *plugincontainer,LONG *id);
//	LONG			      (*ResolveGlobalTagPluginContainer)(LONG *id);
//	Vector					(*GetGuiWorldColor							)(LONG cid);
//
//	LONG						(*GetShortcutCount							)();
//	BaseContainer		(*GetShortcut										)(LONG index);
//	Bool						(*AddShortcut										)(const BaseContainer &bc);
//	Bool						(*RemoveShortcut								)(LONG index);
//	Bool						(*LoadShortcutSet								)(const Filename &fn, Bool add);
//	Bool						(*SaveShortcutSet								)(const Filename &fn);
//	LONG						(*FindShortcutsFromID						)(LONG pluginid, LONG *indexarray, LONG maxarrayelements);
//	LONG						(*FindShortcuts									)(const BaseContainer &scut, LONG *pluginidarray, LONG maxarrayelements);
//	void						(*SetViewColor									)(LONG colid, const Vector &col);
//
//	void						(*RemovePlugin									)(BasePlugin *plug);
//
//	Bool						(*GetAllocSize									)( void *p, VLONG *out_size );
//	void            (*InsertCreateObject            )(BaseDocument *doc, BaseObject *op, BaseObject *activeobj);
//
//	void						(*GeCipher256Encrypt						)(void *h, UCHAR *mem, LONG size);
//	void						(*GeCipher256Decrypt						)(void *h, UCHAR *mem, LONG size);
//
//	IpConnection*		(*IpOpenListener								)(ULONG ipAddr, LONG port, BaseThread* thread, LONG timeout, Bool dontwait, LONG* ferr);
//	IpConnection*		(*IpOpenOutgoing								)(CHAR* hostname, BaseThread* thread, LONG initial_timeout, LONG timeout, Bool dontwait, LONG* ferr);
//	String					(*DateToString									)(const LocalFileTime &t, Bool date_only);
//	Bool						(*ShowInFinder									)(const Filename &fn, Bool open);
//
//	Bool						(*WriteLayout										)(const Filename &fn);
//	Bool						(*WritePreferences							)(const Filename &fn);
//	Bool						(*SaveProjectCopy								)(BaseDocument *t_doc, const Filename &directory, Bool allow_gui);
//
//	LONG						(*ShowPopupMenu									)(CDialog *parent,LONG screenx,LONG screeny,const BaseContainer *bc,LONG flags, LONG *res_mainid);
//
//	Bool						(*AskForAdministratorPrivileges	)(const String &msg, const String &caption, Bool bAllowSuperUser, void **token);
//	void						(*EndAdministratorPrivileges		)();
//	void						(*RestartApplication						)(const UWORD* param, LONG exitcode, const UWORD** path);
//	const Filename	(*GetStartupApplication					)(void);
//
//	Bool						(*EWGlobal2Local								)(EditorWindow *win, LONG *x, LONG *y);
//	Bool						(*EWLocal2Global								)(EditorWindow *win, LONG *x, LONG *y);
//
//	Bool						(*RequestFileFromServer					)(const Filename &fn, Filename &res);
//	Bool						(*ReadPluginReg									)(LONG pluginid, CHAR *buffer, LONG size);
//	Bool						(*WritePluginReg								)(LONG pluginid, CHAR *buffer, LONG size);
//	Bool						(*GeFGetDiskFreeSpace						)(const Filename &vol, LULONG &freecaller, LULONG &total, LULONG &freespace);
//	ULONG						(*GeFGetAttributes							)(const Filename *name);
//	Bool						(*GeFSetAttributes							)(const Filename *name, ULONG flags, ULONG mask);
//
//	void						(*BrowserLibraryPopup						)(LONG mx, LONG my, LONG defw, LONG defh, LONG pluginwindowid, LONG presettypeid, void *userdata, BrowserPopupCallback callback);
//	Bool						(*GeExecuteProgramEx						)(const Filename &program, const String *args, LONG argcnt, GeExecuteProgramExCallback callback, void *userdata);
//	LReal						(*GeGetMilliSeconds							)(void);
//
//	void*						(*ReallocNC											)(void *old_data, VLONG new_size, LONG line, const CHAR *file);
//	Bool						(*GeGetAllocatorStatistics			)(BaseContainer &stat, void *in_allocator);
//	VULONG					(*GeMemGetFreePhysicalMemoryEstimate)(void);
//	void            (*CopyToClipboardB              )(BaseBitmap *map, LONG ownerid);
//	Bool            (*GetStringFromClipboard        )(String *txt);
//	Bool            (*GetBitmapFromClipboard        )(BaseBitmap *map);
//	CLIPBOARDTYPE   (*GetClipboardType              )(void);
//
//	void						(*EndGlobalRenderThread					)(Bool external_only);
//	LONG						(*GeDebugSetFloatingPointChecks	)(LONG on);
//	LONG						(*GetC4DClipboardOwner					)(void);
//	void						(*GeCheckMem										)(void *memptr);
//
//	Bool						(*GetFileTime										)(const Filename &name, LONG mode, LocalFileTime *out);
//	Bool						(*SetFileTime										)(const Filename &name, LONG mode, const LocalFileTime *in);
//	void						(*GetCurrentTime								)(LocalFileTime *out);
//
//	void						(*GeUpdateUI										)();
//	Real						(*CalculateTranslationScale			)(const UnitScaleData *src_unit, const UnitScaleData *dst_unit);
//	LONG						(*CheckPythonColor							)(const String &txt);
//
//	void						(*PrintNoCR											)(const String &str);
//};
//
//struct C4D_Link
//{
//	BaseLink*				(*Alloc								)(void);
//	void						(*Free								)(BaseLink *link);
//	BaseList2D*			(*GetLink							)(const BaseLink *link, const BaseDocument *doc, LONG instanceof);
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
//	C4DAtomGoal*		(*GetLinkAtom					)(const BaseLink *link, const BaseDocument *doc, LONG instanceof);
//	C4DAtomGoal*		(*ForceGetLinkAtom		)(const BaseLink *link);
//};
//
//struct C4D_Neighbor
//{
//	EnumerateEdges*	(*Alloc								)(LONG pcnt, const CPolygon *vadr, LONG vcnt, BaseSelect *bs);
//	void						(*Free								)(EnumerateEdges *nb);
//	void						(*GetEdgePolys				)(EnumerateEdges *nb, LONG a, LONG b,LONG *first,LONG *second);
//	void						(*GetPointPolys				)(EnumerateEdges *nb, LONG pnt, LONG **dadr, LONG *dcnt);
//	LONG						(*GetEdgeCount				)(EnumerateEdges *nb);
//	PolyInfo*				(*GetPolyInfo					)(EnumerateEdges *nb, LONG poly);
//	Bool    				(*GetNGons  					)(EnumerateEdges *nb, PolygonObject* op, LONG &ngoncnt, NgonNeighbor *&ngons);
//	void						(*ResetAddress				)(EnumerateEdges *nb, const CPolygon *a_polyadr);
//};
//
//struct C4D_Painter
//{
//	void*						(*SendPainterCommand  )(LONG command, BaseDocument *doc, PaintTexture *tex, const BaseContainer *bc);
//	Bool						(*CallUVCommand				)(const Vector *padr, LONG PointCount, const CPolygon *polys, LONG lPolyCount, UVWStruct *uvw, BaseSelect *polyselection,
//																						BaseSelect* pointselection, BaseObject*op, LONG mode, LONG cmdid, const BaseContainer &settings);
//
//	TempUVHandle*		(*GetActiveUVSet			)(BaseDocument* doc, LONG flags);
//	void						(*FreeActiveUVSet			)(TempUVHandle *handle);
//
//	LONG						(*UVSetGetMode				)(TempUVHandle *handle);
//	const Vector*		(*UVSetGetPoint				)(TempUVHandle *handle);
//	LONG						(*UVSetGetPointCount	)(TempUVHandle *handle);
//	const CPolygon*	(*UVSetGetPoly				)(TempUVHandle *handle);
//	LONG						(*UVSetGetPolyCount		)(TempUVHandle *handle);
//	UVWStruct*			(*UVSetGetUVW					)(TempUVHandle *handle);
//	BaseSelect*			(*UVSetGetPolySel			)(TempUVHandle *handle);
//	BaseSelect*			(*UVSetGetPointSel		)(TempUVHandle *handle);
//	BaseObject*			(*UVSetGetBaseObject	)(TempUVHandle *handle);
//
//	Bool						(*UVSetSetUVW					)(TempUVHandle *handle, UVWStruct *uv);
//
//	Bool						(*Private1						)(LONG lCommand, AtomArray* pArray, BaseSelect **polyselection,BaseContainer& setttings, BaseThread* th);
//
//	PaintTexture*		(*CreateNewTexture		)(const Filename &path, const BaseContainer &settings);
//	Bool						(*GetTextureDefaults	)(LONG channel,BaseContainer &settings);
//
//	Bool						(*UVSetIsEditable			)(TempUVHandle *handle);
//
//	LONG						(*IdentifyImage				)(const Filename &texpath);
//	Bool						(*BPSetupWizardWithParameters)(BaseDocument *doc, const BaseContainer &settings, AtomArray &objects, AtomArray &material);
//
//	Bool						(*CalculateTextureSize)(BaseDocument *doc, AtomArray &materials, TextureSize *&sizes);
//
//	LONG						(*PB_GetBw)(PaintBitmap *bmp);
//	LONG						(*PB_GetBh)(PaintBitmap *bmp);
//	PaintLayer*			(*PB_GetLayerDownFirst)(PaintBitmap *tex);
//	PaintLayer*			(*PB_GetLayerDownLast)(PaintBitmap *tex);
//	PaintLayerBmp*	(*PT_AddLayerBmp)(PaintTexture *tex,PaintLayer *insertafter,PaintLayer *layerset,COLORMODE mode,Bool useundo, Bool activate);
//	GeListHead*			(*PT_GetPaintTextureHead)();
//	Bool						(*PLB_ImportFromBaseBitmap)(PaintLayerBmp *layer,BaseBitmap *bmp, Bool usealpha);
//	Bool						(*PLB_ImportFromBaseBitmapAlpha)(PaintLayerBmp *layer,BaseBitmap *bmp,BaseBitmap *channel);
//	Bool						(*PLB_GetPixelCnt)(PaintLayerBmp *layer,LONG x,LONG y,LONG num,PIX *dst,COLORMODE dstmode,PIXELCNT flags);
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
//	PaintLayerBmp*	(*PB_AddAlphaChannel)(PaintBitmap *bmp,LONG bitdepth,PaintLayer *prev,Bool undo, Bool activate);
//	Bool						(*PB_AskApplyAlphaMask)(PaintBitmap *bmp);
//	void						(*PB_ApplyAlphaMask)(PaintBitmap *bmp,LONG x,LONG y,LONG num,PIX *bits,LONG mode,Bool inverted, LONG flags);
//	PaintLayerMask*	(*PB_FindSelectionMask)(PaintBitmap *bmp,PaintBitmap **toplevel,LONG *bitdepth);
//	LONG						(*PB_GetColorMode)(PaintBitmap *bmp);
//	ULONG						(*PB_GetDirty)(PaintBitmap *bmp,DIRTYFLAGS flags);
//	void						(*PB_UpdateRefresh)(PaintBitmap *bmp,LONG xmin,LONG ymin,LONG xmax,LONG ymax,ULONG flags);
//	void						(*PB_UpdateRefreshAll)(PaintBitmap *bmp,ULONG flags,Bool reallyall);
//	Bool						(*PB_ReCalc)(PaintBitmap *bmpthis,BaseThread *thread,LONG x1,LONG y1,LONG x2,LONG y2,BaseBitmap *bmp,LONG flags,ULONG showbit);
//	Bool						(*PB_ConvertBits)(LONG num,const PIX *src,LONG srcinc,COLORMODE srcmode,PIX *dst,LONG dstinc,COLORMODE dstmode,LONG dithery,LONG ditherx);
//	Bool						(*PLB_SetPixelCnt)(PaintLayerBmp *layer,LONG x,LONG y,LONG num,const PIX *src,LONG incsrc,COLORMODE srcmode,PIXELCNT flags);
//	void						(*PLB_GetBoundingBox)(PaintLayerBmp *layer,LONG &x1,LONG &y1,LONG &x2,LONG &y2, Bool hasselectionpixels);
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
//	void						(*LSL_SetPreviewMode)(LayerSet *l, LONG mode);
//	LONG						(*LSL_GetPreviewMode)(const LayerSet *l);
//	LayerSetHelper *(*LSH_Alloc)();
//	void						(*LSH_Free)(LayerSetHelper *lsh);
//	Bool						(*LSH_Init)(LayerSetHelper *lsh, const Filename &fn, const LayerSet *selection);
//	Bool						(*LSH_EditLayerSet)(LayerSetHelper *lsh,const String &dialogtitle, LayerSet *layerset, LayerSet *layerset_a);
//
//	Bool						(*CLL_CalculateResolution)(BaseDocument *doc, const Filename &filename, LONG *xres, LONG *yres);
//	Bool						(*CLL_CalculateFilename)(BaseDocument *doc, Filename &fn, LayerSet *lsl);
//
//	Bool						(*PL_GetShowBit)(PaintLayer *bmp,Bool hierarchy, ULONG bit);
//	Bool						(*PL_SetShowBit)(PaintLayer *bmp,Bool onoff, ULONG bit);
//	PaintTexture*		(*PT_CreateNewTextureDialog)(String &result,Filename &resultdirectory,LONG channelid,BaseMaterial *bmat);
//	void						(*PN_ActivateChannel)(LONG channel, Bool multi, Bool enable);
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
//	void						(GlString::*SDKInit4)(LONG n);
//	void						(GlString::*SDKInit5)(Real r, const char* pszFormat);
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
//	LONG						(GlString::*Compare)(const GlString &s) const;
//	LONG						(GlString::*Replace)(const GlString& strSearch, const GlString &strReplace, VLONG lStart, Bool bReplaceAll, Bool bOnlyWord);
//
//	// GlProgramFactory
//	GlProgramFactory* (*GetFactory)(const BaseDraw* pBaseDraw, const C4DAtom* pObj, LULONG ulIndex, GlProgramFactoryMessageCallback fnCallback, void* pIdentity, VLONG lIdentityLength,
//		const GlLight* const* ppLights, LONG lLightCount, LULONG ulFlags,
//		const GlVertexBufferAttributeInfo* const* ppBufferAttributeInfo, LONG lBufferAttributeInfoCount,
//		const GlVertexBufferVectorInfo* const* ppBufferVectorInfo, LONG lBufferVectorInfoCount,
//		GlFBAdditionalTextureInfo* pAdditionalTextures);
//	void						(*RemoveReference)(const C4DAtom* pObj, LULONG lIndex);
//	void						(*RemoveTextureReferenceA)(const C4DAtom* pObj, LONG lIndex);
//	void						(*RemoveTextureReferenceB)(const Filename &fn);
//	void*						(*IncreaseBufferSize)(GlGetIdentity* pIdentity, VLONG lNeededSize, LONG lLine, const char* pszFile);
//	ULONG						(GlProgramFactory::*Init)(LONG lSubdivideCount);
//
//	Bool						(GlProgramFactory::*BindToView)(const BaseDraw* pDraw);
//	Bool						(GlProgramFactory::*CompilePrograms)();
//	Bool						(GlProgramFactory::*BindPrograms)();
//	Bool						(GlProgramFactory::*UnbindPrograms)();
//	Bool						(GlProgramFactory::*DestroyPrograms)(Bool bChangeContext);
//	void						(GlProgramFactory::*LockFactory)();
//	void						(GlProgramFactory::*UnlockFactory)();
//	void*						(GlProgramFactory::*GetPrivateData)(const void* pObj, LONG lDataIndex, GlProgramFactoryAllocPrivate fnAlloc, GlProgramFactoryFreePrivate fnFree);
//	void*						(GlProgramFactory::*GetDescriptionData)(LONG lObjIndex, LONG lDataIndex, GlProgramFactoryAllocDescription fnAlloc, GlProgramFactoryFreeDescription fnFree);
//	Bool						(GlProgramFactory::*IsProgram)(GlProgramType t);
//
//	void						(GlProgramFactory::*InitSetParameters)();
//	void						(GlProgramFactory::*SetScreenCoordinates)(BaseDraw* pBaseDraw);
//	void						(GlProgramFactory::*AddErrorHandler)(GlProgramFactoryErrorHandler fn);
//	Bool						(GlProgramFactory::*SetParameterMatrixTransform)(GlProgramParameter param);
//	Bool						(GlProgramFactory::*SetParameterMatrix1)(GlProgramParameter param, const SMatrix4 &m);
//	Bool						(GlProgramFactory::*SetParameterMatrix2)(GlProgramParameter paramOffset, GlProgramParameter paramAxes, GlProgramParameter paramNormalTrans, const SMatrix &m);
//	Bool						(GlProgramFactory::*SetParameterMatrix3x3)(GlProgramParameter param, const SReal* r);
//	Bool						(GlProgramFactory::*SetParameterMatrix4x4)(GlProgramParameter param, const SReal* r);
//	Bool						(GlProgramFactory::*SetParameterVector1)(GlProgramParameter param, const SVector &v);
//	Bool						(GlProgramFactory::*SetParameterVector2)(GlProgramParameter param, const SVector4 &v);
//	Bool						(GlProgramFactory::*SetParameterVector3)(GlProgramParameter param, const SVector &v, SReal w);
//	Bool						(GlProgramFactory::*SetParameterColor1)(GlProgramParameter param, const Vector &v, Real rBrightness);
//	Bool						(GlProgramFactory::*SetParameterColor2)(GlProgramParameter param, const Vector &v, Real rBrightness, SReal rAlpha);
//	Bool						(GlProgramFactory::*SetParameterColorReverse1)(GlProgramParameter param, const Vector &v, Real rBrightness);
//	Bool						(GlProgramFactory::*SetParameterColorReverse2)(GlProgramParameter param, const Vector &v, Real rBrightness, SReal rAlpha);
//	Bool						(GlProgramFactory::*SetParameterReal)(GlProgramParameter param, SReal r);
//	Bool						(GlProgramFactory::*SetParameterReal2a)(GlProgramParameter param, SReal a, SReal b);
//	Bool						(GlProgramFactory::*SetParameterReal2b)(GlProgramParameter param, const SReal* v);
//	Bool						(GlProgramFactory::*SetParameterReal3a)(GlProgramParameter param, SReal a, SReal b, SReal c);
//	Bool						(GlProgramFactory::*SetParameterReal3b)(GlProgramParameter param, const SReal* v);
//	Bool						(GlProgramFactory::*SetParameterReal4a)(GlProgramParameter param, SReal a, SReal b, SReal c, SReal d);
//	Bool						(GlProgramFactory::*SetParameterReal4b)(GlProgramParameter param, const SReal* v);
//	Bool						(GlProgramFactory::*SetParameterTexture)(GlProgramParameter param, LONG lDimension, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTextureCube)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTexture2D1)(GlProgramParameter param, const BaseBitmap* pBmp, LONG lFlags, DRAW_ALPHA alphamode, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTexture2D2)(GlProgramParameter param, const Filename &fn, LONG lFrame, C4DAtom* pObj, LONG lIndex, LONG lFlags, DRAW_ALPHA alphamode, LONG lMaxSize, LayerSet* pLayerSet, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTexture2D3)(GlProgramParameter param, const BaseBitmap* pBmp, ULONG ulDirty, C4DAtom* pObj, LONG lIndex, LONG lFlags, DRAW_ALPHA alphamode, LONG lMaxSize, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureGradient)(GlProgramParameter param, Gradient* pGradient, C4DAtom* pObj, LONG lIndex, LONG lFlags, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureFunction)(GlProgramParameter param, C4DAtom* pObj, LONG lIndex, LONG lFlags, LONG lDataType, GlProgramFactoryCreateTextureFunctionCallback fn, void* pData, VLONG lDataLen, LONG lInParams, LONG lOutParams, LONG lCycle, Bool bInterpolate, LONG lSizeX, LONG lSizeY, LONG lSizeZ, Bool bParallel, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureRaw)(GlProgramParameter param, C4DAtom* pObj, LONG lIndex, LONG lFlags, LONG lDataType, const void* pData, VLONG lDataLen, ULONG ulDirty, LONG lDimension, LONG lComponents, LONG lCycle, Bool bInterpolate, LONG lSizeX, LONG lSizeY, LONG lSizeZ, C4DGLuint* pnHandle);
//	Bool						(GlProgramFactory::*SetParameterTextureCubeMap)(GlProgramParameter param, C4DAtom* pObj, LONG lIndex, LONG lFlags, LONG lDataType, GlProgramFactoryCreateTextureFunctionCallback fn, void* pData, VLONG lDataLen,	LONG lOutParams, LONG lCycle, Bool bInterpolate, LONG lSize, Bool bParallel, C4DGLuint* pnHandle);
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
//	Bool						(GlProgramFactory::*SetParameterRealArray)(GlProgramParameter param, LONG lElements, const SReal *r);
//	Bool						(GlProgramFactory::*SetParameterTextureNormalizeCube)(GlProgramParameter param);
//	Bool						(GlProgramFactory::*SetParameterTexture2DDepth)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetParameterTextureCubeDepth)(GlProgramParameter param, C4DGLuint nTexture);
//	Bool						(GlProgramFactory::*SetLightParameters)(const GlLight* const* pLights, LONG lLightCount, const SMatrix4& mObject);
//
//	Bool						(GlProgramFactory::*GetCgFXDescription)(BaseContainer* pbcDescription);
//	Bool						(GlProgramFactory::*CompileCgFX)(const Filename& fn, ULONG ulFlags);
//	Bool						(GlProgramFactory::*BindCgFXPrograms)(LONG lTechnique, LONG lPass);
//
//	void						(GlProgramFactory::*AddParameters)(ULONG ulParameters, ULONG ulFormat);
//	LULONG					(GlProgramFactory::*GetParameters)() const;
//	ULONG						(GlProgramFactory::*GetParameterFormats)() const;
//	GlString				(GlProgramFactory::*AddUniformParameter1)(GlProgramType t, GlUniformParamType type, const char* pszName);
//	GlString				(GlProgramFactory::*AddUniformParameter2)(GlProgramType t, GlUniformParamType type, LONG lCount, const char* pszName);
//	Bool						(GlProgramFactory::*HeaderFinished)();
//	Bool						(GlProgramFactory::*AddLightProjection)();
//	void						(GlProgramFactory::*AddLine)(GlProgramType t, const GlString &strLine);
//	void						(GlProgramFactory::*StartLightLoop)();
//	Bool						(GlProgramFactory::*EndLightLoop)();
//	GlString				(GlProgramFactory::*GetUniqueID)();
//	Bool						(GlProgramFactory::*InitLightParameters)();
//	LONG						(GlProgramFactory::*GetMaxLights)();
//	const UCHAR*		(GlProgramFactory::*GetIdentity)() const;
//	GlProgramParameter		(GlProgramFactory::*GetParameterHandle)(GlProgramType t, const char* pszName) const;
//	GlString				(GlProgramFactory::*AddColorBlendFunction)(GlProgramType t, LONG lBlendMode);
//	GlString				(GlProgramFactory::*AddRGBToHSVFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddHSVToRGBFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddRGBToHSLFunction)(GlProgramType t);
//	GlString				(GlProgramFactory::*AddHSLToRGBFunction)(GlProgramType t);
//	Bool						(GlProgramFactory::*AddFunction)(GlProgramType t, const GlString &strFunction);
//	const GlString&	(GlProgramFactory::*AddNoiseFunction)(GlProgramType t, LONG lNoise, LONG lFlags);
//	Bool						(GlProgramFactory::*AddEncryptedBlock)(GlProgramType t, const char* pchData, VLONG lDataLength, const UCHAR* pchKey, LONG lKeyLength);
//	Bool						(GlProgramFactory::*EncryptBlock)(const GlString &strLine, const UCHAR* pchKey, LONG lKeyLength, char *&pchData, VLONG &lDataLength, Bool bCStyle);
//	void						(GlProgramFactory::*GetVectorInfo)(LONG &lVectorCount, const GlVertexBufferVectorInfo* const* &ppVectorInfo) const;
//
//	Bool						(*CacheTextureFn)(BaseDraw* pBaseDraw, const Filename &fn, LONG lFrame, C4DAtom* pObj, LONG lIndex, LONG lFlags, DRAW_ALPHA alphamode, LONG lMaxSize, GlProgramType progType, LayerSet* pLayerSet, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	Bool						(*CacheTextureBmp)(BaseDraw* pBaseDraw, const BaseBitmap* pBmp, ULONG ulDirty, C4DAtom* pObj, LONG lIndex, LONG lFlags, DRAW_ALPHA alphamode, LONG lMaxSize, GlProgramType progType, GlTextureInfo* pInfo, C4DGLuint* pnHandle);
//	ULONG						(*GetLanguageFeatures)(LONG lCompiler, LONG lFlags);
//	Bool						(*HasNoiseSupport)(GlProgramType t, LONG lNoise, BaseDraw* pBaseDraw, LONG lCompiler);
//	LONG						(GlProgramFactory::*GetCompiler)();
//
//	// GlFrameBuffer
//	GlFrameBuffer*	(*GetFrameBuffer)(BaseDraw* pBaseDraw, VULONG lID1, LONG lID2, UINT nWidth, UINT nHeight, LONG lColorTextureCount, LONG lDepthTextureCount, LONG lColorCubeCount, LONG lDepthCubeCount, ULONG ulFlags, LONG lAAMode, GlFBAdditionalTextureInfo* pAdditionalTextures);
//	void						(*RemoveObjectF)(BaseDraw* pBaseDraw, VULONG lID1, LONG lID2);
//	GlFrameBuffer*	(*FindFrameBuffer)(BaseDraw* pBaseDraw, VULONG lID1, LONG lID2);
//
//	void						(GlFrameBuffer::*PrepareForRendering)(LONG lTexture);
//	void						(GlFrameBuffer::*SetInterpolation)(LONG lInterpolate, LONG lTexture);
//	Bool						(GlFrameBuffer::*Activate)(BaseDraw* pBaseDraw);
//	void						(GlFrameBuffer::*Deactivate)(BaseDraw* pBaseDraw);
//	Bool						(GlFrameBuffer::*SetRenderTarget)(LONG lTexture, LONG lFlags);
//	void						(GlFrameBuffer::*GetRatios)(LONG lFlags, Real& rWidth, Real& rHeight);
//	void						(GlFrameBuffer::*GetSize)(LONG lFlags, UINT &nWidth, UINT &nHeight, Bool bFramesize);
//	C4DGLuint				(GlFrameBuffer::*GetTexture)(LONG lTexture, LONG lFlags);
//	LONG						(GlFrameBuffer::*SaveTextureToDisk)(const Filename &fn, LONG lTexture, LONG lFlags);
//	Bool						(GlFrameBuffer::*CopyToBitmap)(BaseBitmap* pBmp, LONG lTexture, LONG lFlags);
//	void						(GlFrameBuffer::*Clear1)();
//	void						(GlFrameBuffer::*Clear2)(const SVector &vColor, SReal rAlpha);
//	Bool						(GlFrameBuffer::*DrawBuffer)(LONG lTexture, LONG lFlags, LONG lColorConversion, C4DGLint nConversionTexture, SReal rAlpha, const SVector &vColor, SReal xs1, SReal ys1, SReal xs2, SReal ys2, SReal xd1, SReal yd1, SReal xd2, SReal yd2);
//	Bool						(GlFrameBuffer::*IsNPOTBuffer)();
//	Bool						(GlFrameBuffer::*CopyToBuffer)(void* pBuffer, VLONG lBufferSize, LONG lTexture, LONG lFlags);
//	Bool						(GlFrameBuffer::*GetTextureData)(BaseDraw* pBaseDraw, LONG x1, LONG y1, LONG x2, LONG y2, void* pData, LONG lTexture, LONG lFlags);
//	GlFBAdditionalTextureInfo* (GlFrameBuffer::*GetAdditionalTexture)(LONG lType, void* pBuffer);
//
//	// GlVertexBuffer
//	GlVertexBuffer* (*GetVertexBuffer)(const BaseDraw* pBaseDraw, const C4DAtom* pObj, LONG lIndex, void* pIdentity, VLONG lIdentityLen, ULONG ulFlags);
//	void						(*RemoveObjectV)(C4DAtom* pObj, LONG lIndex);
//
//	Bool 						(GlVertexBuffer::*IsDirty)();
//	void 						(GlVertexBuffer::*SetDirty)(Bool bDirty);
//	Bool 						(*DrawSubBuffer)(const BaseDraw* pBaseDraw, const GlVertexSubBufferData* pVertexSubBuffer, const GlVertexSubBufferData* pElementSubBuffer,
//																	LONG lDrawinfoCount, const GlVertexBufferDrawSubbuffer* pDrawInfo,
//																	LONG lVectorCount, const GlVertexBufferVectorInfo* const* ppVectorInfo,
//																	GlVertexBufferDrawElementCallback fnCallback, void* pCallbackData);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocSubBuffer)(GlVertexBufferSubBufferType type, VLONG lSize, const void* pData);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocIndexSubBuffer1)(LONG lCount, ULONG* pulIndex, LONG lMaxIndex);
//	GlVertexSubBufferData* (GlVertexBuffer::*AllocIndexSubBuffer2)(LONG lCount, const UWORD* puwIndex);
//	Bool 						(GlVertexBuffer::*FlushAllSubBuffers)();
//	Bool 						(GlVertexBuffer::*FreeBuffers)(GlVertexBufferSubBufferType type);
//	Bool 						(GlVertexBuffer::*FreeBuffer)(GlVertexSubBufferData* pBuffer);
//	Bool						(GlVertexBuffer::*MarkAllForDelete)();
//	Bool						(GlVertexBuffer::*MarkBuffersForDelete)(GlVertexBufferSubBufferType type);
//	Bool						(GlVertexBuffer::*MarkBufferForDelete)(GlVertexSubBufferData* pBuffer);
//	Bool						(GlVertexBuffer::*DeleteMarkedBuffers)();
//	Bool						(GlVertexBuffer::*SetSubBufferData)(GlVertexSubBufferData* pSubBuffer, VLONG lSize, const void* pData);
//	LONG						(GlVertexBuffer::*GetSubbufferCount)() const;
//	void*						(GlVertexBuffer::*MapBuffer)(GlVertexSubBufferData* pSubBuffer, GlVertexBufferAccessFlags flags);
//	Bool						(GlVertexBuffer::*UnmapBuffer)(GlVertexSubBufferData* pSubBuffer);
//};
//
//struct OperatingSystem
//{
//	LONG version;
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
//	LONG InitOS(void *p);
//#endif
//
//extern OperatingSystem C4DOS;

#endif
