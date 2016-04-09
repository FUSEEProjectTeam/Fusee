call apktool b -o Fusee.Engine.SceneViewerUnsigned.apk Fusee.Engine.SceneViewer
copy  Fusee.Engine.SceneViewerUnsigned.apk Fusee.Engine.SceneViewerSigned.apk
jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore FuseeApp.keystore Fusee.Engine.SceneViewerSigned.apk FuseeKey
"C:\Program Files (x86)\Android\android-sdk\build-tools\23.0.1\zipalign" -v 4 Fusee.Engine.SceneViewerSigned.apk Fusee.Engine.SceneViewer.apk
