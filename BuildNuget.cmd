@echo off
setlocal

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf
mkdir bin\Release\nuget

echo Building distribution.
dotnet publish src\Engine\Player\Desktop\Fusee.Engine.Player.Desktop.csproj -c Release

dotnet pack Fusee.sln -c Release-NuGet
msbuild src\Base\Imp\Android\Fusee.Base.Imp.Android.csproj -t:restore,pack -p:Configuration=Release
msbuild src\Engine\Imp\Graphics\Android\Fusee.Engine.Imp.Graphics.Android.csproj -t:restore,pack -p:Configuration=Release
dotnet pack dis\NuGet\Core\Core.csproj -c Release -o bin\Release\nuget
dotnet pack dis\NuGet\Desktop\Desktop.csproj -c Release -o bin\Release\nuget
msbuild dis\NuGet\Android\Android.csproj -t:restore,pack -p:Configuration=Release
dotnet pack dis\DnTemplate\DnTemplate.csproj -c Release -o bin\Release\nuget

rem nuget pack src\Base\Imp\Android\Fusee.Base.Imp.Android.csproj -OutputDirectory bin\Release\nuget -Properties Configuration=Release -Build -Symbols -SymbolPackageFormat snupkg
rem nuget pack src\Engine\Imp\Graphics\Android\Fusee.Engine.Imp.Graphics.Android.csproj -OutputDirectory bin\Release\nuget -Properties Configuration=Release -Build -Symbols -SymbolPackageFormat snupkg
endlocal