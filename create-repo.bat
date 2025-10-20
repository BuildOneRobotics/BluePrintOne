@echo off
echo Pushing to GitHub...
git push -u origin main
echo.
echo Creating first release v1.0.0...
git tag v1.0.0
git push origin v1.0.0
echo.
echo Done! Check releases at:
echo https://github.com/BuildOneRobotics/BluePrintOne/releases
pause
