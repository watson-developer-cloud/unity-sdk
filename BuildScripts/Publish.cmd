//@echo off
echo Publishing UnitySDK ...

SET SRC=%CD%\..\
IF "%TARGET%"=="" SET "TARGET=%CD%\..\..\watson_unity_sdk_public\"

COPY /Y %SRC%license.txt %TARGET%
IF NOT "%ERRORLEVEL%"=="0" goto error

COPY /Y %SRC%.gitignore %TARGET%
IF NOT "%ERRORLEVEL%"=="0" goto error

COPY /Y %SRC%readme.md %TARGET%
IF NOT "%ERRORLEVEL%"=="0" goto error

XCOPY /Y /E %SRC%Assets\Watson %TARGET%Assets\Watson\
IF NOT "%ERRORLEVEL%"=="0" goto error

XCOPY /Y /E %SRC%ProjectSettings %TARGET%ProjectSettings\
IF NOT "%ERRORLEVEL%"=="0" goto error

:success
echo 'Publish COMPLETED!"
exit /B 0

:error
echo "Publish FAILED!"
exit /B 1

