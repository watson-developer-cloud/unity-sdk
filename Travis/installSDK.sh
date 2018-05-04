#! /bin/sh
project="unity-sdk-travis"

echo "Attempting to install credentials"
git clone https://$GITHUB_TOKEN@github.ibm.com/germanatt/sdk-credentials.git Travis/sdk-credentials/

if [ $? = 0 ] ; then
  echo "Credentials install SUCCEEDED! Exited with $?"
else
  echo "Credentials install FAILED! Exited with $?"
  exit 1
fi

echo "Attempting to install Watson Developer Cloud Unity SDK into the test project..."
mkdir -p Travis/watson-unity-sdk-project/Assets
# git clone https://github.com/watson-developer-cloud/unity-sdk.git Travis/watson-unity-sdk-project/Assets/Watson
git clone -b $TRAVIS_BRANCH --single-branch https://github.com/watson-developer-cloud/unity-sdk.git Travis/watson-unity-sdk-project/Assets/Watson

if [ $? = 0 ] ; then
  echo "WDC Unity SDK install SUCCEEDED! Exited with $?"

  echo "Attempting to create Travis/watson-unity-sdk-project/Assets/Scripts/Editor/"
  mkdir -p Travis/watson-unity-sdk-project/Assets/Scripts/Editor/
  if [ $? = 0 ] ; then
    echo "Creating Travis/watson-unity-sdk-project/Assets/Scripts/Editor/ SUCCEEDED! Exited with $?"

    echo "Attempting to move Travis build script..."
    mv Travis/watson-unity-sdk-project/Assets/Watson/Travis/TravisBuild.cs Travis/watson-unity-sdk-project/Assets/Scripts/Editor/TravisBuild.cs
    if [ $? = 0 ] ; then
      echo "Moving travis build script SUCCEEDED! Exited with $?"
      exit 0
    else
      echo "Moving travis build script FAILED! Exited with $?"
      exit 1
    fi

    echo "Attempting to move integration tests script..."
    mv Travis/watson-unity-sdk-project/Assets/Watson/Travis/TravisIntegrationTests.cs Travis/watson-unity-sdk-project/Assets/Scripts/Editor/TravisIntegrationTests.cs
    if [ $? = 0 ] ; then
      echo "Moving travis integration tests script SUCCEEDED! Exited with $?"
      exit 0
    else
      echo "Moving travis integration tests script FAILED! Exited with $?"
      exit 1
    fi

  else
    echo "Creating Travis/watson-unity-sdk-project/Assets/Scripts/Editor/ FAILED! EXITED WITH $?"
  fi
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi
