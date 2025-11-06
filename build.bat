@echo off
cl /EHsc /Fe:BluePrintOne.exe main.cpp user32.lib gdi32.lib comdlg32.lib /link /SUBSYSTEM:WINDOWS
del *.obj
echo Build complete!
pause
