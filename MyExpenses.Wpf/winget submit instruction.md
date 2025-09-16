# Step 1: Update or generate manifest files
# This updates or generates the required manifest files (version.yaml, installer.yaml, and defaultLocale.yaml).
# IMPORTANT: Make sure that --version represents the NEW version of the application (e.g., 1.2.0).

<NEW VERSION> = 1.2.0
<NEW VERSION TAG> = v1.2.0
wingetcreate.exe update TheR7angelo.MyExpenses --version <NEW VERSION> --urls https://github.com/TheR7angelo/MyExpenses/releases/download/<NEW VERSION TAG>/MyExepensesUserSetup.x64.exe https://github.com/TheR7angelo/MyExpenses/releases/download/<NEW VERSION TAG>/MyExepensesUserSetup.arm64.exe

# Step 2: Navigate to the export directory <PATH>
# Replace <PATH> with the actual directory where wingetcreate exported the manifest files.
cd <PATH>

# Step 3: Validate the manifest files
# Ensures that all the required manifest files in the current directory are correctly structured and valid.
winget validate .

# Step 4: Test the manifest locally
# Uses the validated manifest files to test the installation of the application locally.
winget install --manifest .

# Step 5: Submit the manifest
# Submits the manifest files to the Winget-Pkgs repository for public availability.
# Ensure <PATH> points to the directory where your manifest files are located.
wingetcreate.exe submit "<PATH>"