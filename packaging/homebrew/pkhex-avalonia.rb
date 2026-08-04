# Homebrew Cask template for PKHeX-Avalonia.
#
# This is NOT auto-published. Once a signed & notarized macOS release exists,
# submit it to homebrew/homebrew-cask following docs/packaging.md:
#   1. Fill in `version` and both `sha256` values from the release's .dmg files
#      (`shasum -a 256 PKHeX-Avalonia-osx-arm64.dmg PKHeX-Avalonia-osx-x64.dmg`).
#   2. `brew bump-cask-pr` or open a PR against homebrew/homebrew-cask with
#      this file renamed to Casks/p/pkhex-avalonia.rb.
cask "pkhex-avalonia" do
  arch arm: "arm64", intel: "x64"

  version "1.43.3"
  sha256 arm:   "REPLACE_WITH_OSX_ARM64_DMG_SHA256",
         intel: "REPLACE_WITH_OSX_X64_DMG_SHA256"

  url "https://github.com/doctorllll/PKHeX/releases/download/v#{version}/PKHeX-Avalonia-osx-#{arch}.dmg"
  name "PKHeX-Avalonia"
  desc "Avalonia-based save editor for Pokémon games"
  homepage "https://github.com/doctorllll/PKHeX"

  auto_updates false
  depends_on macos: ">= :big_sur"

  app "PKHeX.Avalonia.app"

  # Strip the quarantine bit Homebrew's own download adds so Gatekeeper
  # doesn't prompt on first launch. Safe regardless of which signing tier
  # produced the .dmg (Developer ID notarized, stable self-signed identity,
  # or unsigned) — see the "tertius pattern" section in docs/packaging.md.
  postflight do
    system_command "/usr/bin/xattr",
                   args: ["-dr", "com.apple.quarantine", "#{appdir}/PKHeX.Avalonia.app"],
                   sudo: false
  end

  zap trash: [
    "~/Library/Application Support/PKHeX.Avalonia",
    "~/Library/Preferences/io.pkhex.avalonia.plist",
  ]
end
