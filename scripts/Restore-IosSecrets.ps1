#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Restore iOS code-signing material from AWS Secrets Manager into ./signing, and
    (unless -SkipGitHubSecrets) set the GitHub Actions secrets the iOS TestFlight
    workflow needs. After a fresh `git clone`, this gets you back in business.

.NOTES
    Requires the authenticated `aws` and `gh` CLIs and PowerShell 7+.
#>
[CmdletBinding()]
param(
    [string]$Region = 'us-east-1',
    [string]$SecretName = 'eventhorizonrider/ios-signing',
    [string]$Repo = 'scottlerch/EventHorizonRider',
    [switch]$SkipGitHubSecrets
)

$ErrorActionPreference = 'Stop'
$signing = Join-Path (Split-Path $PSScriptRoot -Parent) 'signing'
New-Item -ItemType Directory -Force -Path $signing | Out-Null

$json = aws secretsmanager get-secret-value --secret-id $SecretName --region $Region --query SecretString --output text
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($json)) {
    throw "Could not read $SecretName from $Region. Is the aws CLI authenticated?"
}
$s = $json | ConvertFrom-Json

# Restore local files.
[IO.File]::WriteAllBytes((Join-Path $signing 'dist.p12'), [Convert]::FromBase64String($s.p12_base64))
[IO.File]::WriteAllBytes((Join-Path $signing 'AppStore.mobileprovision'), [Convert]::FromBase64String($s.provision_profile_base64))
[IO.File]::WriteAllText((Join-Path $signing 'p12-password.txt'), $s.p12_password)
[IO.File]::WriteAllText((Join-Path $signing "AuthKey_$($s.asc_key_id).p8"), $s.asc_api_key_p8)
[IO.File]::WriteAllText((Join-Path $signing 'asc-issuer-id.txt'), $s.asc_issuer_id)
Write-Host "Restored signing material to $signing"

if (-not $SkipGitHubSecrets) {
    # Set the GitHub Actions secrets the workflow reads. --body passes the exact value
    # (no trailing newline), which matters for the password / key id / issuer id.
    $map = [ordered]@{
        IOS_DIST_CERT_P12_BASE64     = $s.p12_base64
        IOS_DIST_CERT_PASSWORD       = $s.p12_password
        IOS_PROVISION_PROFILE_BASE64 = $s.provision_profile_base64
        ASC_KEY_ID                   = $s.asc_key_id
        ASC_ISSUER_ID                = $s.asc_issuer_id
        ASC_API_KEY_P8               = $s.asc_api_key_p8
    }
    foreach ($name in $map.Keys) {
        gh secret set $name --repo $Repo --body $map[$name]
    }
    Write-Host "Set GitHub Actions secrets on $Repo"
}
