@ECHO OFF

REM Extract parts of a file. http://stackoverflow.com/questions/15567809
REM You can only extract path and filename from (1) a parameter of the BAT itself %1,
REM or (2) the parameter of a CALL %1 or (3) a local FOR variable %%a.
REM There are more things you can get, see SO.
SET file=C:\Users\l72rugschiri\Desktop\fs.cfg
FOR %%i IN ("%file%") DO (
	ECHO filedrive=%%~di
	ECHO filepath=%%~pi
	ECHO filename=%%~ni
	ECHO fileextension=%%~xi
)

REM Exit script without closing the command window.
EXIT /b

REM Determine name of the latest package.
for /f "delims=|" %%i in ('dir "%PKGDIR%\*.nupkg" /B /O:-D') do set RECENT=%%i
set RECENT=%RECENT:.symbols.nupkg=%
set FILEROOT=%RECENT:.nupkg=%
set FILEROOT=%THISDIR%%FILEROOT%
set SYMBOLS=%FILEROOT%.symbols.nupkg
set PKG=%FILEROOT%.nupkg


REM Set variable if not set.
SET DROPDIR=%3
IF "%DROPDIR%" == "" SET DROPDIR=%LOCAL_NUGET_FEED%


REM Set variable AIV from a file.
SET /P AIV= < tmp.ver


REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"


REM Check for an environment variable.
IF DEFINED bamboo.planRepository.branchName (


REM Find/replace regular expressions in files.


REM Delete a file, suppressing all output, even if it does not exist.
DEL tmp.ver > NUL 2>&1


REM Figure out git branch and commit. The ^ is cmd escape syntax.
REM See http://stackoverflow.com/questions/2323292/windows-batch-assign-output-of-a-program-to-a-variable?rq=1
REM for /f "delims=" %%i in ('%GITEXE% symbolic-ref --short head') do set BRANCH=%%i
REM for /f "delims=" %%i in ('%GITEXE% rev-parse --short^=12 head') do set COMMIT=%%i

