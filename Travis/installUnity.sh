#! /bin/sh

set -e

BASE_URL=https://netstorage.unity3d.com/unity
HASH=83fbdcd35118
VERSION=2018.2.13f1

download() {
 file=$1
 url="$BASE_URL/$HASH/$package"

 echo "Downloading from $url: "
 cd Unity
 curl -o `basename "$package"` "$url"
 cd ../
}

install() {
 package=$1
 filename=`basename "$package"`
 packagePath="Unity/$filename"
 if [ ! -f $packagePath ] ; then
   echo "$packagePath not found. downloading `basename "$packagePath"`"
   download "$package"
 fi

 echo "Installing "`basename "$package"`
 sudo installer -dumplog -package $packagePath -target /
}

if [ ! -d "Unity" ] ; then
 mkdir -p -m 777 Unity
fi

install "MacEditorInstaller/Unity-$VERSION.pkg"
