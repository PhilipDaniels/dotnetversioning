# This script is designed as a starter-point for customization. It will:
#
#   1. Update the <dependencies> in all nuspec files under this directory, based on the
#      packages.config used in that project.
#   2. Build all solutions in this folder in Release mode.
#   3. Run "nuget pack" on all nuspecs it can find, building both ordinary and symbols packages
#      and dropping the packages into $dropDir, as specified by the LOCAL_NUGET_DIR environment
#      variable (if set), or the same folder as the script, if not set.
#   4. Push the packages to a NuGet Source specified in the LOCAL_NUGET_SOURCE environment
#      variable (if set).
#
# You should consult the documentation for your NuGet server as to how to push packages and/or
# their symbols. For example, with ProGet http://inedo.com/proget you should just push the symbols
# package. Alternatives are described at http://docs.nuget.org/Contribute/Ecosystem

$config = 'Release'                                     # MSBuild configuration to pack.
$nuspecs = (Get-ChildItem -Path "*.nuspec" -Recurse)    # What to pack.
$dropDir = $env:LOCAL_NUGET_DIR                         # Where to drop the nupkg files...
if (!$dropDir) { $dropDir = $PSScriptRoot }             # ...if not set, use script dir.


# Ensure all <dependencies> in nuspec files are up to date.
updeps -r


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


# Ensure we build before packing. It's easy to forget.
$msBuildExe = Find-MSBuild
Get-ChildItem -Path "*.sln" | % { & $msBuildExe /nologo /verbosity:minimal /p:Configuration=$config $_ ;  Write-Host "Build of $_ complete." }


function Push-Pkg
{
    param([string]$nugetSrc, [string]$pkg)

    if (!$nugetSrc)
    {
        Write-Host "`$nugetSrc is not set, cannot push $pkg"
    }
    else
    {
        if (Test-Path $pkg)
        {
            $cmd = if ($nugetSrc.StartsWith('http', [StringComparison]::OrdinalIgnoreCase)) { "push" } else { "add" }
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

        # You will probably need to adjust what you are pushing. A local hierarchical folder, for
        # example, will only accept one package, the second push will be ignored.
        #Push-Pkg $nugetSrc $pkg
        Push-Pkg $nugetSrc $sym
    }
    else
    {
        Write-Warning "`$nugetSrc is not set, cannot push to NuGet feed."
    }
}
