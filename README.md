# BluePrintOne

Planning and Design Application for Windows with CAD Design, Planning, and Project Management.

## Building Windows Installer

1. Run `create-installer.bat`
2. BluePrintOne.exe will be in `installer/` folder
3. This is a portable app - no installation needed

## Running the App

1. Double-click `BluePrintOne.exe`
2. Or run `RUN.bat` to build and run

## Creating a Release

1. Update version in `Installer.iss` and `Updater.cs`
2. Commit and push changes
3. Create and push a tag: `git tag v1.0.0 && git push origin v1.0.0`
4. GitHub Actions will automatically build and publish the installer

## Auto-Updates

The app checks for updates on startup from: https://github.com/BuildOneRobotics/BluePrintOne/releases
