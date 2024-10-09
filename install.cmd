@echo off
rem Provides a simple means of installing the app.
rem You just need to get the executable and .dlls in one directory.
setlocal
set ROOT=%~dp0
set BIN=%ROOT%src\Tambala\bin\Release\net7.0-windows

rem Change the destination path as needed and use this file to install.
set DEST=D:\Utility\Tambala5.0
if exist "%BIN%\Tambala.exe" (
  xcopy "%BIN%\Tambala.exe" %DEST% /Y
  xcopy "%BIN%\*.json" %DEST% /Y
  xcopy "%BIN%\*.dll" %DEST% /E /Y
)

dir %DEST%
endlocal
pause