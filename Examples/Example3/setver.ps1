$av = '2.1.0'
$afv = $av
$aiv = $av

# Only make production releases on the Bamboo CI server when building the master branch.
$bambr = ${env:bamboo.planRepository.branchName}
$bambr = 'masater'
if ($bambr -ne 'master')
{
	$aiv += '-pre{{UtcNow}}'
}

$aiv += ", Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}"

$args = @("--verbose", "--avpat", $av, "--afvpat", $afv, "--aivpat", $aiv, "--write", "AssemblyInfo.ver.cs", "--what", "all")
Write-Host "dnv.exe $args"
& dnv.exe $args

#& dnv.exe --verbose --avpat $av --afvpat $afv --aivpat "`"$aiv`"" --write AssemblyInfo.ver.cs --what all
#& echoargs --verbose --avpat $av --afvpat $afv --aivpat "$aiv" --write AssemblyInfo.ver.cs --what all
#&echoargs $args
