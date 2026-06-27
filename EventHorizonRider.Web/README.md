# Event Horizon Rider — website

The static marketing site for the game, served at **https://eventhorizonrider.com**.

The site is plain static files: `index.html`, `privacy.html`, `privacypolicy.txt`, and the
`css/`, `fonts/`, `images/`, and `js/` folders. The `EventHorizonRider.Web.csproj` / `Program.cs`
are only a local convenience for previewing with `dotnet run`; **they are not deployed** and the
production site does not run any server code.

## Hosting (AWS)

```
eventhorizonrider.com / www.eventhorizonrider.com
        │  (DNS -> CloudFront)
        ▼
CloudFront distribution  E23FGJL5ZWVGNG     ← CDN + HTTPS (TLS cert from AWS ACM, us-east-1)
        │  (origin)
        ▼
S3 bucket  www.eventhorizonrider.com        ← static website hosting (index & error -> index.html)
        └─ logs/                            ← S3 access logs (kept private)
```

- **S3** bucket `www.eventhorizonrider.com` has *static website hosting* enabled (index document and
  error document both `index.html`). Public read is granted by a **bucket policy** (`scripts/s3-bucket-policy.json`)
  rather than per-object ACLs, so newly uploaded files are served without any extra ACL step. The policy
  also denies public reads of the `logs/` prefix.
- **CloudFront** distribution `E23FGJL5ZWVGNG` fronts the bucket for the CDN and HTTPS. Its aliases are
  `eventhorizonrider.com` and `www.eventhorizonrider.com`.
- **ACM** provides the TLS certificate for those domains (must live in **us-east-1** for CloudFront).

## Deploy

With the `aws` CLI authenticated (PowerShell 7+):

```powershell
pwsh ./scripts/Deploy-Website.ps1            # sync to S3 + invalidate CloudFront
pwsh ./scripts/Deploy-Website.ps1 -WhatIf    # dry run (no upload, no invalidation)
```

The script uploads the site files (excluding the `.NET` project scaffolding) and then creates a
CloudFront invalidation (`/*`) so the new content goes live within a few minutes. It does **not** use
`aws s3 sync --delete` — the bucket also holds the large `logs/` prefix, so a delete-sync would be slow
and risky.

### One-time bucket policy (already applied)

If the bucket is ever recreated, re-apply public read:

```powershell
aws s3api put-bucket-policy --bucket www.eventhorizonrider.com --policy file://scripts/s3-bucket-policy.json
```

## Privacy policy

`privacy.html` is published at **https://eventhorizonrider.com/privacy.html** and is the URL used for the
App Store Connect *Privacy Policy URL*. It states the game collects no data, so the *User Privacy Choices URL*
is not needed and the App Privacy questionnaire should be "Data Not Collected".
