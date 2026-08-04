#!/usr/bin/env bash
# Downloads real Pokemon save files from ReignOfComputer/RoCs-PC (public GitHub repo)
# for integration testing with PKHeX-Avalonia.
#
# Usage: bash Tests/savefiles/download_saves.sh
#
# Source: https://github.com/ReignOfComputer/RoCs-PC

set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

BASE="https://raw.githubusercontent.com/ReignOfComputer/RoCs-PC/master"

download() {
    local url="$1"
    local dest="$2"
    if [ -f "$dest" ]; then
        echo "  [skip] $dest already exists"
        return
    fi
    echo "  [download] $dest"
    curl -sL -o "$dest" "$url"
    local size
    size=$(wc -c < "$dest" | tr -d ' ')
    if [ "$size" -lt 100 ]; then
        echo "  [WARN] $dest is only ${size} bytes - may be a 404"
        rm -f "$dest"
    fi
}

echo "=== Gen 1 ==="
download "$BASE/01%20-%20Gen%20I%20-%20RBY%20Collection/Save%20Data/20160424003402%20-%20Red%2001710/00001710/sav.dat" \
    "gen1_red.sav"

echo "=== Gen 2 ==="
download "$BASE/03%20-%20Gen%20II%20-%20GSC%20Collection/Save%20Data/Pokemon%20Gold.sav" \
    "gen2_gold.sav"
download "$BASE/03%20-%20Gen%20II%20-%20GSC%20Collection/Save%20Data/Pokemon%20Crystal.sav" \
    "gen2_crystal.sav"

echo "=== Gen 3 ==="
download "$BASE/05%20-%20Gen%20III%20-%20RSE%20Collection/Save%20Data/Pokemon%20Ruby.sav" \
    "gen3_ruby.sav"
download "$BASE/06%20-%20Gen%20III%20-%20FRLG%20Collection/Save%20Data/Pokemon%20Fire%20Red.sav" \
    "gen3_firered.sav"

echo "=== Gen 4 ==="
download "$BASE/09%20-%20Gen%20IV%20-%20DPPt%20Collection/Save%20Data/Pokemon%20Diamond.sav" \
    "gen4_diamond.sav"
download "$BASE/09%20-%20Gen%20IV%20-%20DPPt%20Collection/Save%20Data/Pokemon%20Platinum.sav" \
    "gen4_platinum.sav"
download "$BASE/10%20-%20Gen%20IV%20-%20HGSS%20Collection/Save%20Data/Pokemon%20HeartGold.sav" \
    "gen4_heartgold.sav"

echo "=== Gen 5 ==="
download "$BASE/14%20-%20Gen%20V%20-%20BW%20Collection/Save%20Data/Pokemon%20Black.sav" \
    "gen5_black.sav"
download "$BASE/15%20-%20Gen%20V%20-%20B2W2%20Collection/Save%20Data/Pokemon%20White%202.sav" \
    "gen5_white2.sav"

echo "=== Gen 6 ==="
download "$BASE/19%20-%20Gen%20VI%20-%20XY%20Collection/Save%20Data/20180219163039%20-%20X/0000055d/main" \
    "gen6_x.main"
download "$BASE/20%20-%20Gen%20VI%20-%20ORAS%20Collection/Save%20Data/20180219163050%20-%20OR/000011c4/main" \
    "gen6_or.main"

echo "=== Gen 7 ==="
download "$BASE/24%20-%20Gen%20VII%20-%20SM%20Collection/Save%20Data/20180228115950%20-%20Sun/00001648/main" \
    "gen7_sun.main"
download "$BASE/25%20-%20Gen%20VII%20-%20USUM%20Collection/Save%20Data/20180228134249%20-%20Ultra%20Sun/00001b50/main" \
    "gen7_ultrasun.main"

echo "=== Gen 8 ==="
download "$BASE/33%20-%20Gen%20VIII%20-%20BDSP%20Collection/Save%20Data/0100000011D90000%20-%20BD/SaveData.bin" \
    "gen8b_brilliantdiamond.bin"
download "$BASE/35%20-%20Gen%20VIII%20-%20PLA%20Collection/Save%20Data/11%20-%20Final%20-%20main" \
    "gen8a_legendsarceus.main"

echo "=== Gen 9 ==="
download "$BASE/38%20-%20Gen%20IX%20-%20SV%20Collection/Save%20Data/Pokemon%20Scarlet/main" \
    "gen9_scarlet.main"
download "$BASE/38%20-%20Gen%20IX%20-%20SV%20Collection/Save%20Data/Pokemon%20Violet/main" \
    "gen9_violet.main"
download "$BASE/41%20-%20Gen%20IX%20-%20PLZA%20Collection/Save%20Data/main" \
    "gen9a_legendsza.main"

echo ""
echo "=== Summary ==="
echo "Save files in $SCRIPT_DIR:"
ls -lhS *.sav *.main *.bin 2>/dev/null || echo "(none found)"
echo ""
echo "Total: $(find . -maxdepth 1 \( -name '*.sav' -o -name '*.main' -o -name '*.bin' \) | wc -l | tr -d ' ') save files"
