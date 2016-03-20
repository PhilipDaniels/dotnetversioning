# Runs 'NuGet pack' for all nuspecs found under this directory, placing the created
# packages into $dropDir (you should define an environment variable called LOCAL_NUGET_DIR
# to be your drop folder, then set it up as a feed in NuGet).

# We only pack builds that were made in Release mode.
$config = 'Release'

# What to pack.
$nuspecs = (Get-ChildItem -Path "*.nuspec" -Recurse)

# Where to drop the nupkg files.
$dropDir = $env:LOCAL_NUGET_DIR
if (!($dropDir))
{
	$dropDir = $PSScriptRoot
}


# -------------------------------------------------------------------------------
# You should not need to edit anything below here.


# Ensure all <dependencies> in nuspec files are up to date.
updeps -r


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

	# Do we want to push? Only do this if running on the Bamboo CI server.
	$bambr = ${env:bamboo.planRepository.branchName}
	if ($bambr)
	{
		$pkg = "$dropDir\$project.$ver.nupkg"
		$sym = "$dropDir\$project.$ver.symbols.nupkg"
		$feed = $null

		if ($feed)
		{
			if (Test-Path $pkg)
			{
				Write-Host "nuget push $pkg -Source $feed"
				nuget push $pkg -Source $feed
			}

			if (Test-Path $sym)
			{
				Write-Host "nuget push $sym -Source $feed"
				nuget push $sym -Source $feed
			}
		}
		else
		{
			Write-Warning "`$feed is not set, cannot push to NuGet feed."
		}
	}
}
