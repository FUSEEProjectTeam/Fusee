/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __GEPREPASS_H
#define __GEPREPASS_H

#include "ge_math.h"

#define MACTYPE_CINEMA		'C4DC'
#define MACCREATOR_CINEMA	'C4D1'

#ifdef __C4D_64BIT
	#define MAX_IMAGE_RESOLUTION 128000	// if changed, also change MAXMIPANZ
#else
	#define MAX_IMAGE_RESOLUTION 16000
#endif

// <Added> for Swig processing
#ifdef ENUM_END_FLAGS
#undef ENUM_END_FLAGS
#define ENUM_END_FLAGS(wtf)
#endif

#ifdef ENUM_END_LIST
#undef ENUM_END_LIST
#define ENUM_END_LIST(wtf)
#endif
// </Added>

enum RENDERJOBLIST
{
	RENDERJOBLIST_INACTIVE = 1 << 1,
	RENDERJOBLIST_ACTIVE	 = 1 << 2,
	RENDERJOBLIST_LOAD		 = 1 << 3,
	RENDERJOBLIST_ALL			 = 14	// RENDERJOBLIST_INACTIVE | RENDERJOBLIST_ACTIVE | RENDERJOBLIST_LOAD
} ENUM_END_FLAGS(RENDERJOBLIST);

enum MESSAGERESULT
{
	MESSAGERESULT_OK = 1000,
	MESSAGERESULT_CONNECTIONERROR,
	MESSAGERESULT_UNHANDLED,
	MESSAGERESULT_MEMORYERROR
} ENUM_END_LIST(MESSAGERESULT);

enum MACHINELIST
{
	MACHINELIST_ONLINE	= 1 << 1,
	MACHINELIST_OFFLINE = 1 << 0,
	MACHINELIST_ALL			= 7,	// MACHINELIST_ONLINE | MACHINELIST_OFFLINE
} ENUM_END_FLAGS(MACHINELIST);

enum VERIFICATIONBIT
{
	VERIFICATIONBIT_0					 = 0,
	VERIFICATIONBIT_ONLINE		 = 1 << 0,
	VERIFICATIONBIT_VERIFIED	 = 1 << 1,
	VERIFICATIONBIT_VERIFIEDME = 1 << 2,
	VERIFICATIONBIT_SHARED		 = 1 << 3,
	VERIFICATIONBIT_VERIFYING	 = 1 << 4,

	// error bits - if you add an enum also add it
	// to netrender/source/common.cpp#GetErrorVerificationBits
	VERIFICATIONBIT_UNKNOWN							 = 1 << 5,
	VERIFICATIONBIT_FAILED							 = 1 << 6,
	VERIFICATIONBIT_SECURITYTOKENCHANGED = 1 << 7,
	VERIFICATIONBIT_WRONGBUILDID				 = 1 << 8,
	VERIFICATIONBIT_WRONGARCHITECTURE		 = 1 << 9,
	VERIFICATIONBIT_REMOTENOTREACHABLE	 = 1 << 10,
	VERIFICATIONBIT_THISNOTREACHABLE		 = 1 << 11,
	VERIFICATIONBIT_WRONGSECURITYTOKEN	 = 1 << 12,
	VERIFICATIONBIT_DEMONOTACTIVATED		 = 1 << 13,
	VERIFICATIONBIT_REMOVING						 = 1 << 14
} ENUM_END_FLAGS(VERIFICATIONBIT);

enum RENDERJOBCREATOR
{
	RENDERJOBCREATOR_BATCHRENDER = 1000,
	RENDERJOBCREATOR_PICTUREVIEWER,
	RENDERJOBCREATOR_USER,
	RENDERJOBCREATOR_OTHER
} ENUM_END_LIST(RENDERJOBCREATOR);

enum STATUSNETSTATE
{
	STATUSNETSTATE_NONE = 0,
	STATUSNETSTATE_DISABLE,
	STATUSNETSTATE_IDLE,
	STATUSNETSTATE_BUSY,
	STATUSNETSTATE_BUSY2
} ENUM_END_FLAGS(STATUSNETSTATE);

#define C4DUUID_SIZE 16	// size of the uuid object

// BaseBitmap::Save
#define FILTER_TIF								1100
#define FILTER_TGA								1101
#define FILTER_BMP								1102
#define FILTER_IFF								1103
#define FILTER_JPG								1104
#define FILTER_PICT								1105
#define FILTER_PSD								1106
#define FILTER_RLA								1107
#define FILTER_RPF								1108
#define FILTER_B3D								1109
#define FILTER_TIF_B3D						1110
#define FILTER_PSB								1111
#define FILTER_AVI								1122
#define FILTER_MOVIE							1125
#define FILTER_QTVRSAVER_PANORAMA	1150
#define FILTER_QTVRSAVER_OBJECT		1151
#define FILTER_HDR								1001379
#define	FILTER_EXR_LOAD						1016605
#define	FILTER_EXR								1016606
#define FILTER_PNG								1023671
#define FILTER_IES								1024463
#define FILTER_B3DNET							1099	// private
#define FILTER_DPX								1023737

#define AVISAVER_FCCTYPE		10000
#define AVISAVER_FCCHANDLER	10001
#define AVISAVER_LKEY				10002
#define AVISAVER_LDATARATE	10003
#define AVISAVER_LQ					10004

#define QTSAVER_COMPRESSOR		 10010
#define QTSAVER_QUALITY				 10011
#define QTSAVER_TEMPQUAL			 10012
#define QTSAVER_FRAMERATE			 10013
#define QTSAVER_KEYFRAMES			 10014
#define QTSAVER_PLANES				 10015
#define QTSAVER_DATARATE			 10016
#define QTSAVER_FRAMEDURATION	 10017
#define QTSAVER_MINQUALITY		 10018
#define QTSAVER_MINTEMPQUAL		 10019
#define QTSAVER_FIXEDFRAMERATE 10020

#define JPGSAVER_QUALITY 10021
#define IMAGESAVER_DPI	 10022
#define PNG_INTERLACED	 11000
#define RLA_OPTIONS			 10024
#define DPX_PLANAR			 11000


enum RLAFLAGS
{
	RLAFLAGS_0							 = 0,
	RLAFLAGS_Z							 = (1 << 0),
	RLAFLAGS_OBJECTBUFFER		 = (1 << 2),
	RLAFLAGS_UV							 = (1 << 3),
	RLAFLAGS_NORMAL					 = (1 << 4),
	RLAFLAGS_ORIGCOLOR			 = (1 << 5),
	RLAFLAGS_COVERAGE				 = (1 << 6),
	RLAFLAGS_OBJECTID				 = (1 << 8),
	RLAFLAGS_COLOR					 = (1 << 9),
	RLAFLAGS_TRANSPARENCY		 = (1 << 10),
	RLAFLAGS_SUBPIXEL_WEIGHT = (1 << 12),
	RLAFLAGS_SUBPIXEL_MASK	 = (1 << 13)
} ENUM_END_FLAGS(RLAFLAGS);

enum ASSETDATA_FLAG
{
	ASSETDATA_FLAG_0								= 0,
	ASSETDATA_FLAG_CURRENTFRAMEONLY	= (1 << 0),
	ASSETDATA_FLAG_TEXTURES					= (1 << 1),	// only return texture assets
	ASSETDATA_FLAG_NET							= (1 << 2)  // set if NET is collecting assets to distribute them to the clients
} ENUM_END_FLAGS(ASSETDATA_FLAG);

// savebits
enum SAVEBIT
{
	SAVEBIT_0									= 0,
	SAVEBIT_ALPHA							= (1 << 0),
	SAVEBIT_MULTILAYER				= (1 << 1),
	SAVEBIT_USESELECTEDLAYERS	= (1 << 2),
	SAVEBIT_16BITCHANNELS			= (1 << 3),
	SAVEBIT_GREYSCALE					= (1 << 4),
	SAVEBIT_INTERNALNET				= (1 << 5),	// private
	SAVEBIT_DONTMERGE					= (1 << 7),	// flag to avoid merging of layers in b3d files
	SAVEBIT_32BITCHANNELS			= (1 << 8),
	SAVEBIT_SAVERENDERRESULT	= (1 << 9),
	SAVEBIT_FIRSTALPHA_ONLY		= (1 << 10)	// private
} ENUM_END_FLAGS(SAVEBIT);

enum SCENEFILTER
{
	SCENEFILTER_0								 = 0,
	SCENEFILTER_OBJECTS					 = (1 << 0),
	SCENEFILTER_MATERIALS				 = (1 << 1),
	SCENEFILTER_DIALOGSALLOWED	 = (1 << 3),
	SCENEFILTER_PROGRESSALLOWED	 = (1 << 4),
	SCENEFILTER_MERGESCENE			 = (1 << 5),
	SCENEFILTER_NONEWMARKERS		 = (1 << 6),
	SCENEFILTER_SAVECACHES			 = (1 << 7),	// for melange export only
	SCENEFILTER_NOUNDO					 = (1 << 8),
	SCENEFILTER_SAVE_BINARYCACHE = (1 << 10),
	SCENEFILTER_IDENTIFY_ONLY		 = (1 << 11)
} ENUM_END_FLAGS(SCENEFILTER);

// GeOutString
enum GEMB
{
	GEMB_OK								= 0x0000,
	GEMB_OKCANCEL					= 0x0001,
	GEMB_ABORTRETRYIGNORE	= 0x0002,
	GEMB_YESNOCANCEL			= 0x0003,
	GEMB_YESNO						= 0x0004,
	GEMB_RETRYCANCEL			= 0x0005,
	GEMB_FORCEDIALOG			= 0x8000,
	GEMB_ICONSTOP					= 0x0010,
	GEMB_ICONQUESTION			= 0x0020,
	GEMB_ICONEXCLAMATION	= 0x0030,
	GEMB_ICONASTERISK			= 0x0040,
	GEMB_MULTILINE				= 0x0080
} ENUM_END_FLAGS(GEMB);

enum GEMB_R
{
	GEMB_R_UNDEFINED = 0,
	GEMB_R_OK				 = 1,
	GEMB_R_CANCEL		 = 2,
	GEMB_R_ABORT		 = 3,
	GEMB_R_RETRY		 = 4,
	GEMB_R_IGNORE		 = 5,
	GEMB_R_YES			 = 6,
	GEMB_R_NO				 = 7
} ENUM_END_LIST(GEMB_R);

enum MOUSEDRAGRESULT
{
	MOUSEDRAGRESULT_ESCAPE	 = 1,
	MOUSEDRAGRESULT_FINISHED = 2,
	MOUSEDRAGRESULT_CONTINUE = 3
} ENUM_END_LIST(MOUSEDRAGRESULT);

enum MOUSEDRAGFLAGS
{
	MOUSEDRAGFLAGS_0										 = 0,
	MOUSEDRAGFLAGS_DONTHIDEMOUSE				 = (1 << 0),	// mousepointer should be visible
	MOUSEDRAGFLAGS_NOMOVE								 = (1 << 1),	// mousedrag returns if no mousemove was done
	MOUSEDRAGFLAGS_EVERYPACKET					 = (1 << 2),	// receive every packet of the queue, otherwise only data of the last packet
	MOUSEDRAGFLAGS_COMPENSATEVIEWPORTORG = (1 << 3),	// compensates the viewport origin during drag
	MOUSEDRAGFLAGS_AIRBRUSH							 = (1 << 4)
} ENUM_END_FLAGS(MOUSEDRAGFLAGS);

enum INITRENDERRESULT
{
	INITRENDERRESULT_OK						= 0,
	INITRENDERRESULT_OUTOFMEMORY	= -100,
	INITRENDERRESULT_ASSETMISSING	= -101,
	INITRENDERRESULT_UNKNOWNERROR	= -102,
	INITRENDERRESULT_THREADEDLOCK	= -103
} ENUM_END_LIST(INITRENDERRESULT);

enum RENDERRESULT
{
	RENDERRESULT_OK									 = 0,
	RENDERRESULT_OUTOFMEMORY				 = 1,
	RENDERRESULT_ASSETMISSING				 = 2,
	RENDERRESULT_SAVINGFAILED				 = 5,
	RENDERRESULT_USERBREAK					 = 6,
	RENDERRESULT_GICACHEMISSING			 = 7,
	RENDERRESULT_NOMACHINE					 = 9,			// can only happen during team rendering

	RENDERRESULT_PROJECTNOTFOUND		 = 1000,	// can only be returned by the app during command line rendering
	RENDERRESULT_ERRORLOADINGPROJECT = 1001,	// can only be returned by the app during command line rendering
	RENDERRESULT_NOOUTPUTSPECIFIED	 = 1002		// can only be returned by the app during command line rendering
} ENUM_END_LIST(RENDERRESULT);

#define BITDEPTH_SHIFT 4

#define BITDEPTH_MAXMODES	3

#define BITDEPTH_UCHAR 0
#define BITDEPTH_UWORD 1
#define BITDEPTH_FLOAT 2

// color mode for bitmaps
// the most common values are COLORMODE_RGB for 24-bit RGB values and COLORMODE_GRAY for 8-bit greyscale values
enum COLORMODE
{
	COLORMODE_ILLEGAL	= 0,

	COLORMODE_ALPHA		= 1,	// only alpha channel
	COLORMODE_GRAY		= 2,
	COLORMODE_AGRAY		= 3,
	COLORMODE_RGB			= 4,
	COLORMODE_ARGB		= 5,
	COLORMODE_CMYK		= 6,
	COLORMODE_ACMYK		= 7,
	COLORMODE_MASK		= 8,	// gray map as mask
	COLORMODE_AMASK		= 9,	// gray map as mask

	// 16 bit modes
	COLORMODE_ILLEGALw = (BITDEPTH_UWORD << BITDEPTH_SHIFT),

	COLORMODE_GRAYw		 = (COLORMODE_GRAY | (BITDEPTH_UWORD << BITDEPTH_SHIFT)),
	COLORMODE_AGRAYw	 = (COLORMODE_AGRAY | (BITDEPTH_UWORD << BITDEPTH_SHIFT)),
	COLORMODE_RGBw		 = (COLORMODE_RGB | (BITDEPTH_UWORD << BITDEPTH_SHIFT)),
	COLORMODE_ARGBw		 = (COLORMODE_ARGB | (BITDEPTH_UWORD << BITDEPTH_SHIFT)),
	COLORMODE_MASKw		 = (COLORMODE_MASK | (BITDEPTH_UWORD << BITDEPTH_SHIFT)),

	// 32 bit modes
	COLORMODE_ILLEGALf = (BITDEPTH_FLOAT << BITDEPTH_SHIFT),

	COLORMODE_GRAYf		 = (COLORMODE_GRAY | (BITDEPTH_FLOAT << BITDEPTH_SHIFT)),
	COLORMODE_AGRAYf	 = (COLORMODE_AGRAY | (BITDEPTH_FLOAT << BITDEPTH_SHIFT)),
	COLORMODE_RGBf		 = (COLORMODE_RGB | (BITDEPTH_FLOAT << BITDEPTH_SHIFT)),
	COLORMODE_ARGBf		 = (COLORMODE_ARGB | (BITDEPTH_FLOAT << BITDEPTH_SHIFT)),
	COLORMODE_MASKf		 = (COLORMODE_MASK | (BITDEPTH_FLOAT << BITDEPTH_SHIFT))
} ENUM_END_FLAGS(COLORMODE);

enum COLORSPACETRANSFORMATION
{
	COLORSPACETRANSFORMATION_NONE						= 0,
	COLORSPACETRANSFORMATION_LINEAR_TO_SRGB	= 1,
	COLORSPACETRANSFORMATION_SRGB_TO_LINEAR	= 2,

	COLORSPACETRANSFORMATION_LINEAR_TO_VIEW	= 10,
	COLORSPACETRANSFORMATION_SRGB_TO_VIEW		= 11
} ENUM_END_LIST(COLORSPACETRANSFORMATION);

enum PIXELCNT
{
	PIXELCNT_0									 = 0,
	PIXELCNT_DITHERING					 = (1 << 0),	// allow dithering
	PIXELCNT_B3DLAYERS					 = (1 << 1),	// merge b3d layers (multipassbmp)
	PIXELCNT_APPLYALPHA					 = (1 << 2),	// apply alpha layers to the result (paintlayer)
	PIXELCNT_INTERNAL_SETLINE		 = (1 << 29),	// PRIVATE! internal setline indicator
	PIXELCNT_INTERNAL_ALPHAVALUE = (1 << 30)	// PRIVATE! get also the alphavalue (rgba 32 bit)
} ENUM_END_FLAGS(PIXELCNT);

enum INITBITMAPFLAGS
{
	INITBITMAPFLAGS_0					= 0,
	INITBITMAPFLAGS_GRAYSCALE	= (1 << 0),
	INITBITMAPFLAGS_SYSTEM		= (1 << 1)
} ENUM_END_FLAGS(INITBITMAPFLAGS);

enum MPB_GETLAYERS
{
	MPB_GETLAYERS_0			= 0,
	MPB_GETLAYERS_ALPHA	= (1 << 1),
	MPB_GETLAYERS_IMAGE	= (1 << 2)
} ENUM_END_FLAGS(MPB_GETLAYERS);

enum MPBTYPE
{
	MPBTYPE_SHOW			 = 1000,	// Bool, get, set
	MPBTYPE_SAVE			 = 1001,	// Bool, get, set
	MPBTYPE_PERCENT		 = 1002,	// Float, get, set
	MPBTYPE_BLENDMODE	 = 1003,	// Int32, get, set
	MPBTYPE_COLORMODE	 = 1004,	// Int32, get, set
	MPBTYPE_BITMAPTYPE = 1005,	// Int32, get
	MPBTYPE_NAME			 = 1006,	// String, get, set
	MPBTYPE_DPI				 = 1007,	// Int32, get, set
	MPBTYPE_USERID		 = 1008,	// Int32, get, set
	MPBTYPE_USERSUBID	 = 1009,	// Int32, get, set
	MPBTYPE_FORCEBLEND = 1010		// Int32, get, set		(special mode used to force blend layers)
} ENUM_END_LIST(MPBTYPE);

enum LENGTHUNIT
{
	LENGTHUNIT_PIXEL = 0,
	LENGTHUNIT_KM		 = 1,
	LENGTHUNIT_M		 = 2,
	LENGTHUNIT_CM		 = 3,
	LENGTHUNIT_MM		 = 4,
	LENGTHUNIT_UM		 = 5,
	LENGTHUNIT_NM		 = 6,
	LENGTHUNIT_MILE	 = 7,
	LENGTHUNIT_YARD	 = 8,
	LENGTHUNIT_FEET	 = 9,
	LENGTHUNIT_INCH	 = 10
} ENUM_END_LIST(LENGTHUNIT);

enum SPLINETYPE
{
	SPLINETYPE_LINEAR	 = 0,
	SPLINETYPE_CUBIC	 = 1,
	SPLINETYPE_AKIMA	 = 2,
	SPLINETYPE_BSPLINE = 3,
	SPLINETYPE_BEZIER	 = 4
} ENUM_END_LIST(SPLINETYPE);

// particle bits
enum PARTICLEFLAGS
{
	PARTICLEFLAGS_0				= 0,
	PARTICLEFLAGS_VISIBLE	= (1 << 0),
	PARTICLEFLAGS_ALIVE		= (1 << 1)
} ENUM_END_FLAGS(PARTICLEFLAGS);

// baselist N-bits
enum NBIT
{
	NBIT_0					 = 0,

	NBIT_TL1_FOLD		 = 1,
	NBIT_TL2_FOLD		 = 2,
	NBIT_TL3_FOLD		 = 3,
	NBIT_TL4_FOLD		 = 4,

	NBIT_TL1_SELECT	 = 5,
	NBIT_TL2_SELECT	 = 6,
	NBIT_TL3_SELECT	 = 7,
	NBIT_TL4_SELECT	 = 8,

	NBIT_TL1_TDRAW	 = 9,
	NBIT_TL2_TDRAW	 = 10,
	NBIT_TL3_TDRAW	 = 11,
	NBIT_TL4_TDRAW	 = 12,

	NBIT_CKEY_ACTIVE = 13,	// active point of animation path in editor

	NBIT_OM1_FOLD		 = 14,
	NBIT_OM2_FOLD		 = 15,
	NBIT_OM3_FOLD		 = 16,
	NBIT_OM4_FOLD		 = 17,

	// defines if the tracks of the object are shown
	NBIT_TL1_FOLDTR						= 18,
	NBIT_TL2_FOLDTR						= 19,
	NBIT_TL3_FOLDTR						= 20,
	NBIT_TL4_FOLDTR						= 21,

	NBIT_TL1_FOLDFC						= 22,
	NBIT_TL2_FOLDFC						= 23,
	NBIT_TL3_FOLDFC						= 24,
	NBIT_TL4_FOLDFC						= 25,

	NBIT_SOURCEOPEN						= 26,

	NBIT_TL1_HIDE							= 27,
	NBIT_TL2_HIDE							= 28,
	NBIT_TL3_HIDE							= 29,
	NBIT_TL4_HIDE							= 30,

	NBIT_SOLO_ANIM						= 31,
	NBIT_SOLO_LAYER						= 32,

	NBIT_TL1_SELECT2					= 33,
	NBIT_TL2_SELECT2					= 34,
	NBIT_TL3_SELECT2					= 35,
	NBIT_TL4_SELECT2					= 36,

	NBIT_SOLO_MOTION					= 37,

	NBIT_CKEY_LOCK_T					= 38,
	NBIT_CKEY_LOCK_V					= 39,
	NBIT_CKEY_MUTE						= 40,
	NBIT_CKEY_CLAMP						= 41,

	NBIT_CKEY_BREAK						= 42,
	NBIT_CKEY_KEEPVISUALANGLE = 43,

	NBIT_CKEY_LOCK_O					= 44,
	NBIT_CKEY_LOCK_L					= 45,
	NBIT_CKEY_AUTO						= 46,
	NBIT_CKEY_ZERO_O_OLD			= 48,
	NBIT_CKEY_ZERO_L_OLD			= 49,

	NBIT_TL1_FCSELECT					= 50,
	NBIT_TL2_FCSELECT					= 51,
	NBIT_TL3_FCSELECT					= 52,
	NBIT_TL4_FCSELECT					= 53,

	NBIT_CKEY_BREAKDOWN				= 54,

	NBIT_TL1_FOLDMOTION				= 55,
	NBIT_TL2_FOLDMOTION				= 56,
	NBIT_TL3_FOLDMOTION				= 57,
	NBIT_TL4_FOLDMOTION				= 58,

	NBIT_TL1_SELECTMOTION			= 59,
	NBIT_TL2_SELECTMOTION			= 60,
	NBIT_TL3_SELECTMOTION			= 61,
	NBIT_TL4_SELECTMOTION			= 62,

	NBIT_OHIDE								= 63,	// Hide object in OM
	NBIT_TL_TBAKE							= 64,

	NBIT_TL1_FOLDSM						= 66,
	NBIT_TL2_FOLDSM						= 67,
	NBIT_TL3_FOLDSM						= 68,
	NBIT_TL4_FOLDSM						= 69,

	NBIT_SUBOBJECT						= 70,
	NBIT_LINK_ACTIVE					= 71,
	NBIT_THIDE								= 72,	// hide in TL
	NBIT_SUBOBJECT_AM					= 74,
	NBIT_PROTECTION						= 75,	// psr protected
	NBIT_NOANIM								= 76,	// no animation
	NBIT_NOSELECT							= 77,	// no selection
	NBIT_EHIDE								= 78,	// hide in viewport
	NBIT_REF									= 79,	// x-ref
	NBIT_REF_NO_DD						= 80,	// x-ref private
	NBIT_REF_OHIDE						= 81,	// x-ref private
	NBIT_NO_DD								= 82,	// disallow duplication/d&d

	NBIT_MAX									= 83,
	NBIT_PRIVATE_MASK1				= -1,
	NBIT_PRIVATE_MASK2				= -2,
	NBIT_PRIVATE_MASK3				= -3,
	NBIT_PRIVATE_MASK4				= -4
} ENUM_END_LIST(NBIT);

enum CREATEJOBRESULT
{
	CREATEJOBRESULT_OK							= 0,
	CREATEJOBRESULT_OUTOFMEMORY			= 1,
	CREATEJOBRESULT_ASSETMISSING		= 2,
	CREATEJOBRESULT_SAVINGFAILED		= 3,
	CREATEJOBRESULT_REPOSITORYERROR	= 4
} ENUM_END_FLAGS(CREATEJOBRESULT);

enum NBITCONTROL
{
	NBITCONTROL_SET							= 1,
	NBITCONTROL_CLEAR						= 2,
	NBITCONTROL_TOGGLE					= 3,
	NBITCONTROL_PRIVATE_NODIRTY	= 0xf0
} ENUM_END_FLAGS(NBITCONTROL);

// baselist bits
#define BIT_ACTIVE	(1 << 1)
#define BIT_ACTIVE2	(1 << 29)

// material bits
#define BIT_MATMARK				(1 << 2)	// marked material
#define BIT_ENABLEPAINT		(1 << 3)	// enable painting
#define BIT_RECALCPREVIEW	(1 << 5)	// recalculate preview
#define BIT_MFOLD					(1 << 6)	// folded in material manager
#define BIT_BP_FOLDLAYERS	(1 << 9)	// fold layers in material manger

// object bits
#define BIT_IGNOREDRAW										 (1 << 2)		// ignore object during draw
#define BIT_OFOLD													 (1 << 6)		// folded in object manager
#define BIT_CONTROLOBJECT									 (1 << 9)		// control object
#define BIT_RECMARK												 (1 << 11)	// help bit for recursive operations
#define BIT_IGNOREDRAWBOX									 (1 << 12)	// ignore box drawing object
#define BIT_EDITOBJECT										 (1 << 13)	// edit object from sds
#define BIT_ACTIVESELECTIONDRAW						 (1 << 15)	// draw active selection
#define BIT_TEMPDRAW_VISIBLE_CACHECHILD		 (1 << 16)	// private, temp bits for faster redraw
#define BIT_TEMPDRAW_VISIBLE_DEFCACHECHILD (1 << 17)	// private, temp bits for faster redraw
#define BIT_TEMPDRAW_VISIBLE_CHILD				 (1 << 18)	// private, temp bits for faster redraw
#define BIT_HIGHLIGHT											 (1 << 20)	// object highlighted in viewport
#define BIT_FORCE_UNOPTIMIZED							 (1 << 21)	// do not optimize the points of a polygon object during OGl redraw

// track bits
#define BIT_TRACKPROCESSED				(1 << 16)	// track has been processed, avoid recursions
#define BIT_ANIM_OFF							(1 << 17)	// is sequence inactive
#define BIT_ANIM_SOLO							(1 << 18)
#define BIT_ANIM_CONSTANTVELOCITY	(1 << 19)

// videopost bits
#define BIT_VPDISABLED (1 << 2)	// videopost effect is disabled

// document bits
#define BIT_DOCUMENT_CHECKREWIND (1 << 2)	// doc needs to check for a rewind

// renderdata bits
#define BIT_ACTIVERENDERDATA (1 << 28)

// object info
#define OBJECT_MODIFIER					 (1 << 0)
#define OBJECT_HIERARCHYMODIFIER (1 << 1)
#define OBJECT_GENERATOR				 (1 << 2)
#define OBJECT_INPUT						 (1 << 3)
#define OBJECT_PARTICLEMODIFIER	 (1 << 5)
#define OBJECT_NOCACHESUB				 (1 << 6)
#define OBJECT_ISSPLINE					 (1 << 7)
#define OBJECT_UNIQUEENUMERATION (1 << 8)
#define OBJECT_CAMERADEPENDENT	 (1 << 9)
#define OBJECT_USECACHECOLOR		 (1 << 10)
#define OBJECT_POINTOBJECT			 (1 << 11)
#define OBJECT_POLYGONOBJECT		 (1 << 12)
#define OBJECT_NO_PLA						 (1 << 13)
#define OBJECT_DONTFREECACHE		 (1 << 14)
#define OBJECT_CALL_ADDEXECUTION (1 << 15)

///////////////////ID's/////////////////////

// list elements
#define Tbaselist2d		110050
#define Tbasedocument	110059
#define Tpluginlayer	110064
#define Tundoablelist	110068
#define Tgelistnode		110069

// materials
#define Mbase				5702
#define Mmaterial		5703
#define Mplugin			5705
#define Mfog				8803
#define Mterrain		8808
#define Mdanel			1011117
#define Mbanji			1011118
#define Mbanzi			1011119
#define Mcheen			1011120
#define Mmabel			1011121
#define Mnukei			1011122
#define MCgFX				450000237
#define Marchigrass	1028461	// Architectural Grass Material

// videopost
#define VPbase 5709

// objects
#define Opolygon					5100
#define Ospline						5101
#define Olight						5102
#define Ocamera						5103
#define Ofloor						5104
#define Osky							5105
#define Oenvironment			5106
#define Oloft							5107
#define Offd							5108
#define Oparticle					5109
#define Odeflector				5110
#define Ogravitation			5111
#define Orotation					5112
#define Owind							5113
#define Ofriction					5114
#define Oturbulence				5115
#define Oextrude					5116
#define Olathe						5117
#define Osweep						5118
#define Oattractor				5119
#define Obezier						5120
#define Oforeground				5121
#define Obackground				5122
#define Obone_EX					5123
#define Odestructor				5124
#define Ometaball					5125
#define Oinstance					5126
#define Obend							5128
#define Obulge						5129
#define Oshear						5131
#define Otaper						5133
#define Otwist						5134
#define Owave							5135
#define Ostage						5136
#define Oline							5137
#define Omicrophone				5138
#define Oloudspeaker			5139
#define Onull							5140
#define Osymmetry					5142
#define Owrap							5143
#define Oboole						1010865
#define Oexplosion				5145
#define Oformula					5146
#define Omelt							5147
#define Oshatter					5148
#define Owinddeform				5149
#define Oarray						5150
#define Oheadphone				5151
#define Oworkplane				5153
#define Oplugin						5154
#define Obase							5155	// for instanceof!
#define Opoint						5156	// for instanceof!
#define Obasedeform				5157	// for instanceof!
#define Oparticlemodifier	5158	// for instanceof!
#define Opolyreduction		1001253
#define Oshowdisplacement	1001196
#define Ojoint						1019362
#define Oskin							1019363
#define Oweighteffector		1019677
#define Ocharacter				1021433
#define Ocmotion					1021824
#define Oxref							1025766
#define Ocube							5159
#define Osphere						5160
#define Oplatonic					5161
#define Ocone							5162
#define Otorus						5163
#define Odisc							5164
#define Otube							5165
#define Ofigure						5166
#define Opyramid					5167
#define Oplane						5168
#define Ofractal					5169
#define Ocylinder					5170
#define Ocapsule					5171
#define Ooiltank					5172
#define Orelief						5173
#define Osinglepoly				5174

#define Opluginpolygon 1001091

// spline primitive objects
#define Osplineprimitive 5152	// base description
#define Osplineprofile	 5175
#define Osplineflower		 5176
#define Osplineformula	 5177
#define Osplinetext			 5178
#define Osplinenside		 5179
#define Ospline4side		 5180
#define Osplinecircle		 5181
#define Osplinearc			 5182
#define Osplinecissoid	 5183
#define Osplinecycloid	 5184
#define Osplinehelix		 5185
#define Osplinerectangle 5186
#define Osplinestar			 5187
#define Osplinecogwheel	 5188
#define Osplinecontour	 5189

#define Oselection				5190
#define Osds							1007455
#define Osplinedeformer		1008982
#define Osplinerail				1008796
#define Oatomarray				1001002
#define Ospherify					1001003
#define Oexplosionfx			1002603
#define Oconnector				1011010
#define Oalembicgenerator 1028083

// small listnode plugin
#define Yplugin	110061

// big listnode plugin
#define Zplugin	110062

// DLayerStruct object
#define Olayer 100004801

// (virtual) filter base
#define Fbase	1001024

// multipass render settings element
#define Zmultipass 300001048

#define SHplugin 110065
#define	VPplugin 110066

// listhead
#define ID_LISTHEAD	110063

// render data
#define Rbase 110304

// shader plugins
#define Xbase							5707
#define Xcolor						5832
#define Xbitmap						5833
#define Xbrick						5804
#define Xcheckerboard			5800
#define Xcloud						5802
#define Xcolorstripes			5822
#define Xcyclone					5821
#define Xearth						5825
#define Xfire							5803
#define Xflame						5817
#define Xgalaxy						5813
#define Xmetal						5827
#define Xsimplenoise			5807
#define Xrust							5828
#define Xstar							5816
#define Xstarfield				5808
#define Xsunburst					5820
#define Xsimpleturbulence	5806
#define Xvenus						5826
#define Xwater						5818
#define Xwood							5823
#define Xplanet						5829
#define Xmarble						5830
#define Xspectral					5831
#define Xgradient					1011100
#define Xfalloff					1011101
#define Xtiles						1011102
#define Xfresnel					1011103
#define Xlumas						1011105
#define Xproximal					1011106
#define Xnormaldirection	1011107
#define Xtranslucency			1011108
#define Xfusion						1011109
#define Xposterizer				1011111
#define Xcolorizer				1011112
#define Xdistorter				1011114
#define Xprojector				1011115
#define Xnoise						1011116
#define Xlayer						1011123
#define Xspline						1011124
#define Xfilter						1011128
#define Xripple						1011199
#define Xvertexmap				1011137
#define Xsss							1001197
#define Xambientocclusion	1001191
#define Xchanlum					1007539
#define Xmosaic						1022119
#define Xxmbsubsurface		1025614
#define Xrainsampler			1026576
#define Xnormalizer				1026588
#define Xreference				1027315
#define Xxmbreflection		1027741
#define	Xterrainmask			1026277


// tags
#define Tpoint						5600
#define	Tanchor_EX				5608
#define	Tphong						5612
#define	Tdisplay					5613
#define	Tkinematic_EX			5614
#define	Ttexture					5616
#define	Ttangent					5617
#define	Tprotection				5629
#define	Tparticle					5630
#define	Tmotionblur				5636
#define	Tcompositing			5637
#define	Twww							5647
#define	Tsavetemp					5650
#define Tpolygon					5604
#define	Tuvw							5671
#define	Tsegment					5672
#define	Tpolygonselection	5673
#define	Tpointselection		5674
#define Tcoffeeexpression	5675
#define	Ttargetexpression	5676
#define	Tfixexpression_EX	5677
#define	Tsunexpression		5678
#define	Tikexpression_EX	5679
#define Tline							5680
#define Tvertexmap				5682
#define Trestriction			5683
#define	Tmetaball					5684
#define	Tbakeparticle			5685
#define Tmorph						5689
#define Tsticktexture			5690
#define	Tplugin						5691
#define	Tstop							5693
#define Tbase							5694	// for instanceof
#define Tvariable					5695	// for instanceof
#define Tvibrate					5698
#define Taligntospline		5699
#define Taligntopath			5700
#define	Tedgeselection		5701
#define Tclaudebonet_EX		5708
#define Tnormal						5711
#define Tcorner						5712
#define Tsds							1007579
#define Tlookatcamera			1001001
#define Texpresso					1001149
#define Tsoftselection		1016641
#define Tbaketexture			1011198
#define Tsdsdata					1018016
#define Tweights					1019365
#define Tposemorph				1024237
#define Tpython						1022749
#define Tsculpt						1023800
#define Tmotioncam				1027742	// Motion Camera Tag
#define Tmorphcam					1027743	// Morph Camera Tag
#define	Tcrane						1028270	// Camera Crane Tag
#define Tarchigrass				1028463	// Architectural Grass Tag
#define Tsculptnormals		1027660 // Private for sculpting

// new anim system
#define NLAbase	5349	// nla system
#define CTbase	5350	// anim system
#define CSbase	5351
#define CKbase	5352

#define CTpla		100004812
#define CTsound	100004813
#define CTmorph	100004822
#define CTtime	-1

#define	TL_MARKEROBJ 465001514

#define ID_MACHINE			300002140
#define ID_MACHINEGROUP	300002142

#define GVbase 1001101

#define ID_BS_HOOK 100004808

// modeling commands
#define MCOMMAND_SPLINE_HARDINTERPOLATION	100
#define MCOMMAND_SPLINE_SOFTINTERPOLATION	101
#define MCOMMAND_SPLINE_REORDER						102
#define MCOMMAND_SPLINE_REVERSE						103
#define MCOMMAND_SPLINE_MOVEDOWN					104
#define MCOMMAND_SPLINE_MOVEUP						105
#define MCOMMAND_SPLINE_JOINSEGMENT				109
#define MCOMMAND_SPLINE_BREAKSEGMENT			110
#define MCOMMAND_SPLINE_EQUALLENGTH				111
#define MCOMMAND_SPLINE_EQUALDIRECTION		112
#define MCOMMAND_SPLINE_LINEUP						113
#define MCOMMAND_SPLINE_CREATEOUTLINE			114
#define MCOMMAND_SPLINE_PROJECT						115
#define MCOMMAND_SPLINE_ADDPOINT					116
#define MCOMMAND_SELECTALL								200
#define MCOMMAND_DESELECTALL							201
#define MCOMMAND_SELECTINVERSE						202
#define MCOMMAND_SELECTCONNECTED					203
#define MCOMMAND_SELECTGROW								204
#define MCOMMAND_SELECTSHRINK							205
#define MCOMMAND_SELECTPOINTTOPOLY				206
#define MCOMMAND_SELECTPOLYTOPOINT				207
#define MCOMMAND_SELECTADJACENT						208
#define MCOMMAND_GENERATESELECTION				209
#define MCOMMAND_HIDESELECTED							211
#define MCOMMAND_HIDEUNSELECTED						212
#define MCOMMAND_HIDEINVERT								213
#define MCOMMAND_UNHIDE										214
#define MCOMMAND_REVERSENORMALS						217
#define MCOMMAND_ALIGNNORMALS							218
#define MCOMMAND_SPLIT										220
#define MCOMMAND_TRIANGULATE							221
#define MCOMMAND_UNTRIANGULATE						222
#define MCOMMAND_DELETE										224
#define MCOMMAND_OPTIMIZE									227
#define MCOMMAND_DISCONNECT								228
#define MCOMMAND_MAKEEDITABLE							230
#define MCOMMAND_MIRROR										237
#define MCOMMAND_MATRIXEXTRUDE						238
#define MCOMMAND_SUBDIVIDE								242
#define MCOMMAND_EXPLODESEGMENTS					243
#define MCOMMAND_KNIFE										244
#define MCOMMAND_CURRENTSTATETOOBJECT			245
#define MCOMMAND_JOIN											246
#define MCOMMAND_CONVERTSELECTION					247
#define MCOMMAND_EDGE_TO_SPLINE						251
#define MCOMMAND_BREAKPHONG								255
#define MCOMMAND_UNBREAKPHONG							256
#define MCOMMAND_PHONGTOSELECTION					257
#define MCOMMAND_MELT											264
#define MCOMMAND_RESETSYSTEM							265

// container data for modeling commands
#define MDATA_SPLINE_FREEHANDTOLERANCE					 2020	// REAL
#define MDATA_SPLINE_OUTLINE										 2021	// REAL
#define MDATA_SPLINE_PROJECTMODE								 2022	// Int32
#define MDATA_SPLINE_ADDPOINTSEGMENT						 2023	// Int32
#define MDATA_SPLINE_ADDPOINTPOSITION						 2024	// REAL
#define MDATA_SPLINE_ADDPOINTSELECT							 2025
#define MDATA_DISCONNECT_PRESERVEGROUPS					 2028	// BOOL
#define MDATA_MIRROR_SNAPPOINTS									 2069	// BOOL
#define MDATA_MIRROR_DUPLICATE									 2070	// BOOL
#define MDATA_MIRROR_WELD												 2071	// BOOL
#define MDATA_MIRROR_TOLERANCE									 2072	// REAL
#define MDATA_MIRROR_SYSTEM											 2067	// Int32
#define MDATA_MIRROR_PLANE											 2068	// Int32
#define MDATA_MIRROR_VALUE											 2073	// REAL
#define MDATA_MIRROR_POINT											 2074	// VECTOR
#define MDATA_MIRROR_VECTOR											 2075	// VECTOR
#define MDATA_OPTIMIZE_TOLERANCE								 2076	// REAL
#define MDATA_OPTIMIZE_POINTS										 2077	// BOOL
#define MDATA_OPTIMIZE_POLYGONS									 2078	// BOOL
#define MDATA_OPTIMIZE_UNUSEDPOINTS							 2079	// BOOL
#define	MDATA_SPLINE_OUTLINESEPARATE						 2080	// BOOL
#define MDATA_CROSSSECTION_ANGLE								 2082	// REAL
#define MDATA_SUBDIVIDE_HYPER										 2098	// BOOL
#define MDATA_SUBDIVIDE_ANGLE										 2099	// REAL
#define MDATA_SUBDIVIDE_SPLINESUB								 2100	// Int32
#define MDATA_SUBDIVIDE_SUB											 2101	// Int32
#define MDATA_KNIFE_P1													 2110	// VECTOR - only for send command
#define MDATA_KNIFE_V1													 2111	// VECTOR - only for send command
#define MDATA_KNIFE_P2													 2112	// VECTOR - only for send command
#define MDATA_KNIFE_V2													 2113	// VECTOR - only for send command
#define MDATA_KNIFE_ANGLE												 2115	// REAL
#define MDATA_MIRROR_SELECTIONS									 2120	// BOOL
#define MDATA_UNTRIANGULATE_ANGLE								 2121	// BOOL
#define MDATA_MIRROR_ONPLANE										 2122	// BOOL
#define MDATA_CONVERTSELECTION_LEFT							 2126	// Int32
#define MDATA_CONVERTSELECTION_RIGHT						 2127	// Int32
#define MDATA_CONVERTSELECTION_TOLERANT					 2128	// BOOL
#define MDATA_CURRENTSTATETOOBJECT_INHERITANCE	 2140	// BOOL
#define MDATA_CURRENTSTATETOOBJECT_KEEPANIMATION 2141	// BOOL
#define MDATA_ROTATECAMERA											 2142	// BOOL
#define MDATA_RING_EDGE													 2154	// Int32
#define MDATA_RING_SELECTION										 2155	// Int32 (must be SELECTION_NEW, SELECTION_ADD or SELECTION_SUB)
#define MDATA_RING_SKIP													 2156	// Int32
#define MDATA_FILLSEL_START_POLY								 2157	// Int32
#define MDATA_FILLSEL_SELECTION									 2158	// Int32 (must be SELECTION_NEW, SELECTION_ADD or SELECTION_SUB)
#define MDATA_OUTLINESEL_START_POLY							 2159	// Int32
#define MDATA_OUTLINESEL_SELECTION							 2160	// Int32 (must be SELECTION_NEW, SELECTION_ADD or SELECTION_SUB)
#define MDATA_UNTRIANGULATE_NGONS								 2143	// BOOL
#define MDATA_UNTRIANGULATE_ANGLE_RAD						 2161	// REAL
#define MDATA_CURRENTSTATETOOBJECT_NOGENERATE		 2162	// BOOL
#define MDATA_RESETSYSTEM_COMPENSATE						 2165	// BOOL
#define MDATA_RESETSYSTEM_RECURSIVE							 2166	// BOOL
#define MDATA_JOIN_MERGE_SELTAGS								 2167	// BOOL

#define MDATA_SETVERTEX_VALUE	4000										// REAL
#define MDATA_SETVERTEX_MODE	4001										// Int32

// world preferences
#define WPREF_UNITS_BASIC				10000			// Int32
#define WPREF_UNITS_TIME				10001			// Int32
#define WPREF_UNITS_AUTOCONVERT 10002			// BOOL
#define WPREF_UNITS_USEUNITS		10003			// BOOL
#define WPREF_REFRESHTIME				10004			// Int32
#define WPREF_RATIO							10005			// REAL

#define WPREF_CENTER								1002	// BOOL
#define WPREF_TABLET								1005	// BOOL
#define WPREF_OPENGL								1008	// BOOL
#define WPREF_LINK_SELECTION				1009	// BOOL
#define WPREF_FULLANIMREDRAW				1010	// BOOL
#define WPREF_SAVE_LAYOUT						1014	// BOOL
#define WPREF_INSERTAT							1016	// Int32
#define WPREF_PASTEAT								1017	// Int32
#define WPREF_MAX_UNDOS							1018	// Int32
#define WPREF_MAX_LAST							1019	// Int32
#define WPREF_CAMERAROTATION				1020	// Int32
#define WPREF_CAMERAROTATION_CENTER	1
#define WPREF_CAMERAROTATION_OBJECT	2
#define WPREF_CAMERAROTATION_CURSOR	3
#define WPREF_CAMERAROTATION_CAMERA	4
#define WPREF_CAMERAROTATION_CUSTOM	5

#define WPREF_DOLLYTOCURSOR	1021													// Int32
#define WPREF_SYNCVIEWPORTS	1022													// BOOL 440000085 // BOOL

#define WPREF_OPENGL_PERSPECT												 1024	// BOOL
#define WPREF_OPENGL_TEXTURE_FILTERING							 1025	// Int32
#define WPREF_OPENGL_TEXTURE_FILTERING_NEAREST			 0
#define WPREF_OPENGL_TEXTURE_FILTERING_LINEAR				 1
#define WPREF_OPENGL_TEXTURE_FILTERING_LINEAR_MIPMAP 2
#define WPREF_USE_TEXTURES													 1026	// BOOL
#define WPREF_NAV_POI_MODE													 1027	// Int32
#define WPREF_NAV_POI_CENTER												 1
#define WPREF_NAV_POI_OBJECT												 2
#define WPREF_NAV_POI_CAMERA												 3

#define WPREF_NAV_CURSOR_MODE			 1028	// Int32
#define WPREF_NAV_CURSOR_OFF			 1
#define WPREF_NAV_CURSOR_SELECTION 2
#define WPREF_NAV_CURSOR_CHILDREN	 3
#define WPREF_NAV_CURSOR_ALL			 4

#define WPREF_USE_QUICKTIME										1029	// BOOL
#define WPREF_TABLET_HIRES										1030	// BOOL
#define WPREF_REVERSE_ORBIT										1031	// Bool
#define WPREF_NAV_CURSOR_DEEP									1032	// Bool
#define WPREF_NAV_LOCK_POI										1033	// Bool
#define WPREF_OPENGL_POLYLINES								1034	// Bool: use polylines
#define WPREF_OPENGL_LIGHTING									1035	// Bool: use opengl lighting
#define WPREF_OPENGL_GLPOINTS									1037	// Bool: allow real gl points
#define WPREF_OPENGL_HIGHENDSHADING						1038	// Bool
#define WPREF_NAV_VIEW_TRANSITION							1039	// Bool
#define WPREF_REALTIMEMANAGER									1042	// BOOL
#define WPREF_MAX_BACKUP											1043	// Int32
#define WPREF_CENTERAXIS											1044	// BOOL
#define WPREF_OPENGL_DUALPLANES_ARB						1047	// Bool: use dualplane ARB extension
#define WPREF_MATPREVIEW_DEFAULTSIZE					1048	// Int32
#define WPREF_DESCRIPTIONLIMIT								1049	// Int32
#define WPREF_MATPREVIEW_DEFAULTOBJECT_MAT		1050	// Int32
#define WPREF_MATPREVIEW_DEFAULTUSERSCENE_MAT	1051	// String
#define WPREF_MATPREVIEW_DEFAULTOBJECT_SHD		1052	// Int32
#define WPREF_MATPREVIEW_DEFAULTUSERSCENE_SHD	1053	// String
#define WPREF_MATPREVIEW_AUTO_UPDATE					1054	// Bool
#define WPREF_MATPREVIEW_REDRAW_TIME					1055	// Int32: max. redraw time in ms
#define WPREF_MATPREVIEW_FPS									1056	// Int32
#define WPREF_MATPREVIEW_LENGTH								1057	// Float
#define WPREF_MATPREVIEW_MAX_MEM							1058	// Int32
#define WPREF_SUBPIXELLIMIT										1062	// Int32
#define WPREF_OPENGL_ROTATEQUADS							1064	// BOOL: rotate quads 90 degree to get another subdivision
#define WPREF_OPENGL_DUALPLANES_HIGHLIGHT			1066	// Bool: allow dualplanes in glmode (gl extension)
#define WPREF_ALLOWBUGREPORTS									1068	// Bool
//#define WPREF_OPENGL_HIGHQUALITY							1069 // BOOL
#define WPREF_OPENGL_COMPILER					1070					// Int32
#define WPREF_OPENGL_COMPILER_GLSL		0
#define WPREF_OPENGL_COMPILER_CG			1
#define WPREF_OPENGL_MAX_TRANS_DEPTH	1071	// Int32
#define WPREF_OPENGL_MAX_LIGHTS				1072	// Int32
#define WPREF_OPENGL_MAX_SHADOWS			1073	// Int32
#define WPREF_SAVE_CACHES							1074	// BOOL
#define WPREF_SAVE_CACHES_ANIM				1075	// BOOL
#define WPREF_OPENGL_ANTIALIAS				1084	// Int32
#define WPREF_OPENGL_MULTITHREADED		1085	// Bool
#define WPREF_OPENGL_USE_SHADER_CACHE	1089	// Bool
#define WPREF_VIEW_DISLAYCOLORPROFILE	1086	// ColorProfile CustomDataType

#define WPREF_MOVEACCELERATION	 1081				// REAL
#define WPREF_SCALEACCELERATION	 1082				// REAL
#define WPREF_ROTATEACCELERATION 1083				// REAL

#define WPREF_GLOBAL_SCRIPTMODE	1090				// Int32 (SCRIPTMODE)

#define WPREF_COLOR_SYSTEM_C4D		1100			// Int32
#define WPREF_COLOR_SYSTEM_BP			1101			// Int32
#define COLORSYSTEM_HSVTAB				22
#define COLORSYSTEM_TABLE					30
#define COLORSYSTEM_RGB_COLOR			11
#define COLORSYSTEM_HSV_COLOR			21
#define WPREF_COLOR_RGBRANGE			1102	// Int32
#define WPREF_COLOR_HRANGE				1103	// Int32
#define WPREF_COLOR_SVRANGE				1104	// Int32
#define COLORSYSTEM_RANGE_PERCENT	0
#define COLORSYSTEM_RANGE_DEGREE	3
#define COLORSYSTEM_RANGE_255			1
#define COLORSYSTEM_RANGE_65535		2
#define WPREF_COLOR_QUICK_C4D			1105
#define WPREF_COLOR_QUICK_BP			1106
#define WPREF_COLOR_MIX_C4D				1107
#define WPREF_COLOR_MIX_BP				1108
#define WPREF_COLOR_SETUPS				1109				// BaseContainer

#define WPREF_COLOR_QUICKPRESET					1200	// Vector
#define WPREF_COLOR_QUICKPRESET_LAST		1299	// Vector
#define WPREF_COLOR_MIXING_1						1301	// Vector
#define WPREF_COLOR_MIXING_2						1302	// Vector
#define WPREF_COLOR_MIXING_3						1303	// Vector
#define WPREF_COLOR_MIXING_4						1304	// Vector
#define WPREF_COLOR_SYSTEM_COLORCHOOSER	1305	// BOOL

#define WPREF_AUTOSAVE_ENABLE					 1400		// Bool
#define WPREF_AUTOSAVE_MIN						 1401		// Int32
#define WPREF_AUTOSAVE_LIMIT_TO				 1402		// Bool
#define WPREF_AUTOSAVE_LIMIT_NUM			 1403		// Int32
#define WPREF_AUTOSAVE_DEST						 1405		// Int32
#define WPREF_AUTOSAVE_DEST_BACKUPDIR	 0
#define WPREF_AUTOSAVE_DEST_USERDIR		 1
#define WPREF_AUTOSAVE_DEST_STARTUPDIR 2
#define WPREF_AUTOSAVE_DEST_PATH			 1406	// Filename
#define WPREF_COMMANDER_AT_CURSOR			 1407	// Bool

#define WPREF_PLUGINS				 30006					// Container
#define WPREF_CPUCOUNT			 30010					// Int32
#define WPREF_LOGFILE				 30011					// BOOL
#define WPREF_CONSOLEGI			 30013					// BOOL
#define WPREF_CPUCUSTOM			 30014					// BOOL
#define WPREF_RENDERQUEUELOG 30015					// BOOL

#define WPREF_BUBBLEHELP2								 21002
#define WPREF_THREADPRIORITY						 21003
#define WPREF_MENUICON									 21004
#define WPREF_MENUSHORTCUT							 21005
#define WPREF_INACTIVEBORDER						 21006
#define WPREF_ACTIVEBORDER							 21007
#define WPREF_FONT_STANDARD							 21050
#define WPREF_FONT_MONO									 21051
#define WPREF_MAC_CTRLCLICK_EMULATES_RMB 21062
#define WPREF_MAC_MENUBAR								 21063
#define WPREF_UV_RELAX_DATA							 21065						// BaseContainer
#define WPREF_UV_OPTIMAL_MAPPING_DATA		 21066						// BaseContainer
#define WPREF_UV_PROJECTION_DATA				 21067						// BaseContainer
#define WPREF_UV_TAB_SEL								 21068						// BaseContainer
#define WPREF_UV_TRANSFORM_DATA					 21069						// BaseContainer
#define WPREF_LINUX_BROWSERPATH					 21070						// Filename
#define	WPREF_MOUSEOVER_SHORTCUT				 21072						// Bool
#define WPREF_ONLINEHELP_PATH						 21075						// path
#define WPREF_ONLINEHELP_URL						 21076
#define WPREF_LINUX_IMAGEEDITPATH				 21077						// Filename
#define WPREF_LOCKINTERFACE							 21078						// Bool
#define WPREF_TOOLCURSOR_BASIC					 21079						// Bool
#define WPREF_TOOLCURSOR_ADV						 21080						// Bool

#define WPREF_ONLINEUPDATER_AUTO_CHECK						 40000	// bool
#define WPREF_ONLINEUPDATER_CHECKSUM							 40001	// private
#define WPREF_ONLINEUPDATER_SHOW_INSTALLED				 40003	// bool
#define WPREF_ONLINEUPDATER_AUTORESTART						 40004	// bool
#define WPREF_ONLINEUPDATER_BACKUP								 40005	// bool
#define WPREF_ONLINEUPDATER_BACKUP_PATH						 40014	// Filename
#define WPREF_ONLINEUPDATER_PROXYSERVER						 40008	// String
#define WPREF_ONLINEUPDATER_PROXYPORT							 40009	// Int32
#define WPREF_ONLINEUPDATER_LAST_OPEN_DAY					 40011	// Int32
#define WPREF_ONLINEUPDATER_LAST_OPEN_HOUR				 40012	// Float
#define WPREF_ONLINEUPDATER_REMOVE_FILES					 40013	// bool
#define WPREF_ONLINEUPDATER_SHOW_DEVELOPER_UPDATES 40015	// bool

#define	WPREF_PV_RENDER_VIEW 430000690										// Int32 - the index id of the PictureViewer dialog that receives render output
#define	WPREF_PV_RECENT			 465001804										//for recent files in PV

#define		PVPREFSDIALOG_ID		 465001700
#define		SCULPTPREFSDIALOG_ID 1027830

enum
{
	WPREFS_PVMEMORY = 1000,
	WPREFS_PVDRAWBUCKETS,
	WPREFS_PVHDMEM,
	WPREFS_PVHDFOLDER,
	WPREFS_PVHDUNLIMIT,
};

enum
{
	WPREFS_SCULPTMEMORY = 1000,
};

enum
{
	WPREF_NET_NAME = 1000,
	WPREF_NET_SHARE,
	WPREF_NET_THREADCUSTOM,
	WPREF_NET_RENDERTHREADS,
	WPREF_NET_SECURITYTOKEN,
	WPREF_NET_ALLOWRESTARTOFC4D,
	WPREF_NET_SERVER_PORTNUMBER,
	WPREF_NET_SERVER_REPOSITORYPATH,
	WPREF_NET_USEBONJOUR,
	WPREF_NET_USEENCRYPTION,
	WPREF_NET_HANDLEWARNINGASERROR,
	WPREF_NET_ABORTRENDERINGONCLIENTERROR,
	WPREF_NET_PEERTOPEERASSETDISTRIBUTION,
	WPREF_NET_REQUESTONDEMAND,
	WPREF_NET_EXCLUDECLIENTONRENDERINGERROR,
	WPREF_NET_RENDERINGTIMEOUT,
	WPREF_NET_ENABLETEAMRENDER,
	EX_WPREF_NET_WEBSERVERPORT,

	// stored in prefs but not visible
	WPREF_NET_SHOWBUCKETMACHINECOLOR,
	WPREF_NET_SHOWNAME,
	WPREF_NET_SHOWICON,
	WPREF_NET_SHOWINFO,
	WPREF_NET_SHOWCHECKBOX,
	WPREF_NET_SHOWADDRESS,
	WPREF_NET_MACHINEICONSIZE,
	// ------------------------------

	WPREF_NET_ENABLERENDERINGTIMEOUT
};

// mouse cursors
#define MOUSE_HIDE							0
#define MOUSE_SHOW							1
#define MOUSE_NORMAL						2
#define MOUSE_BUSY							3
#define MOUSE_CROSS							4
#define MOUSE_QUESTION					5
#define MOUSE_ZOOM_IN						6
#define MOUSE_ZOOM_OUT					7
#define MOUSE_FORBIDDEN					8
#define MOUSE_DELETE						9
#define MOUSE_COPY							10
#define MOUSE_INSERTCOPY				11
#define MOUSE_INSERTCOPYDOWN		12
#define MOUSE_MOVE							13
#define MOUSE_INSERTMOVE				14
#define MOUSE_INSERTMOVEDOWN		15
#define MOUSE_ARROW_H						16
#define MOUSE_ARROW_V						17
#define MOUSE_ARROW_HV					18
#define MOUSE_POINT_HAND				19
#define MOUSE_MOVE_HAND					20
#define MOUSE_IBEAM							21
#define MOUSE_SELECT_LIVE				22
#define MOUSE_SELECT_FREE				23
#define MOUSE_SELECT_RECT				24
#define MOUSE_SELECT_POLY				25
#define MOUSE_SPLINETOOLS				26
#define MOUSE_EXTRUDE						27
#define MOUSE_NORMALMOVE				28
#define MOUSE_ADDPOINTS					29
#define MOUSE_ADDPOLYGONS				30
#define MOUSE_BRIDGE						31
#define MOUSE_MIRROR						32
#define MOUSE_PAINTMOVE					33
#define MOUSE_PAINTSELECTRECT		34
#define MOUSE_PAINTSELECTCIRCLE	35
#define MOUSE_PAINTSELECTPOLY		36
#define MOUSE_PAINTSELECTFREE		37
#define MOUSE_PAINTMAGICWAND		38
#define MOUSE_PAINTCOLORRANGE		39
#define MOUSE_PAINTFILL					40
#define MOUSE_PAINTPICK					41
#define MOUSE_PAINTBRUSH				42
#define MOUSE_PAINTCLONE				43
#define MOUSE_PAINTTEXT					44
#define MOUSE_PAINTCROP					45
#define MOUSE_PAINTLINE					46
#define MOUSE_PAINTPOLYSHAPE		47

// global events
#define EVMSG_CHANGE							 604
#define EVMSG_DOCUMENTRECALCULATED 'drcl'	// view has been animated, expression are executed, some manager data may have changed
#define	EVMSG_TOOLCHANGED					 Int32(0xfff36465)
#define	EVMSG_GRAPHVIEWCHANGED		 400008000
#define EVMSG_AUTKEYMODECHANGED		 200000009
#define	EVMSG_UPDATEHIGHLIGHT			 200000073
#define EVMSG_CHANGEDSCRIPTMODE		 1026569

#define	EVMSG_SHOWIN_SB	 -200000074
#define	EVMSG_SHOWIN_TL	 -200000075
#define	EVMSG_SHOWIN_FC	 -200000076
#define	EVMSG_SHOWIN_LM	 -200000077
#define	EVMSG_TLOM_MERGE -465001000

#define	EVMSG_SHOWIN_MT -200000078

#define EVMSG_TIMELINESELECTION	-1001
#define EVMSG_BROWSERCHANGE			-1002
#define EVMSG_MATERIALSELECTION	-1009
#define EVMSG_FCURVECHANGE			-1010

#define EVMSG_RAYTRACER_FINISHED		-1003
#define EVMSG_MATERIALPREVIEW				-1008
#define EVMSG_ACTIVEVIEWCHANGED			'acvw'
#define EVMSG_ASYNCEDITORMOVE				'edmv'
#define MOVE_START									0
#define MOVE_CONTINUE								1
#define MOVE_END										2	// -> par2 == ESC
#define	EVMSG_TIMECHANGED						'tchg'
#define EVMSG_VIEWWINDOW_OUTPUT			-1011
#define EVMSG_VIEWWINDOW_3DPAINTUPD	-1012
#define EVMSG_UPDATESCHEME					200000010
#define SCHEME_LIGHT								0
#define SCHEME_DARK									1
#define SCHEME_OTHER								2

enum EVENT
{
	EVENT_0								 = 0,
	EVENT_FORCEREDRAW			 = (1 << 0),
	EVENT_ANIMATE					 = (1 << 1),
	EVENT_NOEXPRESSION		 = (1 << 2),
	EVENT_GLHACK					 = (1 << 3),
	EVENT_CAMERAEXPRESSION = (1 << 4)
} ENUM_END_FLAGS(EVENT);

// draw flags
enum DRAWFLAGS
{
	DRAWFLAGS_0														= 0,
	DRAWFLAGS_NO_THREAD										= (1 << 1),
	DRAWFLAGS_NO_REDUCTION								= (1 << 2),
	DRAWFLAGS_NO_ANIMATION								= (1 << 8),
	DRAWFLAGS_ONLY_ACTIVE_VIEW						= (1 << 10),
	DRAWFLAGS_NO_EXPRESSIONS							= (1 << 12),
	DRAWFLAGS_INDRAG											= (1 << 13),
	DRAWFLAGS_NO_HIGHLIGHT_PLANE					= (1 << 14),
	DRAWFLAGS_FORCEFULLREDRAW							= (1 << 15),
	DRAWFLAGS_ONLY_CAMERAEXPRESSION				= (1 << 16),
	DRAWFLAGS_INMOVE											= (1 << 17),
	DRAWFLAGS_ONLY_BASEDRAW								= (1 << 22),	// draw specific BaseDraw only

	DRAWFLAGS_ONLY_HIGHLIGHT							= (1 << 18),
	DRAWFLAGS_STATICBREAK									= (1 << 19),	// only use in combination with DRAWFLAGS_NO_THREAD

	DRAWFLAGS_PRIVATE_NO_WAIT_GL_FINISHED	= (1 << 3),
	DRAWFLAGS_PRIVATE_ONLYBACKGROUND			= (1 << 4),
	DRAWFLAGS_PRIVATE_NOBLIT							= (1 << 9),
	DRAWFLAGS_PRIVATE_OPENGLHACK					= (1 << 11),
	DRAWFLAGS_PRIVATE_ONLY_PREPARE				= (1 << 21),
	DRAWFLAGS_PRIVATE_NO_3DCLIPPING				= (1 << 24)
} ENUM_END_FLAGS(DRAWFLAGS);

// animate scene/object flags
enum ANIMATEFLAGS
{
	ANIMATEFLAGS_0						= 0,
	ANIMATEFLAGS_NO_PARTICLES	= (1 << 2),
	ANIMATEFLAGS_NO_CHILDREN	= (1 << 6),
	ANIMATEFLAGS_INRENDER			= (1 << 7),
	ANIMATEFLAGS_NO_MINMAX		= (1 << 8),	// private
	ANIMATEFLAGS_NO_NLA				= (1 << 9),	// private
	ANIMATEFLAGS_NLA_SUM			= (1 << 10)	// private
} ENUM_END_FLAGS(ANIMATEFLAGS);

enum SAVEDOCUMENTFLAGS
{
	SAVEDOCUMENTFLAGS_0										= 0,
	SAVEDOCUMENTFLAGS_DIALOGSALLOWED			= (1 << 0),
	SAVEDOCUMENTFLAGS_SAVEAS							= (1 << 1),
	SAVEDOCUMENTFLAGS_DONTADDTORECENTLIST	= (1 << 2),
	SAVEDOCUMENTFLAGS_AUTOSAVE						= (1 << 3),
	SAVEDOCUMENTFLAGS_SAVECACHES					= (1 << 4),
	SAVEDOCUMENTFLAGS_EXPORTDIALOG				= (1 << 5),
	SAVEDOCUMENTFLAGS_CRASHSITUATION			= (1 << 6),
	SAVEDOCUMENTFLAGS_NO_SHADERCACHE			= (1 << 7)
} ENUM_END_FLAGS(SAVEDOCUMENTFLAGS);

enum COPYFLAGS
{
	COPYFLAGS_0																 = 0,
	COPYFLAGS_NO_HIERARCHY										 = (1 << 2),
	COPYFLAGS_NO_ANIMATION										 = (1 << 3),
	COPYFLAGS_NO_BITS													 = (1 << 4),
	COPYFLAGS_NO_MATERIALPREVIEW							 = (1 << 5),
	COPYFLAGS_NO_BRANCHES											 = (1 << 7),
	COPYFLAGS_DOCUMENT												 = (1 << 10),	// this flag is read-only, set when a complete document is copied
	COPYFLAGS_NO_NGONS												 = (1 << 11),
	COPYFLAGS_CACHE_BUILD											 = (1 << 13),	// this flags is read-only, set when a cache is built
	COPYFLAGS_RECURSIONCHECK									 = (1 << 14),

	COPYFLAGS_PRIVATE_IDENTMARKER							 = (1 << 0),	// private
	COPYFLAGS_PRIVATE_NO_INTERNALS						 = (1 << 8),	// private
	COPYFLAGS_PRIVATE_NO_PLUGINLAYER					 = (1 << 9),	// private
	COPYFLAGS_PRIVATE_UNDO										 = (1 << 12),	// private
	COPYFLAGS_PRIVATE_CONTAINER_COPY_DIRTY		 = (1 << 15),	// private
	COPYFLAGS_PRIVATE_CONTAINER_COPY_IDENTICAL = (1 << 16),	// private
	COPYFLAGS_PRIVATE_NO_TAGS									 = (1 << 17),	// private
	COPYFLAGS_PRIVATE_DELETE									 = (1 << 18),	// private
	COPYFLAGS_PRIVATE_BODYPAINT_NODATA				 = (1 << 29),	// private
	COPYFLAGS_PRIVATE_BODYPAINT_CONVERTLAYER	 = (1 << 30)	// private
} ENUM_END_FLAGS(COPYFLAGS);

enum UNDOTYPE
{
	UNDOTYPE_0													= 0,

	UNDOTYPE_CHANGE											= 40,	// complete with children
	UNDOTYPE_CHANGE_NOCHILDREN					= 41,	// complete without children
	UNDOTYPE_CHANGE_SMALL								= 42,	// object itself without branches
	UNDOTYPE_CHANGE_SELECTION						= 43,	// modeling point/polygon/edge selection

	UNDOTYPE_NEW												= 44,	// new object, call InitUndo after object is inserted
	UNDOTYPE_DELETE											= 45,	// delete object, call InitUndo before object is deleted

	UNDOTYPE_ACTIVATE										= 46,
	UNDOTYPE_DEACTIVATE									= 47,

	UNDOTYPE_BITS												= 48,
	UNDOTYPE_HIERARCHY_PSR							= 49,	// hierarchical placement and PSR values

	UNDOTYPE_PRIVATE_STRING							= 9996,
	UNDOTYPE_PRIVATE_MULTISELECTIONAXIS	= 9997,
	UNDOTYPE_START											= 9998,	// private
	UNDOTYPE_END												= 9999	// private
} ENUM_END_LIST(UNDOTYPE);

// handle types
enum DRAWHANDLE
{
	DRAWHANDLE_MINI					= 0,
	DRAWHANDLE_SMALL				= 1,
	DRAWHANDLE_MIDDLE				= 2,
	DRAWHANDLE_BIG					= 3,
	DRAWHANDLE_CUSTOM				= 4,
	DRAWHANDLE_POINTSIZE		= 5,
	DRAWHANDLE_SELPOINTSIZE	= 6
} ENUM_END_LIST(DRAWHANDLE);

enum DRAW_ALPHA
{
	DRAW_ALPHA_NONE							 = 10,
	DRAW_ALPHA_INVERTED					 = 11,
	DRAW_ALPHA_NORMAL						 = 12,	// generates alpha channel from the image's alpha channel. If no alpha channel exists, the alpha value is set to 100%
	DRAW_ALPHA_FROM_IMAGE				 = 13,	// generates the alpha channel from the RGB image information
	DRAW_ALPHA_NORMAL_FROM_IMAGE = 14		// generates alpha channel from the image's alpha channel. If no alpha channel exists, the alpha value is generated from the RGB imaged
} ENUM_END_LIST(DRAW_ALPHA);

enum DRAW_TEXTUREFLAGS
{
	DRAW_TEXTUREFLAGS_0	= 0x0,

	// flags for DrawTexture and SetTexture
	DRAW_TEXTUREFLAGS_COLOR_IMAGE_TO_LINEAR	= 0x00000001,	// convert from embedded profile to linear
	DRAW_TEXTUREFLAGS_COLOR_SRGB_TO_LINEAR	= 0x00000002,	// convert from SRGB to linear
	DRAW_TEXTUREFLAGS_COLOR_IMAGE_TO_SRGB		= 0x00000003,	// convert from embedded profile to SRGB
	DRAW_TEXTUREFLAGS_COLOR_LINEAR_TO_SRGB	= 0x00000004,	// convert from linear to SRGB
	DRAW_TEXTUREFLAGS_COLOR_CORRECTION_MASK	= 0x0000000f,	// color correction mask

	DRAW_TEXTUREFLAGS_USE_PROFILE_COLOR			= 0x00000010,
	DRAW_TEXTUREFLAGS_ALLOW_FLOATINGPOINT		= 0x00000020,	//  allow floating point textures (if supported)

	// interpolation flags
	DRAW_TEXTUREFLAGS_INTERPOLATION_NEAREST				= 0x00100000,
	DRAW_TEXTUREFLAGS_INTERPOLATION_LINEAR				= 0x00200000,
	DRAW_TEXTUREFLAGS_INTERPOLATION_LINEAR_MIPMAP	= 0x00400000,
	DRAW_TEXTUREFLAGS_INTERPOLATION_MASK					= 0x00f00000

} ENUM_END_FLAGS(DRAW_TEXTUREFLAGS);

enum TOOLDRAW
{
	TOOLDRAW_0					= 0,
	TOOLDRAW_HANDLES		= (1 << 0),
	TOOLDRAW_AXIS				= (1 << 1),
	TOOLDRAW_HIGHLIGHTS	= (1 << 2)
} ENUM_END_FLAGS(TOOLDRAW);

enum TOOLDRAWFLAGS
{
	TOOLDRAWFLAGS_0					= 0,
	TOOLDRAWFLAGS_INVERSE_Z	= (1 << 0),
	TOOLDRAWFLAGS_HIGHLIGHT	= (1 << 1)
} ENUM_END_FLAGS(TOOLDRAWFLAGS);

// viewport colors
#define VIEWCOLOR_C4DBACKGROUND				0
#define VIEWCOLOR_FILMFORMAT					1
#define VIEWCOLOR_HORIZON							2
#define VIEWCOLOR_GRID_MAJOR					3
#define VIEWCOLOR_GRID_MINOR					4
#define VIEWCOLOR_SPLINESTART					5
#define VIEWCOLOR_SPLINEEND						6
#define VIEWCOLOR_CAMERA							7
#define VIEWCOLOR_PARTICLE						8
#define VIEWCOLOR_PMODIFIER						9
#define DELME_VIEWCOLOR_BONE					10
#define VIEWCOLOR_MODIFIER						11
#define VIEWCOLOR_ACTIVEPOINT					12
#define VIEWCOLOR_INACTIVEPOINT				13
#define VIEWCOLOR_TANGENT							14
#define VIEWCOLOR_ACTIVEPOLYGON				15
#define VIEWCOLOR_INACTIVEPOLYGON			16
#define VIEWCOLOR_TEXTURE							17
#define VIEWCOLOR_TEXTUREAXIS					18
#define VIEWCOLOR_ACTIVEBOX						19
#define VIEWCOLOR_ANIMPATH						20
#define VIEWCOLOR_XAXIS								21
#define VIEWCOLOR_YAXIS								22
#define VIEWCOLOR_ZAXIS								23
#define VIEWCOLOR_WXAXIS							24
#define VIEWCOLOR_WYAXIS							25
#define VIEWCOLOR_WZAXIS							26
#define VIEWCOLOR_SELECT_AXIS					27
#define VIEWCOLOR_LAYER0							28
#define VIEWCOLOR_LAYER1							29
#define VIEWCOLOR_LAYER2							30
#define VIEWCOLOR_LAYER3							31
#define VIEWCOLOR_LAYER4							32
#define VIEWCOLOR_LAYER5							33
#define VIEWCOLOR_LAYER6							34
#define VIEWCOLOR_LAYER7							35
#define VIEWCOLOR_VERTEXSTART					36
#define VIEWCOLOR_VERTEXEND						37
#define VIEWCOLOR_UVMESH_GREYED				38
#define VIEWCOLOR_UVMESH_APOLY				39
#define VIEWCOLOR_UVMESH_IAPOLY				40
#define VIEWCOLOR_UVMESH_APOINT				41
#define VIEWCOLOR_UVMESH_IAPOINT			42
#define VIEWCOLOR_NORMAL							43
#define VIEWCOLOR_ACTIVECHILDBOX			44
#define VIEWCOLOR_ACTIVEPOLYBOX				45
#define VIEWCOLOR_ACTIVEPOLYCHILDBOX	46
#define VIEWCOLOR_SELECTION_PREVIEW		47
#define VIEWCOLOR_MEASURETOOL					48
//#define VIEWCOLOR_AXIS_BAND						49
#define VIEWCOLOR_SHADEDWIRE					50
#define VIEWCOLOR_NGONLINE						51
#define VIEWCOLOR_FRONTFACING					52
#define VIEWCOLOR_BACKFACING					53
#define VIEWCOLOR_MINSOFTSELECT				54
#define VIEWCOLOR_MAXSOFTSELECT				55
#define VIEWCOLOR_MINHNWEIGHT					56
#define VIEWCOLOR_ZEROHNWEIGHT				57
#define VIEWCOLOR_MAXHNWEIGHT					58
#define VIEWCOLOR_IRR									59
#define VIEWCOLOR_OBJECTHIGHLIGHT			60
#define VIEWCOLOR_OBJECTSELECT				61
#define VIEWCOLOR_C4DBACKGROUND_GRAD1	62
#define VIEWCOLOR_C4DBACKGROUND_GRAD2	63
#define VIEWCOLOR_BRUSHPREVIEW				64
#define VIEWCOLOR_SPLINEHULL					65
#define VIEWCOLOR_TOOLHANDLE					66
#define VIEWCOLOR_ACTIVETOOLHANDLE		67

#define VIEWCOLOR_MAXCOLORS	68

enum DIRTYFLAGS
{
	DIRTYFLAGS_0					 = 0,
	DIRTYFLAGS_MATRIX			 = (1 << 1),	// object matrix changed
	DIRTYFLAGS_DATA				 = (1 << 2),	// object internal data changed
	DIRTYFLAGS_SELECT			 = (1 << 3),	// object selections changed
	DIRTYFLAGS_CACHE			 = (1 << 4),	// object caches changed
	DIRTYFLAGS_CHILDREN		 = (1 << 5),
	DIRTYFLAGS_DESCRIPTION = (1 << 6),	// description changed

	// basedocument
	DIRTYFLAGS_SELECTION_OBJECTS	 = (1 << 20),
	DIRTYFLAGS_SELECTION_TAGS			 = (1 << 21),
	DIRTYFLAGS_SELECTION_MATERIALS = (1 << 22),

	DIRTYFLAGS_ALL								 = -1
} ENUM_END_FLAGS(DIRTYFLAGS);

enum HDIRTY_ID
{
	HDIRTY_ID_ANIMATION				 = 0,
	HDIRTY_ID_OBJECT					 = 1,
	HDIRTY_ID_OBJECT_MATRIX		 = 2,
	HDIRTY_ID_OBJECT_HIERARCHY = 3,
	HDIRTY_ID_TAG							 = 4,
	HDIRTY_ID_MATERIAL				 = 5,
	HDIRTY_ID_SHADER					 = 6,
	HDIRTY_ID_RENDERSETTINGS	 = 7,
	HDIRTY_ID_VP							 = 8,
	HDIRTY_ID_FILTER					 = 9,
	HDIRTY_ID_NBITS						 = 10,
	HDIRTY_ID_MAX
} ENUM_END_LIST(HDIRTY_ID);

enum HDIRTYFLAGS
{
	HDIRTYFLAGS_0					= 0,
	HDIRTYFLAGS_ANIMATION			= (1 << 0),
	HDIRTYFLAGS_OBJECT				= (1 << 1),
	HDIRTYFLAGS_OBJECT_MATRIX		= (1 << 2),
	HDIRTYFLAGS_OBJECT_HIERARCHY	= (1 << 3),
	HDIRTYFLAGS_TAG = (1 << 4),
	HDIRTYFLAGS_MATERIAL = (1 << 5),
	HDIRTYFLAGS_SHADER = (1 << 6),
	HDIRTYFLAGS_RENDERSETTINGS = (1 << 7),
	HDIRTYFLAGS_VP = (1 << 8),
	HDIRTYFLAGS_FILTER = (1 << 9),
	HDIRTYFLAGS_NBITS = (1 << 10),

	HDIRTYFLAGS_ALL							 = -1
} ENUM_END_FLAGS(HDIRTYFLAGS);

enum ROTATIONORDER
{
	ROTATIONORDER_YXZGLOBAL	=	0,
	ROTATIONORDER_YZXGLOBAL	=	1,
	ROTATIONORDER_ZYXGLOBAL	=	2,
	ROTATIONORDER_ZXYGLOBAL	=	3,
	ROTATIONORDER_XZYGLOBAL	=	4,
	ROTATIONORDER_XYZGLOBAL	=	5,

	ROTATIONORDER_YXZLOCAL	= 3,
	ROTATIONORDER_YZXLOCAL	= 4,
	ROTATIONORDER_ZYXLOCAL	= 5,
	ROTATIONORDER_ZXYLOCAL	= 0,
	ROTATIONORDER_XZYLOCAL	= 1,
	ROTATIONORDER_XYZLOCAL	= 2,

	ROTATIONORDER_HPB				= 6,
	ROTATIONORDER_DEFAULT		= 6	// HPB is default
} ENUM_END_LIST(ROTATIONORDER);

enum BUILDFLAGS
{
	BUILDFLAGS_0								= 0,
	BUILDFLAGS_INTERNALRENDERER	= (1 << 1),
	BUILDFLAGS_EXTERNALRENDERER	= (1 << 2),
	BUILDFLAGS_ISOPARM					= (1 << 3)
} ENUM_END_FLAGS(BUILDFLAGS);

enum EXECUTIONFLAGS
{
	EXECUTIONFLAGS_0						 = 0,
	EXECUTIONFLAGS_ANIMATION		 = (1 << 1),
	EXECUTIONFLAGS_EXPRESSION		 = (1 << 2),
	EXECUTIONFLAGS_CACHEBUILDING = (1 << 3),
	EXECUTIONFLAGS_CAMERAONLY		 = (1 << 4),
	EXECUTIONFLAGS_INDRAG				 = (1 << 5),
	EXECUTIONFLAGS_INMOVE				 = (1 << 6),
	EXECUTIONFLAGS_RENDER				 = (1 << 7)
} ENUM_END_FLAGS(EXECUTIONFLAGS);

enum SCENEHOOKDRAW
{
	SCENEHOOKDRAW_0													 = 0,
	SCENEHOOKDRAW_DRAW_PASS									 = (1 << 0),
	SCENEHOOKDRAW_HIGHLIGHT_PASS_BEFORE_TOOL = (1 << 1),
	SCENEHOOKDRAW_HIGHLIGHT_PASS						 = (1 << 2),
	SCENEHOOKDRAW_HIGHLIGHT_PASS_INV				 = (1 << 3),
	SCENEHOOKDRAW_DRAW_PASS_AFTER_CLEAR			 = (1 << 4)
} ENUM_END_FLAGS(SCENEHOOKDRAW);

// flags for GetDescription
enum DESCFLAGS_DESC
{
	DESCFLAGS_DESC_0									 = 0,
	DESCFLAGS_DESC_RESOLVEMULTIPLEDATA = (1 << 0),
	DESCFLAGS_DESC_LOADED							 = (1 << 1),
	DESCFLAGS_DESC_RECURSIONLOCK			 = (1 << 2),
	DESCFLAGS_DESC_DONTLOADDEFAULT		 = (1 << 3),	// internal: used for old plugintools
	DESCFLAGS_DESC_MAPTAGS						 = (1 << 4),
	DESCFLAGS_DESC_NEEDDEFAULTVALUE		 = (1 << 5)		// DESC_DEFAULT needed
} ENUM_END_FLAGS(DESCFLAGS_DESC);

// flags for GetDParameter/SetDParameter
enum DESCFLAGS_GET
{
	DESCFLAGS_GET_0											= 0,
	DESCFLAGS_GET_PARAM_GET							= (1 << 1),
	DESCFLAGS_GET_NO_GLOBALDATA					= (1 << 4),
	DESCFLAGS_GET_NO_GEDATADEFAULTVALUE	= (1 << 5)
} ENUM_END_FLAGS(DESCFLAGS_GET);

enum DESCFLAGS_SET
{
	DESCFLAGS_SET_0											= 0,
	DESCFLAGS_SET_PARAM_SET							= (1 << 1),
	DESCFLAGS_SET_USERINTERACTION				= (1 << 2),
	DESCFLAGS_SET_DONTCHECKMINMAX				= (1 << 3),
	DESCFLAGS_SET_DONTAFFECTINHERITANCE	= (1 << 6),	// for render settings and post effects only (SetParameter)
	DESCFLAGS_SET_FORCESET							= (1 << 7)	// SetParameter: force the set value without GetParameter/Compare, use only for calls where you for sure changed the value!
} ENUM_END_FLAGS(DESCFLAGS_SET);

enum DESCFLAGS_ENABLE
{
	DESCFLAGS_ENABLE_0 = 0
} ENUM_END_FLAGS(DESCFLAGS_ENABLE);

enum HIERARCHYCLONEFLAGS
{
	HIERARCHYCLONEFLAGS_0				 = 0,
	HIERARCHYCLONEFLAGS_ASIS		 = (1 << 0),
	HIERARCHYCLONEFLAGS_ASPOLY	 = (1 << 1),
	HIERARCHYCLONEFLAGS_ASLINE	 = (1 << 2),
	HIERARCHYCLONEFLAGS_ASSPLINE = (1 << 3)
} ENUM_END_FLAGS(HIERARCHYCLONEFLAGS);

// error string dialog
enum CHECKVALUEFORMAT
{
	CHECKVALUEFORMAT_NOTHING = 0,
	CHECKVALUEFORMAT_DEGREE	 = 1,
	CHECKVALUEFORMAT_PERCENT = 2,
	CHECKVALUEFORMAT_METER	 = 3,
	CHECKVALUEFORMAT_INT		 = 5
} ENUM_END_LIST(CHECKVALUEFORMAT);

enum CHECKVALUERANGE
{
	CHECKVALUERANGE_GREATER					= 0,
	CHECKVALUERANGE_GREATEROREQUAL	= 1,
	CHECKVALUERANGE_LESS						= 2,
	CHECKVALUERANGE_LESSOREQUAL			= 3,
	CHECKVALUERANGE_BETWEEN					= 4,
	CHECKVALUERANGE_BETWEENOREQUAL	= 5,
	CHECKVALUERANGE_BETWEENOREQUALX	= 6,
	CHECKVALUERANGE_BETWEENOREQUALY	= 7,
	CHECKVALUERANGE_DIFFERENT				= 8
} ENUM_END_LIST(CHECKVALUERANGE);

// paintmesh bits
enum PAINTMESHFLAGS
{
	PAINTMESHFLAGS_0				= 0,

	PAINTMESHFLAGS_QUAD			= (1 << 1),		// polygon is quadrangle
	PAINTMESHFLAGS_SEL			= (1 << 6),		// polygon selected

	PAINTMESHFLAGS_SELA			= (1 << 2),		// point a selected
	PAINTMESHFLAGS_SELB			= (1 << 3),		// point b selected
	PAINTMESHFLAGS_SELC			= (1 << 4),		// point c selected
	PAINTMESHFLAGS_SELD			= (1 << 5),		// point d selected

	PAINTMESHFLAGS_TA				= (1 << 7),		// temporary selection for link mode
	PAINTMESHFLAGS_TB				= (1 << 8),		// temporary selection for link mode
	PAINTMESHFLAGS_TC				= (1 << 9),		// temporary selection for link mode
	PAINTMESHFLAGS_TD				= (1 << 10),	// temporary selection for link mode

	PAINTMESHFLAGS_INACTIVE = (1 << 11),	// no draw no change possible

	PAINTMESHFLAGS_EDGEA		= (1 << 12),	// edge a is ngonline
	PAINTMESHFLAGS_EDGEB		= (1 << 13),	// edge b is ngonline
	PAINTMESHFLAGS_EDGEC		= (1 << 14),	// edge c is ngonline
	PAINTMESHFLAGS_EDGED		= (1 << 15)		// edge d is ngonline
} ENUM_END_FLAGS(PAINTMESHFLAGS);

enum GETBRANCHINFO
{
	GETBRANCHINFO_0								 = 0,
	GETBRANCHINFO_ONLYWITHCHILDREN = (1 << 1),
	GETBRANCHINFO_GELISTNODES			 = (1 << 3),
	GETBRANCHINFO_ONLYMODIFIABLE	 = (1 << 4)
} ENUM_END_FLAGS(GETBRANCHINFO);

enum BRANCHINFOFLAGS
{
	BRANCHINFOFLAGS_0							 = 0,
	BRANCHINFOFLAGS_ANIMATE				 = (1 << 0),
	BRANCHINFOFLAGS_HIDEINTIMELINE = (1 << 4),
} ENUM_END_FLAGS(BRANCHINFOFLAGS);

enum GETACTIVEOBJECTFLAGS
{
	GETACTIVEOBJECTFLAGS_0							= 0,
	GETACTIVEOBJECTFLAGS_CHILDREN				= (1 << 0),
	GETACTIVEOBJECTFLAGS_SELECTIONORDER	= (1 << 1)
} ENUM_END_FLAGS(GETACTIVEOBJECTFLAGS);

enum DRAWPASS
{
	DRAWPASS_OBJECT			= 0,
	DRAWPASS_BOX				= 1,
	DRAWPASS_HANDLES		= 2,
	DRAWPASS_HIGHLIGHTS	= 3,
	DRAWPASS_XRAY				= 4
} ENUM_END_LIST(DRAWPASS);

// im-/export formats
#define FORMAT_PREF	1000
#define FORMAT_WAV	1018
#define FORMAT_L4D	1020
#define FORMAT_P4D	1022

#define FORMAT_C4DIMPORT	 1001025
#define FORMAT_C4DEXPORT	 1001026
#define FORMAT_XMLIMPORT	 1001027
#define FORMAT_XMLEXPORT	 1001028
#define FORMAT_C4D4IMPORT	 1001029
#define FORMAT_C4D5IMPORT	 1001030
#define FORMAT_VRML1IMPORT 1001031
#define FORMAT_VRML1EXPORT 1001032
#define FORMAT_VRML2IMPORT 1001033
#define FORMAT_VRML2EXPORT 1001034
#define FORMAT_DXFIMPORT	 1001035
#define FORMAT_DXFEXPORT	 1001036
#define FORMAT_3DSIMPORT	 1001037
#define FORMAT_3DSEXPORT	 1001038
#define FORMAT_OBJIMPORT	 1001039
#define FORMAT_OBJEXPORT	 1001040
#define FORMAT_Q3DIMPORT	 1001041
#define FORMAT_Q3DEXPORT	 1001042
#define FORMAT_LWSIMPORT	 1001043
#define FORMAT_LWOIMPORT	 1001044
#define FORMAT_AIIMPORT		 1001045
#define FORMAT_DEMIMPORT	 1001046
#define FORMAT_D3DEXPORT	 1001047

#define HIGHLIGHT_TRANSPARENCY -140

#define HERMITEFAK 4.0

#define CREATE_GL_HAS_ROOT 1
#define CREATE_GL_IS_ROOT	 2

#define DELETE_GL_HAS_ROOT 1
#define DELETE_GL_IS_ROOT	 2

enum SAVEPROJECT
{
	SAVEPROJECT_0													  = 0,
	SAVEPROJECT_ASSETS											= (1 << 1),	// Pass if the assets will be taken into account
	SAVEPROJECT_SCENEFILE										= (1 << 2),	// Pass if the scene will be taken into account
	SAVEPROJECT_DIALOGSALLOWED							= (1 << 3),	// Show dialogs like error messages, a file selection for missing assets or alerts if necessary
	SAVEPROJECT_SHOWMISSINGASSETDIALOG			= (1 << 4),	// If an asset is missing show a warning dialog - flag can be set without SAVEPROJECT_DIALOGSALLOWED
	SAVEPROJECT_ADDTORECENTLIST							= (1 << 5),	// Add document to the recent list
	SAVEPROJECT_DONTCOPYFILES								= (1 << 6),	// Does the same as without this flag but doesn't copy the files to the destination - used to simulate the function
	SAVEPROJECT_PROGRESSALLOWED							= (1 << 7),	// Show the progress bar in the main window
	SAVEPROJECT_DONTTOUCHDOCUMENT						= (1 << 8),	// Document will be in the same state as before the call was made
	SAVEPROJECT_DONTFAILONMISSINGASSETS			= (1 << 9),	// If this flag is passed, the function does not fail anymore when assets are missing.
	SAVEPROJECT_ISNET												= (1 << 10), // Private - is set only if the net module is collecting assets
	SAVEPROJECT_USEDOCUMENTNAMEASFILENAME		= (1 << 11)
} ENUM_END_FLAGS(SAVEPROJECT);

enum ICONDATAFLAGS
{
	ICONDATAFLAGS_0									= 0,
	ICONDATAFLAGS_APPLYCOLORPROFILE	= (1 << 0),
	ICONDATAFLAGS_DISABLED					= (1 << 1)
}
ENUM_END_FLAGS(ICONDATAFLAGS);

// userarea flags
enum USERAREAFLAGS
{
	USERAREA_0					 = (0),
	USERAREA_TABSTOP		 = (1 << 0),
	USERAREA_HANDLEFOCUS = (1 << 1),
	USERAREA_COREMESSAGE = (1 << 2),
	USERAREA_SYNCMESSAGE = (1 << 3),
	USERAREA_DONT_MIRROR = (1 << 30)
} ENUM_END_FLAGS(USERAREAFLAGS);

#define RESOURCEIMAGE_EMPTY_TRI_RIGHT						 310002010
#define RESOURCEIMAGE_RED_TRI_RIGHT							 310002011
#define RESOURCEIMAGE_EMPTY_RED_TRI_RIGHT				 310002012
#define RESOURCEIMAGE_YELLOW_DIAMOND						 310002013
#define RESOURCEIMAGE_YELLOW_TRI_RIGHT					 310002014
#define RESOURCEIMAGE_YELLOW_TRI_LEFT						 310002015
#define RESOURCEIMAGE_EMPTY_YELLOW_DIAMOND			 310002016
#define RESOURCEIMAGE_YELLOW_CIRCLE							 310002017
#define RESOURCEIMAGE_EMPTY_YELLOW_CIRCLE				 310002018
#define RESOURCEIMAGE_EMPTY_BLUE_CIRCLE					 310002019
#define RESOURCEIMAGE_BLUE_CIRCLE								 310002020
#define RESOURCEIMAGE_EMPTY_YELLOW_CIRCLE_LEFT	 310002021
#define RESOURCEIMAGE_EMPTY_YELLOW_CIRCLE_RIGHT	 310002022
#define RESOURCEIMAGE_EMPTY_TRI_LEFT						 310002001
#define RESOURCEIMAGE_RED_TRI_LEFT							 310002002
#define RESOURCEIMAGE_EMPTY_RED_TRI_LEFT				 310002003
#define RESOURCEIMAGE_EMPTY_DIAMOND							 310002004
#define RESOURCEIMAGE_RED_DIAMOND								 310002005
#define RESOURCEIMAGE_EMPTY_RED_DIAMOND					 310002006
#define RESOURCEIMAGE_EMPTY_CIRCLE							 200000122
#define RESOURCEIMAGE_RED_CIRCLE								 300000121
#define RESOURCEIMAGE_EMPTY_RED_CIRCLE					 300000122
#define	RESOURCEIMAGE_KEYFRAME_BUTTON_UP				 440000141
#define	RESOURCEIMAGE_KEYFRAME_BUTTON_OVER			 440000142
#define	RESOURCEIMAGE_KEYFRAME_BUTTON_DOWN			 440000143
#define RESOURCEIMAGE_PIN												 9000
#define RESOURCEIMAGE_SUBGROUP									 12678
#define RESOURCEIMAGE_UNLOCKED									 12679
#define RESOURCEIMAGE_LOCKED										 -12679
#define RESOURCEIMAGE_HISTOGRAM									 12680
#define RESOURCEIMAGE_PLUS											 300000118
#define RESOURCEIMAGE_MINUS											 300000119
#define RESOURCEIMAGE_FOLDER										 300000123
#define RESOURCEIMAGE_OPENED										 300000124
#define RESOURCEIMAGE_CLOSED										 300000125
#define RESOURCEIMAGE_ARROWLEFT									 300000126
#define RESOURCEIMAGE_ARROWRIGHT								 300000127
#define RESOURCEIMAGE_ARROWUP										 300000128
#define RESOURCEIMAGE_AMDUPLICATE								 300000129
#define RESOURCEIMAGE_MOVE											 13563
#define RESOURCEIMAGE_SCALE											 13564
#define RESOURCEIMAGE_ROTATE										 13565
#define RESOURCEIMAGE_VIEWCHANGE								 13640
#define RESOURCEIMAGE_FULLSCREEN								 17301
#define RESOURCEIMAGE_CLOSERRELEASED						 12097
#define RESOURCEIMAGE_CLOSERPRESSED							 -12097
#define RESOURCEIMAGE_CANCEL										 300000130
#define RESOURCEIMAGE_OK												 300000131
#define RESOURCEIMAGE_OKCANCEL									 300000132
#define RESOURCEIMAGE_BOOLGROUP									 300000133
#define RESOURCEIMAGE_ADAPTERGROUP							 300000134
#define RESOURCEIMAGE_CALCULATEGROUP						 300000135
#define RESOURCEIMAGE_DEFAULTGROUP							 300000136
#define RESOURCEIMAGE_DEFAULTOPERATOR						 300000137
#define RESOURCEIMAGE_GENERALGROUP							 300000138
#define RESOURCEIMAGE_ITERATORGROUP							 300000139
#define RESOURCEIMAGE_LOGICALGROUP							 300000140
#define RESOURCEIMAGE_TPGROUP										 300000141
#define RESOURCEIMAGE_COFFEESCRIPT							 300000142
#define RESOURCEIMAGE_PYTHONSCRIPT							 1022749
#define RESOURCEIMAGE_UVWTAG_SECONDSTATE				 300000143
#define RESOURCEIMAGE_INSTANCEOBJECT_SECONDSTATE 300000144
#define RESOURCEIMAGE_LIGHT_SHADOWS							 300000145
#define RESOURCEIMAGE_LIGHT_SPOT								 300000146
#define RESOURCEIMAGE_LIGHT_SPOTSHADOWS					 300000147
#define RESOURCEIMAGE_LIGHT_PARALLEL						 300000148
#define RESOURCEIMAGE_LIGHT_PARALLELSHADOWS			 300000149
#define RESOURCEIMAGE_LIGHT_AREA								 300000150
#define RESOURCEIMAGE_LIGHT_AREASHADOWS					 300000151
#define RESOURCEIMAGE_BASEDRAW									 300000152
#define RESOURCEIMAGE_CTRACK										 300000153
#define RESOURCEIMAGE_BASEKEY										 300000154
#define RESOURCEIMAGE_BASESEQUENCE							 300000155
#define RESOURCEIMAGE_BASETRACK									 300000156
#define RESOURCEIMAGE_UNKNOWN										 300000157
#define RESOURCEIMAGE_BASESHADER								 300000158
#define RESOURCEIMAGE_PAINTBITMAP								 300000159
#define RESOURCEIMAGE_MULTIPLE									 300000160
#define RESOURCEIMAGE_EYEACTIVE									 300000161
#define RESOURCEIMAGE_EYEINACTIVE								 300000162
#define RESOURCEIMAGE_PENACTIVE									 300000163
#define RESOURCEIMAGE_PENINACTIVE								 300000164
#define RESOURCEIMAGE_ALPHAACTIVE								 300000165
#define RESOURCEIMAGE_ALPHAINACTIVE							 300000166
#define RESOURCEIMAGE_LINKEDACTIVE							 300000167
#define RESOURCEIMAGE_LINKEDINACTIVE						 300000168
#define RESOURCEIMAGE_BPAXIS										 300000169
#define RESOURCEIMAGE_BPCROSSED									 300000170
#define RESOURCEIMAGE_MOCCATREEVIEWNO						 300000171
#define RESOURCEIMAGE_MOCCATREEVIEWYES					 300000172
#define RESOURCEIMAGE_MOCCATREEVIEWLOCKED				 300000173
#define RESOURCEIMAGE_MOCCAIKTAG1								 300000174
#define RESOURCEIMAGE_MOCCAIKTAG2								 300000175
#define RESOURCEIMAGE_MOCCAIKTAG3								 300000176
#define RESOURCEIMAGE_MOCCAIKTAG4								 300000177
#define RESOURCEIMAGE_MOCCAIKTAG5								 300000178
#define RESOURCEIMAGE_MOCCAIKTAG6								 300000185
#define RESOURCEIMAGE_BITMAPFILTERPLUS					 300000179
#define RESOURCEIMAGE_BITMAPFILTERMINUS					 300000180
#define RESOURCEIMAGE_CLOTHING1									 300000181
#define RESOURCEIMAGE_CLOTHING2									 300000182
#define RESOURCEIMAGE_CLOTHING3									 300000183
#define RESOURCEIMAGE_CLOTHING4									 300000184
#define RESOURCEIMAGE_CLEARSELECTION						 300000187
#define RESOURCEIMAGE_GENERICCOMMAND						 300000188
#define RESOURCEIMAGE_TIMELINE_KEY1							 300000191
#define RESOURCEIMAGE_TIMELINE_KEY2							 300000192
#define RESOURCEIMAGE_AMMODELOCK_1							 300000193
#define RESOURCEIMAGE_AMMODELOCK_2							 300000194
#define RESOURCEIMAGE_SCENEBROWSER_HOME					 300000195
#define RESOURCEIMAGE_SCENEBROWSER_FILTER1			 300000196
#define RESOURCEIMAGE_SCENEBROWSER_FILTER2			 300000197
#define RESOURCEIMAGE_SCENEBROWSER_FIND1				 300000198
#define RESOURCEIMAGE_SCENEBROWSER_FIND2				 300000199
#define RESOURCEIMAGE_SCENEBROWSER_PATH1				 300000200
#define RESOURCEIMAGE_SCENEBROWSER_PATH2				 300000201
#define RESOURCEIMAGE_TIMELINE_STATE1						 300000202
#define RESOURCEIMAGE_TIMELINE_STATE2						 300000203
#define RESOURCEIMAGE_TIMELINE_STATE3						 300000204
#define RESOURCEIMAGE_TIMELINE_STATE4						 300000205
#define RESOURCEIMAGE_TIMELINE_STATE5						 300000206
#define RESOURCEIMAGE_TIMELINE_STATE6						 300000207
#define RESOURCEIMAGE_TIMELINE_KEYSTATE1				 300000208
#define RESOURCEIMAGE_TIMELINE_KEYSTATE2				 300000209
#define RESOURCEIMAGE_TIMELINE_KEYSTATE3				 300000210
#define RESOURCEIMAGE_TIMELINE_KEYSTATE4				 300000211
#define RESOURCEIMAGE_LAYERMANAGER_STATE1				 300000212
#define RESOURCEIMAGE_LAYERMANAGER_STATE2				 300000213
#define RESOURCEIMAGE_LAYERMANAGER_STATE3				 300000214
#define RESOURCEIMAGE_LAYERMANAGER_STATE4				 300000215
#define RESOURCEIMAGE_LAYERMANAGER_STATE5				 300000216
#define RESOURCEIMAGE_LAYERMANAGER_STATE6				 300000217
#define RESOURCEIMAGE_LAYERMANAGER_STATE7				 300000218
#define RESOURCEIMAGE_LAYERMANAGER_STATE8				 300000219
#define RESOURCEIMAGE_LAYERMANAGER_STATE9				 300000220
#define RESOURCEIMAGE_LAYERMANAGER_STATE10			 300000221
#define RESOURCEIMAGE_LAYERMANAGER_STATE11			 300000222
#define RESOURCEIMAGE_LAYERMANAGER_STATE12			 300000223
#define RESOURCEIMAGE_LAYERMANAGER_STATE13			 300000224
#define RESOURCEIMAGE_LAYERMANAGER_STATE14			 300000225
#define RESOURCEIMAGE_LAYERMANAGER_STATE15			 300000226
#define RESOURCEIMAGE_LAYERMANAGER_STATE16			 300000227
#define RESOURCEIMAGE_LAYERMANAGER_STATE17			 300000228
#define RESOURCEIMAGE_LAYERMANAGER_STATE18			 300000229
#define RESOURCEIMAGE_OBJECTMANAGER_STATE1			 300000230
#define RESOURCEIMAGE_OBJECTMANAGER_STATE2			 300000231
#define RESOURCEIMAGE_OBJECTMANAGER_STATE3			 300000232
#define RESOURCEIMAGE_OBJECTMANAGER_STATE4			 300000233
#define RESOURCEIMAGE_OBJECTMANAGER_DOT1				 300000234
#define RESOURCEIMAGE_OBJECTMANAGER_DOT2				 300000235
#define RESOURCEIMAGE_OBJECTMANAGER_DOT3				 300000236
#define RESOURCEIMAGE_OBJECTMANAGER_DOT4				 300000237
#define RESOURCEIMAGE_OBJECTMANAGER_LOCK				 300000238
#define RESOURCEIMAGE_TIMELINE_FOLDER1					 300000239
#define RESOURCEIMAGE_TIMELINE_FOLDER2					 300000240
#define RESOURCEIMAGE_TIMELINE_FOLDER3					 300000241
#define RESOURCEIMAGE_TIMELINE_FOLDER4					 300000242
#define RESOURCEIMAGE_TIMELINE_ROOT1						 300000243
#define RESOURCEIMAGE_TIMELINE_ROOT2						 300000244
#define RESOURCEIMAGE_TIMELINE_ROOT3						 300000245
#define RESOURCEIMAGE_OBJECTMANAGER_DISP1				 300000246
#define RESOURCEIMAGE_OBJECTMANAGER_DISP2				 300000247
#define RESOURCEIMAGE_OBJECTMANAGER_DISP3				 300000248
#define RESOURCEIMAGE_OBJECTMANAGER_DISP4				 300000249
#define RESOURCEIMAGE_BROWSER_DESKTOP						 300000251
#define RESOURCEIMAGE_BROWSER_HOME							 300000252
#define RESOURCEIMAGE_BROWSER_PRESET						 300000253
#define RESOURCEIMAGE_BROWSER_CATALOG						 300000254
#define RESOURCEIMAGE_BROWSER_SEARCH						 300000255
#define RESOURCEIMAGE_BROWSER_PLAY							 300000256
#define RESOURCEIMAGE_BROWSER_PAUSE							 300000257
#define RESOURCEIMAGE_BROWSER_SMALLVIEW					 300000258
#define RESOURCEIMAGE_BROWSER_BIGVIEW						 300000259
#define RESOURCEIMAGE_ONLINEHELP_HOME						 300000260
#define RESOURCEIMAGE_ARROWDOWN									 300000263
#define RESOURCEIMAGE_EYETRISTATE								 300000264
#define RESOURCEIMAGE_PREVIOUSPAGE							 1022433
#define RESOURCEIMAGE_FOLLOWINGPAGE							 1022434
#define RESOURCEIMAGE_LIGHT_PHOTOMETRIC					 300000265
#define RESOURCEIMAGE_LIGHT_PHOTOMETRICSHADOWS	 300000266
#define RESOURCEIMAGE_MENU_OPTIONS							 200000283
#define RESOURCEIMAGE_PICKSESSION								 200000270
#define RESOURCEIMAGE_PICKSESSION2							 200000271
#define HOTKEY_RESIZE_BRUSH											 440000144
#define RESOURCEIMAGE_LAYERMANAGER_STATE19			 1028287
#define RESOURCEIMAGE_LAYERMANAGER_STATE20			 1028288

#ifndef __API_INTERN__

	#define HOTKEY_CAMERA_MOVE	 13563
	#define HOTKEY_CAMERA_SCALE	 13564
	#define HOTKEY_CAMERA_ROTATE 13565

	#define HOTKEY_OBJECT_MOVE	 13566
	#define HOTKEY_OBJECT_SCALE	 13567
	#define HOTKEY_OBJECT_ROTATE 13568

	#define HOTKEY_MODEL_SCALE 13569
	#define HOTKEY_ZOOM				 13570
	#define HOTKEY_SELECT_FREE 13571
	#define HOTKEY_SELECT_LIVE 13572
	#define HOTKEY_SELECT_RECT 13573

	#define HOTKEY_PARENT_MOVE 440000088

	#define IDM_UNDO			 12105
	#define IDM_REDO			 12297
	#define IDM_CUT				 12106
	#define IDM_COPY			 12107
	#define IDM_PASTE			 12108
	#define IDM_DELETE		 12109
	#define IDM_SELECTALL	 12112
	#define IDM_SELECTNONE 12113
	#define IDM_INVERSION	 12374
	#define IDM_KEY_LAST	 12415
	#define IDM_KEY_NEXT	 12416

#endif

// predefined calling points for tags and scene hooks
#define	EXECUTIONPRIORITY_INITIAL				1000
#define EXECUTIONPRIORITY_ANIMATION			2000
#define EXECUTIONPRIORITY_ANIMATION_NLA	2010
#define EXECUTIONPRIORITY_EXPRESSION		3000
#define EXECUTIONPRIORITY_DYNAMICS			4000
#define EXECUTIONPRIORITY_GENERATOR			5000

enum EXECUTIONRESULT
{
	EXECUTIONRESULT_OK					= 0,
	EXECUTIONRESULT_USERBREAK		= 1,
	EXECUTIONRESULT_OUTOFMEMORY	= 2
} ENUM_END_LIST(EXECUTIONRESULT);

enum
{
	DLG_OK		 = 1,
	DLG_CANCEL = 2
};

enum IMAGERESULT
{
	IMAGERESULT_OK						=  1,
	IMAGERESULT_NOTEXISTING		= -1,
	IMAGERESULT_WRONGTYPE			= -2,
	IMAGERESULT_OUTOFMEMORY		= -3,
	IMAGERESULT_FILEERROR			= -4,
	IMAGERESULT_FILESTRUCTURE	= -5,
	IMAGERESULT_MISC_ERROR		= -6,
	IMAGERESULT_PARAM_ERROR		= -7
} ENUM_END_LIST(IMAGERESULT);

enum STRINGENCODING
{
	STRINGENCODING_XBIT		 = 0,
	STRINGENCODING_8BIT		 = 1,
	STRINGENCODING_7BIT		 = 2,
	STRINGENCODING_7BITHEX = 3,
	STRINGENCODING_UTF8		 = 4,
	STRINGENCODING_HTML		 = 5
} ENUM_END_LIST(STRINGENCODING);

enum THREADMODE
{
	THREADMODE_SYNCHRONOUS = 0,
	THREADMODE_ASYNC			 = 1
} ENUM_END_LIST(THREADMODE);

enum THREADPRIORITY
{
	THREADPRIORITY_NORMAL	= 0,
	THREADPRIORITY_ABOVE	= 1000,
	THREADPRIORITY_BELOW	= 1001,
	THREADPRIORITY_LOWEST	= 1002
} ENUM_END_LIST(THREADPRIORITY);

enum HYPERFILEARRAY
{
	HYPERFILEARRAY_CHAR	 = 1,
	HYPERFILEARRAY_WORD	 = 2,
	HYPERFILEARRAY_LONG	 = 3,
	HYPERFILEARRAY_LLONG = 4,
	HYPERFILEARRAY_SREAL = 5,
	HYPERFILEARRAY_LREAL = 6,
	HYPERFILEARRAY_REAL	 = 7
} ENUM_END_LIST(HYPERFILEARRAY);

enum FILEERROR
{
	FILEERROR_NONE				=  0,	// no error
	FILEERROR_OPEN				= -1,	// problems opening the file
	FILEERROR_CLOSE				= -2,	// problems closing the file
	FILEERROR_READ				= -3,	// problems reading the file
	FILEERROR_WRITE				= -4,	// problems writing the file
	FILEERROR_SEEK				= -5,	// problems seeking the file
	FILEERROR_INVALID			= -6,	// invalid parameter or operation (e.g. writing in read-mode)
	FILEERROR_OUTOFMEMORY	= -7,	// not enough memory
	FILEERROR_USERBREAK		= -8,	// user break

	// the following values can only occur in HyperFiles
	FILEERROR_WRONG_VALUE		 = -100,	// other value detected than expected
	FILEERROR_CHUNK_NUMBER	 = -102,	// wrong number of chunks or sub chunks detected
	FILEERROR_VALUE_NO_CHUNK = -103,	// there was a value without any enclosing START/STOP chunks
	FILEERROR_FILE_END			 = -104,	// the file end was reached without finishing reading
	FILEERROR_UNKNOWN_VALUE	 = -105		// unknown value detected
} ENUM_END_LIST(FILEERROR);

enum FILEOPEN
{
	FILEOPEN_APPEND				= 0,
	FILEOPEN_READ					= 1,
	FILEOPEN_WRITE				= 2,
	FILEOPEN_READWRITE		= 3,
	FILEOPEN_READ_NOCACHE	= 4,
	FILEOPEN_SHAREDREAD		= 5,
	FILEOPEN_SHAREDWRITE	= 6
} ENUM_END_LIST(FILEOPEN);

enum LOCATION
{
	LOCATION_DISK					= 1,	// real storage
	LOCATION_IPCONNECTION = 2,	// target is ip connection
	LOCATION_MEMORY				= 3		// target is a memory location
} ENUM_END_LIST(LOCATION);

enum FILESEEK
{
	FILESEEK_START		= 0,
	FILESEEK_RELATIVE	= 2
} ENUM_END_LIST(FILESEEK);

enum FILEDIALOG
{
	FILEDIALOG_NONE				= 0,
	FILEDIALOG_ANY				= 1,
	FILEDIALOG_IGNOREOPEN	= 2
} ENUM_END_LIST(FILEDIALOG);

enum FILESELECT
{
	FILESELECT_LOAD			 = 0,
	FILESELECT_SAVE			 = 1,
	FILESELECT_DIRECTORY = 2
} ENUM_END_LIST(FILESELECT);

enum FILESELECTTYPE
{
	FILESELECTTYPE_ANYTHING	 = 0,
	FILESELECTTYPE_IMAGES		 = 1,
	FILESELECTTYPE_SCENES		 = 2,
	FILESELECTTYPE_COFFEE		 = 3,
	FILESELECTTYPE_BODYPAINT = 4
} ENUM_END_LIST(FILESELECTTYPE);

enum OPERATINGSYSTEM
{
	OPERATINGSYSTEM_WIN	 = 1,
	OPERATINGSYSTEM_OSX	 = 2,
	OPERATINGSYSTEM_UNIX = 3
} ENUM_END_LIST(OPERATINGSYSTEM);

enum BYTEORDER
{
	BYTEORDER_MOTOROLA = 1,
	BYTEORDER_INTEL		 = 2
} ENUM_END_LIST(BYTEORDER);

enum HYPERFILEVALUE
{
	HYPERFILEVALUE_NONE							 =  0,

	HYPERFILEVALUE_START						 =  1,
	HYPERFILEVALUE_STOP							 =  2,
	HYPERFILEVALUE_CSTOP						 =  3,
	HYPERFILEVALUE_CHAR							 = 11,
	HYPERFILEVALUE_UCHAR						 = 12,
	HYPERFILEVALUE_INT16						 = 13,
	HYPERFILEVALUE_UINT16						 = 14,
	HYPERFILEVALUE_INT32						 = 15,
	HYPERFILEVALUE_UINT32						 = 16,
	HYPERFILEVALUE_INT64						 = 17,
	HYPERFILEVALUE_UINT64						 = 18,
	HYPERFILEVALUE_FLOAT						 = 19,
	HYPERFILEVALUE_FLOAT64					 = 20,
	HYPERFILEVALUE_BOOL							 = 21,
	HYPERFILEVALUE_TIME							 = 22,
	HYPERFILEVALUE_VECTOR						 = 23,
	HYPERFILEVALUE_VECTOR64					 = 24,
	HYPERFILEVALUE_MATRIX						 = 25,
	HYPERFILEVALUE_MATRIX64					 = 26,
	HYPERFILEVALUE_VECTOR32					 = 27,
	HYPERFILEVALUE_MATRIX32					 = 28,
	HYPERFILEVALUE_FLOAT32					 = 29,

	HYPERFILEVALUE_MEMORY						 = 128,
	HYPERFILEVALUE_IMAGE						 = 129,
	HYPERFILEVALUE_STRING						 = 130,
	HYPERFILEVALUE_FILENAME					 = 131,
	HYPERFILEVALUE_CONTAINER				 = 132,
	HYPERFILEVALUE_ALIASLINK				 = 138,
	HYPERFILEVALUE_LMEMORY					 = 139,
	HYPERFILEVALUE_VECTOR_ARRAY_EX	 = 133,
	HYPERFILEVALUE_POLYGON_ARRAY_EX	 = 134,
	HYPERFILEVALUE_UINT16_ARRAY_EX	 = 135,
	HYPERFILEVALUE_PARTICLE_ARRAY_EX = 136,
	HYPERFILEVALUE_SREAL_ARRAY_EX		 = 137,
	HYPERFILEVALUE_ARRAY						 = 140,
	HYPERFILEVALUE_UUID							 = 141
} ENUM_END_LIST(HYPERFILEVALUE);

enum FINDANIM
{
	FINDANIM_EXACT = 0,
	FINDANIM_LEFT	 = 1,
	FINDANIM_RIGHT = 2
} ENUM_END_LIST(FINDANIM);

enum CCURVE
{
	CCURVE_CURVE		 = 1,
	CCURVE_HLE_BASE	 = 2,
	CCURVE_HLE_CURVE = 3,
	CCURVE_SS_CURVE	 = 4,

	// multiple Snapshots
	CCURVE_SS_CURVE2 = 5,
	CCURVE_SS_CURVE3 = 6,
	CCURVE_SS_CURVE4 = 7,
	CCURVE_SS_CURVE5 = 8,

	// Scale and Move HLE Curve
	CCURVE_HLE_SCALE = 9,
	CCURVE_HLE_MOVE	 = 10
} ENUM_END_LIST(CCURVE);

enum CLOOP
{
	CLOOP_OFF					 = 0,
	CLOOP_CONSTANT		 = 1,
	CLOOP_CONTINUE		 = 2,
	CLOOP_REPEAT			 = 3,
	CLOOP_OFFSETREPEAT = 4,
	CLOOP_OSCILLATE		 = 5
} ENUM_END_LIST(CLOOP);

enum CINTERPOLATION
{
	CINTERPOLATION_SPLINE = 1,
	CINTERPOLATION_LINEAR = 2,
	CINTERPOLATION_STEP		= 3,

	CINTERPOLATION_DUMMY	= 4
} ENUM_END_LIST(CINTERPOLATION);

enum CLIPBOARDTYPE
{
	CLIPBOARDTYPE_EMPTY	 =0,
	CLIPBOARDTYPE_STRING =1,
	CLIPBOARDTYPE_BITMAP =2
} ENUM_END_LIST(CLIPBOARDTYPE);

enum EDGESELECTIONTYPE
{
	EDGESELECTIONTYPE_SELECTION = 0,
	EDGESELECTIONTYPE_HIDDEN		= 1,
	EDGESELECTIONTYPE_PHONG			= 2
} ENUM_END_LIST(EDGESELECTIONTYPE);

enum REGISTRYTYPE
{
	REGISTRYTYPE_ANY							=  0,
	REGISTRYTYPE_WINDOW						=  1,
	REGISTRYTYPE_OBJECT						=  2,
	REGISTRYTYPE_TRACK_EX					=  3,
	REGISTRYTYPE_SEQUENCE_EX			=  4,
	REGISTRYTYPE_KEY_EX						=  5,
	REGISTRYTYPE_TAG							=  6,
	REGISTRYTYPE_MATERIAL					=  7,
	REGISTRYTYPE_SHADER						=  8,
	REGISTRYTYPE_COFFEE_EXT				=  9,
	REGISTRYTYPE_SOUND						=	10,
	REGISTRYTYPE_LAYOUT						=	11,
	REGISTRYTYPE_BITMAPFILTER			=	12,
	REGISTRYTYPE_VIDEOPOST				=	13,
	REGISTRYTYPE_SCENEHOOK				=	14,
	REGISTRYTYPE_NODE							=	15,
	REGISTRYTYPE_DESCRIPTION			=	16,
	REGISTRYTYPE_LIBRARY					=	17,
	REGISTRYTYPE_CUSTOMDATATYPE		=	18,
	REGISTRYTYPE_RESOURCEDATATYPE	=	19,
	REGISTRYTYPE_SCENELOADER			=	20,
	REGISTRYTYPE_SCENESAVER				=	21,
	REGISTRYTYPE_SNHOOK						=	22,
	REGISTRYTYPE_CTRACK						= 23,
	REGISTRYTYPE_CSEQ							= 24,
	REGISTRYTYPE_CKEY							= 25,
	REGISTRYTYPE_PAINTER					=	26,
	REGISTRYTYPE_GV_VALUE					= 27,
	REGISTRYTYPE_GV_VALGROUP			= 28,
	REGISTRYTYPE_GV_OPGROUP				= 29,
	REGISTRYTYPE_GV_OPCLASS				= 30,
	REGISTRYTYPE_GV_DATA					= 31,
	REGISTRYTYPE_GADGETS					= 32,
	REGISTRYTYPE_PREFS						= 33
} ENUM_END_LIST(REGISTRYTYPE);

enum MODELINGCOMMANDMODE
{
	MODELINGCOMMANDMODE_ALL							 = 0,
	MODELINGCOMMANDMODE_POINTSELECTION	 = 1,
	MODELINGCOMMANDMODE_POLYGONSELECTION = 2,
	MODELINGCOMMANDMODE_EDGESELECTION		 = 3
} ENUM_END_LIST(MODELINGCOMMANDMODE);

enum MODELINGCOMMANDFLAGS
{
	MODELINGCOMMANDFLAGS_0					= 0,
	MODELINGCOMMANDFLAGS_CREATEUNDO	= (1 << 0)
} ENUM_END_FLAGS(MODELINGCOMMANDFLAGS);

enum PLUGINTYPE
{
	PLUGINTYPE_ANY								=  0,

	PLUGINTYPE_SHADER							=  1,
	PLUGINTYPE_MATERIAL						=  2,
	PLUGINTYPE_COFFEEMESSAGE			=  3,
	PLUGINTYPE_COMMAND						=  4,
	PLUGINTYPE_OBJECT							=  5,
	PLUGINTYPE_TAG								=  6,
	PLUGINTYPE_BITMAPFILTER				=  7,
	PLUGINTYPE_VIDEOPOST					=  8,
	PLUGINTYPE_TOOL								=  9,
	PLUGINTYPE_SCENEHOOK					= 10,
	PLUGINTYPE_NODE								= 11,
	PLUGINTYPE_LIBRARY						= 12,
	PLUGINTYPE_BITMAPLOADER				= 13,
	PLUGINTYPE_BITMAPSAVER				= 14,
	PLUGINTYPE_SCENELOADER				= 15,
	PLUGINTYPE_SCENESAVER					= 16,
	PLUGINTYPE_COREMESSAGE				= 17,
	PLUGINTYPE_CUSTOMGUI					= 18,
	PLUGINTYPE_CUSTOMDATATYPE			= 19,
	PLUGINTYPE_RESOURCEDATATYPE		= 20,
	PLUGINTYPE_MANAGERINFORMATION	= 21,
	PLUGINTYPE_CTRACK							= 32,
	PLUGINTYPE_FALLOFF						= 33,
	PLUGINTYPE_VMAPTRANSFER				= 34,
	PLUGINTYPE_PREFS							= 35,
	PLUGINTYPE_SNAP								= 36
} ENUM_END_LIST(PLUGINTYPE);

enum DRAWRESULT
{
	DRAWRESULT_ERROR = 0,
	DRAWRESULT_OK		 = 1,
	DRAWRESULT_SKIP	 = 2
} ENUM_END_LIST(DRAWRESULT);

enum DISPLAYMODE
{
	DISPLAYMODE_UNKNOWN					= -1,
	DISPLAYMODE_GOURAUD					= 0,
	DISPLAYMODE_QUICK						= 1,
	DISPLAYMODE_WIRE						= 2,
	DISPLAYMODE_ISOPARM					= 3,
	DISPLAYMODE_SHADEDBOX				= 4,
	DISPLAYMODE_BOX							= 5,
	DISPLAYMODE_SKELETON				= 6,
	DISPLAYMODE_GOURAUDWIRE			= 7,
	DISPLAYMODE_GOURAUDISOPARM	= 8,
	DISPLAYMODE_QUICKWIRE				= 9,
	DISPLAYMODE_QUICKISOPARM		= 10,
	DISPLAYMODE_FLATWIRE				= 11,
	DISPLAYMODE_FLATISOPARM			= 12,
	DISPLAYMODE_FLATBOX					= 13,
	DISPLAYMODE_HIDDENWIRE			= 14,
	DISPLAYMODE_HIDDENISOPARM		= 15,
	DISPLAYMODE_HIDDENBOX				= 16,
	DISPLAYMODE_SHADEDBOXWIRE		= 17,
	DISPLAYMODE_QUICKBOXWIRE		= 18,
	DISPLAYMODE_QUICKBOX				= 19,

	DISPLAYMODE_PRIVATE_ISOLINE	= 100,
	DISPLAYMODE_PRIVATE_FLAT		= 1100,
	DISPLAYMODE_PRIVATE_HIDDEN	= 1400
} ENUM_END_LIST(DISPLAYMODE);

enum DOCUMENTSETTINGS
{
	DOCUMENTSETTINGS_GENERAL				 = 0,
	DOCUMENTSETTINGS_MODELING				 = 1,
	DOCUMENTSETTINGS_DOCUMENT				 = 2,
	DOCUMENTSETTINGS_ANIMATIONSYSTEM = 7,
	DOCUMENTSETTINGS_TOOLS					 = 8
} ENUM_END_LIST(DOCUMENTSETTINGS);

enum SERIALINFO
{
	SERIALINFO_CINEMA4D			= 0,
	SERIALINFO_MULTILICENSE	= 2
} ENUM_END_LIST(SERIALINFO);

enum VERSIONTYPE
{
	VERSIONTYPE_PRIME								 = 0,
	VERSIONTYPE_BODYPAINT						 = 1,
	VERSIONTYPE_STUDIO							 = 2,
	VERSIONTYPE_VISUALIZE						 = 3,
	VERSIONTYPE_BROADCAST						 = 4,
	VERSIONTYPE_BENCHMARK						 = 5,
	VERSIONTYPE_UPDATER							 = 6,
	VERSIONTYPE_INSTALLER						 = 7,
	VERSIONTYPE_NET_CLIENT					 = 8,
	VERSIONTYPE_NET_SERVER_3				 = 9,
	VERSIONTYPE_NET_SERVER_UNLIMITED = 10,
	VERSIONTYPE_UNKNOWN							 = 11,	// unknown
	VERSIONTYPE_LICENSESERVER				 = 12
} ENUM_END_LIST(VERSIONTYPE);

enum LAYERSETMODE
{
	LAYERSETMODE_LAYERS,
	LAYERSETMODE_LAYERMASKS,
	LAYERSETMODE_ALPHAS,
	LAYERSETMODE_LAYERALPHA,
	LAYERSETMODE_DISABLED
} ENUM_END_LIST(LAYERSETMODE);

enum SYSTEMINFO
{
	SYSTEMINFO_0									= 0,
	SYSTEMINFO_COMMANDLINE				= (1 << 1),	// application runs in command line mode
	SYSTEMINFO_DEMO								= (1 << 2),	// (deprecated)
	SYSTEMINFO_SAVABLEDEMO				= (1 << 3),	// savable demo
	SYSTEMINFO_SAVABLEDEMO_ACTIVE	= (1 << 4),	// activated savable demo, SYSTEMINFO_SAVABLEDEMO is still set
	SYSTEMINFO_OPENGL							= (1 << 5),	// OpenGL is activated and loaded correctly
	SYSTEMINFO_STUDENT						= (1 << 6),	// activated student version, this is always set along with SYSTEMINFO_SAVABLEDEMO
	SYSTEMINFO_LITE								= (1 << 7),	// light version, cannot load any plugins
	SYSTEMINFO_LITE_ACTIVE				= (1 << 8)	// light version is registered
} ENUM_END_FLAGS(SYSTEMINFO);

#define ID_MT_SOURCECOUNTER	465001520	//Int32

// maximum number of texture paths
#define MAX_GLOBAL_TEXTURE_PATHS 10

enum SELECTIONFILTERBIT
{
	SELECTIONFILTERBIT_0					= 0,
	SELECTIONFILTERBIT_NULL				= (1 << 0),
	SELECTIONFILTERBIT_POLYGON		= (1 << 1),
	SELECTIONFILTERBIT_SPLINE			= (1 << 2),
	SELECTIONFILTERBIT_GENERATOR	= (1 << 3),
	SELECTIONFILTERBIT_HYPERNURBS = (1 << 4),
	SELECTIONFILTERBIT_DEFORMER		= (1 << 6),
	SELECTIONFILTERBIT_CAMERA			= (1 << 7),
	SELECTIONFILTERBIT_LIGHT			= (1 << 8),
	SELECTIONFILTERBIT_SCENE			= (1 << 9),
	SELECTIONFILTERBIT_PARTICLE		= (1 << 10),
	SELECTIONFILTERBIT_OTHER			= (1 << 11),
	SELECTIONFILTERBIT_JOINT			= (1 << 25)
} ENUM_END_FLAGS(SELECTIONFILTERBIT);

enum OBJECTSTATE
{
	OBJECTSTATE_EDITOR = 0,
	OBJECTSTATE_RENDER = 1,
	OBJECTSTATE_DEFORM = 2
} ENUM_END_LIST(OBJECTSTATE);

// display filter	(nullptr to OTHER match SELECTIONFILTERBIT_)
enum DISPLAYFILTER
{
	DISPLAYFILTER_0									 = 0,
	DISPLAYFILTER_NULL							 = (1 << 0),
	DISPLAYFILTER_POLYGON						 = (1 << 1),
	DISPLAYFILTER_SPLINE						 = (1 << 2),
	DISPLAYFILTER_GENERATOR					 = (1 << 3),
	DISPLAYFILTER_HYPERNURBS				 = (1 << 4),
	DISPLAYFILTER_UNUSED1						 = (1 << 5),
	DISPLAYFILTER_DEFORMER					 = (1 << 6),
	DISPLAYFILTER_CAMERA						 = (1 << 7),
	DISPLAYFILTER_LIGHT							 = (1 << 8),
	DISPLAYFILTER_SCENE							 = (1 << 9),
	DISPLAYFILTER_PARTICLE					 = (1 << 10),
	DISPLAYFILTER_OTHER							 = (1 << 11),
	DISPLAYFILTER_GRID							 = (1 << 13),
	DISPLAYFILTER_HORIZON						 = (1 << 14),
	DISPLAYFILTER_WORLDAXIS					 = (1 << 15),
	DISPLAYFILTER_BOUNDS						 = (1 << 16),
	DISPLAYFILTER_HUD								 = (1 << 17),
	DISPLAYFILTER_SDS								 = (1 << 18),
	DISPLAYFILTER_HIGHLIGHTING			 = (1 << 19),
	DISPLAYFILTER_MULTIAXIS					 = (1 << 20),
	DISPLAYFILTER_OBJECTHANDLES			 = (1 << 21),
	DISPLAYFILTER_HANDLEBANDS				 = (1 << 22),
	DISPLAYFILTER_SDSCAGE						 = (1 << 23),
	DISPLAYFILTER_NGONLINES					 = (1 << 24),
	DISPLAYFILTER_JOINT							 = (1 << 25),
	DISPLAYFILTER_OBJECTHIGHLIGHTING = (1 << 26),
	DISPLAYFILTER_GUIDELINES				 = (1 << 27),
	DISPLAYFILTER_POI								 = (1 << 28),
	DISPLAYFILTER_GRADIENT					 = (1 << 29)
} ENUM_END_FLAGS(DISPLAYFILTER);

enum DISPLAYEDITSTATE
{
	DISPLAYEDITSTATE_0				= 0,
	DISPLAYEDITSTATE_SDS			= (1 << 0),
	DISPLAYEDITSTATE_DEFORM		= (1 << 1),

	DISPLAYEDITSTATE_DOCUMENT	= -1
} ENUM_END_FLAGS(DISPLAYEDITSTATE);

enum THREADTYPE
{
	THREADTYPE_0							= 0,
	THREADTYPE_EDITORREDRAW		= (1 << 0),
	THREADTYPE_RENDEREDITOR		= (1 << 1),
	THREADTYPE_RENDEREXTERNAL	= (1 << 2)
} ENUM_END_FLAGS(THREADTYPE);

enum RENDERPROGRESSTYPE
{
	RENDERPROGRESSTYPE_BEFORERENDERING		= 0,
	RENDERPROGRESSTYPE_DURINGRENDERING		= 1,
	RENDERPROGRESSTYPE_AFTERRENDERING			= 2,
	RENDERPROGRESSTYPE_GLOBALILLUMINATION	= 3
} ENUM_END_LIST(RENDERPROGRESSTYPE);

enum RDATA_SAVECALLBACK_CMD
{
	RDATA_SAVECALLBACK_CMD_OPEN	 = 1,
	RDATA_SAVECALLBACK_CMD_WRITE = 2,
	RDATA_SAVECALLBACK_CMD_CLOSE = 3
} ENUM_END_LIST(RDATA_SAVECALLBACK_CMD);

enum VPGETINFO
{
	VPGETINFO_XRESOLUTION	= 0,
	VPGETINFO_YRESOLUTION	= 1,
	VPGETINFO_BITDEPTH		= 2,
	VPGETINFO_CPP					= 3,
	VPGETINFO_VISIBLE			= 4,
	VPGETINFO_LINEOFFSET	= 5	// offset of component in line
} ENUM_END_LIST(VPGETINFO);

enum DRAWOBJECT
{
	DRAWOBJECT_0								= 0,
	DRAWOBJECT_FORCELINES				= (1 << 0),
	DRAWOBJECT_NOBACKCULL				= (1 << 1),
	DRAWOBJECT_LOCALMATRIX			= (1 << 2),
	DRAWOBJECT_EDITMODE					= (1 << 3),
	DRAWOBJECT_FORCEBASE				= (1 << 9),
	DRAWOBJECT_FORCEPOINTS			= (1 << 10),
	DRAWOBJECT_NO_EOGL					= (1 << 11),
	DRAWOBJECT_USE_OBJECT_COLOR = (1 << 12),
	DRAWOBJECT_USE_CUSTOM_COLOR = (1 << 13),
	DRAWOBJECT_XRAY_ON					= (1 << 14),
	DRAWOBJECT_XRAY_OFF					= (1 << 15),
	DRAWOBJECT_IMMEDIATELY			= (1 << 16),
	DRAWOBJECT_Z_OFFSET					= (1 << 17),	// don't change the Z offset during DrawObject
	DRAWOBJECT_PRIVATE_ANY			= (1 << 30)
} ENUM_END_FLAGS(DRAWOBJECT);

enum RENDERFLAGS
{
	RENDERFLAGS_0										 = 0,
	RENDERFLAGS_EXTERNAL						 = (1 << 0),
	RENDERFLAGS_NODOCUMENTCLONE			 = (1 << 1),
	RENDERFLAGS_SHOWERRORS					 = (1 << 2),
	RENDERFLAGS_PREVIEWRENDER				 = (1 << 3),
	RENDERFLAGS_IRR									 = (1 << 4),	// Render in Interactive Render Region
	RENDERFLAGS_CREATE_PICTUREVIEWER = (1 << 5),	// Render in Picture Viewer
	RENDERFLAGS_OPEN_PICTUREVIEWER	 = (1 << 6),
	RENDERFLAGS_KEEP_CONTEXT				 = (1 << 7),	// private
	RENDERFLAGS_BATCHRENDER					 = (1 << 8),	// Render in Batch Render - private
	RENDERFLAGS_NET									 = (1 << 9)		// Use NET System for rendering
} ENUM_END_FLAGS(RENDERFLAGS);

enum WRITEMODE
{
	WRITEMODE_STANDARD = 0,
	WRITEMODE_ASSEMBLE_MOVIE = 1,
	WRITEMODE_ASSEMBLE_SINGLEIMAGE = 2
} ENUM_END_LIST(WRITEMODE);

enum NETRENDERFLAGS
{
	NETRENDERFLAGS_0														 = 0,
	NETRENDERFLAGS_OPEN_PICTUREVIEWER						 = (1 << 0),
	NETRENDERFLAGS_SHOWERRORS										 = (1 << 2),
	NETRENDERFLAGS_DELETEAFTERRENDERING					 = (1 << 3),
	NETRENDERFLAGS_NOPEERTOPEERASSETDISTRIBUTION = (1 << 4),
	NETRENDERFLAGS_NOREQUESTONDEMAND						 = (1 << 5),
	NETRENDERFLAGS_EXCLUDECLIENTONRENDERINGERROR = (1 << 6),
	NETRENDERFLAGS_SAVERESULTSINREPOSITORY			 = (1 << 7),
	NETRENDERFLAGS_ASSEMBLEB3DFILESIMMEDIATLEY	 = (1 << 8),
	NETRENDERFLAGS_NOWRITETEST									 = (1 << 9)
} ENUM_END_FLAGS(NETRENDERFLAGS);

enum CHECKISRUNNING
{
	CHECKISRUNNING_ANIMATIONRUNNING				= 0,
	CHECKISRUNNING_VIEWDRAWING						= 1,
	CHECKISRUNNING_EDITORRENDERING				= 2,
	CHECKISRUNNING_EXTERNALRENDERING			= 3,
	CHECKISRUNNING_PAINTERUPDATING				= 4,
	CHECKISRUNNING_MATERIALPREVIEWRUNNING	= 5,
	CHECKISRUNNING_EVENTSYSTEM						= 6
} ENUM_END_LIST(CHECKISRUNNING);

enum BAKE_TEX_ERR
{
	BAKE_TEX_ERR_NONE								= 0,
	BAKE_TEX_ERR_NO_DOC							= 3000,	// no document
	BAKE_TEX_ERR_NO_MEM							= 3001,	// no more memory available
	BAKE_TEX_ERR_NO_RENDER_DOC			= 3002,	// no render document
	BAKE_TEX_ERR_NO_TEXTURE_TAG			= 3003,	// textag is nullptr or not in doc
	BAKE_TEX_ERR_NO_OBJECT					= 3004,	// one of the tags is not assigned to an object or to another object
	BAKE_TEX_ERR_NO_UVW_TAG					= 3005,	// UVW Tag is missing
	BAKE_TEX_ERR_TEXTURE_MISSING		= 3006,	// no texture
	BAKE_TEX_ERR_WRONG_BITMAP				= 3007,	// MultipassBitmap was used, but it has the wrong type or wrong resolution
	BAKE_TEX_ERR_USERBREAK					= 3008,	// user break
	BAKE_TEX_ERR_NO_OPTIMAL_MAPPING	= 3009,	// optimal mapping failed
	BAKE_TEX_ERR_NO_SOURCE_UVW_TAG	= 3010	// UVW Tag for the source object is missing
} ENUM_END_LIST(BAKE_TEX_ERR);

enum GL_MESSAGE
{
	GL_MESSAGE_OK							 = 1,
	GL_MESSAGE_ERROR					 = 0,
	GL_MESSAGE_FORCE_EMULATION = 2
} ENUM_END_LIST(GL_MESSAGE);

enum VIEWPORT_PICK_FLAGS
{
	VIEWPORT_PICK_FLAGS_0													= 0,
	VIEWPORT_PICK_FLAGS_ALLOW_OGL									= (1 << 0),
	VIEWPORT_PICK_FLAGS_DONT_STOP_THREADS					= (1 << 1),
	VIEWPORT_PICK_FLAGS_USE_SEL_FILTER						= (1 << 2),
	VIEWPORT_PICK_FLAGS_OGL_ONLY_TOPMOST					= (1 << 3),	// use this only when you don't need the object pointer, does only work with OpenGL
	VIEWPORT_PICK_FLAGS_OGL_ONLY_VISIBLE					= (1 << 4),	// this has only an effect when the PickObject functions are called that take ViewportPixel as argument
	VIEWPORT_PICK_FLAGS_OGL_IGNORE_Z							= (1 << 5),	// set this if you are only interested if (and which) an object was hit, not its Z position
	VIEWPORT_PICK_FLAGS_OGL_ONLY_TOPMOST_WITH_OBJ	= (1 << 6)	// only returns the topmost object with its Z position
} ENUM_END_FLAGS(VIEWPORT_PICK_FLAGS);

// HandleShaderPopup
#define SHADERPOPUP_SETSHADER				 99989
#define SHADERPOPUP_SETFILENAME			 99990
#define SHADERPOPUP_LOADIMAGE				 99991
#define SHADERPOPUP_EDITPARAMS			 99999
#define SHADERPOPUP_RELOADIMAGE			 99998
#define SHADERPOPUP_EDITIMAGE				 99997
#define SHADERPOPUP_COPYCHANNEL			 99995
#define SHADERPOPUP_PASTECHANNEL		 99994
#define SHADERPOPUP_CREATENEWTEXTURE 99993
#define SHADERPOPUP_CLEARSHADER			 99992

#define DEFAULTFILENAME_SHADER_SURFACES	1001
#define DEFAULTFILENAME_SHADER_EFFECTS	1002
#define DEFAULTFILENAME_SHADER_VOLUME		1003

// Background handler
enum BACKGROUNDHANDLERCOMMAND
{
	BACKGROUNDHANDLERCOMMAND_ISRUNNING = 100,
	BACKGROUNDHANDLERCOMMAND_STOP			 = 101,
	BACKGROUNDHANDLERCOMMAND_START		 = 102,
	BACKGROUNDHANDLERCOMMAND_REMOVE		 = 103
} ENUM_END_LIST(BACKGROUNDHANDLERCOMMAND);

#define BACKGROUNDHANDLER_PRIORITY_RENDERACTIVEMATERIAL		 5000
#define BACKGROUNDHANDLER_PRIORITY_REDRAWVIEW							 4000
#define BACKGROUNDHANDLER_PRIORITY_RENDERINACTIVEMATERIALS 3000
#define BACKGROUNDHANDLER_PRIORITY_RENDEREXTERNAL					 -1000
#define BACKGROUNDHANDLER_PRIORITY_REDRAWANTS							 -2000

enum BACKGROUNDHANDLERFLAGS
{
	BACKGROUNDHANDLERFLAGS_0									= 0,
	BACKGROUNDHANDLERFLAGS_VIEWREDRAW					= (1 << 0),
	BACKGROUNDHANDLERFLAGS_EDITORRENDDER			= (1 << 1),
	BACKGROUNDHANDLERFLAGS_MATERIALPREVIEW		= (1 << 2),
	BACKGROUNDHANDLERFLAGS_RENDEREXTERNAL			= (1 << 3),
	BACKGROUNDHANDLERFLAGS_PRIVATE_VIEWREDRAW	= (1 << 4),

	BACKGROUNDHANDLERFLAGS_SHUTDOWN						= -1
} ENUM_END_FLAGS(BACKGROUNDHANDLERFLAGS);

#define BACKGROUNDHANDLER_TYPECLASS_C4D	1000

// Identify File
enum IDENTIFYFILE
{
	IDENTIFYFILE_0						 = 0,
	IDENTIFYFILE_SCENE				 = (1 << 0),
	IDENTIFYFILE_IMAGE				 = (1 << 1),
	IDENTIFYFILE_MOVIE				 = (1 << 2),
	IDENTIFYFILE_SKIPQUICKTIME = (1 << 3),
	IDENTIFYFILE_SCRIPT				 = (1 << 4),
	IDENTIFYFILE_COFFEE				 = (1 << 5),
	IDENTIFYFILE_SOUND				 = (1 << 6),
	IDENTIFYFILE_LAYOUT				 = (1 << 7),
	IDENTIFYFILE_PYTHON				 = (1 << 8)
} ENUM_END_FLAGS(IDENTIFYFILE);

enum CALCHARDSHADOW
{
	CALCHARDSHADOW_0								 = 0,
	CALCHARDSHADOW_TRANSPARENCY			 = (1 << 0),
	CALCHARDSHADOW_SPECIALGISHADOW	 = (1 << 29),
	CALCHARDSHADOW_SPECIALSELFSHADOW = (1 << 30)
} ENUM_END_FLAGS(CALCHARDSHADOW);

enum ILLUMINATEFLAGS
{
	ILLUMINATEFLAGS_0																 = 0,
	ILLUMINATEFLAGS_SHADOW													 = (1 << 0),
	ILLUMINATEFLAGS_NOENVIRONMENT										 = (1 << 1),
	ILLUMINATEFLAGS_DISABLESHADOWMAP_CORRECTION			 = (1 << 20),
	ILLUMINATEFLAGS_DISABLESHADOWCASTERMP_CORRECTION = (1 << 21),
	ILLUMINATEFLAGS_LIGHTDIRNORMALS									 = (1 << 22),
	ILLUMINATEFLAGS_NODISTANCEFALLOFF								 = (1 << 23),
	ILLUMINATEFLAGS_NOGRAIN													 = (1 << 24),
	ILLUMINATEFLAGS_BACKLIGHT												 = (1 << 25)
} ENUM_END_FLAGS(ILLUMINATEFLAGS);

enum RAYBIT
{
	RAYBIT_0								 = 0,
	RAYBIT_REFLECTION				 = (1 << 0),	// ray chain contains a reflection ray
	RAYBIT_TRANSPARENCY			 = (1 << 1),	// ray chain contains a transparency ray (note: refractions are not contained)
	RAYBIT_REFRACTION				 = (1 << 2),	// ray chain contains a refraction ray
	RAYBIT_CUSTOM						 = (1 << 3),	// ray chain contains a custom ray

	RAYBIT_CURR_REFLECTION	 = (1 << 4),	// current ray is a reflection ray
	RAYBIT_CURR_TRANSPARENCY = (1 << 5),	// current ray is a transparency ray
	RAYBIT_CURR_REFRACTION	 = (1 << 6),	// current ray is a refraction ray
	RAYBIT_CURR_CUSTOM			 = (1 << 7),	// current ray is a custom ray

	RAYBIT_VOLUMETRICLIGHT	 = (1 << 8),	// current ray is used to calculate a volumetric light
	RAYBIT_ALLOWVLMIX				 = (1 << 9),	// custom mixing of visible light sources allowed for this ray; bit must be deleted by shader if used

	RAYBIT_GI								 = (1 << 10),	// current ray is a Global Illumination ray
	RAYBIT_BLURRY						 = (1 << 11),	// current ray is a blurry ray
	RAYBIT_SSS							 = (1 << 12),	// current ray is a subsurface ray

	RAYBIT_AO								 = (1 << 13),	// current ray is an Ambient Occlusion ray
	RAYBIT_COMPOSITING			 = (1 << 14)	// current ray is a compositing ray
} ENUM_END_FLAGS(RAYBIT);

enum VOLUMEINFO
{
	VOLUMEINFO_0									= 0,
	VOLUMEINFO_REFLECTION					= 0x00000002,	// shader calculates reflections
	VOLUMEINFO_TRANSPARENCY				= 0x00000004,	// shader calculates transparency
	VOLUMEINFO_ALPHA							= 0x00000008,	// shader calculates alpha
	VOLUMEINFO_CHANGENORMAL				= 0x00002000,	// shader calculates bump mapping
	VOLUMEINFO_DISPLACEMENT				= 0x00004000,	// shader calculates displacement mapping
	VOLUMEINFO_ENVREQUIRED				= 0x00100000,	// shader needs environment reflection data
	VOLUMEINFO_DUDVREQUIRED				= 0x00200000,	// shader needs du/dv bump mapping data
	VOLUMEINFO_MIPSAT							= 0x02000000,	// shader requires MIP/SAT data
	VOLUMEINFO_VOLUMETRIC					= 0x20000000,	// shader is a volumetric shader
	VOLUMEINFO_TRANSFORM					= 0x00000010,	// shader needs back-transformed data
	VOLUMEINFO_EVALUATEPROJECTION	= 0x04000000,	// shader requires texture tag projections
	VOLUMEINFO_PRIVATE_GLOW				= 0x10000000,	// shader calculates glow (private)
	VOLUMEINFO_INITCALCULATION		= -1	// shader needs initcalculation call
} ENUM_END_FLAGS(VOLUMEINFO);

enum VIDEOPOSTINFO
{
	VIDEOPOSTINFO_0											 = 0,
	VIDEOPOSTINFO_STOREFRAGMENTS				 = (1 << 0),	// VP needs fragment information for whole image at VP_INNER/VP_RENDER
	VIDEOPOSTINFO_EXECUTELINE						 = (1 << 4),	// line override
	VIDEOPOSTINFO_EXECUTEPIXEL					 = (1 << 5),	// pixel override
	VIDEOPOSTINFO_REQUEST_MOTIONMATRIX	 = (1 << 6),
	VIDEOPOSTINFO_REQUEST_MOTIONGEOMETRY = (1 << 7),
	VIDEOPOSTINFO_CALCVOLUMETRIC				 = (1 << 8),
	VIDEOPOSTINFO_CALCSHADOW						 = (1 << 9),
	VIDEOPOSTINFO_CUSTOMLENS						 = (1 << 10),
	VIDEOPOSTINFO_GLOBALILLUMINATION		 = (1 << 11),	// post effect is GI hook
	VIDEOPOSTINFO_CAUSTICS							 = (1 << 12),	// post effect is Caustics hook
	VIDEOPOSTINFO_CUSTOMLENS_EXTENDED		 = (1 << 13),	// post effect is extended lens for physical render
	VIDEOPOSTINFO_NETFRAME							 = (1 << 14),	// post effect is Net Frame hook
	VIDEOPOSTINFO_NETRUNONSERVER				 = (1 << 15),	// post effect can be run on the Net Server
	VIDEOPOSTINFO_NETCREATEBUFFER				 = (1 << 16),	// post effect creates a buffer for Net Client
} ENUM_END_FLAGS(VIDEOPOSTINFO);

enum SHADERINFO
{
	SHADERINFO_0								 = 0,
	SHADERINFO_TRANSFORM				 = 0x00000004,	// channel shader needs back-transformed data
	SHADERINFO_BUMP_SUPPORT			 = 0x00000010,	// channel shader supports new bump system (strongly recommended for all shader but simple 2d (UV) samplers)
	SHADERINFO_ALPHA_SUPPORT		 = 0x00000020,	// channel shader supports alpha output
	SHADERINFO_REFLECTIONS			 = 0x00000040,	// channel shader computes reflections
	SHADERINFO_DUDVREQUIRED			 = 0x00000080,	// channel shader needs du/dv bump mapping data
	SHADERINFO_DYNAMICSUBSHADERS = 0x00000100		// channel shader has a dynamic list of sub shaders in its descriptions
} ENUM_END_FLAGS(SHADERINFO);

enum SAMPLEBUMP
{
	SAMPLEBUMP_0					= 0,
	SAMPLEBUMP_MIPFALLOFF	= (1 << 0)
};

enum INITCALCULATION
{
	INITCALCULATION_SURFACE			 = 0,
	INITCALCULATION_TRANSPARENCY = 1,
	INITCALCULATION_DISPLACEMENT = 3
} ENUM_END_LIST(INITCALCULATION);

// COFFEE Scripts
#define ID_SCRIPTFOLDER	1026688
#define ID_COFFEESCRIPT	1001085
#define ID_PYTHONSCRIPT	1026256

#define COFFEESCRIPT_TEXT					1000
#define COFFEESCRIPT_SHOWINMENU		1002
#define COFFEESCRIPT_ADDEVENT			1003
#define COFFEESCRIPT_SCRIPTENABLE	1006

#define COFFEESCRIPT_CONTAINER 65536	// + language identification

#define COFFEESCRIPT_SCRIPTNAME	1
#define COFFEESCRIPT_SCRIPTHELP	2

#define MSG_SCRIPT_EXECUTE				1001184	// no arguments
#define MSG_SCRIPT_RETRIEVEBITMAP	1001185	// pass pointer to bitmap pointer

#define PYTHONSCRIPT_TEXT					1000
#define PYTHONSCRIPT_SHOWINMENU		1002
#define PYTHONSCRIPT_ADDEVENT			1003
#define PYTHONSCRIPT_SCRIPTENABLE	1006

#define PYTHONSCRIPT_CONTAINER 65536	// + language identification

#define PYTHONSCRIPT_SCRIPTNAME	1
#define PYTHONSCRIPT_SCRIPTHELP	2

#define BASEDRAW_DRAWPORTTYPE			 1888
#define BASEDRAW_IS_SHADOWPASS		 1889
#define BASEDRAW_IS_RENDERASEDITOR 1890
#define BASEDRAW_IS_OGL_PREPASS		 1891
#define BASEDRAW_IS_PICK_OBJECT		 1892

enum MULTIPASSCHANNEL
{
	MULTIPASSCHANNEL_0							 = 0,
	MULTIPASSCHANNEL_IMAGELAYER			 = (1 << 0),
	MULTIPASSCHANNEL_MATERIALCHANNEL = (1 << 1)
} ENUM_END_LIST(MULTIPASSCHANNEL);

enum DLG_TYPE
{
	DLG_TYPE_MODAL = 10,
	DLG_TYPE_MODAL_RESIZEABLE,

	DLG_TYPE_ASYNC = 20,
	DLG_TYPE_ASYNC_POPUP_RESIZEABLE,
	DLG_TYPE_ASYNC_POPUPEDIT,

	DLG_TYPE_ASYNC_FULLSCREEN_WORK = 30,
	DLG_TYPE_ASYNC_FULLSCREEN_MONITOR,

	DLG_TYPE_
} ENUM_END_LIST(DLG_TYPE);

enum MULTIMSG_ROUTE
{
	MULTIMSG_ROUTE_NONE			 = 0,
	MULTIMSG_ROUTE_UP				 = 1,
	MULTIMSG_ROUTE_ROOT			 = 2,
	MULTIMSG_ROUTE_DOWN			 = 3,
	MULTIMSG_ROUTE_BROADCAST = 4
} ENUM_END_LIST(MULTIMSG_ROUTE);

enum VPGETFRAGMENTS
{
	VPGETFRAGMENTS_0	 = 0,
	VPGETFRAGMENTS_Z_P = (1 << 0),
	VPGETFRAGMENTS_N	 = (1 << 1)
} ENUM_END_FLAGS(VPGETFRAGMENTS);

#define MSG_GICSEX					 1000969
#define MSG_GINEW						 1021096
#define ID_OLDCAUSTICS			 1000970
#define VPglobalillumination 1021096
#define VPGIShadingChain		 1026950
#define VPAOShadingChain		 1029427

enum SIGNALMODE
{
	SIGNALMODE_DEFAULT	= 0,
	SIGNALMODE_RESERVED	= 1
} ENUM_END_LIST(SIGNALMODE);

enum QUALIFIER
{
	QUALIFIER_0				 = 0,
	QUALIFIER_SHIFT		 = (1 << 0),
	QUALIFIER_CTRL		 = (1 << 1),
	QUALIFIER_MOUSEHIT = (1 << 10)
} ENUM_END_FLAGS(QUALIFIER);

#define CODEEDITOR_SETMODE				 'setm'
#define CODEEDITOR_GETSTRING			 'gets'
#define CODEEDITOR_SETSTRING			 'sets'
#define CODEEDITOR_COMPILE				 'comp'
#define CODEEDITOR_GETERROR_RES		 'resr'
#define CODEEDITOR_GETERROR_STRING 'ress'
#define CODEEDITOR_GETERROR_LINE	 'resl'
#define CODEEDITOR_GETERROR_POS		 'resp'
#define CODEEDITOR_EXECUTE				 'exec'
#define CODEEDITOR_DISABLEUNDO		 'dsud'


enum
{
	DIALOG_PIN = 1,											// Int32 flags
	DIALOG_CHECKBOX,										// Int32 id, Int32 flags, String *name
	DIALOG_STATICTEXT,									// Int32 id, Int32 flags, String *name, Int32 borderstyle
	DIALOG_BUTTON,											// Int32 id, Int32 flags, String *name
	DIALOG_ARROWBUTTON,									// Int32 id, Int32 flags, Int32 arrowtype
	DIALOG_EDITTEXT,										// Int32 id, Int32 flags
	DIALOG_EDITNUMBER,									// Int32 id, Int32 flags
	DIALOG_EDITNUMBERUD,								// Int32 id, Int32 flags
	DIALOG_EDITSLIDER,									// Int32 id, Int32 flags
	DIALOG_SLIDER,											// Int32 id, Int32 flags
	DIALOG_COLORFIELD,									// Int32 id, Int32 flags
	DIALOG_COLORCHOOSER,								// Int32 id, Int32 flags
	DIALOG_USERAREA,										// Int32 id, Int32 flags
	DIALOG_RADIOGROUP,									// Int32 id, Int32 flags
	DIALOG_COMBOBOX,										// Int32 id, Int32 flags
	DIALOG_POPUPBUTTON,									// Int32 id, Int32 flags
	DIALOG_CHILD,												// Int32 id, Int32 subid, String *child);
	DIALOG_FREECHILDREN,								// Int32 id
	DIALOG_DLGGROUP,										// Int32 flags
	DIALOG_SETTITLE,										// String *name
	DIALOG_GROUPSPACE,									// spacex,spacey
	DIALOG_GROUPBORDER,									// borderstyle
	DIALOG_GROUPBORDERSIZE,							// left, top, right, bottom
	DIALOG_SETIDS,											// left, top, right, bottom
	DIALOG_LAYOUTCHANGED,								// relayout dialog
	DIALOG_ACTIVATE,										// activate gadget
	DIALOG_ADDSUBMENU,									// add submenu, text
	DIALOG_ENDSUBMENU,									// add submenu, text
	DIALOG_ADDMENUCMD,									// add menutext, text, cmdid
	DIALOG_FLUSHMENU,										// delete all menu entries
	DIALOG_INIT,												// create new layout
	DIALOG_CHECKNUMBERS,								// range check of all dialog elements
	DELME_DIALOG_SETGROUP,							// set group as insert group
	DIALOG_FLUSHGROUP,									// flush all elements of this group and set insert point to this group
	DIALOG_SETMENU,											// set and display menu in manager
	DIALOG_SCREEN2LOCALX,								// Screen2Local coordinates
	DIALOG_SCREEN2LOCALY,								// Screen2Local coordinates
	DIALOG_ADDMENUSTR,									// add menutext, text, id
	DIALOG_RADIOBUTTON,									// Int32 id, Int32 flags, String *name
	DIALOG_ADDMENUSEP,									// add menu separator
	DIALOG_SEPARATOR,										// add separator
	DIALOG_MULTILINEEDITTEXT,						// add multiline editfield
	DIALOG_INITMENUSTR,									// enable/disable/check/uncheck menustrings
	DIALOG_RADIOTEXT,
	DIALOG_MENURESOURCE,								// private for painter
	DIALOG_LISTVIEW,										// Int32 id, Int32 flags
	DIALOG_SUBDIALOG,										// Int32 id, Int32 flags
	DIALOG_CHECKCLOSE,									// CheckCloseMessage for dialog
	DIALOG_GETTRISTATE,									// get TriState-flag of the specified gadget
	DIALOG_SDK,													// Int32 id, Int32 flags, String *name
	DIALOG_SCROLLGROUP,									// create ScrollGroup
	DIALOG_ISOPEN,											// returns true if the dialog is open
	DIALOG_REMOVEGADGET,								// removes an element from the layout
	DIALOG_MENUGROUPBEGIN,
	DIALOG_NOMENUBAR,										// removes the menubar
	DIALOG_SAVEWEIGHTS,									// store the weights of a group
	DIALOG_LOADWEIGHTS,									// restore the weights of a group
	DIALOG_EDITSHORTCUT,								// editfield to enter shortcuts
	DIALOG_ISVISIBLE,										// returns true if the dialog is visible (e.g. false if tabbed behind)
	DIALOG_HIDEELEMENT,									// true - hides the element
	DIALOG_SETDEFAULTCOLOR,							// set the mapcolor for an color to anotehr value e.g. COLOR_BG = 1,1,1
	DIALOG_COMBOBUTTON,									// ComboButton
	DIALOG_PRIVATE_NOSTOPEDITORTHREADS,	// no params

	DIALOG_
};

#define EDITTEXT_PASSWORD	(1 << 0)	// creates a passwordfield
#define EDITTEXT_HELPTEXT	(1 << 1)	// sets the helptext for an editfield, this text appears if the editfield is empty

enum
{
	LV_GETLINECOUNT						 = 1,	// request the number of lines of the listview
	LV_GETCOLUMNCOUNT					 = 2,	// request the number of columns of listview
	LV_GETLINEHEIGHT					 = 3,	// ask for the line height of the specific 'line'
	LV_GETCOLUMNWIDTH					 = 4,	// ask for the width of the specific 'column' in 'line'
	LV_GETCOLUMTYPE						 = 5,	// ask for the type of the column in 'line',
	LV_COLUMN_TEXT						 = C4D_FOUR_BYTE(0, 't', 'x', 't'),
	LV_COLUMN_EDITTEXT				 = C4D_FOUR_BYTE(0, 'e', 'd', 't'),
	LV_COLUMN_BMP							 = C4D_FOUR_BYTE(0, 'b', 'm', 'p'),
	LV_COLUMN_CHECKBOX				 = C4D_FOUR_BYTE(0, 'c', 'h', 'k'),
	LV_COLUMN_BUTTON					 = C4D_FOUR_BYTE(0, 'b', 't', 'n'),
	LV_COLUMN_USERDRAW				 = C4D_FOUR_BYTE(0, 'u', 's', 'r'),
	LV_COLUMN_COLORVIEW				 = C4D_FOUR_BYTE(0, 'c', 'l', 'v'),

	LV_GETCOLUMDATA						 = 6,		// ask for the data of the column in 'line',
	LV_GETLINESELECTED				 = 7,		// ask if the line is selected
	LV_GETCOLSPACE						 = 8,		// ask for space in pixels between two columns
	LV_GETLINESPACE						 = 9,		// ask for space in pixels between two lines
	LV_GETFIXEDLAYOUT					 = 10,	// ask for fixed layout: false...indiv. layout, true...fixed layout
	LV_DESTROYLISTVIEW				 = 11,	// destroy listview and all data
	LV_INITCACHE							 = 12,	// (internal) before call the listview
	LV_NOAUTOCOLUMN						 = 13,	// ask for fast layout: false...eachline is ask for the width, true...only the first line is asked for the columnwidth -> huge speedup

	LV_LMOUSEDOWN							 = 50,	// mouse down at line, col,
	LV_ACTION									 = 51,	// gadget command, col, data1 = msg,
	LV_USERDRAW								 = 52,
	LV_REDRAW									 = 53,	// redraw the listview (supermessage)
	LV_DATACHANGED						 = 54,	// layout data has changed
	LV_SHOWLINE								 = 55,	// scroll line into the visible area
	LV_DRAGRECEIVE						 = 56,	// drag receive
	LV_RMOUSEDOWN							 = 57,	// mouse down at line, col,

	LV_SIMPLE_SELECTIONCHANGED = 100,	// simplelistview: selection changed
	LV_SIMPLE_CHECKBOXCHANGED	 = 101,	// simplelistview: checkbox changed
	LV_SIMPLE_FOCUSITEM				 = 102,	// simplelistview: set focus to item
	LV_SIMPLE_BUTTONCLICK			 = 103,	// simplelistview: button click
	LV_SIMPLE_ITEM_ID					 = 1,
	LV_SIMPLE_COL_ID					 = 2,
	LV_SIMPLE_DATA						 = 3,
	LV_SIMPLE_DOUBLECLICK			 = 104,	// doubleclick occured
	LV_SIMPLE_FOCUSITEM_NC		 = 105,	// focus item, but no change
	LV_SIMPLE_RMOUSE					 = 106,
	LV_SIMPLE_USERDRAW				 = 107,

	// result types
	LV_RES_INT		= 'long',
	LV_RES_BITMAP = C4D_FOUR_BYTE(0, 'b', 'm', 'p'),
	LV_RES_STRING = 'strg',
	LV_RES_VECTOR = C4D_FOUR_BYTE(0, 'v', 'e', 'c'),
	LV_RES_NIL		= C4D_FOUR_BYTE(0, 'n', 'i', 'l'),

	LV__
};

#ifndef C4D_GL_VARS_DEFINED
enum GlVertexBufferSubBufferType { VBArrayBuffer = 0, VBElementArrayBuffer16 = 1, VBElementArrayBuffer32 = 2 };
enum GlVertexBufferAccessFlags { VBReadWrite = 0, VBReadOnly = 1, VBWriteOnly = 2 };

	#if defined __PC
typedef	UINT C4DGLuint;
typedef INT C4DGLint;
	#elif defined __MAC
typedef unsigned int C4DGLuint;
typedef int	C4DGLint;
	#elif defined __LINUX
typedef	UINT C4DGLuint;
typedef INT C4DGLint;
	#endif

typedef Int GlProgramParameter;
	#define C4D_GL_VARS_DEFINED
#endif

#ifndef _C4D_GL_H_
enum GlProgramType { VertexProgram = 1, FragmentProgram = 2, CompiledProgram = 3, GeometryProgram = 4 };
enum GlUniformParamType { UniformFloat1					 = 0, UniformFloat2 = 1, UniformFloat3 = 2, UniformFloat4 = 3,
													UniformInt1						 = 4, UniformInt2 = 5, UniformInt3 = 6, UniformInt4 = 7,
													UniformUint1					 = 8, UniformUint2 = 9, UniforUint3 = 10, UniformUint4 = 11,
													UniformFloatMatrix2		 = 12, UniformFloatMatrix3 = 13, UniformFloatMatrix4 = 14,
													UniformTexture1D			 = 15, UniformTexture2D = 16, UniformTexture3D = 17, UniformTextureCube = 18,
													UniformTexture1Di			 = 19, UniformTexture2Di = 20, UniformTexture3Di = 21, UniformTextureCubei = 22,
													UniformTexture1Du			 = 23, UniformTexture2Du = 24, UniformTexture3Du = 25, UniformTextureCubeu = 26,
													UniformTexture1DArray	 = 27, UniformTexture2DArray = 28,
													UniformTexture1DArrayi = 29, UniformTexture2DArrayi = 30,
													UniformTexture1DArrayu = 31, UniformTexture2DArrayu = 32 };
#endif

typedef UChar PIX;

enum NOTIFY_EVENT
{
	NOTIFY_EVENT_NONE				 = -1,
	NOTIFY_EVENT_ALL				 = 10,
	NOTIFY_EVENT_ANY				 = 11,
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_PRE_DEFORM	 = 100,
	NOTIFY_EVENT_POST_DEFORM = 101,
	NOTIFY_EVENT_UNDO				 = 102,
	NOTIFY_EVENT_MESSAGE		 = 103,	// NotifyEventMsg
	NOTIFY_EVENT_FREE				 = 104,
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_COPY				 = 107,
	NOTIFY_EVENT_CACHE			 = 108,
	NOTIFY_EVENT_REMOVE			 = 109,
	NOTIFY_EVENT_CLONE			 = 110,
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_SETNAME		 = 200,
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_0					 = 0
} ENUM_END_LIST(NOTIFY_EVENT);

enum NOTIFY_EVENT_FLAG
{
	NOTIFY_EVENT_FLAG_REMOVED				 = (1 << 0),	// PRIVATE
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_FLAG_COPY_UNDO			 = (1 << 10),
	NOTIFY_EVENT_FLAG_COPY_CACHE		 = (1 << 11),
	NOTIFY_EVENT_FLAG_COPY_DUPLICATE = (1 << 12),
	NOTIFY_EVENT_FLAG_ONCE					 = (1 << 13),
	NOTIFY_EVENT_FLAG_COPY					 = ((1 << 10) | (1 << 11) | (1 << 12)),
	//////////////////////////////////////////////////////////////////////////
	NOTIFY_EVENT_FLAG_0							 =0
} ENUM_END_FLAGS(NOTIFY_EVENT_FLAG);

enum DESCIDSTATE
{
	DESCIDSTATE_0			 = 0,
	DESCIDSTATE_LOCKED = 1 << 0,
	DESCIDSTATE_HIDDEN = 1 << 1
} ENUM_END_FLAGS(DESCIDSTATE);

enum BASEDRAW_HOOK_MESSAGE
{
	BASEDRAW_MESSAGE_ADAPTVIEW				= 1,
	BASEDRAW_MESSAGE_SET_SCENE_CAMERA	= 2,
	BASEDRAW_MESSAGE_DELETEBASEDRAW		= 3
} ENUM_END_LIST(BASEDRAW_HOOK_MESSAGE);

enum CINEMAINFO
{
	CINEMAINFO_TABLETT			 = 4,
	CINEMAINFO_OPENGL				 = 7,
	CINEMAINFO_TABLETT_HIRES = 8,
	CINEMAINFO_FORBID_GUI		 = 9,
	CINEMAINFO_FORBID_OGL		 = 10,
	CINEMAINFO_LISTEN				 = 11,
	CINEMAINFO_WATCH_PID		 = 12,
	CINEMAINFO_SETFOREGROUND = 13
} ENUM_END_FLAGS(CINEMAINFO);

//IpAddr
enum PROTOCOL
{
	PROTOCOL_ZERO = 0,
	PROTOCOL_IPV4	=	1000,
	PROTOCOL_IPV6
} ENUM_END_LIST(PROTOCOL);

enum PROXYTYPE
{
	PROXYTYPE_HTML,
	PROXYTYPE_SOCKS4,
	PROXYTYPE_SOCKS5
} ENUM_END_LIST(PROXYTYPE);

enum RESOLVERESULT
{
	RESOLVERESULT_OK				=0,
	RESOLVERESULT_RETRY			=2,	// temporary failure, a retry might succeed
	RESOLVERESULT_SETUP			=3,	// internal failure (should not happen though)
	RESOLVERESULT_UNKNOWN		=4,	// unknown error while resolving address
	RESOLVERESULT_ERRINT		=5,	// the interface family is not supported
	RESOLVERESULT_NONAME		=6,	// could not resolve request, no name found
	RESOLVERESULT_SERVICE		=7,	// internal failure (pServicename parameter not supported)
	RESOLVERESULT_SOCKETTYPE=8,	// internal failure (ai_socktype not supported)
	RESOLVERESULT_UNKNOWNINT=9	// could resolve name but not for the requested interface
} ENUM_END_LIST(RESOLVERESULT);

enum SERVERJOBLIST
{
	SERVERJOBLIST_INACTIVE=1000,
	SERVERJOBLIST_ACTIVE,
	SERVERJOBLIST_DOWNLOAD,
	SERVERJOBLIST_ALL
} ENUM_END_LIST(SERVERJOBLIST);

enum JOBCOMMAND
{
	JOBCOMMAND_NONE = 1000,	// do nothing
	JOBCOMMAND_FETCHJOB,
	JOBCOMMAND_ALLOCATESPACE,
	JOBCOMMAND_DOWNLOAD,
	JOBCOMMAND_RENDER,
	JOBCOMMAND_DELETE,
	JOBCOMMAND_STOPANDDELETE,
	JOBCOMMAND_ASSEMBLE,
	JOBCOMMAND_END
} ENUM_END_LIST(JOBCOMMAND);

enum RENDERTARGET						// BaseContainer ID
{
	RENDERTARGET_ALL = 1000,	// use all machines
	RENDERTARGET_SPECIFIED,		// uuids
	RENDERTARGET_MINMAX				// (1000;min) (1001;max)
} ENUM_END_LIST(RENDERTARGET);

enum JOBSTATE
{
	JOBSTATE_IDLE = 1000,

	// preparing only for server in async mode for StartRendering
	JOBSTATE_PREPARING_RUNNING,
	JOBSTATE_PREPARING_FAILED,
	JOBSTATE_PREPARING_OK,

	JOBSTATE_RENDER_RUNNING,
	JOBSTATE_RENDER_PAUSED,
	JOBSTATE_RENDER_OK,
	JOBSTATE_RENDER_FAILED,

	JOBSTATE_ALLOCATESPACE_RUNNING,
	JOBSTATE_ALLOCATESPACE_OK,
	JOBSTATE_ALLOCATESPACE_FAILED,

	JOBSTATE_DOWNLOAD_RUNNING,
	JOBSTATE_DOWNLOAD_OK,
	JOBSTATE_DOWNLOAD_FAILED,

	JOBSTATE_ASSEMBLE_RUNNING,
	JOBSTATE_ASSEMBLE_OK,
	JOBSTATE_ASSEMBLE_FAILED,

	JOBSTATE_STOPPED
} ENUM_END_LIST(JOBSTATE);

enum ZEROCONFMACHINESTATE
{
	ZEROCONFMACHINESTATE_ONLINE	 = 1,
	ZEROCONFMACHINESTATE_OFFLINE = 2,
	ZEROCONFMACHINESTATE_REMOVED = 3,
	ZEROCONFMACHINESTATE_UPDATE	 = 4,
} ENUM_END_LIST(ZEROCONFMACHINESTATE);

enum ZEROCONFACTION
{
	ZEROCONFACTION_0			 = 0,
	ZEROCONFACTION_RESOLVE = (1 << 0),
	ZEROCONFACTION_MONITOR = (1 << 1)
} ENUM_END_FLAGS(ZEROCONFACTION);

enum ZEROCONFERROR
{
	ZEROCONFERROR_NOERROR										= 0,
	ZEROCONFERROR_UNKNOWN										= -65537,	/* 0xFFFE FFFF */
	ZEROCONFERROR_NOSUCHNAME								= -65538,
	ZEROCONFERROR_NOMEMORY									= -65539,
	ZEROCONFERROR_BADPARAM									= -65540,
	ZEROCONFERROR_BADREFERENCE							= -65541,
	ZEROCONFERROR_BADSTATE									= -65542,
	ZEROCONFERROR_BADFLAGS									= -65543,
	ZEROCONFERROR_UNSUPPORTED								= -65544,
	ZEROCONFERROR_NOTINITIALIZED						= -65545,
	ZEROCONFERROR_ALREADYREGISTERED					= -65547,
	ZEROCONFERROR_NAMECONFLICT							= -65548,
	ZEROCONFERROR_INVALID										= -65549,
	ZEROCONFERROR_FIREWALL									= -65550,
	ZEROCONFERROR_INCOMPATIBLE							= -65551,	/* Client Library Incompatible with daemon */
	ZEROCONFERROR_BADINTERFACEINDEX					= -65552,
	ZEROCONFERROR_REFUSED										= -65553,
	ZEROCONFERROR_NOSUCHRECORD							= -65554,
	ZEROCONFERROR_NOAUTH										= -65555,
	ZEROCONFERROR_NOSUCHKEY									= -65556,
	ZEROCONFERROR_NATTRAVERSAL							= -65557,
	ZEROCONFERROR_DOUBLENAT									= -65558,
	ZEROCONFERROR_BADTIME										= -65559,	/* Codes up to here existed in Tiger */
	ZEROCONFERROR_BADSIG										= -65560,
	ZEROCONFERROR_BADKEY										= -65561,
	ZEROCONFERROR_TRANSIENT									= -65562,
	ZEROCONFERROR_SERVICENOTRUNNING					= -65563,	/* Background daemon not running */
	ZEROCONFERROR_NATPORTMAPPINGUNSUPPORTED = -65564,	/* NAT doesnt't support NAT_PMP or UPNP */
	ZEROCONFERROR_NATPORTMAPPINGDISABLED		= -65565,	/* NAT supports NAT-PMP or UPNP but it's disabled by the administrator */
	ZEROCONFERROR_NOROUTER									= -65566,	/* No router currently configured (probably no network connectivity) */
	ZEROCONFERROR_POLLINGMODE								= -65567
} ENUM_END_LIST(ZEROCONFERROR);

#define RENDERSETTING_STATICTAB_OUTPUT			 1
#define RENDERSETTING_STATICTAB_SAVE				 2
#define RENDERSETTING_STATICTAB_MULTIPASS		 3
#define RENDERSETTING_STATICTAB_ANTIALIASING 4
#define RENDERSETTING_STATICTAB_OPTIONS			 5
#define RENDERSETTING_STATICTAB_STEREO			 6
#define RENDERSETTING_STATICTAB_NET					 7

#endif
