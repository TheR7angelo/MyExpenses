# Path to the source file
$sourceFile = "..\MyExpenses.Sql\Database Models\Model.sqlite"

# Path to the destination file
$destinationFile = "Resources\Raw\Database Models\Model.sqlite"

# Get the directory path of the destination file
$destinationDirectory = Split-Path -Path $destinationFile -Parent

# Check if the destination directory exists, if not, create it
if (-not (Test-Path -Path $destinationDirectory)) {
    New-Item -ItemType Directory -Path $destinationDirectory
}

# Copy the file from the source to the destination
Copy-Item -Path $sourceFile -Destination $destinationFile -Force