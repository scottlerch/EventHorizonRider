#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Back up the local iOS code-signing material in ./signing to AWS Secrets Manager
    as a single JSON secret. Run after first setting up the signing assets, or after
    rotating them (new certificate / provisioning profile / API key).

.NOTES
    Requires the authenticated `aws` CLI and PowerShell 7+.
    Never prints secret values; the JSON is passed to AWS via a temp file, not the command line.
#>
[CmdletBinding()]
param(
    [string]$Region = 'us-east-1',
    [string]$SecretName = 'eventhorizonrider/ios-signing'
)

$ErrorActionPreference = 'Stop'
$signing = Join-Path (Split-Path $PSScriptRoot -Parent) 'signing'

$p12        = Join-Path $signing 'dist.p12'
$passFile   = Join-Path $signing 'p12-password.txt'
$issuerFile = Join-Path $signing 'asc-issuer-id.txt'
$p8         = Get-ChildItem -Path $signing -Filter 'AuthKey_*.p8'   -ErrorAction SilentlyContinue | Select-Object -First 1
$prof       = Get-ChildItem -Path $signing -Filter '*.mobileprovision' -ErrorAction SilentlyContinue | Select-Object -First 1

foreach ($f in @($p12, $passFile, $issuerFile, $p8, $prof)) {
    if (-not $f -or -not (Test-Path $f)) { throw "Missing required signing file: $f" }
}
$keyId = ([IO.Path]::GetFileNameWithoutExtension($p8.Name)) -replace '^AuthKey_', ''

$payload = [ordered]@{
    p12_base64               = [Convert]::ToBase64String([IO.File]::ReadAllBytes($p12))
    p12_password             = (Get-Content -Raw $passFile)
    provision_profile_base64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($prof.FullName))
    asc_key_id               = $keyId
    asc_issuer_id            = (Get-Content -Raw $issuerFile).Trim()
    asc_api_key_p8           = (Get-Content -Raw $p8.FullName)
}
$json = $payload | ConvertTo-Json -Compress

$tmp = New-TemporaryFile
try {
    [IO.File]::WriteAllText($tmp.FullName, $json)
    $uri = 'file://' + ($tmp.FullName -replace '\\', '/')

    aws secretsmanager describe-secret --secret-id $SecretName --region $Region 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) {
        aws secretsmanager put-secret-value --secret-id $SecretName --region $Region --secret-string $uri | Out-Null
        Write-Host "Updated $SecretName in $Region."
    }
    else {
        aws secretsmanager create-secret --name $SecretName --region $Region `
            --description 'Event Horizon Rider iOS code-signing material' --secret-string $uri | Out-Null
        Write-Host "Created $SecretName in $Region."
    }
}
finally {
    Remove-Item $tmp.FullName -Force -ErrorAction SilentlyContinue
}
