@ECHO OFF
setlocal

if "%1" == "" goto :USAGE

pushd Release\%1

copy ..\..\README.md
copy ..\..\CommandLine.%1.nuspec .
nuget pack "CommandLine.%1.nuspec" -properties Version=%APPVEYOR_BUILD_VERSION%
if errorlevel 1 popd&exit 1 /b
goto :END

:USAGE

:END
popd
