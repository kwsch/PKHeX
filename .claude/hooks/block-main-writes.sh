#!/bin/bash
# PreToolUse hook (Bash): block git commit/push while checked out on the protected branch.
# Project convention is to always work in a branch and land changes via PR.
cmd=$(jq -r '.tool_input.command // empty')
branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null)

if [[ "$branch" == "master" ]] && echo "$cmd" | grep -qE '(^|&&|;|\|)[[:space:]]*git[[:space:]]+(commit|push)\b'; then
  jq -n '{hookSpecificOutput:{hookEventName:"PreToolUse",permissionDecision:"deny",permissionDecisionReason:"Direct commits/pushes to master are blocked — create a feature branch and open a PR instead (project convention: no direct pushes to master)."}}'
else
  echo '{}'
fi
