#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Report App Store Connect build + TestFlight state for the app, to diagnose "why won't
    this build install in TestFlight": processing state, export compliance, expiry, the
    internal/external beta state, which beta groups a build is assigned to and their testers,
    and the app's available territories.

.DESCRIPTION
    Mints a short-lived ES256 JWT from an App Store Connect API key and queries the API.
    By default it reads the key material from ./signing (as restored by Restore-IosSecrets.ps1):
    the API key (signing/AuthKey_<KeyID>.p8) and Issuer ID (signing/asc-issuer-id.txt).
    Override any of those with -KeyPath / -KeyId / -IssuerId -- e.g. to use an Admin key that
    can read beta groups and testers (an App Manager key gets 403 on those).

.EXAMPLE
    pwsh ./scripts/Get-IosBuildState.ps1

.EXAMPLE
    pwsh ./scripts/Get-IosBuildState.ps1 -KeyPath ~/Downloads/AuthKey_ABC123.p8 -KeyId ABC123 -IssuerId 69a6de77-0289-47e3-e053-5b8c7c11a4d1

.NOTES
    Requires PowerShell 7+. Reading beta groups / testers needs an Admin (or App Manager) key.
#>
[CmdletBinding()]
param(
    [string]$BundleIdentifier = 'com.eventhorizonrider',
    [string]$KeyPath,
    [string]$KeyId,
    [string]$IssuerId,
    [int]$BuildLimit = 6
)

$ErrorActionPreference = 'Stop'
$signing = Join-Path (Split-Path $PSScriptRoot -Parent) 'signing'

# --- Resolve key material (params override the ./signing defaults) ---
if (-not $KeyPath) {
    $p8 = Get-ChildItem -Path $signing -Filter 'AuthKey_*.p8' -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $p8) { throw "No -KeyPath given and no signing/AuthKey_*.p8 found. Run Restore-IosSecrets.ps1 or pass -KeyPath." }
    $KeyPath = $p8.FullName
}
if (-not (Test-Path $KeyPath)) { throw "Key file not found: $KeyPath" }
if (-not $KeyId) { $KeyId = ([IO.Path]::GetFileNameWithoutExtension((Split-Path $KeyPath -Leaf))) -replace '^AuthKey_', '' }
if (-not $IssuerId) {
    $issFile = Join-Path $signing 'asc-issuer-id.txt'
    if (-not (Test-Path $issFile)) { throw "No -IssuerId given and signing/asc-issuer-id.txt not found." }
    $IssuerId = (Get-Content -Raw $issFile).Trim()
}

# --- Mint an ES256 JWT (same pattern as New-IosProvisioningProfile.ps1) ---
function ConvertTo-Base64Url([byte[]]$bytes) {
    [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
}
$ec = [System.Security.Cryptography.ECDsa]::Create()
$ec.ImportFromPem((Get-Content -Raw $KeyPath))
$now     = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$header  = (@{ alg = 'ES256'; kid = $KeyId; typ = 'JWT' } | ConvertTo-Json -Compress)
$payload = (@{ iss = $IssuerId; iat = $now; exp = $now + 600; aud = 'appstoreconnect-v1' } | ConvertTo-Json -Compress)
$signingInput = (ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($header))) + '.' +
                (ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($payload)))
$sig = $ec.SignData([Text.Encoding]::UTF8.GetBytes($signingInput),
    [System.Security.Cryptography.HashAlgorithmName]::SHA256)
$jwt = $signingInput + '.' + (ConvertTo-Base64Url $sig)
$headers = @{ Authorization = "Bearer $jwt" }
$base = 'https://api.appstoreconnect.apple.com'
function Invoke-Asc([string]$Path) { Invoke-RestMethod -Headers $headers -Uri ("$base$Path") }

# --- App ---
$enc  = [uri]::EscapeDataString($BundleIdentifier)
$apps = Invoke-Asc "/v1/apps?filter[bundleId]=$enc&limit=10"
$app  = $apps.data | Select-Object -First 1
if (-not $app) { throw "App $BundleIdentifier not found in this account." }
Write-Host "App: $($app.attributes.name)   (id $($app.id))   bundle $($app.attributes.bundleId)"

# --- App Store version states. Internal testing doesn't require a live version, but a
#     non-live app (no READY_FOR_SALE version) is useful context for "not available" errors. ---
try {
    $vers = Invoke-Asc "/v1/apps/$($app.id)/appStoreVersions?fields[appStoreVersions]=versionString,appStoreState,appVersionState&limit=5"
    Write-Host "App Store versions:"
    foreach ($v in $vers.data) {
        Write-Host ("  {0,-5} appStoreState={1} appVersionState={2}" -f `
            $v.attributes.versionString, $v.attributes.appStoreState, $v.attributes.appVersionState)
    }
} catch { Write-Host "App Store versions: (could not read) $($_.Exception.Message)" }

# --- Builds (newest first) ---
$buildsUri = "/v1/builds?filter[app]=$($app.id)&limit=$BuildLimit&sort=-uploadedDate" +
             "&fields[builds]=version,processingState,uploadedDate,expired,expirationDate,usesNonExemptEncryption,minOsVersion,buildAudienceType"
$builds = Invoke-Asc $buildsUri
Write-Host "`nRecent builds:"
foreach ($b in $builds.data) {
    $a = $b.attributes
    $comp = if ($null -eq $a.usesNonExemptEncryption) { 'NULL!' } else { "$($a.usesNonExemptEncryption)" }
    Write-Host ("  build {0,-4} proc={1,-10} minOS={2,-5} audience={3,-20} expired={4,-5} enc={5,-6} uploaded={6}" -f `
        $a.version, $a.processingState, $a.minOsVersion, $a.buildAudienceType, $a.expired, $comp, $a.uploadedDate)
}

# --- Deep dive on the newest non-expired build ---
$target = $builds.data | Where-Object { -not $_.attributes.expired } | Select-Object -First 1
if (-not $target) { $target = $builds.data | Select-Object -First 1 }
$ta = $target.attributes
Write-Host "`n=== Newest installable build: $($ta.version)  (id $($target.id)) ==="
Write-Host ("  processingState         : {0}" -f $ta.processingState)
Write-Host ("  usesNonExemptEncryption : {0}" -f $(if ($null -eq $ta.usesNonExemptEncryption) { 'NULL  <-- MISSING EXPORT COMPLIANCE' } else { $ta.usesNonExemptEncryption }))
Write-Host ("  expired                 : {0}" -f $ta.expired)
Write-Host ("  expirationDate          : {0}" -f $ta.expirationDate)
try {
    $bbd = Invoke-Asc "/v1/builds/$($target.id)/buildBetaDetail"
    Write-Host ("  internalBuildState      : {0}" -f $bbd.data.attributes.internalBuildState)
    Write-Host ("  externalBuildState      : {0}" -f $bbd.data.attributes.externalBuildState)
} catch { Write-Host "  buildBetaDetail         : (unavailable) $($_.Exception.Message)" }

# --- Beta groups for the app, their testers, and which builds each group can see.
#     (Queried by app filter: the /v1/builds/{id}/betaGroups relationship 403s even for Admin keys.) ---
try {
    $groups = Invoke-Asc "/v1/betaGroups?filter[app]=$($app.id)&fields[betaGroups]=name,isInternalGroup,hasAccessToAllBuilds&limit=50"
    if (-not $groups.data) { Write-Host "`n  betaGroups: NONE defined for this app" }
    foreach ($g in $groups.data) {
        Write-Host ("`n  Beta group '{0}'  (internal={1}, allBuilds={2})" -f `
            $g.attributes.name, $g.attributes.isInternalGroup, $g.attributes.hasAccessToAllBuilds)
        try {
            $testers = Invoke-Asc "/v1/betaGroups/$($g.id)/betaTesters?fields[betaTesters]=firstName,lastName,email,inviteType&limit=200"
            if (-not $testers.data) { Write-Host "    (no testers in this group)" }
            foreach ($t in $testers.data) {
                Write-Host ("    tester: {0} {1} <{2}>  invite={3}" -f `
                    $t.attributes.firstName, $t.attributes.lastName, $t.attributes.email, $t.attributes.inviteType)
            }
        } catch { Write-Host "    testers: (unavailable)" }
        try {
            $gb = Invoke-Asc "/v1/betaGroups/$($g.id)/builds?fields[builds]=version&limit=15"
            $vlist = ($gb.data | ForEach-Object { $_.attributes.version }) -join ', '
            $flag = if ($gb.data.attributes.version -contains $ta.version) { '' } else { "   <-- build $($ta.version) NOT in this group" }
            Write-Host ("    builds in group: {0}{1}" -f $vlist, $flag)
        } catch { Write-Host "    builds: (unavailable)" }
    }
} catch { Write-Host "  betaGroups: (unavailable) $($_.Exception.Message)" }
