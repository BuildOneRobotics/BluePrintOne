@echo off
echo Building BluePrintOne Release...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
echo.
if exist "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe" (
    if not exist release mkdir release
    copy /Y "bin\Release\net8.0-windows\win-x64\publish\BluePrintOne.exe" "release\BluePrintOne.exe"
    echo Done! BluePrintOne.exe is in release\ folder
) else (
    echo Build failed!
)
pause
