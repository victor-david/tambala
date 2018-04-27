@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%Source\DrumMaster\bin\release
rem Change the destination path as needed and use this file to install.
set DEST=D:\Utility\DrumMaster
if exist "%BIN%\DrumMaster.exe" (
  xcopy "%BIN%\DrumMaster.exe" %DEST% /Y
  xcopy "%BIN%\*.dll" %DEST% /E /Y
)

dir %DEST%
endlocal
pause
