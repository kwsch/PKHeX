#!/bin/bash
# PreToolUse hook (Bash): block git commit/push while checked out on the protected branch.
# Project convention is to always work in a branch and land changes via PR.
#
# Exception: releases are tag-driven (`git push origin v<UIVersion>` — see docs/releasing.md).
# GitHub's branch protection does not cover tags, so pushing a tag from master is legitimate and
# should not force a throwaway branch. A push is allowed only when *every* refspec it names is a
# v* tag; `git push`, `git push origin master`, and `git push origin v1.0.0 master` stay blocked.
cmd=$(jq -r '.tool_input.command // empty')
branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null)

allow() { echo '{}'; exit 0; }
deny() {
  jq -n --arg reason "$1" '{hookSpecificOutput:{hookEventName:"PreToolUse",permissionDecision:"deny",permissionDecisionReason:$reason}}'
  exit 0
}

[[ "$branch" == "master" ]] || allow

if echo "$cmd" | grep -qE '(^|&&|;|\|)[[:space:]]*git[[:space:]]+commit\b'; then
  deny "Direct commits to master are blocked — create a feature branch and open a PR instead (project convention: no direct pushes to master)."
fi

if echo "$cmd" | grep -qE '(^|&&|;|\|)[[:space:]]*git[[:space:]]+push\b'; then
  # Take the push's own arguments only: stop at the first shell operator so a trailing
  # `2>&1 | tail -3` cannot smuggle a branch name past the check.
  push_args=$(echo "$cmd" | sed -nE 's/.*git[[:space:]]+push[[:space:]]*([^|;&>]*).*/\1/p')
  # Drop flags, then drop the remote (the first remaining token); what is left are the refspecs.
  # A bare number is the leftover file descriptor of a `2>&1` redirection, never a refspec.
  refs=$(echo "$push_args" | tr ' ' '\n' | grep -vE '^(-.*)?$' | tail -n +2 | grep -vE '^[0-9]+$')

  if [ -n "$refs" ] && ! echo "$refs" | grep -qvE '^(refs/tags/)?v[0-9][A-Za-z0-9._-]*$'; then
    allow
  fi

  deny "Direct pushes to master are blocked — create a feature branch and open a PR instead (project convention: no direct pushes to master). Release tags are exempt: 'git push origin v<UIVersion>' is allowed."
fi

allow
