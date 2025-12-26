@echo off
echo Building TicTacToe...
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" TicTacToe.sln /p:Configuration=Debug /t:Build /v:minimal /nologo

if %errorlevel% equ 0 (
    echo.
    echo Build successful! Starting game...
    echo.
    start "" "TicTacToe\bin\Debug\TicTacToe.exe"
) else (
    echo.
    echo Build failed!
    pause
)
