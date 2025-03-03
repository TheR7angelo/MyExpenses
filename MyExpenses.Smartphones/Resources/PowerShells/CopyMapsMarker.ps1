# Get the current directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Source folder path
$sourcePath = Join-Path -Path $scriptDirectory -ChildPath "..\..\..\MyExpenses.Commons\Resources\Assets\Maps"

# Destination folder path
$destinationPath = Join-Path -Path $scriptDirectory -ChildPath "..\Raw\Resources\Assets\Maps"

# Check if the destination folder exists, if not create it
if (-Not (Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

# Copy all .svg files
Get-ChildItem -Path $sourcePath -Filter "*.svg" | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destinationPath -Force
}

# Output success message
Write-Output "Copy operation completed successfully."

