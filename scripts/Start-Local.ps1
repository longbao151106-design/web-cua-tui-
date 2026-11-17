param([int]$Port = 5186)
$ErrorActionPreference = 'Stop'
$solutionRoot = Split-Path -Parent $PSScriptRoot
$webPath = Join-Path $solutionRoot 'TraSuaMoc'
$iisPath = Join-Path $env:ProgramFiles 'IIS Express\iisexpress.exe'
if (-not (Test-Path -LiteralPath $iisPath)) { throw 'Can cai IIS Express (workload ASP.NET and web development trong Visual Studio).' }
if (-not (Test-Path -LiteralPath (Join-Path $webPath 'bin\TraSuaMoc.dll'))) {
    Push-Location $solutionRoot
    try {
        & dotnet restore TraSuaMoc.sln
        if ($LASTEXITCODE -ne 0) { throw 'NuGet restore failed.' }
        & dotnet build TraSuaMoc.sln --no-restore
        if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    } finally { Pop-Location }
}
$iisProcess = Start-Process -FilePath $iisPath -ArgumentList @("/path:`"$webPath`"", "/port:$Port", '/systray:false') -WindowStyle Hidden -PassThru
Write-Output "Local URL: http://localhost:$Port/"
Write-Output "IIS Express PID: $($iisProcess.Id)"
