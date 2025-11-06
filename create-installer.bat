@echo off
echo Building BluePrintOne Windows Installer...
echo.

REM Build the application
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if not exist "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe" (
    echo Build failed!
    pause
    exit /b 1
)

REM Create installer directory
if not exist "installer" mkdir installer

REM Copy exe to installer folder
copy /Y "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe" "installer\BluePrintOne.exe"

echo.
echo ========================================
echo BluePrintOne.exe created successfully!
echo Location: installer\BluePrintOne.exe
echo ========================================
echo.
echo This is a portable Windows application.
echo No installation needed - just run BluePrintOne.exe
echo.
pause
