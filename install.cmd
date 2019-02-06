@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%src\Tambala\bin\x64\release
rem Change the destination path as needed and use this file to install.
set DEST=D:\Utility\Tambala
if exist "%BIN%\Tambala.exe" (
  xcopy "%BIN%\Tambala.exe" %DEST% /Y
  xcopy "%BIN%\*.dll" %DEST% /E /Y
)

rem Copy CHM file if it's there
if exist "%ROOT%\Help\Tambala.Reference.chm" (
  xcopy "%ROOT%\Help\Tambala.Reference.chm" %DEST% /Y
)

dir %DEST%
endlocal
pause
