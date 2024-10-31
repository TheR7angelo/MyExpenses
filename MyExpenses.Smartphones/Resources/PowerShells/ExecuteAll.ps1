# Get the current directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Path to the CopyDatabaseModel script
$copyDatabaseModelScript = Join-Path -Path $scriptDirectory -ChildPath "CopyDatabaseModel.ps1"

# Path to the CopyMapsMarker script
$copyMapsMarkerScript = Join-Path -Path $scriptDirectory -ChildPath "CopyMapsMarker.ps1"

# Function to execute a script and handle errors
function Execute-Script {
    param (
        [string]$scriptPath
    )

    if (Test-Path -Path $scriptPath) {
        Write-Output "Executing $scriptPath..."
        & $scriptPath
        if ($?) {
            Write-Output "$scriptPath executed successfully."
        } else {
            Write-Output "Error executing $scriptPath."
        }
    } else {
        Write-Output "Script $scriptPath not found."
    }
}

# Execute CopyDatabaseModel script
Execute-Script -scriptPath $copyDatabaseModelScript

# Execute CopyMapsMarker script
Execute-Script -scriptPath $copyMapsMarkerScript

# Output success message for the whole execution
Write-Output "All scripts executed successfully."