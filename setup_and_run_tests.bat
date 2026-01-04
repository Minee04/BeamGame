@echo off
echo Installing MSTest NuGet packages...
cd /d "%~dp0"

if not exist "packages" mkdir packages

echo Downloading MSTest.TestFramework...
powershell -Command "Invoke-WebRequest -Uri 'https://www.nuget.org/api/v2/package/MSTest.TestFramework/2.2.10' -OutFile 'packages\MSTest.TestFramework.2.2.10.zip'"

echo Downloading MSTest.TestAdapter...
powershell -Command "Invoke-WebRequest -Uri 'https://www.nuget.org/api/v2/package/MSTest.TestAdapter/2.2.10' -OutFile 'packages\MSTest.TestAdapter.2.2.10.zip'"

echo Extracting packages...
powershell -Command "Expand-Archive -Path 'packages\MSTest.TestFramework.2.2.10.zip' -DestinationPath 'packages\MSTest.TestFramework.2.2.10' -Force"
powershell -Command "Expand-Archive -Path 'packages\MSTest.TestAdapter.2.2.10.zip' -DestinationPath 'packages\MSTest.TestAdapter.2.2.10' -Force"

echo Building test project...
"%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" BeamGame.Tests\BeamGame.Tests.csproj /p:Configuration=Debug /t:Build /v:minimal /nologo

echo.
echo Running tests...
"%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "BeamGame.Tests\bin\Debug\BeamGame.Tests.dll"

pause
