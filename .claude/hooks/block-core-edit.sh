#!/bin/bash
# PreToolUse hook (Edit|Write|MultiEdit): PKHeX.Core must stay a 1:1 mirror of
# upstream kwsch/PKHeX. Block edits to it except on a sync branch created by
# the sync-upstream-core skill.
f=$(jq -r '.tool_input.file_path // empty')

case "$f" in
  */PKHeX.Core/*|PKHeX.Core/*)
    branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null)
    case "$branch" in
      chore/sync-pkhex-core-*)
        echo '{}'
        ;;
      *)
        jq -n '{hookSpecificOutput:{hookEventName:"PreToolUse",permissionDecision:"deny",permissionDecisionReason:"PKHeX.Core must stay a byte-for-byte mirror of upstream kwsch/PKHeX — port the fix into PKHeX.Application/Infrastructure/Presentation/Avalonia instead. (Allowed on a chore/sync-pkhex-core-* branch during an upstream sync — see the sync-upstream-core skill.)"}}'
        ;;
    esac
    ;;
  *)
    echo '{}'
    ;;
esac
