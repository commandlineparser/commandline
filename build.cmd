@echo off

cls

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
echo Fake build.fsx
.\packages\FAKE\tools\Fake %*

if "%SKIP_RESTORE%" == "true" goto :BUILD_NETSTD
echo.
echo dotnet restore
dotnet restore

:BUILD_NETSTD
echo.
echo dotnet build
dotnet build --configuration Release --output Release\netstandard1.5 --framework netstandard1.5 src\commandline