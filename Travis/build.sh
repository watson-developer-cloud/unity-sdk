#! /bin/sh
project="unity-sdk-travis"

ERROR_CODE=0

echo "Attempting to build $project for Windows..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/Travis/watson-unity-sdk-project/windowsBuild.log \
  -projectPath $(pwd)/Travis/watson-unity-sdk-project \
  -executemethod RunTravisBuild.Windows \
  -quit
if [ $? = 0 ] ; then
  echo "Build Windows COMPLETED! Exited with $?"
else
  echo "Build Windows FAILED! Exited with $?"
  ERROR_CODE=$?
fi

echo 'Logs from build'
cat $(pwd)/Travis/watson-unity-sdk-project/windowsBuild.log

echo "Attempting to build $project for OS X..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/Travis/watson-unity-sdk-project/osxBuild.log \
  -projectPath $(pwd)/Travis/watson-unity-sdk-project \
  -executemethod RunTravisBuild.OSX \
  -quit
if [ $? = 0 ] ; then
  echo "Build Mac COMPLETED! Exited with $?"
else
  echo "Build Mac FAILED! Exited with $?"
  ERROR_CODE=$?
fi

echo 'Logs from build'
cat $(pwd)/Travis/watson-unity-sdk-project/osxBuild.log

if [ $ERROR_CODE = 0 ] ; then
  echo "BUILDS SUCCEEDED! Exited with $ERROR_CODE"
  exit 0
else
  echo "BUILDS FAILED! Exited with $ERROR_CODE"
  exit 1
fi
