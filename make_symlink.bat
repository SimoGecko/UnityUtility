@echo off
set /p projname="Insert your project name:"

mklink /J "x:\Unity\%projname%\Assets\Scripts\Utility\" "x:\Unity\Utility\"