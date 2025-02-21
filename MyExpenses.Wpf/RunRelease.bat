@echo off
chcp 1252 >nul

set "scriptPath=%~dp0"
set "workingDirectory=%scriptPath%.."

echo Working directory for npx command: %workingDirectory%
cd /d "%workingDirectory%"

set "npxCommand=npx.cmd"
set "npxArguments=conventional-changelog -p angular -i CHANGELOG.md -s -sr 0"

echo Running 'npx conventional-changelog' command...
cmd /c "%npxCommand% %npxArguments%"
if errorlevel 1 (
    echo Error while updating the changelog.
    pause
    exit /b 1
)
echo Changelog updated successfully.

set "projectFile="
for %%f in ("%scriptPath%*.csproj") do (
    set "projectFile=%%f"
    goto :foundProject
)

echo Error: No project file (.csproj) found in the current directory.
pause
exit /b 1

:foundProject
echo Detected project file: %~nxi(projectFile)%

for /f "tokens=2 delims=<>" %%A in ('findstr /r "<TargetFramework>.*</TargetFramework>" "%projectFile%"') do (
    set "outputFramework=%%A"
    goto :foundFramework
)

echo Error: Could not find TargetFramework in the .csproj file.
pause
exit /b 1

:foundFramework
echo Extracted target framework: %outputFramework%

set "configuration=Release"
set "runtimeIdentifier=win-x64"
set "outputFolder=%scriptPath%bin\%configuration%\%outputFramework%\%runtimeIdentifier%"

if exist "%outputFolder%" (
    echo Removing existing output folder: %outputFolder%
    rmdir /s /q "%outputFolder%"
    echo Output folder removed successfully.
) else (
    echo No existing output folder found.
)

echo Starting self-contained publish process...

dotnet publish "%projectFile%" -c %configuration% -r %runtimeIdentifier% --self-contained true
if errorlevel 1 (
    echo Error while publishing.
    pause
    exit /b 1
)
echo Self-contained publish completed successfully in: %outputFolder%

set "outputFolderPublish=%outputFolder%\publish"

set /p response=Do you want to open the publish folder? (Y/N)
if /i "%response%"=="Y" (
    echo Opening publish folder: %outputFolderPublish%
    start "" "%outputFolderPublish%"
) else (
    echo You chose not to open the publish folder. Exiting script.
)

pause