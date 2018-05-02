#! /bin/sh
echo "Attempting to create directory for empty project Travis/watson-unity-sdk-project..."
mkdir -p Travis

echo "Attempting to add project"
git clone https://github.com/mediumTaj/watson-unity-sdk-project.git Travis/watson-unity-sdk-project

if [ $? = 0 ] ; then
  echo "Project creation COMPLETED! Exited with $?"
  exit 0
else
  echo "Project creation FAILED! Exited with $?"
  echo 'Logs create project'
  cat $(pwd)/createProject.log
  exit 1
fi
