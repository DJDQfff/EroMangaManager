param(
    [string]$Version = (Get-Date -Format "yyyy.M.d"),
    [string]$PfxPassword
)

$outDir = ".\publish"
$tmpDir = "$env:TEMP\eromanga-build"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
Remove-Item -Recurse -Force $tmpDir -ErrorAction SilentlyContinue

# WinApp MSIX
$platforms = @("x64", "x86", "ARM64")
$rids = @{ "x64" = "win-x64"; "x86" = "win-x86"; "ARM64" = "win-arm64" }

foreach ($plat in $platforms) {
    Write-Output "MSIX $plat"
    $tmp = "$tmpDir\winapp\$plat"
    dotnet build WinApp\WinApp.csproj `
        -c Release `
        -p:Platform=$plat `
        -p:RuntimeIdentifier=$($rids[$plat]) `
        -p:AppxPackageDir="$tmp\" `
        /p:GenerateAppxPackageOnBuild=true `
        /p:PackageCertificateKeyFile=".\certificate.pfx" `
        /p:PackageCertificatePassword=$PfxPassword

    $msix = Get-ChildItem $tmp -Filter "*.msix" -Recurse | Where-Object { $_.Name -notmatch "^(Main|Singleton)" } | Select-Object -First 1
    if ($msix) {
        $newName = "EroMangaBrowser_${Version}_$plat.msix"
        Copy-Item $msix.FullName "$outDir\$newName"
        Write-Output "  ‚ú?$newName"
    }
}

# Android APK
$archs = @(
    @{ rid = "android-arm64"; suffix = "arm64-v8a" },
    @{ rid = "android-arm"; suffix = "armeabi-v7a" }
)

foreach ($arch in $archs) {
    Write-Output "APK $($arch.suffix)"
    $tmp = "$tmpDir\android\$($arch.suffix)"
    dotnet publish UnoApp\UnoApp\UnoApp.csproj `
        -f net10.0-android `
        -r $($arch.rid) `
        -c Release `
        -p:AndroidPackageFormats=apk `
        -p:AndroidSigningKeyStore=".\certificate.pfx" `
        -p:AndroidSigningKeyPass=$PfxPassword `
        -p:AndroidSigningStorePass=$PfxPassword `
        -o $tmp

    $apk = Get-ChildItem $tmp -Filter "*.apk" | Select-Object -First 1
    if ($apk) {
        $newName = "EroMangaBrowser_${Version}_$($arch.suffix).apk"
        Copy-Item $apk.FullName "$outDir\$newName"
        Write-Output "  ‚ú?$newName"
    }
}

Remove-Item -Recurse -Force $tmpDir -ErrorAction SilentlyContinue
Write-Output "ÂÆåÊàê„Ä?