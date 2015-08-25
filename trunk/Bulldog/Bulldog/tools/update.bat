@echo off
echo remember you only can update 250 times a day! :)
pushd ..\bin\release\web\
start C:\util\appengine-java-sdk-1.2.5\bin\appcfg.cmd update www
popd