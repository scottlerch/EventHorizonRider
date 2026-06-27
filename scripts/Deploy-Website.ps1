#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy the static marketing site (EventHorizonRider.Web) to AWS and refresh the CDN.

.DESCRIPTION
    Hosting (see EventHorizonRider.Web/README.md):
      S3 bucket  www.eventhorizonrider.com  (static website hosting)
        -> CloudFront E23FGJL5ZWVGNG         (CDN + HTTPS via an ACM certificate)
        -> eventhorizonrider.com / www.eventhorizonrider.com

    Syncs the site files to S3 (skipping the .NET project scaffolding), then invalidates
    CloudFront so the new content goes live. Does not use --delete: the bucket also holds a
    large access-log prefix, so a delete-sync would be slow and risky. Remove stale objects
    manually if ever needed (and exclude logs/).

.PARAMETER WhatIf
    Show what would change without uploading or invalidating (aws s3 sync --dryrun).

.NOTES
    Requires the authenticated `aws` CLI and PowerShell 7+.
#>
[CmdletBinding()]
param(
    [string]$Bucket = 'www.eventhorizonrider.com',
    [string]$DistributionId = 'E23FGJL5ZWVGNG',
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'
# aws writes progress to stderr and can exit non-zero on benign conditions; don't let PowerShell
# auto-throw on that - we check $LASTEXITCODE explicitly after each call.
$PSNativeCommandUseErrorActionPreference = $false
$site = Join-Path (Split-Path $PSScriptRoot -Parent) 'EventHorizonRider.Web'

# Exclude S3 access logs and the .NET project scaffolding (not part of the published site).
$excludes = @(
    '--exclude', 'logs/*'
    '--exclude', '*.md'
    '--exclude', '*.csproj'
    '--exclude', '*.csproj.user'
    '--exclude', 'Program.cs'
    '--exclude', 'Properties/*'
    '--exclude', 'bin/*'
    '--exclude', 'obj/*'
)

$syncArgs = @('s3', 'sync', $site, "s3://$Bucket") + $excludes
if ($WhatIf) { $syncArgs += '--dryrun' }

Write-Host "Syncing $site -> s3://$Bucket"
aws $syncArgs
if ($LASTEXITCODE -ne 0) { throw 's3 sync failed' }

if ($WhatIf) {
    Write-Host '(--WhatIf) Skipped CloudFront invalidation.'
    return
}

Write-Host "Invalidating CloudFront $DistributionId (/*)"
$status = aws cloudfront create-invalidation --distribution-id $DistributionId --paths '/*' --query 'Invalidation.Status' --output text
if ($LASTEXITCODE -ne 0) { throw 'CloudFront invalidation failed' }
Write-Host "Invalidation status: $status"

Write-Host 'Deployed. CloudFront invalidation usually completes within a few minutes.'
