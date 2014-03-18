#include "stdafx.h"
#include "PluginB.h"

#include <vcclr.h>

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;


// The Resolver and Initializer classes are needed to find managed DLLs in the
// plugin folder path.
public ref class Resolver
{
public:
	static Assembly^ ResolveIt(Object^ sender, ResolveEventArgs^ args)
    {
		// Get the DLL file name out of the Name
		if (args->Name->StartsWith("mscorlib.resources"))
		{
			// Find out current framework version and installation path
			String^ msCorLibPath = System::IO::Path::Combine(System::Runtime::InteropServices::RuntimeEnvironment::GetRuntimeDirectory(), "de\\mscorlib.resources.dll");
			return Assembly::LoadFrom(msCorLibPath);
			// return Assembly::LoadFrom("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\de\\mscorlib.resources.dll");
		}

		if (args->Name != nullptr)
		{
			String^ dllName = args->Name->Substring(0, args->Name->IndexOf(','));

			Assembly ^thisDll = Assembly::GetExecutingAssembly();
			String ^filePath = thisDll->Location;
			String ^dirPath = Path::GetDirectoryName(filePath);
			String^ dllPath;
			DirectoryInfo ^di;
			// First TRY to look in the directory we are currently in. If the dll we are looking for is
			// branded to 32 or 64 bit, we will find it in the right directory.
			try
			{
				di = gcnew DirectoryInfo(dirPath);
				dllPath = ScanDirectories(di, 2, dllName + ".dll");
				if (!String::IsNullOrEmpty(dllPath))
				{
					return Assembly::LoadFrom(dllPath);
				}
			}
			catch (Exception  ^ex)
			{
				int i = 44;
			}

			// We didn't find te correct dll, so we have to look broader. Go up two dir levels and
			// try recursing again.
			dirPath = Path::GetDirectoryName(dirPath);
			dirPath = Path::GetDirectoryName(dirPath);
			// int seperateCharPos = Path::GetDirectoryName(dirPath)->LastIndexOf(Path::DirectorySeparatorChar);
			// DirectorySeparatorChar{\}
			di = gcnew DirectoryInfo(dirPath);
			dllPath = ScanDirectories(di, 2, dllName+".dll");
			if(!String::IsNullOrEmpty(dllPath))
			{
					return Assembly::LoadFrom(dllPath);
			}

			return nullptr;
			// return Assembly::LoadFrom(Path::GetDirectoryName(filePath) + "\\" + dllName + ".dll");
		}
		else
			return nullptr;
    }

	static String^ ScanDirectories(DirectoryInfo ^di, int recursionDepth, String^ dllName)
        {
            if (recursionDepth < 0)
                return nullptr;

            // First scan all files in the directory
			array<FileInfo^>^ files = di->GetFiles();

			for (int i = 0; i < files->Length; i++)
            {
				if(files[i]->Name == dllName)
				{
					return files[i]->FullName;
				}
            }

			// Our
			array<DirectoryInfo^>^ directories = di->GetDirectories();

			for (int i = 0; i < directories->Length; i++)
            {
				String ^dllPath = ScanDirectories(directories[i], recursionDepth - 1, dllName);

				if(!String::IsNullOrEmpty(dllPath))
				{
					return dllPath;
				}
            }
			return nullptr;
        }
};



struct __declspec(dllexport) Initializer 
{
	Initializer() 
	{
		// This is called after DllMain but before any other managed stuff.
		AppDomain ^currentDomain = AppDomain::CurrentDomain;

		// This registers a "Callback function" (a Delegate) that will be called whenever the CLR
		// cannot find an Assembly (= a managed DLL) in the standard search paths
		currentDomain->AssemblyResolve += gcnew ResolveEventHandler(&Resolver::ResolveIt);
	}

	// We need the test method to keep the compiler from optimizing the entire class away.
	void Test() 
	{
		volatile int a = 42;
	}
};
 
// Global instance of object - the object itself is never needed, but this forces the initialization code
// to be performed before anything else.
Initializer obj;


// Reference the managed dll here
#using "C4d.dll"

using namespace C4d;


// Declare the container struct for keeping the reference to the managed Plugin
struct PluginB::PluginBInternal
{
	// Constructor for creating instances
    PluginBInternal(Plugin^ mk) { _mk = mk; }
    
	// The reference that will contain a reference to an instance of Plugin
	// note the "gcroot" type - this allows us to store an instance managed by .Net's
	// garbage collector inside an unmanaged instance of PluginB.
	gcroot<Plugin^> _mk;
};


PluginB::PluginB()
{
	_pImpl = new PluginB::PluginBInternal(gcnew Plugin());
}

bool PluginB::Start()
{
	return _pImpl->_mk->Start();
}

void PluginB::End()
{
	_pImpl->_mk->End();
}

bool PluginB::Message(int id)
{
	return _pImpl->_mk->Message(id);
}


PluginB::PluginB(PluginBInternal* pImpl)
{
	_pImpl = pImpl;
}


// Implementation of the unmanaged destructor
PluginB::~PluginB()
{
	// we need to delete _pImpl. This will also de-reference the gcroot reference and 
	// mark it as unused for the .net garbage collector
	delete _pImpl;
}