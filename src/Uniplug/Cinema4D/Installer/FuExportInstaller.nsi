; FUSEE 3D Web Exporter for CINEMA 4D installer
; Written by Christoph Mueller

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"
  !include "LogicLib.nsh"
  !include "FileFunc.nsh"
  !include "FuExportFunctions.nsh"
  
;--------------------------------
;General

  ;Name and file
  Name "FUSEE CINEMA 4D Web Exporter"
  OutFile "FuseeC4DWebExporter.exe"

  ;Default installation folder
  ; InstallDir "$PROGRAMFILES64\MAXON\CINEMA 4D R15\plugins"
  
  ;Get installation folder from registry if available
  ; InstallDirRegKey HKLM "Software\FUSEE C4D EXPORTER" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin
  
  ;Variable declarations
  Var C4DCommand
  Var MaxC4DVersion
  Var MinC4DVersion
  Var fuVar0
  Var fuVar1
  Var fuVar2
  Var fuStr0
  Var fuStr1
  Var fuStr2
  Var Cinema4DFileName
  Var Cinema4DFilePath
  Var Cinema4DDirPath
  Var Cinema4DFileNameReg
  Var Cinema4DFilePathReg
  Var Cinema4DDirPathReg
  Var ConfirmInstallDir
  Var FU_INSTDIR

  !define MUI_ICON FuseeIcon.ico
  
;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages
  !insertmacro MUI_PAGE_LICENSE "License.rtf"
;  !insertmacro MUI_PAGE_COMPONENTS
  !define MUI_PAGE_CUSTOMFUNCTION_PRE dirPre
  !define MUI_PAGE_CUSTOMFUNCTION_LEAVE dirLeave
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Functions 
Function .onInit
  ; Find a CINEMA 4D installation
  ; First try to look at the standard installation directories
  ; in reverse order of revision number
  StrCpy $MaxC4DVersion "17"
  StrCpy $MinC4DVersion "14"
  StrCpy $fuVar0 $MaxC4DVersion
  ${While} $fuVar0 >= $MinC4DVersion
	StrCpy $fuStr0 "$PROGRAMFILES64\MAXON\CINEMA 4D R$fuVar0"
	FindFirst $fuVar1 $fuStr1 "$fuStr0\CINEMA*.exe"
	IfFileExists $fuStr0\$fuStr1 filefound filenotfound
    filefound:
      StrCpy $Cinema4DFileName $fuStr1
      StrCpy $Cinema4DFilePath $fuStr0\$fuStr1
	  StrCpy $Cinema4DDirPath $fuStr0
	  ${Break}
    filenotfound:
	FindClose $fuVar1
	IntOp $fuVar0 $fuVar0 - 1
  ${EndWhile}
  ; Not found. Perhaps the user was at least smart enough to install it below

  ; Additionally try to find something in the registry
  ReadRegStr $C4DCommand HKCR "CINEMA 4D Document\shell\open\command" ""
  ${If} $C4DCommand != ""
    ${StrRep} $C4DCommand $C4DCommand "%1" "" 
    ${StrRep} $C4DCommand $C4DCommand "%2" "" 
	${StrRep} $C4DCommand $C4DCommand "$\"" ""
	${Trim} $C4DCommand $C4DCommand
	IfFileExists $C4DCommand filefoundreg filenotfoundreg
    filefoundreg:
	  StrCpy $Cinema4DFilePathReg $C4DCommand
      ${GetParent} $C4DCommand $Cinema4DDirPathReg
      ${GetFileName} $C4DCommand $Cinema4DFileNameReg
    filenotfoundreg:
  ${Endif} 
  
  /*
  MessageBox MB_OK \
  "Cinema4DFileName:      $Cinema4DFileName$\r$\n\
  Cinema4DFilePath:      $Cinema4DFilePath$\r$\n\
  Cinema4DDirPath:       $Cinema4DDirPath$\r$\n\
  Cinema4DFileNameReg:   $Cinema4DFileNameReg$\r$\n\
  Cinema4DFilePathReg:   $Cinema4DFilePathReg$\r$\n\
  Cinema4DDirPathReg:    $Cinema4DDirPathReg$\r$\n"
  */
  
  ; Decide if we need to show the installation dir page. If both, registry entry
  ; and standard installation path found an installation, skip the installation dir page.
  StrCpy $ConfirmInstallDir 1
  ${If} $Cinema4DDirPath != ""
    ${If} $Cinema4DDirPathReg != ""
	  ${If} $Cinema4DDirPathReg == $Cinema4DDirPath
        StrCpy $ConfirmInstallDir 0
      ${Endif} 
    ${Else} ; found the file path but no registry entry
    ${Endif} 
  ${Else} 
    ${If} $Cinema4DDirPathReg != ""
	  StrCpy $Cinema4DDirPath $Cinema4DDirPathReg
    ${Else} ; neither file path but nor registry entry found
	  StrCpy $Cinema4DDirPath $PROGRAMFILES64
    ${Endif} 
  ${Endif} 

  StrCpy $INSTDIR $Cinema4DDirPath\plugins
  
  ; Now see if a CINEMA 4D instance is running (no matter where its installed)
  ; We can see this on the Window Class Name which is: C4DR_WIN1_150_0 for R15 
  ; and C4DR_WIN1_140_0 for R14. So we try to find a window class with the partial
  ; class name C4DR_WIN
  ${EnhancedFindWindow} 0 "C4DR_WIN" $fuStr1 $fuStr2
  ${If} $fuStr1 != "failed"
	MessageBox MB_YESNO|MB_ICONEXCLAMATION|MB_DEFBUTTON2 \
	"CINEMA 4D appears to be running. Make sure to close CINEMA 4D before continuing this installation!$\r$\n$\r$\n\
	Do you want to continue the installation?" \
	  IDYES mb_yes ; IDCANCEL mb_cancel
	  Abort
	mb_yes:
  ${Endif} 
FunctionEnd  


Function dirPre
  ;MessageBox MB_OK "Before Dir"
  ${If} $ConfirmInstallDir = 0
    ; The installation seems to be unambiguous (registry key and file search produced the same result)
	; we'll skip asking the user for the installation directory
  	StrCpy $FU_INSTDIR $INSTDIR
    Abort
  ${Endif}
FunctionEnd


Function dirLeave
    ; remove trailing backslash (if present)
	${WordReplace} "$INSTDIR" "\" "" "E}" $INSTDIR
	
	; Get parent directory (strip off "\plugins" if present)
	${WordReplace} "$INSTDIR" "\plugins" "" "E}" $fuStr0
	;${GetParent} $INSTDIR $fuStr0	
	 
	FindFirst $fuVar1 $fuStr1 "$fuStr0\CINEMA*.exe"
	IfFileExists $fuStr0\$fuStr1 filefounddl filenotfounddl
    filefounddl:
	  ;MessageBox MB_OK "$fuStr0\$fuStr1"
      StrCpy $Cinema4DFileName $fuStr1
      StrCpy $Cinema4DFilePath $fuStr0\$fuStr1
	  StrCpy $Cinema4DDirPath $fuStr0
  	  StrCpy $FU_INSTDIR $Cinema4DDirPath\plugins
	  ;MessageBox MB_OK "$FU_INSTDIR"
	Goto carryOn
    filenotfounddl:
	MessageBox MB_YESNO|MB_ICONEXCLAMATION|MB_DEFBUTTON2 \
	"The specified folder does not appear to be the plugin subfolder of a valid CINEMA 4D installation!$\r$\n$\r$\n\
	Do you want to install to this folder anyway?" \
	  IDYES mb_yes ; IDCANCEL mb_cancel
	  Abort
	;mb_cancel:
	;  Quit
	mb_yes:
      StrCpy $Cinema4DFileName ""
      StrCpy $Cinema4DFilePath ""
	  StrCpy $Cinema4DDirPath ""	
  	  StrCpy $FU_INSTDIR $INSTDIR
	carryOn:
	FindClose $fuVar1
FunctionEnd
  
;--------------------------------
;Installer Sections
Section "Main Section" SecMain
  StrCpy $INSTDIR $FU_INSTDIR

/* Doesn't work
   IfFileExists $INSTDIR\UninstallFuseeC4DWebExporter.exe uninstfound uninstNOTfound
    uninstfound:
	ExecWait "$INSTDIR\UninstallFuseeC4DWebExporter.exe"
    uninstNOTfound:
*/
  
  ; ADD FILES / DIRECTORIES HERE...
  SetOutPath "$FU_INSTDIR\ManagedPlugIn"
  File /r "$%C4D_DIR%\plugins\ManagedPlugIn\*.dll"  
  File /nonfatal /r "$%C4D_DIR%\plugins\ManagedPlugIn\*.cdl"  
  File /r "$%C4D_DIR%\plugins\ManagedPlugIn\*.cdl64"  
  SetOutPath "$FU_INSTDIR\ManagedPlugIn\x64\res"
  File /r "$%C4D_DIR%\plugins\ManagedPlugIn\x64\res\*"  

  SetOutPath "$FU_INSTDIR\FuExport"
  File /r "$%C4D_DIR%\plugins\FuExport\*.dll"  
  SetOutPath "$FU_INSTDIR\FuExport\res"
  File /r "$%C4D_DIR%\plugins\FuExport\res\*"  
  SetOutPath "$FU_INSTDIR\FuExport\Viewer"
  File /r "$%C4D_DIR%\plugins\FuExport\Viewer\*"  
  
  ;Store installation folder
  WriteRegStr HKLM "Software\FUSEE C4D EXPORTER" "" $FU_INSTDIR
  
  ;Create uninstaller
  WriteUninstaller "$FU_INSTDIR\UninstallFuseeC4DWebExporter.exe"

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "A test section."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} $(DESC_SecMain)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD FILES / DIRECTORIES HERE...
  RMDir /r "$INSTDIR\ManagedPlugIn"
  RMDir /r "$INSTDIR\FuExport"
  Delete "$INSTDIR\UninstallFuseeC4DWebExporter.exe"

  ; RMDir "$INSTDIR"

  DeleteRegKey /ifempty HKLM "Software\FUSEE C4D EXPORTER"

SectionEnd


