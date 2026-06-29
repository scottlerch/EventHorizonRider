# Store publishing

How Event Horizon Rider ships to each store. **iOS** and **Windows** are active; **Android** is shelved.

## iOS — App Store / TestFlight

Built and uploaded by [`ios-testflight.yml`](.github/workflows/ios-testflight.yml). Signing material
lives in AWS Secrets Manager (`eventhorizonrider/ios-signing`, us-east-1) — restore it with
[`scripts/Restore-IosSecrets.ps1`](scripts/Restore-IosSecrets.ps1). Inspect build/listing state with
[`scripts/Get-IosBuildState.ps1`](scripts/Get-IosBuildState.ps1). Store screenshots come from
[`ios-screenshots.yml`](.github/workflows/ios-screenshots.yml) (committed under [`screenshots/`](screenshots/)).

## Windows — Microsoft Store (MSIX)

The Windows head is a Win32 (WindowsDX) desktop app, shipped as an **MSIX bundle**.

**Build:** run the **Windows MSIX** workflow ([`windows-msix.yml`](.github/workflows/windows-msix.yml))
with a `version` input (`a.b.c.d`, higher than the current Store version). It:

1. publishes the app self-contained (`win-x64`, so users don't need .NET installed),
2. packs it into an `.msix` using the identity in [`EventHorizonRider.Windows/Package/AppxManifest.xml`](EventHorizonRider.Windows/Package/AppxManifest.xml),
3. wraps it in an `.msixbundle` — Partner Center requires a bundle because the app's history includes
   a Windows 8.1 / Phone 8.1 `.appxbundle`,
4. uploads the `windows-msixbundle` artifact.

**Package identity** (must match Partner Center exactly):

| Field | Value |
| --- | --- |
| Name | `11033ScottLerch.EventHorizonRider` |
| Publisher | `CN=427E0D6A-DF11-4676-8850-592258173525` |
| PublisherDisplayName | `Scott Lerch` |

Store logos live in [`EventHorizonRider.Windows/Package/Assets/`](EventHorizonRider.Windows/Package/Assets)
(generated from the 1024px app icon).

**Submit:** Partner Center → the product → new submission → **Packages** → upload the `.msixbundle`. It's
unsigned on purpose — the Store signs it; no code-signing certificate is needed for Store distribution.
Refresh age rating / screenshots / description as needed, then submit.

**Notes:**
- Only Windows 10/11 desktop is supported. Windows Phone 8.1 / Windows 8.1 are end-of-life and dropped;
  their old packages stay in Store history but no new ones are produced.
- Run the app locally with `dotnet run --project EventHorizonRider.Windows` (the unsigned bundle won't
  side-load without signing, but the executable runs).

## Android — Google Play (shelved)

The original Play listing's developer account was **closed for inactivity** (Mar 2024) and **can't be
reopened or appealed**. Republishing requires a **new** Play account, almost certainly a **new package
name** (`com.eventhorizonrider` is permanently reserved by Google), and — for personal accounts — a
**closed test with 12 testers for 14 consecutive days** before production access. Organization accounts
(with a D-U-N-S number) are exempt from that testing requirement.

The build still works — the **Android AAB** workflow ([`android-aab.yml`](.github/workflows/android-aab.yml))
produces a Google Play `.aab` on a Windows runner. To revive Android later:

1. Change `package=` in [`EventHorizonRider.Android/Properties/AndroidManifest.xml`](EventHorizonRider.Android/Properties/AndroidManifest.xml)
   to a fresh application id.
2. Add the signing secrets the workflow reads (`ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`,
   `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`) and it will sign the bundle.
3. Create the new Play account, satisfy the testing requirement, and upload with a `versionCode` higher
   than any previously published.
