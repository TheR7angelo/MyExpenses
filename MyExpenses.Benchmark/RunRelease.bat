@echo off
REM Navigate to the directory where the project or solution file is located
cd /d "%~dp0"

REM Run dotnet run in Release mode
dotnet run -c Release

REM Keep the window open to check the results (optional)
pause