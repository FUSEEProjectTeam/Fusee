---
title: FUSEE 3D
subtitle: Realtime 3D Engine
homepagetitle: 3D REALTIME ENGINE
homepagesubtitle: Open Source, Lightweight, Portable, Built with .NET Core
comments: false
---


## What's FUSEE?

FUSEE is an Open Source 3D Realtime Engine written in C#. Applications written on top of FUSEE 
can be published to a number of platforms.
 
## Create 3D Content using Blender

3D Content displayed in FUSEE applications (_Assets_) can be generated using the free 
Blender 3D modelling software. 

FUSEE comes with an export Add-on for Blender.
_Either_ save Blender contents as assets and access them from your C# code in your FUSEE app 
_or_ generate an entire interactive web application directly from your Blender content without
programming a single line of code.

## Write 3D Apps with Visual Studio Code

Use the popular lightweight IDE to create FUSEE Apps, debug into your code and publish
your App to the supported platforms. The generated code is built on top of .NET core, 
the Open Source platform independent variant among the different .NET implementations.

FUSEE comes with a .NET core template. Start writing FUSEE Apps within VS Code by
simply typing `dotnet new fusee` at the command line.

## Publish Your App on different Platforms

The FUSEE binary distribution supports two platforms out-of-the-box: 

- **.NET App**. This will embed your application code into a .NET Framework
  3D application and bundles all necessary files into one directory. Easily distribute
  your 

- **Web App**. This will cross-compile your application code into a
  a HTML/JavaScript WebGL-based Web-App runnable on most browsers, including
  mobile versions.

- **Android**. Using Xamarin, your code will be transformed into a native
  Android apk. Currently, the Xamarin build is not integrated into 
  the FUSEE Visual Studio Code build process. To create Android Apps from 
  your FUSEE projects, you need to get the FUSEE sources from the 
  [FUSEE GitHub repository](https://github.com/FUSEEProjectTeam/Fusee).
