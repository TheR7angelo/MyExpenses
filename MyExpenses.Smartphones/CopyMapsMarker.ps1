# Source folder path
$sourcePath = "..\MyExpenses.Wpf\Resources\Maps"

# Destination folder path
$destinationPath = "Resources\Raw\Resources\Maps"

# Check if the destination folder exists, if not create it
if (-Not (Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

# Copy all .png files
Get-ChildItem -Path $sourcePath -Filter "*.png" | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destinationPath
}
