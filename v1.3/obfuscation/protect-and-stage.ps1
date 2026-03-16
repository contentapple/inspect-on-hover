param(
    [Parameter(Mandatory = $true)]
    [string]$ConfuserCli,

    [Parameter(Mandatory = $true)]
    [ValidateSet("sticky", "nonsticky")]
    [string]$Variant,

    [Parameter(Mandatory = $true)]
    [string]$InputDll,

    [Parameter(Mandatory = $false)]
    [string]$ManagedDir = "C:\Program Files (x86)\Steam\steamapps\common\People Playground\People Playground_Data\Managed"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$work = Join-Path $PSScriptRoot "work"
if (Test-Path $work) {
    Remove-Item $work -Recurse -Force
}
New-Item -ItemType Directory -Path $work | Out-Null

switch ($Variant) {
    "sticky" {
        $projectName = "InspectOnHover.Sticky.crproj"
        $moduleName = "InspectOnHover.Sticky.dll"
        $outDirName = "out-sticky"
        $targetModFolder = Join-Path $root "DLL Test Sticky"
        $targetOutputName = "InspectOnHover.Sticky.Protected.dll"
    }
    "nonsticky" {
        $projectName = "InspectOnHover.NonSticky.crproj"
        $moduleName = "InspectOnHover.NonSticky.dll"
        $outDirName = "out-nonsticky"
        $targetModFolder = Join-Path $root "DLL Test Non Sticky"
        $targetOutputName = "InspectOnHover.NonSticky.Protected.dll"
    }
}

Copy-Item -Path (Join-Path $PSScriptRoot $projectName) -Destination (Join-Path $work $projectName)
Copy-Item -Path $InputDll -Destination (Join-Path $work $moduleName)

if (Test-Path $ManagedDir) {
    $dependencies = @((Join-Path $ManagedDir "Assembly-CSharp.dll")) + (Get-ChildItem -Path $ManagedDir -Filter "UnityEngine*.dll" | Select-Object -ExpandProperty FullName)
    foreach ($dep in $dependencies) {
        if (Test-Path $dep) {
            Copy-Item -Path $dep -Destination (Join-Path $work ([System.IO.Path]::GetFileName($dep))) -Force
        }
    }
}

Push-Location $work
try {
    & $ConfuserCli (Join-Path $work $projectName)
    $protectedDll = Join-Path $work "$outDirName\$moduleName"
    if ($LASTEXITCODE -ne 0 -and -not (Test-Path $protectedDll)) {
        throw "ConfuserEx failed with exit code $LASTEXITCODE"
    }

    if ($LASTEXITCODE -ne 0 -and (Test-Path $protectedDll)) {
        Write-Warning "ConfuserEx returned exit code $LASTEXITCODE but produced output. Continuing."
    }

    if (-not (Test-Path $protectedDll)) {
        throw "Protected DLL not found at $protectedDll"
    }

    Copy-Item -Path $protectedDll -Destination (Join-Path $targetModFolder $targetOutputName) -Force
}
finally {
    Pop-Location
}

Write-Host "Protected build staged to $targetModFolder\$targetOutputName"
