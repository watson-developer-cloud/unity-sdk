#! /bin/sh
project="unity-sdk-travis"

if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
  echo "Attempting to install credentials"
  git clone https://$CREDENTIALS_GITHUB_TOKEN@github.ibm.com/germanatt/sdk-credentials.git Travis/sdk-credentials/

  if [ $? = 0 ] ; then
    echo "Credentials install SUCCEEDED! Exited with $?"
    export IBM_CREDENTIALS_FILE=$(pwd)/Travis/sdk-credentials/ibm-credentials.env
  else
    echo "Credentials install FAILED! Exited with $?"
    exit 1
  fi
else
  echo "This is a pull request - do not attempt to install credentials"
fi

echo "Attempting to install IBM Watson SDK for Unity into the test project..."
mkdir -p Travis/UnityTestProject/Assets/Watson/
mkdir -p Travis/UnityTestProject/Assets/IBMSdkCore/
git clone -b $TRAVIS_BRANCH https://github.com/watson-developer-cloud/unity-sdk.git Travis/UnityTestProject/Assets/Watson/
git clone https://github.com/IBM/unity-sdk-core.git Travis/UnityTestProject/Assets/IBMSdkCore/

if [ $? = 0 ] ; then
  echo "WDC Unity SDK install SUCCEEDED! Exited with $?"

  echo "Attempting to create Travis/UnityTestProject/Assets/Scripts/Editor/"
  mkdir -p Travis/UnityTestProject/Assets/Scripts/Editor/
  if [ $? = 0 ] ; then
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ SUCCEEDED! Exited with $?"
  else
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ FAILED! EXITED WITH $?"
  fi
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi