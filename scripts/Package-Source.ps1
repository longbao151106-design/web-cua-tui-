param([string]$OutputFile)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$solutionRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputFile)) { $OutputFile = Join-Path $solutionRoot 'artifacts\TraSuaMoc-source.zip' }
$archivePath = [IO.Path]::GetFullPath($OutputFile)
if (Test-Path -LiteralPath $archivePath) { throw 'File ZIP da ton tai. Chon ten moi de khong ghi de.' }
New-Item -ItemType Directory -Path (Split-Path -Parent $archivePath) -Force | Out-Null
$archive = [IO.Compression.ZipFile]::Open($archivePath, [IO.Compression.ZipArchiveMode]::Create)
try {
    $files = @()
    foreach ($file in @('TraSuaMoc.sln','README.md','.gitignore')) { $files += Get-Item -LiteralPath (Join-Path $solutionRoot $file) }
    foreach ($folder in @('TraSuaMoc','scripts','docs','tests')) {
        $files += Get-ChildItem -LiteralPath (Join-Path $solutionRoot $folder) -File -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.vs|__pycache__)\\' -and $_.Extension -notin @('.user','.suo','.log','.pyc') }
    }
    foreach ($file in $files) {
        $relative = $file.FullName.Substring($solutionRoot.Length + 1).Replace('\','/')
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, $relative, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
} finally { $archive.Dispose() }
Write-Output "Source ZIP: $archivePath"
