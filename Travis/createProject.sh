#! /bin/sh
project="unity-sdk-travis"

echo "Attempting to create directory for empty project Travis/UnityTestProject..."
mkdir -p Travis/UnityTestProject

echo "Attempting to create an empty project into Travis/UnityTestProject...."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/createProject.log \
  -createProject $(pwd)/Travis/UnityTestProject \
  -quit
if [ $? = 0 ] ; then
  echo "Project creation COMPLETED! Exited with $?"
  exit 0
else
  echo "Project creation FAILED! Exited with $?"
  echo 'Logs create project'
  cat $(pwd)/createProject.log
  exit 1
fi
