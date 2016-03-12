@echo off
SETLOCAL EnableDelayedExpansion

REM We expect one parameter, the input file containing the current version.
set VERFILE=%1

REM This is the current version.
set VER=1.0.7


REM When building locally we want to do a quick "pre" build with a timestamp, and we don't really
REM care about the Git info.
SET PATAV=%VER%-pre{{UtcNow}}
SET PATAFV=%VER%-pre{{UtcNow}}
SET PATAIV=%VER%-pre{{UtcNow}}
SET VERB=


REM When building on Bamboo, we want to do something different if we are on the "master" branch vs
REM any other branch. A build on master means a real, production release. Any other branch means
REM a pre-release build.
REM bamboo.repository.branch.name is an environment variable which Bamboo sets up.
if defined bamboo.repository.branch.name (
    SET VERB=--verbose
	
	if "bamboo.repository.branch.name" == "master" (
		SET PATAV=%VER%
		SET PATAFV=%VER%
		SET PATAIV="%VER%, Commit {{GitCommit}} on branch {{GitBranch}} at {{UtcNow}} UTC."
	) else (
		SET PATAIV="%VER%-pre{{UtcNow}}, Commit {{GitCommit}} on branch {{GitBranch}} at {{UtcNow}} UTC."
	)
)


@echo on
REM dotnet-setversion.exe %VERB% --avpat "%PATAV%" --avfpat "%PATAFV%" --aivpat "%PATAIV%" --write %VERFILE% all


@echo off



REM Figure out git branch and commit. The ^ is cmd escape syntax.
REM See http://stackoverflow.com/questions/2323292/windows-batch-assign-output-of-a-program-to-a-variable?rq=1
REM for /f "delims=" %%i in ('%GITEXE% symbolic-ref --short head') do set BRANCH=%%i
REM for /f "delims=" %%i in ('%GITEXE% rev-parse --short^=12 head') do set COMMIT=%%i
