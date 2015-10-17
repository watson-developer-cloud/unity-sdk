@echo off
echo Running UnitySDK UnitTest...

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET "UNITY_EXE=C:\Program Files\Unity\Editor\Unity.exe"

"%UNITY_EXE%" -batchmode -executemethod RunUnitTest.All -projectPath %PROJECT%
IF "%ERRORLEVEL%"=="0" goto success
echo "UnitTest FAILED!"
exit /B 1
:success
echo 'UnitTest COMPLETED!"
exit /B 0

