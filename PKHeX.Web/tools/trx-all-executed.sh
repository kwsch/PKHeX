#!/usr/bin/env bash
# Fails unless every test in a VSTest .trx results file was executed.
#
# The E2E and RealSave tiers are opt-in (PKHEX_WEB_TEST_TIERS): without the opt-in, a run filtered to them skips every
# test and still reports success. CI runs this on the E2E results so that a missing opt-in fails instead of passing
# with nothing tested. A run with no tests at all fails too.
#
# Usage: PKHeX.Web/tools/trx-all-executed.sh <results.trx>
set -euo pipefail

if [ $# -ne 1 ]; then
    echo "Usage: $0 <results.trx>" >&2
    exit 2
fi
if [ ! -f "$1" ]; then
    echo "No test results at $1; did the test run start?" >&2
    exit 1
fi

# VSTest writes exactly one Counters element; anything else fails rather than being misread.
counters="$(grep -o '<Counters [^>]*>' "$1" || true)"
if [ "$(printf '%s' "$counters" | grep -c '<Counters')" -ne 1 ]; then
    echo "Expected one test Counters element in $1." >&2
    exit 1
fi
total="$(printf '%s' "$counters" | sed -n -E 's/.* total="([0-9]+)".*/\1/p')"
executed="$(printf '%s' "$counters" | sed -n -E 's/.* executed="([0-9]+)".*/\1/p')"
if ! [[ "$total" =~ ^[0-9]+$ ]] || ! [[ "$executed" =~ ^[0-9]+$ ]]; then
    echo "Could not read the test counters in $1." >&2
    exit 1
fi
if [ "$total" -eq 0 ]; then
    echo "No tests were recorded in $1; did the test run abort, or does the filter match nothing?" >&2
    exit 1
fi
if [ "$executed" -ne "$total" ]; then
    echo "Only $executed of $total tests in $1 were executed; set PKHEX_WEB_TEST_TIERS for the selected tier." >&2
    exit 1
fi
echo "All $total tests in $1 were executed."
