@echo off
IF "%1"=="" (
  tools\nant\nant.exe publish
  GOTO :EOF
)

tools\nant\nant.exe tag publish -D:release.version=%1

