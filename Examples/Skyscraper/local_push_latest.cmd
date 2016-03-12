@echo off
SETLOCAL EnableDelayedExpansion

REM http://ss64.com/nt/ for info on dos scripting, in particular http://ss64.com/nt/delayedexpansion.html
REM if you start to use IF statements - you will be confused :-)
REM
REM To run from a PowerShell prompt, do:  cmd /c .\local_push_latest.cmd


if not defined LOCAL_NUGET_FEED (
    echo The environment variable LOCAL_NUGET_FEED is not defined, so cannot push. Stopping.
	exit /b
)


REM Determine the directory that the script should be run against.
REM This is either the first parameter, or if none is specified
REM then the location of this script, which is specified by '%~dp0'
set PKGDIR=%1
if "%PKGDIR%" == "" set PKGDIR=%~dp0
echo PKGDIR = %PKGDIR%


REM Determine name of the latest package.
for /f "delims=|" %%i in ('dir "%PKGDIR%\*.nupkg" /B /O:-D') do set RECENT=%%i
set RECENT=%RECENT:.symbols.nupkg=%
set FILEROOT=%RECENT:.nupkg=%
set FILEROOT=%THISDIR%%FILEROOT%
set SYMBOLS=%FILEROOT%.symbols.nupkg
set PKG=%FILEROOT%.nupkg


REM echo Pkg = %PKG%, Symbols = %SYMBOLS%
@echo on
nuget push "%PKG%" -Source "%LOCAL_NUGET_FEED%"
nuget push "%SYMBOLS%" -Source "%LOCAL_NUGET_FEED%"
