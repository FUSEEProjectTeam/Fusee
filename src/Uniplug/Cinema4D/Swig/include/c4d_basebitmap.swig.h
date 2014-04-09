/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __C4DBASEBITMAP_H
#define __C4DBASEBITMAP_H

#ifdef __API_INTERN__
#else
	#include "ge_math.h"
	#include "operatingsystem.h"
	#include "c4d_gedata.h"
	#include "c4d_baseplugin.h"
	#include "c4d_filterdata.h"
	#include "c4d_misc.h"
	#include "c4d_customdatatype.h"
#endif

#ifdef USE_API_MAXON
	#include "sort.h"
#endif

class BaseContainer;
class Filename;
class GeDialog;


// number of color info bits
#define PIX_UCHAR(p) ((PIX_C*)(p))
#define PIX_UWORD(p) ((PIX_W*)(p))
#define PIX_FLOAT(p) ((PIX_F*)(p))

#define B3D_BITDEPTH(mode)	 ((mode >> BITDEPTH_SHIFT) & (BITDEPTH_UCHAR | BITDEPTH_UWORD | BITDEPTH_FLOAT))
#define B3D_COLOR_MODE(mode) (COLORMODE((Int32)mode & ~((BITDEPTH_UCHAR | BITDEPTH_UWORD | BITDEPTH_FLOAT) << BITDEPTH_SHIFT)))

#define B3D_SETDEPTH(depth)	(COLORMODE(depth << BITDEPTH_SHIFT))

#define B3D_IS_CHAR(mode)	 (B3D_BITDEPTH(mode) == BITDEPTH_UCHAR)
#define B3D_IS_UWORD(mode) (B3D_BITDEPTH(mode) == BITDEPTH_UWORD)
#define B3D_IS_FLOAT(mode) (B3D_BITDEPTH(mode) == BITDEPTH_FLOAT)

#define COLORMODE_MAXCOLOR ((1 << 6) - 1)

#define COLORBYTES_GRAY	 1
#define COLORBYTES_AGRAY 2
#define COLORBYTES_RGB	 3
#define COLORBYTES_ARGB	 4
#define COLORBYTES_CMYK	 4
#define COLORBYTES_ACMYK 5

#define COLORBYTES_GRAYw	(COLORBYTES_GRAY * sizeof(PIX_W))		// 16bit GREY
#define COLORBYTES_AGRAYw	(COLORBYTES_AGRAY * sizeof(PIX_W))	// 16bit GREY+ALPHA
#define COLORBYTES_RGBw		(COLORBYTES_RGB * sizeof(PIX_W))		// 16bit RGBs
#define COLORBYTES_ARGBw	(COLORBYTES_ARGB * sizeof(PIX_W))		// 16bit RGBAs
#define COLORBYTES_GRAYf	(COLORBYTES_GRAY * sizeof(PIX_F))		// float GREY
#define COLORBYTES_AGRAYf	(COLORBYTES_AGRAY * sizeof(PIX_F))	// float GREY+ALPHA
#define COLORBYTES_RGBf		(COLORBYTES_RGB * sizeof(PIX_F))		// float RGBs
#define COLORBYTES_ARGBf	(COLORBYTES_ARGB * sizeof(PIX_F))		// float RGBAs

#define COLORBYTES_MAX (COLORBYTES_ARGBf)


#define BASEBITMAP_DATA_GUIPIXELRATIO		100		//Real
#define BASEBITMAP_DATA_NAME						1003	//String
#define BASEBITMAP_DATA_PROGRESS_TIME		1004	//String
#define BASEBITMAP_DATA_PROGRESS_ACTION	1005	//String
#define BASEBITMAP_DATA_PROGRESS_FRAME	1006	//Float	(0...1)  frame progress
#define BASEBITMAP_DATA_PROGRESS_SEQ		1007	//Float	(0...1)  sequence progress
#define BASEBITMAP_DATA_PROGRESS_FNUM		1008	//String (1 of 91) (F 1)
#define BASEBITMAP_DATA_DRAW_REGIONS		1009	//container	- contains region info
#define BASEBITMAP_DATA_SPINMODE				1010	//Bool
#define BASEBITMAP_DATA_HOLDTIME				1011	//Int32
#define BASEBITMAP_DATA_STARTTIME				1012	//Int32
#define BASEBITMAP_DATA_PROGRESS_PHASE	1013	//RENDERPROGRESSTYPE
#define BASEBITMAP_DATA_FRAMETIME				1015	//Int32
#define BASEBITMAP_DATA_TEXTURE_ERROR		1019	//String

enum BITMAP_UPDATEREGION
{
	BITMAP_UPDATEREGION_X1			= 1,
	BITMAP_UPDATEREGION_Y1			= 2,
	BITMAP_UPDATEREGION_X2			= 3,
	BITMAP_UPDATEREGION_Y2			= 4,
	BITMAP_UPDATEREGION_TYPE		= 5,
	BITMAP_UPDATEREGION_COLOR		= 6,
	BITMAP_UPDATEREGION_PREPARE = 7
};

//-----------------
#ifndef __API_INTERN__

	#define CUSTOMDATATYPE_COLORPROFILE	200000266
	#define CUSTOMGUI_COLORPROFILE			200000267

class ColorProfile : public CustomDataType
{
private:
	ColorProfile();
	~ColorProfile();
	ColorProfile(const ColorProfile& src);

public:
	enum COLORPROFILEINFO
	{
		COLORPROFILEINFO_DESCRIPTION	= 0,
		COLORPROFILEINFO_MANUFACTURER = 1,
		COLORPROFILEINFO_MODEL				= 2,
		COLORPROFILEINFO_COPYRIGHT		= 3
	};

public:
	static ColorProfile* Alloc(void);
	static void Free(ColorProfile*& profile);

	ColorProfile& operator = (const ColorProfile& src);
	Bool operator == (const ColorProfile& src) const;
	Bool operator != (const ColorProfile& src) const;

	Bool CreateDefaultWindow(GeDialog* dlg);

	Bool OpenProfileFromFile(const Filename& fn);
	Bool OpenProfileFromMemory(const void* mem, Int64 memsize);

	Bool WriteProfileToMemory(void*& mem, Int64& memsize) const;
	Bool WriteProfileToFile(const Filename& fn) const;

	String GetInfo(COLORPROFILEINFO) const;

	Bool HasProfile() const;
	Bool IsMonitorProfileMode() const;
	Bool SetMonitorProfileMode(Bool on);
	Bool CheckColorMode(COLORMODE colormode) const;

	static const ColorProfile* GetDefaultSRGB();
	static const ColorProfile* GetDefaultLinearRGB();
	static const ColorProfile* GetDefaultSGray();
	static const ColorProfile* GetDefaultLinearGray();
};

//-----------------

class ColorProfileConvert
{
private:
	ColorProfileConvert();
	~ColorProfileConvert();
	ColorProfileConvert(const ColorProfileConvert& src);
	ColorProfileConvert& operator = (const ColorProfileConvert& src);

public:
	static ColorProfileConvert* Alloc(void);
	static void Free(ColorProfileConvert*& convert);

	Bool PrepareTransform(COLORMODE srccolormode, const ColorProfile* srcprofile, COLORMODE dstcolormode, const ColorProfile* dstprofile, Bool bgr);
	void Convert(const PIX* src, PIX* dst, Int32 cnt, Int32 SkipInputComponents, Int32 SkipOutputComponents) const;
};

//-----------------

class BaseBitmap
{
private:
	BaseBitmap();
	~BaseBitmap();

public:
	BaseBitmap* GetClone(void) const;
	BaseBitmap* GetClonePart(Int32 x, Int32 y, Int32 w, Int32 h) const;
	Bool CopyTo(BaseBitmap* dst) const;

	void FlushAll(void);

	Int32 GetBw (void) const { return C4DOS.Bm->GetBw(this); }
	Int32 GetBh (void) const { return C4DOS.Bm->GetBh(this); }
	Int32 GetBt (void) const { return C4DOS.Bm->GetBt(this); }
	Int32 GetBpz(void) const { return C4DOS.Bm->GetBpz(this); }
	COLORMODE GetColorMode(void) const;

	static IMAGERESULT Init(BaseBitmap*& res, const Filename& name, Int32 frame = -1, Bool* ismovie = nullptr, BitmapLoaderPlugin** loaderplugin = nullptr);
	IMAGERESULT Init(const Filename& name, Int32 frame = -1, Bool* ismovie = nullptr);
	IMAGERESULT Init(Int32 x, Int32 y, Int32 depth = 24, INITBITMAPFLAGS flags = INITBITMAPFLAGS_0);

	IMAGERESULT Save(const Filename& name, Int32 format, BaseContainer* data, SAVEBIT savebits) const;

	void SetCMAP(Int32 i, Int32 r, Int32 g, Int32 b);

	Bool SetPixelCnt(Int32 x, Int32 y, Int32 cnt, UChar* buffer, Int32 inc, COLORMODE srcmode, PIXELCNT flags) { return C4DOS.Bm->SetPixelCnt(this, x, y, cnt, buffer, inc, srcmode, flags); }
	void GetPixelCnt(Int32 x, Int32 y, Int32 cnt, UChar* buffer, Int32 inc, COLORMODE dstmode, PIXELCNT flags, ColorProfileConvert* conversion = nullptr) const { C4DOS.Bm->GetPixelCnt(this, x, y, cnt, buffer, inc, dstmode, flags, conversion); }

	void ScaleIt(BaseBitmap* dst, Int32 intens, Bool sample, Bool nprop) const;
	void ScaleBicubic(BaseBitmap* dst, Int32 src_xmin, Int32 src_ymin, Int32 src_xmax, Int32 src_ymax, Int32 dst_xmin, Int32 dst_ymin, Int32 dst_xmax, Int32 dst_ymax) const;

	void SetPen(Int32 r, Int32 g, Int32 b) { C4DOS.Bm->SetPen(this, r, g, b); }
	void Clear(Int32 r, Int32 g, Int32 b);
	void Clear(Int32 x1, Int32 y1, Int32 x2, Int32 y2, Int32 r, Int32 g, Int32 b);
	void Line(Int32 x1, Int32 y1, Int32 x2, Int32 y2) { C4DOS.Bm->Line(this, x1, y1, x2, y2); }
	void Arc(Int32 x, Int32 y, Float radius, Float angle_start, Float angle_end, Int32 subdiv = 32)	 { C4DOS.Bm->Arc(this, x, y, radius, angle_start, angle_end, subdiv); }
	Bool SetPixel(Int32 x, Int32 y, Int32 r, Int32 g, Int32 b) { return C4DOS.Bm->SetPixel(this, x, y, r, g, b); }
	void GetPixel(Int32 x, Int32 y, UInt16* r, UInt16* g, UInt16* b) const { C4DOS.Bm->GetPixel(this, x, y, r, g, b); }
	BaseBitmap* AddChannel(Bool internal, Bool straight);
	void RemoveChannel(BaseBitmap* channel);
	void GetAlphaPixel(BaseBitmap* channel, Int32 x, Int32 y, UInt16* val) const { C4DOS.Bm->GetAlphaPixel(this, channel, x, y, val); }
	Bool SetAlphaPixel(BaseBitmap* channel, Int32 x, Int32 y, Int32 val) { return C4DOS.Bm->SetAlphaPixel(this, channel, x, y, val); }
	const BaseBitmap* GetInternalChannel(void) const;
	BaseBitmap* GetInternalChannel(void);
	Int32	GetChannelCount(void) const;
	const BaseBitmap* GetChannelNum(Int32 num) const;
	BaseBitmap* GetChannelNum(Int32 num);

	Bool IsMultipassBitmap(void) const;
	Bool SetData(Int32 id, const GeData& data);
	GeData GetData(Int32 id, const GeData& t_default) const;

	static BaseBitmap* Alloc(void);
	static void Free(BaseBitmap*& bc);

	UInt32 GetDirty() const { return C4DOS.Bm->GetDirty(this); }
	void SetDirty() { C4DOS.Bm->SetDirty(this); }

	Bool CopyPartTo(BaseBitmap* dst, Int32 x, Int32 y, Int32 w, Int32 h) const;

	Int GetMemoryInfo(void) const { return C4DOS.Bm->GetMemoryInfo(this); }

	BaseBitmap* GetUpdateRegionBitmap() { return C4DOS.Bm->GetUpdateRegionBitmap(this); }
	const BaseBitmap* GetUpdateRegionBitmap() const { return C4DOS.Bm->GetUpdateRegionBitmap(this); }

	Bool SetColorProfile(const ColorProfile* profile) { return C4DOS.Bm->SetColorProfile(this, profile); }
	const ColorProfile* GetColorProfile() const { return C4DOS.Bm->GetColorProfile(this); }
};

class MultipassBitmap : public BaseBitmap
{
private:
	MultipassBitmap();
	~MultipassBitmap();

public:
	Int32 GetLayerCount() const { return C4DOS.Bm->MPB_GetLayerCount(this); }
	Int32 GetAlphaLayerCount() const { return C4DOS.Bm->MPB_GetAlphaLayerCount(this); }
	Int32 GetHiddenLayerCount() const { return C4DOS.Bm->MPB_GetHiddenLayerCount(this); }
	MultipassBitmap* GetSelectedLayer() { return C4DOS.Bm->MPB_GetSelectedLayer(this); }
	MultipassBitmap* GetLayerNum(Int32 num) { return C4DOS.Bm->MPB_GetLayerNum(this, num); }
	MultipassBitmap* GetAlphaLayerNum(Int32 num) { return C4DOS.Bm->MPB_GetAlphaLayerNum(this, num); }
	MultipassBitmap* GetHiddenLayerNum(Int32 num) { return C4DOS.Bm->MPB_GetHiddenLayerNum(this, num); }
	MultipassBitmap* AddLayer(MultipassBitmap* insertafter, COLORMODE colormode, Bool hidden = false) { return C4DOS.Bm->MPB_AddLayer(this, insertafter, colormode, hidden); }
	MultipassBitmap* AddFolder(MultipassBitmap* insertafter, Bool hidden = false) { return C4DOS.Bm->MPB_AddFolder(this, insertafter, hidden); }
	MultipassBitmap* AddAlpha(MultipassBitmap* insertafter, COLORMODE colormode) { return C4DOS.Bm->MPB_AddAlpha(this, insertafter, colormode); }
	Bool DeleteLayer(MultipassBitmap*& layer) { Bool ret = C4DOS.Bm->MPB_DeleteLayer(this, layer); layer = nullptr; return ret; }
	MultipassBitmap* FindUserID(Int32 id, Int32 subid = 0) { return C4DOS.Bm->MPB_FindUserID(this, id, subid); }
	void ClearImageData(void) { C4DOS.Bm->MPB_ClearImageData(this); }
	PaintBitmap* GetPaintBitmap() { return C4DOS.Bm->MPB_GetPaintBitmap(this); }

	void FreeHiddenLayers() { C4DOS.Bm->MPB_FreeHiddenLayers(this); }

	void SetMasterAlpha(BaseBitmap* master) { C4DOS.Bm->MPB_SetMasterAlpha(this, master); }
	GeData GetParameter(MPBTYPE id) const { return C4DOS.Bm->MPB_GetParameter(this, id); }
	Bool SetParameter(MPBTYPE id, const GeData& par) { return C4DOS.Bm->MPB_SetParameter(this, id, par); }

	static MultipassBitmap* Alloc(Int32 bx, Int32 by, COLORMODE mode) { return C4DOS.Bm->MPB_AllocWrapperPB(bx, by, mode); }
	static MultipassBitmap* AllocWrapper(BaseBitmap* bmp) { return C4DOS.Bm->MPB_AllocWrapper(bmp); }
	static void Free(MultipassBitmap*& bc);

	Bool GetLayers(maxon::BaseArray<BaseBitmap*>& list, MPB_GETLAYERS flags = MPB_GETLAYERS_IMAGE | MPB_GETLAYERS_ALPHA);
	Bool GetLayers(maxon::BaseArray<MultipassBitmap*>& list, MPB_GETLAYERS flags = MPB_GETLAYERS_IMAGE | MPB_GETLAYERS_ALPHA);

	Bool SetTempColorProfile(const ColorProfile* profile, Bool dithering);
	Int32	GetUserID(void) const;
	void SetUserID(Int32 id);
	void SetUserSubID(Int32 subid);
	void SetSave(Bool save);
	void SetBlendMode(Int32 mode);
	void SetName(const String& name);
	void SetColorMode(COLORMODE mode);
	void SetComponent(Int32 c);
	void SetDpi(Int32 dpi);
};

// parameter for Insert...
	#define BMP_INSERTLAST ((MultipassBitmap*)-1)

class BaseBitmapLink
{
private:
	BaseBitmapLink ();
	~BaseBitmapLink ();

public:
	BaseBitmap* Get() const;
	void Set(BaseBitmap* bmp);

	static BaseBitmapLink* Alloc(void);
	static void Free(BaseBitmapLink*& bc);
};

class MovieLoader
{
private:
	MovieLoader(void);
	~MovieLoader(void);
	// gDelete_TEMPLATE
	void InitData(void);
	void FreeData(void);

public:
	static MovieLoader* Alloc(void);
	static void Free(MovieLoader*& ml);

	IMAGERESULT Open(const Filename& fn);
	void Close(void);
	BaseBitmap* Read(Int32 new_frame_idx = -1, Int32* _result = nullptr);
	Int32 GetInfo(Float* _fps);

private:
	BitmapLoaderPlugin*			 plugin;
	BitmapLoaderAnimatedData plugin_data;
	BaseBitmap*							 bm;
	Bool										 is_movie;
	Int32										 frame_cnt;
	Float										 fps;
	Int32										 frame_idx;
	IMAGERESULT							 result;
};

class MovieSaver
{
private:
	MovieSaver();
	~MovieSaver();

public:
	IMAGERESULT Open(const Filename& name, BaseBitmap* bm, Int32 fps, Int32 format, BaseContainer* data, SAVEBIT savebits, BaseSound* sound = nullptr);
	IMAGERESULT Write(BaseBitmap* bm);
	void Close(void);
	Bool Choose(Int32 format, BaseContainer* bc);

	static MovieSaver* Alloc(void);
	static void Free(MovieSaver*& bc);
};

class PluginMovieData;

class BitmapLoaderPlugin : public BasePlugin
{
private:
	BitmapLoaderPlugin();
	~BitmapLoaderPlugin();

public:
	Bool BmIdentify(const Filename& name, UChar* probe, Int32 size);
	IMAGERESULT BmLoad(const Filename& name, BaseBitmap* bm, Int32 frame);
	Int32 BmGetSaver(void);
	Bool BmGetInformation(const Filename& name, Int32* frames, Float* fps);
	IMAGERESULT BmLoadAnimated(BitmapLoaderAnimatedData* bd, BITMAPLOADERACTION action, BaseBitmap* bm, Int32 frame);
	IMAGERESULT BmExtractSound(BitmapLoaderAnimatedData* bd, BaseSound* snd);
	IMAGERESULT BmHasSound(BitmapLoaderAnimatedData* bd);
};

class BitmapSaverPlugin : public BasePlugin
{
private:
	BitmapSaverPlugin();
	~BitmapSaverPlugin();

public:
	void BmGetDetails(Int32* alpha, String* suffix);
	Bool BmEdit(BaseContainer* data);
	Int32 BmGetMaxResolution(Bool layers);
	IMAGERESULT BmSave(const Filename& name, BaseBitmap* bm, BaseContainer* data, SAVEBIT savebits);
	IMAGERESULT BmOpen(PluginMovieData*& md, const Filename& name, BaseBitmap* bm, BaseContainer* data, SAVEBIT savebits, Int32 fps);
	IMAGERESULT BmWrite(PluginMovieData* md, BaseBitmap* bm);
	void BmClose(PluginMovieData*& md);
};


class AutoBitmap
{
private:
	BaseBitmap* bmp;

public:
	AutoBitmap(const String& str, Float pixelRatio = 1.0);
	AutoBitmap(Int32 id);
	~AutoBitmap();

	operator BaseBitmap*() const { return bmp; }
};


IconData InitResourceIcon(Int32 resource_id);
BaseBitmap* InitResourceBitmap(Int32 resource_id);

#endif

#endif
