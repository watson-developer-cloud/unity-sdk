@echo off

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET "UNITY_EXE=C:\Program Files\Unity\Editor\Unity.exe"

echo Building OSX Player...
mkdir %PROJECT%Clients\OSX\
"%UNITY_EXE%" -batchmode -quit -projectPath %PROJECT% -buildOSX64Player %PROJECT%Clients\OSX\UnityTest.app
IF NOT "%ERRORLEVEL%"=="0" goto error


:success
echo Build COMPLETED!
exit /B 0
:error
echo Build FAILED!
exit /B 1

