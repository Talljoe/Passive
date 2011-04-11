@echo off
IF "%1"=="" (
  ECHO No version number supplied.
  GOTO :EOF
)

tools\nant\nant.exe tag dist publish -D:release.version=%1