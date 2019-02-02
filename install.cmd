@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%Source\DrumMaster\bin\x64\release
rem Change the destination path as needed and use this file to install.
set DEST=D:\Utility\DrumMaster4.0
if exist "%BIN%\DrumMaster.exe" (
  xcopy "%BIN%\DrumMaster.exe" %DEST% /Y
  xcopy "%BIN%\*.dll" %DEST% /E /Y
)

rem Copy CHM file if it's there
if exist "%ROOT%\Help\DrumMaster.Reference.chm" (
  xcopy "%ROOT%\Help\DrumMaster.Reference.chm" %DEST% /Y
)

dir %DEST%
endlocal
pause
