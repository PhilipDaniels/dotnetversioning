@ECHO OFF
SETLOCAL EnableDelayedExpansion

REM This script is structured so that it can be called as a sub-batch.

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"


REM We expect to be told where the finished assembly is, the name of nuspec file,
REM amd the directory to drop packages in.
SET ASSEMBLYFILE=%1
SET NUSPEC=%2
SET DROPDIR=%3

IF "%DROPDIR%" == "" SET DROPDIR=%LOCAL_NUGET_FEED%
IF "%DROPDIR%" == "" SET DROPDIR=%~dp0


REM Derive LIB from the name of the nuspec (we assume everything is setup to match
REM by convention). We need this to figure out the name of the nupkg file.
REM Example: If %NUSPEC% is Foo\Example1.NuSpec then LIB is Example1.
FOR %%i IN ("%NUSPEC%") DO (
	SET LIB=%%~ni
)


REM The version is determined by extracting it from the AssemblyInformationalVersion attribute.
dnv.exe --read %ASSEMBLYFILE% --what aiv > tmp.ver
SET /P AIV= < tmp.ver


@ECHO ON
nuget pack %NUSPEC% -Version %AIV% -Symbols -Verbosity detailed -OutputDirectory %DROPDIR%
REM nuget pack %NUSPEC% -Version %AIV% -Symbols -Verbosity detailed -OutputDirectory %DROPDIR% -Properties Configuration=Release;Foo=Bar
@ECHO OFF



REM =====================================================================================================================
REM =====================================================================================================================
REM =====================================================================================================================


REM Do we want to push? It is unnecessary to push to LOCAL_NUGET_FEED because we
REM have already dropped there. But if we are in CI, then we can push to the
REM corporate MyGet server.
SET bamboo.planRepository.branchName=TESTING

IF DEFINED bamboo.planRepository.branchName (
	SET NUPKGFILE=!DROPDIR!\!LIB!.%AIV%.nupkg
	SET SYMBOLSFILE=!DROPDIR!\!LIB!.%AIV%.symbols.nupkg
	SET FEED=x

	IF NOT "!FEED!" == "" (
		IF EXIST !NUPKGFILE! (
			@ECHO ON
			REM nuget push "!NUPKGFILE!" -Source "%FEED%"

			@ECHO OFF
		) ELSE (
			ECHO The nupkg file !NUPKGFILE! does not exist.
		)

		IF EXIST !SYMBOLSFILE! (
			@ECHO ON
			REM nuget push "!SYMBOLSFILE!" -Source "%FEED%"
			@ECHO OFF
		) ELSE (
			ECHO The symbols file !SYMBOLSFILE! does not exist.
		)
	)
)
