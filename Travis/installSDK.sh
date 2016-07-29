#! /bin/sh
project="unity-sdk-travis"

echo "Attempting to install Watson Developer Cloud Unity SDK into the test project..."
mkdir -p Travis/UnityTestProject/Assets/Watson/
git clone https://github.com/watson-developer-cloud/unity-sdk.git Travis/UnityTestProject/Assets/Watson/
#git clone -b feature-97-integrationTesting --single-branch https://github.com/watson-developer-cloud/unity-sdk.git Travis/UnityTestProject/Assets/Watson/

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

  echo "Attempting to remove TravisIntegrationTests from Travis directory..."
  rm Travis/TravisIntegrationTests.cs
  if [ $? = 0 ] ; then
    echo "Removing travis build script SUCCEEDED! Exited with $?"
  else
    echo "Removing travis build script FAILED! Exited with $?"
    exit 1
  fi

  echo "Attempting to create Travis/UnityTestProject/Assets/Scripts/Editor/"
  mkdir -p Travis/UnityTestProject/Assets/Scripts/Editor/
  if [ $? = 0 ] ; then
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ SUCCEEDED! Exited with $?"

    echo "Attempting to move Travis build script..."
    mv Travis/UnityTestProject/Assets/Watson/Travis/TravisBuild.cs Travis/UnityTestProject/Assets/Scripts/Editor/TravisBuild.cs
    if [ $? = 0 ] ; then
      echo "Moving travis build script SUCCEEDED! Exited with $?"
      exit 0
    else
      echo "Moving travis build script FAILED! Exited with $?"
      exit 1
    fi

    echo "Attempting to move integration tests script..."
    mv Travis/UnityTestProject/Assets/Watson/Travis/TravisIntegrationTests.cs Travis/UnityTestProject/Assets/Scripts/Editor/TravisIntegrationTests.cs
    if [ $? = 0 ] ; then
      echo "Moving travis integration tests script SUCCEEDED! Exited with $?"
      exit 0
    else
      echo "Moving travis integration tests script FAILED! Exited with $?"
      exit 1
    fi
  else
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ FAILED! EXITED WITH $?"
  fi
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi
