#! /bin/sh
project="unity-sdk-travis"

echo "Attempting to install Watson Developer Cloud Unity SDK into the test project..."
mkdir -p Travis/UnityTestProject/Assets/Watson/
git clone https://github.com/watson-developer-cloud/unity-sdk.git Travis/UnityTestProject/Assets/Watson/
if [ $? = 0 ] ; then
  echo "WDC Unity SDK install SUCCEEDED! Exited with $?"
  echo "Attempting to remove TravisBuild from Travis directory..."
  rm Travis/TravisBuild.cs
  if [ $? = 0 ] ; then
    echo "Removing travis build script SUCCEEDED! Exited with $?"
  else
    echo "Removing travis build script FAILED! Exited with $?"
    exit 1
  fi

  echo "Attempting to move Travis build script..."
  mkdir -p Travis/UnityTestProject/Assets/Scripts/Editor/
  mv Travis/UnityTestProject/Assets/Watson/Travis/TravisBuild.cs Travis/UnityTestProject/Assets/Scripts/Editor/TravisBuild.cs
  if [ $? = 0 ] ; then
    echo "Moving travis build script SUCCEEDED! Exited with $?"
    exit 0
  else
    echo "Moving travis build script FAILED! Exited with $?"
    exit 1
  fi
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi
