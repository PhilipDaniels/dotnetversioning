# Runs 'NuGet pack' for all projects found under this directory, placing the created
# packages into $dropDir (you should define an environment variable called LOCAL_NUGET_DIR
# to be your drop folder, then set it up as a feed in NuGet).

# We only pack builds that were made in Release mode.
$config = 'Release'

# What to pack.
$projects = (Get-ChildItem -Path "project.json" -Recurse)

# Where to drop the nupkg files.
$dropDir = $env:LOCAL_NUGET_DIR
if (!($dropDir))
{
	$dropDir = $PSScriptRoot
}


# -------------------------------------------------------------------------------
# You should not need to edit anything below here.

foreach ($project in $projects)
{
	# Pack and drop directly into the destination folder.
	Write-Host "dotnet pack $project --output $dropDir --configuration $config"
	$messages = dotnet pack $project --output $dropDir --configuration $config          	# --versionSuffix SFX
	
	# Determine which packages were created.
	$pkgs = @()
	foreach ($m in $messages)
	{
		if ($m -match '.* -> (?<pkg>.*)')
		{
			# Car.Components -> \\Hyperbox\Data\LocalNuGetFeed\Car.Components.1.0.7-pre160320-171250.nupkg
			$pkgs += $Matches['pkg']
		}
		
		# Ensure output of dotnet pack is written to stdout.
		Write-Host $m
	}
	
	# Do we want to push? Only do this if running on the Bamboo CI server
	# and the NuGet feed is set.
    $feed = $null
	$bambr = ${env:bamboo.planRepository.branchName}
	if ($bambr)
	{
		if ($feed)
		{
			foreach ($pkg in $pkgs)
			{
				Write-Host "nuget push $pkg -Source $feed"
				nuget push $pkg -Source $feed
			}
		}
		else
		{
			Write-Warning "`$feed is not set, cannot push to NuGet feed."
		}
	}
}
