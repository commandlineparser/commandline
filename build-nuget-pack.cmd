@ECHO OFF
setlocal

if "%1" == "" goto :USAGE
if "%2" == "" goto :USAGE

pushd Release\%1

copy ..\..\README.md
copy ..\..\%2.nuspec .
nuget pack "%2.nuspec" -properties Version=%APPVEYOR_BUILD_VERSION%
if errorlevel 1 popd&exit 1 /b
goto :END

:USAGE
echo build-nuget-pack <build_target> <nuspec_file_name>

:END
popd
