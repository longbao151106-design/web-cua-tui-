param([string]$Destination)
$ErrorActionPreference = 'Stop'
$solutionRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($Destination)) { $Destination = Join-Path $solutionRoot 'artifacts\publish' }
$publishPath = [IO.Path]::GetFullPath($Destination)
if (Test-Path -LiteralPath $publishPath) { throw 'Thu muc dich da ton tai. Chon thu muc moi de khong ghi de du lieu.' }
Push-Location $solutionRoot
try {
    & dotnet restore TraSuaMoc.sln
    if ($LASTEXITCODE -ne 0) { throw 'NuGet restore failed.' }
    & dotnet build TraSuaMoc.sln --no-restore -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    New-Item -ItemType Directory -Path $publishPath | Out-Null
    $webPath = Join-Path $solutionRoot 'TraSuaMoc'
    foreach ($folder in @('bin','Content','Scripts','Views')) {
        Copy-Item -LiteralPath (Join-Path $webPath $folder) -Destination $publishPath -Recurse
    }
    foreach ($file in @('Web.config','Global.asax')) { Copy-Item -LiteralPath (Join-Path $webPath $file) -Destination $publishPath }
    Write-Output "Publish files: $publishPath"
    Write-Output 'Chua upload. Cau hinh SQL Server va tai khoan host truoc khi trien khai.'
} finally { Pop-Location }
