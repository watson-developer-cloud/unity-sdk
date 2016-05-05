#! /bin/sh
project="unity-sdk-travis"

if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
	echo '$TRAVIS_PULL_REQUEST is false, running tests'
  openssl aes-256-cbc -K $encrypted_984f19857b4c_key -iv $encrypted_984f19857b4c_iv -in Config.json.enc -out Travis/UnityTestProject/Assets/StreamingAssets/Config.json -d
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
else
	echo '$TRAVIS_PULL_REQUEST is not false ($TRAVIS_PULL_REQUEST), skipping tests'
fi




