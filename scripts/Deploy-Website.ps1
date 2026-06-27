#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy the static marketing site (EventHorizonRider.Web) to AWS and refresh the CDN.

.DESCRIPTION
    Hosting (see EventHorizonRider.Web/README.md):
      S3 bucket  www.eventhorizonrider.com  (static website hosting)
        -> CloudFront E23FGJL5ZWVGNG         (CDN + HTTPS via an ACM certificate)
        -> eventhorizonrider.com / www.eventhorizonrider.com

    Uploads the site files to S3 (skipping the .NET project scaffolding), then invalidates
    CloudFront so the new content goes live. Uses `aws s3 cp --recursive` rather than `sync`,
    because the bucket also holds a huge access-log prefix that `sync` would have to list every
    run. `cp` doesn't remove orphans; delete stale objects manually if ever needed.

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

# Use `cp --recursive`, not `sync`: the bucket also holds a huge access-log prefix, and `sync`
# would list all of it to build its comparison (very slow). `cp` just uploads the small site.
$cpArgs = @('s3', 'cp', $site, "s3://$Bucket", '--recursive') + $excludes
if ($WhatIf) { $cpArgs += '--dryrun' }

Write-Host "Uploading $site -> s3://$Bucket"
aws $cpArgs
if ($LASTEXITCODE -ne 0) { throw 's3 upload failed' }

if ($WhatIf) {
    Write-Host '(--WhatIf) Skipped CloudFront invalidation.'
    return
}

Write-Host "Invalidating CloudFront $DistributionId (/*)"
$status = aws cloudfront create-invalidation --distribution-id $DistributionId --paths '/*' --query 'Invalidation.Status' --output text
if ($LASTEXITCODE -ne 0) { throw 'CloudFront invalidation failed' }
Write-Host "Invalidation status: $status"

Write-Host 'Deployed. CloudFront invalidation usually completes within a few minutes.'
