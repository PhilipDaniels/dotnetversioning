$av = '1.0.7'

# Only make production releases on the Bamboo CI server when building the master branch.
$bambr = ${env:bamboo.planRepository.branchName}
if ($bambr -ne 'master')
{
	$av += '-pre{{UtcNow}}'
}

# TODO: Figure out how to support this extended information on dotnet/cli.
# $av += ", Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}"

& dnv --verbose --avpat "$av" --write Car.Components\project.json --what av
& dnv --verbose --avpat "$av" --write Car\project.json --what av



