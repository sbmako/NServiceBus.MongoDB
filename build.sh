#!/bin/bash
#set -x

#Ensure the working directory is same directory this script is in
buildRootPath="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$buildRootPath"

#Install the Cake build system as a local pure .Net Core tool
cakeVersion=0.30.0
cakeCommandPath=./tools/cake_$cakeVersion
cake="$cakeCommandPath/dotnet-cake"
which "$cake" || dotnet tool install Cake.Tool --version $cakeVersion --tool-path "$cakeCommandPath"

#Run the Cake build script
target=$(echo $1 | tr '[:upper:]' '[:lower:]')
$cake build.cake  --verbosity=Quiet -target=$target ${@:2} \
    && echo "######### BUILD SUCCESSFUL #########" \
    || { echo "######### BUILD FAILED! #########"; exit -1; }

#Clean up tools when the target is "cleanall"
if [ "$target" = "cleanall" ]; then
    echo "Removing tools..."
    rm -rf ./tools
fi