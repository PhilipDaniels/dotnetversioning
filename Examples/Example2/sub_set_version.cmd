@ECHO OFF
SETLOCAL EnableDelayedExpansion

REM This script is structured so that it can be called as a sub-batch.

REM We expect two parameters, the main version number stem and the path of the file to version.
SET VER=%1
SET VERFILE=%2


REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"


REM When building locally we want to do a quick "pre" build with a timestamp, and we don't really
REM care about the Git info. On my machine, asking for the git info increases the execution time
REM from 45 msec to 245 msec, which could be significant if you have lots of projects.
SET PATAV=%VER%-pre{{UtcNow}}
SET VERBOSE=--verbose


REM When building on Bamboo, we want to do something different if we are on the "master" branch vs
REM any other branch. A build on master means a real, production release. Any other branch means
REM a pre-release build.
REM bamboo.planRepository.branchName is an environment variable which Bamboo sets up.
IF DEFINED bamboo.planRepository.branchName (
    SET VERBOSE=--verbose
	
	IF "bamboo.repository.branch.name" == "master" (
		SET PATAIV=%VER%, Commit {{GitCommit}} on branch {{GitBranch}}, built at {{UtcNow}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}
	) ELSE (
		SET PATAIV=%VER%-pre{{UtcNow}}, Commit {{GitCommit}} on branch {{GitBranch}} by {{UserDomainName}}\\{{UserName}} on {{MachineName}}
	)
)


@ECHO ON
dnv.exe %VERBOSE% --avpat "%PATAV%" --write %VERFILE% --what av

