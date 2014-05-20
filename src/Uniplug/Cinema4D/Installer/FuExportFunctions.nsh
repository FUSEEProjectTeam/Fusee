; FUSEE 3D Web Exporter for CINEMA 4D installer
; Written by Christoph Mueller

; Trim
;   Removes leading & trailing whitespace from a string
; Usage:
;   Push 
;   Call Trim
;   Pop 
Function Trim
	Exch $R1 ; Original string
	Push $R2
 
Loop:
	StrCpy $R2 "$R1" 1
	StrCmp "$R2" " " TrimLeft
	StrCmp "$R2" "$\r" TrimLeft
	StrCmp "$R2" "$\n" TrimLeft
	StrCmp "$R2" "$\t" TrimLeft
	GoTo Loop2
TrimLeft:	
	StrCpy $R1 "$R1" "" 1
	Goto Loop
 
Loop2:
	StrCpy $R2 "$R1" 1 -1
	StrCmp "$R2" " " TrimRight
	StrCmp "$R2" "$\r" TrimRight
	StrCmp "$R2" "$\n" TrimRight
	StrCmp "$R2" "$\t" TrimRight
	GoTo Done
TrimRight:	
	StrCpy $R1 "$R1" -1
	Goto Loop2
 
Done:
	Pop $R2
	Exch $R1
FunctionEnd

; Usage:
; ${Trim} $trimmedString $originalString
!define Trim "!insertmacro Trim"
!macro Trim ResultVar String
  Push "${String}"
  Call Trim
  Pop "${ResultVar}"
!macroend


!define StrRep "!insertmacro StrRep"
!macro StrRep output string old new
    Push `${string}`
    Push `${old}`
    Push `${new}`
    !ifdef __UNINSTALL__
        Call un.StrRep
    !else
        Call StrRep
    !endif
    Pop ${output}
!macroend
 
!macro Func_StrRep un
    Function ${un}StrRep
        Exch $R2 ;new
        Exch 1
        Exch $R1 ;old
        Exch 2
        Exch $R0 ;string
        Push $R3
        Push $R4
        Push $R5
        Push $R6
        Push $R7
        Push $R8
        Push $R9
 
        StrCpy $R3 0
        StrLen $R4 $R1
        StrLen $R6 $R0
        StrLen $R9 $R2
        loop:
            StrCpy $R5 $R0 $R4 $R3
            StrCmp $R5 $R1 found
            StrCmp $R3 $R6 done
            IntOp $R3 $R3 + 1 ;move offset by 1 to check the next character
            Goto loop
        found:
            StrCpy $R5 $R0 $R3
            IntOp $R8 $R3 + $R4
            StrCpy $R7 $R0 "" $R8
            StrCpy $R0 $R5$R2$R7
            StrLen $R6 $R0
            IntOp $R3 $R3 + $R9 ;move offset by length of the replacement string
            Goto loop
        done:
 
        Pop $R9
        Pop $R8
        Pop $R7
        Pop $R6
        Pop $R5
        Pop $R4
        Pop $R3
        Push $R0
        Push $R1
        Pop $R0
        Pop $R1
        Pop $R0
        Pop $R2
        Exch $R1
    FunctionEnd
!macroend
!insertmacro Func_StrRep ""
!insertmacro Func_StrRep "un."
 
!macro IndexOf Var Str Char
Push "${Char}"
Push "${Str}"
 Call IndexOf
Pop "${Var}"
!macroend
!define IndexOf "!insertmacro IndexOf"
 
Function RIndexOf
Exch $R0
Exch
Exch $R1
Push $R2
Push $R3
 
 StrCpy $R3 $R0
 StrCpy $R0 0
 IntOp $R0 $R0 + 1
  StrCpy $R2 $R3 1 -$R0
  StrCmp $R2 "" +2
  StrCmp $R2 $R1 +2 -3
 
 StrCpy $R0 -1
 
Pop $R3
Pop $R2
Pop $R1
Exch $R0
FunctionEnd
 
!macro RIndexOf Var Str Char
Push "${Char}"
Push "${Str}"
 Call RIndexOf
Pop "${Var}"
!macroend
!define RIndexOf "!insertmacro RIndexOf"

Function WordReplace
	!define WordReplace `!insertmacro WordReplaceCall`
 
	!macro WordReplaceCall _STRING _WORD1 _WORD2 _NUMBER _RESULT
		Push `${_STRING}`
		Push `${_WORD1}`
		Push `${_WORD2}`
		Push `${_NUMBER}`
		Call WordReplace
		Pop ${_RESULT}
	!macroend
 
	Exch $2
	Exch
	Exch $1
	Exch
	Exch 2
	Exch $0
	Exch 2
	Exch 3
	Exch $R0
	Exch 3
	Push $3
	Push $4
	Push $5
	Push $6
	Push $7
	Push $8
	Push $9
	Push $R1
	ClearErrors
 
	StrCpy $R1 $R0
	StrCpy $9 ''
	StrCpy $3 $2 1
	StrCpy $2 $2 '' 1
	StrCmp $3 'E' 0 +3
	StrCpy $9 E
	goto -4
 
	StrCpy $4 $2 1 -1
	StrCpy $5 ''
	StrCpy $6 ''
	StrLen $7 $0
 
	StrCmp $7 0 error1
	StrCmp $R0 '' error1
	StrCmp $3 '{' beginning
	StrCmp $3 '}' ending errorchk
 
	beginning:
	StrCpy $8 $R0 $7
	StrCmp $8 $0 0 +4
	StrCpy $R0 $R0 '' $7
	StrCpy $5 '$5$1'
	goto -4
	StrCpy $3 $2 1
	StrCmp $3 '}' 0 merge
 
	ending:
	StrCpy $8 $R0 '' -$7
	StrCmp $8 $0 0 +4
	StrCpy $R0 $R0 -$7
	StrCpy $6 '$6$1'
	goto -4
 
	merge:
	StrCmp $4 '*' 0 +5
	StrCmp $5 '' +2
	StrCpy $5 $1
	StrCmp $6 '' +2
	StrCpy $6 $1
	StrCpy $R0 '$5$R0$6'
	goto end
 
	errorchk:
	StrCmp $3 '+' +2
	StrCmp $3 '-' 0 error3
 
	StrCpy $5 $2 1
	IntOp $2 $2 + 0
	StrCmp $2 0 0 one
	StrCmp $5 0 error2
	StrCpy $3 ''
 
	all:
	StrCpy $5 0
	StrCpy $2 $R0 $7 $5
	StrCmp $2 '' +4
	StrCmp $2 $0 +6
	IntOp $5 $5 + 1
	goto -4
	StrCmp $R0 $R1 error1
	StrCpy $R0 '$3$R0'
	goto end
	StrCpy $2 $R0 $5
	IntOp $5 $5 + $7
	StrCmp $4 '*' 0 +3
	StrCpy $6 $R0 $7 $5
	StrCmp $6 $0 -3
	StrCpy $R0 $R0 '' $5
	StrCpy $3 '$3$2$1'
	goto all
 
	one:
	StrCpy $5 0
	StrCpy $8 0
	goto loop
 
	preloop:
	IntOp $5 $5 + 1
 
	loop:
	StrCpy $6 $R0 $7 $5
	StrCmp $6$8 0 error1
	StrCmp $6 '' minus
	StrCmp $6 $0 0 preloop
	IntOp $8 $8 + 1
	StrCmp $3$8 +$2 found
	IntOp $5 $5 + $7
	goto loop
 
	minus:
	StrCmp $3 '-' 0 error2
	StrCpy $3 +
	IntOp $2 $8 - $2
	IntOp $2 $2 + 1
	IntCmp $2 0 error2 error2 one
 
	found:
	StrCpy $3 $R0 $5
	StrCmp $4 '*' 0 +5
	StrCpy $6 $3 '' -$7
	StrCmp $6 $0 0 +3
	StrCpy $3 $3 -$7
	goto -3
	IntOp $5 $5 + $7
	StrCmp $4 '*' 0 +3
	StrCpy $6 $R0 $7 $5
	StrCmp $6 $0 -3
	StrCpy $R0 $R0 '' $5
	StrCpy $R0 '$3$1$R0'
	goto end
 
	error3:
	StrCpy $R0 3
	goto error
	error2:
	StrCpy $R0 2
	goto error
	error1:
	StrCpy $R0 1
	error:
	StrCmp $9 'E' +3
	StrCpy $R0 $R1
	goto +2
	SetErrors
 
	end:
	Pop $R1
	Pop $9
	Pop $8
	Pop $7
	Pop $6
	Pop $5
	Pop $4
	Pop $3
	Pop $2
	Pop $1
	Pop $0
	Exch $R0
FunctionEnd


Function  EnhancedFindWindow
	!define EnhancedFindWindow '!insertmacro EnhancedFindWindowCall'
 
	!macro EnhancedFindWindowCall _WORD1 _STRING _RESULT1 _RESULT2
		Push `${_WORD1}`
		Push `${_STRING}`
		Call EnhancedFindWindow
		Pop ${_RESULT1}
		Pop ${_RESULT2}
	!macroend

  ; input, save variables
  Exch  $0   # part of the class name to search for
  Exch
  Exch  $1   # starting offset
  Push  $2   # length of $0
  Push  $3   # window handle
  Push  $4   # class name
  Push  $5   # temp
 
  ; set up the variables
  StrCpy  $4  0
  StrLen  $2  $0
 
 ; loop to search for open windows
 search_loop:
  FindWindow  $3  ""  ""  0  $3
   IntCmp  $3  0  search_failed
    IsWindow  $3  0  search_loop
     System::Call 'user32.dll::GetClassName(i r3, t .r4, i ${NSIS_MAX_STRLEN}) i .n'
     StrCmp  $4  0  search_loop
     StrCpy  $5  $4  $2  $1
     StrCmp  $0  $5  search_end  search_loop
 
 ; no matching class-name found, return "failed"
 search_failed:
  StrCpy  $3  "failed"
  StrCpy  $4  "failed"
 
 ; search ended, output and restore variables
 search_end:
  StrCpy  $1  $3
  StrCpy  $0  $4
  Pop  $5
  Pop  $4
  Pop  $3
  Pop  $2
  Exch  $1
  Exch
  Exch  $0
FunctionEnd