@ECHO OFF

REM Set cwd to the directory of the script so that relative paths work as we expect.
PUSHD "%~dp0"

SET VER=2.1.0

CALL sub_set_version.cmd %VER% AssemblyInfo.ver.cs
