$av = '1.0.7'
$verbose = '--verbose'

# Only make production releases on the Bamboo CI server when building the master branch.
$bambr = ${env:bamboo.planRepository.branchName}
if ($bambr -ne 'master')
{
	$av += '-pre{{UtcNow}}'
}

& dnv $verbose --avpat "$av" --write House.Basement\project.json --what av
& dnv $verbose --avpat "$av" --write House\project.json --what av


# It seems that in the dotnet/cli world we have to put this attribute in a different file.
$aiv = $av + ", Commit {{GitCommit:12}} on branch {{GitBranch}} at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}"

& dnv $verbose --aivpat "$aiv" --write House.Basement\Properties\AssemblyInfo.ver.cs --what aiv
& dnv $verbose --aivpat "$aiv" --write House\Properties\AssemblyInfo.ver.cs --what aiv
