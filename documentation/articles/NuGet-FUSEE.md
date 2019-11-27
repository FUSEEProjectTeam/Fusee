  > ⚠️ **Pre-Release Content**

# Prerequisite
* Install .NET Core 3.0

# Installing fusee.exe
* `dotnet tool install -g Fusee.Tools.CmdLine --add-source https://fuseeprojectteam.github.io/FuseeNuGetPacketRegistry/index.json` (--add-source will be gone when we decide to go to nuget.org)

# Installing Blender Add-On
* Install fusee.exe
* `fusee install [-b]`

# Installing dotnet/VS-Code FUSEE Template
* `dotnet new -i Fusee.Template.dotnet --nuget-source https://fuseeprojectteam.github.io/FuseeNuGetPacketRegistry/index.json` (--nuget-source will be gone when we decide to go to nuget.org)

# Running a .fus file or a Fusee .dll
* Install fusee.exe
* `fusee player -i <fus/dll>`

# Update the NuGet Fusee Packages
* `dotnet tool update -g Fusee.Tools.CmdLine`