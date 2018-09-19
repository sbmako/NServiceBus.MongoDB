@ECHO ON
@SETLOCAL

@ECHO.
@ECHO  **** STARTING BUILD  ****

@SET THIS_SCRIPT_FOLDER=%~dp0
CD /D "%THIS_SCRIPT_FOLDER%"

REM Install the Cake build system as a local pure .Net Core tool
SET CakeVersion=0.30.0
SET CakeCommandPath=.\tools\cake_%CakeVersion%
SET Cake=%cakeCommandPath%\dotnet-cake.exe

IF NOT EXIST "%Cake%" (
    dotnet tool install Cake.Tool --version %cakeVersion% --tool-path "%cakeCommandPath%"
)

REM Run the Cake build script
SET Target=%1
%Cake% build.cake -target=%* || GOTO BuildFailed

@REM Clean up tools when the target is "cleanall"
@IF /I "%Target%" == "cleanall" (
    ECHO "Removing tools..."
    RD /q /s .\tools
)

@ECHO.
@ECHO **** BUILD SUCCESSFUL ****
GOTO:EOF

:BuildFailed
@ECHO.
@ECHO *** BUILD FAILED ***
EXIT /B -1