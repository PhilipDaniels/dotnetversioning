# Runs 'NuGet pack' for all nuspecs found under this directory, placing the created
# packages into $dropDir (you should define an environment variable called LOCAL_NUGET_DIR
# to be your drop folder, then set it up as a feed in NuGet).

# We only pack builds that were made in Release mode, and the solution is built before packing
# because it is easy to forget to do so.
$config = 'Release'

function Find-MSBuild
{
    foreach ($dotNetVersion in @("14.0", "12.0", "4.0", "3.5", "2.0"))
    {
        $regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
        $regProperty = "MSBuildToolsPath"
        $msbuild = Join-Path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"
        if (Test-Path $msbuild)
        {
            Write-Host "Found MSBuild.exe at $msbuild"
            return $msbuild
        }
    }
}

# Rebuild before packing. It's easy to forget.
$msBuildExe = Find-MSBuild
Get-ChildItem -Path "*.sln" | % { & $msBuildExe /nologo /verbosity:minimal /p:Configuration=$config $_ ;  Write-Host "Build of $_ complete." }



# What to pack.
$nuspecs = (Get-ChildItem -Path "*.nuspec" -Recurse)

# Where to drop the nupkg files.
$dropDir = $env:LOCAL_NUGET_DIR
if (!($dropDir))
{
    $dropDir = $PSScriptRoot
}


# -------------------------------------------------------------------------------

# Ensure all <dependencies> in nuspec files are up to date.
updeps -r


function Push-Pkg
{
    param([string]$nugetSrc, [string]$pkg)

    if (!$nugetSrc)
    {
        Write-Host "`$nugetSrc is not set, cannot push $pg"
    }
    else
    {
        if (Test-Path $pkg)
        {
            $cmd = if ($nugetSrc.StartsWith('http', [StringComparison]::CurrentCultureIgnoreCase)) { "push" } else { "add" }
            Write-Host "nuget $cmd $pkg -Source $nugetSrc"
            nuget $cmd $pkg -Source $nugetSrc
        }
        else
        {
            Write-Warning "$pkg does not exist, cannot push to $nugetSrc"
        }
    }
}

foreach ($nuspec in $nuspecs)
{
    Write-Host "Packing $nuspec..."

    # This works if we are following the usual conventions.
    $projectDir = Split-Path $nuspec -Parent
    $project = (Split-Path $nuspec -Leaf).Replace('.nuspec', '')
    $asm = "$projectDir\bin\$config\$project.dll"
    if (!(Test-Path $asm))
    {
        Write-Host "The output assembly $asm does not exist."
        Exit 1
    }
    
    # Get the version from the built assembly. We want the "pre" bit from AIV,
    # but get rid of the reset of the message - commit shas etc.
    $ver = dnv --read $asm --what aiv
    $ver = $ver.substring(0, $ver.indexof(','))
    
    # Pack and drop directly into the destination folder.
    Write-Host "nuget pack $nuspec -Version $ver -Symbols -Verbosity normal -OutputDirectory $dropDir -Properties Configuration=$config"
    nuget pack $nuspec -Version $ver -Symbols -Verbosity normal -OutputDirectory $dropDir -Properties Configuration=$config


    # Now, optionally push to a true NuGet server or use the new "add" command in NuGet 3.3 to push
    # to a hierarchical folder structure as specified in environment variable LOCAL_NUGET_SOURCE.
    # See http://blog.nuget.org/20150922/Accelerate-Package-Source.html
    # and https://docs.nuget.org/consume/command-line-reference#add-command

    $nugetSrc = $env:LOCAL_NUGET_SOURCE
    if (!($nugetSrc) -and ${env:bamboo.planRepository.branchName})
    {
        $nugetSrc = $null # change this to the address of the corporate NuGet server
    }
    
    if ($nugetSrc)
    {
        $pkg = "$dropDir\$project.$ver.nupkg"
        $sym = "$dropDir\$project.$ver.symbols.nupkg"

        Push-Pkg $nugetSrc $pkg
        Push-Pkg $nugetSrc $sym
    }
    else
    {
        Write-Warning "`$nugetSrc is not set, cannot push to NuGet feed."
    }
}
