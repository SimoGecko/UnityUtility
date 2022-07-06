@echo off

set /p folder="Enter folder: "

mklink /J "x:\Unity\%folder%\Assets\Scripts\Utility\" "x:\Unity\Utility\"

pause