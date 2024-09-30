$sourceFile = "..\MyExpenses.Sql\Database Models\Model.sqlite"

$destinationFile = "Resources\Raw\Database Models\Model.sqlite"

$destinationDirectory = Split-Path -Path $destinationFile -Parent
if (-not (Test-Path -Path $destinationDirectory)) {
    New-Item -ItemType Directory -Path $destinationDirectory
}

Copy-Item -Path $sourceFile -Destination $destinationFile -Force