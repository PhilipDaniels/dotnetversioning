# This script is designed as a starter-point for customization. It will:
#
#   1. Run "dotnet pack" to compile and pack all projects under this directory, which are found
#      by searching for project.json files. The nupkg files are written to $dropDir, as specified
#      by the LOCAL_NUGET_DIR environment variable (if set), or the same folder as the script, if
#      not set.
#   2. Push the packages to a NuGet Source specified in the LOCAL_NUGET_SOURCE environment
#      variable (if set).
#
# You should consult the documentation for your NuGet server as to how to push packages and/or
# their symbols. For example, with ProGet http://inedo.com/proget you should just push the symbols
# package. Alternatives are described at http://docs.nuget.org/Contribute/Ecosystem


$config = 'Release'                                         # MSBuild configuration to pack.
$projects = (Get-ChildItem -Path "project.json" -Recurse)   # What to pack.
$dropDir = $env:LOCAL_NUGET_DIR                             # Where to drop the nupkg files...
if (!$dropDir) { $dropDir = $PSScriptRoot }                 # ...if not set, use script dir.



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

foreach ($project in $projects)
{
    # Pack and drop directly into the destination folder.
    Write-Host "dotnet pack $project --output $dropDir --configuration $config"
    $messages = dotnet pack $project --output $dropDir --configuration $config
    
    # Determine which packages were created.
    $pkgs = @()
    foreach ($m in $messages)
    {
        if ($m -match '.* -> (?<pkg>.*)')
        {
            # Something like: House.Basement-> \\Hyperbox\Data\LocalNuGetDir\House.Basement.1.0.7-pre160320-171250.nupkg
            $pkgs += $Matches['pkg']
        }
        
        # Ensure output of dotnet pack is written to stdout.
        Write-Host $m
    }
    
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
        # You will probably need to adjust what you are pushing. A local hierarchical folder, for
        # example, will only accept one package, the second push will be ignored, whereas ProGet
        # allows a second push to overwrite the first.
        foreach ($pkg in $pkgs)
        {
            if ($pkg.EndsWith('.symbols.nupkg', [StringComparison]::OrdinalIgnoreCase))
            {
                Push-Pkg $nugetSrc $pkg
            }
        }
    }
    else
    {
        Write-Warning "`$nugetSrc is not set, cannot push to NuGet feed."
    }
}
