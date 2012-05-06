// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the BRIDGE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// BRIDGE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef BRIDGE_EXPORTS
#define BRIDGE_API __declspec(dllexport)
#else
#define BRIDGE_API __declspec(dllimport)
#endif



// This class is exported from the BRIDGE.dll
class BRIDGE_API PluginB 
{
public:
	// Wrapper methods
	PluginB();
	bool Start();
	void End();
	bool Message(int id);

	// Wrapper-specific stuff:
	// Forward declaration of the MeineKlasseInternal type. We cannot declare it here because
	// it contains a managed reference to the managed MeineKlasse type. If we declared it here
	// we could not include this header file into unmanaged projects (like UnmanagedApp)
    struct PluginBInternal;

	// Pointer to the implementation (google the pimple pattern to understand this!!!).
	PluginBInternal *_pImpl;

	// Internal constructor for creating wrapper objects (PluginB instances) wrapping already existing MeineKlasse instances
    PluginB(PluginBInternal* pImpl);

	// Destructor
	virtual ~PluginB();
};

/*
extern BRIDGE_API int nBRIDGE;

BRIDGE_API int fnBRIDGE(void);
*/