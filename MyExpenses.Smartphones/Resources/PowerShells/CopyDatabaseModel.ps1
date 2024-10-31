# Get the current directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Path to the source file
$sourceFile = Join-Path -Path $scriptDirectory -ChildPath "..\..\..\MyExpenses.Sql\Database Models\Model.sqlite"

# Path to the destination file
$destinationFile = Join-Path -Path $scriptDirectory -ChildPath "..\Raw\Database Models\Model.sqlite"

# Get the directory path of the destination file
$destinationDirectory = Split-Path -Path $destinationFile -Parent

# Check if the destination directory exists, if not, create it
if (-not (Test-Path -Path $destinationDirectory)) {
    New-Item -ItemType Directory -Path $destinationDirectory
}

# Copy the file from the source to the destination
Copy-Item -Path $sourceFile -Destination $destinationFile -Force

# Output success message
Write-Output "File copy completed successfully."