@ECHO OFF
SETLOCAL EnableDelayedExpansion

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"

REM We only pack builds that were made in Release mode.
FOR %%P IN (Car.Components Car) DO (
	SET ASSEMBLYFILE=%%P\bin\Release\%%P.dll
	SET NUSPEC=%%P\%%P.nuspec
	SET DROPDIR=

	@ECHO ON
	CALL sub_pack_and_push.cmd !ASSEMBLYFILE! !NUSPEC! !DROPDIR! 
	@ECHO OFF
)
