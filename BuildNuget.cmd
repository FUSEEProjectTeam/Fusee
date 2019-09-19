@echo off
setlocal
set CONFIGURATION=Release
set TARGETPLATFORM=Desktop

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf

echo Building distribution.
dotnet build Fusee.Engine.sln -c %CONFIGURATION%-%TARGETPLATFORM%

echo Packing up
nuget pack src\Math\Core -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Serialization -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Xene -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Xirkit -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Base\Common -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Jometri -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Base\Core -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Base\Imp\Desktop -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Engine\Common -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Engine\Core -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Engine\GUI -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Engine\Imp\Graphics\Desktop -properties Configuration=%CONFIGURATION% -OutputDirectory bin\%CONFIGURATION%\nuget

nuget pack src\Meta\nuget\Fusee.Core.nuspec -OutputDirectory bin\%CONFIGURATION%\nuget
nuget pack src\Meta\nuget\Fusee.Desktop.nuspec -OutputDirectory bin\%CONFIGURATION%\nuget
endlocal