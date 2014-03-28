/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __C4DFILE_H
#define __C4DFILE_H

#include "ge_math.h"

#ifdef __API_INTERN__
	#include "ge_file.h"
#else

#include "c4d_memory.h"
#include "c4d_string.h"

class Filename;
class HyperFile;
class BaseContainer;
class BaseDocument;
class BaseBitmap;
class BaseChannel;

class MemoryFileStruct
{
	private:
		MemoryFileStruct();
		~MemoryFileStruct();
	public:
		static MemoryFileStruct* Alloc();
		static void Free(MemoryFileStruct *&mfs);

		void GetData(void *&data, VLONG &size, Bool release = FALSE);
};

class Filename
{
	private:
		C4D_RESERVE_PRIVATE_TYPE(String,dummy1);
		C4D_RESERVE_PRIVATE_TYPE(void*,dummy2);
		C4D_RESERVE_PRIVATE_TYPE(VLONG,dummy3);
		C4D_RESERVE_PRIVATE_TYPE(Int32,dummy4);
		
	public:

		#ifdef	__C4D_NO_NEW_DELETE__	
		void* operator new(size_t,const std::nothrow_t&,int line,const Char *file) throw();
		void  operator delete(void *del);
		void  operator delete(void *del,int line, const Char *file);

		void* operator new(size_t,void *place) { return place; }
		void  operator delete(void*, void*) {}
		#endif

		Filename(void);
		Filename(const Char *string);
		Filename(const String &string);
		Filename(const Filename &src);
		~Filename(void);

		void CopyTo(Filename *dst) const;

		Bool FileSelect(FILESELECTTYPE type, FILESELECT flags, const String &title, const String &force_suffix=String());
		
		Bool Content(void) const;

		String GetString(void) const;
		void SetString(const String &str);
		
		const Filename GetDirectory(void) const;
		const Filename GetFile(void) const;
		String GetFileString(void) const;
		
		void ClearSuffix(void);
		void ClearSuffixComplete(void);
		void SetSuffix(const String &str);
		Bool CheckSuffix(const String &str) const;

		void SetDirectory(const Filename &str);
		void SetFile(const Filename &str);

		void SetMemoryReadMode(void *adr, VLONG size = -1);
		void SetMemoryWriteMode(MemoryFileStruct *mfs);

		void SetIpConnection(IpConnection *ipc);
		IpConnection *GetIpConnection() const;

		const Filename& 			operator =  (const Filename &fname);
		friend const Filename	operator +  (const Filename &fname1,const Filename &fname2);
		const Filename&				operator += (const Filename &fname);
		Bool									operator == (const Filename &fname) const;
		Bool									operator != (const Filename &fname) const;
};

class BaseFile
{
	private:
		BaseFile();
		~BaseFile();
	public:
		// Open a file, name is of type filename
		// mode					: read, write or readwrite
		// error_dialog	: display any errors in dialogs
		// order				:	little or big endian
		// type,creator	: MAC file types
		Bool Open(const Filename &name, FILEOPEN mode=FILEOPEN_READ, FILEDIALOG error_dialog=FILEDIALOG_IGNOREOPEN, BYTEORDER order=BYTEORDER_MOTOROLA, Int32 type=MACTYPE_CINEMA, Int32 creator=MACCREATOR_CINEMA);
		
		// Closes a file - automatically called when BaseFile is destructed
		Bool Close();	

		// Change byte order while reading - little or big endian allowed as parameter
		void SetOrder(BYTEORDER order);
		
		// Read len bytes
		VLONG ReadBytes(void *data, VLONG len, Bool just_try_it=FALSE);

		// Write len bytes
		Bool WriteBytes(const void *data, VLONG len);

		// Tries to read len bytes - if less bytes were read no error state is set
		// the number of read bytes is returned
		VLONG TryReadBytes(void *data, VLONG len);

		// Seek - returns error
		Bool Seek(LLONG pos, FILESEEK mode=FILESEEK_RELATIVE);
		
		// Return actual file position
		LLONG GetPosition();

		// Return file length
		LLONG GetLength();
		
		// Return file error
		FILEERROR GetError() const;

		// Manually set file error
		void SetError(FILEERROR error);  

		Bool ReadChar (Char  *v);
		Bool ReadUChar(UChar *v);
		Bool ReadWord (SWORD  *v);
		Bool ReadUWord(UWORD *v);
		Bool ReadLong (Int32  *v);
		Bool ReadULong(ULONG *v);
		Bool ReadSReal(Float32 *v);
		Bool ReadLReal(LReal *v);
		Bool ReadLLong(LLONG *v);
		Bool ReadFilename(Filename *v);
		Bool ReadBool(Bool *v);
		Bool ReadString(String *v);
		Bool ReadSVector(SVector *v);
		Bool ReadLVector(LVector *v);
		Bool ReadSMatrix(SMatrix *v);
		Bool ReadLMatrix(LMatrix *v);

		Bool WriteChar (Char  v);
		Bool WriteUChar(UChar v);
		Bool WriteWord (SWORD  v);
		Bool WriteUWord(UWORD v);
		Bool WriteLong (Int32  v);
		Bool WriteULong(ULONG v);
		Bool WriteSReal(Float32 v);
		Bool WriteLReal(LReal v);
		Bool WriteLLong(LLONG v);
		Bool WriteFilename(const Filename &v);
		Bool WriteBool(Bool  v);
		Bool WriteString(const String &v);
		Bool WriteSVector(const SVector &v);
		Bool WriteLVector(const LVector &v);
		Bool WriteSMatrix(const SMatrix &v);
		Bool WriteLMatrix(const LMatrix &v);

		static BaseFile *Alloc(void);
		static void Free(BaseFile *&fl);
};

class AESFile : public BaseFile
{
private:
	AESFile();
	~AESFile();
public:
	// Open a file, name is of type filename
	// mode					: read, write or readwrite
	// error_dialog	: display any errors in dialogs
	// order				:	little or big endian
	// type,creator	: MAC file types
	Bool Open(const Filename &name, const char* key, Int32 keylen, Int32 blocksize, ULONG aes_flags, FILEOPEN mode=FILEOPEN_READ, FILEDIALOG error_dialog=FILEDIALOG_IGNOREOPEN, BYTEORDER order=BYTEORDER_MOTOROLA, Int32 type=MACTYPE_CINEMA, Int32 creator=MACCREATOR_CINEMA);

	// checks if encrypt is the encrypted version of decrypt
	static Bool CheckEncryption(const Filename& encrypt, const Filename& decrypt, const char* key, Int32 keylen, Int32 blocksize);

	static AESFile *Alloc(void);
	static void Free(AESFile *&fl);
};

class LocalFileTime
{
	public:
		UWORD	year;
		UWORD	month;
		UWORD	day;
		UWORD	hour;
		UWORD	minute;
		UWORD	second;

	void Init(void)
	{
		year=month=day=hour=minute=second=0;
	}

	// > 0: t0 > t1
	// = 0: t0 == t1
	// < 0: t0 < t1
	Int32	Compare( const LocalFileTime &t0, const LocalFileTime &t1 )
	{
		Int32	result;
		
		result = t0.year - t1.year;
		if ( result == 0 )
		{
			result = t0.month - t1.month;
			if ( result == 0 )
			{
				result = t0.day - t1.day;
				if ( result == 0 )
				{
					result = t0.hour - t1.hour;
					if ( result == 0 )
					{
						result = t0.minute - t1.minute;
						if ( result == 0 )
							result = t0.second - t1.second;
					}
				}
			}
		}
		return( result );
	}

	Bool operator == (const LocalFileTime &x)
	{
		return year==x.year && month==x.month && day==x.day && hour==x.hour && minute==x.minute && second==x.second;
	}

	Bool operator != (const LocalFileTime &x)
	{
		return year!=x.year || month!=x.month || day!=x.day || hour!=x.hour || minute!=x.minute || second!=x.second;
	}

	Bool operator > (const LocalFileTime &x)
	{
		return( Compare( *this, x ) > 0 );
	}

	Bool operator < (const LocalFileTime &x)
	{
		return( Compare( *this, x ) < 0 );
	}

	Bool operator >= (const LocalFileTime &x)
	{
		return( Compare( *this, x ) >= 0 );
	}

	Bool operator <= (const LocalFileTime &x)
	{
		return( Compare( *this, x ) <= 0 );
	}
};

#define	GE_FILETIME_CREATED		0
#define	GE_FILETIME_MODIFIED	1
#define	GE_FILETIME_ACCESS		2

#define BROWSEFILES_CALCSIZE				(1<<0)
#define BROWSEFILES_SUPPRESSCACHING	(1<<1)

class BrowseFiles
{
	private:
		BrowseFiles();
		~BrowseFiles();
	public:
		void Init(const Filename &directory, Int32 flags);
		Bool GetNext(void);		
		
		LLONG GetSize(void);
		Bool IsDir(void);
		Bool IsHidden(void);
		Bool IsBundle(void);
		Bool IsReadOnly(void);

		void GetFileTime(Int32 mode, LocalFileTime *out);

		Filename GetFilename(void);

		static BrowseFiles *Alloc();
		static void Free(BrowseFiles *&bf);
};

#define BROWSEVOLUMES_VOLUME_NOT_AVAILABLE		( 1 << 7 )

class BrowseVolumes
{
	private:
		BrowseVolumes();
		~BrowseVolumes();
	public:
		void Init(void);
		Bool GetNext(void);		
		Filename	GetFilename(void);
		String		GetVolumeName( Int32 *out_flags );

		static BrowseVolumes *Alloc();
		static void Free(BrowseVolumes *&bf);
};

class HyperFile
{
	private:
		HyperFile();
		~HyperFile();
	public:
		Bool WriteChar(Char  v);
		Bool WriteUChar(UChar v);
		Bool WriteWord(SWORD  v);
		Bool WriteUWord(UWORD v);
		Bool WriteLong(Int32  v);
		Bool WriteULong(ULONG v);
		Bool WriteLLong(LLONG v);
		Bool WriteReal(Float  v);
		Bool WriteSReal(Float32 v);
		Bool WriteLReal(LReal v);
		Bool WriteBool(Bool  v);
		Bool WriteTime(const BaseTime &v);
		Bool WriteVector(const Vector  &v);
		Bool WriteSVector(const SVector  &v);
		Bool WriteLVector(const LVector  &v);
		Bool WriteMatrix(const Matrix  &v);
		Bool WriteSMatrix(const SMatrix  &v);
		Bool WriteLMatrix(const LMatrix  &v);
		Bool WriteString(const String &v);
		Bool WriteFilename(const Filename &v);
		Bool WriteImage(BaseBitmap *v, Int32 format, BaseContainer *data, SAVEBIT savebits=SAVEBIT_ALPHA); 
		Bool WriteGeData(const GeData &v);
		Bool WriteContainer(const BaseContainer &v);
		Bool WriteMemory(const void *data, VLONG count);
		Bool WriteChannel(BaseChannel *bc);
		Bool WriteArray(const void *data, HYPERFILEARRAY datatype, Int32 structure_increment, Int32 count);

		Bool ReadChar(Char *v);
		Bool ReadUChar(UChar *v);
		Bool ReadWord(SWORD *v);
		Bool ReadUWord(UWORD *v);
		Bool ReadLong(Int32 *v);
		Bool ReadULong(ULONG *v);
		Bool ReadLLong(LLONG *v);
		Bool ReadReal(Float *v);
		Bool ReadSReal(Float32 *v);
		Bool ReadLReal(LReal *v);
		Bool ReadBool(Bool *v);
		Bool ReadTime(BaseTime *v);
		Bool ReadVector(Vector *v);
		Bool ReadSVector(SVector *v);
		Bool ReadLVector(LVector *v);
		Bool ReadMatrix(Matrix *v);
		Bool ReadSMatrix(SMatrix *v);
		Bool ReadLMatrix(LMatrix *v);
		Bool ReadString(String *v);
		Bool ReadFilename(Filename *v);
		Bool ReadImage(BaseBitmap *v);
		Bool ReadGeData(GeData *v);
		Bool ReadContainer(BaseContainer *v, Bool flush);
		Bool ReadMemory(void **data, VLONG *size);
		Bool ReadChannel(BaseChannel *bc);
		Bool ReadChannelConvert(GeListNode *node, Int32 link_id);
		Bool ReadArray(void *data, HYPERFILEARRAY type, Int32 structure_increment, Int32 count);

		FILEERROR GetError() const;
		void SetError(FILEERROR err);
		Bool ReadValueHeader(HYPERFILEVALUE *h);
		Bool SkipValue(HYPERFILEVALUE h); 
		Bool WriteChunkStart(Int32 id, Int32 level);
		Bool WriteChunkEnd(void);
		Bool ReadChunkStart(Int32 *id, Int32 *level);
		Bool ReadChunkEnd(void);
		Bool SkipToEndChunk(void); 

		Int32 GetFileVersion() const;

		BaseDocument *GetDocument() const;

		static HyperFile *Alloc(void);
		static void Free(HyperFile *&fl);
		Bool Open(Int32 ident, const Filename &filename, FILEOPEN mode, FILEDIALOG error_dialog);
		Bool Close();
};

FILEERROR ReadHyperFile(BaseDocument *doc, GeListNode *node, const Filename &filename, Int32 ident, String *warning_string);
FILEERROR WriteHyperFile(BaseDocument *doc, GeListNode *node, const Filename &filename, Int32 ident);

// file IO
#define	GE_FKILL_DIRECTORY	1
#define	GE_FKILL_RECURSIVE	2
#define	GE_FKILL_FORCE			4

#define GE_FCOPY_OVERWRITE					1
#define GE_FCOPY_DONTCOPYREADONLY		2

#define GE_FILE_ATTRIBUTE_READONLY				0x00000001
#define GE_FILE_ATTRIBUTE_HIDDEN					0x00000002
#define GE_FILE_ATTRIBUTE_LOCKED					0x00000010 // only for Mac, ignored on Windows
#define GE_FILE_ATTRIBUTE_OWNER_R					0x00000100 // unix attributes
#define GE_FILE_ATTRIBUTE_OWNER_W					0x00000200
#define GE_FILE_ATTRIBUTE_OWNER_X					0x00000400
#define GE_FILE_ATTRIBUTE_GROUP_R					0x00000800
#define GE_FILE_ATTRIBUTE_GROUP_W					0x00001000
#define GE_FILE_ATTRIBUTE_GROUP_X					0x00002000
#define GE_FILE_ATTRIBUTE_PUBLIC_R				0x00004000
#define GE_FILE_ATTRIBUTE_PUBLIC_W				0x00008000
#define GE_FILE_ATTRIBUTE_PUBLIC_X				0x00010000
#define GE_FILE_ATTRIBUTS_UNIX_MASK				0x0001ff00

Bool GeFExist    (const Filename &name, Bool isdir=FALSE);
Bool GeSearchFile(const Filename &directory, const Filename &name, Filename *found);
Bool GeFKill     (const Filename &name, Int32 flags=0);
Bool GeFCopyFile (const Filename &source, const Filename &dest, Int32 flags);
Bool GeFRename   (const Filename &source, const Filename &dest);
Bool GeFMove     (const Filename &source, const Filename &dest);
Bool GeFCreateDir(const Filename &name);
Bool GeFCreateDirRec(const Filename &name);
Bool GeExecuteFile(const Filename &path);
Bool GeExecuteProgram(const Filename &program, const Filename &file);
typedef Int32 (*GeExecuteProgramExCallback)(Int32 cmd, void *userdata, const Filename &logfile);
// callback return values:
// return 0 to continue
// return 'stop' to kill the running process
Bool GeExecuteProgramEx(const Filename &program, const String *args, Int32 argcnt, GeExecuteProgramExCallback callback, void *userdata);
Bool GeFGetDiskFreeSpace(const Filename &vol, LULONG &freecaller, LULONG &total, LULONG &freespace);
ULONG GeFGetAttributes(const Filename &name);
Bool GeFSetAttributes(const Filename &name, ULONG flags, ULONG mask = (ULONG)-1);
const Filename GeGetStartupPath(void);
const Filename GeGetStartupApplication(void);
const Filename GeGetStartupWritePath(void);
const Filename GeGetPluginPath(void);

#define C4D_PATH_PREFS					1	// c4d prefs directory
#define C4D_PATH_RESOURCE				2	// c4d resource directory
#define C4D_PATH_LIBRARY				3	// c4d library (built-in)
#define C4D_PATH_LIBRARY_USER		4 // c4d library (different if multiuser mode enabled)
#define C4D_PATH_ONLINE_HELP		5 // c4d online help directory 
#define C4D_PATH_DESKTOP				6 // OS desktop directory
#define C4D_PATH_HOME						7 // OS home directory
#define C4D_PATH_STARTUPWRITE		8 // Writeable StartupDir!
#define C4D_PATH_MYDOCUMENTS		9 // my documents path!
#define C4D_PATH_APPLICATION		10 // OS Application Directory

const Filename GeGetC4DPath(Int32 whichpath);

Bool GeGetFileTime(const Filename &name, Int32 mode, LocalFileTime *out);
Bool GeSetFileTime(const Filename &name, Int32 mode, const LocalFileTime *in);
void GeGetCurrentTime(LocalFileTime *out);

String DateToString(const LocalFileTime &t, Bool date_only);
Bool ShowInFinder(const Filename &fn, Bool open);

Bool RequestFileFromServer(const Filename &fn, Filename &res);

#endif

#endif
