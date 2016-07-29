#! /bin/sh
project="unity-sdk-travis"

BASE_URL=https://bitbucket.org/Unity-Technologies/unitytesttools/get/295e5eea2eee.zip
DIRECTORY_NAME="Unity-Technologies-unitytesttools-295e5eea2eee"
FILENAME="unityTestTools.zip"

echo "Downloading from $BASE_URL"
curl -o $FILENAME $BASE_URL
if [ $? = 0 ] ; then
  echo "Attempting to unzip $FILENAME"
  unzip $FILENAME
  if[$? = 0] ; then
    echo "Unzip of $FILENAME succeeded!"
    echo "Attempting to move $FILENAME to test project"
    mv $DIRECTORY_NAME/Assets/UnityTestTools Travis/UnityTestProject/Assets/
    if[$? = 0]; then
      echo "Move $FILENAME to test project succeed!"
      exit 0
    else
      echo "Move $FILENAME to test project failed!"
      exit 1
    fi
  else
    echo "Unzip of $FILENAME failed!"
    exit 1
  fi
else
  echo "UnityTestTools download FAILED! Exited with $?"
  exit 1
fi
