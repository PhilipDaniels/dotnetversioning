$av = '1.0.7'
$afv = $av
$aiv = $av

# Only make production releases on the Bamboo CI server when building the master branch.
$bambr = ${env:bamboo.planRepository.branchName}
if ($bambr -ne 'master')
{
	$aiv += '-pre{{UtcNow}}'
}

$aiv += ", Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}"

& dnv --verbose --avpat "$av" --afvpat "$afv" --aivpat "$aiv" --write Car.Components\Properties\AssemblyInfo.ver.cs --what all
& dnv --verbose --avpat "$av" --afvpat "$afv" --aivpat "$aiv" --write Car\Properties\AssemblyInfo.ver.cs --what all


