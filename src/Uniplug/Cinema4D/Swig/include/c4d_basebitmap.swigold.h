/////////////////////////////////////////////////////////////
// CINEMA 4D SDK - adopted for Swig wrapping               //
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
	#include "ge_mtools.h"
	#include "c4d_customdatatype.h"
#endif

class BaseContainer;
class Filename;
class GeDialog;


// number of color info bits
#define PIX_UCHAR(p)		((PIX_C*)(p))
#define PIX_UWORD(p)		((PIX_W*)(p))
#define PIX_FLOAT(p)		((PIX_F*)(p))

#define B3D_BITDEPTH(mode)		((mode>>BITDEPTH_SHIFT) & (BITDEPTH_UCHAR|BITDEPTH_UWORD|BITDEPTH_FLOAT))
#define B3D_COLOR_MODE(mode)	(COLORMODE((LONG)mode & ~((BITDEPTH_UCHAR|BITDEPTH_UWORD|BITDEPTH_FLOAT)<<BITDEPTH_SHIFT)))

#define B3D_SETDEPTH(depth)		(COLORMODE(depth<<BITDEPTH_SHIFT))

#define B3D_IS_CHAR(mode)		(B3D_BITDEPTH(mode)==BITDEPTH_UCHAR)
#define B3D_IS_UWORD(mode)	(B3D_BITDEPTH(mode)==BITDEPTH_UWORD)
#define B3D_IS_FLOAT(mode)	(B3D_BITDEPTH(mode)==BITDEPTH_FLOAT)

#define COLORMODE_MAXCOLOR		((1<<6)-1)

#define COLORBYTES_GRAY       1
#define COLORBYTES_AGRAY      2
#define COLORBYTES_RGB        3
#define COLORBYTES_ARGB       4
#define COLORBYTES_CMYK       4
#define COLORBYTES_ACMYK      5

#define COLORBYTES_GRAYw			(COLORBYTES_GRAY	* sizeof(PIX_W))  // 16bit GREY
#define COLORBYTES_AGRAYw			(COLORBYTES_AGRAY * sizeof(PIX_W))  // 16bit GREY+ALPHA
#define COLORBYTES_RGBw				(COLORBYTES_RGB   * sizeof(PIX_W))  // 16bit RGBs
#define COLORBYTES_ARGBw			(COLORBYTES_ARGB  * sizeof(PIX_W))  // 16bit RGBAs
#define COLORBYTES_GRAYf			(COLORBYTES_GRAY	* sizeof(PIX_F))  // float GREY
#define COLORBYTES_AGRAYf			(COLORBYTES_AGRAY * sizeof(PIX_F))  // float GREY+ALPHA
#define COLORBYTES_RGBf				(COLORBYTES_RGB	  * sizeof(PIX_F))  // float RGBs
#define COLORBYTES_ARGBf			(COLORBYTES_ARGB  * sizeof(PIX_F))  // float RGBAs

#define COLORBYTES_MAX        (COLORBYTES_ARGBf)


#define BASEBITMAP_DATA_NAME						1003	//String
#define BASEBITMAP_DATA_PROGRESS_TIME		1004	//String
#define BASEBITMAP_DATA_PROGRESS_ACTION	1005	//String
#define BASEBITMAP_DATA_PROGRESS_FRAME	1006	//Real	(0...1)  frame progress
#define BASEBITMAP_DATA_PROGRESS_SEQ		1007	//Real	(0...1)  sequence progress
#define BASEBITMAP_DATA_PROGRESS_FNUM		1008	//String (1 of 91) (F 1)
#define BASEBITMAP_DATA_DRAW_REGIONS		1009	//container	- contains region info
#define BASEBITMAP_DATA_SPINMODE				1010	//Bool
#define BASEBITMAP_DATA_HOLDTIME				1011	//LONG
#define BASEBITMAP_DATA_STARTTIME				1012  //LONG
#define BASEBITMAP_DATA_PROGRESS_PHASE  1013  //RENDERPROGRESSTYPE
#define BASEBITMAP_DATA_FRAMETIME				1015  //LONG
#define BASEBITMAP_DATA_TEXTURE_ERROR		1019  //String

enum BITMAP_UPDATEREGION
{
	BITMAP_UPDATEREGION_X1 			= 1,
	BITMAP_UPDATEREGION_Y1			= 2,
	BITMAP_UPDATEREGION_X2			= 3,
	BITMAP_UPDATEREGION_Y2			= 4,
	BITMAP_UPDATEREGION_TYPE		= 5
};

//-----------------
#ifndef __API_INTERN__

#define CUSTOMDATATYPE_COLORPROFILE		200000266
#define CUSTOMGUI_COLORPROFILE				200000267

class ColorProfile : public CustomDataType
{
	private:
		ColorProfile();
		~ColorProfile();
		ColorProfile(const ColorProfile &src);
		
	public:
		enum COLORPROFILEINFO
		{
			COLORPROFILEINFO_DESCRIPTION  = 0,
			COLORPROFILEINFO_MANUFACTURER = 1,
			COLORPROFILEINFO_MODEL        = 2,
			COLORPROFILEINFO_COPYRIGHT    = 3
		};

	public:

		static ColorProfile *Alloc(void);
		static void Free(ColorProfile *&profile);

		ColorProfile& operator = (const ColorProfile &src);
		Bool operator == (const ColorProfile &src) const;
		Bool operator != (const ColorProfile &src) const;

		Bool CreateDefaultWindow(GeDialog *dlg);

		Bool OpenProfileFromFile(const Filename &fn);
		Bool OpenProfileFromMemory(const void *mem, LLONG memsize);

		Bool WriteProfileToMemory(void *&mem, LLONG &memsize) const;
		Bool WriteProfileToFile(const Filename &fn) const;

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
		ColorProfileConvert(const ColorProfileConvert &src);
		ColorProfileConvert& operator = (const ColorProfileConvert &src);

	public:
		static ColorProfileConvert *Alloc(void);
		static void Free(ColorProfileConvert *&convert);

		Bool PrepareTransform(COLORMODE srccolormode, const ColorProfile *srcprofile, COLORMODE dstcolormode, const ColorProfile *dstprofile, Bool bgr);
		void Convert(const PIX *src, PIX *dst, LONG cnt, LONG SkipInputComponents, LONG SkipOutputComponents) const;
};

//-----------------

class BaseBitmap
{
	private:
		BaseBitmap();
		~BaseBitmap();
	public:
		BaseBitmap *GetClone(void) const;
		BaseBitmap *GetClonePart(LONG x, LONG y, LONG w, LONG h) const;
		Bool CopyTo(BaseBitmap *dst) const;
	
		void FlushAll(void); 
		
		LONG  GetBw (void) const { return C4DOS.Bm->GetBw(this); }
		LONG  GetBh (void) const { return C4DOS.Bm->GetBh(this); }
		LONG  GetBt (void) const { return C4DOS.Bm->GetBt(this); }
		LONG  GetBpz(void) const { return C4DOS.Bm->GetBpz(this); }
		COLORMODE  GetColorMode(void) const;

		static IMAGERESULT Init(BaseBitmap *&res, const Filename &name, LONG frame=-1, Bool *ismovie=NULL, BitmapLoaderPlugin **loaderplugin=NULL);
		IMAGERESULT  Init(const Filename &name, LONG frame=-1, Bool *ismovie=NULL);
		IMAGERESULT  Init(LONG x, LONG y, LONG depth=24, INITBITMAPFLAGS flags=INITBITMAPFLAGS_0);

	  IMAGERESULT  Save(const Filename &name, LONG format, BaseContainer *data, SAVEBIT savebits) const;

		void	SetCMAP(LONG i, LONG r, LONG g, LONG b);

		Bool	SetPixelCnt(LONG x, LONG y, LONG cnt, UCHAR *buffer, LONG inc, COLORMODE srcmode, PIXELCNT flags) { return C4DOS.Bm->SetPixelCnt(this,x,y,cnt,buffer,inc,srcmode,flags); }
		void  GetPixelCnt(LONG x, LONG y, LONG cnt, UCHAR *buffer, LONG inc, COLORMODE dstmode, PIXELCNT flags, ColorProfileConvert *conversion=NULL) const { C4DOS.Bm->GetPixelCnt(this,x,y,cnt,buffer,inc,dstmode,flags,conversion); }

		void	ScaleIt(BaseBitmap *dst, LONG intens, Bool sample, Bool nprop) const;
		void	ScaleBicubic(BaseBitmap *dst, LONG src_xmin, LONG src_ymin, LONG src_xmax, LONG src_ymax, LONG dst_xmin, LONG dst_ymin, LONG dst_xmax, LONG dst_ymax) const;

		void	SetPen(LONG r, LONG g, LONG b) { C4DOS.Bm->SetPen(this,r,g,b); }
		void	Clear(LONG r, LONG g, LONG b);
		void	Clear(LONG x1, LONG y1, LONG x2, LONG y2, LONG r, LONG g, LONG b);
		void	Line(LONG x1, LONG y1, LONG x2, LONG y2) { C4DOS.Bm->Line(this,x1,y1,x2,y2); }
		Bool	SetPixel(LONG x, LONG y, LONG r, LONG g, LONG b) { return C4DOS.Bm->SetPixel(this,x,y,r,g,b); }
		void	GetPixel(LONG x, LONG y, UWORD *r, UWORD *g, UWORD *b) const { C4DOS.Bm->GetPixel(this,x,y,r,g,b); }
		BaseBitmap *AddChannel(Bool internal, Bool straight);
		void	RemoveChannel(BaseBitmap *channel);
		void	GetAlphaPixel(BaseBitmap *channel, LONG x, LONG y, UWORD *val) const { C4DOS.Bm->GetAlphaPixel(this,channel,x,y,val); }
		Bool	SetAlphaPixel(BaseBitmap *channel, LONG x, LONG y, LONG val) {	return C4DOS.Bm->SetAlphaPixel(this,channel,x,y,val); }
		const BaseBitmap *GetInternalChannel(void) const;
		BaseBitmap *GetInternalChannel(void);
		LONG	GetChannelCount(void) const;
		const BaseBitmap *GetChannelNum(LONG num) const;
		BaseBitmap *GetChannelNum(LONG num);

		Bool IsMultipassBitmap(void) const;
		Bool SetData(LONG id,const GeData &data);
		GeData GetData(LONG id, const GeData &t_default) const;

		static BaseBitmap *Alloc(void);
		static void Free(BaseBitmap *&bc);

		ULONG GetDirty() const { return C4DOS.Bm->GetDirty(this); }
		void SetDirty() { C4DOS.Bm->SetDirty(this); }

		Bool CopyPartTo(BaseBitmap *dst, LONG x, LONG y, LONG w, LONG h) const;

		VLONG GetMemoryInfo(void) const { return C4DOS.Bm->GetMemoryInfo(this); }

		BaseBitmap *GetUpdateRegionBitmap() { return C4DOS.Bm->GetUpdateRegionBitmap(this); }
		const BaseBitmap *GetUpdateRegionBitmap() const { return C4DOS.Bm->GetUpdateRegionBitmap(this); }

		Bool SetColorProfile(const ColorProfile *profile) { return C4DOS.Bm->SetColorProfile(this,profile); }
		const ColorProfile* GetColorProfile() const { return C4DOS.Bm->GetColorProfile(this); }
};

class MultipassBitmap : public BaseBitmap
{
	private:
		MultipassBitmap();
		~MultipassBitmap();

	public:
		LONG GetLayerCount() const { return C4DOS.Bm->MPB_GetLayerCount(this); }
		LONG GetAlphaLayerCount() const { return C4DOS.Bm->MPB_GetAlphaLayerCount(this); }
		LONG GetHiddenLayerCount() const { return C4DOS.Bm->MPB_GetHiddenLayerCount(this); }
		MultipassBitmap *GetSelectedLayer() { return C4DOS.Bm->MPB_GetSelectedLayer(this); }
		MultipassBitmap *GetLayerNum(LONG num) { return C4DOS.Bm->MPB_GetLayerNum(this,num); }
		MultipassBitmap *GetAlphaLayerNum(LONG num) { return C4DOS.Bm->MPB_GetAlphaLayerNum(this,num); }
		MultipassBitmap *GetHiddenLayerNum(LONG num) { return C4DOS.Bm->MPB_GetHiddenLayerNum(this,num); }
		MultipassBitmap *AddLayer(MultipassBitmap *insertafter,COLORMODE colormode,Bool hidden=FALSE) { return C4DOS.Bm->MPB_AddLayer(this,insertafter,colormode,hidden); }
		MultipassBitmap *AddFolder(MultipassBitmap *insertafter,Bool hidden=FALSE) { return C4DOS.Bm->MPB_AddFolder(this,insertafter,hidden); }
		MultipassBitmap *AddAlpha(MultipassBitmap *insertafter,COLORMODE colormode) { return C4DOS.Bm->MPB_AddAlpha(this,insertafter,colormode); }
		Bool DeleteLayer(MultipassBitmap *&layer) { Bool ret=C4DOS.Bm->MPB_DeleteLayer(this,layer); layer=NULL; return ret; }
		MultipassBitmap *FindUserID(LONG id,LONG subid=0) { return C4DOS.Bm->MPB_FindUserID(this,id,subid); }
		void ClearImageData(void) { C4DOS.Bm->MPB_ClearImageData(this); }
		PaintBitmap* GetPaintBitmap() { return C4DOS.Bm->MPB_GetPaintBitmap(this); }

		void FreeHiddenLayers() { C4DOS.Bm->MPB_FreeHiddenLayers(this); }

		void SetMasterAlpha(BaseBitmap *master) { C4DOS.Bm->MPB_SetMasterAlpha(this,master); }
		GeData GetParameter(MPBTYPE id) const { return C4DOS.Bm->MPB_GetParameter(this,id); }
		Bool SetParameter(MPBTYPE id,const GeData &par) { return C4DOS.Bm->MPB_SetParameter(this,id,par); }

		static MultipassBitmap *Alloc(LONG bx, LONG by, COLORMODE mode) { return C4DOS.Bm->MPB_AllocWrapperPB(bx,by,mode); }
		static MultipassBitmap *AllocWrapper(BaseBitmap *bmp) { return C4DOS.Bm->MPB_AllocWrapper(bmp); }
		static void Free(MultipassBitmap *&bc);

		Bool GetLayers(GeTempDynArray<BaseBitmap> &list, MPB_GETLAYERS flags=MPB_GETLAYERS_IMAGE|MPB_GETLAYERS_ALPHA);
		Bool GetLayers(GeTempDynArray<MultipassBitmap> &list, MPB_GETLAYERS flags=MPB_GETLAYERS_IMAGE|MPB_GETLAYERS_ALPHA);

		Bool  SetTempColorProfile(const ColorProfile *profile, Bool dithering);
};

// parameter for Insert...
#define BMP_INSERTLAST ((MultipassBitmap*)-1)

class BaseBitmapLink 
{
	private:
		BaseBitmapLink ();
		~BaseBitmapLink ();
	public:
		BaseBitmap *Get() const;
		void Set(BaseBitmap *bmp);
		
		static BaseBitmapLink *Alloc(void);
		static void Free(BaseBitmapLink *&bc);
};

class MovieLoader
{
	private:
		MovieLoader( void );
		~MovieLoader( void );
		// gDelete_TEMPLATE
		void	InitData( void );
		void	FreeData( void );
		
	public:
		static MovieLoader *Alloc( void );
		static void Free( MovieLoader *&ml );

		IMAGERESULT Open( const Filename &fn );
		void	Close( void );
		BaseBitmap	*Read( LONG new_frame_idx = -1, LONG *_result = NULL );
		LONG GetInfo( Real *_fps );

	private:
		BitmapLoaderPlugin	*plugin;
		BitmapLoaderAnimatedData	plugin_data;
		BaseBitmap *bm;
		Bool	is_movie;
		LONG	frame_cnt;
		Real	fps;
		LONG	frame_idx;
		IMAGERESULT result;
};

class MovieSaver
{
	private:
		MovieSaver();
		~MovieSaver();
	public:
		IMAGERESULT Open(const Filename &name, BaseBitmap *bm, LONG fps, LONG format, BaseContainer *data, SAVEBIT savebits, BaseSound *sound = NULL);
		IMAGERESULT Write(BaseBitmap *bm);
		void Close(void);
		Bool Choose(LONG format, BaseContainer *bc);

		static MovieSaver *Alloc(void);
		static void Free(MovieSaver *&bc);
};

class PluginMovieData;

class BitmapLoaderPlugin : public BasePlugin
{
	private:
		BitmapLoaderPlugin();
		~BitmapLoaderPlugin();
	public:
		Bool BmIdentify(const Filename &name, UCHAR *probe, LONG size);
		IMAGERESULT BmLoad(const Filename &name, BaseBitmap *bm, LONG frame);
		LONG BmGetSaver(void);
		Bool BmGetInformation(const Filename &name, LONG *frames, Real *fps);
		IMAGERESULT BmLoadAnimated(BitmapLoaderAnimatedData *bd, BITMAPLOADERACTION action, BaseBitmap *bm, LONG frame);
		IMAGERESULT BmExtractSound(BitmapLoaderAnimatedData *bd, BaseSound *snd);
};

class BitmapSaverPlugin : public BasePlugin
{
	private:
		BitmapSaverPlugin();
		~BitmapSaverPlugin();
	public:
		void BmGetDetails(LONG *alpha, String *suffix);
		Bool BmEdit(BaseContainer *data);
		LONG BmGetMaxResolution(Bool layers);
		IMAGERESULT BmSave(const Filename &name, BaseBitmap *bm, BaseContainer *data, SAVEBIT savebits);
		IMAGERESULT BmOpen(PluginMovieData *&md, const Filename &name, BaseBitmap *bm, BaseContainer *data, SAVEBIT savebits, LONG fps);
		IMAGERESULT BmWrite(PluginMovieData *md, BaseBitmap *bm);
		void BmClose(PluginMovieData *&md);
};

BaseBitmap *InitResourceBitmap(LONG resource_id);

#endif

#endif
