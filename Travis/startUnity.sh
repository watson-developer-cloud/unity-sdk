#! /bin/sh
echo "Attempting to start Unity"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -logfile $(pwd)/startUnity.log &
sleep 30
sudo killall Unity

echo "Start Unity log"
cat $(pwd)/startUnity.log

if [ $? = 0 ] ; then
    echo "Starting Unity SUCCEEDED! Exited with $?"
    exit 0
else
    echo "Starting Unity FAILED! Exited with $?"
    exit 1
fi