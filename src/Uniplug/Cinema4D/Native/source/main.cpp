// Its possible to select values between 1000001 and 1000010 - But these are only for testing.
// Get your own plugin id on the maxon website.
#define ID_PLUGINTEMPLATE 1000001

#ifdef __MAC

#else
#include <Windows.h>
#include <TCHAR.h>
#endif

#include "c4d.h"
#include <sys/stat.h> 

#include <glib.h>
#include <mono/jit/jit.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/mono-debug.h>
#include "PluginB.h"


class newPluginTemplate : public CommandData		// data class for creating new commands
{
public:
  virtual Bool Execute(BaseDocument *doc)
  {
    MessageDialog("ManagedPlugIn");					// The message in the popup window
    GePrint("Console Output: ManagedPlugIn");		// this is output to C4D's "Console window" you can reach that with window>Console or Shift+F10
    return TRUE;
  }
};

bool FileExists(TCHAR *pFilename) 
{ 
	struct stat stFileInfo; 
	bool bReturn; 
	int intStat; 

	// Attempt to get the file attributes 
	intStat = stat(pFilename,&stFileInfo); 
	if(intStat == 0) 
	{ 
		// We were able to get the file attributes 
		// so the file obviously exists. 
		bReturn = true; 
	}
	else 
	{ 
		// We were not able to get the file attributes. 
		// This may mean that we don't have permission to 
		// access the folder which contains this file. If you 
		// need to do that level of checking, lookup the 
		// return values of stat which will give you 
		// more details on why stat failed. 
		bReturn = false; 
	} 
   
return bReturn; 
}


extern HINSTANCE g_hinstDLL;

BOOL pluginStarted = false;
BOOL useMono = false;
MonoMethod *methodStart = NULL;
MonoMethod *methodEnd = NULL;
MonoMethod *methodMessage = NULL;
MonoClass *pluginclass = NULL;
MonoObject *pluginobject = NULL;
MonoDomain *domain = NULL;
PluginB *g_pi = NULL;

Bool PluginStart(void)			// the main function C4D calls to start the plugin - nearly as a main
{
	// MessageDialog("'Cinema 4D' - Started with plugin in developement");					// The message in the popup window

	pluginStarted = true;

	bool b64 = false;

#ifdef _M_X64
	TCHAR pDir[] = _T("\\x64");
	b64 = true;
#else
	TCHAR pDir[] = _T("\\Win32");
#endif

#ifdef __MAC
	// On mac we'll use mono anyway. No MS.NET present here.
	useMono = TRUE;

#else
	// Get the "current" directory - the directory this DLL resides in.
	TCHAR pDirName[512];
	GetModuleFileName(g_hinstDLL, pDirName, 511);
	TCHAR *pBS = _tcsrchr(pDirName, (int)_T('\\'));
	if (NULL != pBS)
	{
		*pBS = _T('\0');
	}

	// Add either "\\x64" or "\\Win32" to the path
	TCHAR pPlatformDirName[512] = "";
	_tcscpy(pPlatformDirName, pDirName);
	_tcscat_s(pPlatformDirName, pDir);

	// Set the dll loading direcory
	SetDllDirectory(pPlatformDirName);


	// Decide whether to use mono or not.
	BOOL tryMono = true;

	// First try: see if there's a file named "msdotnet.cfg" - the user wants to use MS.Net
	TCHAR pRuntimeIndicator[512] = "";
	_tcscpy(pRuntimeIndicator, pDirName);
	_tcscat_s(pRuntimeIndicator, _T("\\msdotnet.cfg"));
	if (FileExists(pRuntimeIndicator))
	{
		tryMono = false;
		GePrint("There is a msdotnet.cfg - mono is disabled.");
	}


	// This might be overwritten by an environment variable
	char *sRuntime = getenv("C4D_MANAGED_RUNTIME");

	if (sRuntime != NULL)
	{
		if (0 == strncmp(sRuntime, "mono", strlen("mono")))
			tryMono = true;
		else if (0 == strncmp(sRuntime, "msdotnet", strlen("msdotnet")))
			tryMono = false;
	}

	char *sMonoDebugOption = getenv("C4D_MANAGED_DEBUGGER_ARGUMENTS");

	// Determine which runtime to use: Mono or MS.Net.
	// If we are running the 64 bit version of C4D bit we cannot do anything else but use MS.Net 
	// unless we build our own 64 bit windows version of mono.
	if (b64)
	{
		useMono = false;
		GePrint("Running in a 64bit process - starting .NET as managed runtime");
	}
	else
	{
		// We are running the 32 bit version of C4D under windows - we're free to choose.
		if (!tryMono)
		{
			GePrint("Mono is disabled because you wanted to disable it. We are using .NET for now.");
			useMono = false;
		}
		else
		{
			// User wants to use mono. See if it's really present -  WAS "c" instead of mono..
			if (NULL == LoadLibrary("mono-2.0.dll"))
			{
				GePrint("Cannot find mono-2.0.dll - starting MS .Net as managed runtime instead"); 
				useMono = false;
			}
			else
			{
				useMono = true;
				GePrint("'Mono-2.0.dll' found - will try to use 'Mono' instead of '.NET' from now.");
			}
		}
	}
#endif


	if (useMono)
	{
		// TODO: call mono_jit_parse_options(--debugger-agent="transport=dt_socket,address=$ADDRESS:$PORT");
		MonoAssembly *assembly;
 
		TCHAR dllPath[512] = "";
		_tcscpy(dllPath, pPlatformDirName);
		_tcscat(dllPath, _T("\\C4d.dll"));

		GePrint(sMonoDebugOption);

		if (sMonoDebugOption != NULL && *sMonoDebugOption != _T('\0'))
		{			
		// TCHAR *argv[] = {_T("--debugger-agent=transport=dt_socket,address=127.0.0.1:57432,embedding=1")};
			char *argv[] = {sMonoDebugOption};
			mono_jit_parse_options(1, argv);
			
			// NEW
			// Get the environment variable for mono installation here.
			const char *fusee_mono_envi_var = getenv("MONODIR");
			GePrint("Checking MONO environment variable ... ");
			GePrint(fusee_mono_envi_var);
			if(fusee_mono_envi_var != "")
			{
				const char *libdir = "\\lib";
				const char *etcdir = "\\etc";
				
				char fusee_mono_libdir[1000] = {};
				strcpy(fusee_mono_libdir, fusee_mono_envi_var);
				strcat(fusee_mono_libdir, libdir);
				
				char fusee_mono_etcdir[1000] = {};
				strcpy(fusee_mono_etcdir, fusee_mono_envi_var);
				strcat(fusee_mono_etcdir, etcdir);
				
				mono_set_dirs(fusee_mono_libdir, fusee_mono_etcdir);
				mono_debug_init (MONO_DEBUG_FORMAT_MONO);

				domain = mono_jit_init(dllPath);
			}
			
			//TODO: Use System Variable here instead of full path
			char* monoDir = getenv("MONODIR");
			GePrint("Here is env var for MONODIR");
			GePrint(monoDir);

			mono_set_dirs("C:\\Program Files (x86)\\Mono-2.10.2\\lib","C:\\Program Files (x86)\\Mono-2.10.2\\etc");
			mono_debug_init (MONO_DEBUG_FORMAT_MONO);

			domain = mono_jit_init(dllPath);

#ifndef __MAC
			// At this point (under Windows) the unmanaged exception handler should be installed 
			// That's where we end up when a breakpoint is hit. Unfortunately C4d handles this exception
			// so the unhandled handler won't be called. So we get a pointer to that handler and install 
			// it as a VectoredExceptionHandler to be first in line in the exception notification chain.
			LPTOP_LEVEL_EXCEPTION_FILTER seh_handler = SetUnhandledExceptionFilter(NULL);
			AddVectoredExceptionHandler(1, seh_handler);
			SetUnhandledExceptionFilter(seh_handler);
#endif


		}
		else
		{
			// just init mono. No debugger required
			domain = mono_jit_init(dllPath);
			GePrint("Just initialized 'Mono' - No special debugger activated");
		}

		assembly = mono_domain_assembly_open (domain, dllPath);
		MonoImage *image = mono_assembly_get_image(assembly);
		MonoMethodDesc *descStart =  mono_method_desc_new(_T("C4d.Plugin:Start()"), false);
		MonoMethodDesc *descEnd =  mono_method_desc_new(_T("C4d.Plugin:End()"), false);
		MonoMethodDesc *descMessage =  mono_method_desc_new(_T("C4d.Plugin:Message()"), false);

		methodStart = mono_method_desc_search_in_image(descStart, image);
		methodEnd = mono_method_desc_search_in_image(descEnd, image);
		methodMessage = mono_method_desc_search_in_image(descMessage, image);

		pluginclass = mono_class_from_name (image, _T("C4d"),  _T("Plugin"));

		// Plugin plugin = new Plugin()
		pluginobject = mono_object_new (domain, pluginclass);
		mono_runtime_object_init(pluginobject);
	
		if (methodStart != NULL)
		{
			MonoObject *result = mono_runtime_invoke(methodStart, pluginobject, NULL, NULL);

			// unpack bool return value from retVal
			Bool bool_result = *(Bool*)mono_object_unbox (result);
			return bool_result;
		}
		return FALSE;
	}
	else
	{
		g_pi = new PluginB();
		return g_pi->Start();

	}
	// return RegisterCommandPlugin( ID_PLUGINTEMPLATE, "ManagedPlugIn", 0, NULL, String("Da Managed PlugIn"), gNew newPluginTemplate);
}

void PluginEnd(void)	// will be called when the plugin is unloaded from C4D
{
	if (useMono)
	{
		if (methodEnd != NULL)
		{
			// plugin.End();
			mono_runtime_invoke(methodEnd, pluginobject, NULL, NULL);
		}
		if (domain != NULL)
			mono_jit_cleanup (domain);
	}
	else
	{
		if (g_pi != NULL)
		{
			g_pi->End();
			delete g_pi;
			g_pi = NULL;
		}
	}
}

Bool PluginMessage(Int32 id, void *data)		// allows you to receive plugin messages from C4D or other plugins - so the different plugins can communicate - i think error messages might also be catched here
{
	switch (id)
	{
	case C4DPL_COMMANDLINEARGS:
		{
			C4DPL_CommandLineArgs *args = (C4DPL_CommandLineArgs*)data;
			char sMonoDebugOption[128] = "";
			BOOL tryMono = false;
			
			Int32 i;
			for (i=0;i<args->argc;i++)
			{
				if (!args->argv[i]) continue;
				
				/*
				if (!strncmp(args->argv[i], "--debugger-agent", strlen("--debugger-agent")))
				{
					strncpy(sMonoDebugOption, args->argv[i], 127);
					args->argv[i] = NULL;
				}
				else if (!strcmp(args->argv[i],"--mono"))
				{
					tryMono = true;
					args->argv[i] = NULL;
				}
				else */ if (!strcmp(args->argv[i],"--help") || !strcmp(args->argv[i],"-help"))
				{
					// do not clear the entry so that other plugins can make their output!!!
					GePrint("\x01-Managed Plugin is here :-)");
				}
				else if (!strcmp(args->argv[i],"-plugincrash"))
				{
					args->argv[i] = NULL;
					*((Int32*)0) = 1234;
				}
			}
			/*
			if (!pluginStarted)
			{
				DoPluginStart(tryMono, sMonoDebugOption);
				pluginStarted = true;
			}
			*/
		}
		break;

	default:
		{
			if (useMono)
			{	// TODO: Delegate message to managed plugin
			}
			else
			{
				if (g_pi != NULL)
					return g_pi->Message(id);
			}
		}
	}
	return TRUE;
}
