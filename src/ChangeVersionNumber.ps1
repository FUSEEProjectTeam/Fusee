# Searches recursively through directories and renames the old major, minor, maintenance
# number with the new ones in files such as AssemblyInfo.cs and other files
# containting version strings.
#
# example
#   recursively change version 0.6.0[.0] to 0.6.1[.0] starting in files from cwd
#
#  .\ChangeVersionNumber.ps1 -dir . -mao 0 -mio 6 -mno 0 -man 0 -min 6 -mnn 1

param(
    [string]$dir, # The directory where to start the rename
    [int]$mao, # old major number
    [int]$mio, # old minor number
    [int]$mno, # old maintenance number
    [int]$man, # new major number
    [int]$min, # new minor number
    [int]$mnn  # new maintenance number
)

# RENAME IN FILENAMES
#Get-ChildItem -Path $dir\*  -Include *.cs, *.csproj, *.sln, *.xml  -Recurse |
#    ForEach {
#        Rename-Item -Path $_.PSPath -NewName $_.Name.replace($old,$new)
#    }


######  RENAME in AssemblyInfo.cs files

$old = $mao.ToString() + "." + $mio.ToString() + "." + $mno.ToString() + ".0";
$new = $man.ToString() + "." + $min.ToString() + "." + $mnn.ToString() + ".0";

Write-Host "Recursively Changing version number from " $old " to " $new

Get-ChildItem -Path $dir\*  -Include AssemblyInfo.cs, *.shfbsproj -Recurse |
    ForEach-Object {
        If (Get-Content $_.FullName | Select-String -Pattern $old) 
        {
            (Get-Content $_ | ForEach-Object {$_ -replace $old, $new}) | Set-Content $_ 
        }
    }

######  RENAME in __init__.py files

$old_py = $mao.ToString() + ", " + $mio.ToString() + ", " + $mno.ToString();
$new_py = $man.ToString() + ", " + $min.ToString() + ", " + $mnn.ToString();

Write-Host "In python: from " $old_py " to " $new_py

Get-ChildItem -Path $dir\*  -Include __init__.py -Recurse |
    ForEach-Object {
        If (Get-Content $_.FullName | Select-String -Pattern $old_py) 
        {
            (Get-Content $_ | ForEach-Object {$_ -replace $old_py, $new_py}) | Set-Content $_ 
        }
    }


######  RENAME version number in Tools and documentation files (as in "Fusee_v0.6.1.exe")

$old_exe = "v" + $mao.ToString() + "." + $mio.ToString() + "." + $mno.ToString();
$new_exe = "v" + $man.ToString() + "." + $min.ToString() + "." + $mnn.ToString();

Write-Host "In build tools and documentation: from " $old_exe " to " $new_exe

Get-ChildItem -Path $dir\*  -Include *.md, BuildDistribution.cmd -Recurse |
    ForEach-Object {
        If (Get-Content $_.FullName | Select-String -Pattern $old_exe) 
        {
            (Get-Content $_ | ForEach-Object {$_ -replace $old_exe, $new_exe}) | Set-Content $_ 
        }
    }
