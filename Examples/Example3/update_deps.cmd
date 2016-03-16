@ECHO OFF
SETLOCAL EnableDelayedExpansion

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"


FOR /R %%F IN (*.nuspec) DO (
    SET PKGS=%%~dF%%~pFpackages.config

    IF EXIST %%F (
        IF EXIST !PKGS! (
            echo Updating dependencies in %%F
            updeps.exe %%F !PKGS!
        )
    )
)
