# Packaging & distribution

This document covers how `release.yml` builds the maintained macOS packages,
which optional secrets unlock Apple Developer ID signing / notarization, and how to submit the
package-manager templates under `packaging/` once signed builds exist. The default release path
does not require paid credentials: macOS artifacts are ad-hoc signed.

Everything described here runs automatically on every release (a `v<UIVersion>` tag
push, or a manual `workflow_dispatch`). There are no manual signing steps — the workflow uses the
best available tier and labels the artifact accordingly.

## Artifact matrix

| Platform | Artifact(s) | Notes |
|---|---|---|
| macOS arm64 / x64 | `PKHeX-Avalonia-osx-{arm64,x64}-adhoc.zip` + `PKHeX-Avalonia-osx-{arm64,x64}-adhoc.dmg` by default; suffix changes with the signing tier | `.dmg` contains the `.app` bundle plus an `Applications` symlink |

Developer ID artifacts have no suffix; stable self-signed artifacts use `-selfsigned`. The
ad-hoc tier is the default and requires no Apple Developer membership.

## Release trigger

The release workflow is tag-driven. After merging a release-ready change, verify that
`Directory.Build.props` contains the intended `<UIVersion>`, then run:

```bash
git tag -a v<UIVersion> -m "PKHeX-Avalonia <UIVersion>"
git push origin v<UIVersion>
```

The workflow validates the tag against `<UIVersion>`, builds the two macOS architecture legs,
and creates the GitHub Release. A manual Actions run is available when the tag must be created by
the workflow; for a failed tag-triggered run, re-run that workflow from the Actions history.

## macOS: signing & notarization

`release.yml`'s `build` job (macOS legs of the matrix) gates real Developer
ID signing on the following secrets:

| Secret | Contents |
|---|---|
| `MACOS_CERT_P12` | Base64-encoded `.p12` export of a **Developer ID Application** certificate (`base64 -i cert.p12 \| pbcopy`) |
| `MACOS_CERT_PASSWORD` | Password used when exporting the `.p12` |
| `MACOS_SIGN_IDENTITY` | The identity string codesign should use, e.g. `Developer ID Application: Your Name (TEAMID)` |
| `APPLE_NOTARY_KEY_ID` | Key ID of an App Store Connect API key with the Developer role |
| `APPLE_NOTARY_KEY` | Base64-encoded `.p8` private key for that API key |
| `APPLE_NOTARY_ISSUER_ID` | Issuer ID (UUID) for the API key, from App Store Connect > Users and Access > Keys |

If all six Developer ID/notarization secrets are present, the workflow:

1. Imports the certificate into a temporary keychain.
2. Re-signs the `.app` with `codesign --options runtime` (hardened runtime,
   required for notarization) using the Developer ID identity.
3. Submits it to `notarytool`, waits for the result, and staples the ticket
   with `stapler staple`.
4. Packs the notarized `.app` into `PKHeX-Avalonia-osx-<arch>.dmg`.

If those secrets are absent and the optional self-signed secrets are also absent, the workflow
uses the no-cost ad-hoc tier and creates `PKHeX-Avalonia-osx-<arch>-adhoc.zip` plus
`PKHeX-Avalonia-osx-<arch>-adhoc.dmg`.
Ad-hoc signing provides code-integrity checks but is not Apple-trusted or notarized, so the first
launch may require right-click → **Open** or clearing the quarantine attribute:

```bash
xattr -dr com.apple.quarantine /Applications/PKHeX-Avalonia.app
```

## macOS: stable self-signed identity (the "tertius" pattern)

Real Developer ID signing needs a paid Apple Developer account. Without one,
`release.yml` can optionally use a second tier before the default ad-hoc path:
a **stable self-signed identity**.

**Why a stable identity matters.** Gatekeeper's "this app is from an
unidentified developer" prompt, and macOS's re-prompting for TCC permissions
(Accessibility, Full Disk Access, etc.) and keychain access, are keyed off
the app's *codesign designated requirement* — effectively a hash derived
from the signing certificate. An ad-hoc signature (`codesign --sign -`) has
no stable identity: every build gets a new one, so macOS treats every
update as a brand-new, never-before-seen app. That means every single
release re-triggers Gatekeeper's "are you sure?" dialog and drops any TCC
grants the user already approved.

If instead every release is signed with the **same** self-signed
certificate, the designated requirement stays identical release over
release. macOS then recognizes an update as *the same app* upgrading in
place — TCC grants and keychain items persist across `brew upgrade` or a
manual re-download and reinstall, and Gatekeeper only has an opinion about
the app once, not on every update.

This does **not** replace notarization — a self-signed cert isn't trusted by
Apple, so first launch of a freshly downloaded artifact still shows the
Gatekeeper "unidentified developer" prompt once. What it fixes is the
*repeat* prompting on every subsequent update, and it works entirely without
an Apple Developer Program membership.

**Generating the certificate (one-time, by the repo owner):**

```bash
P12_PASSWORD='choose-a-password' Scripts/make-signing-cert.sh ./secrets
base64 -i ./secrets/signing.p12 | pbcopy   # paste into SELFSIGN_CERT_P12_BASE64
```

This produces a 10-year self-signed EC certificate with the `codeSigning`
extended key usage, packaged as `signing.p12`. Keep this file (and the
password) somewhere durable outside the repo — regenerating it produces a
*different* identity, which resets the stability this whole pattern exists
for.

**The three secrets to add** (repo Settings → Secrets and variables →
Actions), used only when the Developer ID secrets above are absent:

| Secret | Contents |
|---|---|
| `SELFSIGN_CERT_P12_BASE64` | Base64 of the `.p12` produced above |
| `SELFSIGN_CERT_PASSWORD` | The `P12_PASSWORD` used to generate it |
| `SELFSIGN_IDENTITY` | The certificate's CN, default `Patrik Lleshaj` (override via `CERT_CN` when running the script) |

When present, `release.yml` imports the cert into a dedicated CI keychain
(`Scripts/import-cert.sh`, same idempotent import-and-unlock-partition-list
flow used for local testing) and re-signs every Mach-O in
`PKHeX.Avalonia.app` with that identity before building the `.dmg`. The
artifacts are named `PKHeX-Avalonia-osx-<arch>-selfsigned.zip` and
`PKHeX-Avalonia-osx-<arch>-selfsigned.dmg` so they are distinguishable from a
Developer ID build and from the default ad-hoc tier.

**Homebrew users get the whole problem solved for them.** The cask template
(`packaging/homebrew/pkhex-avalonia.rb`) runs a `postflight` block that
strips the quarantine extended attribute Homebrew's downloader adds
(`xattr -dr com.apple.quarantine`), so `brew install --cask pkhex-avalonia`
never shows a Gatekeeper prompt at all, on first install or any later
upgrade.

**Manual `.dmg`/`.zip` downloads** still see the one-time "unidentified
developer" prompt on first launch (self-signed, not notarized). Either
right-click → **Open** once, or clear the quarantine bit yourself:

```bash
xattr -dr com.apple.quarantine /Applications/PKHeX-Avalonia.app
```

After that one time, updating in place (replacing the same `/Applications`
copy) keeps the same designated requirement release to release, so this
manual step should not be needed again as long as the self-signed cert
itself isn't regenerated.

## Windows: installer & code signing

`release.yml`'s Windows leg installs Inno Setup via `choco install
innosetup`, then builds `packaging/windows/installer.iss` into
`PKHeX-Avalonia-Setup.exe` (registers Start Menu / desktop shortcuts and an
Add/Remove Programs entry that uninstalls cleanly).

Code signing (via `signtool.exe`, part of the Windows SDK already present on
`windows-latest`) is gated on:

| Secret | Contents |
|---|---|
| `WINDOWS_CERT_P12` | Base64-encoded `.pfx`/`.p12` of an OV (or EV) code-signing certificate |
| `WINDOWS_CERT_PASSWORD` | Password for that `.pfx` |

If both secrets are present, the workflow signs both the published
`PKHeX.Avalonia.exe` (before packaging) and the final
`PKHeX-Avalonia-Setup.exe` with a SHA-256 signature and a DigiCert RFC 3161
timestamp. If they're absent, the installer is renamed to
`PKHeX-Avalonia-Setup-unsigned.exe` so SmartScreen's "unknown publisher"
warning is expected and self-explanatory from the filename.

**Note:** an OV certificate alone does not eliminate SmartScreen warnings
immediately — Microsoft's reputation system needs download volume to build
trust for a given cert. An EV certificate (or Azure Trusted Signing) avoids
the warning from day one. Either kind of certificate works with the signing
steps above; only the secret contents change.

## Linux: why AppImage, not Flatpak/Flathub

The existing `.AppImage` build (via `appimagetool`) is kept as-is, per the
issue's scope ("AppImage stays as-is"). Flathub distribution is **not**
implemented here:

- Flathub requires a manifest-driven build from source inside a Flatpak
  sandbox (no bundling a self-contained `dotnet publish` output directly),
  plus a review/approval process on their side — this is a separate,
  larger effort than an additive CI step, and the issue explicitly scopes
  Flathub as a stretch goal alongside Homebrew/winget rather than a hard
  CI requirement.
- AppImage requires no external approval and works today, so it remains the
  primary Linux distribution channel until a Flatpak manifest is built as
  follow-up work.

## Package managers (Homebrew cask, winget)

Publishing to Homebrew/winget means opening a PR against **their**
repositories (`homebrew/homebrew-cask`, `microsoft/winget-pkgs`) — this repo
cannot and does not auto-publish there. `packaging/` contains ready-to-fill
templates plus the exact submission steps:

### Homebrew cask — `packaging/homebrew/pkhex-avalonia.rb`

Prerequisite: a release with signed & notarized `.dmg` files (see above) —
Homebrew cask maintainers reject casks whose binaries fail Gatekeeper.

1. `shasum -a 256 PKHeX-Avalonia-osx-arm64.dmg PKHeX-Avalonia-osx-x64.dmg`
   and fill in `version` + both `sha256` values in the template.
2. `brew bump-cask-pr --cask pkhex-avalonia --version <version>` (once the
   cask already exists upstream), or for the first submission, fork
   `homebrew/homebrew-cask`, copy the file to
   `Casks/p/pkhex-avalonia.rb`, and open a PR.
3. `brew audit --cask --online pkhex-avalonia` locally before submitting.

### winget — `packaging/winget/doctorllll.PKHeXAvalonia.*.yaml`

Prerequisite: a release with a signed `PKHeX-Avalonia-Setup.exe` — winget
also flags unsigned installers during validation and Microsoft's manual
review is far more likely to reject them.

1. Replace `{{VERSION}}` in all three files and `{{INSTALLER_SHA256}}` in
   the installer manifest (`Get-FileHash PKHeX-Avalonia-Setup.exe -Algorithm
   SHA256` on Windows, or `sha256sum` elsewhere).
2. Easiest path: `wingetcreate update doctorllll.PKHeXAvalonia -u
   https://github.com/doctorllll/PKHeX/releases/download/v<version>/PKHeX-Avalonia-Setup.exe
   -v <version> -s` (the `-s` submits a PR directly if you're
   authenticated with `gh`).
3. Manual path: `winget validate --manifest packaging/winget/` then copy the
   three files into a fork of `microsoft/winget-pkgs` under
   `manifests/d/doctorllll/PKHeXAvalonia/<version>/` and open a PR.

## Summary: what's automatic vs. gated vs. manual

- **Fully automatic, every release:** macOS zip artifacts, `SHA256SUMS.txt`, and `.dmg`
  (Developer ID, self-signed, or ad-hoc), GitHub Release creation and asset upload.
- **Gated on secrets (automatic once configured):** Developer ID codesigning
  + notarization/stapling for macOS (tier 1), stable self-signed identity
  for macOS (tier 2, the "tertius" pattern — no Apple Developer account
  needed), code signing for the Windows installer and exe.
- **Manual, one-time-per-version, by design (cannot be automated without
  publishing into third-party repos on the maintainer's behalf):**
  submitting the Homebrew cask and winget manifest PRs. Flathub packaging is
  left as future work.
