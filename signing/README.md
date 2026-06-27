# iOS code-signing material (gitignored)

Everything in this folder **except this README is gitignored** — it holds the Apple
code-signing assets and must never be committed.

| File | What it is |
| --- | --- |
| `dist.p12` | Apple Distribution certificate + private key (password-protected) |
| `p12-password.txt` | the password for `dist.p12` |
| `AuthKey_<KeyID>.p8` | App Store Connect API key (used to upload to TestFlight) |
| `asc-issuer-id.txt` | App Store Connect API **Issuer ID** |
| `*.mobileprovision` | App Store provisioning profile |
| `dist.key` / `dist.csr` / `distribution.cer` / `dist.pem` | intermediate certificate material (kept for reference) |

## Restore on a fresh clone

The material is backed up in **AWS Secrets Manager** (`eventhorizonrider/ios-signing`, region `us-east-1`).
With the `aws` and `gh` CLIs authenticated:

```powershell
pwsh ./scripts/Restore-IosSecrets.ps1
```

That repopulates this folder **and** sets the GitHub Actions secrets the `iOS TestFlight`
workflow uses. Pass `-SkipGitHubSecrets` to restore only the local files.

## Back up after rotating assets

```powershell
pwsh ./scripts/Backup-IosSecrets.ps1
```

## Note

The distribution certificate and provisioning profile **expire after one year**. When they do,
regenerate them (Apple Developer portal or the App Store Connect API), drop the new files here,
and re-run the backup script.
