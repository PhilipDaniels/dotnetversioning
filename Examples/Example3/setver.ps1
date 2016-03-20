$av = '2.1.0'
$afv = $av
$aiv = $av

# Only make production releases on the Bamboo CI server when building the master branch.
$bambr = ${env:bamboo.planRepository.branchName}
if ($bambr -ne 'master')
{
	$aiv += '-pre{{UtcNow}}'
}

$aiv += ", Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}"

& dnv --verbose --avpat "$av" --afvpat "$afv" --aivpat "$aiv" --write AssemblyInfo.ver.cs --what all
