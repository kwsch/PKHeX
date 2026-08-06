# Release manual

How a PKHeX-Avalonia release is cut. macOS and Android ship **from the same tag, on the same
version number** — there is no separate Android release train.

`docs/packaging.md` covers the macOS signing tiers and the package-manager templates in
`packaging/` in detail; this document is the end-to-end procedure and the Android half.

## The one rule

**One tag → every platform.** `v<UIVersion>` triggers `.github/workflows/release.yml`, which builds
the macOS legs and the Android leg in parallel and attaches all of them to a single GitHub Release.
Never tag a platform separately, and never hand-upload an APK to a release the workflow produced —
that breaks the guarantee that a given version number means the same code everywhere.

## Versioning

Two version fields live in `Directory.Build.props` and mean different things:

| Field | Meaning | Bump when |
|---|---|---|
| `<UIVersion>` | This fork's own version. Drives the release tag and every artifact name. | Every PR: `feat` → minor, `fix`/`chore`/`deps`/`refactor` → patch, breaking → major |
| `<Version>` | Tracks upstream PKHeX.Core's version (e.g. `26.07.07`) | Only during a `PKHeX.Core` upstream sync, to match upstream's value |

The release workflow refuses to run if the tag does not match `<UIVersion>` exactly, so the bump
must already be merged to `master` before tagging.

Android's `versionCode` — the integer Android actually compares to decide whether an APK is an
upgrade — is derived from `<UIVersion>` as `major*100000 + minor*1000 + patch`, so it rises on its
own with each release. Nothing to set by hand; just keep minor and patch below 1000.

## Cutting a release

1. **Confirm `master` is green.** `gh run list --branch master --limit 3`. CI runs the macOS
   build + the full test suite; a red master is never releasable.

2. **Confirm the Android leg builds.** CI's `build-android` job compiles it on every PR, so a
   green `master` already covers this — `PKHeX.Android` is not in `PKHeX.sln`, which is why it
   needs its own job rather than riding along with the test job. To check a change you have not
   pushed yet:

   ```bash
   dotnet build PKHeX.Android/PKHeX.Android.csproj -c Release --disable-build-servers -m:1
   ```

   Expect `0 Warning(s) 0 Error(s)`. A build is not a UI check: if you touched anything the
   Android host consumes (`PKHeX.Avalonia` views, `PKHeX.Presentation` view models, DI wiring),
   install it on a device and run the smoke checks in "Post-release verification" *before*
   tagging.

3. **Verify the version.** `grep UIVersion Directory.Build.props` — this is the number about to
   become the tag. If it is wrong, fix it in a PR and merge that first.

4. **Tag and push.**

   ```bash
   git checkout master && git pull --ff-only
   git tag -a v<UIVersion> -m "PKHeX-Avalonia <UIVersion>"
   git push origin v<UIVersion>
   ```

5. **Watch the run.** `gh run watch` — or `gh run list --workflow=Release --limit 1`. The
   `release` job waits on both `build` and `build-android`, so a failed Android leg blocks the
   whole release rather than publishing a macOS-only one.

6. **Verify the release.** `gh release view v<UIVersion>` should list, at minimum:
   `PKHeX-Avalonia-osx-arm64*.zip/.dmg`, `PKHeX-Avalonia-osx-x64*.zip/.dmg`,
   `PKHeX-Android-arm64*.apk`, and `SHA256SUMS.txt`.

`workflow_dispatch` is available for a manual run; it creates the tag itself and skips if that tag
already exists.

## Artifacts and signing tiers

Both platforms degrade gracefully when signing secrets are absent, and the artifact name always
says which tier produced it — an unsuffixed name means properly signed.

| Platform | Artifact | Tiers (best → fallback) |
|---|---|---|
| macOS arm64 / x64 | `PKHeX-Avalonia-osx-<arch>[-suffix].zip` + `.dmg` | Developer ID + notarized (no suffix) → stable self-signed (`-selfsigned`) → ad-hoc (`-adhoc`) |
| Android arm64 | `PKHeX-Android-arm64[-suffix].apk` | Release keystore (no suffix) → SDK debug keystore (`-debugsigned`) |

### Android signing secrets

Set all four to get a distributable APK; set none and the workflow still produces an installable
but clearly-labelled `-debugsigned` build.

| Secret | Contents |
|---|---|
| `ANDROID_KEYSTORE_BASE64` | Base64 of the release `.keystore`/`.jks` (`base64 -i release.keystore \| pbcopy`) |
| `ANDROID_KEYSTORE_PASSWORD` | Keystore password |
| `ANDROID_KEY_ALIAS` | Key alias inside the keystore |
| `ANDROID_KEY_PASSWORD` | Password for that key |

Creating the keystore once:

```bash
keytool -genkeypair -v -keystore release.keystore -alias pkhex \
  -keyalg RSA -keysize 4096 -validity 10000
```

**Keep it and its passwords forever.** Android identifies an app by its signing key: an APK signed
with a different key cannot upgrade an installed one in place — users have to uninstall first,
losing app-private data. Losing the keystore is unrecoverable for the existing install base.

Unlike Apple's Developer ID, an Android signing key costs nothing: it is self-generated and
self-signed, and sideloaded APKs need no third-party enrolment. (A Google Play listing would need a
one-time developer account, but this project distributes through GitHub Releases.)

### Verifying a downloaded APK

Every release APK from `v1.47.0` onward is signed by this key:

```
CN=doctorllll, OU=PKHeX-Avalonia, O=doctorllll, L=Shenzhen, ST=Guangdong, C=CN
SHA-256: a8:c5:c7:09:ea:fc:c3:3a:84:e8:19:0a:c3:58:e9:3d:3a:89:a5:ee:74:5a:07:89:c3:a4:50:9d:77:36:6e:58
```

Check any downloaded APK against it before installing:

```bash
apksigner verify --print-certs PKHeX-Android-arm64.apk
```

A mismatch means the file is not ours. Note that builds produced before the release keystore
existed — anything named `-debugsigned`, and every locally-built APK — carry the Android SDK's
debug key instead, so the **first release-signed build cannot upgrade one of those in place**;
uninstall the old one first. This applies only once, at the switchover.

## Following upstream

Upstream `kwsch/PKHeX` moves independently of this fork. The loop is already automated; releases
hang off it.

1. `.github/workflows/check-upstream-sync.yml` runs daily, compares upstream against
   `.github/upstream-sync/last-synced-sha.txt`, and opens a `sync`-labelled
   **PKHeX.Core Sync Required** issue when upstream has moved.
2. Run the `sync-upstream-core` skill on that issue. It mirrors `PKHeX.Core/` 1:1 (never edit Core
   by hand), fixes only consumer call sites, bumps `<UIVersion>` by a patch and `<Version>` to
   upstream's, writes the new SHA, and ships it as a PR that auto-merges once CI is green.
3. The sync also classifies upstream's non-Core commits and opens `frontend-parity` issues for
   WinForms-side features the Avalonia UI does not have yet. A green build proves nothing broke —
   it does not prove the UI gained anything.
4. **Then decide whether to release.** A Core sync alone is a legitimate release (users get the new
   legality data), but batching a few syncs plus any feature work into one version is usually
   better than a release per sync.

Because Android and macOS share one version, a Core sync automatically reaches Android users at the
next tag — no separate step, provided step 2 of "Cutting a release" passes.

## Post-release verification

macOS:

```bash
# from the release page
hdiutil attach PKHeX-Avalonia-osx-arm64*.dmg
cp -R /Volumes/PKHeX-Avalonia/PKHeX.Avalonia.app /Applications/
open /Applications/PKHeX.Avalonia.app
```

Open a save from `Tests/savefiles/`, confirm the title bar version and that the editor loads.

Android:

```bash
adb install -r PKHeX-Android-arm64*.apk
adb shell am start -n io.github.doctorllll.pkhex.android/crc64dbee79d1eb49a76a.MainActivity
adb logcat -d | grep -E "PKHEX_ANDROID|FATAL"
```

Smoke path (each step has broken at least once, so run all of them):

1. Cold start reaches the editor UI with `PKHaX` shown in the status bar.
2. `File > Open` opens the SAF picker and a real `.main` loads (status bar shows the game).
3. The `Box` tab renders sprites, and long-pressing a slot offers View / Set / Delete.
4. `Tools > Auto Legality Mod` opens, typed text lands in the overlay (not the editor behind it),
   and `Legalize` reports `Legal!`.
5. `File > Save` writes back through SAF — re-open the same file and confirm the change persisted.

## When a release goes wrong

Do **not** move or reuse a tag that has already published a release; people may have downloaded it.
Fix forward:

1. `gh release delete v<UIVersion> --yes` and `git push --delete origin v<UIVersion>` *only* if the
   release is minutes old and demonstrably unused.
2. Otherwise merge the fix with a patch `<UIVersion>` bump and tag the new version.
