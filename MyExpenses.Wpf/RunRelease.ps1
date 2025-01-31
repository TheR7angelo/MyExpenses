# Set encoding to CP1252
[Console]::OutputEncoding = [System.Text.Encoding]::GetEncoding("windows-1252")

# Define the working directory (one level above the script directory)
$workingDirectory = Split-Path -Path $PSScriptRoot -Parent

Write-Host "Working directory for npx command: $workingDirectory" -ForegroundColor Cyan

# Explicit npx command path
$npxCommand = "npx.cmd"
$npxArguments = "conventional-changelog -p angular -i CHANGELOG.md -s -sr 0"

# Step 1: Run the npx command in the correct working directory
Write-Host "Running 'npx conventional-changelog' command..." -ForegroundColor Cyan

# Execute the npx command in the parent directory
try {
    Invoke-Expression "cd $workingDirectory; & '$npxCommand' $npxArguments"
    Write-Host "Changelog updated successfully." -ForegroundColor Green
} catch {
    Write-Host "Error while updating the changelog: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press any key to exit"
    exit 1
}

# Step 2: Detect the project file (.csproj)
$projectFile = Get-ChildItem -Path $PSScriptRoot -Filter *.csproj | Select-Object -First 1

if (-not $projectFile) {
    Write-Host "Error: No project file (.csproj) found in the current directory." -ForegroundColor Red
    Read-Host "Press any key to exit"
    exit 1
}

Write-Host "Detected project file: $($projectFile.Name)" -ForegroundColor Green

# Step 3: Extract the target framework from the .csproj file
try {
    $frameworkLine = Select-String -Path $projectFile.FullName -Pattern "<TargetFramework>(.*?)</TargetFramework>" | Select-Object -First 1
    if ($frameworkLine -and $frameworkLine.Matches.Groups.Count -ge 2) {
        $outputFramework = $frameworkLine.Matches.Groups[1].Value
        Write-Host "Extracted target framework: $outputFramework" -ForegroundColor Green
    } else {
        Write-Host "Error: Could not find TargetFramework in the .csproj file" -ForegroundColor Red
        Read-Host "Press any key to exit"
        exit 1
    }
} catch {
    Write-Host "Error while extracting TargetFramework: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press any key to exit"
    exit 1
}

# Step 4: Remove the existing output folder
$configuration = "Release"
$runtimeIdentifier = "win-x64"  # Example: win-x64, linux-x64, osx-x64
$outputFolder = Join-Path $PSScriptRoot "bin\$configuration\$outputFramework\$runtimeIdentifier"

if (Test-Path -Path $outputFolder) {
    Write-Host "Removing existing output folder: $outputFolder" -ForegroundColor Yellow
    Remove-Item -Recurse -Force -Path $outputFolder
    Write-Host "Output folder removed successfully." -ForegroundColor Green
} else {
    Write-Host "No existing output folder found." -ForegroundColor Cyan
}

# Step 5: Execute the publish process
Write-Host "Starting self-contained publish process..." -ForegroundColor Cyan

try {
    dotnet publish $projectFile.FullName -c $configuration -r $runtimeIdentifier --self-contained true
    Write-Host "Self-contained publish completed successfully in: $outputFolder" -ForegroundColor Green
} catch {
    Write-Host "Error while publishing: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press any key to exit"
    exit 1
}

$outputFolderPublish = Join-Path $PSScriptRoot "bin\$configuration\$outputFramework\$runtimeIdentifier\publish"

# Step 6: Ask if the user wants to open the publish folder
$response = Read-Host "Do you want to open the publish folder? (Y/N)"
if ($response -eq "Y" -or $response -eq "y") {
    try {
        Write-Host "Opening publish folder: $outputFolderPublish" -ForegroundColor Cyan
        Start-Process $outputFolderPublish
    } catch {
        Write-Host "Error while opening the publish folder: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "You chose not to open the publish folder. Exiting script." -ForegroundColor Yellow
}

# Wait for user input before exiting
Read-Host "Press any key to exit"