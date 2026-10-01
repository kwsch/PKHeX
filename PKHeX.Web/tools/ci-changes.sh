#!/usr/bin/env bash
# Decides which parts of the web workflow a pull request needs, from the paths it changes.
#
# Reads changed paths (one per line, relative to the repository root) on stdin and prints two GitHub output lines:
#   web=true      the published app can differ: publish it, check it and run the E2E tier
#   sprites=true  the publish with sprites can differ: build it and run the tests that need it
# Core tests and the Web Unit tier always run, and pushes to the merge targets run everything, so a path missed here
# is still caught before it ships; the lists only decide what a pull request may leave out.
# An empty list (the diff could not be read) selects everything.
#
# Usage: git diff --name-only <base> <head> | PKHeX.Web/tools/ci-changes.sh
set -euo pipefail

web=false
sprites=false
seen=false
while IFS= read -r path || [ -n "$path" ]; do
    [ -n "$path" ] || continue
    seen=true
    case "$path" in
        # Compiled into the app, published with it, or deciding how it is built and tested.
        # PKHeX.Drawing.PokeSprite is listed because the app compiles its Util/SpriteName.cs.
        PKHeX.Core/* | PKHeX.Web/* | PKHeX.Web.SpriteAtlas/* | PKHeX.Drawing.PokeSprite/* | Tests/PKHeX.Web.Tests/* | \
            Directory.Build.props | LICENSE | .gitattributes | .github/workflows/web.yml)
            web=true
            ;;
    esac
    case "$path" in
        # The generator and its inputs, the app code that loads and draws sprites, the files the atlas is served with,
        # and the tests and test host that check them.
        PKHeX.Core/* | PKHeX.Drawing.PokeSprite/* | PKHeX.Web.SpriteAtlas/* | \
            PKHeX.Web/PKHeX.Web.csproj | PKHeX.Web/Program.cs | PKHeX.Web/THIRD-PARTY-NOTICES.md | PKHeX.Web/Services/Sprites/* | \
            PKHeX.Web/Services/BuildInfo.cs | PKHeX.Web/Components/Slot* | PKHeX.Web/Components/AboutPanel.razor | PKHeX.Web/wwwroot/* | \
            Tests/PKHeX.Web.Tests/Sprite* | Tests/PKHeX.Web.Tests/PublishedApp* | Tests/PKHeX.Web.Tests/StaticHost* | \
            Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj | Directory.Build.props | .github/workflows/web.yml)
            sprites=true
            ;;
    esac
done

if [ "$seen" = false ]; then
    web=true
    sprites=true
fi
echo "web=$web"
echo "sprites=$sprites"
