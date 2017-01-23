#! /bin/sh
project="unity-sdk-travis"

if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
  echo '$TRAVIS_PULL_REQUEST is false, running tests'
  echo "Attempting to create Streaming Assets directory..."
  mkdir -p Travis/UnityTestProject/Assets/StreamingAssets/
  if [ $? = 0 ] ; then
    echo "Creating StreamingAssets directory COMPLETED! Exited with $?"
    echo "Attempting to decrypt config..."
    openssl aes-256-cbc -K $encrypted_984f19857b4c_key -iv $encrypted_984f19857b4c_iv -in Config.json.enc -out Travis/UnityTestProject/Assets/StreamingAssets/Config.json -d
    if [ $? = 0 ] ; then
      echo "Decrypting config COMPLETED! Exited with $?"
    else
      echo "Decrypting config FAILED! Exited with $?"
      exit 1
    fi
  else
    echo "Creating StreamingAssets directory FAILED! Exited with $?"
    exit 1
  fi

  echo "Attempting to run UnitySDK integration Tests..."
  /Applications/Unity/Unity.app/Contents/MacOS/Unity \
    -batchmode \
    -force-opengl \
    -silent-crashes \
    -logFile $(pwd)/integrationTests.log \
    -projectPath $(pwd)/Travis/UnityTestProject \
    -executemethod IBM.Watson.DeveloperCloud.Editor.TravisIntegrationTests.RunTests \
    -quit
  if [ $? = 0 ] ; then
    echo "UnitTest COMPLETED! Exited with $?"
    exit 0
  else
    echo "UnitTest FAILED! Exited with $?"
    echo 'Logs tests'
    cat $(pwd)/integrationTests.log
    exit 1
  fi
else
  echo '$TRAVIS_PULL_REQUEST is not false ($TRAVIS_PULL_REQUEST), skipping tests'
fi
