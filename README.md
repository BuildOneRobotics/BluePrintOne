# BluePrintOne

Planning and Design Application for Windows with CAD Design, Planning, and Project Management.

## Building Release

1. Run `build-installer.exe`
2. BluePrintOne.exe will be in `release/` folder

## Creating a Release

1. Update version in `Installer.iss` and `Updater.cs`
2. Commit and push changes
3. Create and push a tag: `git tag v1.0.0 && git push origin v1.0.0`
4. GitHub Actions will automatically build and publish the installer

## Auto-Updates

The app checks for updates on startup from: https://github.com/BuildOneRobotics/BluePrintOne/releases
