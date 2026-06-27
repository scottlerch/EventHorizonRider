#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Create (or recreate) an App Store provisioning profile via the App Store Connect API
    and save it to ./signing. Useful for the initial setup and for the yearly renewal when
    the profile expires.

.DESCRIPTION
    Reads the API key (signing/AuthKey_<KeyID>.p8), Issuer ID (signing/asc-issuer-id.txt),
    and the distribution certificate (signing/distribution.cer). Mints a short-lived ES256
    JWT, looks up the bundle id + matching certificate, creates an IOS_APP_STORE profile, and
    writes the profile bytes to signing/AppStore.mobileprovision.

.NOTES
    Requires PowerShell 7+. The API key must have the App Manager (or Admin) role.
#>
[CmdletBinding()]
param(
    [string]$BundleIdentifier = 'com.eventhorizonrider',
    [string]$ProfileName = "Event Horizon Rider App Store $((Get-Date).ToString('yyyyMMdd-HHmmss'))",
    [string]$OutFile
)

$ErrorActionPreference = 'Stop'
$signing = Join-Path (Split-Path $PSScriptRoot -Parent) 'signing'
if (-not $OutFile) { $OutFile = Join-Path $signing 'AppStore.mobileprovision' }

$p8   = Get-ChildItem -Path $signing -Filter 'AuthKey_*.p8' | Select-Object -First 1
if (-not $p8) { throw "No AuthKey_*.p8 found in $signing" }
$keyId  = ([IO.Path]::GetFileNameWithoutExtension($p8.Name)) -replace '^AuthKey_', ''
$issuer = (Get-Content -Raw (Join-Path $signing 'asc-issuer-id.txt')).Trim()
$cerB64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes((Join-Path $signing 'distribution.cer')))

# --- Mint an ES256 JWT for the App Store Connect API ---
function ConvertTo-Base64Url([byte[]]$bytes) {
    [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
}
$ec = [System.Security.Cryptography.ECDsa]::Create()
$ec.ImportFromPem((Get-Content -Raw $p8.FullName))
$now = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$header  = (@{ alg = 'ES256'; kid = $keyId; typ = 'JWT' } | ConvertTo-Json -Compress)
$payload = (@{ iss = $issuer; iat = $now; exp = $now + 600; aud = 'appstoreconnect-v1' } | ConvertTo-Json -Compress)
$signingInput = (ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($header))) + '.' +
                (ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($payload)))
# The 2-arg ECDsa.SignData returns the signature in IEEE P1363 (raw r||s) form, which is
# exactly what JWS/ES256 requires (no DER-to-JOSE conversion needed).
$sig = $ec.SignData([Text.Encoding]::UTF8.GetBytes($signingInput),
    [System.Security.Cryptography.HashAlgorithmName]::SHA256)
$jwt = $signingInput + '.' + (ConvertTo-Base64Url $sig)
$headers = @{ Authorization = "Bearer $jwt" }
$base = 'https://api.appstoreconnect.apple.com'

# --- Resolve the bundle id resource ---
$enc = [uri]::EscapeDataString($BundleIdentifier)
$bundles = Invoke-RestMethod -Headers $headers -Uri "$base/v1/bundleIds?filter[identifier]=$enc&limit=200"
$bundle = $bundles.data | Where-Object { $_.attributes.identifier -eq $BundleIdentifier } | Select-Object -First 1
if (-not $bundle) { throw "Bundle id '$BundleIdentifier' not found in your App Store Connect account." }

# --- Find the certificate resource matching our local distribution cert ---
$certs = Invoke-RestMethod -Headers $headers -Uri "$base/v1/certificates?limit=200"
$cert = $certs.data | Where-Object { $_.attributes.certificateContent -eq $cerB64 } | Select-Object -First 1
if (-not $cert) { throw "Could not find a certificate in App Store Connect matching signing/distribution.cer." }

# --- Create the App Store profile ---
$body = @{
    data = @{
        type          = 'profiles'
        attributes    = @{ name = $ProfileName; profileType = 'IOS_APP_STORE' }
        relationships = @{
            bundleId     = @{ data = @{ type = 'bundleIds'; id = $bundle.id } }
            certificates = @{ data = @(@{ type = 'certificates'; id = $cert.id }) }
        }
    }
} | ConvertTo-Json -Depth 10
$resp = Invoke-RestMethod -Headers $headers -Method Post -ContentType 'application/json' -Uri "$base/v1/profiles" -Body $body

[IO.File]::WriteAllBytes($OutFile, [Convert]::FromBase64String($resp.data.attributes.profileContent))
Write-Host "Created profile '$($resp.data.attributes.name)' (expires $($resp.data.attributes.expirationDate))"
Write-Host "Saved $OutFile"
