#!/usr/bin/env bash
# Compares the trim-analysis warnings of a diagnostic Release publish of PKHeX.Web with trim-warnings.baseline.txt.
#
# Blazor suppresses trim-analysis warnings by default, so a normal publish reports none. This script publishes once more
# with SuppressTrimAnalysisWarnings=false and normalises every warning to "ILxxxx: message", dropping the source file,
# line and column and the project suffix, so that line shifts elsewhere in Core do not change the baseline. The ordinals
# in compiler-generated names (lambdas <M>b__16_0, iterators <M>d__69, closures <>c__DisplayClass5_0) are replaced with N,
# because they change whenever a member is added earlier in the same type.
# Duplicates are kept, so a warning that appears once more is still a difference.
#
# Any difference fails, in either direction: a new warning must be justified, and a removed warning is taken out of the
# baseline so that it stays exact (and, while the baseline is not empty, a publish that reported nothing cannot pass).
#
# Usage: PKHeX.Web/tools/trim-warnings.sh [--update] [--report <file>]
#   --update         rewrite the baseline from this publish instead of comparing
#   --report <file>  also write the normalised warnings to <file>
set -euo pipefail

tools_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
project_dir="$(dirname "$tools_dir")"
baseline="$project_dir/trim-warnings.baseline.txt"

update=false
report=""
while [ $# -gt 0 ]; do
    case "$1" in
        --update) update=true ;;
        --report)
            if [ $# -lt 2 ] || [ -z "$2" ]; then
                echo "--report needs a file path." >&2
                exit 2
            fi
            report="$2"
            shift
            ;;
        *) echo "Unknown argument: $1" >&2; exit 2 ;;
    esac
    shift
done

if [ -n "$report" ] && [ ! -d "$(dirname "$report")" ]; then
    echo "The directory for $report does not exist." >&2
    exit 2
fi

work="$(mktemp -d "${TMPDIR:-/tmp}/pkhex-trim.XXXXXX")"
trap 'rm -rf "$work"' EXIT

echo "Publishing with trim-analysis warnings enabled..."
if ! dotnet publish "$project_dir/PKHeX.Web.csproj" -c Release -o "$work/publish" \
    -p:SuppressTrimAnalysisWarnings=false "-flp:warningsonly;NoSummary;logfile=$work/warnings.log" > "$work/publish.log" 2>&1; then
    cat "$work/publish.log" >&2
    echo "Diagnostic publish failed." >&2
    exit 1
fi

# "  1:7>/path/File.cs(1,2): Trim analysis warning IL2070: message [/path/PKHeX.Web.csproj]" -> "IL2070: message"
# The log is CRLF on Windows.
tr -d '\r' < "$work/warnings.log" \
    | sed -n -E 's/^.*warning (IL[0-9]+: .*)$/\1/p' \
    | sed -E 's/ \[[^]]*\.csproj\]$//' \
    | sed -E 's/>(b|d)__[0-9]+(_[0-9]+)?/>\1__N/g; s/<>c__DisplayClass[0-9]+(_[0-9]+)?/<>c__DisplayClassN/g' \
    | LC_ALL=C sort > "$work/actual.txt"

count="$(wc -l < "$work/actual.txt" | tr -d ' ')"
echo "Trim-analysis warnings: $count"
if [ -n "$report" ]; then
    cp "$work/actual.txt" "$report"
fi

# The baseline has no final newline (.editorconfig [*.txt]).
if [ "$update" = true ]; then
    printf '%s' "$(cat "$work/actual.txt")" > "$baseline"
    echo "Updated $baseline."
    exit 0
fi

if [ ! -f "$baseline" ]; then
    echo "Missing $baseline; run with --update to create it." >&2
    exit 1
fi
printf '%s\n' "$(cat "$baseline")" | sed '/^$/d' > "$work/expected.txt"
if ! diff -u --label baseline --label publish "$work/expected.txt" "$work/actual.txt"; then
    echo "Trim-analysis warnings differ from the baseline (- removed, + new)." >&2
    echo "Justify new warnings, then rerun with --update and commit the baseline." >&2
    exit 1
fi
echo "Trim-analysis warnings match the baseline."
