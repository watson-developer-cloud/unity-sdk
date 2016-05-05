#! /bin/sh
project="unity-sdk-travis"

echo "Changing directory to testProject directory. pwd: $(pwd)"
cd Travis/UnityTestProject

echo "Installing Watson Developer Cloud Unity SDK into the test project. pwd: $(pwd)"
git clone https://github.com/watson-developer-cloud/unity-sdk.git Assets/Watson/
if [ $? = 0 ] ; then
  echo "WDC Unity SDK install SUCCEEDED! Exited with $?"
  exit 0
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi
