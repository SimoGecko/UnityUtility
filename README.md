# Utility for Unity
A set of common useful functionality shared among unity projects.

Most of the useful functionality (extension and static methods) is inside `Utility.cs`

### Instructions
This folder is usually added as a git submodule under `Assets/Scripts/Utility`.

You can run this command from the Unity project root directory:
`git submodule add https://github.com/SimoGecko/UnityUtility.git Assets\Scripts\Utility\`

Or using this batch file to set it up easily for any Unity Project:
```
@echo off
set /p projname="Insert your project name: "
cd "x:\Unity\%projname%
git submodule add https://github.com/SimoGecko/UnityUtility.git Assets\Scripts\Utility\
pause
```