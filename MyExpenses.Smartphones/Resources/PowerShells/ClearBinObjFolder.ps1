# Get the current directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

$projectRoot = Join-Path $scriptDirectory "..\.."  # Adjust based on your folder structure
Write-Host "Targeting folders in: $projectRoot" -ForegroundColor Yellow

# Remove 'bin' and 'obj' directories recursively
Get-ChildItem -Path $projectRoot -Recurse -Directory -Include "bin", "obj" | ForEach-Object {
    Write-Host "Deleting $($_.FullName)" -ForegroundColor Red
    Remove-Item -Recurse -Force -Path $_.FullName
}

# Confirmation message
Write-Host "All 'bin' and 'obj' folders have been successfully deleted!" -ForegroundColor Green
