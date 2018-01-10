@echo off
setlocal

cls

if "%1" == "" goto :USAGE
if "%1" == "base" set BUILD_TARGET=base
if "%1" == "fsharp" set BUILD_TARGET=fsharp

echo.
echo SKIP_RESTORE=%SKIP_RESTORE% ^<^< Set to true if have already restored packages
if "%SKIP_RESTORE%" == "" choice /T 5 /D Y /M "Continue?"

if "%SKIP_RESTORE%" == "true" goto :BUILD_NET
.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

:BUILD_NET
echo.

msbuild CommandLine.sln /p:Configuration=Release /p:OutputPath=%~dp0\release\%BUILD_TARGET%\net4x

if "%SKIP_RESTORE%" == "true" goto :BUILD_NETSTD
echo.
echo dotnet restore
dotnet restore

:BUILD_NETSTD
echo.
echo dotnet build --output %~dp0\release\%BUILD_TARGET%\netstandard1.5
dotnet build --configuration Release --output %~dp0release\%BUILD_TARGET%\netstandard1.5 --framework netstandard1.5 src\commandline

goto :END

:USAGE
echo.
echo Invalid arguments specified.
echo.
echo Usage: build <build_target>
echo  where <build_target> is base or fsharp

:END
