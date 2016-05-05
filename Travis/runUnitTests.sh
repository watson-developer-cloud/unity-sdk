#! /bin/sh
project="unity-sdk-travis"

echo "Changing directory to testProject directory. pwd: $(pwd)"
cd Travis/UnityTestProject

echo "Running UnitySDK UnitTests...  pwd: $(pwd)"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -executemethod RunUnitTest.All \
  -projectPath $(pwd) \
  -quit
if [ $? = 0 ] ; then
  echo "UnitTest COMPLETED! Exited with $?"
	exit 0
else
  echo "UnitTest FAILED! Exited with $?"
  exit 1
fi
