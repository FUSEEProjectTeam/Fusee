#Analyzing The Xamarain Android build

##Building my own msbuild command line prompt:
Create a Shortcut with the following command.
```
%comspec% /k ""C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat""
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"
```

##Building Fusee.Engine.SceneViewer.Android on Command line reveals
From
https://developer.xamarin.com/guides/android/under_the_hood/build_process/
we learn that we need to call `msbuild` with the `/t:Install` option. The standard build option will not
create the apk.
The apk is first created in the intermediate directory (tmp..) and then copied to the output directory.
The output of `msbuild /t:Install` contains:
```
_Mandroid:
  Die Datei wird von "C:\Users\mch\Documents\_DEV\Fusee\tmp\Obj\Fusee.Engine.SceneViewer.Android\Debug\android\bin\Fusee.Engine.SceneViewer.apk" in "..\..\..\..\bin\Debug
  \SceneViewer\Android\Fusee.Engine.SceneViewer.apk" kopiert.
```
  
##Searcing for the _Mandroid target
Xamarin Android .csproj files contains
```XML
 <Import Project="$(MSBuildExtensionsPath)\Xamarin\Android\Xamarin.Android.CSharp.targets" />
```

Expanding the `$(MSBuildExtensionsPath)` points to the file
```
C:\Program Files (x86)\MSBuild \Xamarin\Android\Xamarin.Android.CSharp.targets 
```

#Change APK contents:
##Prerequisites

###Create Keystore
```keytool -genkey -v -keystore FuseeApp.keystore -alias FuseeKey -keyalg RSA -keysize 2048 -validity 10000```
(from http://developer.android.com/tools/publishing/app-signing.html)

###Install apktool from http://ibotpeaches.github.io/Apktool

##Uncompress .apk file and decode contents
```apktool d TheApp.Original.apk```
(as seen in http://ibotpeaches.github.io/Apktool/documentation/)

###Change apk contents
Replace assets in ```TheApp.Original\assets``` 
Change app name in ```TheApp.Original\AndroidManifest.xml```
 - Make sure to include one dot ```.``` in the new package name
   <manifest xmlns:android="http://schemas.android.com/apk/res/android" android:installLocation="auto" package="org.TheOtherApp" ...
   ```
 - Apply the same name changes as above to the provider
   ```XML
   <provider android:authorities="org.TheOtherApp.mono.MonoRuntimeProvider.__mono_init__"
   ```
 - Rename the ```android.label``` properties in the ```<application>``` and ```<activity>``` tags.
 

###Build new apk with changed contents
```apktool b -o TheAppUnsigned.apk TheApp.Original```

###Sign the apk
Create a copy of the unsigned apk called ```TheAppSigned.apk```. Then call ```jarsigner```
```jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore FuseeApp.keystore TheAppSigned.apk FuseeKey```

###Zipalign the apk
Note that the zipaligntool lives in a platform-(read: Android-version)-specific subdirectory of the Android SDK which typically is not in the
PATH variable.
```"C:\Program Files (x86)\Android\android-sdk\build-tools\23.0.1\zipalign" -v 4 TheAppSigned.apk TheApp.apk```

###Upload the apk
```adb install TheApp.apk```

Before that, any existing version (with the same package name) should be uninstalled first

```adb install TheApp```  -- **without ".apk" !!!**




