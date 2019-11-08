@echo off
setlocal

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf

echo Building distribution.
dotnet pack Fusee.Engine.sln -c Release-NuGet -o bin\Release\nuget --include-symbols --include-source
dotnet pack dis\NuGet\Core\Core.csproj -c Release -o bin\Release\nuget
dotnet pack dis\NuGet\Desktop\Desktop.csproj -c Release -o bin\Release\nuget
dotnet pack dis\DnTemplate\DnTemplate.csproj -c Release -o bin\Release\nuget
endlocal