@ECHO OFF

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"

SET VER=1.0.7

CALL sub_set_version.cmd %VER% Car.Components\project.json
CALL sub_set_version.cmd %VER% Car\project.json

