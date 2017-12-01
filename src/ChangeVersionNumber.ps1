# Searches recursively through directories and renames the 'old' phrase to the 'new' 
# both, in file contents (solutions, projects and c# files) as well as in file names
#
# example
#   change version 0.6.0[.0] to 0.6.1[.0] starting in files from pwd
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

$old = $mao.ToString() + "." + $mio.ToString() + "." + $mno.ToString() + ".0";
$new = $man.ToString() + "." + $min.ToString() + "." + $mnn.ToString() + ".0";

Write-Host "Changing version number from " $old " to " $new

$old_py = $mao.ToString() + ", " + $mio.ToString() + ", " + $mno.ToString();
$new_py = $man.ToString() + ", " + $min.ToString() + ", " + $mnn.ToString();

Write-Host "In python: " $old_py " to " $new_py


## SEARCH AND REPLACE IN cs, shfbproj FILES
#Get-ChildItem -Path $dir\*  -Include AssemblyInfo.cs, *.shfbsproj -Recurse |
#    ForEach-Object {
#        If (Get-Content $_.FullName | Select-String -Pattern $old) 
#        {
#            (Get-Content $_ | ForEach-Object {$_ -replace $old, $new}) | Set-Content $_ 
#        }
#    }

# SEARCH AND REPLACE IN __init__.py FILES
Get-ChildItem -Path $dir\*  -Include __init__.py -Recurse |
    ForEach-Object {
        If (Get-Content $_.FullName | Select-String -Pattern $old_py) 
        {
            (Get-Content $_ | ForEach-Object {$_ -replace $old_py, $new_py}) | Set-Content $_ 
        }
    }


