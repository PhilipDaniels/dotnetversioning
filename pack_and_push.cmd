@ECHO OFF
SETLOCAL EnableDelayedExpansion

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"

SET CONFIG=%1
IF "%CONFIG%" == "" SET CONFIG=Release


REM We only pack builds that were made in Release mode.
FOR %%P IN (Computer.MainUnit.Components Computer.MainUnit Computer) DO (
	SET ASSEMBLYFILE=%%P\bin\%CONFIG%\%%P.dll
	SET NUSPEC=%%P\%%P.nuspec
	SET DROPDIR=

	@ECHO ON
	CALL sub_pack_and_push.cmd %CONFIG% !ASSEMBLYFILE! !NUSPEC! !DROPDIR! 
	@ECHO OFF
)
