---
title: Install FUSEE
subtitle: Step-by-step installation instructions
comments: false
date: 2017-11-25
weight: 20
---

>  **NOTE**: Developing FUSEE Apps is currently only supported on Windows operating systems.

-------------

Before following the FUSEE installation instructions on this page, make sure all
[Necessary Tools](../necessary-tools/) are installed.

## Installation

- Download the latest 
  [FUSEE Release](https://github.com/FUSEEProjectTeam/Fusee/releases/download/v0.7.0-BinDist/Fusee_v0.7.0.exe) 
  from the 
  [FUSEE GitHub Repository](https://github.com/FUSEEProjectTeam/Fusee/releases).

- Copy or move the downloaded `Fusee_v0.7.0.exe` to a convenient place on your hard drive. This will
  become the home of your FUSEE installation.

>  **IMPORTANT:** On some systems, the executable file is blocked and it may fail to run.
>  Before double-clicking it, [unblock](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/)
>  the file `Fusee_v0.7.0.exe`.
  
- Double-click on the executable file.
  This will extract its contents into a `Fusee` folder below the folder where the executable file is placed.

- In the newly extracted `Fusee` folder, 

  - ***either*** start `SetupPerUser.bat` if you want to register the FUSEE installation only for the current user.
    This is the only option to install FUSEE without Administrator privileges. You can simply double-click
    on `SetupPerUser.bat`.

  - ***or*** start `SetupMachineWide.bat`. This will perform the necessary FUSEE registration steps to enable
    all users on a machine to use the FUSEE installation. You need to start the command with Administrator privileges,
    otherwise the installation will fail. To start the machine-wide-installation, right-click on `SetupMachineWide.bat`
    and select "Start as administrator" from the context-menu.

### Enable the FUSEE Export Add-On within Blender

If a Blender installation exists at a typical installation path (e.g. "C:\Program Files\Blender Foundation\...")
one of the `Setup...` commands in the previous step already copied the FUSEE Blender Add-on to a directory recognized
by Blender on start up. Still, the Add-on needs to be activated inside Blender:

1. Open Blender
1. Open the _User Preferences_ window ("File &rarr; User Preferences" or `Ctrl+Alt+U`)
1. Open the _Add-ons_ Tab
1. Under _Supported Level_ activate _Testing_ (The FUSEE Blender Add-on is still experimental).
1. As a result, the Export Add-on should show up as _Import-Export: .fus format_. 
   If the Add-on does not appear in the list, the Blender Add-on installation part of the setup process 
   failed. 
1. Activate the Add-on by checking the check-box.
1. _Save User Settings_ and close the User Preferences window.
1. Blender's File->Export menu should now contain the _FUS (.fus)_ option capable of writing
  FUSEE's .fus file format for 3D Assets.

#### Screen Cast: Enable the FUSEE Export Add-On within Blender.
![Enable FUSEE Blender Add-On](enableblenderaddon.gif)



