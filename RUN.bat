@echo off
echo Building BluePrintOne for Windows...
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
if exist "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe" (
    echo.
    echo Running BluePrintOne...
    start "" "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe"
) else (
    echo Build failed!
)
pause
