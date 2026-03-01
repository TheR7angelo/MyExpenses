<#
  Winget workflow (interactive, full console) - NO --output:
  - Ask version
  - Checkbox selection (x64/arm64) with arrows + Space + Enter
  - wingetcreate update (default export behavior)
  - Ask manifests folder path (supports %USERPROFILE% etc.)
  - Ask "Test manifest?" -> if No: stop
  - winget validate
  - winget install --manifest .
  - Ask "Submit?" -> if Yes: wingetcreate submit

  Requirements:
  - winget.exe available
  - wingetcreate.exe available (in PATH or set full path below)

  Tip:
  - Save this file as UTF-8.
#>

# -----------------------------
# CONSTANTES (modifie ici)
# -----------------------------
$PackageId = "TheR7angelo.MyExpenses"

$RepoOwner = "TheR7angelo"
$RepoName  = "MyExpenses"
$TagPrefix = "v"

# Noms des installers dans GitHub Releases
$InstallerNameX64   = "MyExpensesUserSetup.x64.exe"
$InstallerNameArm64 = "MyExpensesUserSetup.arm64.exe"

# Commandes (mettre un chemin complet si besoin)
$WingetCreate = "wingetcreate.exe"
$Winget       = "winget"

# Options de test d'installation
$WingetInstallArgs = @(
    "--silent",
    "--accept-package-agreements",
    "--accept-source-agreements"
)

# -----------------------------
# UTILITAIRES
# -----------------------------
$ErrorActionPreference = "Stop"

function Clear-Screen {
    # Selon la console (Rider/Terminal), Clear-Host peut être ignoré.
    try {
        Clear-Host
        return
    } catch { }

    $esc = [char]27
    try {
        Write-Host "$esc[2J$esc[H" -NoNewline
        return
    } catch { }

    0..60 | ForEach-Object { Write-Host "" }
}

function Assert-Command([string]$name) {
    if ([string]::IsNullOrWhiteSpace($name)) {
        throw "Commande vide. Verifie les variables `$Winget et `$WingetCreate."
    }
    if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
        throw "Commande introuvable: '$name'. Mets-la dans le PATH ou utilise un chemin complet."
    }
}

function Ask-YesNo([string]$question, [bool]$defaultYes = $true) {
    $suffix = if ($defaultYes) { "[O/n]" } else { "[o/N]" }
    while ($true) {
        $a = Read-Host "$question $suffix"
        if ([string]::IsNullOrWhiteSpace($a)) { return $defaultYes }

        switch ($a.Trim().ToLowerInvariant()) {
            "o" { return $true }
            "oui" { return $true }
            "y" { return $true }
            "yes" { return $true }
            "n" { return $false }
            "non" { return $false }
            default { Write-Host "Reponds par o/oui ou n/non." -ForegroundColor Yellow }
        }
    }
}

function Ask-Version {
    while ($true) {
        $v = Read-Host "Version a publier (ex: 1.4.2)"
        if ($v -match '^\d+\.\d+\.\d+(\.\d+)?$') { return $v }
        Write-Host "Format invalide. Attendu: 1.2.3 (ou 1.2.3.4)." -ForegroundColor Yellow
    }
}

function Invoke-StepWithRetry([string]$title, [scriptblock]$action) {
    while ($true) {
        Write-Host ""
        Write-Host "==> $title" -ForegroundColor Cyan
        try {
            & $action
            return
        }
        catch {
            Write-Host ""
            Write-Host "ERREUR pendant: $title" -ForegroundColor Red
            Write-Host $_.Exception.Message -ForegroundColor Red

            if ($_.ErrorDetails -and $_.ErrorDetails.Message) {
                Write-Host $_.ErrorDetails.Message -ForegroundColor DarkRed
            }

            if (-not (Ask-YesNo "Voulez-vous retenter cette etape ?" $true)) {
                throw
            }
        }
    }
}

function Select-CheckboxesConsole {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string[]]$Options,

        [bool[]]$DefaultChecked
    )

    if (-not $DefaultChecked -or $DefaultChecked.Count -ne $Options.Count) {
        $DefaultChecked = @(for ($i = 0; $i -lt $Options.Count; $i++) { $true })
    }

    [bool[]]$checked = @($DefaultChecked)
    $index = 0

    $oldTreat = [Console]::TreatControlCAsInput
    [Console]::TreatControlCAsInput = $true

    try {
        while ($true) {
            Clear-Screen
            Write-Host $Title -ForegroundColor Cyan
            Write-Host "Fleches: bouger   Espace: cocher/decocher   Entree: OK   Echap: annuler" -ForegroundColor DarkGray
            Write-Host ""

            for ($i = 0; $i -lt $Options.Count; $i++) {
                $cursor = if ($i -eq $index) { ">" } else { " " }
                $box = if ($checked[$i]) { "[x]" } else { "[ ]" }
                Write-Host ("{0} {1} {2}" -f $cursor, $box, $Options[$i])
            }

            $keyInfo = [Console]::ReadKey($true)

            if (($keyInfo.Modifiers -band [ConsoleModifiers]::Control) -and ($keyInfo.Key -eq [ConsoleKey]::C)) {
                throw "Interrompu (Ctrl+C)."
            }

            switch ($keyInfo.Key) {
                ([ConsoleKey]::UpArrow)   { if ($index -gt 0) { $index-- } }
                ([ConsoleKey]::DownArrow) { if ($index -lt $Options.Count - 1) { $index++ } }
                ([ConsoleKey]::Spacebar)  { $checked[$index] = -not $checked[$index] }

                ([ConsoleKey]::Enter) {
                    $selected = @()
                    for ($i = 0; $i -lt $Options.Count; $i++) {
                        if ($checked[$i]) { $selected += $Options[$i] }
                    }

                    if ($selected.Count -eq 0) {
                        Write-Host ""
                        Write-Host "Coche au moins une option." -ForegroundColor Yellow
                        Start-Sleep -Milliseconds 900
                        continue
                    }

                    return $selected
                }

                ([ConsoleKey]::Escape) { throw "Selection annulee." }
            }
        }
    }
    finally {
        [Console]::TreatControlCAsInput = $oldTreat
    }
}

function Ask-Architectures {
    return Select-CheckboxesConsole `
    -Title "Selectionne les architectures a inclure :" `
    -Options @("x64", "arm64") `
    -DefaultChecked @($true, $true)
}

function Ask-ManifestsPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PackageId,

        [Parameter(Mandatory = $true)]
        [string]$Version
    )

    while ($true) {
        Write-Host ""
        Write-Host "wingetcreate a exporte les manifests dans un dossier." -ForegroundColor Cyan
        Write-Host "Colle ici le chemin du dossier VERSION (celui qui contient les .yaml)." -ForegroundColor Cyan
        Write-Host "Tu peux coller un chemin avec %USERPROFILE% (il sera resolu automatiquement)." -ForegroundColor DarkGray
        Write-Host "Exemple: %USERPROFILE%\Documents\...\$PackageId\$Version" -ForegroundColor DarkGray

        $p = Read-Host "Chemin du dossier manifests"
        $p = $p.Trim().Trim('"')

        if ([string]::IsNullOrWhiteSpace($p)) {
            Write-Host "Chemin vide." -ForegroundColor Yellow
            continue
        }

        # Expand %VAR% (ex: %USERPROFILE%)
        $p = [Environment]::ExpandEnvironmentVariables($p)

        # Support ~\...
        if ($p -like "~\*") {
            $p = Join-Path $HOME $p.Substring(2)
        }

        if (-not (Test-Path -LiteralPath $p -PathType Container)) {
            Write-Host "Dossier introuvable: $p" -ForegroundColor Yellow
            continue
        }

        $yaml = Get-ChildItem -LiteralPath $p -Filter *.yaml -File -ErrorAction SilentlyContinue
        if (-not $yaml -or $yaml.Count -lt 1) {
            Write-Host "Aucun fichier .yaml trouve dans: $p" -ForegroundColor Yellow
            continue
        }

        return $p
    }
}

# -----------------------------
# PROGRAMME PRINCIPAL
# -----------------------------
try {
    Assert-Command $Winget
    Assert-Command $WingetCreate

    $version = Ask-Version
    $tag = "$TagPrefix$version"
    $archs = Ask-Architectures

    $urls = New-Object System.Collections.Generic.List[string]
    if ($archs -contains "x64") {
        $urls.Add("https://github.com/$RepoOwner/$RepoName/releases/download/$tag/$InstallerNameX64")
    }
    if ($archs -contains "arm64") {
        $urls.Add("https://github.com/$RepoOwner/$RepoName/releases/download/$tag/$InstallerNameArm64")
    }

    Clear-Screen
    Write-Host "Resume :" -ForegroundColor Green
    Write-Host "  PackageId : $PackageId"
    Write-Host "  Version   : $version"
    Write-Host "  Tag       : $tag"
    Write-Host "  Archs     : $($archs -join ', ')"
    Write-Host "  URLs      :"
    $urls | ForEach-Object { Write-Host "    - $_" }

    if (-not (Ask-YesNo "Continuer (wingetcreate update) ?" $true)) {
        Write-Host "Stop." -ForegroundColor Yellow
        exit 0
    }

    Invoke-StepWithRetry "wingetcreate update (generation manifests)" {
        & $WingetCreate update $PackageId `
      --version $version `
      --urls @($urls) | Out-Host
    }

    $manifestsDir = Ask-ManifestsPath -PackageId $PackageId -Version $version

    if (-not (Ask-YesNo "Voulez-vous tester le manifeste ? (si Non, on arrete tout ici)" $true)) {
        Write-Host ""
        Write-Host "Arret demande." -ForegroundColor Yellow
        Write-Host "Manifests exportes ici:" -ForegroundColor Yellow
        Write-Host "  $manifestsDir" -ForegroundColor Yellow
        exit 0
    }

    Push-Location $manifestsDir
    try {
        Invoke-StepWithRetry "winget validate ." {
            & $Winget validate . | Out-Host
        }

        Invoke-StepWithRetry "winget install --manifest . (test install local)" {
            & $Winget install --manifest . @WingetInstallArgs | Out-Host
        }
    }
    finally {
        Pop-Location
    }

    Write-Host ""
    Write-Host "OK: validation + test termines." -ForegroundColor Green
    Write-Host "Manifests:" -ForegroundColor Green
    Write-Host "  $manifestsDir" -ForegroundColor Green

    if (Ask-YesNo "Voulez-vous SUBMIT (ouvrir la PR) maintenant ?" $false) {
        Invoke-StepWithRetry "wingetcreate submit" {
            & $WingetCreate submit $manifestsDir | Out-Host
        }
        Write-Host "Submit lance." -ForegroundColor Green
    }
    else {
        Write-Host ""
        Write-Host "Submit ignore. Commande manuelle :" -ForegroundColor Yellow
        Write-Host "  $WingetCreate submit `"$manifestsDir`"" -ForegroundColor Yellow
    }
}
catch {
    Write-Host ""
    Write-Host "Arret suite a erreur." -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}