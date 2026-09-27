#!/usr/bin/env bash
# Prints a Markdown size report for a published PKHeX.Web wwwroot: totals, and the largest uncompressed assets with the
# sizes of their precompressed Brotli (.br) and gzip (.gz) siblings. Report only; the deployment limits are enforced by
# the E2E test PublishesOnlyStaticDeployableFiles.
#
# Usage: PKHeX.Web/tools/size-report.sh <wwwroot> [count]
set -euo pipefail

if [ $# -lt 1 ] || [ ! -d "$1" ]; then
    echo "Usage: $0 <wwwroot> [count]" >&2
    exit 2
fi
root="$(cd "$1" && pwd)"
top="${2:-10}"

# Byte count of a file, or empty if it does not exist. wc -c behaves the same on BSD and GNU, unlike stat.
size_of() {
    if [ -f "$1" ]; then
        wc -c < "$1" | tr -d ' '
    fi
}

# Bytes as MiB with two decimals, or "-" when empty.
mib() {
    if [ -z "$1" ]; then
        echo "-"
    else
        awk -v b="$1" 'BEGIN { printf "%.2f MiB", b / 1048576 }'
    fi
}

list="$(mktemp "${TMPDIR:-/tmp}/pkhex-size.XXXXXX")"
trap 'rm -f "$list"' EXIT

# Totals per kind; uncompressed assets are also listed as "size<TAB>relative path" for the table.
files=0
raw_total=0
br_total=0
gz_total=0
while IFS= read -r file; do
    size="$(size_of "$file")"
    files=$((files + 1))
    case "$file" in
        *.br) br_total=$((br_total + size)) ;;
        *.gz) gz_total=$((gz_total + size)) ;;
        *)
            raw_total=$((raw_total + size))
            printf '%s\t%s\n' "$size" "${file#"$root"/}" >> "$list"
            ;;
    esac
done < <(find "$root" -type f | LC_ALL=C sort)

raw_count="$(wc -l < "$list" | tr -d ' ')"

echo "## PKHeX.Web publish size"
echo
echo "| | Files | Size |"
echo "|---|---:|---:|"
echo "| Uncompressed assets | $raw_count | $(mib "$raw_total") |"
echo "| Brotli (.br) | | $(mib "$br_total") |"
echo "| gzip (.gz) | | $(mib "$gz_total") |"
echo "| All files | $files | $(mib $((raw_total + br_total + gz_total))) |"
echo
echo "### Largest $top assets"
echo
echo "| Asset | Raw | Brotli | gzip |"
echo "|---|---:|---:|---:|"
# awk reads all of its input, unlike head, so sort never gets SIGPIPE under pipefail on a large publish.
LC_ALL=C sort -t "$(printf '\t')" -k1,1nr -k2,2 "$list" | awk -v n="$top" 'NR <= n' | while IFS="$(printf '\t')" read -r size path; do
    echo "| \`$path\` | $(mib "$size") | $(mib "$(size_of "$root/$path.br")") | $(mib "$(size_of "$root/$path.gz")") |"
done
