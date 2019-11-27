# TLDR
## HFU PC

```
dotnet tool install -g Fusee.Tools.CmdLine
set PATH=%PATH%;%USERPROFILE%\.dotnet\tools
dotnet new -i Fusee.Template.dotnet
fusee install
```

Run these commands every time you log onto a machine.

## Your own PC

Install .NET Core 3.0 (https://dotnet.microsoft.com/download/dotnet-core/3.0)

```
dotnet tool install -g Fusee.Tools.CmdLine
dotnet new -i Fusee.Template.dotnet
fusee install
```

# Prerequisite
## .NET Core 3.0

Install the latest Version of .NET Core 3.0 (https://dotnet.microsoft.com/download/dotnet-core/3.0)

# Fusee command line tool
## Installing

`dotnet tool install -g Fusee.Tools.CmdLine`

On HFU Pcs you in addition have to `set PATH=%PATH%;%USERPROFILE%\.dotnet\tools`

## Uninstalling

`dotnet tool uninstall -g Fusee.Tools.CmdLine`

## Updating

Unfortunately dotnet doesn't provide an update command, so you have to uninstall and reinstall the tool.

## Usage

`fusee` and follow the help text.

# Visual Studio Code template
## Installing

`dotnet new -i Fusee.Template.dotnet`

## Uninstalling

`dotnet new -u Fusee.Template.dotnet`

## Creating a project using this template

`dotnet new fusee` 

## Updating
### Template package

Unfortunately dotnet doesn't provide an update command, so you have to uninstall and reinstall the template.

### Packages in a project

Run `dotnet add package Fusee.Desktop` in the same directory as the csproj file. You can optionally force it to use a certain version with the `--version` switch.

## FUSEE's Blender addon
### Install

Install Fusee command line tool.

`fusee install -b`

### Uninstall

Install Fusee command line tool.

`fusee install -bu`

## Packaging your FUSEE application to a .fuz-file

In Visual Studio Code press `Ctrl`+`Shift`+`B` select 'Pack Fusee app as fuz file'. You can find the fuz file in your build directory (usualy 'bin/Debug').

Upload the .fuz file to your webspace and link it with a fusee URI scheme handler e.g.: `fusee://mywebspace.com/myfuseeapp.fuz`.

