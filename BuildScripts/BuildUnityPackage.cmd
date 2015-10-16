@echo off
echo Building UnitySDK package...

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET UNITY_EXE='C:\Program Files\Unity\Editor\Unity.exe'

"%UNITY_EXE%" -batchmode -quit -exportPackage Assets/Plugins/Watson WatsonUnitySDK.unitypackage -projectPath %PROJECT%
echo %ERRORLEVEL%
IF "%ERRORLEVEL%"=="0" goto success
echo Package build FAILED!
exit /B 1
:success
echo Package build COMPLETED!
exit /B 0

