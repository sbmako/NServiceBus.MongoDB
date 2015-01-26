@echo off
rem Helper script for those who want to run psake from cmd.exe
rem psake "default.ps1" "BuildHelloWord" "4.0" 

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

@ECHO OFF
SETLOCAL

SET POWRESHELL=powershell.exe -NoProfile -ExecutionPolicy Bypass -NonInteractive
SET PACKAGES=%~dp0\src\packages
@ECHO.
@ECHO Restoring NuGet Packages
"%~dp0\src\.nuget\NuGet.exe" restore "%~dp0\src\NServiceBus.MongoDB.sln" ||  GOTO BuildFailed

@ECHO.
@ECHO Executing psake build...
%POWRESHELL% -Command "& '%PACKAGES%\psake.4.4.1\tools\psake.ps1' default.ps1 %*; if ($psake.build_success -eq $false) { exit -1 } else { exit 0 }" ^
    ||  GOTO BuildFailed
@ECHO.

IF /I "%1"=="CleanAll" (
    @ECHO Removing Downloaded NuGet Packages
    %POWRESHELL% -Command "Dir '%PACKAGES%' | Where { $_.PSIsContainer } | ForEach { Remove-Item $_.FullName -Force -Recurse }" ||  GOTO BuildFailed
)

@ECHO.
@ECHO **** BUILD SUCCESSFUL ****
GOTO:EOF

:BuildFailed
@ECHO.
@ECHO *** BUILD FAILED ***
EXIT /B -1