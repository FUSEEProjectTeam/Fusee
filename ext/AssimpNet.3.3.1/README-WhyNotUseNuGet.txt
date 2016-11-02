The contents of this directory is exactly what NuGet would create below package when 
referencing Assimp-Net with NuGet.
https://www.nuget.org/packages/AssimpNet/3.3.1

Unfortunately with FUSEE's solution structure, the native Assimp64.dll does not get
copied to the output directory when building a project referencing NuGet Assimp-Net.
There's an open issue about this:
https://github.com/assimp/assimp-net/issues/32

Once this is fixed, Assimp-Net should be referenced using NuGet instead of being
copied into FUSEE's ext directory.


