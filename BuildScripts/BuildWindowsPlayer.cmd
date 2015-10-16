@echo off
echo Building Windows Player...

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET UNITY_EXE='C:\Program Files\Unity\Editor\Unity.exe'

mkdir %CD%\Clients\Windows\
"%UNITY_EXE%" -batchmode -quit -projectPath %PROJECT% -buildWindowsPlayer %CD%\Clients\Windows\UnityTest.exe

IF "%ERRORLEVEL%"=="0" goto success
echo Build FAILED!
exit /B 1
:success
echo Build COMPLETED!
exit /B 0

