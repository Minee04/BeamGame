@echo off
echo Building BeamGame...
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" BeamGame.sln /p:Configuration=Debug /t:Build /v:minimal /nologo

if %errorlevel% equ 0 (
    echo.
    echo Build successful! Starting game...
    echo.
    start "" "BeamGame\bin\Debug\BeamGame.exe"
) else (
    echo.
    echo Build failed!
    pause
)
