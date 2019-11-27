@echo off
setlocal

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf

echo Building distribution.
dotnet pack Fusee.sln -c Release-NuGet -o bin\Release\nuget --include-symbols --include-source
dotnet pack dis\NuGet\Core\Core.csproj -c Release -o bin\Release\nuget
dotnet pack dis\NuGet\Desktop\Desktop.csproj -c Release -o bin\Release\nuget
dotnet pack dis\DnTemplate\DnTemplate.csproj -c Release -o bin\Release\nuget

rem nuget pack src\Base\Imp\Android\Fusee.Base.Imp.Android.csproj -OutputDirectory bin\Release\nuget -Properties Configuration=Release -Build -Symbols -SymbolPackageFormat snupkg
rem nuget pack src\Engine\Imp\Graphics\Android\Fusee.Engine.Imp.Graphics.Android.csproj -OutputDirectory bin\Release\nuget -Properties Configuration=Release -Build -Symbols -SymbolPackageFormat snupkg
endlocal